using System.Collections;
using System.Collections.Generic;
using Bro.Client;
using Bro.Client.Network;

namespace Bro.Sketch.Client
{
    public class SessionModule : IClientContextModule
    {
        private bool _isAuthorized;
        private int _userId;
        public int SessionId { get; private set; }

        public short OwnerId; // get/set

        public int HeroUserId
        {
            get
            {
                if (!_isAuthorized)
                {
                    Bro.Log.Error("session module :: try to get user id when user is not authorized");
                }
                return _userId;
            }
            set
            {
                _userId = value;
                IsAuthorized = true;
            }
        }

        public bool IsAuthorized
        {
            get => _isAuthorized;
            set
            {
                if (_isAuthorized != value)
                {
                    _isAuthorized = value;
                    new AuthorizationEvent(_isAuthorized).Launch();
                }
            }
        }

        IList<CustomHandlerDispatcher.HandlerInfo> IClientContextModule.Handlers => null;

        void IClientContextModule.Initialize(IClientContext context)
        {
            var networkEngine = context.GetNetworkEngine();
            networkEngine.OnStatusChanged += OnNetworkStatusChanged;
            SessionId = Random.Instance.Range(0, (int) (TimeInfo.GlobalTimestamp - TimeInfo.GlobalLastMidnightTimestamp));
        }

        IEnumerator IClientContextModule.Load()
        {
            return null;
        }

        IEnumerator IClientContextModule.Unload()
        {
            return null;
        }

        private void OnNetworkStatusChanged(NetworkStatus networkStatus, int descriptionCode)
        {
            if (networkStatus == NetworkStatus.Disconnected)
            {
                IsAuthorized = false;
            }
        }
    }
}