using Bro.Json;
using UnityEngine;

namespace Bro.Toolbox.Navigation
{
    [System.Serializable]
    public class MapPassabilityConfig
    {
        [JsonProperty("boundary")] public Rect WorldBoundary;
        [JsonProperty("sector_size")] public Vector2 SectorSize;
        [JsonProperty("data")] public byte[] Data;

        [JsonIgnore]
        public bool IsSectorSizeValid => SectorSize.x >= float.Epsilon && SectorSize.y >= float.Epsilon;
    }
}