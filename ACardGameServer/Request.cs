using Newtonsoft.Json;

namespace ACardGameServer
{
    public class Request
    {
        public ServerEndpoint Endpoint { get; set; }

        public string Data { get; set; }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public Request() { }

        public Request(ServerEndpoint endpoint, object obj)
        {
            Endpoint = endpoint;
            Data = JsonConvert.SerializeObject(obj);
        }

        public T DeserializeJson<T>()
        {
            return JsonConvert.DeserializeObject<T>(Data);
        }
    }

    public class LoginMessage
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }

    public class RegisterMessage
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
