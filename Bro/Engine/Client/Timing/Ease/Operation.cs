using System;

namespace Bro.Client
{
    public partial class Timing
    {
        
        public partial class Ease
        {

            public class Operation<T> : Base 
                where T : struct
            {
                private Update _update;

                private readonly EasingFunctions.Function _easingFunction;
                
                private readonly Setter<T> _setter;
                private readonly Calculator<T> _calculator;

                private readonly T _startValue;
                private readonly T _endValue;
                private T _currentValue;
                
                // переделать!!!
                public Operation(Setter<T> setter, Calculator<T> calculator, T startValue, T endValue, float duration, 
                    EasingFunctions.Ease ease = EasingFunctions.Ease.Linear)  
                {
                    
                    _setter = setter;
                    _calculator = calculator;

                    _startValue = startValue;
                    _endValue = endValue;
                    _currentValue = _startValue;
                    
                    _duration = duration;

                    _easingFunction = EasingFunctions.GetEasingFunction(ease);
                    
                }

                protected override void Update(float dt)
                {
                    if (_elapsedTime < _duration)
                    {
                        _elapsedTime += dt;

                        var lerpFactor = BroMath.Clamp(_elapsedTime / _duration, 0f, 1f);
                        var easeFactor = _easingFunction.Invoke(0f, 1f, lerpFactor);
                        
                        _currentValue = _calculator.Invoke(_startValue, _endValue, easeFactor);
                        _setter.Invoke(_currentValue);
                    }
                    else
                    {
                        _setter.Invoke(_endValue);
                        Stop();
                    }
                }

                public override void Start()
                {
                    _update = new Update(Update, _scheduler);
                    _update.Start();
                    
                    base.Start();
                }

                public override void Stop()
                {
                    base.Stop();
                    
                    _update.Stop();
                }
                
            }
        }
    }
}