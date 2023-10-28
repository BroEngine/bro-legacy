using System;
using System.Collections;

namespace Bro.Client.Network
{
    public class TcpSocket : BaseSocket
    {
        // private readonly IConnectionConfig _config;
        // private readonly string _connectingAddress;
        // private WebSocket _webSocket;

        public TcpSocket(IClientContext globalContext, IConnectionConfig config) : base(globalContext, config)
        {
            throw new NotImplementedException();
        }

        public override bool Connect()
        {
            throw new NotImplementedException();
        }

        public override void Disconnect(int code)
        {
            throw new NotImplementedException();
        }

        public override void Send(byte[] data, bool isReliable, bool isOrdered)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerator CoroutineReceiveLoop()
        {
            throw new NotImplementedException();
        }
    }
}