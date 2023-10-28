using System;

namespace Bro
{
    public struct GameTime
    {
        private readonly long _timestamp;
        
        public GameTime(long timestamp)
        {
            _timestamp = timestamp;
        }

        public string GetHourMinutesSeconds()
        {
            var t = TimeSpan.FromSeconds( _timestamp / 1000.0f );
            return string.Format("{0:D2}:{1:D2}:{2:D2}",   t.Hours,  t.Minutes,  t.Seconds);
        }
    }
}