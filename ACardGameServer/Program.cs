using LiteNetLib;
using LiteNetLib.Utils;

namespace ACardGameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            NetManager server = new(listener);
            server.Start(9050);

            listener.ConnectionRequestEvent += request =>
            {
                if (server.ConnectedPeersCount < 10)
                    request.AcceptIfKey("SomeConnectionKey");
                else
                    request.Reject();
            };

            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got connection: {0}", peer);
                NetDataWriter writer = new();
                writer.Put("Hello client!");
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
            };

            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }
            server.Stop();
        }
    }
}
