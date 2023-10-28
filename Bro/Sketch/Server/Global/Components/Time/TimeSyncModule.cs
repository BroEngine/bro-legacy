using System.Collections.Generic;
using Bro.Network;
using Bro.Server.Context;
using Bro.Server.Network;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server
{
    public class TimeSyncModule : IServerContextModule
    {
        private IServerContext _context;

        void IServerContextModule.Initialize(IServerContext context)
        {
            _context = context;
        }

        IList<CustomHandlerDispatcher.HandlerInfo> IServerContextModule.Handlers => null;

        [RequestHandler(Request.OperationCode.TimeSync)]
        private INetworkResponse OnReceiveTimeSyncRequest(INetworkRequest request, IClientPeer peer)
        {
            return NetworkOperationFactory.CreateResponse(request);
        }

        [UpdateHandler(updatePeriod: GameConfig.Time.SyncPeriodTimestamp)]
        private void UpdateTimeSync()
        {
            _context.ForEachPeer((p) => { p.Send(NetworkPool.GetOperation<TimeSyncEvent>()); });
        }
    }
}