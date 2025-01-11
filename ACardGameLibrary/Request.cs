using Newtonsoft.Json;

namespace ACardGameLibrary
{
    public class Request
    {
        public ServerEndpoint Endpoint { get; set; }

        public string Data { get; set; }

        public override string ToString()
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

    public class AuthenticateMessage
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string GameVersion { get; set; }
    }
}
