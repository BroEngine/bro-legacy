// ----------------------------------------------------------------------------
// The MIT License
// Lightweight ECS framework https://github.com/Leopotam/ecslite
// Copyright (c) 2021-2022 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace Leopotam.EcsLite 
{
    public sealed class EcsFilter 
    {
        readonly EcsWorld _world;
        readonly EcsMask _ecsMask;
        int[] _denseEntities;
        int _entitiesCount;
        internal int[] SparseEntities;
        int _lockCount;
        DelayedOp[] _delayedOps;
        int _delayedOpsCount;
        
        internal EcsFilter (EcsWorld world, EcsMask ecsMask, int denseCapacity, int sparseCapacity) {
            _world = world;
            _ecsMask = ecsMask;
            _denseEntities = new int[denseCapacity];
            SparseEntities = new int[sparseCapacity];
            _entitiesCount = 0;
            _delayedOps = new DelayedOp[512];
            _delayedOpsCount = 0;
            _lockCount = 0;
        }
        
        public void Clear()
        {
            _delayedOpsCount = 0;
            _lockCount = 0;
            _entitiesCount = 0;
            for (int i = 0; i < _denseEntities.Length; i++)
            {
                _denseEntities[i] = 0;
            }
            for (int i = 0; i < SparseEntities.Length; i++)
            {
                SparseEntities[i] = 0;
            }

        }

        internal void Set(EcsFilter sourceFilter)
        {
            _ecsMask.Set(sourceFilter._ecsMask);
            if (sourceFilter._entitiesCount > _denseEntities.Length) {
                Array.Resize (ref _denseEntities, sourceFilter._entitiesCount);
            }
            for (int i = 0, iMax = sourceFilter._entitiesCount; i < iMax; i++) {
                _denseEntities[i] = sourceFilter._denseEntities[i];
            }
            _entitiesCount = sourceFilter._entitiesCount;
            
            if (sourceFilter.SparseEntities.Length > SparseEntities.Length)
                Array.Resize(ref SparseEntities, sourceFilter.SparseEntities.Length);
            for (int i = 0, iMax = sourceFilter.SparseEntities.Length; i < iMax; i++) {
                SparseEntities[i] = sourceFilter.SparseEntities[i];
            }

            if (sourceFilter._delayedOpsCount > _delayedOps.Length) {
                Array.Resize (ref _delayedOps, sourceFilter._delayedOpsCount);
            }
            for (int i = 0, iMax = sourceFilter._delayedOpsCount; i < iMax; i++) {
                _delayedOps[i] = sourceFilter._delayedOps[i];
            }
            _delayedOpsCount = sourceFilter._delayedOpsCount;
            _lockCount = sourceFilter._lockCount;

        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public EcsWorld GetWorld () {
            return _world;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public int GetEntitiesCount () {
            return _entitiesCount;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public int[] GetRawEntities () {
            return _denseEntities;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public int[] GetSparseIndex () {
            return SparseEntities;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator () {
            _lockCount++;
            return new Enumerator (this);
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        internal void ResizeSparseIndex (int capacity) {
            Array.Resize (ref SparseEntities, capacity);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        internal EcsMask GetMask () {
            return _ecsMask;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        internal void AddEntity (int entity) 
        {
            if (AddDelayedOp (true, entity)) { return; }
            if (_entitiesCount == _denseEntities.Length) 
            {
                Array.Resize (ref _denseEntities, _entitiesCount << 1);
            }
            _denseEntities[_entitiesCount++] = entity;
            SparseEntities[entity] = _entitiesCount;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        internal void RemoveEntity (int entity) 
        {
            if (AddDelayedOp (false, entity)) { return; }
            var idx = SparseEntities[entity] - 1;
            SparseEntities[entity] = 0;
            _entitiesCount--;
            if (idx < _entitiesCount) {
                _denseEntities[idx] = _denseEntities[_entitiesCount];
                SparseEntities[_denseEntities[idx]] = idx + 1;
            }
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        bool AddDelayedOp (bool added, int entity) {
            if (_lockCount <= 0) { return false; }
            if (_delayedOpsCount == _delayedOps.Length) {
                Array.Resize (ref _delayedOps, _delayedOpsCount << 1);
            }
            ref var op = ref _delayedOps[_delayedOpsCount++];
            op.Added = added;
            op.Entity = entity;
            return true;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        void Unlock () 
        {
            _lockCount--;
            if (_lockCount == 0 && _delayedOpsCount > 0) {
                for (int i = 0, iMax = _delayedOpsCount; i < iMax; i++) {
                    ref var op = ref _delayedOps[i];
                    if (op.Added) {
                        AddEntity (op.Entity);
                    } else {
                        RemoveEntity (op.Entity);
                    }
                }
                _delayedOpsCount = 0;
            }
        }
        
        public struct Enumerator : IDisposable 
        {
            readonly EcsFilter _filter;
            readonly int[] _entities;
            readonly int _count;
            int _idx;

            public Enumerator (EcsFilter filter) {
                _filter = filter;
                _entities = filter._denseEntities;
                _count = filter._entitiesCount;
                _idx = -1;
            }

            public int Current {
                [MethodImpl (MethodImplOptions.AggressiveInlining)]
                get => _entities[_idx];
            }

            [MethodImpl (MethodImplOptions.AggressiveInlining)]
            public bool MoveNext () {
                return ++_idx < _count;
            }

            [MethodImpl (MethodImplOptions.AggressiveInlining)]
            public void Dispose () {
                _filter.Unlock ();
            }
        }

        struct DelayedOp {
            public bool Added;
            public int Entity;
        }
    }
}