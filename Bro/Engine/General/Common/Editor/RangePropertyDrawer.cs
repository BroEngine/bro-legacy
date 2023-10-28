#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Bro
{
    [CustomPropertyDrawer(typeof(Range<>))]
    public class RangePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUI.indentLevel = 0;

            var rectWidth = position.width / 2 - 10;

            EditorGUIUtility.labelWidth = 40;
            var startRect = new Rect(position.x, position.y, rectWidth, position.height);
            EditorGUI.PropertyField(startRect, property.FindPropertyRelative("Min"), new GUIContent("From"));

            EditorGUIUtility.labelWidth = 30;
            var endRect = new Rect(position.x + position.width / 2, position.y, rectWidth, position.height);
            EditorGUI.PropertyField(endRect, property.FindPropertyRelative("Max"), new GUIContent("To"));

            EditorGUI.indentLevel = indent;
            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUI.EndProperty();
        }
    }
}
#endif