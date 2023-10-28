using System;
using Bro.Json;

namespace Bro.Sketch
{
    [Serializable]
    public class VersionModel
    {
        [JsonProperty("version")] public int Version;
        [JsonProperty("has_separate_config")] public bool HasSeparateConfig = false;
        [JsonProperty("is_only_one")] public bool IsOnlyOne = false;
        [JsonProperty("parts_count")] public short PartsCount;
    }
}