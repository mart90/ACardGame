using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;

namespace ACardGameServer
{
    public class ServerMain
    {
        private readonly List<Client> _clients;
        private readonly DataContext _dataContext;

        public ServerMain()
        {
            _clients = new List<Client>();
            _dataContext = new DataContext();
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
                Console.WriteLine($"Connection from {peer}");
                _clients.Add(new Client(peer));
            };

            listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
            {
                _clients.RemoveAll(e => e.NetPeer == peer);
            };

            listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                var str = dataReader.GetString();
                var serverMessage = JsonConvert.DeserializeObject<Request>(str);

                Receive(serverMessage, fromPeer);

                dataReader.Recycle();
            };

            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }

            server.Stop();
        }

        public void Receive(Request message, NetPeer fromPeer)
        {
            var client = _clients.Where(e => e.NetPeer == fromPeer).FirstOrDefault();

            if (client == null)
            {
                Console.WriteLine($"Received message from unregistered client: {fromPeer.Address}");
            }

            if (client.AuthenticatedUser == null)
            {
                if (message.Endpoint == ServerEndpoint.Login)
                {
                    Login(client, message.DeserializeJson<LoginMessage>());
                }
                else if (message.Endpoint == ServerEndpoint.Register)
                {
                    RegisterUser(client, message.DeserializeJson<RegisterMessage>());
                }

                return;
            }

            switch (message.Endpoint)
            {
                case ServerEndpoint.CreateChallenge: CreateChallenge(client); break;
                case ServerEndpoint.CancelChallenge: CancelChallenge(client); break;
                case ServerEndpoint.JoinFromClipboard: JoinChallengeById(client, message.DeserializeJson<string>()); break;

                case ServerEndpoint.MakeMove: MakeMove(client, message.DeserializeJson<GameMove>()); break;
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

        public void JoinChallengeById(Client client, string challengeId)
        {
            var opponent = _clients.SingleOrDefault(e => e.ChallengeGuid == challengeId);

            if (opponent == null)
            {
                SendResponse(client, StatusCode.Error, "Challenge not found");
                return;
            }

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
                ShuffleSeed = seed,
                IPlayFirst = seed % 2 == 0
            });

            SendMessage(opponent, ServerMessageType.GameStart, new GameStartMessage
            {
                GameId = game.Id,
                OpponentName = client.AuthenticatedUser.Name,
                ShuffleSeed = seed,
                IPlayFirst = seed % 2 != 0
            });

            client.InGame = game;
            opponent.InGame = game;
        }

        public void RegisterUser(Client client, RegisterMessage registerMessage)
        {
            client.AuthenticatedUser = new User
            {
                Name = registerMessage.Username,
                PasswordHash = registerMessage.PasswordHash
            };

            _dataContext.Add(client.AuthenticatedUser);

            try
            {
                _dataContext.SaveChanges();

                SendResponse(client, StatusCode.Ok, new UserAuthenticatedResponse
                {
                    UserId = client.AuthenticatedUser.Id,
                    UserName = client.AuthenticatedUser.Name
                });
            }
            catch (Exception e)
            {
                _dataContext.Remove(client.AuthenticatedUser);
                SendResponse(client, StatusCode.Error, "Error creating user. Username might be taken");
            }
        }

        public void Login(Client client, LoginMessage loginMessage)
        {
            client.AuthenticatedUser = _dataContext.Users
                .Where(e => e.Name == loginMessage.Username && e.PasswordHash == loginMessage.PasswordHash)
                .SingleOrDefault();

            if (client.AuthenticatedUser != null)
            {
                SendResponse(client, StatusCode.Ok, new UserAuthenticatedResponse
                {
                    UserId = client.AuthenticatedUser.Id,
                    UserName = client.AuthenticatedUser.Name
                });
            }
            else
            {
                SendResponse(client, StatusCode.BadRequest, "Invalid credentials");
            }
        }

        public void MakeMove(Client client, GameMove makeMoveMessage)
        {
            var opponent = _clients.Where(e => e.InGame?.Id == makeMoveMessage.GameId && e != client).SingleOrDefault();

            if (opponent == null)
            {
                SendResponse(client, StatusCode.Error, "Opponent disconnected");
                return;
            }

            SendMessage(opponent, ServerMessageType.MakeMove, makeMoveMessage);

            SendResponse(client, StatusCode.Ok);

            makeMoveMessage.ToDatabaseFriendly();
            makeMoveMessage.UserId = client.AuthenticatedUser.Id;
            _dataContext.Add(makeMoveMessage);
            _dataContext.SaveChanges();
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
    }
}
