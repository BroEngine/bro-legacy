using System.Collections.Generic;

namespace Bro.Toolbox.Client.Gesture
{
    public class GestureResolver
    {
        private readonly List<Gesture> _gestures = new List<Gesture>();
        private Gesture _lastGesture;

        public void AddGesture(Gesture gesture)
        {
            _gestures.Add(gesture);
            _gestures.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }
        
        public Gesture GetGesture(List<InputState> inputStates)
        {
            foreach (var gesture in _gestures)
            {
                if (gesture.Resolve(inputStates))
                {
                    if (_lastGesture != null && (!_lastGesture.Type.Equals(gesture.Type) || _lastGesture.Phase == Gesture.GesturePhase.PressEnded))
                    {
                        _lastGesture.Reset();
                    }

                    _lastGesture = gesture;
                    gesture.Process(inputStates);
                    return gesture;
                }
            }

            return null;
        }
    }
}