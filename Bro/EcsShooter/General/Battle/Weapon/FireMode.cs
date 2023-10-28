using Bro.Json;
using Bro.Json.Converters;

namespace Bro.Shooter
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FireMode : short
    {
        [JsonProperty("single")] Single = 0,
        [JsonProperty("burst")] Burst = 1,
        [JsonProperty("auto")] Auto = 2,
    }
}