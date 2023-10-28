using Bro.Json;
using Bro.Toolbox.SectoredArea;

namespace Bro.Toolbox.Navigation
{
    public class PassabilitySector: Sector
    {
        public bool IsPassable;
        [JsonIgnore] public int Cost;
        [JsonIgnore] public PassabilitySector PreviousSector { get; set; }
    }
}