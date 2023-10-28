using Bro.Network;
using Bro.Server.Context;
using Bro.Server.Network;

namespace Bro.Server.Observers
{
    public class HandShakeObserver
    {
        private bool _handShakeReceived = false;
        private bool _handShakeHandled = false;

        private readonly IServerContext _startContext;

        public HandShakeObserver(IServerContext startContext)
        {
            _startContext = startContext;
            if (_startContext == null)
            {
                Bro.Log.Error("handshake observer :: start context == null");
            }
        }

        public void OnReceive(HandShakeOperation operation, IClientPeer peer)
        {
            if (_startContext == null)
            {
                Bro.Log.Info("handshake observer :: start context == null, no handshaking will happen");
                return;
            }

            var response = new HandShakeOperation();
            if (_handShakeHandled)
            {
                peer.Send(response);
                return;
            }

            if (_handShakeReceived)
            {
                return;
            }

            _handShakeReceived = true;
            _startContext.Join(peer, () =>
            {
                peer.Send(response);
                _handShakeHandled = true;
            });
        }
    }
}