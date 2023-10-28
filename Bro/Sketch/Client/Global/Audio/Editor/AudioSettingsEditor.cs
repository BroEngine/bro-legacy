#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Bro.Sketch.Client.Editor
{
    [CustomEditor(typeof(AudioSettings))]
    public class AudioSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
         
            var settings = target as AudioSettings;
            if (GUILayout.Button("Update paths to files"))
            {
                #if UNITY_EDITOR
                settings.UpdatePaths();
                #endif
            }
        }
    }
}
#endif