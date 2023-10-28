using System;
using Bro.Network;

namespace Bro.Client.Network
{
    public class NetworkEventObserver<T> : IDisposable where T : Bro.Network.INetworkEvent
    {
        private readonly Type _eventType;
        private Action<T> _handler;
        private readonly NetworkEngine _networkEngine;

        public NetworkEventObserver(NetworkEngine networkEngine)
        {
            _networkEngine = networkEngine;
            _eventType = typeof(T);
        }
        
        public NetworkEventObserver(Action<T> handler, NetworkEngine networkEngine) : this(networkEngine)
        {   
            Subscribe(handler);
        }

        ~NetworkEventObserver()
        {
            Unsubscribe();
        }

        public void Subscribe(Action<T> handler)
        {
            Unsubscribe();
            _handler = handler;
            _networkEngine.OnNetworkEventReceived += OnNetworkEventReceived;
        }

        private void OnNetworkEventReceived(INetworkEvent e)
        {
            if (e.GetType() == _eventType)
            {
                _handler?.Invoke((T) e);
            }
        }

        public void Unsubscribe()
        {
            _handler = null;
            _networkEngine.OnNetworkEventReceived -= OnNetworkEventReceived;
        }

        public void Dispose()
        {
            Unsubscribe();
        }
    }
}