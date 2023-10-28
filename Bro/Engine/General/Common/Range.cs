using Bro.Json;
using System;
using System.Collections.Generic;

namespace Bro
{
    [Serializable]
    public struct Range<T>
    {
        [JsonProperty("min")] public T Min;
        [JsonProperty("max")] public T Max;

        public Range(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public static Range<T> Default()
        {
            return new Range<T>();
        }

        public override string ToString()
        {
            return "{ min = " + Min + ", max = " + Max + " }";
        }
    }

    public static class RangeExtension
    {
        public static bool IsFit<T>(this Range<T> range, T value) where T : IComparable
        {
            return value.CompareTo(range.Min) >= 0 && value.CompareTo(range.Max) <= 0;
        }

        public static bool IsCorresponds(List<Range<int>> ranges)
        {
            var corresponds = true;
            for (var i = 0; i < ranges.Count; ++i)
            {
                for (var j = 0; j < ranges.Count; ++j)
                {
                    var a = ranges[i];
                    var b = ranges[j];
                    // если я правильно помню дискретную математику... может быть не верно)
                    var left = Math.Max( a.Min, b.Min );
                    var right = Math.Min( a.Max, b.Max );
                    if ( left > right )
                    {
                        corresponds = false;
                        break;
                    }
                }
            }
            return corresponds;
        }
    }
}