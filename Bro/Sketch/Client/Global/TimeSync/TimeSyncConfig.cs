namespace Bro.Sketch.Client
{
    public static class TimeSyncConfig
    {
        public const float StandardRefreshRequestPeriod = 1f;
        
        public const float FastSyncRefreshRequestPeriod = 0.02f;
        public const int FastSyncTriesMax = 5;
    }
}