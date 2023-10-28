// ----------------------------------------------------------------------------
// The MIT License
// Lightweight ECS framework https://github.com/Leopotam/ecslite
// Copyright (c) 2021-2022 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using Bro.Ecs;
using Bro.Network.TransmitProtocol;

namespace Leopotam.EcsLite {
    
    
    public interface IEcsPool
    {
        void Resize (int capacity);
        bool Has (int entity);
        void Del (int entity);
        void AddRaw (int entity, object dataRaw);
        object GetRaw (int entity);
        void SetRaw (int entity, object dataRaw);
        int GetId ();
        Type GetComponentType ();
        void Copy(IEcsPool ecsPool);
        void Copy(int toEntity, IEcsPool fromPool, int fromEntity);
        void Deserialize(IReader reader, IComponentSerializer serializer, int entity);
        void Serialize(IWriter writer, IComponentSerializer serializer, int entity);
        void Clear();
    }

    public interface IEcsAutoReset<T> where T : struct {
        void AutoReset (ref T c);
    }

#if UNITY_2018_1_OR_NEWER
        [UnityEngine.Scripting.Preserve]
#endif
    public sealed class EcsPool<T> : IEcsPool where T : struct {
        private Type _type;
        private int _id;

        private readonly EcsWorld _world;
        private readonly AutoResetHandler _autoReset;
        // 1-based index.
        private T[] _denseItems;
        private int[] _sparseItems;
        private int _denseItemsCount;
        private int[] _recycledItems;
        private int _recycledItemsCount;

        
        
        public static EcsPool<T> Create(EcsWorld world, int id, int denseCapacity, int sparseCapacity, int recycledCapacity)
        {
            return new EcsPool<T>( world,  id,  denseCapacity,  sparseCapacity,  recycledCapacity);
        }
        

        public EcsPool (EcsWorld world, int id, int denseCapacity, int sparseCapacity, int recycledCapacity) {
            _type = typeof (T);
            _world = world;
            _id = id;
            _denseItems = new T[denseCapacity + 1];
            _sparseItems = new int[sparseCapacity];
            _denseItemsCount = 1;
            _recycledItems = new int[recycledCapacity];
            _recycledItemsCount = 0;
            var isAutoReset = typeof (IEcsAutoReset<T>).IsAssignableFrom (_type);
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (!isAutoReset && _type.GetInterface ("IEcsAutoReset`1") != null) {
                throw new Exception ($"IEcsAutoReset should have <{typeof (T).Name}> constraint for component \"{typeof (T).Name}\".");
            }
#endif
            if (isAutoReset) {
                var autoResetMethod = typeof (T).GetMethod (nameof (IEcsAutoReset<T>.AutoReset));
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                if (autoResetMethod == null) {
                    throw new Exception (
                        $"IEcsAutoReset<{typeof (T).Name}> explicit implementation not supported, use implicit instead.");
                }
#endif
                _autoReset = (AutoResetHandler) Delegate.CreateDelegate (
                    typeof (AutoResetHandler),

                    null,

                    autoResetMethod);
            }
        }

        public void Clear()
        {

            for (int i = 0; i < _denseItemsCount; i++)
            {
                _denseItems[i] = default;
            }
            for (int i = 0; i < _sparseItems.Length; i++)
            {
                _sparseItems[i] = default;
            }
            _denseItemsCount = 1;
            _recycledItemsCount = 0;
        }

        public EcsPool()
        {
        }

        public EcsPool (EcsWorld world, object sourcePoolObject)
        {
            var sourcePool = (EcsPool<T>) sourcePoolObject;
            _type = typeof (T);
            _world = world;
            _id = sourcePool._id;
            _denseItems = new T[sourcePool._denseItems.Length];
            _sparseItems = new int[sourcePool._sparseItems.Length];
            _denseItemsCount = 1;
            _recycledItems = new int[sourcePool._recycledItems.Length];
            _recycledItemsCount = 0;
            var isAutoReset = typeof (IEcsAutoReset<T>).IsAssignableFrom (_type);
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (!isAutoReset && _type.GetInterface ("IEcsAutoReset`1") != null) {
                throw new Exception ($"IEcsAutoReset should have <{typeof (T).Name}> constraint for component \"{typeof (T).Name}\".");
            }
#endif
            if (isAutoReset) {
                var autoResetMethod = typeof (T).GetMethod (nameof (IEcsAutoReset<T>.AutoReset));
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                if (autoResetMethod == null) {
                    throw new Exception (
                        $"IEcsAutoReset<{typeof (T).Name}> explicit implementation not supported, use implicit instead.");
                }
#endif
                _autoReset = (AutoResetHandler) Delegate.CreateDelegate (
                    typeof (AutoResetHandler),
                    null,
                    autoResetMethod);
            }
        }
        
        public void Copy(int toEntity, IEcsPool fromPool, int fromEntity)
        {
            var inputPool = (EcsPool<T>) fromPool;
            Add(toEntity) = inputPool.Get(fromEntity);
        }

        public void Copy(IEcsPool ecsPool)
        {
            #if UNITY_EDITOR
            if(!(ecsPool is EcsPool<T>))
                throw new Exception($"invalid pool type {ecsPool.GetType()} {typeof(EcsPool<T>)}");
            #endif
            var inputPool = (EcsPool<T>) ecsPool;

            _type = inputPool._type;
            _id = inputPool._id;
//            readonly AutoResetHandler _autoReset;
            // 1-based index.
            _denseItemsCount = inputPool._denseItemsCount;
            for (int i = 0; i < inputPool._denseItemsCount; i++) {
                _denseItems[i] = inputPool._denseItems[i];
            }

            if (_sparseItems.Length != inputPool._sparseItems.Length) {
                Array.Resize (ref _sparseItems, inputPool._sparseItems.Length);
            }
            for (int i = 0; i < inputPool._sparseItems.Length; i++) {
                _sparseItems[i] = inputPool._sparseItems[i];
            }

            _recycledItemsCount = inputPool._recycledItemsCount;
            if (_recycledItems.Length < _recycledItemsCount) {
                Array.Resize (ref _recycledItems, _recycledItemsCount << 1);
            }

            for (int i = 0; i < _recycledItemsCount; i++) {
                _recycledItems[i] = inputPool._recycledItems[i];
            }
            
        }

#if UNITY_2020_3_OR_NEWER
        [UnityEngine.Scripting.Preserve]
#endif
        void ReflectionSupportHack () {
            _world.GetPool<T> ();
            _world.Filter<T> ().Exc<T> ().End ();
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public EcsWorld GetWorld () {
            return _world;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public int GetId () {
            return _id;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public Type GetComponentType () {
            return _type;
        }

        void IEcsPool.Deserialize(IReader reader, IComponentSerializer serializer, int entity)
        {
            ref var component = ref Add(entity);
            serializer.Deserialize(reader, out component);
        }
        
        void IEcsPool.Serialize(IWriter writer, IComponentSerializer serializer, int entity)
        {
            ref var component = ref Get(entity);
            serializer.Serialize(writer, ref component);
        }

        void IEcsPool.Resize (int capacity) {
            Array.Resize (ref _sparseItems, capacity);
        }

        object IEcsPool.GetRaw (int entity) {
            return Get (entity);
        }

        void IEcsPool.SetRaw (int entity, object dataRaw) {
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (dataRaw == null || dataRaw.GetType () != _type) { throw new Exception ("Invalid component data, valid \"{typeof (T).Name}\" instance required."); }
            if (_sparseItems[entity] <= 0) { throw new Exception ($"Component \"{typeof (T).Name}\" not attached to entity."); }
#endif
            _denseItems[_sparseItems[entity]] = (T) dataRaw;
        }

        void IEcsPool.AddRaw (int entity, object dataRaw) {
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (dataRaw == null || dataRaw.GetType () != _type) { throw new Exception ("Invalid component data, valid \"{typeof (T).Name}\" instance required."); }
#endif
            ref var data = ref Add (entity);
            data = (T) dataRaw;
        }

        public T[] GetRawDenseItems () {
            return _denseItems;
        }

        public ref int GetRawDenseItemsCount () {
            return ref _denseItemsCount;
        }

        public int[] GetRawSparseItems () {
            return _sparseItems;
        }

        public int[] GetRawRecycledItems () {
            return _recycledItems;
        }

        public ref int GetRawRecycledItemsCount () {
            return ref _recycledItemsCount;
        }

        public ref T Add (int entity) {
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (!_world.IsEntityAliveInternal (entity)) { throw new Exception ("Cant touch destroyed entity."); }

            if (entity >= _sparseItems.Length)
            {
                Bro.Log.Error("entity = " + entity + ", lenght = " + _sparseItems.Length);
            }

            if (_sparseItems[entity] > 0) { throw new Exception ($"Component \"{typeof (T).Name}\" already attached to entity."); }
#endif
            int idx;
            if (_recycledItemsCount > 0) {
                idx = _recycledItems[--_recycledItemsCount];
            } else {
                idx = _denseItemsCount;
                if (_denseItemsCount == _denseItems.Length) {
                    Array.Resize (ref _denseItems, _denseItemsCount << 1);
                }
                _denseItemsCount++;
                _autoReset?.Invoke (ref _denseItems[idx]);
            }
            _sparseItems[entity] = idx;
            _world.OnEntityChangeInternal (entity, _id, true);
            _world.Entities[entity].ComponentsCount++;
            return ref _denseItems[idx];
        }
    
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public ref T Get (int entity) {
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (!_world.IsEntityAliveInternal (entity)) { throw new Exception ("Cant touch destroyed entity =. " + entity); }
            if (_sparseItems[entity] == 0) { throw new Exception ($"Cant get \"{typeof (T).Name}\" component - not attached."); }
#endif
            return ref _denseItems[_sparseItems[entity]];
        }
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public bool Has (int entity) {
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (!_world.IsEntityAliveInternal (entity)) { throw new Exception ("Cant touch destroyed entity."); }
#endif
            return _sparseItems[entity] > 0;
        }

        public void Del (int entity) 
        {
            ref var sparseData = ref _sparseItems[entity];
            if (sparseData > 0) {
                _world.OnEntityChangeInternal (entity, _id, false);
                if (_recycledItemsCount == _recycledItems.Length) {
                    Array.Resize (ref _recycledItems, _recycledItemsCount << 1);
                }
                _recycledItems[_recycledItemsCount++] = sparseData;
                if (_autoReset != null) {
                    _autoReset.Invoke (ref _denseItems[sparseData]);
                } else {
                    _denseItems[sparseData] = default;
                }
                sparseData = 0;
                ref var entityData = ref _world.Entities[entity];
                entityData.ComponentsCount--;
                if (entityData.ComponentsCount == 0) {
                    _world.DelEntity (entity);
                }
            }
        }

        delegate void AutoResetHandler (ref T component);
    }
}