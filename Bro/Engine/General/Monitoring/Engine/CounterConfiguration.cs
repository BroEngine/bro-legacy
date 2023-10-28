#if BRO_SERVER || BRO_SERVICE || BRO_TEST
namespace Bro.Monitoring
{
    public sealed class CounterConfiguration : MetricConfiguration
    {
        internal static readonly CounterConfiguration Default = new CounterConfiguration();
    }
}
#endif
