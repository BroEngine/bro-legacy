﻿#if !(UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || UNITY_EDITOR || CONSOLE_CLIENT)
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Quaternion : IEquatable<Quaternion>, IFormattable
    {
        // X component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
        public float x;
        // Y component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
        public float y;
        // Z component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
        public float z;
        // W component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
        public float w;

        // Access the x, y, z, w components using [0], [1], [2], [3] respectively.
        public float this[int index]
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }

            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    case 3: w = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }
        }

        // Constructs new Quaternion with given x,y,z,w components.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public Quaternion(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }

        // Set x, y, z and w components of an existing Quaternion.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void Set(float newX, float newY, float newZ, float newW)
        {
            x = newX;
            y = newY;
            z = newZ;
            w = newW;
        }

        static readonly Quaternion identityQuaternion = new Quaternion(0F, 0F, 0F, 1F);

        // The identity rotation (RO). This quaternion corresponds to "no rotation": the object
        public static Quaternion identity
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get
            {
                return identityQuaternion;
            }
        }

        // Combines rotations /lhs/ and /rhs/.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
        {
            return new Quaternion(
                lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
                lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
        }

        // Rotates the point /point/ with /rotation/.
        public static Vector3 operator *(Quaternion rotation, Vector3 point)
        {
            float x = rotation.x * 2F;
            float y = rotation.y * 2F;
            float z = rotation.z * 2F;
            float xx = rotation.x * x;
            float yy = rotation.y * y;
            float zz = rotation.z * z;
            float xy = rotation.x * y;
            float xz = rotation.x * z;
            float yz = rotation.y * z;
            float wx = rotation.w * x;
            float wy = rotation.w * y;
            float wz = rotation.w * z;

            Vector3 res;
            res.x = (1F - (yy + zz)) * point.x + (xy - wz) * point.y + (xz + wy) * point.z;
            res.y = (xy + wz) * point.x + (1F - (xx + zz)) * point.y + (yz - wx) * point.z;
            res.z = (xz - wy) * point.x + (yz + wx) * point.y + (1F - (xx + yy)) * point.z;
            return res;
        }

        // *undocumented*
        public const float kEpsilon = 0.000001F;

        // Is the dot product of two quaternions within tolerance for them to be considered equal?
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        private static bool IsEqualUsingDot(float dot)
        {
            // Returns false in the presence of NaN values.
            return dot > 1.0f - kEpsilon;
        }

        // Are two quaternions equal to each other?
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool operator ==(Quaternion lhs, Quaternion rhs)
        {
            return IsEqualUsingDot(Dot(lhs, rhs));
        }

        // Are two quaternions different from each other?
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool operator !=(Quaternion lhs, Quaternion rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        // The dot product between two rotations.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static float Dot(Quaternion a, Quaternion b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public void SetLookRotation(Vector3 view)
        //{
        //    Vector3 up = Vector3.up;
        //    SetLookRotation(view, up);
        //}

        // Creates a rotation with the specified /forward/ and /upwards/ directions.
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public void SetLookRotation(Vector3 view, Vector3 up)
        //{
        //    this = LookRotation(view, up);
        //}

        // Returns the angle in degrees between two rotations /a/ and /b/.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static float Angle(Quaternion a, Quaternion b)
        {
            float dot = Mathf.Min(Mathf.Abs(Dot(a, b)), 1.0F);
            return IsEqualUsingDot(dot) ? 0.0f : Mathf.Acos(dot) * 2.0F * Mathf.Rad2Deg;
        }

        // Makes euler angles positive 0/360 with 0.0001 hacked to support old behaviour of QuaternionToEuler
        private static Vector3 Internal_MakePositive(Vector3 euler)
        {
            float negativeFlip = -0.0001f * Mathf.Rad2Deg;
            float positiveFlip = 360.0f + negativeFlip;

            if (euler.x < negativeFlip)
                euler.x += 360.0f;
            else if (euler.x > positiveFlip)
                euler.x -= 360.0f;

            if (euler.y < negativeFlip)
                euler.y += 360.0f;
            else if (euler.y > positiveFlip)
                euler.y -= 360.0f;

            if (euler.z < negativeFlip)
                euler.z += 360.0f;
            else if (euler.z > positiveFlip)
                euler.z -= 360.0f;

            return euler;
        }

        //public Vector3 eulerAngles
        //{
        //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //    get { return Internal_MakePositive(Internal_ToEulerRad(this) * Mathf.Rad2Deg); }
        //    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //    set { this = Internal_FromEulerRad(value * Mathf.Deg2Rad); }
        //}
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public static Quaternion Euler(float x, float y, float z) { return Internal_FromEulerRad(new Vector3(x, y, z) * Mathf.Deg2Rad); }
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public static Quaternion Euler(Vector3 euler) { return Internal_FromEulerRad(euler * Mathf.Deg2Rad); }
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public void ToAngleAxis(out float angle, out Vector3 axis) { Internal_ToAxisAngleRad(this, out axis, out angle); angle *= Mathf.Rad2Deg; }
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public void SetFromToRotation(Vector3 fromDirection, Vector3 toDirection) { this = FromToRotation(fromDirection, toDirection); }

        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public static Quaternion RotateTowards(Quaternion from, Quaternion to, float maxDegreesDelta)
        //{
        //    float angle = Quaternion.Angle(from, to);
        //    if (angle == 0.0f) return to;
        //    return SlerpUnclamped(from, to, Mathf.Min(1.0f, maxDegreesDelta / angle));
        //}

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Quaternion Normalize(Quaternion q)
        {
            float mag = Mathf.Sqrt(Dot(q, q));

            if (mag < Mathf.Epsilon)
                return Quaternion.identity;

            return new Quaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public void Normalize()
        {
            this = Normalize(this);
        }

        public Quaternion normalized
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get { return Normalize(this); }
        }

        // used to allow Quaternions to be used as keys in hash tables
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
        }

        // also required for being able to use Quaternions as keys in hash tables
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public override bool Equals(object other)
        {
            if (!(other is Quaternion)) return false;

            return Equals((Quaternion)other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public bool Equals(Quaternion other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public override string ToString()
        {
            return ToString(null, null);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F5";
            if (formatProvider == null)
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            return string.Format("({0}, {1}, {2}, {3})", x.ToString(format, formatProvider), y.ToString(format, formatProvider), z.ToString(format, formatProvider), w.ToString(format, formatProvider));
        }

        //[System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //static public Quaternion EulerRotation(float x, float y, float z) { return Internal_FromEulerRad(new Vector3(x, y, z)); }
        //[System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public static Quaternion EulerRotation(Vector3 euler) { return Internal_FromEulerRad(euler); }
        //[System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public void SetEulerRotation(float x, float y, float z) { this = Internal_FromEulerRad(new Vector3(x, y, z)); }
        //[System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public void SetEulerRotation(Vector3 euler) { this = Internal_FromEulerRad(euler); }
        //[System.Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public Vector3 ToEuler() { return Internal_ToEulerRad(this); }
        //[System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //static public Quaternion EulerAngles(float x, float y, float z) { return Internal_FromEulerRad(new Vector3(x, y, z)); }
        //[System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public static Quaternion EulerAngles(Vector3 euler) { return Internal_FromEulerRad(euler); }
        //[System.Obsolete("Use Quaternion.ToAngleAxis instead. This function was deprecated because it uses radians instead of degrees.")]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public void ToAxisAngle(out Vector3 axis, out float angle) { Internal_ToAxisAngleRad(this, out axis, out angle); }
        //[System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public void SetEulerAngles(float x, float y, float z) { SetEulerRotation(new Vector3(x, y, z)); }
        //[System.Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public void SetEulerAngles(Vector3 euler) { this = EulerRotation(euler); }
        //[System.Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public static Vector3 ToEulerAngles(Quaternion rotation) { return Quaternion.Internal_ToEulerRad(rotation); }
        //[System.Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public Vector3 ToEulerAngles() { return Quaternion.Internal_ToEulerRad(this); }
        //[System.Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees.")]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public void SetAxisAngle(Vector3 axis, float angle) { this = AxisAngle(axis, angle); }
        //[System.Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
        //[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        //public static Quaternion AxisAngle(Vector3 axis, float angle) { return AngleAxis(Mathf.Rad2Deg * angle, axis); }




        //extern public static Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection);
        //extern public static Quaternion Inverse(Quaternion rotation);

        //extern public static Quaternion Slerp(Quaternion a, Quaternion b, float t);
        //extern public static Quaternion SlerpUnclamped(Quaternion a, Quaternion b, float t);
        //extern public static Quaternion Lerp(Quaternion a, Quaternion b, float t);
        //extern public static Quaternion LerpUnclamped(Quaternion a, Quaternion b, float t);

        //extern private static Quaternion Internal_FromEulerRad(Vector3 euler);
        //extern private static Vector3 Internal_ToEulerRad(Quaternion rotation);
        //extern private static void Internal_ToAxisAngleRad(Quaternion q, out Vector3 axis, out float angle);
        //extern public static Quaternion AngleAxis(float angle, Vector3 axis);

        //extern public static Quaternion LookRotation(Vector3 forward, Vector3 upwards);
        //public static Quaternion LookRotation(Vector3 forward) { return LookRotation(forward, Vector3.up); }
    }
}
#endif