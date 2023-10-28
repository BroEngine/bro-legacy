using Bro.Json;
using Bro.Json.Converters;

namespace Bro
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RegionType : byte
    {
        [JsonProperty("undefined")] Undefined = 0,
        [JsonProperty("eu-c")] WesternEurope = 1,
        [JsonProperty("undefined")] NorthAmerica = 2,
        [JsonProperty("ap-c")] SoutheastAsia = 3,
    }
}