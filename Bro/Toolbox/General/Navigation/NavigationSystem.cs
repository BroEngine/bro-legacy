using System.Collections.Generic;
using Bro.Sketch;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Bro.Toolbox.Navigation
{
    public class NavigationSystem
    {
        public delegate bool CheckObstaclesDelegate(Vector2 from, Vector2 to);

        public MapPassability MapPassability => _mapPassability;
        public NavigationConfig NavigationConfig => _navigationConfig;
        
        private NavigationConfig _navigationConfig;
        private MapPassability _mapPassability;
        private CheckObstaclesDelegate _checkObstacleDelegate;
        private PathfinderAStar _pathfinderAStar;
        private List<PassabilitySector> _shortAStarPath;
        private List<int> _aStarSectorIndexesToDelete;

        /* Near passability check */
        private float _passabilityCheckStep = 2f;
        private readonly List<Vector2> _passabilityCheckVectors = new List<Vector2>() { Vector2.up, Vector2.up + Vector2.right, Vector2.right, Vector2.right + Vector2.down, Vector2.down, Vector2.down + Vector2.left, Vector2.left, Vector2.left + Vector2.up };

        private Vector2 CheckNearPassability(Vector2 position)
        {
            var pointsCount = _passabilityCheckVectors.Count;
            
            var index = Random.Instance.Range(0, pointsCount);
            var iterations = 0;
            while (iterations < pointsCount)
            {
                if (index >= pointsCount)
                {
                    index = 0;
                }

                var point = position + _passabilityCheckVectors[index] * _passabilityCheckStep;
                var isValid = IsPassable(point);
                if (isValid)
                {
                    return point;
                }

                ++index;
                ++iterations;
            }
            
            return position;
        }
        /* Near passability check */
            
        public void Init(CheckObstaclesDelegate checkObstaclesHandler, NavigationConfig navConfig)
        {
            _navigationConfig = navConfig;
            _mapPassability = new MapPassability(navConfig.MapPassabilityConfig);
            _checkObstacleDelegate = checkObstaclesHandler;
            _pathfinderAStar = new PathfinderAStar();
            _shortAStarPath = new List<PassabilitySector>();
            _aStarSectorIndexesToDelete = new List<int>();
        }

        public bool IsPassable(Vector2 position)
        {
            if (_mapPassability == null)
            {
                Bro.Log.Error(new System.Exception("_mapPassability is null"));
                return true;
            }

            return _mapPassability.IsPassable(position);
        }
        
        public bool IsPassable(Vector2 from, Vector2 to, bool isPrecise)
        {
            if (_mapPassability == null)
            {
                //UnityEngine.Debug.LogException(new System.Exception("_mapPassability is null"));
                if (_checkObstacleDelegate == null)
                {
                    Bro.Log.Error(new System.Exception("_mapPassability and _checkObstacleDelegate are null"));
                    return true;
                }
                return !_checkObstacleDelegate(from, to);
            }

            
            /* Main section */
            if (_mapPassability.IsPassable(from, to))
            {
                if (_checkObstacleDelegate == null)
                {
                    //UnityEngine.Debug.LogException(new System.Exception("_checkObstacleDelegate is null"));
                    return true;
                }
                return !_checkObstacleDelegate(from, to);
            }
            /* Main section */
            
            
            if (isPrecise)
            {
                if (_checkObstacleDelegate == null)
                {
                    //UnityEngine.Debug.LogException(new System.Exception("_checkObstacleDelegate is null"));
                    return true;
                }
                return !_checkObstacleDelegate(from, to);
            }

            return false;
        }

        public List<PassabilitySector> GetSimplifiedAStarSectors(Vector2 startPosition, Vector2 finishPosition)
        {       

            var startSector = _mapPassability.GetSector(startPosition);
            var finishSector = _mapPassability.GetSector(finishPosition);
            
            if (!startSector.IsPassable)
            {
                foreach (var startSectorNeighbor in _mapPassability.GetNeighbors(startSector))
                {
                    if (IsPassable(startPosition, startSectorNeighbor.Center, true) && startSectorNeighbor.IsPassable)
                    {
                        startSector = startSectorNeighbor;
                        break;
                    }
                }
            }
            
            if (!finishSector.IsPassable)
            {
                foreach (var finishSectorNeighbor in _mapPassability.GetNeighbors(finishSector))
                {
                    if (IsPassable(finishPosition, finishSectorNeighbor.Center, true) && finishSectorNeighbor.IsPassable )
                    {
                        finishSector = finishSectorNeighbor;
                        break;
                    }
                }
            }
            

            var attempts = 0;
            var maxSimplificationCycles = 5;
            var result = _pathfinderAStar.FindSectorsAStar(_mapPassability, startSector, finishSector);
            _shortAStarPath.Clear();
            _aStarSectorIndexesToDelete.Clear();
            if (result.Count > 2)
            {
                _shortAStarPath.Add(result[0]);
                for (int i = 1; i < result.Count - 1; i++)
                {
                    if (result[i].Center - result[i - 1].Center != result[i + 1].Center - result[i].Center)
                    {
                        _shortAStarPath.Add(result[i]);
                    }
                }
                _shortAStarPath.Add(result[result.Count-1]);
              
                
                if (_shortAStarPath.Count > 2)
                {
                    bool hasPointsToDelete;
                    do
                    {
                        attempts++;
                        hasPointsToDelete = false;
                        for (int currentIndex = 0; currentIndex < _shortAStarPath.Count - 1; currentIndex++)
                        {
                            if (_aStarSectorIndexesToDelete.Contains(currentIndex))
                            {
                                continue;
                            }
                            
                            var targetIndex = currentIndex + 2;
                            while (targetIndex<_shortAStarPath.Count && IsPassable(_shortAStarPath[currentIndex].Center, _shortAStarPath[targetIndex].Center,false))
                            {
                                _aStarSectorIndexesToDelete.Add(targetIndex - 1);
                                targetIndex++;
                                hasPointsToDelete = true;
                            }
                        }
                    
                        _aStarSectorIndexesToDelete.Reverse();
                    
                        for (int i = 0;i < _aStarSectorIndexesToDelete.Count;i ++)
                        {
                            _shortAStarPath.RemoveAt(_aStarSectorIndexesToDelete[i]);
                        }
                        _aStarSectorIndexesToDelete.Clear();
                    } while (hasPointsToDelete && attempts <= maxSimplificationCycles);
                }
            }
            else
            {
                return result;
            }
            
            return _shortAStarPath;
        }

        public Path GetPathAstar(Vector2 startPos, Vector2 finishPos)
        {
            Path path = new Path();
            if (IsPassable(startPos, finishPos, isPrecise: false))
            {
                path = new Path(startPos, finishPos);
            }
            else
            {
                List<PassabilitySector> pathSectors = GetSimplifiedAStarSectors(startPos, finishPos);
                path.Points.Add(startPos);
                foreach (var sector in pathSectors)
                {
                    path.Points.Add(sector.Center);
                }
                path.Points.Add(finishPos);
                
            }

            return path;

        }

    }
}