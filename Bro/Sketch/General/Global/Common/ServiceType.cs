using System.ComponentModel;
using Bro.Json;
using Bro.Json.Converters;


namespace Bro.Sketch
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ServiceType
    {
        [JsonProperty("match_making")] [Description("match_making")] Matchmaking
    }
}
