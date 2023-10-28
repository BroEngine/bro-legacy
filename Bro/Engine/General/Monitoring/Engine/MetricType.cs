#if BRO_SERVER || BRO_SERVICE || BRO_TEST

namespace Bro.Monitoring
{
    internal enum MetricType
    {
        Counter,
        Gauge,
        Summary,
        Histogram
    }
}
#endif