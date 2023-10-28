#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Bro
{
    public static class UnityPoolManagerLogSaver
    {
        [InitializeOnLoadMethod]
        static void LoadPoolManager()
        {
            EditorApplication.playModeStateChanged -= StateChanged;
            EditorApplication.playModeStateChanged += StateChanged;
        }
        
        private static void StateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.ExitingPlayMode)
            {
                NetworkPool.StateChanged(Application.dataPath);
            }
        }
    }
}
#endif