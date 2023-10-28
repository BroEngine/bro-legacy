using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class InputState
    {
        public enum TouchPhase
        {
            Undefined,
            PressBegan,
            PressContinue,
            PressEnded,
        }

        public Vector2 Position { get; set; }

        public TouchPhase Phase { get; set; }
    }
}