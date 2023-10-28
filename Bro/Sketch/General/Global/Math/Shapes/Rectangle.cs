using System;
using System.Runtime.InteropServices;
using Bro.Json;
using UnityEngine;

namespace Bro.Sketch
{
    [Serializable]
    public struct Rectangle : IShape
    {
        private Vector2 _centerPosition;
        private Vector2 _size;
        private Vector2 _dir;
        
        private Rect _bBox;
        private bool _isBBoxDirty;
        
        private bool _isVerticesDirty;

        private Vector2 _topLeft;
        private Vector2 _topRight;
        private Vector2 _bottomLeft;
        private Vector2 _bottomRight;

        public Vector2 Size
        {
            get => _size;
            set
            {
                _isBBoxDirty = true;
                _isVerticesDirty = true;
                _size = value;
            }
        }

        public ShapeType ShapeType => ShapeType.Rectangle;

        public Vector2 CenterPosition
        {
            get => _centerPosition;
            set
            {
                _isBBoxDirty = true;
                _isVerticesDirty = true;
                _centerPosition = value;
            }
        }

        public Vector2 Direction
        {
            get => _dir;
            set
            {
                if (value.Equals(Vector2.zero))
                {
                    throw new ArgumentOutOfRangeException("Direction cannot have zero lenght");
                }
                _isBBoxDirty = true;
                _isVerticesDirty = true;
                _dir = value.normalized;
            }
        }

        [JsonIgnore] public Rect BoundingBox
        {
            get
            {
                if (_isBBoxDirty)
                {
                    _isBBoxDirty = false;
                    var position = CenterPosition;
                    var size = Size;
                    var up = Direction * size.y * 0.5f;
                    var left = Vector2.Perpendicular(Direction) * size.x * 0.5f;
                    var topLeft = position + up + left;
                    var topRight = position + up - left;
                    var bottomLeft = position - up + left;
                    var bottomRight = position - up - left;
                    var minX = Math.Min(Math.Min(topLeft.x, bottomLeft.x), Math.Min(topRight.x, bottomRight.x));
                    var maxX = Math.Max(Math.Max(topLeft.x, bottomLeft.x), Math.Max(topRight.x, bottomRight.x));
                    var minY = Math.Min(Math.Min(topLeft.y, bottomLeft.y), Math.Min(topRight.y, bottomRight.y));
                    var maxY = Math.Max(Math.Max(topLeft.y, bottomLeft.y), Math.Max(topRight.y, bottomRight.y));
                    _bBox = new Rect(minX, minY, maxX - minX, maxY - minY);
                }
                return _bBox;
            }
        }

        [JsonIgnore]public Vector2[] Vertices
        {
            get
            {
                if (_isVerticesDirty)
                {
                    _isVerticesDirty = false;

                    var up = Direction * Size.y * 0.5f;
                    var left = Direction.PerpendicularClockwise() * (Size.x * 0.5f);
                    _topLeft = CenterPosition + up - left;
                    _topRight = CenterPosition + up + left;
                    _bottomLeft = CenterPosition - up - left;
                    _bottomRight = CenterPosition - up + left;
                }
                return new Vector2[] {_bottomLeft, _bottomRight, _topRight, _topLeft}; 
            }
        }
        
        [JsonIgnore]public float CutWidth => throw new NotImplementedException();


        public Rectangle(Vector2 centerPosition, Vector2 size, Vector2 direction)
        {
            _bBox = new Rect();
            _isBBoxDirty = true;

            _centerPosition = centerPosition;
            _size = size;
            _dir = direction.normalized;
            
            _isVerticesDirty = false;
            
            var up = _dir * _size.y * 0.5f;
            var left = _dir.PerpendicularClockwise() * (_size.x * 0.5f);
            _topLeft = _centerPosition + up - left;
            _topRight = _centerPosition + up + left;
            _bottomLeft = _centerPosition - up - left;
            _bottomRight = _centerPosition - up + left;
        }

        public Rectangle(Vector2 centerPosition) : this(centerPosition, Vector2.zero, Vector2.up)
        {
            _centerPosition = centerPosition;
        }
        
        public bool GetNormal(Vector2 atPoint, out Vector2 normal)
        {
            var poly = Vertices;

            for (int i = 0, max = poly.Length; i < max; i++)
            {
                var startPoint = poly[i];
                var finishPoint = poly[(i + 1 >= max) ? 0 : i + 1];

                if (Geometry2d.IsPointInsideLineSegement(atPoint, startPoint, finishPoint))
                {
                    normal = UnityVector2Extensions.PerpendicularClockwise(finishPoint - startPoint).normalized;
                    return true;
                }
            }

            normal = Vector2.zero;
            return false;
        }

        public bool Overlaps<T>(T shape) where T : struct, IShape
        {
            return ShapeOverlapping.Overlaps(this, shape);
        }

        public bool Contains(Vector2 point)
        {
            if (BoundingBox.Contains(point))
            {
                return Geometry2d.IsPointInsidePolygon(point, Vertices);
            }
            return false;
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
                result = Geometry2d.GetIntersectionWithLine(Vertices, from, to, out intersection);
            }
            return result;
        }
    }
}
