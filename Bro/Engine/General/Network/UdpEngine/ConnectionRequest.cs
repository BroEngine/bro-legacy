// ----------------------------------------------------------------------------
// The framework that was used:
// The MIT License
// https://github.com/RevenantX/LiteNetLib
// ----------------------------------------------------------------------------

using System.Net;
using System.Threading;

namespace Bro.Network.Udp.Engine
{
    public enum ConnectionRequestType
    {
        Incoming,
        PeerToPeer
    }

    internal enum ConnectionRequestResult
    {
        Accept,
        Reject,
        RejectForce
    }

    public class ConnectionRequest
    {
        private readonly NetManager _listener;
        private int _used;
        private ConnectionRequestType _type;
        
        internal ConnectionRequestResult Result { get; private set; }
        internal long ConnectionTime;
        internal byte ConnectionNumber;
        public readonly IPEndPoint RemoteEndPoint;

        private bool TryActivate()
        {
            return Interlocked.CompareExchange(ref _used, 1, 0) == 0;
        }

        internal void UpdateRequest(NetConnectRequestPacket connRequest)
        {
            if (connRequest.ConnectionTime >= ConnectionTime)
            {
                ConnectionTime = connRequest.ConnectionTime;
                ConnectionNumber = connRequest.ConnectionNumber;
            }
        }

        internal ConnectionRequest( long connectionId, byte connectionNumber, ConnectionRequestType type, IPEndPoint endPoint, NetManager listener)
        {
            ConnectionTime = connectionId;
            ConnectionNumber = connectionNumber;
            _type = type;
            RemoteEndPoint = endPoint;
            _listener = listener;
        }

        public NetPeer Accept()
        {
            if (!TryActivate())
            {
                return null;
            }

            Result = ConnectionRequestResult.Accept;
            return _listener.OnConnectionSolved(this );
        }
        
        public void Reject()
        {
            if (!TryActivate())
            {
                return;
            }

            Result = ConnectionRequestResult.Reject;
            _listener.OnConnectionSolved(this);
        }
    }
}