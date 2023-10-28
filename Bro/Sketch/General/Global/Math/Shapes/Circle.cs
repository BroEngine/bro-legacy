using System;
using Bro.Json;
using UnityEngine;

namespace Bro.Sketch
{
    [Serializable]
    public struct Circle : IShape
    {
        private Vector2 _centerPosition;
        private Vector2 _direction;
        private float _radius;

        private Rect _bBox;
        private bool _isBBoxDirty;

        public ShapeType ShapeType => ShapeType.Circle;

        public Vector2 CenterPosition
        {
            get => _centerPosition;
            set
            {
                _isBBoxDirty = true;
                _centerPosition = value;
            }
        }

        public Vector2 Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public float Radius
        {
            get => _radius;
            set
            {
                _isBBoxDirty = true;
                _radius = value;
            }
        }
        
        [JsonIgnore] public Vector2 Size
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [JsonIgnore] public Rect BoundingBox
        {
            get
            {
                if (_isBBoxDirty)
                {
                    _isBBoxDirty = false;
                    _bBox.position = _centerPosition - new Vector2(_radius, _radius);
                    _bBox.size = new Vector2(_radius * 2f, _radius * 2f);
                }
                return _bBox;
            }
        }

        [JsonIgnore] public Vector2[] Vertices => throw new NotImplementedException();

        [JsonIgnore] public float CutWidth => Radius * 2;

        public Circle(Vector2 centerPosition, float radius)
        {
            _centerPosition = centerPosition;
            _direction = Vector2.up;
            _radius = radius;
            _isBBoxDirty = true;
            _bBox = new Rect();
        }
        
        public bool GetIntersection(Vector2 from, Vector2 to, out Vector2 intersection)
        {
            var minX = Math.Min(from.x, to.x);
            var minY = Math.Min(from.y, to.y);
            var maxX = Math.Max(from.x, to.x);
            var maxY = Math.Max(from.y, to.y);
            var lineBBox = new Rect(minX, minY, maxX - minX, maxY - minY);

            bool result;
            if (!BoundingBox.Overlaps(lineBBox))
            {
                intersection = Vector2.zero;
                result = false;
            }
            else
            {
                Vector2 intersection1, intersection2;
                int intersectionsCount = Geometry2d.GetIntersectionWithLine(CenterPosition, Radius, from, to, out intersection1, out intersection2);
                result = intersectionsCount > 0;
                if (intersectionsCount == 0)
                {
                    intersection = Vector2.zero;
                }
                else if (intersectionsCount == 1)
                {
                    intersection = intersection1;
                }
                else //if(intersectionsCount == 2)
                {
                    if (UnityVector2Extensions.SqrDistance(from, intersection1) < UnityVector2Extensions.SqrDistance(from, intersection2))
                    {
                        intersection = intersection1;
                    }
                    else
                    {
                        intersection = intersection2;
                    }
                }
                
            }
            return result;
        }

        public bool GetNormal(Vector2 atPoint, out Vector2 normal)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps<T>(T shape) where T : struct, IShape
        {
            return ShapeOverlapping.Overlaps(this, shape);
        }

        public bool Contains(Vector2 point)
        {
            return UnityVector2Extensions.SqrDistance(point, CenterPosition) <= Radius * Radius;
        }
    }
}