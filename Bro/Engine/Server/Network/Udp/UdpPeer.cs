using System;
using System.Threading;
using Bro.Network;
using Bro.Network.Udp.Engine;

namespace Bro.Server.Network.Udp
{
    public class UdpPeer : INetworkPeer
    {
        private NetPeer _udpPeer;
        private readonly int _id;
        private static int _idCounter;

        public Action<int> OnDisconnectHandler { private get; set; }
        public Action OnConnectHandler { private get; set; }
        public Action<byte[]> OnReceiveBinaryHandler { private get; set; }

        private readonly object _lock = new object();
        
        public UdpPeer(ITransportPeer transportPeer)
        {
            _id = Interlocked.Increment(ref _idCounter);
            _udpPeer = transportPeer as NetPeer;
            
            Bro.Log.Info("udp peer :: create, udp id = " + _id);
        }
        

        int INetworkPeer.Id => _id;

        public void OnConnect()
        {
            OnConnectHandler();
        }

        public void Disconnect( int code )
        {   
            Bro.Log.Info("udp peer :: disconnect called, code = " + code + ", udp id = " + _id);
            
            lock (_lock)
            {         
                OnDisconnectHandler.Invoke( code );
            
                if (_udpPeer != null)
                {
                    _udpPeer?.Disconnect(code);
                    _udpPeer.NetworkPeer = null;
                    _udpPeer = null;
                }
            }
        }

        public void OnReceive(byte[] data)
        {
            OnReceiveBinaryHandler(data);
        }

        public void Send(byte[] data, bool isReliable, bool isOrdered)
        {
            var point = PerformanceMeter.Register(PerformancePointType.NetworkPeerSendData);

            _udpPeer?.Send(data, NetUtils.GetSendOptions(isReliable, isOrdered));

            point?.Done();
        }
    }
}
