using System.Collections.Generic;

namespace Bro
{
    public class MedianStack<T>
    {
        private readonly List<T> _samples = new List<T>();
        private readonly List<T> _sortedSamples = new List<T>();

        
        private readonly int _size;
        
        public MedianStack(int size)
        {
            _size = size;
        }
        
        public void AddSample(T newSample)
        {
            if (_samples.Count == _size)
            {
                _samples.RemoveAt(0);
            }
            _samples.Add(newSample);
        } 
        
        public int SamplesCount { get { return _samples.Count; } }
        
        public T MedianValue
        {
            get
            {
                if (_samples.Count == 0)
                {
                    return default(T);
                }

                _sortedSamples.Clear();
                _sortedSamples.AddRange(_samples);
                _sortedSamples.Sort();
                var medianIndex = _sortedSamples.Count / 2;
                return _sortedSamples[medianIndex];
            }
        }
        public void Clear()
        {
            _samples.Clear();
            _sortedSamples.Clear();
        }

        
    }
    

}