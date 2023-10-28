using System.Threading;
using Bro.Network;
using Bro.Network.Udp.Engine;
using Bro.Threading;

namespace Bro.Server.Network.Udp
{
    public class NetworkEngine : INetEventListener
    {
        private const int UpdateTime = 33;
        
        private NetManager _server;
        private NetworkEngineConfig _config;
        private BroThread _thread;
        private bool _isRunning;
        
        private readonly INetworkPeerFactory _peerFactory;

        public NetworkEngine(INetworkPeerFactory peerFactory)
        {
            _peerFactory = peerFactory;
        }

        public void Start(NetworkEngineConfig config)
        {
            _config = config;
            _server = new NetManager(this);
            _server.Start(config.Port);

            _isRunning = true;
            _thread = new BroThread(EventCycle);
            _thread.Start();
            
            Bro.Log.Info("udp engine :: started at port = " + config.Port + "; max connections = " + config.MaxConnections + "; bottleneck = " + config.BottleneckConnections + "; threads = " + config.Threads);
        }
        
        private void EventCycle()
        {
            while (_isRunning)
            {
                var point = PerformanceMeter.Register(PerformancePointType.UdpEngineEvents);
                _server.PollEvents();
                point?.Done();
                
                Thread.Sleep(UpdateTime);
            }
        }

        public void Stop()
        {
            if (_thread != null)
            {
                _isRunning = false;
                _thread.Join();
                _thread = null;
            }
            
            _server.Stop();
        }

        void INetEventListener.OnPeerConnected(NetPeer udpPeer)
        {
            Bro.Log.Info("udp engine :: peer connected, ip = " + udpPeer.EndPoint.Address);
            var peerPair = _peerFactory.CreatePeer(ConnectionProtocol.Udp, udpPeer);
            peerPair.NetworkPeer.OnConnect();
        }
        
        void INetEventListener.OnPeerDisconnected(NetPeer peer, int code)
        {
            Bro.Log.Info("udp engine :: peer disconnected " + code);
            if (peer.NetworkPeer is INetworkPeer netPeer)
            {
                netPeer.Disconnect(code);
            }
        }

        void INetEventListener.OnNetworkReceive(NetPeer peer, byte[] data)
        {
            if (data != null && data.Length > 0 && peer.NetworkPeer != null)
            {
                if (peer.NetworkPeer is INetworkPeer clientPeer)
                {
                    clientPeer.OnReceive(data);
                }
            }
        }

        void INetEventListener.OnConnectionRequest(ConnectionRequest request)
        {
            if (_config != null && _server.GetPeersCount(ConnectionState.Any) < _config.MaxConnections )
            {
                request.Accept();
            }
            else
            {
                request.Reject();
            }
        }
    }
}