using System;
using Bro.Engine;
using Bro.Network.Udp.Engine;

namespace Bro.Client.Network
{
    public class UdpSocket : BaseSocket, INetEventListener
    {
        private const int UpdateTime = 33;
        private NetManager _socket;
        private NetPeer _serverPeer;

        private readonly Timing.YieldWaitForMilliseconds _yieldWaitForMilliseconds = new Timing.YieldWaitForMilliseconds(UpdateTime);
        public UdpSocket(IClientContext globalContext, IConnectionConfig config) : base(globalContext, config)
        {
        }

        public long Rtt => _serverPeer.Rtt;
        
        public override bool Connect()
        {
            ThreadAssert.AssertMainThread();
            
            State = SocketState.Connecting;
            _socket = new NetManager(this);
            _socket.Start(0);
            _serverPeer = _socket.Connect(ConnectionConfig.Host, ConnectionConfig.Port);
            StartReceiveLoop();
            return _serverPeer != null;
        }

        public override void Disconnect(int code)
        {
            ThreadAssert.AssertMainThread();
            
            if (State == SocketState.Disconnected)
            {
                return;
            }

            State = SocketState.Disconnecting;

            if (_socket != null)
            {
                try
                {
                    _socket.Stop();
                }
                catch (Exception ex)
                {
                    Bro.Log.Error("udp socket :: exception on disconnect : " + ex);
                }

                _socket = null;
                _serverPeer = null;
            }

            State = SocketState.Disconnected;
            OnDisconnect?.Invoke(code);
        }

        public override void Send(byte[] data, bool isReliable, bool isOrdered)
        {
            ThreadAssert.AssertMainThread();
            
            if (_serverPeer != null)
            {
                _serverPeer.Send(data, NetUtils.GetSendOptions(isReliable, isOrdered));
            }
            else
            {
                Bro.Log.Error("udp socket :: server peer == null");
            }
        }

      

        protected override System.Collections.IEnumerator CoroutineReceiveLoop()
        {
            while ((State != SocketState.Disconnected || State != SocketState.Connecting) && _socket != null)
            {
                ThreadAssert.AssertMainThread();
                _socket.PollEvents();
                _yieldWaitForMilliseconds.Reset();
                yield return _yieldWaitForMilliseconds;
            }
        }

        void INetEventListener.OnPeerConnected(NetPeer peer)
        {
            ThreadAssert.AssertMainThread();
            _serverPeer = peer;
            State = SocketState.Connected;
            OnConnect?.Invoke();
        }

        void INetEventListener.OnPeerDisconnected(NetPeer peer, int code)
        {
            ThreadAssert.AssertMainThread();
            Disconnect( code ); 
        }

        void INetEventListener.OnNetworkReceive( NetPeer peer, byte[] data )
        {
            ThreadAssert.AssertMainThread();
            if (data != null && data.Length > 0)
            {
                if (OnDataReceivedBinary != null)
                {
                    OnDataReceivedBinary.Invoke(data);
                }
            }
        }

        void INetEventListener.OnConnectionRequest(ConnectionRequest request) {  }
    }
}