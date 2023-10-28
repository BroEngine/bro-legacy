using Leopotam.EcsLite;

namespace Bro.Ecs
{
    public class EventSystem : IEcsRunSystem
    {

        public void Run(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var filterCreated = world.Filter<EventCreatedComponent>().End();
            var filterProcessed = world.Filter<EventComponent>().End();

            foreach (var entity in filterProcessed)
            {
                world.DelEntity(entity); // ok
            }
            
            foreach (var entity in filterCreated)
            {
                world.CreateComponent<EventComponent>(entity);
                world.RemoveComponent<EventCreatedComponent>(entity);
            }
        }
    }
}