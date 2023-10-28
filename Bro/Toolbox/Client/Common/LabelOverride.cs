#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII )
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Bro.Toolbox.Client
{

    public class LabelOverride : PropertyAttribute
    {
        public string label;
        public LabelOverride(string label)
        {
            this.label = label;
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(LabelOverride))]
        public class ThisPropertyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var propertyAttribute = this.attribute as LabelOverride;
                label.text = propertyAttribute.label;
                EditorGUI.PropertyField(position, property, label);
            }
        }
#endif
    }

}
#endif