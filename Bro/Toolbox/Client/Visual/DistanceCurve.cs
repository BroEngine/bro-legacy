using System;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class DistanceCurve
    {
        // var factor = passedTime / _totalTime;
        // var curveDistance = _distanceCurve.Lenght * factor;
        // var curveFactor = _distanceCurve.GetFactor(curveDistance); // x [0 ... 1]
        // var curveValue = _distanceCurve.Evaluate(curveDistance); // y [0 ... y]
        // _ball = new Vector3(curveFactor * _distanceX, curveValue * _distanceY, 0);
        
        private readonly AnimationCurve _curve;
        private readonly int _integral;
        private readonly float[] _distances;

        private float _curveLenght;
        private float _lenghtStep;

        public float Lenght => _curveLenght;
        
        // у кого черный пояс по интегралам - велкам переделывать
        public DistanceCurve(AnimationCurve curve, int integral = 500)
        {
            _integral = integral;
            _curve = curve;
            _distances = new float[integral];

            Calculate();
        }

        public float GetFactor(float distance) // x
        {
            Bro.Log.Assert(distance <= _curveLenght, $"passed distance = {distance} greater curve lenght = {_curveLenght}");
            distance = Math.Min(distance, _curveLenght);
            var index = (int) (Math.Floor(distance / _lenghtStep));
            var factor = _distances[index];
            return factor;
        }

        public float Evaluate(float distance)
        {
            return _curve.Evaluate(GetFactor(distance));
        }

        private void Calculate()
        {
            var factorStep = 1.0f / _integral;
            var curveLenght = 0.0f;

            var amplitude = _curve.Evaluate(0f);
            for (var i = 1; i <= _integral; i++) 
            {
                var factor = _curve.Evaluate(factorStep * i);
                curveLenght += (float)Math.Sqrt( factorStep * factorStep + (amplitude-factor) * (amplitude-factor));
                amplitude = factor;
            }

            _curveLenght = curveLenght;
            _lenghtStep = curveLenght / (_integral - 1);
            
            
            var enlargement = 10;
            var passedLenght = 0.0f;
            var handledIndex = 0;
            var index = 0;
            var evaluateValue = 0f;
            
            factorStep = 1.0f / (_integral * enlargement);
            for (var i = 1; i <= _integral * enlargement; i++)
            {
                evaluateValue = factorStep * i;
                var factor = _curve.Evaluate(factorStep * i);
                passedLenght +=  (float)Math.Sqrt( factorStep * factorStep + (amplitude-factor) * (amplitude-factor));
                amplitude = factor;
                
                index = (int) (Math.Floor(passedLenght / _lenghtStep));
                if (index != handledIndex)
                {
                    ++handledIndex; // pass handled
                    for (; handledIndex < index ; ++handledIndex)
                    {
                        _distances[handledIndex] = evaluateValue;
                    }
                }

                handledIndex = index;

                _distances[index] = evaluateValue;
            }

            index = _integral;
            for (; handledIndex < index ; ++handledIndex)
            {
                _distances[handledIndex] = evaluateValue;
            }
        }
    }
}