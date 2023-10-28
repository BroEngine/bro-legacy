using Bro.Json;
using Bro.Json.Converters;

namespace Bro.Shooter
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InputMode : short
    {
        [JsonProperty("single")] Single = 0,
        [JsonProperty("auto")] Auto = 1,
    }
}