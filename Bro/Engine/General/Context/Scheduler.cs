using System;
using System.Collections.Generic;

namespace Bro
{
    public class Scheduler : IScheduler
    {
        private readonly Threading.Fiber _fiber;

        public Scheduler(string ownerName)
        {
            _fiber = new Threading.Fiber(ownerName);
            _fiber.ScheduleOnInterval(Update, 33L, 33L);
        }

        public void Start()
        {
            _fiber.Start();
        }

        public void Stop()
        {
            _fiber.Stop();
        }
        
        void IScheduler.Schedule<T>(Action<T> callback, T arg)
        {
            _fiber.Enqueue(() => callback(arg));
        }

        void IScheduler.Schedule<T1, T2>(Action<T1, T2> callback, T1 arg1, T2 arg2)
        {
            _fiber.Enqueue(() => callback(arg1, arg2));
        }

        void IScheduler.Schedule<T1, T2, T3>(Action<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3)
        {
            _fiber.Enqueue(() => callback(arg1, arg2, arg3));
        }

        void IScheduler.Schedule(Action callback)
        {
            _fiber.Enqueue(callback);
        }

        IDisposable IScheduler.ScheduleUpdate(Action handler, long interval)
        {
            return _fiber.ScheduleOnInterval(handler, interval, interval);
        }

        IDisposable IScheduler.Schedule(Action callback, long delayInMs)
        {
            var result = new DelayedAction() {Action = callback, InvokeTimestamp = TimeInfo.LocalTimestamp + delayInMs};
            lock (_pendingDelayedActionsLock)
            {
                _pendingDelayedActions.Add(result);
            }
            return result;
        }

        #region DelayedActions

        private class DelayedAction : IDisposable
        {
            public long InvokeTimestamp;
            public Action Action;
            public bool IsValid = true;

            public void Dispose()
            {
                IsValid = false;
            }
        }


        
        private readonly List<DelayedAction> _delayedActions = new List<DelayedAction>();
        private readonly List<DelayedAction> _pendingDelayedActions = new List<DelayedAction>();
        private readonly object _pendingDelayedActionsLock = new object();
        
        private void Update()
        {
            lock (_pendingDelayedActionsLock)
            {
                if (_pendingDelayedActions.Count > 0)
                {
                    _delayedActions.AddRange(_pendingDelayedActions);
                    _pendingDelayedActions.Clear();
                }
            }

            if (_delayedActions.Count < 0)
            {
                return;
            }

            var curTimestamp = TimeInfo.LocalTimestamp;
            for (int i = _delayedActions.Count - 1; i >= 0; --i)
            {
                if (_delayedActions[i].InvokeTimestamp <= curTimestamp)
                {
                    try
                    {
                        if (_delayedActions[i].IsValid)
                        {
                            _delayedActions[i].Action.Invoke();
                        }
                    }
                    catch (Exception e)
                    {
                        Bro.Log.Error(e);
                    }
                    finally
                    {
                        _delayedActions.FastRemoveAtIndex(i);
                    }
                }
            }
        }
        

        #endregion

    }
}