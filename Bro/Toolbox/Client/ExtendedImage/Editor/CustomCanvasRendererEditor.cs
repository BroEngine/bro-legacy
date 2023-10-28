#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    [CustomEditor(typeof(UnityEngine.CanvasRenderer), true)]
    public class CustomCanvasRendererEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (!HaveExtendedImageComponent())
            {
                EditorGUILayout.HelpBox($"{typeof(UnityEngine.UI.Image)} component attached to object. Use {typeof(ExtendedImage)} instead.", MessageType.Error);
            }
        }

        private void OnEnable()
        {
            if (!HaveExtendedImageComponent())
            {
                var rt = target as CanvasRenderer;
                Debug.LogError($"{typeof(UnityEngine.UI.Image)} component attached to object {rt.gameObject.name}. Use {typeof(ExtendedImage)} instead.");
            }
        }
        
        private bool HaveExtendedImageComponent()
        {
            var rt = target as CanvasRenderer;
            var img = rt.GetComponent<UnityEngine.UI.Image>();
            if (img == null)
            {
                return true;
            }
            
            return img is ExtendedImage;
        }
    }
}
#endif