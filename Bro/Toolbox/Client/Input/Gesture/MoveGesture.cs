using UnityEngine;
using System.Collections.Generic;
using Bro.Sketch;

namespace Bro.Toolbox.Client.Gesture
{
    public class MoveGesture : Gesture
    {
        private readonly float _resolveMoveLimit;
        
        private Vector2 _startPosition = Vector2.zero;
        private Vector2 _currentPosition = Vector2.zero;
        private Vector2 _prevPosition = Vector2.zero;
        private float _diffSqrResolve = float.Epsilon;
        private bool _isMoveResolve;

        private Camera _mainCamera;
        private Camera MainCamera => _mainCamera ? _mainCamera : (_mainCamera = Camera.main);
        
        public Vector2 Direction => (_currentPosition - _startPosition).normalized;
        
        public Vector3 MoveDelta()
        {
            var diff = _currentPosition - _prevPosition;
            var diffV3 = new Vector3(diff.x * MainCamera.aspect, diff.y, 0f);
            return MainCamera.ScreenToViewportPoint(diffV3);
        }
        
        public MoveGesture(float moveResolve = 0)
        {
            Type = GestureType.Move;
            _resolveMoveLimit = moveResolve;
            _resolveMoveLimit *= _resolveMoveLimit;
        }

        public override bool Resolve(List<InputState> inputStates)
        {
            var touch = inputStates[0];

            if (touch.Phase == InputState.TouchPhase.PressBegan || _startPosition.IsZero())
            {
                _startPosition = touch.Position;
            }
            
            if (!_isMoveResolve)
            {
                _currentPosition = touch.Position;
                _diffSqrResolve = (_currentPosition - _startPosition).sqrMagnitude;
                var isResolve = _diffSqrResolve > _resolveMoveLimit;
                if (isResolve)
                {
                    _isMoveResolve = true;
                }
            }

            return inputStates.Count > 0 && _isMoveResolve;
        }
        
        public override void Process(List<InputState> inputStates)
        {
            var touch = inputStates[0];
            var isStartValue = _startPosition.IsZero() || _currentPosition.IsZero() || _prevPosition.IsZero();
            
            if (touch.Phase == InputState.TouchPhase.PressBegan || isStartValue)
            {
                _startPosition = touch.Position;
                _currentPosition = touch.Position;
                Phase = GesturePhase.PressBegan;
            }

            _prevPosition = _currentPosition;
            _currentPosition = touch.Position;

            if (touch.Phase == InputState.TouchPhase.PressEnded)
            {
                Phase = GesturePhase.PressEnded;
                _isMoveResolve = false;
            }
        }

        public override void Reset()
        {
            _startPosition = Vector2.zero;
            _currentPosition = Vector2.zero;
            _prevPosition = Vector2.zero;
            _diffSqrResolve = float.Epsilon;
            _isMoveResolve = false;
            Phase = GesturePhase.Undefined;
        }
    }
}