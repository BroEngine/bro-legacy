using System;
using System.Collections.Generic;
using Bro.Json.Utilities;
using Bro.Network.Tcp.Engine.Client;
using Bro.Sketch;
using RabbitMQ.Client;
using UnityEngine;

namespace Bro.Sketch
{
    public static class Geometry2d
    {
        public static bool GetPerpendicularIntersection(Vector2 segmentStart, Vector2 segmentEnd, Vector2 fromPoint,
            float maxDistance, out Vector2 intersection)
        {
            var perpendicular = Vector2.Perpendicular(segmentEnd - segmentStart).normalized;

            bool hasIntersection = GetLinesIntersection(segmentEnd, segmentStart,
                fromPoint - perpendicular * maxDistance,
                fromPoint + perpendicular * maxDistance, out intersection);

            if (!hasIntersection)
            {
                intersection = perpendicular * maxDistance + fromPoint;
                return false;
            }

            return true;
        }

        public static bool HasIntersectionWithLine(Vector2[] polygonVerts, Vector2 linePoint1, Vector2 linePoint2)
        {
            for (int i = 0, max = polygonVerts.Length - 1; i < max; ++i)
            {
                if (HasLinesIntersection(polygonVerts[i], polygonVerts[i + 1], linePoint1, linePoint2))
                {
                    return true;
                }
            }

            return HasLinesIntersection(polygonVerts[polygonVerts.Length - 1], polygonVerts[0], linePoint1, linePoint2);
        }

        public static bool HasLinesIntersection(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            float num1 = ((end1.x - start1.x) * (end2.y - start2.y) - (end1.y - start1.y) * (end2.x - start2.x));
            if (System.Math.Abs(num1) <= float.Epsilon)
            {
                return false;
            }

            float num2 = ((start1.y - start2.y) * (end2.x - start2.x) - (start1.x - start2.x) * (end2.y - start2.y)) /
                         num1;
            float num3 = ((start1.y - start2.y) * (end1.x - start1.x) - (start1.x - start2.x) * (end1.y - start1.y)) /
                         num1;
            if (num2 >= 0.0f && num2 <= 1.0f && num3 >= 0.0f)
            {
                return num3 <= 1.0f;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="circleCenter"></param>
        /// <param name="cicleRadius"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineFinish"></param>
        /// <param name="intersection1"></param>
        /// <param name="intersection2"></param>
        /// <returns>number of valid intersections</returns>
        public static int GetIntersectionWithLine(Vector2 circleCenter, float cicleRadius,
            Vector2 lineStart, Vector2 lineFinish, out Vector2 intersection1, out Vector2 intersection2)
        {
            float dx, dy, A, B, C, det, t;

            dx = lineFinish.x - lineStart.x;
            dy = lineFinish.y - lineStart.y;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (lineStart.x - circleCenter.x) + dy * (lineStart.y - circleCenter.y));
            C = (lineStart.x - circleCenter.x) * (lineStart.x - circleCenter.x) +
                (lineStart.y - circleCenter.y) * (lineStart.y - circleCenter.y) - cicleRadius * cicleRadius;

            det = B * B - 4 * A * C;
            int result = 0;
            if ((A <= 0.0000001) || (det < 0f))
            {
                // No real solutions.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
            }
            else if (det <= float.Epsilon && det > -float.Epsilon)
            {
                t = -B / (2 * A);

                if (t < 0f)
                {
                    intersection1 = new Vector2(float.NaN, float.NaN);
                    intersection2 = new Vector2(float.NaN, float.NaN);
                }
                else
                {
                    result += 1;
                    intersection1 = new Vector2(lineStart.x + t * dx, lineStart.y + t * dy);
                    intersection2 = new Vector2(float.NaN, float.NaN);
                }
            }
            else
            {

                t = (float) ((-B + Math.Sqrt(det)) / (2 * A));
                if (t < 0f)
                {
                    intersection1 = new Vector2(float.NaN, float.NaN);
                }
                else
                {
                    result += 1;
                    intersection1 = new Vector2(lineStart.x + t * dx, lineStart.y + t * dy);
                }

                t = (float) ((-B - Math.Sqrt(det)) / (2 * A));
                if (t < 0f)
                {
                    intersection2 = new Vector2(float.NaN, float.NaN);
                }
                else
                {
                    result += 1;
                    intersection2 = new Vector2(lineStart.x + t * dx, lineStart.y + t * dy);
                }
            }

            return result;
        }

        public static bool GetIntersectionWithLine(Vector2[] polygonVerts, Vector2 startLine, Vector2 endLine,
            out Vector2 intersection)
        {
            bool result = false;
            Vector2 curIntersection;
            intersection = Vector2.zero;

            for (int i = 0, max = polygonVerts.Length; i < max; ++i)
            {
                if (GetLinesIntersection(polygonVerts[i], polygonVerts[i + 1 == polygonVerts.Length ? 0 : i + 1],
                    startLine, endLine, out curIntersection))
                {
                    bool hasAlreadyIntersection = result == true;
                    if (hasAlreadyIntersection)
                    {
                        bool isCurrentIntersectionIsNearToStartLine =
                            (startLine - intersection).sqrMagnitude > (startLine - curIntersection).sqrMagnitude;
                        if (isCurrentIntersectionIsNearToStartLine)
                        {
                            intersection = curIntersection;
                        }
                    }
                    else
                    {
                        result = true;
                        intersection = curIntersection;
                    }
                }
            }

            return result;
        }

        public static bool GetLinesIntersection(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2,
            out Vector2 intersection)
        {
            float denom = ((end1.x - start1.x) * (end2.y - start2.y)) - ((end1.y - start1.y) * (end2.x - start2.x));

            //  AB & CD are parallel 
            if (System.Math.Abs(denom) <= float.Epsilon)
            {
                intersection = new Vector2();
                return false;
            }

            float numer = ((start1.y - start2.y) * (end2.x - start2.x)) -
                          ((start1.x - start2.x) * (end2.y - start2.y));

            float r = numer / denom;

            float numer2 = ((start1.y - start2.y) * (end1.x - start1.x)) -
                           ((start1.x - start2.x) * (end1.y - start1.y));

            float s = numer2 / denom;

            if ((r < 0 || r > 1) || (s < 0 || s > 1))
            {
                intersection = new Vector2();
                return false;
            }

            // Find intersection point
            intersection = new Vector2(start1.x + (r * (end1.x - start1.x)), start1.y + (r * (end1.y - start1.y)));
            return true;
        }

        public static bool IsPointInsideLineSegement(Vector2 point, Vector2 segmentStart, Vector2 segmentFinish)
        {
            float maxX = Math.Max(segmentStart.x, segmentFinish.x);
            float minX = Math.Min(segmentStart.x, segmentFinish.x);
            float maxY = Math.Max(segmentStart.y, segmentFinish.y);
            float minY = Math.Min(segmentStart.y, segmentFinish.y);
            if (point.x > maxX || point.x < minX || point.y > maxY || point.y < minY)
            {
                return false;
            }

            var distanceViaPoint = (segmentFinish - point).magnitude + (segmentStart - point).magnitude;
            return System.Math.Abs((segmentStart - segmentFinish).sqrMagnitude - distanceViaPoint * distanceViaPoint) <
                   0.001f;
        }

        public static bool IsPointInsidePolygon(Vector2 point, List<Vector2> polygon)
        {
            int polygonLength = polygon.Count, i = 0;
            bool inside = false;
            // x, y for tested point.
            float pointX = point.x, pointY = point.y;
            // start / end point for the current polygon segment.
            float startX, startY, endX, endY;
            Vector2 endPoint = polygon[polygonLength - 1];
            endX = endPoint.x;
            endY = endPoint.y;
            while (i < polygonLength)
            {
                startX = endX;
                startY = endY;
                endPoint = polygon[i++];
                endX = endPoint.x;
                endY = endPoint.y;
                //
                inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                          && /* if so, test if it is under the segment */
                          ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
            }

            return inside;
        }
        
        public static bool IsPointInsidePolygon(Vector2 point, Vector2[] polygon)
        {
            int polygonLength = polygon.Length, i = 0;
            bool inside = false;
            // x, y for tested point.
            float pointX = point.x, pointY = point.y;
            // start / end point for the current polygon segment.
            float startX, startY, endX, endY;
            Vector2 endPoint = polygon[polygonLength - 1];
            endX = endPoint.x;
            endY = endPoint.y;
            while (i < polygonLength)
            {
                startX = endX;
                startY = endY;
                endPoint = polygon[i++];
                endX = endPoint.x;
                endY = endPoint.y;
                //
                inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                          && /* if so, test if it is under the segment */
                          ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
            }

            return inside;
        }

        public static bool IsPointInsideCircle(Vector2 point, Vector2 centerPos, float radius)
        {
            var inside =
                (point.x - centerPos.x) * (point.x - centerPos.x) + (point.y - centerPos.y) * (point.y - centerPos.y) <=
                radius * radius;
            return inside;
        }

        public static List<Vector2> GetPolygonsIntersection(List<Vector2> polygon1, List<Vector2> polygon2, int enityId)
        {
            var resultPolygon = new List<Vector2>();
            polygon1 = GetClockwisePolygon(polygon1);
            polygon2 = GetClockwisePolygon(polygon2);
            
            var index = 0;
            var errorIndex = 25;
            var targetSet = polygon1;
            var oppositeSet = polygon2;
            var lastPoint = polygon1[0];
            for (int i = 0; i < polygon1.Count; i++)
            {
                var point = polygon1[i];
                if (polygon2.Contains(point))
                {
                    lastPoint = point;
                    index = i;
                    break;
                }
            }
            
            var minCount = 1;
            while (resultPolygon.Count <= minCount || !(lastPoint == targetSet[index % targetSet.Count]))
            {
                errorIndex--;
                if (errorIndex <= 0)
                {
                    /*var debug = "";
                    foreach (var point in polygon1)
                    {
                        debug += point + " / ";
                    }
                    Bro.Log.Error(debug);
                    debug = "";
                    foreach (var point in polygon2)
                    {
                        debug += point + " / ";
                    }
                    Bro.Log.Error(debug);*/
                    // Bro.Log.Error("geometry 2d - get polygons intersection:: possible stack overflow in while loop:: " + enityId);
                    break;
                }

                index = index % targetSet.Count;
                var targetPoint = targetSet[index];
                var isPointIntersectPolygon =
                    IsPointInsidePolygon(targetPoint, oppositeSet.ToArray()) || oppositeSet.Contains(targetPoint);
                // Bro.Log.Error("check 1:: " + targetPoint + " / " + isPointIntersectPolygon);
                if (isPointIntersectPolygon)
                {
                    if (!resultPolygon.Contains(targetPoint))
                    {
                        resultPolygon.Add(targetPoint);
                    }

                    index++;
                    
                    continue;
                }
                
                var previousPoint = index - 1 < 0 ? targetSet[targetSet.Count - 1] : targetSet[index - 1];
                var bufferIndex = index;
                var resultOppositeTargetPoint = Vector2.zero;
                var resultIntersectionPoint = Vector2.zero;
                var minPointDistance = float.MaxValue;
                for (int i = 0; i < oppositeSet.Count; i++)
                {
                    var oppositeTargetPoint = oppositeSet[i];
                    var oppositePreviousPoint = i - 1 < 0 ? oppositeSet[oppositeSet.Count - 1] : oppositeSet[i - 1];
                    var isIntersect = GetLinesIntersection(previousPoint, targetPoint, oppositePreviousPoint,
                        oppositeTargetPoint, out var intersectionPoint);
                    var targetPointDistance = (targetPoint - intersectionPoint).magnitude;
                    if (isIntersect && minPointDistance > targetPointDistance)
                    {
                        minPointDistance = targetPointDistance;
                        resultOppositeTargetPoint = oppositeTargetPoint;
                        resultIntersectionPoint = intersectionPoint;
                    }
                }
                
                // Bro.Log.Error("check 2:: " + resultIntersectionPoint + " / " + targetPoint + " : " + previousPoint + " / " + resultOppositeTargetPoint);
                index = oppositeSet.FindIndex(x => x == resultOppositeTargetPoint);
                
                if (index == -1)
                {
                    index = bufferIndex + 1;
                    lastPoint = targetSet[index % targetSet.Count];
                    continue;
                }

                if (lastPoint == targetPoint)
                {
                    lastPoint = resultOppositeTargetPoint;
                    minCount++;
                }
                
                var bufferSet = targetSet;
                targetSet = oppositeSet;
                oppositeSet = bufferSet;
                if (resultPolygon.Contains(resultIntersectionPoint))
                {
                    continue;
                }

                resultPolygon.Add(resultIntersectionPoint);
            }

            return resultPolygon;
        }

        public static List<Vector2> GetClockwisePolygon(List<Vector2> polygon)
        {
            var clockwiseCounter = 0f;
            for (int i = 0; i < polygon.Count; i++)
            {
                var targetPoint = polygon[i];
                var previousPoint = i - 1 < 0 ? polygon[polygon.Count - 1] : polygon[i - 1];
                clockwiseCounter += (targetPoint.x - previousPoint.x) * (targetPoint.y + previousPoint.y);
            }

            if (clockwiseCounter < 0)
            {
                polygon.Reverse();
            }
            
            return polygon;
        }

        public static List<Vector2> SplitPolygonOnTriangles(List<Vector2> polygon)
        {
            if (polygon.Count < 3)
            {
                Bro.Log.Error("split polygon on triangles:: polygon must have at least 3 points");
            }
            
            var triangles = new List<Vector2>();

            var i = polygon.Count - 1;
            while (i > 1)
            {
                triangles.Add(polygon[0]);
                triangles.Add(polygon[i]);
                triangles.Add(polygon[i - 1]);
                i--;
            }

            return triangles;
        }

        public static Vector2[] GetRectangleVertices(Vector2 direction, Vector2 size, Vector2 centerPosition)
        {
            var up = direction * size.y * 0.5f;
            var left = direction.PerpendicularClockwise() * (size.x * 0.5f);
            var topLeft = centerPosition + up - left;
            var topRight = centerPosition + up + left;
            var bottomLeft = centerPosition - up - left;
            var bottomRight = centerPosition - up + left;
            return new Vector2[] {topLeft, topRight, bottomLeft, bottomRight};
        }
    }
}