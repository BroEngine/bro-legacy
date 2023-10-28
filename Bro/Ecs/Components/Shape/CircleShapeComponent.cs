using UnityEngine;

namespace Bro.Ecs
{
    [SerializationComponent]
    public struct CircleShapeComponent
    {
        public Vector2 CenterPosition; // meh.....
        public float Radius;
    }
}