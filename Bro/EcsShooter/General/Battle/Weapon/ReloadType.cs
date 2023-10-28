using Bro.Json;
using Bro.Json.Converters;

namespace Bro.Shooter
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ReloadType : short
    {
        [JsonProperty("none")] None = 0,
    }
}