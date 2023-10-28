using System.Diagnostics;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    public class YieldWaitForSeconds: IBehaviourTreeYieldInstruction
    {
        
        private readonly Stopwatch _timer = new Stopwatch();
        private readonly long _waitMs;

        public YieldWaitForSeconds(float seconds)
        {
            _timer.Start();
            _waitMs = (long) (seconds * 1000);
        }

        void IBehaviourTreeYieldInstruction.Tick()
        {
            if (!_timer.IsRunning)
            {
                _timer.Start();
            }
        }

        bool IBehaviourTreeYieldInstruction.IsFinished => _timer.ElapsedMilliseconds >= _waitMs;
            
        public void Reset()
        {
            _timer.Reset();
        }
    }
}