using System;

namespace Bro.Client
{
    public class TimingEase
    {
        public enum Type
        {
            BounceIn,
            BounceOut,
            BounceInOut
        }

        private Action<float> _callback;
        private float _animationTime = 0.3f;
        private float _t;
        private EasingFunctions.Ease _ease;
        private EasingFunctions.Function _easeFunction;

        public TimingEase(float time, Action<float> callback, EasingFunctions.Ease ease = EasingFunctions.Ease.Linear)
        {
            _ease = ease;
            _callback = callback;
            _animationTime = time;

            if (_ease != EasingFunctions.Ease.Linear)
            {
                _easeFunction = EasingFunctions.GetEasingFunction(_ease);
            }
        }

        private void HandleCallback(float f)
        {
            if (_easeFunction != null)
            {
                _callback(_easeFunction.Invoke(0.0f, 1.0f, f));
            }
            else
            {
                _callback(f);
            }
        }

        public bool Tick(float dt)
        {
            float factor = Clamp01(_t / _animationTime);
            _t += dt;

            if (_t > _animationTime)
            {
                HandleCallback(1.0f);
                return false;
            }
            else
            {
                HandleCallback(factor);
                return true;
            }
        }

        private static float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }
            return value > 1f ? 1f : value;
        }
    }
}