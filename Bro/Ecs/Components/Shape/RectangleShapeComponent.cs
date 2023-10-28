using UnityEngine;

namespace Bro.Ecs
{
    [SerializationComponent]
    public struct RectangleShapeComponent
    {
        public Vector2 CenterPosition;
        public Vector2 Size;
        public Vector2 Direction;
    }
}