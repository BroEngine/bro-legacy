// ----------------------------------------------------------------------------
// The MIT License
// Lightweight ECS framework https://github.com/Leopotam/ecslite
// Copyright (c) 2021-2022 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Leopotam.EcsLite
{
    public partial class EcsWorld
    {
        private Type[] _buffer = new Type[512];
        private int _bufferIndex;
        
        public void Set(EcsWorld world)
        {
            //copy entities
            if (Entities.Length < world.GetWorldSize())
            {
                ResizeEntity(world.GetWorldSize());
            }

            for (int i = 0; i < world._entitiesCount; i++)
            {
                Entities[i] = world.Entities[i];
            }

            if (_entitiesCount > world._entitiesCount)
            {
                for (int i = world._entitiesCount, iMax = _entitiesCount; i < iMax; i++)
                {
                    DelEntity(i);
                }
            }

            _entitiesCount = world._entitiesCount;
            
            //copy recycle entities
            if (_recycledEntities.Length < world._recycledEntities.Length)
            {
                Array.Resize (ref _recycledEntities, world._recycledEntities.Length );
            }

            for (int i = 0; i < world._recycledEntitiesCount; i++)
            {
                _recycledEntities[i] = world._recycledEntities[i];
            }

            _recycledEntitiesCount = world._recycledEntitiesCount;
            // set pool
            
            foreach (var sourcePoolItem in world._poolHashes)
            {
               var poolType = sourcePoolItem.Key;
               if( _poolHashes.TryGetValue(poolType, out var pool))
               {
                   pool.Copy(sourcePoolItem.Value);
               }
               else if (_recycledPools.TryGetValue(poolType, out var restorePool))
               {
                   _poolHashes[poolType] = restorePool;
               }
               else
               {
                    var poolGenericType = typeof(EcsPool<>).MakeGenericType(poolType);
                    var o = Activator.CreateInstance(poolGenericType, new object[] { this, sourcePoolItem.Value });
                   _poolHashes[poolType] = (IEcsPool) o;
               }
               _poolHashes[poolType].Copy(sourcePoolItem.Value);
            }

            _bufferIndex = 0;
            foreach (var poolHashPair in _poolHashes)
            {
                var poolType = poolHashPair.Key;
                if (!world._poolHashes.ContainsKey(poolType))
                {
                    _recycledPools[poolType] = _poolHashes[poolType];

                    _buffer[_bufferIndex] = poolType;
                    _bufferIndex++;
                    
                    // _poolHashes.Remove(poolType);
                }
            }

            for (int i = 0; i < _bufferIndex; ++i)
            {
                Bro.Log.Error("chech");
                _poolHashes.Remove(_buffer[i]);
            }

            
            //
            
            var poolMaxCount = _poolsCount > world._poolsCount ? _poolsCount : world._poolsCount;
            for (int i = 0; i < poolMaxCount; i++)
            {
                var sourcePool = world._pools[i];
                if (sourcePool != null)
                {
                    var type = sourcePool.GetComponentType();
                    _pools[i] = _poolHashes[type];
                }
                else
                {
                    _pools[i] = null;
                }
            }
            
            if (_poolHashes.Count != world._poolHashes.Count)
            {
                Bro.Log.Error($"ecs world :: pools are not the same after copy. pool-{_poolHashes.Count} source pool-{world._poolHashes.Count}");
            }
            
            _poolsCount = world._poolsCount;
            
            
            
            
            

            // clear filters
            if (world._allFiltersCount > _allFilters.Length) 
            {
                Array.Resize (ref _allFilters, world._allFiltersCount);
            }
            
            _filterHashes.Clear();

            for (int i = 0, iMax = _filtersByIncludedComponents.Length; i < iMax;i++) {
                if (_filtersByIncludedComponents[i] != null) {
                    _filtersByIncludedComponents[i].Clear();
                }
            }
            if (world._filtersByIncludedComponents.Length > _filtersByIncludedComponents.Length) {
                Array.Resize (ref _filtersByIncludedComponents, world._filtersByIncludedComponents.Length);

            }
            for (int i = 0, iMax = _filtersByExcludedComponents.Length; i < iMax;i++) {
                if (_filtersByExcludedComponents[i] != null) {
                    _filtersByExcludedComponents[i].Clear();
                }
            }
            if (world._filtersByExcludedComponents.Length > _filtersByExcludedComponents.Length) {
                Array.Resize (ref _filtersByExcludedComponents, world._filtersByExcludedComponents.Length);
            }
            
            //recycle filters
            for (int i = world._allFiltersCount, iMax = _allFiltersCount; i < iMax; i++)
            {
                var filter = _allFilters[i];
                RecycleFilter(filter);
                _allFilters[i] = null;

            }
            // set filters
            for (int i = 0, iMax = world._allFiltersCount;  i < iMax; i++)
            {
                if (_allFilters[i] == null) {
                    if (_recycledMasksCount > 0 && _recycledFilters.TryGetValue(_recycledMasks[_recycledMasksCount - 1], out var filter))
                    {
                        _allFilters[i] = filter;
                        _recycledMasksCount--;
                        Bro.Log.Error("!!! set :: use recycle filter , _recycledMasksCount = " + _recycledMasksCount);
                    }
                    else
                    {
                        _allFilters[i] = new EcsFilter(this, new EcsMask(this), 1, 1 );
                    }
                }

                _allFilters[i].Set(world._allFilters[i]);
                var updatedFilter = _allFilters[i];
                var mask = updatedFilter.GetMask();
                var hash = mask.Hash;
                _filterHashes[hash] = updatedFilter;
                for (int j = 0, jMax = mask.IncludeCount; j < jMax; j++)
                {
                    var componentId = mask.Include[j];
                    if (_filtersByIncludedComponents[componentId] == null)
                    {
                        _filtersByIncludedComponents[componentId] = new List<EcsFilter>();
                    }
                    _filtersByIncludedComponents[componentId].Add(updatedFilter);
                }
                
                for (int j = 0, jMax = mask.ExcludeCount; j < jMax; j++)
                {
                    var componentId = mask.Exclude[j];
                    if (_filtersByExcludedComponents[componentId] == null)
                    {
                        _filtersByExcludedComponents[componentId] = new List<EcsFilter>();
                    }
                    _filtersByExcludedComponents[componentId].Add(updatedFilter);
                }
            }

            _allFiltersCount = world._allFiltersCount;


            _destroyed = false;
        }
    }
}