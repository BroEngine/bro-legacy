using Bro.Json;

namespace Bro.Sketch.Server
{
    #pragma warning disable 660,661
    public class BattleServerInfo
    {
        [JsonProperty("battle_server_id")] public int BattleServerId;
        [JsonProperty("ip")] public string Ip;
        [JsonProperty("port_udp")] public int PortUdp;
        [JsonProperty("port_tcp")] public int PortTcp;
        [JsonProperty("total_online")] public int TotalOnline;
        [JsonProperty("bots_online")] public int BotsOnline;
        [JsonProperty("max_online")] public int MaxOnline;
        [JsonProperty("available")] public int Available;
        
        public override string ToString()
        {
            return Ip + ":(" + PortUdp + "|" + PortTcp + ")";
        }

        public static bool operator ==(BattleServerInfo p1, BattleServerInfo p2)
        {
            var p1IsNull = ReferenceEquals(p1, null);
            var p2IsNull = ReferenceEquals(p2, null);
            
            if (p1IsNull && p2IsNull)
            {
                return true;
            }

            if (!p1IsNull && !p2IsNull)
            {
                return p1.BattleServerId == p2.BattleServerId;
            }
            
            return false;
        }

        public static bool operator !=(BattleServerInfo p1, BattleServerInfo p2)
        {
            return !(p1 == p2);
        }
    }
}