﻿#if BRO_SERVER || BRO_SERVICE || BRO_TEST
namespace Bro.Monitoring
{
    public sealed class HistogramConfiguration : MetricConfiguration
    {
        internal static readonly HistogramConfiguration Default = new HistogramConfiguration();

        /// <summary>
        /// Custom histogram buckets to use. If null, will use Histogram.DefaultBuckets.
        /// </summary>
        public double[] Buckets { get; set; }
    }
}
#endif