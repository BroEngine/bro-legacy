using System;
using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Bro.Ecs
{
    /* тут bro компоенеты, в разрезе игры добавляйте в ComponentsPrecompile в неймспейсе игры */
    public static class ComponentsPrecompile 
    {
        private static readonly List<Type> RegisteredTypes = new List<Type>();
        
        public static void Register<T>() where T : struct
        {
            RegisteredTypes.Add(typeof(T));
        }
        
        public static void Initialize()
        {
            Register<SingletonComponent>();
            Register<IdComponent>();
            Register<OwnerComponent>();
            Register<TransformComponent>();
            Register<SpeedComponent>();
            Register<EventComponent>();
            Register<EventCreatedComponent>();
            Register<CircleShapeComponent>();
            Register<LineShapeComponent>();
            Register<RectangleShapeComponent>();
            Register<FrameIndexComponent>();
        }

        public static void Precompile(EcsWorld ecsWorld)
        {
            var types = ComponentsRegistry.GetSerializationTypes;

            foreach (var type in types)
            {
                if (RegisteredTypes.Contains(type))
                {
                    ecsWorld.RegisterPool(type);
                }
                else
                {
                    Bro.Log.Error("register type = " + type + " in Precompile.cs" );
                }
            }
            
            types = ComponentsRegistry.GetRegularTypes;

            foreach (var type in types)
            {
                if (RegisteredTypes.Contains(type))
                {
                    ecsWorld.RegisterPool(type);
                }
                else
                {
                    Bro.Log.Error("register type = " + type + " in ComponentsPrecompile.cs" );
                }
            }
        }
    }
}