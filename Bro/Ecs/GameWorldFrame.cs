using Leopotam.EcsLite;

namespace Bro.Ecs.Network
{
    public class GameWorldFrame
    {
        public readonly EcsWorld World;
        public int Frame;
        
        public GameWorldFrame(EcsWorld world, int frame)
        {
            World = world;
            Frame = frame;
        }
    }
}