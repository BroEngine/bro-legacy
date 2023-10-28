using System.Diagnostics;

namespace Bro.Client
{
    public partial class Timing
    {
        public class YieldWaitForSeconds : IYieldInstruction
        {
            private readonly Stopwatch _timer = new Stopwatch();
            private readonly long _waitMs;

            public YieldWaitForSeconds(float seconds)
            {
                _timer.Start();
                _waitMs = (long) (seconds * 1000);
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