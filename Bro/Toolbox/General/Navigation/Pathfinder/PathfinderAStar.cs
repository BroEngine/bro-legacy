using System;
using System.Collections.Generic;
using Bro.Toolbox.Navigation;
using Bro.Toolbox.SectoredArea;

namespace Bro
{
    public class PathfinderAStar
    {
        private const int MaxSteps = 2048;
        private const int MaxPathSize = 255;
        private readonly MinHeap<PassabilitySector> _frontier;
        private readonly HashSet<PassabilitySector> _visited;

        public PathfinderAStar()
        {
            _frontier = new MinHeap<PassabilitySector>();
            _visited = new HashSet<PassabilitySector>();

        }
        
        public List<PassabilitySector> FindSectorsAStar(Area<PassabilitySector> area, PassabilitySector start, PassabilitySector end)
        {
            foreach (var sector in area.Sectors)
            {
                sector.Cost = int.MaxValue;
                sector.PreviousSector = null;
            }

            start.Cost = 0;

            Comparison<PassabilitySector> heuristicComparison = (lhs, rhs) =>
            {
                var lhsCost = lhs.Cost + GetEuclideanHeuristicCost(lhs, end);
                var rhsCost = rhs.Cost + GetEuclideanHeuristicCost(rhs, end);
                return lhsCost.CompareTo(rhsCost);
            };

            _frontier.SetComparer(heuristicComparison);
            _frontier.Clear();
            _frontier.Add(start);

            _visited.Clear();
            _visited.Add(start);

            start.PreviousSector = null;

            while (_frontier.Count > 0)
            {
                var current = _frontier.Remove();
                if (current == end)
                {
                    break;
                }

                foreach (var neighbor in area.GetNeighbors(current))
                {
                    var newNeighborCost = current.Cost + ( neighbor.IsPassable ? 0 : short.MaxValue );
                    if (newNeighborCost < neighbor.Cost)
                    {
                        neighbor.Cost = newNeighborCost;
                        neighbor.PreviousSector = current;
                    }

                    if (!_visited.Contains(neighbor))
                    {
                        _frontier.Add(neighbor);
                        _visited.Add(neighbor);
                    }
                }
            }

            var path = BacktrackToPath(end);
            return path;
        }
        
        
        
        private float GetEuclideanHeuristicCost(PassabilitySector current, PassabilitySector end)
        {
            return (current.Center - end.Center).magnitude;
        }
        
        private List<PassabilitySector> BacktrackToPath(PassabilitySector end)
        {
            var current = end;
            var path = new List<PassabilitySector>();

            while (current != null)
            {
                path.Add(current);
                current = current.PreviousSector;

                if (path.Count > MaxPathSize)
                {
                    Bro.Log.Error("pathfinder :: max path size is reached, size = " + path.Count);
                    break;
                }
            }

            path.Reverse();

            return path;
        }
    }
}