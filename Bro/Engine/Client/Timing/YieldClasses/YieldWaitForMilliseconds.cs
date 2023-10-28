using System.Diagnostics;

namespace Bro.Client
{
    public partial class Timing
    {
        public class YieldWaitForMilliseconds : IYieldInstruction
        {
            private readonly Stopwatch _timer = new Stopwatch();
            private readonly long _waitMs;

            public YieldWaitForMilliseconds(long ms)
            {
                _waitMs = ms;
                _timer.Start();
            }

            void IYieldInstruction.Tick(TickType tickType)
            {
                if (!_timer.IsRunning)
                {
                    _timer.Start();
                }
            }

            bool IYieldInstruction.IsFinished => _timer.ElapsedMilliseconds >= _waitMs;
            
            public void Reset()
            {
                _timer.Reset();
            }
        }
    }
}