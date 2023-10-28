using System;
using System.Runtime.InteropServices;
using Bro.Json;
using UnityEngine;

namespace Bro.Sketch
{
    [Serializable]
    public struct Polygon : IShape
    {
        private Rect _bBox;
        private bool _isBBoxDirty;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = _maxVertices)]
        private Vector2[] _vertices;
        private const int _maxVertices = 12;

        public Vector2[] Vertices
        {
            get
            {
                return _vertices;
            }
            set
            {
                if (value.Length > _maxVertices)
                {
                    throw new ArgumentException($"polygon :: can't have more than {_maxVertices} vertices");
                }
                
                _isBBoxDirty = true;
                _vertices = value;
            }
        }

        public ShapeType ShapeType => ShapeType.Polygon;

        [JsonIgnore] public Vector2 CenterPosition 
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        
        [JsonIgnore] public Vector2 Direction
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Vector2 Size
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
                    if (_vertices.Length > 0)
                    {
                        float minX = _vertices[0].x;
                        float maxX = _vertices[0].x;
                        float minY = _vertices[0].y;
                        float maxY = _vertices[0].y;

                        for (int i = 1; i < _vertices.Length; i++)
                        {
                            if (minX > _vertices[i].x)
                            {
                                minX = _vertices[i].x;
                            }

                            if (maxX < _vertices[i].x)
                            {
                                maxX = _vertices[i].x;
                            }

                            if (minY > _vertices[i].y)
                            {
                                minY = _vertices[i].y;
                            }

                            if (maxY < _vertices[i].y)
                            {
                                maxY = _vertices[i].y;
                            }
                        }

                        _bBox = new Rect(minX, minY, maxX - minX, maxY - minY);
                    }

                }
                return _bBox;
            }
        }

        [JsonIgnore] public float CutWidth
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool GetIntersection(Vector2 @from, Vector2 to, out Vector2 intersection)
        {
            throw new System.NotImplementedException();
        }

        public bool GetNormal(Vector2 atPoint, out Vector2 normal)
        {
            throw new System.NotImplementedException();
        }

        public Polygon(Vector2[] vertices)
        {
            if (vertices.Length > _maxVertices)
            {
                throw new ArgumentException($"polygon :: can't have more than {_maxVertices} vertices");
            }
            
            _vertices = vertices;
            _bBox = new Rect();
            _isBBoxDirty = true;
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
    }
}