using System;
using System.Collections.Generic;

namespace Bro
{
    public class MinHeap<T>
    {
        private const int InitialCapacity = 4;

        private T[] _arr;
        private int _lastItemIndex;
        private IComparer<T> _comparer;

        public MinHeap() : this(InitialCapacity, Comparer<T>.Default)
        {
        }

        public MinHeap(int capacity) : this(capacity, Comparer<T>.Default)
        {
        }

        public MinHeap(Comparison<T> comparison) : this(InitialCapacity, Comparer<T>.Create(comparison))
        {
        }
        

        public MinHeap(IComparer<T> comparer) : this(InitialCapacity, comparer)
        {
        }

        public MinHeap(int capacity, IComparer<T> comparer)
        {
            _arr = new T[capacity];
            _lastItemIndex = -1;
            _comparer = comparer;
        }

        public void SetComparer(Comparison<T> comparison)
        {
            _comparer = Comparer<T>.Create(comparison);
        }

        public int Count
        {
            get
            {
                return _lastItemIndex + 1;
            }
        }

        public void Add(T item)
        {
            if (_lastItemIndex == _arr.Length - 1)
            {
                Resize();
            }

            _lastItemIndex++;
            _arr[_lastItemIndex] = item;

            MinHeapUp(_lastItemIndex);
        }

        public T Remove()
        {
            if (_lastItemIndex == -1)
            {
                throw new InvalidOperationException("pathfinder :: the heap is empty");
            }

            var removedItem = _arr[0];
            _arr[0] = _arr[_lastItemIndex];
            _lastItemIndex--;
            MinHeapDown(0);
            return removedItem;
        }

        public T Peek()
        {
            if (_lastItemIndex == -1)
            {
                throw new InvalidOperationException("pathfinder :: the heap is empty");
            }

            return _arr[0];
        }

        public void Clear()
        {
            _lastItemIndex = -1;
        }

        private void MinHeapUp(int index)
        {
            if (index == 0)
            {
                return;
            }

            var childIndex = index;
            var parentIndex = (index - 1) / 2;

            if (_comparer.Compare(_arr[childIndex], _arr[parentIndex]) < 0)
            {
                var temp = _arr[childIndex];
                _arr[childIndex] = _arr[parentIndex];
                _arr[parentIndex] = temp;
                MinHeapUp(parentIndex);
            }
        }

        private void MinHeapDown(int index)
        {
            var leftChildIndex = index * 2 + 1;
            var rightChildIndex = index * 2 + 2;
            var smallestItemIndex = index;
            
            if (leftChildIndex <= _lastItemIndex && _comparer.Compare(_arr[leftChildIndex], _arr[smallestItemIndex]) < 0)
            {
                smallestItemIndex = leftChildIndex;
            }

            if (rightChildIndex <= _lastItemIndex && _comparer.Compare(_arr[rightChildIndex], _arr[smallestItemIndex]) < 0)
            {
                smallestItemIndex = rightChildIndex;
            }

            if (smallestItemIndex != index)
            {
                var temp = _arr[index];
                _arr[index] = _arr[smallestItemIndex];
                _arr[smallestItemIndex] = temp;
                MinHeapDown(smallestItemIndex);
            }
        }

        private void Resize()
        {
            var newArr = new T[_arr.Length * 2];
            for (int i = 0; i < _arr.Length; i++)
            {
                newArr[i] = _arr[i];
            }

            _arr = newArr;
        }
    }
}
