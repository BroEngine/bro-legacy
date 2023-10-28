using Leopotam.EcsLite;

namespace Bro.Ecs
{
    public abstract class EventObserverSystem<T> : IEcsRunSystem where T : struct
    {
        public abstract void HandleEvent(EcsWorld world, T component);
        
        public virtual void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var filter = world.Filter<EventComponent>().Inc<T>().End();
            foreach (var entity in filter)
            {
                HandleEvent(world, world.GetComponent<T>(entity));
            }
        }
    }
}