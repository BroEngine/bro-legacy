using System;
namespace Bro
{
    public static class FloatExtensions
    {
        public static bool IsZero(this float target)
        {
            return Math.Abs(target) <= float.Epsilon;
        }
        
        public static bool IsApproximatelyEqual(this float me, float target)
        {
            return Math.Abs(target - me) <= float.Epsilon;
        }
    }
}