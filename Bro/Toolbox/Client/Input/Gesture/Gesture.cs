using System;
using System.Collections.Generic;

namespace Bro.Toolbox.Client.Gesture
{
    public abstract class Gesture
    {
        public enum GestureType
        {
            Touch,
            Move,
            Scale,
        }
        
        public enum GesturePhase
        {
            Undefined,
            PressBegan,
            PressContinue,
            PressEnded
        }
        
        public byte Priority;
        public Enum Type;
        public GesturePhase Phase = GesturePhase.Undefined;

        public abstract bool Resolve(List<InputState> inputStates);

        public abstract void Process(List<InputState> inputStates);

        public abstract void Reset();

    }
}