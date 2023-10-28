#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII ||CONSOLE_CLIENT )
using Bro.Client;
using UnityEngine;

namespace Bro.Toolbox.Client.UI
{
    public abstract class BaseWindow : MonoBehaviour
    {
        protected abstract void OnShow(IWindowArgs args);

        protected virtual void OnHide()
        {
        }
    }
}


#endif