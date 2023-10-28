using System.Collections.Generic;
using Bro.Server.Context;
using Bro.Server.Network;

namespace Bro.Sketch.Server
{
    public class IdentificationModule : IServerContextModule
    {
        private IServerContext _context;

        void IServerContextModule.Initialize(IServerContext context)
        {
            _context = context;
        }

        IList<CustomHandlerDispatcher.HandlerInfo> IServerContextModule.Handlers => null;
        
        [PeerJoinedHandler]
        private void OnPeerJoin(IClientPeer peer, object data)
        {
            Bro.Log.Info("identification module :: request received from peerId = " + peer.PeerId);
            var actor = new Actor();
            peer.SetActor(actor);
            actor.Peer = peer;
            _context.ContextStorage.GetEntryContext().Join(peer);
        }
    }
}