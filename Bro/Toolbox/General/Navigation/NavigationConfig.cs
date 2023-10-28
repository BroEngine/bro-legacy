using System;
using Bro.Json;

namespace Bro.Toolbox.Navigation
{
    [Serializable]
    public class NavigationConfig
    {
        [JsonProperty(PropertyName = "map_passability")] public MapPassabilityConfig MapPassabilityConfig;
    }
}