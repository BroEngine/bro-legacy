using System;
using Bro.Json;

namespace Bro.Sketch
{
    [Serializable]
    public struct GameServer
    {
        [JsonProperty("server_id")] public int ServerId;
        [JsonProperty("type")] public string Type;
        [JsonProperty("ip")] public string Ip;
        [JsonProperty("port")] public int Port;
        [JsonProperty("version")] public int Version;
    }
}