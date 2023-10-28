using System.Collections.Generic;

namespace Bro.Sketch.Server
{
    public class ActorStorage : Bro.StaticSingleton<ActorStorage>
    {
        private readonly object _lock = new object();
        private readonly Dictionary<int, Actor> _registeredActors = new Dictionary<int, Actor>();

        public void RegisterActor(Actor actor, int userId)
        {
            lock (_lock)
            {
                _registeredActors[userId] = actor;
            }
        }

        public void UnregisterActor(Actor actor, int userId)
        {
            lock (_lock)
            {
                if (_registeredActors.FastTryGetValue(userId, out var extActor) && extActor == actor)
                {
                    _registeredActors.Remove(userId);
                }
            }
        }

        public Actor GetActor(int userId)
        {
            lock (_lock)
            {
                _registeredActors.TryGetValue(userId, out var actor);
                return actor;
            }
        }
    }
}
