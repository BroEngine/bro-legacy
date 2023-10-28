#if BRO_SERVER || BRO_SERVICE || BRO_TEST

namespace Bro.Monitoring
{
    public sealed class GaugeConfiguration : MetricConfiguration
    {
        internal static readonly GaugeConfiguration Default = new GaugeConfiguration();
    }
}
#endif