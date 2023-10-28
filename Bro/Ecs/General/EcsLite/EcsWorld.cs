// ----------------------------------------------------------------------------
// The MIT License
// Lightweight ECS framework https://github.com/Leopotam/ecslite
// Copyright (c) 2021-2022 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Bro;
using Bro.Ecs;
using Bro.Network.Tcp.Engine.Client;

namespace Leopotam.EcsLite {

    public partial class EcsWorld
    {
        public const int Singleton = 0;
        
        
        internal EntityData[] Entities;
        private int _entitiesCount;
        private int[] _recycledEntities;
        private int _recycledEntitiesCount;
        private int _poolsCount;

        private readonly List<IEcsPool> _pools;
        private readonly Dictionary<Type,IEcsPool> _recycledPools = new Dictionary<Type, IEcsPool>();
        private readonly int _poolDenseSize;
        private readonly int _poolRecycledSize;
        private readonly Dictionary<Type, IEcsPool> _poolHashes;
        private readonly Dictionary<int, EcsFilter> _filterHashes;
        private readonly Dictionary<EcsMask, EcsFilter> _recycledFilters;

        private EcsFilter[] _allFilters;
        private int _allFiltersCount;
        private List<EcsFilter>[] _filtersByIncludedComponents;
        private List<EcsFilter>[] _filtersByExcludedComponents;
        
        private EcsMask[] _recycledMasks;
        private int _recycledMasksCount;
        
        public override string ToString()
        {
            // #if UNITY_EDITOR
            int[] entities = null;
            Type[] typesCache = null;
            GetAllEntities(ref entities);
            var entitiesInfo = new StringBuilder();
            
            foreach (var entity in entities)
            {
             
                
                var size = GetComponentTypes(entity, ref typesCache);
                var entityName = new StringBuilder(entity + " = ");
                for (var i = 0; i < size; i++)
                {
                    var type = typesCache[i] != null ? typesCache[i].ToString() : "! null !";
                    entityName.Append( type + "; ");
                }
                entitiesInfo.Append($"{entityName}\n");
            }
            return $"ecs world info :: entities:\n" + entitiesInfo;
            // #endif
            return string.Empty;
        }

        bool _destroyed;
#if ECS_DEBUG || LEOECSLITE_WORLD_EVENTS
        List<IEcsWorldEventListener> _eventListeners;

        public void AddEventListener (IEcsWorldEventListener listener) {
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (listener == null) { throw new Exception ("Listener is null."); }
#endif
            _eventListeners.Add (listener);
        }

        public void RemoveEventListener (IEcsWorldEventListener listener) {
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (listener == null) { throw new Exception ("Listener is null."); }
#endif
            _eventListeners.Remove (listener);
        }

        public void RaiseEntityChangeEvent (int entity) {
            for (int ii = 0, iMax = _eventListeners.Count; ii < iMax; ii++) {
                _eventListeners[ii].OnEntityChanged (entity);
            }
        }
#endif
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
        readonly List<int> _leakedEntities = new List<int> (512);

        internal bool CheckForLeakedEntities () {
            if (_leakedEntities.Count > 0) {
                for (int i = 0, iMax = _leakedEntities.Count; i < iMax; i++) {
                    ref var entityData = ref Entities[_leakedEntities[i]];
                    if (entityData.Gen > 0 && entityData.ComponentsCount == 0) {
                        return true;
                    }
                }
                _leakedEntities.Clear ();
            }
            return false;
        }
#endif

   
        public EcsWorld (in Config cfg = default) {
        
            // entities.
            var capacity = cfg.Entities > 0 ? cfg.Entities : Config.EntitiesDefault;
            Entities = new EntityData[capacity];
            capacity = cfg.RecycledEntities > 0 ? cfg.RecycledEntities : Config.RecycledEntitiesDefault;
            _recycledEntities = new int[capacity];
            _entitiesCount = 0;
            _recycledEntitiesCount = 0;
            // pools.
            capacity = cfg.Pools > 0 ? cfg.Pools : Config.PoolsDefault;
            _pools = new List<IEcsPool>();
            _pools.Resize(capacity, null);
            _poolHashes = new Dictionary<Type, IEcsPool> (capacity);
            _filtersByIncludedComponents = new List<EcsFilter>[capacity];
            _filtersByExcludedComponents = new List<EcsFilter>[capacity];

            _poolDenseSize = cfg.PoolDenseSize > 0 ? cfg.PoolDenseSize : Config.PoolDenseSizeDefault;
            _poolRecycledSize = cfg.PoolRecycledSize > 0 ? cfg.PoolRecycledSize : Config.PoolRecycledSizeDefault;
            _poolsCount = 0;
            // filters.
            capacity = cfg.Filters > 0 ? cfg.Filters : Config.FiltersDefault;
            _filterHashes = new Dictionary<int, EcsFilter> (capacity);
            _allFilters = new EcsFilter[capacity];
            _allFiltersCount = 0;
            // masks.
            _recycledMasks = new EcsMask[64];
            _recycledFilters = new Dictionary<EcsMask, EcsFilter>();
            _recycledMasksCount = 0;
          
            _destroyed = false;

            var singleton = NewEntity();
            this.CreateComponent<SingletonComponent>(singleton);
            Bro.Log.Assert(singleton == Singleton, "singleton have to be 0");
            
            ComponentsPrecompile.Precompile(this);
        }
        
        

        private void RecycleFilter(EcsFilter filter)
        {
            var mask = filter.GetMask();
            filter.Clear();
            
            Recycle(mask);
            
            _recycledFilters[mask] = filter;
        }

        public void Recycle(EcsMask ecsMask)
        {
            ecsMask.Reset ();
            if (_recycledMasksCount == _recycledMasks.Length) 
            {
                Array.Resize (ref _recycledMasks, _recycledMasksCount << 1);
            }
            _recycledMasks[_recycledMasksCount++] = ecsMask;
        }


        public void Destroy () {
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (CheckForLeakedEntities ()) { throw new Exception ($"Empty entity detected before EcsWorld.Destroy()."); }
#endif
            _destroyed = true;
            for (var i = _entitiesCount - 1; i >= 0; i--) {
                ref var entityData = ref Entities[i];
                if (entityData.ComponentsCount > 0) {
                    DelEntity (i);
                }
            }
            _pools.Clear();
            _recycledPools.Clear();
            _poolHashes.Clear ();
            _filterHashes.Clear ();
            _recycledFilters.Clear();
            _allFilters = Array.Empty<EcsFilter>();
            _filtersByIncludedComponents = Array.Empty<List<EcsFilter>>();
            _filtersByExcludedComponents = Array.Empty<List<EcsFilter>>();
#if ECS_DEBUG || LEOECSLITE_WORLD_EVENTS
            for (var ii = _eventListeners.Count - 1; ii >= 0; ii--) {
                _eventListeners[ii].OnWorldDestroyed (this);
            }
#endif
        }
        
       
        public void Clear() {
            for (var i = _entitiesCount - 1; i >= 0; i--) {
                ref var entityData = ref Entities[i];
                if (entityData.ComponentsCount > 0) {
                    DelEntity (i);
                }
                entityData.Gen = 0;
                entityData.ComponentsCount = 0;
            }
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (CheckForLeakedEntities ()) { throw new Exception ($"Empty entity detected before EcsWorld.Clear()"); }
#endif
            _entitiesCount = 0;
            _recycledEntitiesCount = 0;
            
            foreach (var poolPair in _recycledPools)
            {
                poolPair.Value.Clear();
            }
            foreach (var poolPair in _poolHashes)
            {
                poolPair.Value.Clear();
            }

            for(int i=0; i<_allFiltersCount; i++)
            {
                RecycleFilter(_allFilters[i]);
            }

            for (int i = 0; i < _allFiltersCount; i++)
            {
                _allFilters[i] = null;
            }
            _allFiltersCount = 0;
            _filterHashes.Clear();

            for (int i = 0, iMax = _filtersByIncludedComponents.Length; i < iMax; i++)
            {
                _filtersByIncludedComponents[i] = null;
                _filtersByExcludedComponents[i] = null;
            }

#if ECS_DEBUG || LEOECSLITE_WORLD_EVENTS
            _eventListeners.Clear();
#endif
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool IsAlive () {
            return !_destroyed;
        }

        public int NewEntity () {
            int entity;
            if (_recycledEntitiesCount > 0) {
                entity = _recycledEntities[--_recycledEntitiesCount];
                ref var entityData = ref Entities[entity];
                entityData.Gen = (short) -entityData.Gen;
            } else {
                // new entity.
                if (_entitiesCount == Entities.Length)
                {
                    // resize entities and component pools.
                    var newSize = _entitiesCount << 1;
                    ResizeEntity(newSize);
                }

                entity = _entitiesCount++;
                if (entity > Config.WarningEntitiesCount)
                {
                    Bro.Log.Error("warning! ecs world:: too many entities used");
                }
                Entities[entity].Gen = 1;
            }
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            _leakedEntities.Add (entity);
#endif
#if ECS_DEBUG || LEOECSLITE_WORLD_EVENTS
            for (int ii = 0, iMax = _eventListeners.Count; ii < iMax; ii++) {
                _eventListeners[ii].OnEntityCreated (entity);
            }
#endif
            return entity;
        }


        private void ResizeEntity(int newSize)
        { 
            Bro.Log.Assert(Entities.Length < newSize, "size have to be greater");
            Array.Resize (ref Entities, newSize);
            for (int i = 0, iMax = _poolsCount; i < iMax; i++) {
                _pools[i].Resize (newSize);
            }
            for (int i = 0, iMax = _allFiltersCount; i < iMax; i++) {
                _allFilters[i].ResizeSparseIndex (newSize);
            }
#if ECS_DEBUG || LEOECSLITE_WORLD_EVENTS
            for (int ii = 0, iMax = _eventListeners.Count; ii < iMax; ii++) {
                _eventListeners[ii].OnWorldResized (newSize);
            }
#endif
        }

        public void DelEntity (int entity) 
        {
            
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (entity < 0 || entity >= _entitiesCount) { throw new Exception ("Cant touch destroyed entity."); }
#endif
            ref var entityData = ref Entities[entity];
            if (entityData.Gen < 0) {
                return;
            }
            // kill components.
            if (entityData.ComponentsCount > 0) {
                var idx = 0;
                while (entityData.ComponentsCount > 0 && idx < _poolsCount) {
                    for (; idx < _poolsCount; idx++) {
                        if (_pools[idx].Has (entity)) {
                            _pools[idx++].Del (entity);
                            break;
                        }
                    }
                }
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                if (entityData.ComponentsCount != 0) { throw new Exception ($"Invalid components count on entity {entity} => {entityData.ComponentsCount}."); }
#endif
                return;
            }
            entityData.Gen = (short) (entityData.Gen == short.MaxValue ? -1 : -(entityData.Gen + 1));
            if (_recycledEntitiesCount == _recycledEntities.Length) {
                Array.Resize (ref _recycledEntities, _recycledEntitiesCount << 1);
            }
            _recycledEntities[_recycledEntitiesCount++] = entity;
#if ECS_DEBUG || LEOECSLITE_WORLD_EVENTS
            for (int ii = 0, iMax = _eventListeners.Count; ii < iMax; ii++) {
                _eventListeners[ii].OnEntityDestroyed (entity);
            }
#endif
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public int GetComponentsCount (int entity) {
            return Entities[entity].ComponentsCount;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public short GetEntityGen (int entity) {
            return Entities[entity].Gen;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public int GetAllocatedEntitiesCount () {
            return _entitiesCount;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public int GetWorldSize () {
            return Entities.Length;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public EntityData[] GetRawEntities () {
            return Entities;
        }

        public void RegisterPool(Type type) // todo по красоте
        {
            if (_poolHashes.TryGetValue (type, out var rawPool)) {
                return;
            }
            // var pool = (IEcsPool)Activator.CreateInstance(type, new object[] { this, _poolsCount, _poolDenseSize, Entities.Length, _poolRecycledSize });
            
            
            Type openType = typeof(EcsPool<>);
            Type[] tArgs = { type };
            Type target = openType.MakeGenericType(tArgs);
            var pool  = (IEcsPool)Activator.CreateInstance(target, new object[] { this, _poolsCount, _poolDenseSize, Entities.Length, _poolRecycledSize });

            
            // var pool = new EcsPool<T> (this, _poolsCount, _poolDenseSize, Entities.Length, _poolRecycledSize);
            _poolHashes[type] = pool;
            if (_poolsCount == _pools.Count) {
                var newSize = _poolsCount << 1;
                _pools.Resize(newSize,null);
                Array.Resize (ref _filtersByIncludedComponents, newSize);
                Array.Resize (ref _filtersByExcludedComponents, newSize);
            }
            _pools[_poolsCount++] = pool;
        }

        public EcsPool<T> GetPool<T> () where T : struct { // todo по красоте - обьеденить с выше
            var poolType = typeof (T);
            if (_poolHashes.TryGetValue (poolType, out var rawPool)) {
                return (EcsPool<T>) rawPool;
            }
            
            
            // Bro.Log.Error("!!! new pool " + poolType + " precompile it");
            var pool = new EcsPool<T> (this, _poolsCount, _poolDenseSize, Entities.Length, _poolRecycledSize);
            _poolHashes[poolType] = pool;
            if (_poolsCount == _pools.Count) {
                var newSize = _poolsCount << 1;
                _pools.Resize(newSize,null);
                Array.Resize (ref _filtersByIncludedComponents, newSize);
                Array.Resize (ref _filtersByExcludedComponents, newSize);
            }
            _pools[_poolsCount++] = pool;
            return pool;
        }


        
        public IEcsPool GetPool (Type poolType) 
        {
            if (_poolHashes.TryGetValue (poolType, out var rawPool)) 
            {
                return  rawPool;
            }
            
            var poolGenericType = typeof(EcsPool<>).MakeGenericType(poolType);
            var o = Activator.CreateInstance(poolGenericType, new object[] { this, _poolsCount, _poolDenseSize, Entities.Length, _poolRecycledSize });
            var pool = (IEcsPool) o;
            _poolHashes[poolType] = pool;
            if (_poolsCount == _pools.Count) {
                var newSize = _poolsCount << 1;
                _pools.Resize(newSize,null);
                Array.Resize (ref _filtersByIncludedComponents, newSize);
                Array.Resize (ref _filtersByExcludedComponents, newSize);
            }
            _pools[_poolsCount++] = pool;
            return pool;
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public IEcsPool GetPoolById (int typeId) {
            return typeId >= 0 && typeId < _poolsCount ? _pools[typeId] : null;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public IEcsPool GetPoolByType (Type type) {
            return _poolHashes.TryGetValue (type, out var pool) ? pool : null;
        }

        public int GetAllEntities (ref int[] entities) {
            var count = _entitiesCount - _recycledEntitiesCount;
            if (entities == null || entities.Length < count) {
                entities = new int[count];
            }
            var id = 0;
            for (int i = 0, iMax = _entitiesCount; i < iMax; i++) {
                ref var entityData = ref Entities[i];
                // should we skip empty entities here?
                if (entityData.Gen > 0 && entityData.ComponentsCount >= 0) {
                    entities[id++] = i;
                }
            }
            return count;
        }

        public int GetAllPools (ref IEcsPool[] pools) {
            var count = _poolsCount;
            if (pools == null || pools.Length < count) {
                pools = new IEcsPool[count];
            }
            for (int i = 0; i < count; i++)
            {
                pools[i] = _pools[i];
            }
            return _poolsCount;
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public EcsMask Filter<T> () where T : struct 
        {
            var mask = _recycledMasksCount > 0 ? _recycledMasks[--_recycledMasksCount] : new EcsMask (this);
            return mask.Inc<T>();
        }

        public int GetComponents (int entity, ref object[] list) {
            var itemsCount = Entities[entity].ComponentsCount;
            if (itemsCount == 0) { return 0; }
            if (list == null || list.Length < itemsCount) {
                list = new object[_pools.Count];
            }
            for (int i = 0, j = 0, iMax = _poolsCount; i < iMax; i++) {
                if (_pools[i].Has (entity)) {
                    list[j++] = _pools[i].GetRaw (entity);
                }
            }
            return itemsCount;
        }

        public int GetComponentTypes (int entity, ref Type[] list) {
            var itemsCount = Entities[entity].ComponentsCount;
            if (itemsCount == 0) { return 0; }
            if (list == null || list.Length < itemsCount) {
                list = new Type[_pools.Count];
            }
            for (int i = 0, j = 0, iMax = _poolsCount; i < iMax; i++) {
                if (_pools[i].Has (entity)) {
                    list[j++] = _pools[i].GetComponentType ();
                }
            }
            return itemsCount;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        internal bool IsEntityAliveInternal (int entity) {
            return entity >= 0 && entity < _entitiesCount && Entities[entity].Gen > 0;
        }

        public (EcsFilter, bool) GetFilterInternal (EcsMask ecsMask, int capacity = 512) 
        {
            var hash = ecsMask.Hash;
            var exists = _filterHashes.TryGetValue (hash, out var filter);
            if (exists)
            {
                return (filter, false);
            }
            if (!_recycledFilters.TryGetValue(ecsMask, out filter))
            {
                filter = new EcsFilter (this, ecsMask, capacity, Entities.Length);
            }
            else
            {
                _recycledFilters.Remove(ecsMask);
            }
            
            _filterHashes[hash] = filter;
            
            if (_allFiltersCount == _allFilters.Length) 
            {
                Array.Resize (ref _allFilters, _allFiltersCount << 1);
            }
            _allFilters[_allFiltersCount++] = filter;
            // add to component dictionaries for fast compatibility scan.
            for (int i = 0, iMax = ecsMask.IncludeCount; i < iMax; i++) 
            {
                var list = _filtersByIncludedComponents[ecsMask.Include[i]];
                if (list == null) 
                {
                    list = new List<EcsFilter> (8);
                    _filtersByIncludedComponents[ecsMask.Include[i]] = list;
                }
                list.Add (filter);
            }
            
            for (int i = 0, iMax = ecsMask.ExcludeCount; i < iMax; i++) 
            {
                var list = _filtersByExcludedComponents[ecsMask.Exclude[i]];
                if (list == null) 
                {
                    list = new List<EcsFilter> (8);
                    _filtersByExcludedComponents[ecsMask.Exclude[i]] = list;
                }
                list.Add (filter);
            }

            // scan exist entities for compatibility with new filter.
            for (int i = 0, iMax = _entitiesCount; i < iMax; i++) 
            {
                ref var entityData = ref Entities[i];
                if (entityData.ComponentsCount > 0 && IsMaskCompatible (ecsMask, i)) 
                {
                    filter.AddEntity (i);
                }
            }

            return (filter, true);
        }

        public void OnEntityChangeInternal (int entity, int componentType, bool added) 
        {
            var includeList = _filtersByIncludedComponents[componentType];
            var excludeList = _filtersByExcludedComponents[componentType];
            if (added) 
            {
                // add component.
                if (includeList != null) {
                    foreach (var filter in includeList) {
                        if (IsMaskCompatible (filter.GetMask (), entity)) 
                        {
                            filter.AddEntity (entity);
                        }
                    }
                }
                if (excludeList != null) {
                    foreach (var filter in excludeList) {
                        if (IsMaskCompatibleWithout (filter.GetMask (), entity, componentType)) 
                        {
                            filter.RemoveEntity (entity);
                        }
                    }
                }
            } 
            else 
            {
                // remove component.
                if (includeList != null) {
                    foreach (var filter in includeList) {
                        if (IsMaskCompatible (filter.GetMask (), entity)) {
                            filter.RemoveEntity (entity);
                        }
                    }
                }
                if (excludeList != null) {
                    foreach (var filter in excludeList) {
                        if (IsMaskCompatibleWithout (filter.GetMask (), entity, componentType)) {
                            filter.AddEntity (entity);
                        }
                    }
                }
            }
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        bool IsMaskCompatible (EcsMask filterEcsMask, int entity) {
            for (int i = 0, iMax = filterEcsMask.IncludeCount; i < iMax; i++) {
                if (!_pools[filterEcsMask.Include[i]].Has (entity)) {
                    return false;
                }
            }
            for (int i = 0, iMax = filterEcsMask.ExcludeCount; i < iMax; i++) {
                if (_pools[filterEcsMask.Exclude[i]].Has (entity)) {
                    return false;
                }
            }
            return true;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        bool IsMaskCompatibleWithout (EcsMask filterEcsMask, int entity, int componentId) {
            for (int i = 0, iMax = filterEcsMask.IncludeCount; i < iMax; i++) {
                var typeId = filterEcsMask.Include[i];
                if (typeId == componentId || !_pools[typeId].Has (entity)) {
                    return false;
                }
            }
            for (int i = 0, iMax = filterEcsMask.ExcludeCount; i < iMax; i++) {
                var typeId = filterEcsMask.Exclude[i];
                if (typeId != componentId && _pools[typeId].Has (entity)) {
                    return false;
                }
            }
            return true;
        }

        public struct Config {
            public int Entities;
            public int RecycledEntities;
            public int Pools;
            public int Filters;
            public int PoolDenseSize;
            public int PoolRecycledSize;

            internal const int EntitiesDefault = 512;
            internal const int WarningEntitiesCount = 384;
            internal const int RecycledEntitiesDefault = 512;
            internal const int PoolsDefault = 512;
            internal const int FiltersDefault = 512;
            internal const int PoolDenseSizeDefault = 512;
            internal const int PoolRecycledSizeDefault = 512;
        }


        public struct EntityData {
            public short Gen;
            public short ComponentsCount;
        }


        
      
    }

}