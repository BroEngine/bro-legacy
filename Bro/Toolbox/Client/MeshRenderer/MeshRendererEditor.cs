#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;


namespace Bro.Toolbox.Client
{
    [CustomEditor(typeof(MeshRenderer))]
    public class MeshRendererEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MeshRenderer renderer = target as MeshRenderer;


            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            int newId = DrawSortingLayersPopup(renderer.sortingLayerID);
            if (EditorGUI.EndChangeCheck())
            {
                renderer.sortingLayerID = newId;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            int order = EditorGUILayout.IntField("Sorting Order", renderer.sortingOrder);
            if (EditorGUI.EndChangeCheck())
            {
                renderer.sortingOrder = order;
            }

            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("Set low settings"))
            {
                DisableShadows(renderer);
            }
            
            if (GUILayout.Button("Set high settings"))
            {
                EnableShadows(renderer);
            }

        }

        private void EnableShadows(MeshRenderer meshRenderer)
        {
            meshRenderer.receiveShadows = true;
            meshRenderer.shadowCastingMode = ShadowCastingMode.On;
            meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            meshRenderer.allowOcclusionWhenDynamic = false;
            meshRenderer.staticShadowCaster = false;
            meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            meshRenderer.rayTracingMode = RayTracingMode.Off;
        }

        private void DisableShadows(MeshRenderer meshRenderer)
        {
            meshRenderer.receiveShadows = false;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            meshRenderer.allowOcclusionWhenDynamic = false;
            meshRenderer.staticShadowCaster = false;
            meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            meshRenderer.rayTracingMode = RayTracingMode.Off; 
        }
        
        int DrawSortingLayersPopup(int layerID)
        {
            var layers = SortingLayer.layers;
            var names = layers.Select(l => l.name).ToArray();
            if (!SortingLayer.IsValid(layerID))
            {
                layerID = layers[0].id;
            }

            var layerValue = SortingLayer.GetLayerValueFromID(layerID);
            var newLayerValue = EditorGUILayout.Popup("Sorting Layer", layerValue, names);
            return layers[newLayerValue].id;
        }
    }
        
       
    
}
#endif