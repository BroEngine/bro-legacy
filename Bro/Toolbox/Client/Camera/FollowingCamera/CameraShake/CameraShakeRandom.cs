using UnityEngine;

namespace Bro.Toolbox.Client
{
    class CameraShakeRandom : CameraShakeBase
    {
        private Vector3 _noiseOffset;
        private Vector3 _noise;

        public Vector3 Noise => _noise;

        private const float _maxRandom = 32.0f;

        public CameraShakeRandom(CameraShakeData data, float distance, Vector2 direction, bool isSingleShake) : base(data, distance, direction, isSingleShake)
        {
            _noiseOffset.x = UnityEngine.Random.Range(0.0f, _maxRandom);
            _noiseOffset.y = UnityEngine.Random.Range(0.0f, _maxRandom);
            _noiseOffset.z = UnityEngine.Random.Range(0.0f, _maxRandom);
        }

        public override void UpdateOffset()
        {
            float deltaTime = Time.deltaTime;

            _timeRemaining -= deltaTime;

            float noiseOffsetDelta = deltaTime * _data.Frequency * _intensity;

            _noiseOffset.x += noiseOffsetDelta;
            _noiseOffset.y += noiseOffsetDelta;
            _noiseOffset.z += noiseOffsetDelta;

            _noise.x = Mathf.PerlinNoise(_noiseOffset.x, 0.0f) * _direction.x;
            _noise.y = Mathf.PerlinNoise(_noiseOffset.y, 1.0f);
            _noise.z = Mathf.PerlinNoise(_noiseOffset.z, 2.0f) * _direction.y;

            _noise -= Vector3.one * 0.5f;

            _noise *= _data.Amplitude * _intensity;

            float agePercent = 1.0f - (_timeRemaining / _duration);
            var blendOverLifetime = _isSingleShake ? _data.BlendOverLifetimeSingle : _data.BlendOverLifetimeAuto;
            _noise *= blendOverLifetime.Evaluate(agePercent);

            _offset = _noise;
        }
    }
}