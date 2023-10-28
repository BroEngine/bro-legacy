using System.Collections.Generic;

namespace Bro
{
    public class AverageContainer
    {
        private double _avgValue;
        private readonly LinkedList<double> _samples = new LinkedList<double>();
        private readonly int _minSamplesAmount;
        private readonly int _recalcSamplesAtAmount;

        public double AvgValue
        {
            get { return _avgValue; }
        }

        public AverageContainer(int minSamples, int recalcSamplesAt)
        {
            _minSamplesAmount = minSamples;
            _recalcSamplesAtAmount = recalcSamplesAt;
        }

        public void Clear()
        {
            _samples.Clear();
        }

        public void AddSample(double newSample)
        {
            InsertSample(newSample);
            RemoveUnusedSamples();

            var samplesCount = _samples.Count;
            if (samplesCount < _minSamplesAmount)
            {
                _avgValue = 0f;
                foreach (var s in _samples)
                {
                    _avgValue += s;
                }

                _avgValue /= ((double) samplesCount);
            }
            else
            {
                var sideOffset = (samplesCount - _minSamplesAmount) / 2;
                int fromIndex = sideOffset;
                int toIndex = _minSamplesAmount + sideOffset;
                int counter = 0;
                _avgValue = 0f;
                foreach (var s in _samples)
                {
                    if (counter >= fromIndex)
                    {
                        if (counter >= toIndex)
                        {
                            break;
                        }
                        else
                        {
                            _avgValue += s;
                        }
                    }

                    ++counter;
                }

                _avgValue /= ((double) _minSamplesAmount);
            }
        }

        private void RemoveUnusedSamples()
        {
            var samplesCount = _samples.Count;
            if (samplesCount >= _recalcSamplesAtAmount)
            {
                var removeCount = samplesCount - _minSamplesAmount;
                while (removeCount > 0)
                {
                    --removeCount;
                    if (removeCount % 2 == 0)
                    {
                        _samples.RemoveFirst();
                    }
                    else
                    {
                        _samples.RemoveLast();
                    }
                }
            }
        }

        private void InsertSample(double newSample)
        {
            var current = _samples.First;
            if (current == null)
            {
                _samples.AddFirst(newSample);
            }
            else
            {
                while (true)
                {
                    if (current.Value >= newSample)
                    {
                        _samples.AddBefore(current, newSample);
                        break;
                    }

                    current = current.Next;
                    if (current == null)
                    {
                        _samples.AddLast(newSample);
                        break;
                    }
                }
            }
        }
    }
}