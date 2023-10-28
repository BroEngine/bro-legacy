using System.Collections.Generic;
using Bro.Network;

using Bro.Server;
using Bro.Server.Context;

namespace Bro.Sketch.Server
{
    public class KickOutModule : IServerContextModule
    {
        private IServerContext _context;
        private readonly long _timeoutMs;

        public KickOutModule(long timeoutMs)
        {
            _timeoutMs = timeoutMs;
        }

        void IServerContextModule.Initialize(IServerContext context)
        {
            _context = context;
        }

        IList<CustomHandlerDispatcher.HandlerInfo> IServerContextModule.Handlers => null;

        [UpdateHandler(updatePeriod: 1000L)]
        private void Update()
        {
            var timestamp = TimeInfo.LocalTimestamp;
            _context.ForEachPeer(peer =>
            {
                if (peer.InactiveTimeMs > _timeoutMs)
                {
                    Bro.Log.Info($"kick out module :: peer {peer.PeerId} disconnected case received timeout == {peer.InactiveTimeMs} ms");
                    peer.Disconnect(DisconnectCode.SystemTimeout);
                }
            });
        }
    }
}