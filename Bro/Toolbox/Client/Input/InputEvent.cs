using System.Collections.Generic;

namespace Bro.Toolbox.Client
{
    public class InputEvent : Bro.Client.Event
    {
        public readonly List<InputState> InputStates;

        public InputEvent(List<InputState> inputStates)
        {
            InputStates = inputStates;
        }
    }
}