using ACardGameLibrary;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ACardGameServer
{
    public class ServerMain
    {
        private readonly List<Client> _clients;
        private readonly DataContext _dataContext;

        public static IConfiguration Config { get; set; }

        public ServerMain()
        {
            _clients = new List<Client>();
            _dataContext = new DataContext();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false);

            Config = builder.Build();
        }

        public void Start()
        {
            EventBasedNetListener listener = new();
            NetManager server = new(listener);

            server.Start(41124);

            listener.ConnectionRequestEvent += request =>
            {
                if (server.ConnectedPeersCount < 100)
                {
                    request.AcceptIfKey("iYMVZT6XYMKNvu5nj7DEHOztsqBdX9kX");
                }
                else
                {
                    request.Reject();
                }
            };

            listener.PeerConnectedEvent += peer =>
            {
                _clients.Add(new Client(peer));
            };

            listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
            {
                _clients.RemoveAll(e => e.NetPeer == peer);
            };

            listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                string str = null;
                Request? serverMessage = null;

                try
                {
                    str = dataReader.GetString();
                    serverMessage = JsonConvert.DeserializeObject<Request>(str);
                }
                catch (Exception e)
                {
                    Logger.LogError($"Couldn't parse request:\n{str}\nException:\n{e}");
                }

                Receive(serverMessage, fromPeer);

                dataReader.Recycle();
            };

            while (true)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }
        }

        public void Receive(Request message, NetPeer fromPeer)
        {
            var client = _clients.Where(e => e.NetPeer == fromPeer).FirstOrDefault();

            try
            {
                if (client == null)
                {
                    Logger.LogDebug($"Received message from unregistered client: {fromPeer.Address}");
                }

                if (client.AuthenticatedUser == null)
                {
                    var authenticateMessage = message.DeserializeJson<AuthenticateMessage>();

                    var serverVersion = _dataContext.LatestVersion.Single().VersionString;
                    if (authenticateMessage.GameVersion != serverVersion)
                    {
                        SendResponse(client, StatusCode.Error, "Version mismatch");
                        return;
                    }

                    if (message.Endpoint == ServerEndpoint.Login)
                    {
                        Login(client, authenticateMessage);
                    }
                    else if (message.Endpoint == ServerEndpoint.Register)
                    {
                        RegisterUser(client, authenticateMessage);
                    }

                    return;
                }

                switch (message.Endpoint)
                {
                    case ServerEndpoint.CreateChallenge: CreateChallenge(client); break;
                    case ServerEndpoint.CancelChallenge: CancelChallenge(client); break;
                    case ServerEndpoint.JoinFromClipboard: JoinChallengeById(client, message.DeserializeJson<string>()); break;
                    case ServerEndpoint.JoinNearestOpen: JoinNearestOpenChallenge(client); break;

                    case ServerEndpoint.MakeMove: MakeMove(client, message.DeserializeJson<GameMove>()); break;
                    case ServerEndpoint.SetResult: SetResult(client); break;

                    default: SendResponse(client, StatusCode.Error, "Unknown endpoint"); break;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Error handling request from client logged in as {client.AuthenticatedUser?.Name}. Request:\n{message}\nException:{e}");
                SendResponse(client, StatusCode.Error, "Server error");
            }
        }

        public void CreateChallenge(Client client)
        {
            client.ChallengeGuid = Guid.NewGuid().ToString();
            SendResponse(client, StatusCode.Ok, client.ChallengeGuid);
        }

        public void CancelChallenge(Client client)
        {
            client.ChallengeGuid = null;
            SendResponse(client, StatusCode.Ok);
        }

        public void SetResult(Client client)
        {
            client.InGame.WinnerId = client.AuthenticatedUser.Id;

            var opponent = GetOpponent(client);

            double p1rating = client.AuthenticatedUser.Rating;
            double p2rating = opponent.AuthenticatedUser.Rating;

            UpdateEloRatings(ref p1rating, ref p2rating, client.AuthenticatedUser.KFactor, opponent.AuthenticatedUser.KFactor, 1);

            // TODO rating change in game_player

            client.AuthenticatedUser.Rating = p1rating;
            opponent.AuthenticatedUser.Rating = p2rating;

            if (client.AuthenticatedUser.KFactor > 30)
            {
                client.AuthenticatedUser.KFactor -= 5;
            }
            if (opponent.AuthenticatedUser.KFactor > 30)
            {
                opponent.AuthenticatedUser.KFactor -= 5;
            }

            _dataContext.SaveChanges();

            SendResponse(client, StatusCode.Ok);
        }

        public void JoinChallengeById(Client client, string challengeId)
        {
            var opponent = _clients.SingleOrDefault(e => e.ChallengeGuid == challengeId);

            if (opponent == null)
            {
                SendResponse(client, StatusCode.Error, "Challenge not found");
                return;
            }
            else if (opponent == client)
            {
                SendResponse(client, StatusCode.Error, "Can't join your own challenge");
                return;
            }

            opponent.ChallengeGuid = null;

            CreateGame(client, opponent);
        }

        public void JoinNearestOpenChallenge(Client client)
        {
            var openChallenges = _clients.Where(e => e.ChallengeGuid != null && e != client);

            if (!openChallenges.Any())
            {
                SendResponse(client, StatusCode.Error, "No open challenges");
                return;
            }

            client.ChallengeGuid = null;

            var opponent = openChallenges
                .OrderBy(e => Math.Abs(e.AuthenticatedUser.Rating - client.AuthenticatedUser.Rating))
                .First();

            CreateGame(client, opponent);
        }

        private void CreateGame(Client client, Client opponent)
        {
            int seed = new Random().Next();

            var game = new Game
            {
                PlayedOnVersion = _dataContext.LatestVersion.Single().VersionString,
                ShuffleSeed = seed
            };
            game.Players.AddRange(new List<GamePlayer>
            {
                new() { UserId = client.AuthenticatedUser.Id },
                new() { UserId = opponent.AuthenticatedUser.Id }
            });

            _dataContext.Games.Add(game);
            _dataContext.SaveChanges();

            SendResponse(client, StatusCode.Ok);

            SendMessage(client, ServerMessageType.GameStart, new GameStartMessage
            {
                GameId = game.Id,
                OpponentName = opponent.AuthenticatedUser.Name,
                OpponentRating = opponent.AuthenticatedUser.Rating,
                ShuffleSeed = seed,
                IPlayFirst = seed % 2 == 0
            });

            SendMessage(opponent, ServerMessageType.GameStart, new GameStartMessage
            {
                GameId = game.Id,
                OpponentName = client.AuthenticatedUser.Name,
                OpponentRating = client.AuthenticatedUser.Rating,
                ShuffleSeed = seed,
                IPlayFirst = seed % 2 != 0
            });

            client.InGame = game;
            opponent.InGame = game;
        }

        public void RegisterUser(Client client, AuthenticateMessage registerMessage)
        {
            var user = new User
            {
                Name = registerMessage.Username,
                PasswordHash = registerMessage.PasswordHash
            };

            _dataContext.Add(user);
            try
            {
                _dataContext.SaveChanges();

                SendResponse(client, StatusCode.Ok, new UserAuthenticatedResponse
                {
                    UserId = user.Id,
                    UserName = user.Name,
                    Rating = user.Rating
                });

                client.AuthenticatedUser = user;
            }
            catch (Exception)
            {
                _dataContext.Remove(user);
                SendResponse(client, StatusCode.Error, "Error creating user. Username might be taken");
            }
        }

        public void Login(Client client, AuthenticateMessage loginMessage)
        {
            client.AuthenticatedUser = _dataContext.Users
                .Where(e => e.Name == loginMessage.Username && e.PasswordHash == loginMessage.PasswordHash)
                .SingleOrDefault();

            if (client.AuthenticatedUser != null)
            {
                SendResponse(client, StatusCode.Ok, new UserAuthenticatedResponse
                {
                    UserId = client.AuthenticatedUser.Id,
                    UserName = client.AuthenticatedUser.Name,
                    Rating = client.AuthenticatedUser.Rating
                });
            }
            else
            {
                SendResponse(client, StatusCode.BadRequest, "Invalid credentials");
            }
        }

        public void MakeMove(Client client, GameMove makeMoveMessage)
        {
            var opponent = GetOpponent(client);

            if (opponent == null)
            {
                SendResponse(client, StatusCode.Error, "Opponent disconnected");
                return;
            }

            SendMessage(opponent, ServerMessageType.MakeMove, makeMoveMessage);

            SendResponse(client, StatusCode.Ok);

            if (makeMoveMessage.Type == MoveType.BuyingFromShop || makeMoveMessage.Type == MoveType.FreeTradeBuying)
            {
                var cardBuy = new CardBuy
                {
                    GameId = client.InGame.Id,
                    UserId = client.AuthenticatedUser.Id,
                    CardName = makeMoveMessage.CardName,
                    TurnNumber = makeMoveMessage.TurnNumber
                };

                _dataContext.Add(cardBuy);
                _dataContext.SaveChanges();
            }
        }

        public static void SendResponse(Client client, StatusCode statusCode, object data = null)
        {
            ServerResponse response = new()
            {
                StatusCode = statusCode,
                Data = data == null ? null : JsonConvert.SerializeObject(data)
            };

            NetDataWriter writer = new();
            writer.Put("r" + response.ToJsonString());

            client.NetPeer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendMessage(Client client, ServerMessageType messageType, object data)
        {
            ServerMessage message = new()
            {
                MessageType = messageType,
                Data = JsonConvert.SerializeObject(data)
            };

            NetDataWriter writer = new();
            writer.Put("m" + JsonConvert.SerializeObject(message));

            client.NetPeer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        private Client GetOpponent(Client client)
        {
            return _clients.Where(e => e.InGame?.Id == client.InGame.Id && e != client).SingleOrDefault();
        }

        public static void UpdateEloRatings(ref double player1Rating, ref double player2Rating, double player1KFactor, double player2KFactor, double result)
        {
            double expectedScorePlayer1 = 1.0 / (1.0 + Math.Pow(10, (player2Rating - player1Rating) / 400));
            double expectedScorePlayer2 = 1.0 / (1.0 + Math.Pow(10, (player1Rating - player2Rating) / 400));

            player1Rating += player1KFactor * (result - expectedScorePlayer1);
            player2Rating += player2KFactor * ((1 - result) - expectedScorePlayer2);
        }
    }
}
