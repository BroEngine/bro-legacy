using System.Collections.Generic;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class MobileInputController : InputControllerBase
    {
        private readonly List<InputState> _inputStates = new List<InputState>();
        private readonly List<int> _touchId = new List<int>();

        private Vector2 _position = new Vector2();
        private UnityEngine.Vector2 _uiPressPosition = new UnityEngine.Vector2();
        
        public override void ProcessInput()
        {
            if (Input.touchCount > 0)
            {
                _inputStates.Clear();

                var touches = Input.touches;
                for (int i = 0, max = touches.Length; i < max; i++)
                {
                    var touch = touches[i];

                    var phase = InputState.TouchPhase.Undefined;
                    if (touch.phase == TouchPhase.Began)
                    {
                        _uiPressPosition.x = touch.position.x;
                        _uiPressPosition.y = touch.position.y;
                        if (!IsUiPressed(_uiPressPosition))
                        {
                            phase = InputState.TouchPhase.PressBegan;
                            _touchId.Add(touch.fingerId);
                        }
                    }
                    
                    if (!_touchId.Contains(touch.fingerId))
                    {
                        continue;
                    }

                    if (phase != InputState.TouchPhase.PressBegan)
                    {
                        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                        {
                            phase = InputState.TouchPhase.PressContinue;
                        }
                        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        {
                            phase = InputState.TouchPhase.PressEnded;
                            _touchId.Remove(touch.fingerId);
                        }
                    }

                    if (phase != InputState.TouchPhase.Undefined)
                    {
                        _position.x = touch.position.x;
                        _position.y = touch.position.y;
                        _inputStates.Add(new InputState {Position = _position, Phase = phase});
                    }
                }

                if (_inputStates.Count > 0)
                {
                    new InputEvent(_inputStates).Launch();
                }
            }
        }
    }
}