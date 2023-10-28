using UnityEngine;

namespace Bro.Ecs
{
    [SerializationComponent]
    public struct LineShapeComponent
    {
        public Vector2 StartPoint;
        public Vector2 EndPoint;
    }
}