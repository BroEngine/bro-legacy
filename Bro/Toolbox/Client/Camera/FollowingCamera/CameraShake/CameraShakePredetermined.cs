using UnityEngine;

namespace Bro.Toolbox.Client
{
    class CameraShakePredetermined : CameraShakeBase
    {
        private readonly int _animationIndex;

        public CameraShakePredetermined(CameraShakeData data, float distance, Vector2 direction, bool isSingleShake) : base(data, distance, direction, isSingleShake)
        {
            _animationIndex = Random.Instance.Range(0, _data.ShakeAnimations.Count - 1);
        }

        public override void UpdateOffset()
        {
            var deltaTime = Time.deltaTime;

            _timeRemaining -= deltaTime;

            var remainingPercent = 1.0f - (_timeRemaining / _duration);
            
            _offset.x = _data.ShakeAnimations[_animationIndex].ShakeX.Evaluate(remainingPercent) * _direction.x;
            _offset.y = _data.ShakeAnimations[_animationIndex].ShakeY.Evaluate(remainingPercent);
            _offset.z = _data.ShakeAnimations[_animationIndex].ShakeZ.Evaluate(remainingPercent) * _direction.y;

            var blendOverLifetime = _isSingleShake ? _data.BlendOverLifetimeSingle : _data.BlendOverLifetimeAuto;
            var noise = blendOverLifetime.Evaluate(remainingPercent);

            _offset *= noise * _intensity;
        }
        
    }
}