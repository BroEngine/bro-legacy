using System;
using UnityEngine;

namespace Bro.Sketch
{
    public static class ShapeOverlapping
    {
        public static bool Overlaps<T, U>(T shapeA, U shapeB) 
            where T : struct, IShape 
            where U : struct, IShape
        {
            var typeA = shapeA.ShapeType;
            var typeB = shapeB.ShapeType;
            
            if (typeA == ShapeType.Circle && typeB == ShapeType.Circle)
            {
                if (!shapeA.BoundingBox.Overlaps(shapeA.BoundingBox))
                {
                    return false;
                }

                var radiusA = shapeA.BoundingBox.size.x / 2f;
                var radiusB = shapeB.BoundingBox.size.x / 2f;
                
                return (shapeA.CenterPosition - shapeA.CenterPosition).magnitude < (radiusA + radiusB);
            }

            if ( (typeA == ShapeType.Circle && typeB == ShapeType.Rectangle) || (typeA == ShapeType.Rectangle && typeB == ShapeType.Circle))
            {
                var circleBBox = typeA == ShapeType.Circle ? shapeA.BoundingBox : shapeB.BoundingBox;
                var rectBBox = typeA == ShapeType.Circle ? shapeB.BoundingBox : shapeA.BoundingBox;
                
                if (!circleBBox.Overlaps(rectBBox))
                {
                    return false;
                }
                
                var circleCenter = shapeA.CenterPosition;

                if (rectBBox.Contains(circleCenter))
                {
                    return true;
                }
                
                var circleRadius = typeA == ShapeType.Circle ? shapeA.BoundingBox.size.x / 2f : shapeB.BoundingBox.size.x / 2f;
                var sqrCircleRadius = circleRadius * circleRadius;
                
                var rectCenter = typeA == ShapeType.Circle ? shapeB.CenterPosition : shapeA.CenterPosition;
                var rectUpDir = typeA == ShapeType.Circle ? shapeB.Direction : shapeA.Direction;
                var rectSize = typeA == ShapeType.Circle ? shapeB.Size : shapeA.Size;
                
                var up = rectUpDir * rectSize.y * 0.5f;
                var left = Vector2.Perpendicular(rectUpDir) * rectSize.x * 0.5f;
                var topLeft = rectCenter + up + left;
                if (UnityVector2Extensions.SqrDistance(circleCenter, topLeft) <= sqrCircleRadius)
                {
                    return true;
                }
                var topRight = rectCenter + up - left;
                if (UnityVector2Extensions.SqrDistance(circleCenter, topRight) <= sqrCircleRadius)
                {
                    return true;
                }
                var bottomLeft = rectCenter - up + left;
                if (UnityVector2Extensions.SqrDistance(circleCenter, bottomLeft) <= sqrCircleRadius)
                {
                    return true;
                }
                var bottomRight = rectCenter - up - left;
                if (UnityVector2Extensions.SqrDistance(circleCenter, bottomRight) <= sqrCircleRadius)
                {
                    return true;
                }

                Vector2 intersection;
                if (Geometry2d.GetPerpendicularIntersection(topLeft, topRight, circleCenter, circleRadius, out intersection))
                {
                    if (UnityVector2Extensions.SqrDistance(circleCenter, intersection) <= sqrCircleRadius)
                    {
                        return true;
                    }
                }

                if (Geometry2d.GetPerpendicularIntersection(topRight, bottomRight, circleCenter, circleRadius, out intersection))
                {
                    if (UnityVector2Extensions.SqrDistance(circleCenter, intersection) <= sqrCircleRadius)
                    {
                        return true;
                    }
                }

                if (Geometry2d.GetPerpendicularIntersection(bottomRight, bottomLeft, circleCenter, circleRadius, out intersection))
                {
                    if (UnityVector2Extensions.SqrDistance(circleCenter, intersection) <= sqrCircleRadius)
                    {
                        return true;
                    }
                }

                if (Geometry2d.GetPerpendicularIntersection(bottomLeft, topLeft, circleCenter, circleRadius, out intersection))
                {
                    if (UnityVector2Extensions.SqrDistance(circleCenter, intersection) <= sqrCircleRadius)
                    {
                        return true;
                    }
                }

                return false;
            }
            
            if (typeA == ShapeType.Rectangle && typeB == ShapeType.Rectangle)
            {
                if (!shapeA.BoundingBox.Overlaps(shapeB.BoundingBox))
                {
                    return false;
                }

                var verticesA = shapeA.Vertices;
                var verticesB = shapeB.Vertices;

                if (Geometry2d.HasIntersectionWithLine(verticesA, verticesB[0], verticesB[1]) ||
                    Geometry2d.HasIntersectionWithLine(verticesA, verticesB[1], verticesB[2]) ||
                    Geometry2d.HasIntersectionWithLine(verticesA, verticesB[2], verticesB[3]) ||
                    Geometry2d.HasIntersectionWithLine(verticesA, verticesB[3], verticesB[0]))
                {
                    return true;
                }
            
                return false;
            }

            throw new NotImplementedException();
        }
        /*
        public static bool Overlaps(ref Circle circleA, ref Circle circleB)
        {
            if (!circleA.BoundingBox.Overlaps(circleB.BoundingBox))
            {
                return false;
            }
            return (circleA.CenterPosition - circleB.CenterPosition).magnitude < (circleA.Radius + circleB.Radius);
        }

        public static bool Overlaps(ref Line lineA, ref Line lineB)
        {
            throw new NotImplementedException();
        }

        public static bool Overlaps(ref Line lineA, ref Rectangle rectB)
        {
            throw new NotImplementedException();
        }

        public static bool Overlaps(ref Line lineA, ref Polygon polygonB)
        {
            throw new NotImplementedException();
        }

        public static bool Overlaps(ref Rectangle rectA, ref Rectangle rectB)
        {
            if (!rectA.BoundingBox.Overlaps(rectB.BoundingBox))
            {
                return false;
            }

            var polyA = rectA.Vertices;
            var polyB = rectB.Vertices;

            if (Geometry2d.HasIntersectionWithLine(polyA.ToArray(), polyB[0], polyB[1]) ||
                Geometry2d.HasIntersectionWithLine(polyA.ToArray(), polyB[1], polyB[2]) ||
                Geometry2d.HasIntersectionWithLine(polyA.ToArray(), polyB[2], polyB[3]) ||
                Geometry2d.HasIntersectionWithLine(polyA.ToArray(), polyB[3], polyB[0]))
            {
                return true;
            }
            
            return false;
        }

        public static bool Overlaps(ref Polygon polygonA, ref Polygon polygonB)
        {
            throw new NotImplementedException();
        }

        public static bool Overlaps(ref Circle circleA, ref Line lineB)
        {
            throw new NotImplementedException();
        }

        public static bool Overlaps(ref Circle circleA, ref Rectangle rectB)
        {
            if (!circleA.BoundingBox.Overlaps(rectB.BoundingBox))
            {
                return false;
            }

            if (rectB.BoundingBox.Contains(circleA.CenterPosition))
            {
                return true;
            }

            var circleCenter = circleA.CenterPosition;
            var circleRadius = circleA.Radius;
            var sqrCircleRadius = circleRadius * circleRadius;
            var rectCenter = rectB.CenterPosition;
            var rectUpDir = rectB.Direction;
            var rectSize = rectB.Size;
            var up = rectUpDir * rectSize.y * 0.5f;
            var left = Vector2.Perpendicular(rectUpDir) * rectSize.x * 0.5f;
            var topLeft = rectCenter + up + left;
            if (UnityVectorExtentions.SqrDistance(circleCenter, topLeft) <= sqrCircleRadius)
            {
                return true;
            }
            var topRight = rectCenter + up - left;
            if (UnityVectorExtentions.SqrDistance(circleCenter, topRight) <= sqrCircleRadius)
            {
                return true;
            }
            var bottomLeft = rectCenter - up + left;
            if (UnityVectorExtentions.SqrDistance(circleCenter, bottomLeft) <= sqrCircleRadius)
            {
                return true;
            }
            var bottomRight = rectCenter - up - left;
            if (UnityVectorExtentions.SqrDistance(circleCenter, bottomRight) <= sqrCircleRadius)
            {
                return true;
            }

            Vector2 intersection;
            if (Geometry2d.GetPerpendicularIntersection(topLeft, topRight, circleCenter, circleRadius, out intersection))
            {
                if (UnityVectorExtentions.SqrDistance(circleCenter, intersection) <= sqrCircleRadius)
                {
                    return true;
                }
            }

            if (Geometry2d.GetPerpendicularIntersection(topRight, bottomRight, circleCenter, circleRadius, out intersection))
            {
                if (UnityVectorExtentions.SqrDistance(circleCenter, intersection) <= sqrCircleRadius)
                {
                    return true;
                }
            }

            if (Geometry2d.GetPerpendicularIntersection(bottomRight, bottomLeft, circleCenter, circleRadius, out intersection))
            {
                if (UnityVectorExtentions.SqrDistance(circleCenter, intersection) <= sqrCircleRadius)
                {
                    return true;
                }
            }

            if (Geometry2d.GetPerpendicularIntersection(bottomLeft, topLeft, circleCenter, circleRadius, out intersection))
            {
                if (UnityVectorExtentions.SqrDistance(circleCenter, intersection) <= sqrCircleRadius)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Overlaps(ref Circle circleA, ref Polygon polygonB)
        {
            throw new NotImplementedException();
        }

        public static bool Overlaps(ref Rectangle rect, ref Polygon polygonB)
        {
            throw new NotImplementedException();
        }
        */
    }
}