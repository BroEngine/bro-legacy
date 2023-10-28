#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT )
using UnityEngine;

namespace Bro.Client.UI
{
    public class UIWindowsContainer : MonoBehaviour
    {
        public static Transform CurrentContainer { get; private set; }

        private void OnEnable()
        {
            CurrentContainer = transform;
        }

        private void OnDisable()
        {
            CurrentContainer = null;
        }
    }
}
#endif