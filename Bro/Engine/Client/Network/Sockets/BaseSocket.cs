using System;
using System.Collections;

namespace Bro.Client.Network
{
    public abstract class BaseSocket
    {
        public SocketState State { get; protected set; }

        protected readonly IConnectionConfig ConnectionConfig;
        
        private readonly IClientContext _globalContext;
        private IDisposable _receivingDataLoop;
        
        protected BaseSocket(IClientContext globalContext, IConnectionConfig config)
        {
            _globalContext = globalContext;
            ConnectionConfig = config;
        }

        public abstract bool Connect();

        public abstract void Disconnect(int code);

        public abstract void Send(byte[] data, bool isReliable, bool isOrdered);

        public System.Action<byte[]> OnDataReceivedBinary;

        public System.Action<int> OnDisconnect;
        public System.Action OnConnect;

        protected abstract IEnumerator CoroutineReceiveLoop();

        protected void StartReceiveLoop()
        {
            if (_receivingDataLoop == null)
            {
                _receivingDataLoop = _globalContext.Scheduler.StartCoroutine(CoroutineReceiveLoop());
            }
        }

        protected void StopReceiveLoop()
        {
            if (_receivingDataLoop != null)
            {
                _receivingDataLoop.Dispose();
                _receivingDataLoop = null;
            }
        }
    }
}