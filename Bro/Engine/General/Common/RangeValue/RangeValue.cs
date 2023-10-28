using System;
using Bro.Json;

namespace Bro
{
    [Serializable]
    public struct RangeValue<TRange, TValue>
    {
        [JsonProperty("range")]public Range<TRange> Range;
        [JsonProperty("value")]public TValue Value;

        public RangeValue(Range<TRange> range, TValue value)
        {
            Range = range;
            Value = value;
        }
    }
}