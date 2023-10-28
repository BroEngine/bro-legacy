using Bro.Json;
using Bro.Json.Converters;
using UnityEngine;

namespace Bro.Sketch
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ShapeType
    {
        [JsonProperty("undefined")] Undefined = 0,
        [JsonProperty("line")] Line = 1,
        [JsonProperty("circle")] Circle = 2,
        [JsonProperty("rect")]  Rectangle = 3,
        [JsonProperty("poly")] Polygon = 4,
    }
    
    public interface IShape
    {
        ShapeType ShapeType { get; }
        Vector2 CenterPosition { get; set; }
        Vector2 Direction { get; set; }
        Vector2 Size { get; set; }
        Rect BoundingBox { get; }
        Vector2[] Vertices { get; }
        float CutWidth { get; }
        bool GetIntersection(Vector2 from, Vector2 to, out Vector2 intersection);
        bool GetNormal(Vector2 atPoint, out Vector2 normal);
        bool Overlaps<T>(T shape) where T : struct, IShape;
        bool Contains(Vector2 point);
    }
}