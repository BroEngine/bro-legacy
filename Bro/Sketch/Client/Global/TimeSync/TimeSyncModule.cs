using System;
using System.Collections;
using System.Collections.Generic;
using Bro.Client;
using Bro.Client.Network;
using Bro.Sketch.Network;

namespace Bro.Sketch.Client
{
    public class TimeSyncModule : IClientContextModule
    {
        private NetworkEventObserver<TimeSyncEvent> _timeSyncEventObserver;
        private EventObserver<AuthorizationEvent> _authorizationEventObserver;
        private IDisposable _onUpdateHandler;
        private IClientContext _context;
        private TimeSyncHandler _timeSyncHandler;
        private bool IsEnabled = false;

        IList<CustomHandlerDispatcher.HandlerInfo> IClientContextModule.Handlers => null;

        void IClientContextModule.Initialize(IClientContext context)
        {
            _context = context;
            _timeSyncHandler = new TimeSyncHandler(_context);
        }

        IEnumerator IClientContextModule.Load()
        {
            _onUpdateHandler = _context.Scheduler.ScheduleUpdate(OnUpdate);
            _authorizationEventObserver = new EventObserver<AuthorizationEvent>(OnAuthorizationEvent);
            _timeSyncEventObserver = new NetworkEventObserver<TimeSyncEvent>(OnTimeSyncEvent, _context.GetNetworkEngine());
            return null;
        }

        IEnumerator IClientContextModule.Unload()
        {
            _onUpdateHandler.Dispose();
            _onUpdateHandler = null;
            _timeSyncEventObserver.Dispose();
            _timeSyncEventObserver = null;
            _authorizationEventObserver.Dispose();
            _authorizationEventObserver = null;
            return null;
        }
        
        private void OnAuthorizationEvent(AuthorizationEvent e)
        {
            IsEnabled = e.IsAuthorized;
            if (!e.IsAuthorized)
            {
                TimeInfo.ResetTimeSync(true);
            }
            else
            {
                _timeSyncHandler.StartFastSync();
            }
        }

        private void OnTimeSyncEvent(TimeSyncEvent timeSyncEvent)
        {
            if (IsEnabled)
            {
                _timeSyncHandler.SyncTime(timeSyncEvent.ServerTimestamp.Value);
            }
        }

        private void OnUpdate(float delta)
        {
            if (IsEnabled)
            {
                _timeSyncHandler.OnUpdate();
            }
        }
    }
}