using System;

namespace Bro.Toolbox
{
    public class RangeAttribute : Attribute
    {
        public readonly float Min;
        public readonly float Max;

        public RangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}