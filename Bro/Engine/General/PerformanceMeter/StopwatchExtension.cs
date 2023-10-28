using System.Diagnostics;

namespace Bro
{
    public static class StopwatchExtension
    {
        public static long ElapsedMicroseconds(this Stopwatch stopwatch)
        {
            return stopwatch.ElapsedTicks / 10;
        }   
        
        public static float ElapsedSeconds(this Stopwatch stopwatch)
        {
            return stopwatch.ElapsedMilliseconds / 1000.0f;
        }
    }
}