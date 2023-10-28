using System;
using System.Collections;

namespace Bro.Client
{
    public partial class Timing
    {
        public class Coroutine : IDisposable, IYieldInstruction
        {
            private IYieldInstruction _lastYieldInstruction;

            public readonly IEnumerator Enumerator;

            public bool IsCompleted { get; private set; }

            public void Complete()
            {
                IsCompleted = true;
            }

            void IDisposable.Dispose()
            {
                Complete();
            }

            public Coroutine(IEnumerator enumerator)
            {
                IsCompleted = false;
                Enumerator = enumerator;
            }



            public void Tick(TickType tickType)
            {
                if (Enumerator == null || IsCompleted)
                {
                    return;
                }

                if (_lastYieldInstruction != null)
                {
                    _lastYieldInstruction.Tick(tickType);

                    if (!_lastYieldInstruction.IsFinished)
                    {
                        return;
                    }
                }

                _lastYieldInstruction = null;

                if (Enumerator.MoveNext())
                {
                    if (Enumerator.Current is IYieldInstruction t)
                    {
                        _lastYieldInstruction = t;
                    }
                    else if (Enumerator.Current is IEnumerator current)
                    {
                        _lastYieldInstruction = new Coroutine(current);
                    }
                    else
                    {   
                        if (Enumerator.Current != null)
                        {
                            Bro.Log.Error($"coroutine :: not supported yield type = {Enumerator.Current.GetType()}");
                        }
                    }
                }
                else
                {
                    IsCompleted = true;
                }
            }

            public void Reset()
            {
               
            }

            public bool IsFinished => IsCompleted;
        }
    }
}