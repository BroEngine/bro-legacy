using Bro.Server.Context;
using Bro.Server.Network;

namespace Bro.Sketch.Server
{
    public class LimbContext : ServerContext
    {
        public readonly IdentificationModule IdentificationModule = new IdentificationModule();
        
        public LimbContext(ContextStorage contextStorage, ConfigStorageCollector configStorageCollector) : base(contextStorage, configStorageCollector)
        {
        }
        
        [PeerJoinedHandler]
        private void OnPeerJoined(IClientPeer peer, object data)
        {
            Bro.Log.Info($"limb context :: OnPeerJoin");
        }
    }
}