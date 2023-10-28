using System.Collections.Generic;
using Bro.Sketch;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class DefaultInputController : InputControllerBase
    {
        private const byte PressId = 1;
        
        private readonly List<InputState> _inputStates = new List<InputState>();
        private byte pressId = 0;
        
        public override void ProcessInput()
        {
            _inputStates.Clear();

            var phase = InputState.TouchPhase.Undefined;
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsUiPressed(new UnityEngine.Vector2(Input.mousePosition.x, Input.mousePosition.y)))
                {
                    phase = InputState.TouchPhase.PressBegan;
                    pressId = PressId;
                }
            }

            if (pressId == 0)
            {
                return;
            }

            if (phase != InputState.TouchPhase.PressBegan)
            {
                if (Input.GetMouseButton(0))
                {
                    phase = InputState.TouchPhase.PressContinue;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    phase = InputState.TouchPhase.PressEnded;
                    pressId = 0;
                }
            }
        }
    }
}