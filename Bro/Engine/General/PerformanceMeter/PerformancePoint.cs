using System;
using System.Diagnostics;

namespace Bro
{
    public class PerformancePoint
    {
        private const long FailTimeout = 5000000L; // micro
        private readonly Stopwatch _timer;
        
        public readonly Enum Enum;
        public readonly string Data;
        
        public long MicroTime
        {
            get { return _timer.ElapsedMicroseconds(); }
        }

        public string Key
        {
            get
            {
                return Name + ( Data == null ? string.Empty : "_" + Data );
            }
        }

        public string Name
        {
            get { return Enum.GetDescription(); }
        }

        public bool IsDone
        {
            get { return ! _timer.IsRunning; }
        }

        public bool IsFailed
        {
            get { return _timer.ElapsedMilliseconds > FailTimeout; }
        }

        public PerformancePoint(Enum type, string data)
        {
            Data = data;
            Enum = type;
            _timer = new Stopwatch();
            _timer.Start();
        }

        public void Done()
        {
            _timer.Stop();
        }
    }
}