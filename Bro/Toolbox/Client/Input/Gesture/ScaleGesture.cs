using System.Collections.Generic;
using UnityEngine;

namespace Bro.Toolbox.Client.Gesture
{
    public class ScaleGesture : Gesture
    {
        private float _currentSqrMagnitude = float.MaxValue;
        private float _prevSqrMagnitude = float.MaxValue;
        private float _currentDelta = float.Epsilon;

        private readonly float _maxSqrMagnitudeRange;
        
        private const float ScaleDeltaFactor = 90f;

        public float Scale => _currentDelta / _maxSqrMagnitudeRange;

        // distance - distance on screen in centimeters. 2.56 - inch.
        public ScaleGesture(float distance)
        {
            Type = GestureType.Scale;
            var magnitudeRange = ScaleDeltaFactor * distance / 2.56f;
            _maxSqrMagnitudeRange = magnitudeRange * magnitudeRange;
        }
        
        public override bool Resolve(List<InputState> inputStates)
        {
            return inputStates.Count > 1;
        }

        public override void Process(List<InputState> inputStates)
        {
            var firstTouch = inputStates[0];
            var secondTouch = inputStates[1];
            
            var isFirstTouch = firstTouch.Phase == InputState.TouchPhase.PressBegan ||
                                secondTouch.Phase == InputState.TouchPhase.PressBegan;
            var isLastTouch = firstTouch.Phase == InputState.TouchPhase.PressEnded ||
                               secondTouch.Phase == InputState.TouchPhase.PressEnded;
            
            if (isFirstTouch)
            {
                _prevSqrMagnitude = (secondTouch.Position - firstTouch.Position).sqrMagnitude;
                Phase = GesturePhase.PressBegan;
            } 
            else if (isLastTouch)
            {
                Phase = GesturePhase.PressEnded;
            }

            _currentSqrMagnitude = (secondTouch.Position - firstTouch.Position).sqrMagnitude;
            _currentDelta = _currentSqrMagnitude - _prevSqrMagnitude;
            _prevSqrMagnitude = _currentSqrMagnitude;
        }

        public override void Reset()
        {
            _currentSqrMagnitude = float.MaxValue;
            _prevSqrMagnitude = float.MaxValue;
            _currentDelta = float.Epsilon;
            Phase = GesturePhase.Undefined;
        }
    }
}