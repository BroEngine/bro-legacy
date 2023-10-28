using System;

namespace Bro.Client
{
    public partial class Timing
    {
        public partial class Ease
        {
            public delegate void Setter<in T>(T value);
            public delegate T Calculator<T>(T startValue, T endValue, float easeCoef);
            
            public abstract class Base : IDisposable
            {
                protected Scheduler _scheduler;

                protected float _duration;
                protected float _elapsedTime;
                private bool _isRunning;

                public bool IsRunning => _isRunning;

                public void Setup(Scheduler scheduler)
                {
                    _scheduler = scheduler;
                }

                protected abstract void Update(float dt);

                public virtual void Start()
                {
                    if (_isRunning)
                    {
                        return;
                    }
                    _isRunning = true;
                }

                public virtual void Stop()
                {
                    if (!_isRunning)
                    {
                        return;
                    }
                    _isRunning = false;
                }

                void IDisposable.Dispose()
                {
                    Stop();
                }
            }
        }
    }
    
}