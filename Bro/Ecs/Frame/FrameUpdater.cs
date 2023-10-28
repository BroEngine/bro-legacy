using System;
using System.Diagnostics;

namespace Bro.Ecs
{
    public abstract class FrameUpdater
    {
        private readonly long _frameInterval;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private int _currentTick;

        public bool IsRunning => _stopwatch.IsRunning;

        public int GetLag()
        {
            var passedTime = _stopwatch.ElapsedMilliseconds;
            var framesExpected = (int) Math.Floor(passedTime * 1.0 / _frameInterval);
            var framesExist = _currentTick;
            var framesToCreate = framesExpected - framesExist;
            return framesToCreate;
        }

        protected FrameUpdater(long frameInterval)
        {
            _frameInterval = frameInterval;
        }

        public virtual void Start()
        {
            _stopwatch.Start();
            _currentTick = 0;
        }

        public virtual void Update()
        {
            if (_stopwatch.IsRunning)
            {
                var lag = GetLag();
                for (var i = 0; i < lag; i++)
                {
                    OnFrame();
                    ++_currentTick;
                }
            }
        }

        protected abstract void OnFrame();
    }
}