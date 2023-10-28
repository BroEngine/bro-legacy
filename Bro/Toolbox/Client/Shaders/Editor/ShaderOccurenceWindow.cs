#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class ShaderOccurenceWindow : EditorWindow
    {
        [MenuItem("Tools/Shader Occurence")]
        public static void Open()
        {
            GetWindow<ShaderOccurenceWindow>();
        }
 
        private Shader _shader;
        private List<string> _materials = new List<string>();
        private Vector2 _scroll;
 
        void OnGUI()
        {
            var prev = _shader;
            _shader = EditorGUILayout.ObjectField(_shader, typeof(Shader), false) as Shader;
            if (_shader != prev)
            {
                var shaderPath = AssetDatabase.GetAssetPath(_shader);
                var allMaterials = AssetDatabase.FindAssets("t:Material");
                _materials.Clear();
                for (int i = 0; i < allMaterials.Length; i++)
                {
                    allMaterials[i] = AssetDatabase.GUIDToAssetPath(allMaterials[i]);
                    var dep = AssetDatabase.GetDependencies(allMaterials[i]);
                    if (ArrayUtility.Contains(dep, shaderPath))
                    {
                        _materials.Add(allMaterials[i]);
                    }
                }
            }
 
            _scroll = GUILayout.BeginScrollView(_scroll);
            {
                for(var i = 0; i < _materials.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(Path.GetFileNameWithoutExtension(_materials[i]));
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Show"))
                        {
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(_materials[i], typeof(Material)));
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
        }
    }
}

#endif