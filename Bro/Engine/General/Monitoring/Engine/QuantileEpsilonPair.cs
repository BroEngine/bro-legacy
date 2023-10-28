#if BRO_SERVER || BRO_SERVICE || BRO_TEST

namespace Bro.Monitoring
{
    public readonly struct QuantileEpsilonPair
    {
        public QuantileEpsilonPair(double quantile, double epsilon)
        {
            Quantile = quantile;
            Epsilon = epsilon;
        }

        public double Quantile { get; }
        public double Epsilon { get; }
    }
}
#endif