using System.Runtime.CompilerServices;
using UnityEngine;

namespace Bro.Toolbox
{
    public static class Convert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(Vector2 v, float height)
        {
            return new Vector3(v.x, height, v.y);
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(Vector2 v)
        {
            return new Vector3(v.x, 0f, v.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }
    }
}