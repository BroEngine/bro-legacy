using System;
using System.Collections.Generic;
using Bro.Server.Context;
using Bro.Server.Network;

namespace Bro.Sketch.Server
{
    public class SaveProfileModule : IServerContextModule
    {
        private IServerContext _context;
        private IProfileProvider _profileProvider;

        public IList<CustomHandlerDispatcher.HandlerInfo> Handlers => new List<CustomHandlerDispatcher.HandlerInfo>();
        public void Initialize(IServerContext context)
        {
            _context = context;
            _profileProvider = context.GetModule(c => c is IProfileProvider) as IProfileProvider;
            Bro.Log.Assert(_profileProvider != null, "save profile module :: profile provider is null");
            
            AssemblyEvent.Delegate += OnAssemblyEvent;
        }
        
        private void OnAssemblyEvent(Enum e)
        {
            if (e is AssemblyEventType type)
            {
                switch (type)
                {
                    case AssemblyEventType.SaveProfileRequest:
                        SaveAll();
                        break;
                }
            }
        }
        
        [PeerLeftHandler]
        private void OnPeerLeft(IClientPeer peer)
        {
            var actor = peer.GetActor();
            Save(actor);
        }

        public void Save(Actor actor, Action callback = null)
        {
            var profile = actor.Profile;
            if (profile != null)
            {
                _profileProvider.Save(profile, actor.Peer, (result) =>
                {
                    callback?.Invoke();
                });
            }
        }

        public void SaveAll()
        {
            _context.ForEachPeer((peer) =>
            {
                var actor = peer.GetActor();
                Save(actor);
            });
        }
    }
}