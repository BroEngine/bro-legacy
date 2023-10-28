using System;
using System.Collections.Generic;
using System.Linq;
using Bro.Json;

namespace Bro
{
    [Serializable]
    public class RangeValueList<TRange,TValue> where TRange : IComparable
    {
        [JsonProperty("distribution")]public readonly List<RangeValue<TRange, TValue>> Distribution = new List<RangeValue<TRange, TValue>>();
        
        public TValue GetValue(TRange valueFromRange)
        {
            foreach (var item in Distribution)
            {
                if (item.Range.Min.CompareTo(valueFromRange) <= 0 && item.Range.Max.CompareTo(valueFromRange) >= 0)
                {
                    return item.Value;
                }
            }
            Bro.Log.Error("range value list :: cannot find any value for " + valueFromRange);
            return Distribution.FirstOrDefault().Value;
        }
    }
}