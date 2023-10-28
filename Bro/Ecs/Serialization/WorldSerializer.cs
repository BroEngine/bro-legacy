using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Bro.Network.TransmitProtocol;
using Leopotam.EcsLite;

namespace Bro.Ecs
{
    public class WorldSerializer : IComponentSerializer, IDisposable
    {
        private readonly Dictionary<int,IntPtr> _ptrCache = new Dictionary<int, IntPtr>();
        private readonly byte[] _buffer = new byte[32 * 1024];
        private readonly BinaryCacheReader _reader = new BinaryCacheReader();
        private readonly int _defaultCapacity =  EcsWorld.Config.EntitiesDefault;
        private IEcsPool[] _pools;

        public WorldSerializer()
        {
            _pools = new IEcsPool[_defaultCapacity];
        }
        
        private IntPtr GetPtr(int size)
        {
            if(!_ptrCache.ContainsKey(size))
            {
                _ptrCache[size] = Marshal.AllocHGlobal(size);
            }
            return _ptrCache[size];
        }

        public void Dispose()
        {
            foreach (var item in _ptrCache)
            {
                Marshal.FreeHGlobal(item.Value);
            }
            _ptrCache.Clear();
        }
        
        public int SerializeWorld(EcsWorld world, byte[] dst)
        {
            using (IWriter writer = new BinaryCacheWriter(dst))
            {
                var poolsCount = world.GetAllPools(ref _pools);
                var rawEntitiesCount = world.GetAllocatedEntitiesCount();
                var rawEntities = world.GetRawEntities();
                var validEntitiesCount = 0;
                var entityCountPosition = writer.Position;
                writer.Write(0);
                
                for (var entity = 0; entity < rawEntitiesCount; entity++)
                {
                    ref var entityData = ref rawEntities[entity];
                    var componentsCount = SerializableComponentsCount(ref entityData, entity, ref _pools, poolsCount);
                    
                    if (componentsCount > 0)
                    {
                        // var componentsCount = entityData.ComponentsCount;
                        if (componentsCount > byte.MaxValue)
                        {
                            throw new Exception($"cannot serialize entity with more than {byte.MaxValue} components");
                        }
                        writer.Write((byte) componentsCount);
          
                        for (int poolIndex = 0; poolIndex < poolsCount; poolIndex++)
                        {
                            if (_pools[poolIndex].Has(entity))
                            {
                                var componentType = _pools[poolIndex].GetComponentType();
                                if (ComponentsRegistry.IsRegistered(componentType))
                                {
                                    var componentTypeIndex = ComponentsRegistry.GetTypeCode(componentType);
                                    writer.Write(componentTypeIndex);
                                    _pools[poolIndex].Serialize(writer, this, entity);
                                }
                            }
                        }

                        ++validEntitiesCount;
                    }
                }
                var finalPosition = (int)writer.Position;
                writer.Position = entityCountPosition;
                writer.Write(validEntitiesCount);
                writer.Position = finalPosition;
                return finalPosition;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SerializableComponentsCount(ref EcsWorld.EntityData data, int entity, ref IEcsPool[] pools, int poolSize)
        {
            if (!data.IsEntityValid())
            {
                return 0;
            }

            var count = 0;
            for (var poolIndex = 0; poolIndex < poolSize; poolIndex++)
            {
                if (pools[poolIndex].Has(entity))
                {
                    var componentType = pools[poolIndex].GetComponentType();
                    if (ComponentsRegistry.IsRegistered(componentType))
                    {
                        ++count;
                    }
                }
            }

            return count;
        }

     
        
        public void DeserializeWorld(EcsWorld world, byte[] src, int size)
        {
            _reader.Write(src, size);
            _reader.Read(out int entitiesCount);
            for (var i = 0; i < entitiesCount; i++)
            {
                DeserializeEntity(_reader, world);
            }
        }
           
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private void DeserializeEntity(IReader reader, EcsWorld world)
        {
            reader.Read(out byte componentsCount);
            if (componentsCount != 0)
            {

                var newEntity = world.NewEntity();
                for (var component = 0; component < componentsCount; ++component)
                {
                    reader.Read(out byte componentIndex);
                    var type = ComponentsRegistry.GetType(componentIndex);
                    var pool = world.GetPool(type);
                    pool.Deserialize(reader, this, newEntity);
                }
            }
        }
        
        private byte[] Serialize<T>(T s) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var array = new byte[size];
            var ptr = GetPtr(size);
            Marshal.StructureToPtr(s, ptr, true);
            Marshal.Copy(ptr, array, 0, size);
            return array;
        }
        
        void IComponentSerializer.Serialize<T>(IWriter writer, ref T component)
        {
            writer.Write(Serialize(component));
        }

        private T Deserialize<T>(byte[] array)  where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var ptr = GetPtr(size);
            Marshal.Copy(array, 0, ptr, size);
            var s = Marshal.PtrToStructure<T>(ptr);
            return s;
        }
        
        void IComponentSerializer.Deserialize<T>(IReader reader, out T component) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            reader.Read(_buffer, size);
            try
            {
                component = Deserialize<T>(_buffer);
            }
            catch (Exception e)
            {
                Bro.Log.Error("" + typeof(T) + "   " + e.ToString());
                component = default(T);
            }
        }
    }
}