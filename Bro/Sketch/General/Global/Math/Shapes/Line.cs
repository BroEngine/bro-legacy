using System;
using UnityEngine;

namespace Bro.Sketch
{
    [Serializable]
    public struct Line : IShape
    {
        public Vector2 StartPoint;
        public Vector2 EndPoint;
        
        public Vector2 CenterPosition
        {
            get
            {
                return (StartPoint + EndPoint) * 0.5f;
            }
            set
            {
                var translate = (value - CenterPosition);
                StartPoint += translate;
                EndPoint += translate;
            }
        }

        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.Line;
            }
        }
        
        public Vector2 Direction
        {
            get => (StartPoint - EndPoint).normalized;
            set => throw new NotImplementedException("");
        }
        
        public Vector2 Size
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public Rect BoundingBox
        {
            get
            {
                var minX = Math.Min(StartPoint.x, EndPoint.x);
                var minY = Math.Min(StartPoint.y, EndPoint.y);
                var maxX = Math.Max(StartPoint.x, EndPoint.x);
                var maxY = Math.Max(StartPoint.y, EndPoint.y);
                return new Rect(minX, minY, maxX - minX, maxY - minY);
            }
        }

        public Vector2[] Vertices => throw new NotImplementedException("");

        public float CutWidth => throw new NotImplementedException("");

        public bool GetIntersection(Vector2 @from, Vector2 to, out Vector2 intersection)
        {
            throw new NotImplementedException();
        }

        public bool GetNormal(Vector2 atPoint, out Vector2 normal)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps<T>(IShape shape) where T : struct, IShape
        {
            throw new NotImplementedException();
        }

        public Line(Vector2 startPoint, Vector2 endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public bool Overlaps<T>(T shape) where T : struct, IShape
        {
            return ShapeOverlapping.Overlaps(this, shape);
        }

        public bool Contains(Vector2 point)
        {
            throw new NotImplementedException();
        }
    }
}