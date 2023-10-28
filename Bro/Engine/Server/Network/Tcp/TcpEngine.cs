
namespace Bro.Server.Network.Tcp
{
    public class NetworkEngine
    {
        private readonly INetworkPeerFactory _peerFactory;

        public NetworkEngine(INetworkPeerFactory peerFactory)
        {
            _peerFactory = peerFactory;
        }

        public void Start(NetworkEngineConfig config)
        {
        }

        public void Stop()
        {
        }
        
    }
}