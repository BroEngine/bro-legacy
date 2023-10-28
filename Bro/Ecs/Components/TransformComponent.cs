using UnityEngine;

namespace Bro.Ecs
{
    [SerializationComponent]
    public struct TransformComponent
    {
        public Vector2 Position;
        public Vector2 Direction;
    }
}