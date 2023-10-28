using Bro.Json;
using Bro.Json.Converters;

namespace Bro.Shooter
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AimType : int
    {
        [JsonProperty("position")] Position = 0,
        [JsonProperty("cone")] Cone = 1,
        [JsonProperty("line")] Line = 2,
        [JsonProperty("line_laser")] LineLaser = 3,
    }
}