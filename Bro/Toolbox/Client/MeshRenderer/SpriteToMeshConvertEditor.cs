using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Bro.Toolbox.Client
{
    [CustomEditor(typeof(SpriteToMeshConverter))]
    public class SpriteToMeshConvertEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            SpriteToMeshConverter spriteToMeshConverter = (SpriteToMeshConverter) target;

            DrawDefaultInspector();

            if (GUILayout.Button("Set Model directory"))
            {
                spriteToMeshConverter.SetDirectory(SpriteToMeshConverter.PathType.Model);
            }
            
            if (GUILayout.Button("Set Material directory"))
            {
                spriteToMeshConverter.SetDirectory(SpriteToMeshConverter.PathType.Material);
            }
            
            if (GUILayout.Button("Set Prefab directory"))
            {
                spriteToMeshConverter.SetDirectory(SpriteToMeshConverter.PathType.Prefab);
            }
            
            if (GUILayout.Button("Load Children"))
            {
                spriteToMeshConverter.LoadChildrenObject();
            }

            if (GUILayout.Button("Convert"))
            {
                spriteToMeshConverter.ConvertObjects();
            }
        }
    }
}
#endif