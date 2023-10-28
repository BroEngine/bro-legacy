namespace Bro.Client
{
    public partial class Timing
    {
        private class FixedUpdate : Base
        {
            private Scheduler _scheduler;
            public FixedUpdate(Handler updateHandler, Scheduler scheduler)
            {
                TimingHandler = updateHandler;
                _scheduler = scheduler;
            }
    
            public override void Stop()
            {
                _scheduler.RemoveFixedTiming(this);
            }
        }
    }
    
}