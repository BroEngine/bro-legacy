#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Bro.Toolbox.Client.Settings.Editor
{
    [CustomPropertyDrawer(typeof(OnChangedAttribute))]
    public class OnChangedAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property);
            if(EditorGUI.EndChangeCheck())
            {
                OnChangedAttribute at = attribute as OnChangedAttribute;
                MethodInfo method = property.serializedObject.targetObject.GetType().GetMethods().First(m => m.Name == at.MethodName);

                if (method != null && !method.GetParameters().Any())
                    method.Invoke(property.serializedObject.targetObject, null);
            }
        }
    }
}
#endif