using UnityEngine;

namespace Bro.Sketch
{
    public static class UnityVector3Extensions
    {
        public static bool IsEqual(this Vector3 current, Vector3 other)
        {
            return Mathf.Abs(current.x - other.x) < float.Epsilon 
                   && Mathf.Abs(current.y - other.y) < float.Epsilon
                   && Mathf.Abs(current.z - other.z) < float.Epsilon;
        }

        public static bool IsZero(this Vector3 vector)
        {
            return Mathf.Abs(vector.x) < float.Epsilon 
                   && Mathf.Abs(vector.y) < float.Epsilon
                   && Mathf.Abs(vector.z) < float.Epsilon;
        }

        public static Vector3 Rotated(this Vector3 vector, float degrees)
        {
            float sin = (float)Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = (float)Mathf.Cos(degrees * Mathf.Deg2Rad);
            float tx = vector.x;
            float ty = vector.y;
            vector.x = (cos * tx) - (sin * ty);
            vector.y = (sin * tx) + (cos * ty);

            return vector;
        }  
        
        public static Vector3 RotatedRad(this Vector3 vector, float rad)
        {
            float sin = (float)Mathf.Sin(rad);
            float cos = (float)Mathf.Cos(rad);
            float tx = vector.x;
            float ty = vector.y;
            vector.x = (cos * tx) - (sin * ty);
            vector.y = (sin * tx) + (cos * ty);

            return vector;
        }

        public static Vector3 Slerp(Vector3 start, Vector3 end, float percent)
        {
            var dot = Mathf.Clamp(Vector3.Dot(start, end), -1.0f, 1.0f);
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
            
            if (Mathf.Abs(result.z) < float.Epsilon)
            {
                result.y = 0.0f;
            }

            return result;
        }

        public static Vector3 Projection(Vector3 target, Vector3 projectOnVector)
        {
            float length = projectOnVector.magnitude;
            if (length <= float.Epsilon)
            {
                return target;
            }
            return projectOnVector.normalized * (target.x * projectOnVector.x + target.y * projectOnVector.y + target.z * projectOnVector.z) / length;
        }
        
        public static float AngleRad(Vector3 from, Vector3 to)
        {
            var num = Mathf.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            if (num < 1.00000000362749E-15)
            {
                return 0.0f;
            }

            return (float) (Mathf.Acos(BroMath.Clamp(Vector3.Dot(@from, to) / num, -1f, 1f)));
        }
        
        public static float Angle(Vector3 from, Vector3 to)
        {
            return AngleRad(from, to) * Mathf.Rad2Deg;
        }
        
        public static float SignedAngle(Vector3 from, Vector3 to)
        {
            return Angle(from, to) * Mathf.Sign(from.x * to.y - from.y * to.x);
        }

        public static float SqrDistance(Vector3 a, Vector3 b)
        {
            float diff_x = a.x - b.x;
            float diff_y = a.y - b.y;
            return diff_x * diff_x + diff_y * diff_y;
        }
    }
}