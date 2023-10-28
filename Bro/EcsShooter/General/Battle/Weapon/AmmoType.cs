using Bro.Json;
using Bro.Json.Converters;

namespace Bro.Shooter
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AmmoType : short
    {
        [JsonProperty("bullet")] Bullet = 0, 
        [JsonProperty("arc")] Arc,
    }
}