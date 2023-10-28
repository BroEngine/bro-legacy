namespace Bro.Client
{
    public partial class Timing
    {
        private class LateUpdate : Base
        {
            private readonly Scheduler _scheduler;
            public LateUpdate(Timing.Handler updateHandler,Scheduler scheduler)
            {
                TimingHandler = updateHandler;
                _scheduler = scheduler;
            }

            public override void Stop()
            {
                _scheduler.RemoveAfterFrameTiming(this);
            }
        }
    }
}