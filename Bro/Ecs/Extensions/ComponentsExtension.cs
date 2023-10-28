using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using UnityEngine;

namespace Bro.Ecs
{
    public static class ComponentsExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetIterationIndex(this EcsWorld world)
        {
            return world.GetSingleComponent<FrameIndexComponent>().Index;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetEntityById(this EcsWorld world, int id, out int entity)
        {
            var pool = world.GetPool<IdComponent>();
            var filter = world.Filter<IdComponent>().End();
            foreach (var candidate in filter)
            {
                if (pool.Get(candidate).Id == id)
                {
                    entity = candidate;
                    return true;
                }
            }
            
            entity = 0;
            return false;
        }
        
        public static void CopyComponent<T>(this EcsWorld world, int from, int to) where T : struct
        {
            if (world.HasComponent<T>(from))
            {
                world.CreateComponent<T>(to) = world.GetComponent<T>(from);
            }
        }  
        
        public static void CopyComponent<T>(this EcsWorld fromWorld, int from, EcsWorld toWorld, int to) where T : struct
        {
            if (fromWorld.HasComponent<T>(from))
            {
                toWorld.CreateComponent<T>(to) = fromWorld.GetComponent<T>(from);
            }
        }
        
        public static void AddId(this EcsWorld world, int entity, int id)
        {
            world.CreateComponent<IdComponent>(entity).Id = id;
        }   
        
        public static int GetId(this EcsWorld world, int entity)
        {
            return world.GetComponent<IdComponent>(entity).Id;
        } 
        
        public static bool HasId(this EcsWorld world, int entity)
        {
            return world.HasComponent<IdComponent>(entity);
        }
        
        public static void AddOwner(this EcsWorld world, int entity, short owner)
        {
            world.CreateComponent<OwnerComponent>(entity).OwnerId = owner;
        }
        
        public static short GetOwnerId(this EcsWorld world, int entity)
        {
            return world.GetComponent<OwnerComponent>(entity).OwnerId;
        }
        
        public static bool HasOwnerId(this EcsWorld world, int entity)
        {
            return world.HasComponent<OwnerComponent>(entity);
        }
        
        public static void AddTransform(this EcsWorld world, int entity, Vector2 position)
        {
            ref var component = ref world.CreateComponent<TransformComponent>(entity);
            component.Position = position;
        }   
        
        public static bool HasTransform(this EcsWorld world, int entity)
        {
            return world.HasComponent<TransformComponent>(entity);
        }   
             
        public static Vector2 GetPosition(this EcsWorld world, int entity)
        {
            return world.GetComponent<TransformComponent>(entity).Position;
        }    
                 
        public static void SetPosition(this EcsWorld world, int entity, Vector2 position)
        {
            world.GetComponent<TransformComponent>(entity).Position = position;
        }    
        
        public static Vector2 GetDirection(this EcsWorld world, int entity)
        {
            return world.GetComponent<TransformComponent>(entity).Direction;
        }   
        
        public static void SetDirection(this EcsWorld world, int entity, Vector2 direction)
        {
            world.GetComponent<TransformComponent>(entity).Direction = direction;
        }   
        
        public static void AddTransform(this EcsWorld world, int entity, Vector2 position, Vector2 direction)
        {
            ref var component = ref world.CreateComponent<TransformComponent>(entity);
            component.Position = position;
            component.Direction = direction;
        }
    }
}