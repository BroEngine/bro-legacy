#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Bro.Sketch.Client.Editor
{
    [CustomPropertyDrawer(typeof(AudioSettings.AudioClipSetting))]
    public class AudioClipSettingDrawer : PropertyDrawer
    {
        public override void OnGUI(UnityEngine.Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var indent = EditorGUI.indentLevel;
            var labelWidth = EditorGUIUtility.labelWidth;
            var rectWidth = position.width / 2;
            var emptyContent = GUIContent.none;

            EditorGUI.indentLevel = 2;
            
            var clipRect = new UnityEngine.Rect(position.x, position.y, rectWidth, position.height);
            EditorGUI.PropertyField(clipRect, property.FindPropertyRelative("_clip"), emptyContent);
            
            EditorGUI.indentLevel = 0;
            
            var volumeRect = new UnityEngine.Rect(position.x + rectWidth + 5, position.y, rectWidth, position.height);
            EditorGUI.Slider(volumeRect, property.FindPropertyRelative("_volume"), 0, 1, emptyContent);
            
            EditorGUI.indentLevel = indent;
            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUI.EndProperty();
        }
    }
}
#endif