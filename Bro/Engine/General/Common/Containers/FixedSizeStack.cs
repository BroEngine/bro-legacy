using System;
using System.Collections;
using System.Collections.Generic;

namespace Bro
{
    public class FixedSizeStack<T> // очень странный класс(реализация)
    {
        private readonly int _limit;
        private readonly LinkedList<T> _list;
       
        public FixedSizeStack(int maxSize)
        {
            _limit = maxSize;
            _list = new LinkedList<T>();
        }
 
        public void Push(T value)
        {
            if (_list.Count == _limit)
            {
                _list.RemoveLast();
            }
            _list.AddFirst(value);
        }
 
        public T Pop()
        {
            if (_list.Count > 0)
            {
                T value = _list.First.Value;
                _list.RemoveFirst();
                return value;
            }
            else
            {
                throw new InvalidOperationException("The Stack is empty");
            }
        }
 
        public T Peek()
        {
            if (_list.Count > 0)
            {
                T value = _list.First.Value;
                return value;
            }
            else
            {
                throw new InvalidOperationException("The Stack is empty");
            }
        }
 
        public void Clear()
        {
            _list.Clear();
        }
 
        public int Count
        {
            get { return _list.Count; }
        }
 
        public bool Contains(T value)
        {
            bool result = false;
            if (this.Count > 0)
            {
                result = _list.Contains(value);
            }
            return result;
        }
 
        public IEnumerable<T> GetEnumerable()
        {
            return _list;
        }
    }
}