using Newtonsoft.Json;

namespace ACardGameServer
{
    public class ServerMessage
    {
        public ServerMessageType MessageType { get; set; }
        public string Data { get; set; }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class GameStartMessage
    {
        public int GameId { get; set; }
        public string OpponentName { get; set; }
        public int ShuffleSeed { get; set; }
        public bool IPlayFirst { get; set; }
    }
}
