#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Bro.Toolbox.Client
{
    public class ServerSaveProfile : EditorWindow
    {
        [MenuItem ("Tools/Save Profile")]
        static void SaveProfile()
        {
            if (Application.isPlaying)
            {
                AssemblyEvent.Invoke(AssemblyEventType.SaveProfileRequest);
            }
            else
            {
                Debug.LogError("application is not running");
            }
        }
    }
}
#endif