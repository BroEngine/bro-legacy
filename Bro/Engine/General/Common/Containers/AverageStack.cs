using System.Collections.Generic;

namespace Bro
{
    public class AverageStack
    {
        private readonly List<double> _samples = new List<double>();
        private readonly int _size;

        public double AvgValue
        {
            get
            {
                if (_samples.Count == 0)
                {
                    return 0;
                }

                double sum = 0;
                for (var i = 0; i < _samples.Count; ++i)
                {
                    sum += _samples[i];
                }

                var result = sum / _samples.Count;
                return !double.IsNaN(result) && !double.IsInfinity(result) ? result : 0;
            }
        }

        public int SamplesCount { get { return _samples.Count; } }

        public AverageStack(int size)
        {
            _size = size;
        }

        public void Clear()
        {
            _samples.Clear();
        }

        public void AddSample(double newSample)
        {
            _samples.Add(newSample);

            if (_samples.Count > _size)
            {
                _samples.RemoveRange(0, _samples.Count - _size);
            }
        }
    }
}