using Bro.Server.Network;
using Unity.Plastic.Newtonsoft.Json;

namespace Bro.Sketch.Server
{
    public class ServerConfig
    {
        [JsonProperty("is_offline")] public bool IsOffline;
        
        [JsonProperty("log_prefix")] public string LogPrefix = "server";
        
        [JsonProperty("udp_port")] public int UdpPort = 3030;
        [JsonProperty("tcp_port")] public int TcpPort = 0;

        #warning todo replace it
        [JsonProperty("broker_uri")] public string BrokerUri = "amqp://local:local@###:5672/";
        [JsonProperty("master_uri")] public string MasterUri = "http://###/server";
        
        [JsonProperty("network_operation_thread_amount")] public int NetworkOperationThreadAmount = 128;
        [JsonProperty("network_read_thread_amount")] public int NetworkReadThreadAmount = 32;
        [JsonProperty("network_write_thread_amount")] public int NetworkWriteThreadAmount = 64;
        [JsonProperty("network_udp_thread_amount")] public int NetworkUdpThreadAmount = 32;
        [JsonProperty("network_max_connections")] public int NetworkMaxConnections = 1024;
        [JsonProperty("network_bottleneck_connections")] public int NetworkBottleneckConnections = 200;
        

        [JsonProperty("add_local_battle_server_relation")] public bool AddLocalBattleServerRelation = false;
        [JsonProperty("kick_out_timeout")] public long KickOutTimeout = 10000L;
        
        
        public NetworkEngine.Config GetNetworkConfig()
        {
            return  new NetworkEngine.Config
            {
                OperationThreads = NetworkOperationThreadAmount,
                ReadThreads = NetworkReadThreadAmount,
                WriteThreads = NetworkWriteThreadAmount,
                UdpConfig = new Bro.Server.Network.Udp.NetworkEngineConfig
                {
                    Port = UdpPort,
                    MaxConnections = NetworkMaxConnections,
                    BottleneckConnections = NetworkBottleneckConnections,
                    Threads = NetworkUdpThreadAmount
                }
            };
        }
    }
}