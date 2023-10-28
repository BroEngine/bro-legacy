using System;
using System.Collections;
using System.Collections.Generic;
using Bro.Client;
using Bro.Client.Network;
using Bro.Sketch.Network;

namespace Bro.Sketch.Client
{
    public class UserStatusModule : IClientContextModule
    {
        private readonly UserStatusStorage _storage = new UserStatusStorage();
        private IDisposable _onUpdateHandler;
        private IClientContext _context;
        
        public IList<CustomHandlerDispatcher.HandlerInfo> Handlers => new List<CustomHandlerDispatcher.HandlerInfo>();

        public void Initialize(IClientContext context)
        {
            _context = context;
        }
        
        public IEnumerator Load()
        {
            var engine = _context.GetNetworkEngine();
            _context.AddDisposable(new NetworkEventObserver<UserStatusEvent>(OnUserStatusEvent, engine));
            _onUpdateHandler = _context.Scheduler.ScheduleUpdate(OnUpdate);
            
            yield return null;
        }

        public IEnumerator Unload()
        {
            _onUpdateHandler.Dispose();
            _onUpdateHandler = null;
            
            yield return null;
        }

        private void OnUpdate(float delta)
        {
            _storage.OnUpdate();
        }
        
        private void OnUserStatusEvent(UserStatusEvent statusEvent)
        {
            var status = statusEvent.Status.Value;
            RegisterAndValidate(ref status);
        }
        
        public virtual void RegisterAndValidate(ref UserStatus status) 
        {
            _storage.RegisterAndValidate(ref status);
        }
        
        public UserStatus GetUserStatus(int userId)
        {
            return _storage.GetUserStatus(userId);
        }
        
        public byte GetUserState(int userId)
        {
            return _storage.GetUserState(userId);
        }
    }
}