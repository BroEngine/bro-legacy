using Bro.Toolbox.SectoredArea;
using UnityEngine;

namespace Bro.Toolbox.Navigation
{
    public class MapPassability : Area<PassabilitySector>
    {
        public MapPassability(MapPassabilityConfig config) 
        {
            Init(config.WorldBoundary, config.SectorSize);

            for (int i = 0, max = config.Data.Length; i < max; ++i)
            {
                Sectors[i].IsPassable = config.Data[i] == 1;
            }
        }

        public bool IsPassable(Vector2 position)
        {
            if (!IsInside(position))
            {
                return false;
            }
            
            return GetSector(position).IsPassable;
        }

        public bool IsPassable(Vector2 startPoint, Vector2 finishPoint)
        {
            bool result = true;

            if (!IsInside(startPoint) || !IsInside(finishPoint))
            {
                return false;
            }

            ForEachSector(startPoint, finishPoint, sector =>
            {
                if (!sector.IsPassable)
                {
                    result = false;
                    return true; // stop iterating
                }
                return false;
            } );
            return result;
        }
    }

}
