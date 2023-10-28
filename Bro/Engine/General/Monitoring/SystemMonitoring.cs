using System;
using Bro.Monitoring;

namespace Bro
{
    public static class SystemMonitoring
    {
        public static readonly IGauge NetworkPeersCount;
        public static readonly ISummary NetworkUdpSentBytes;
        public static readonly ISummary NetworkUdpReceivedBytes;
        public static readonly ISummary NetworkConnectionsRate;
        public static readonly ISummary NetworkDisconnectionsRate;
        
        #if BRO_SERVER || BRO_SERVICE || BRO_TEST

        private static MetricServer _server;
        
        static SystemMonitoring()
        {
            NetworkPeersCount = Metrics.CreateGauge("bro_system_network_peers_count","todo");
            NetworkUdpSentBytes = Metrics.CreateSummary("bro_system_network_udp_sent_bytes", "todo", new SummaryConfiguration() { MaxAge  = TimeSpan.FromSeconds(1)});
            NetworkUdpReceivedBytes = Metrics.CreateSummary("bro_system_network_udp_received_bytes", "todo", new SummaryConfiguration() { MaxAge  = TimeSpan.FromSeconds(1)});
            NetworkConnectionsRate = Metrics.CreateSummary("bro_system_network_connections_rate", "todo", new SummaryConfiguration() { MaxAge  = TimeSpan.FromMinutes(1)});
            NetworkDisconnectionsRate = Metrics.CreateSummary("bro_system_network_disconnections_rate", "todo", new SummaryConfiguration() { MaxAge  = TimeSpan.FromMinutes(1)});
        }

        public static void Start()
        {
            const int port = 6060;
            AppDomain.CurrentDomain.ProcessExit += OnApplicationExit;
            _server = new MetricServer("*", port);
            _server.Start();
            
            Bro.Log.Info("monitoring :: server started at port = " + port);
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            if (_server != null)
            {
                _server.Stop();
                Bro.Log.Info("monitoring :: server stopped");
            }
        }
        
        #endif
    }
}