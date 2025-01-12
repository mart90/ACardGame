using ACardGameLibrary;
using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using System.Text;
using System.Threading;

namespace ACardGame
{
    public class ServerConnection
    {
        private readonly NetManager _client;

        public ServerResponse IncomingResponse { get; set; }
        public ServerMessage IncomingMessage { get; set; }

        public User AuthenticatedUser { get; set; }

        public ServerConnection()
        {
            EventBasedNetListener listener = new();

            _client = new(listener);

            _client.Start();
            _client.Connect("85.146.96.60", 41124, "iYMVZT6XYMKNvu5nj7DEHOztsqBdX9kX");
            //_client.Connect("localhost", 41124, "iYMVZT6XYMKNvu5nj7DEHOztsqBdX9kX");

            listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                var str = dataReader.GetString();
                bool isResponse = str[0] == 'r';
                str = str[1..];

                if (isResponse)
                {
                    IncomingResponse = JsonConvert.DeserializeObject<ServerResponse>(str);
                }
                else
                {
                    IncomingMessage = JsonConvert.DeserializeObject<ServerMessage>(str);
                }
                
                dataReader.Recycle();
            };
        }

        public ServerResponse Login(string username, string password)
        {
            AuthenticateMessage message = new()
            {
                Username = username,
                PasswordHash = HashPassword(password),
                GameVersion = Main.GameVersion
            };

            var response = SendMessageAwaitResponse(ServerEndpoint.Login, message);

            if (response.StatusCode == StatusCode.Ok)
            {
                var authenticatedUser = JsonConvert.DeserializeObject<UserAuthenticatedResponse>(response.Data);
                AuthenticatedUser = new User
                {
                    Id = authenticatedUser.UserId,
                    Name = authenticatedUser.UserName,
                    Rating = authenticatedUser.Rating
                };
            }

            return response;
        }

        public ServerResponse Register(string username, string password)
        {
            AuthenticateMessage message = new()
            {
                Username = username,
                PasswordHash = HashPassword(password),
                GameVersion = Main.GameVersion
            };

            var response = SendMessageAwaitResponse(ServerEndpoint.Register, message);

            if (response.StatusCode == StatusCode.Ok)
            {
                var authenticatedUser = JsonConvert.DeserializeObject<UserAuthenticatedResponse>(response.Data);
                AuthenticatedUser = new User
                {
                    Id = authenticatedUser.UserId,
                    Name = authenticatedUser.UserName,
                    Rating = AuthenticatedUser.Rating
                };
            }

            return response;
        }

        public ServerResponse CreateChallenge()
        {
            return SendMessageAwaitResponse(ServerEndpoint.CreateChallenge, null);
        }

        public ServerResponse CancelChallenge()
        {
            return SendMessageAwaitResponse(ServerEndpoint.CancelChallenge, null);
        }

        public ServerResponse JoinFromClipboard(string challengeId)
        {
            return SendMessageAwaitResponse(ServerEndpoint.JoinFromClipboard, challengeId);
        }

        public ServerResponse JoinNearestOpen()
        {
            return SendMessageAwaitResponse(ServerEndpoint.JoinNearestOpen, null);
        }

        public ServerResponse SendMakeMove(GameMove message)
        {
            return SendMessageAwaitResponse(ServerEndpoint.MakeMove, message);
        }

        public ServerResponse SendResult()
        {
            return SendMessageAwaitResponse(ServerEndpoint.SetResult, null);
        }

        private ServerResponse SendMessageAwaitResponse(ServerEndpoint endpoint, object obj)
        {
            IncomingResponse = null;

            Request message = new(endpoint, obj);

            NetDataWriter writer = new();
            writer.Put(message.ToString());

            _client.SendToAll(writer, DeliveryMethod.ReliableOrdered);

            while (IncomingResponse == null)
            {
                Thread.Sleep(15);
                PollEvents();
            }

            return IncomingResponse;
        }

        public void PollEvents()
        {
            _client.PollEvents();
        }

        private void SendMessage(ServerEndpoint endpoint, object obj)
        {
            Request message = new(endpoint, obj);

            NetDataWriter writer = new();
            writer.Put(message.ToString());

            _client.SendToAll(writer, DeliveryMethod.ReliableOrdered);
        }

        private static string HashPassword(string password)
        {
            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
