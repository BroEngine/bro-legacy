using System;

namespace Bro.Sketch.Client
{
    public class GameSpeedController : MonoSingleton<GameSpeedController>
    {
        private float _deltaTime;

        private long _previousLocalTimestamp;

        private float _timer;
        private float _distance;

        public float SpeedCoef => GameSpeed;

        private float _deltaFps = 1.0f;
        private float _smoothFps = 0.1f;
        private float _smoothCoef = 0.9f;

        private float _previousFps = 30.0f;
        private float _currentFps = 30.0f;

        public float CurrentFPS => _currentFps;

        private float _gameSpeed = 1.0f;
        private float _targetGameSpeed = 1.0f;
        private float _gameSpeedChangeSpeed = 1.0f;

        private float GameSpeed
        {
            get
            {
                var isFpsChanged = Math.Abs(_currentFps - _previousFps) > _deltaFps;
                if (isFpsChanged)
                {
                    RecalculateGameSpeed();
                }
                return _gameSpeed;
            }
        }


        private void Awake()
        {
#warning починить
            //GameObjectsSceneManager.Instance.AddSingle(gameObject);
        }

        private void Update()
        {
            _deltaTime += (UnityEngine.Time.unscaledDeltaTime - _deltaTime) * _smoothFps;
            _currentFps = 1.0f / _deltaTime;

            if (UnityEngine.Mathf.Abs(_targetGameSpeed - _gameSpeed) > 0.01f)
            {
                _gameSpeed = Bro.Sketch.MathOperations.Lerp(_gameSpeed, _targetGameSpeed, UnityEngine.Time.deltaTime * _gameSpeedChangeSpeed);
            }
        }

        private void RecalculateGameSpeed()
        {
            for (var i = 0; i < GameConfig.GameSpeed.GameSpeedMargins.Count; i++)
            {
                var margin = GameConfig.GameSpeed.GameSpeedMargins[i];
                if (_currentFps > margin.MinFps && _currentFps <= margin.MaxFps)
                {
                    _targetGameSpeed = Bro.Sketch.MathOperations.Lerp(margin.MinFpsSpeed, margin.MaxFpsSpeed, (_currentFps - margin.MinFps) / (margin.MaxFps - margin.MinFps)) * _smoothCoef;
                }
            }

            _deltaFps = GameConfig.GameSpeed.DeltaFps;
            _gameSpeedChangeSpeed = GameConfig.GameSpeed.GameSpeedChangeSpeed;
            _previousFps = _currentFps;
        }
    }
}