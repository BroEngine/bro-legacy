// ----------------------------------------------------------------------------
// The MIT License
// Lightweight ECS framework https://github.com/Leopotam/ecslite
// Copyright (c) 2021-2022 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Bro.Json;

namespace Leopotam.EcsLite
{
    public static class EcsWorldExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T CreateComponent<T>(this EcsWorld world, int entity) where T : struct
        {
            var pool = world.GetPool<T>();
            #if UNITY_EDITOR
            if (pool.Has(entity))
            {
                Bro.Log.Error("already has such component");
            }
            #endif
            return ref pool.Add(entity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetComponent<T>(this EcsWorld world, int entity) where T : struct
        {
            return ref world.GetPool<T>().Get(entity);
        }
   
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent<T>(this EcsWorld world, int entity) where T : struct
        {
            return world.GetPool<T>().Has(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveComponent<T>(this EcsWorld world, int entity) where T : struct
        {
            var pool = world.GetPool<T>();
            if (pool.Has(entity)) // remove
            {
                world.GetPool<T>().Del(entity);
            }
        }

        public static ref T GetSingleComponent<T>(this EcsWorld world, bool createIfNotExist = true) where T : struct
        {
            var filter = world.Filter<T>().End();
            foreach (var entity in filter)
            {
                return ref world.GetComponent<T>(entity);
            }

            if (createIfNotExist)
            {
                var entity = world.NewEntity();
                return ref  world.CreateComponent<T>(entity);
            }
            throw new Exception("no entity with component type = " + typeof(T));
        }    
        
        public static int GetSingleEntity<T>(this EcsWorld world) where T : struct
        {
            var filer = world.Filter<T>().End();
            foreach (var entity in filer)
            {
                return entity;
            }
            throw new Exception("no entity with component type = " + typeof(T));
        }    
        
        public static bool HasSingleComponent<T>(this EcsWorld world) where T : struct
        {
            var filer = world.Filter<T>().End();
            foreach (var entity in filer)
            {
                return true;
            }

            return false;
        }
        
        public static void RemoveSingleComponent<T>(this EcsWorld world) where T : struct
        { 
            var filer = world.Filter<T>().End();
            foreach (var entity in filer)
            {
                world.RemoveComponent<T>(entity);
            }
        }

        
        public static bool IsEntityValid(ref this EcsWorld.EntityData entityData)
        {
            return entityData.Gen > 0 && entityData.ComponentsCount >= 0;
        }
        
        
        
        
        
        
        
        
        
        
        // дебаги, todo вынести в отдельное место
        public static string ToFullString(this EcsWorld world)
        {
            int[] entities = null;
            object[] components = null;
            world.GetAllEntities(ref entities);
            var entitiesInfo = new StringBuilder();
            
            foreach (var entity in entities)
            {
                var size = world.GetComponents(entity, ref components);
                var entityString = new StringBuilder(entity + " = ");
                var jsonString = new StringBuilder();
                for (var i = 0; i < size; i++)
                {
                    var component = components[i];
                    var type = component != null ? component.GetType().Name : "NULL";
                    var json = JsonConvert.SerializeObject(component);
                    entityString.Append( type + "; ");
                    jsonString.Append( "            " + json + "\n");
                }
                entitiesInfo.Append($"{entityString}\n{jsonString}");
            }
            return $"ecs world info :: entities:\n" + entitiesInfo;
            // #endif
            return string.Empty;
        }

        public static string GetWorldDiff(string a, string b)
        {
            var c = new StringBuilder();
            using (var reader = new StringReader(a))
            {
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    if (!b.Contains(line))
                    {
                        c.AppendLine(line);
                    }
                }
            }
            
            using (var reader = new StringReader(b))
            {
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    if (!a.Contains(line))
                    {
                        c.AppendLine(line);
                    }
                }
            }

            return c.ToString();
        }

        public static string ToFullString(this EcsWorld world, object searchComponent)
        {
            int[] entities = null;
            object[] components = null;
            world.GetAllEntities(ref entities);
            var entitiesInfo = new StringBuilder();
            
            foreach (var entity in entities)
            {
                var size = world.GetComponents(entity, ref components);
                var entityString = new StringBuilder(entity + " = ");
                var jsonString = new StringBuilder();
                var print = false;
                for (var i = 0; i < size; i++)
                {
                    var component = components[i];

                    if (searchComponent == null || component.Equals(searchComponent))
                    {
                        print = true;
                    }

                    var type = component != null ? component.GetType().Name : "NULL";
                    var json = JsonConvert.SerializeObject(component);
                    entityString.Append( type + "; ");
                    jsonString.Append( "            " + json + "\n");
                }

                if (print)
                {
                    entitiesInfo.Append($"{entityString}\n{jsonString}");
                }
            }
            return $"ecs world info :: entities:\n" + entitiesInfo;
        }
    }
}