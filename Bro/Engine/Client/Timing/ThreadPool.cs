using System;
using System.Collections.Generic;
using System.Threading;
using Bro.Threading;

namespace Bro.Client
{
    public partial class Timing
    {
        public class ThreadPool : IThreadPool
        {
            public event Action<IThreadPool> OnDispose;

            private IDisposable _update;

            private Action[] _queue = new Action[1024];
            private short _queueIndex = 0;

            private Action[] _pass = new Action[1024];
            private short _passIndex = 0;

            public ThreadPool(IClientContext globalContext)
            {
                _update = globalContext.Scheduler.ScheduleUpdate(OnUpdate);
            }

            ~ThreadPool()
            {
                Stop();
            }

            public void AddOperation(int index, Action operation)
            {
                if (_queueIndex >= _queue.Length)
                {
                    Bro.Log.Error("timing thread pool :: operations overflow");
                    return;
                }

                _queue[_queueIndex] = operation;
                ++_queueIndex;
            }

            public void Stop()
            {
                if (_update != null)
                {
                    _update.Dispose();
                    _update = null;
                    OnDispose?.Invoke(this);
                }
            }

            private void OnUpdate(float deltaTime)
            {
                var empty = _pass;

                _pass = _queue;
                _passIndex = _queueIndex;

                _queue = empty;
                _queueIndex = 0;

                for (var i = 0; i <= _passIndex; ++i)
                {
                    if (_pass[i] != null)
                    {
                        _pass[i].Invoke();
                        _pass[i] = null;
                    }
                }

            }
        }
    }
}