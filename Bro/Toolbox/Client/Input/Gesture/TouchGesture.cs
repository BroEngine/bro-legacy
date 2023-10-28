using System.Collections.Generic;
using UnityEngine;

namespace Bro.Toolbox.Client.Gesture
{
    public class TouchGesture : Gesture
    {
        public Vector2 Position { get; private set; } = Vector2.zero;

        public TouchGesture()
        {
            Type = GestureType.Touch;
        }

        public override bool Resolve(List<InputState> inputStates)
        {
            return inputStates.Count > 0 && inputStates[0].Phase == InputState.TouchPhase.PressEnded;
        }

        public override void Process(List<InputState> inputStates)
        {
            var touch = inputStates[0];

            Position = touch.Position;
        }

        public override void Reset()
        {
            Position = Vector2.zero;
        }
    }
}