using System;
using System.Threading;

namespace Bro.Threading
{
    public class Fiber : IDisposable
    {
        private readonly IThreadPool _pool;

        private bool _isRunning;
        private bool _isDisposed = false;
        
        private readonly int _threadType;

        private readonly IThreadScheduler _timer;

        public Fiber(string name)
        {
            _threadType = name.GetHashCode();
            _pool = ThreadManagement.GetThreadPool(name, 1);
            _timer = new ThreadScheduler(this);
        }

        void IDisposable.Dispose()
        { 
            Stop(); 
        }

        public void Enqueue(Action action)
        {
            _pool.AddOperation(0, action);
        }

        public IDisposable ScheduleOnInterval(Action<int> action, long firstInMs, long regularInMs)
        {
            return _timer.ScheduleOnInterval(action, firstInMs, regularInMs);
        }

        public IDisposable ScheduleOnInterval(Action action, long firstInMs, long regularInMs)
        {
            return _timer.ScheduleOnInterval(action, firstInMs, regularInMs);
        }

        public void Start()
        {
            if (_isDisposed)
            {
                throw new ThreadStateException("fiber :: cannot start, fiber is disposed");
            }

            if (_isRunning)
            {
                throw new ThreadStateException("fiber :: cannot start, already running");
            }

            _isRunning = true;
            Enqueue(delegate { });
        }

        public void Stop()
        {
            if (_isDisposed)
            {
                return;
            }
            _isRunning = false;
            _timer.Dispose();
            _pool.Stop();

            _isDisposed = true;
        }
    }
}