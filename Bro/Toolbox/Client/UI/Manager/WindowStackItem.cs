#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT )

namespace Bro.Toolbox.Client.UI.Manager
{
    public class WindowStackItem
    {
        public Window Window;
        public IWindowArgs Args;

        public WindowStackItem(Window window, IWindowArgs args)
        {
            Window = window;
            Args = args;
        }
    }
}

#endif