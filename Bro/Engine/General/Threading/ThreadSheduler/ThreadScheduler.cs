// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Bro.Threading
{
    public class ThreadScheduler : IThreadScheduler
    {
        private readonly Fiber _executionContext;
        private List<IDisposable> _pending = new List<IDisposable>();
        private volatile bool _running = true;
        
        private static volatile int _isEnabled = 1;

        public static void SetIsEnabled(bool isEnabled)
        { 
            var value = isEnabled ? 1 : 0;
           Interlocked.Exchange(ref _isEnabled, value);
        }

        public ThreadScheduler(Fiber executionContext)
        {
            _executionContext = executionContext;
        }

        private void AddPending(TimerAction pending)
        {
            Action action = delegate
            {
                if (_running)
                {
                    _pending.Add(pending);
                    pending.Schedule();
                }
            };
            _executionContext.Enqueue(action);
        }

        public void Dispose()
        {
            _running = false;
            foreach (IDisposable disposable in Interlocked.Exchange<List<IDisposable>>(ref _pending, new List<IDisposable>()))
            {
                disposable.Dispose();
            }
        }

        private void Enqueue(Action action)
        {
            _executionContext.Enqueue(action);
        }

        private void Remove(IDisposable toRemove)
        {
            _executionContext.Enqueue((Action) (() => _pending.Remove(toRemove)));
        }

        public IDisposable ScheduleOnInterval(Action action, long firstInMs, long regularInMs)
        {
            var pending = new TimerAction(this, action, firstInMs, regularInMs);
            AddPending(pending);
            return pending;
        }

        public IDisposable ScheduleOnInterval(Action<int> action, long firstInMs, long regularInMs)
        {
            var pending = new TimerAction(this, action, firstInMs, regularInMs);
            AddPending(pending);
            return pending;
        }

        public static void TerminateAll()
        {
            TimerActionPool.TerminateAll();
        }


        /* PendingAction */
        private class PendingAction : IDisposable
        {
            private readonly Action _action;
            private bool _cancelled;

            public PendingAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _cancelled = true;
            }

            public void Execute()
            {
                if (!_cancelled)
                {
                    _action();
                }
            }
        }
        /* PendingAction */
        
        
        /* TimerAction */
        private class TimerAction : IDisposable
        {
            private Action _actionSimple;
            private Action<int> _actionDelta;
            private bool _cancelled;
            private readonly long _firstIntervalInMs;
            private readonly long _intervalInMs;
            private readonly ThreadScheduler _registry;
            private Timer _timer;
            private readonly Stopwatch _stopwatch = new Stopwatch();
            
            public TimerAction(ThreadScheduler registry, Action action, long firstIntervalInMs, long intervalInMs)
            {
                TimerActionPool.Register(this);
                _registry = registry;
                _actionSimple = action;
                _firstIntervalInMs = firstIntervalInMs;
                _intervalInMs = intervalInMs;
            }
    
            public TimerAction(ThreadScheduler registry, Action<int> action, long firstIntervalInMs, long intervalInMs)
            {
                TimerActionPool.Register(this);
                _registry = registry;
                _actionDelta = action;
                _firstIntervalInMs = firstIntervalInMs;
                _intervalInMs = intervalInMs;
            }
    
            public void Dispose()
            {
                _cancelled = true;
                _actionSimple = null;
                _actionDelta = null;
                _registry.Remove(this);
                var timer = Interlocked.Exchange<Timer>(ref _timer, null);

                timer?.Dispose();

                TimerActionPool.Unregister(this);
            }
    
            public void Schedule()
            {
                _stopwatch.Start();
                _timer = new Timer(x => ExecuteOnTimerThread(), null, _firstIntervalInMs, _intervalInMs);
            }
    
            private void ExecuteOnFiberThread()
            {
                if (!_cancelled)
                {
                    var deltaTime = _stopwatch.ElapsedMilliseconds;
    
                    if (deltaTime == 0)
                    {
                        return;
                    }
     
                    _stopwatch.Reset();
                    _stopwatch.Start();

                    _actionDelta?.Invoke((int) deltaTime);

                    //test
                    //var point = PerformanceMeter.Enabled ? PerformanceMeter.Register(Bro.PerformancePointType.ThreadScheduler, _actionSimple.GetDescription()) : null;

                    _actionSimple?.Invoke();
    
                    //test
                    //point?.Done();
                }
            }
    
            private void ExecuteOnTimerThread()
            {
                if (_isEnabled == 0)
                {
                    return;
                }
                if ((_intervalInMs == -1L) || _cancelled)
                {
                    _registry.Remove(this);
                    var timer = Interlocked.Exchange(ref _timer, null);

                    timer?.Dispose();

                    return;
                }
    
                _registry.Enqueue(ExecuteOnFiberThread);
            }
        }
        /* TimerAction */
        
        
        /* TimerActionPool */
        private static class TimerActionPool
        {
            private static readonly List<TimerAction> _registeredThreadPools = new List<TimerAction>();
            private static readonly object _lock = new object();

            public static void Register(TimerAction action)
            {
                lock (_lock)
                {
                    _registeredThreadPools.Add(action);
                }
            }

            public static void Unregister(TimerAction action)
            {
                lock (_lock)
                {
                    _registeredThreadPools.Remove(action);
                }
            }

            public static void TerminateAll()
            {
                TimerAction[] copy;

                lock (_lock)
                {
                    copy = _registeredThreadPools.ToArray();
                    _registeredThreadPools.Clear();
                }

                for (var i = 0; i < copy.Length; ++i)
                {
                    copy[i].Dispose();
                }
            }
        }
        /* TimerActionPool */
    }
}