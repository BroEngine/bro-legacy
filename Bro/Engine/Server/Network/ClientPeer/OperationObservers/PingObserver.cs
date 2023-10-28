using Bro.Network;
using Bro.Server.Network;

namespace Bro.Server.Observers
{
    public class PingObserver
    {
        public void OnReceive(PingOperation operation, IClientPeer peer)
        {
            var response = new PingOperation();
            peer.Send(response);
        }
    }
}