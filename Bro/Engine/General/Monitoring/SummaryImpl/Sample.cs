#if BRO_SERVER || BRO_SERVICE || BRO_TEST

namespace Bro.Monitoring
{
    // Sample holds an observed value and meta information for compression. 
    internal struct Sample
    {
        public double Value;
        public double Width;
        public double Delta;
    }
}
#endif