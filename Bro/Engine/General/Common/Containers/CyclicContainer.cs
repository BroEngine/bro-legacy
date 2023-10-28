using System;
using System.Collections;
using System.Collections.Generic;

namespace Bro
{
    public class CyclicContainer<T> : IEnumerable<T> where T : new()
    {
        private class Enumerator : IEnumerator<T>
        {
            private readonly CyclicContainer<T> _owner;
            private int _currentIndex = -1;
            private T _current;

            T IEnumerator<T>.Current
            {
                get { return _current; }
            }

            public object Current
            {
                get { return _current; }
            }

            public Enumerator(CyclicContainer<T> owner)
            {
                _owner = owner;
            }

            public bool MoveNext()
            {
                if ((_currentIndex + 1) >= _owner._size)
                {
                    return false;
                }

                ++_currentIndex;
                _current = _owner[_currentIndex];
                return true;
            }

            public void Reset()
            {
                _currentIndex = -1;
                _current = default(T);
            }

            public void Dispose()
            {
            }
        }

        private readonly int _size;
        private int _currentIndex;
        private readonly T[] _items;

        public int Size
        {
            get { return _size; }
        }

        public CyclicContainer(int size)
        {
            _size = size;
            _items = new T[size];
            for (int i = 0; i < size; ++i)
            {
                _items[i] = new T();
            }
        }

        public T this[int decrement]
        {
            get
            {
                if (decrement >= _size || decrement < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (decrement == 0)
                {
                    return _items[_currentIndex];
                }

                decrement = _currentIndex - decrement;
                if (decrement < 0)
                {
                    decrement += _size;
                }

                return _items[decrement];
            }
        }
        public T Next
        {
            get
            {
                ++_currentIndex;
                if (_currentIndex >= _size)
                {
                    _currentIndex = 0;
                }

                return _items[_currentIndex];
            }
            set
            {
                ++_currentIndex;
                if (_currentIndex >= _size)
                {
                    _currentIndex = 0;
                }

                _items[_currentIndex] = value;
            }
        }

        public T Current
        {
            get { return this[0]; }
        }

        public T Previous
        {
            get { return this[1]; }
        }

        public void ApplyForeach(System.Action<T> action)
        {
            for (int i = 0; i < _size; ++i)
            {
                action(this[i]);
            }
        }
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }
    }
}