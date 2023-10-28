using System;
using System.Threading;
using Bro.Network;
using Bro.Server.Network;

namespace Bro.Server
{
    public class ClientPeerFactory : INetworkPeerFactory
    {
        private readonly Context.IServerContext[] _startContexts;
        private int _contextIndex;

        public ClientPeerFactory(Context.IServerContext[] startContexts)
        {
            _startContexts = startContexts;
        }

        public PeerPair CreatePeer(ConnectionProtocol protocol, ITransportPeer transportPeer)
        {
            INetworkPeer networkPeer;
            IClientPeer clientPeer;

            if (_startContexts.Length == 0)
            {
                Bro.Log.Error("client peer factory :: no start context");
                throw new Exception("client peer factory :: no start context");
            }
            
            var startContext = _startContexts[Interlocked.Increment(ref _contextIndex) % _startContexts.Length];

            switch (protocol)
            {
                case ConnectionProtocol.Udp:
                    networkPeer = new Network.Udp.UdpPeer(transportPeer);
                    clientPeer = new ClientPeer(startContext, networkPeer);
                    break;
                case ConnectionProtocol.Tcp:
                    networkPeer = new Network.Tcp.TcpPeer(transportPeer);
                    clientPeer = new ClientPeer(startContext, networkPeer);
                    break;
                case ConnectionProtocol.Offline:
                    networkPeer = new Network.Offline.OfflineNetworkPeer(transportPeer);
                    clientPeer = new OfflineClientPeer(startContext, networkPeer);
                    break;
                default:
                    throw new System.NotSupportedException();
            }

            transportPeer.NetworkPeer = networkPeer;

            return new PeerPair(networkPeer, clientPeer);
        }
    }
}