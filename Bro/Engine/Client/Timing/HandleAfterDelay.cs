using System.Diagnostics;

namespace Bro.Client
{
    public partial class Timing
    {
        private class HandleAfterDelay : Base
        {
            private readonly System.Action _callback;
            private readonly long _waitDelayInMs;
            private readonly Stopwatch _timer = new Stopwatch();
            private readonly Scheduler _scheduler;

            public HandleAfterDelay(System.Action callback, long  milliSeconds, Scheduler scheduler)
            {
                _callback = callback;
                _waitDelayInMs = milliSeconds;

                _timer.Start();

                TimingHandler = OnTick;
                _scheduler = scheduler;
            }

            public HandleAfterDelay(System.Action callback, float seconds, Scheduler scheduler):this(callback,(long)(seconds * 1000L),scheduler)
            {
            }

            public override void Stop()
            {
                _scheduler.RemoveFrameTiming(this);
            }

            private void OnTick(float deltaTime)
            {
                var passedTime = _timer.ElapsedMilliseconds;

                if (passedTime >= _waitDelayInMs)
                {
                    Complete();
                }
            }

            private void Complete()
            {
                _callback();
                Stop();
            }
        }
    }

    
}