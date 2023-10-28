#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT )
using Bro.Client;

namespace Bro.Toolbox.Client.UI
{
    public class WindowEvent : Event
    {
        public enum ActionType
        {
            Showed,
            Hided
        }

        public ActionType EventActionType { get; private set; }

        public Window Window { get; private set; }

        public WindowEvent( ActionType actionType, Window window )
        {
            EventActionType = actionType;
            Window = window;
        }
    }
}
#endif