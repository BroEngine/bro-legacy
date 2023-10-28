using System;

namespace Bro
{
    public class PoolCounter : IPoolCounter
    {
        public int Counter => _counter;
        private int _counter;

        private readonly Action _onRelease;

        public PoolCounter(Action onRelease)
        {
            _onRelease = onRelease;
        }

        public void Retain()
        {
            _counter++;
            
            //Bro.Log.Error($"[PoolCounter] Retain {_counter} {GetHashCode()}");
        }

        public void Release()
        {
            _counter--;

            //Bro.Log.Error($"[PoolCounter] Release {_counter} {GetHashCode()}");

            if (_counter <= 0)
            {
                _counter = 0;
                _onRelease();
            }
        }

        public bool IsPoolElement { get; set; }
    }
}