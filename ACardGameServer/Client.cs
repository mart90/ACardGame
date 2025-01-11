using ACardGameLibrary;
using LiteNetLib;

namespace ACardGameServer
{
    public class Client
    {
        public NetPeer NetPeer { get; set; }

        public User? AuthenticatedUser { get; set; }

        public string? ChallengeGuid { get; set; }

        public Game? InGame { get; set; }

        public Client(NetPeer netPeer)
        {
            NetPeer = netPeer;
        }
    }
}
