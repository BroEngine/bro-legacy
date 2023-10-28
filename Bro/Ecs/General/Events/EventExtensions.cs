using Leopotam.EcsLite;

namespace Bro.Ecs
{
    public static class EventExtensions
    {
        public static ref T NewEvent<T>(this EcsWorld world) where T : struct
        {
            var entity = world.NewEntity();
            world.CreateComponent<EventCreatedComponent>(entity);
            return ref world.CreateComponent<T>(entity);
        }
    }
}