using UnityEngine;

namespace Bro.Sketch
{
    public static class UnityVector2Extensions
    {
        public static bool IsEqual(this Vector2 current, Vector2 other)
        {
            return Mathf.Abs(current.x - other.x) < float.Epsilon && Mathf.Abs(current.y - other.y) < float.Epsilon;
        }

        public static Vector2 PerpendicularClockwise(this Vector2 current)
        {
            return new Vector2(current.y, -current.x);
        }
        
        public static Vector2 PerpendicularCounterClockwise(this Vector2 current)
        {
            return new Vector2(-current.y, current.x);
        }

        public static bool IsZero(this Vector2 vector)
        {
            return Mathf.Abs(vector.x) < float.Epsilon && Mathf.Abs(vector.y) < float.Epsilon;
        }

        public static Vector2 Rotated(this Vector2 vector, float degrees)
        {
            float sin = (float)Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = (float)Mathf.Cos(degrees * Mathf.Deg2Rad);
            float tx = vector.x;
            float ty = vector.y;
            vector.x = (cos * tx) - (sin * ty);
            vector.y = (sin * tx) + (cos * ty);

            return vector;
        }  
        
        public static Vector2 RotatedRad(this Vector2 vector, float rad)
        {
            float sin = (float)Mathf.Sin(rad);
            float cos = (float)Mathf.Cos(rad);
            float tx = vector.x;
            float ty = vector.y;
            vector.x = (cos * tx) - (sin * ty);
            vector.y = (sin * tx) + (cos * ty);

            return vector;
        }

        public static Vector2 Slerp(Vector2 start, Vector2 end, float percent)
        {
            var dot = Mathf.Clamp(Vector2.Dot(start, end), -1.0f, 1.0f);
            var theta = Mathf.Acos(dot) * percent;
            var relativeVec = (end - start * dot).normalized;
            var result = ((start * (float)Mathf.Cos(theta)) + (relativeVec * (float)Mathf.Sin(theta)));

            if (Mathf.Abs(result.x) < float.Epsilon)
            {
                result.x = 0.0f;
            }

            if (Mathf.Abs(result.y) < float.Epsilon)
            {
                result.y = 0.0f;
            }

            return result;
        }

        public static Vector2 Projection(Vector2 target, Vector2 projectOnVector)
        {
            float length = projectOnVector.magnitude;
            if (length <= float.Epsilon)
            {
                return target;
            }
            return projectOnVector.normalized * (target.x * projectOnVector.x + target.y * projectOnVector.y) / length;
        }
        
        public static float AngleRad(Vector2 from, Vector2 to)
        {
            var num = Mathf.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            if (num < 1.00000000362749E-15)
            {
                return 0.0f;
            }

            return (float) (Mathf.Acos(BroMath.Clamp(Vector2.Dot(@from, to) / num, -1f, 1f)));
        }
        
        public static float Angle(Vector2 from, Vector2 to)
        {
            return AngleRad(from, to) * Mathf.Rad2Deg;
        }
        
        public static float SignedAngle(Vector2 from, Vector2 to)
        {
            return Angle(from, to) * Mathf.Sign(from.x * to.y - from.y * to.x);
        }

        public static float SqrDistance(Vector2 a, Vector2 b)
        {
            float diff_x = a.x - b.x;
            float diff_y = a.y - b.y;
            return diff_x * diff_x + diff_y * diff_y;
        }

        public static bool Contains(this Rect target, Rect rect)
        {
            return rect.xMin > target.xMin && rect.yMin > target.yMin && rect.xMax < target.xMax && rect.yMax < target.yMax;
        }
    }
}