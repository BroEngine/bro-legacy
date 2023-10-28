using UnityEngine;

namespace Bro.Toolbox.Client
{
    abstract class CameraShakeBase : ICameraShake
    {
        protected readonly float _duration;
        protected readonly float _intensity;
        protected readonly CameraShakeData _data;
        
        protected float _timeRemaining;
        protected Vector3 _offset;
        protected Vector2 _direction;

        protected bool _isSingleShake;

        public Vector3 Offset => _offset;
        public CameraShakeData.Property Target => _data.TargetProperty;
        public bool IsAlive => _timeRemaining > 0.0f;

        protected CameraShakeBase(CameraShakeData data, float distance, Vector2 direction, bool isSingleShake)
        {
            _data = data;
            _intensity = 1 - distance / _data.MaxDistanceSquared;
            _duration = data.Duration * _intensity;
            _timeRemaining = _duration;
            _direction = direction;
            _isSingleShake = isSingleShake;
        }

        public virtual void UpdateOffset()
        {

        }
    }
}