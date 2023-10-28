using System.Diagnostics;
using Bro.Client;
using Bro.Client.Network;


namespace Bro.Sketch.Client
{
    // ReSharper disable ConditionIsAlwaysTrueOrFalse
    public class TimeSyncHandler
    {
        private class RouteTimeData
        {
            public long ClientSendTimestamp;
            public long ServerReceivedTimestamp;
        }

        private RouteTimeData _preparingRoute;
        private RouteTimeData _currentRoute;
        private RouteTimeData _previousRoute;
        private readonly Stopwatch _stopwatch;
        private float _sendRefreshRequestTimer = 0f;
        private long _currentPingTimestamp;

        private int _fastSyncTries;

        private readonly NetworkEngine _networkEngine;
        private readonly IClientContext _context;
        
        private float _sleepTime => _stopwatch.ElapsedMilliseconds / 1000.0f;
        private bool _isAbnormalSleep => _sleepTime > TimeSyncConfig.StandardRefreshRequestPeriod;

        public TimeSyncHandler(IClientContext context)
        {
            _context = context;
            _networkEngine = _context.GetNetworkEngine();

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public void StartFastSync()
        {
            _fastSyncTries = 0;
        }

        public void OnUpdate()
        {
            if (_isAbnormalSleep)
            {
                TimeInfo.ResetTimeSync(false);
                _sendRefreshRequestTimer = 0.0f;
            }

            var deltaSeconds = _sleepTime;

            _stopwatch.Reset();
            _stopwatch.Start();

            _sendRefreshRequestTimer -= deltaSeconds;
            if (_sendRefreshRequestTimer < 0f)
            {
                var isFastSync = _fastSyncTries < TimeSyncConfig.FastSyncTriesMax;
                if (isFastSync)
                {
                    _fastSyncTries++;
                }
                _sendRefreshRequestTimer = isFastSync ? TimeSyncConfig.FastSyncRefreshRequestPeriod : TimeSyncConfig.StandardRefreshRequestPeriod;

                var timeSyncTask = new TimeSyncTask(_context, _networkEngine);
                OnSendRefreshRequest(timeSyncTask);
                timeSyncTask.OnComplete += ((t) => { OnReceiveRefreshResponse(t.TimestampWhenServerReceivedRefresh, t); });

                timeSyncTask.Launch(_context);
            }
        }

        public void SyncTime(long recievedServerTimestamp)
        {
            if (_isAbnormalSleep)
            {
                return;
            }

            _currentPingTimestamp = GetRoundTripTime(recievedServerTimestamp, TimeInfo.LocalTimestamp) / 2;
            TimeInfo.SyncTime(recievedServerTimestamp, _currentPingTimestamp);
            _sendRefreshRequestTimer = TimeSyncConfig.StandardRefreshRequestPeriod;
        }

        private long GetRoundTripTime(long serverSendTimestamp, long clientReceivedTimestamp)
        {
            if (_currentRoute == null)
            {
                return 0;
            }

            var activeRoute = _currentRoute.ServerReceivedTimestamp > serverSendTimestamp ? _previousRoute : _currentRoute;

            if (activeRoute == null)
            {
                return 0;
            }

            return (clientReceivedTimestamp - activeRoute.ClientSendTimestamp) - (serverSendTimestamp - activeRoute.ServerReceivedTimestamp);
        }

        private void OnSendRefreshRequest(TimeSyncTask task)
        {
            _preparingRoute = new RouteTimeData() {ClientSendTimestamp = TimeInfo.LocalTimestamp};
        }

        private void OnReceiveRefreshResponse(long timestampWhenServerRecievedRefresh, TimeSyncTask task)
        {
            if (_preparingRoute == null)
            {
                return;
            }

            _preparingRoute.ServerReceivedTimestamp = timestampWhenServerRecievedRefresh;
            _previousRoute = _currentRoute;
            _currentRoute = _preparingRoute;
            _preparingRoute = null;
        }
    }
}