namespace Bro.Client
{
    public partial class Timing
    {
        private class Update : Base
        {
            private bool _isRunning;
            private readonly Scheduler _scheduler;

            public Update(Handler updateHandler, Scheduler scheduler)
            {
                TimingHandler = updateHandler;
                _scheduler = scheduler;
            }

            public void Start()
            {
                if (_isRunning)
                {
                    return;
                }

                _scheduler.AddFrameTiming(this);
                _isRunning = true;
            }

            public override void Stop()
            {
                if (!_isRunning)
                {
                    return;
                }

                _scheduler.RemoveFrameTiming(this);
                _isRunning = false;
            }
        }
    }
}