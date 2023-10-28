namespace Bro
{
    public static class DoubleIntExtensions
    {
        static (int , int) FromLong(long longValue) {
            int intValue1 = (int)(longValue & uint.MaxValue);
            int intValue2 = (int)(longValue >> 32);
            return (intValue1, intValue2);
        }

        static long ToLong(int intValue1, int intValue2)
        {
            long longValue = intValue2;
            longValue = longValue << 32;
            longValue = longValue | (uint)intValue1;
            return longValue;
        }
    }
}