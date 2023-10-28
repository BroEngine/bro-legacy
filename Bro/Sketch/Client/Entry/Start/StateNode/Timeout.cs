using System;
using System.Diagnostics;
using Bro.Toolbox.Logic.BehaviourTree;

namespace Bro.Sketch.Client
{
    public class Timeout:EndPoint
    {
        private readonly Stopwatch _stopwatch;
        private readonly long _timeoutPeriod;
        private readonly Action _timeoutAction;

        public Timeout(Stopwatch stopwatch, long timeoutPeriod, Action timeoutAction=null)
        {
            _stopwatch = stopwatch;
            _timeoutPeriod = timeoutPeriod;
            _timeoutAction = timeoutAction;
        }
        public override Result Process()
        {
            if (_stopwatch.ElapsedMilliseconds > _timeoutPeriod)
            {
                _timeoutAction?.Invoke();
                _stopwatch.Reset();
                return Result.Fail;
            }
            else
            {
                return Result.Running;
            }
        }
    }
}