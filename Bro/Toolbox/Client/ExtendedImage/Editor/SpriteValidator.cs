#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

namespace Bro.Toolbox.Client
{
    public class SpriteAtlasValidator
    {
        [MenuItem("Assets/Validator/Get Used Atlases")]
        private static void GetAtlasesIsSelected()
        {
            var obj = Selection.activeObject;
            var path = obj == null ? "Assets/Resources/UI" : AssetDatabase.GetAssetPath(obj.GetInstanceID());

            PrintAtlases(path);
        }
        
        private static void PrintAtlases(string path)
        {
            var usedAtlases = new Dictionary<string, int>();
            var stringBuilder = new StringBuilder();
            
            if (Directory.Exists(path))
            {
                var prefabs = LoadAllPrefabs(path);
                if (prefabs.Count <= 0)
                {
                    Debug.Log("validator :: no prefabs in selected directory");
                    return;
                }

                foreach (var prefab in prefabs)
                {
                    var sb = GetUsedAtlases(prefab, usedAtlases);
;                   stringBuilder.Append(sb);
                }
            }
            else
            {
                var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                if (prefab is GameObject go)
                {
                    var sb = GetUsedAtlases(go, usedAtlases);
                    stringBuilder.Append(sb);
                }
            }

            if (stringBuilder.Length > 0)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            
            foreach (var atlas in usedAtlases)
            {
                Debug.Log($"validator :: used atlas - {atlas.Key}, sprites count - {atlas.Value}: {stringBuilder}");
            }
        }

        private static StringBuilder GetUsedAtlases(GameObject go, Dictionary<string, int> atlasNames)
        {
            var images = go.GetComponentsInChildren<UnityEngine.UI.Image>();
            var stringBuilder = new StringBuilder();
            
            foreach (var image in images)
            {
                var sprite = image.sprite;
                
                if (sprite == null)
                {
                    continue;
                }

                Texture2D source;
                
                try
                {
                    source = SpriteUtility.GetSpriteTexture(sprite, true);
                }
                catch
                {
                    Debug.LogWarning("validator :: sprite \"" + sprite.name + "\" not in atlas");
                    continue;
                }

                if (atlasNames.ContainsKey(source.name))
                {
                    atlasNames[source.name] += 1;
                }
                else
                {
                    atlasNames.Add(source.name, 1);
                }

                stringBuilder.Append($" {sprite.name},");
            }

            return stringBuilder;
        }
        
        public static List<GameObject> LoadAllPrefabs(string path)
        {
            var prefabs = new List<GameObject>();
            var dirInfo = new DirectoryInfo(path);
            var fileInfo = dirInfo.GetFiles("*.prefab");
            
            foreach (var info in fileInfo)
            {
                var fullPath = info.FullName.Replace(@"\","/");
                var assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
                var prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                
                if (prefab != null)
                {
                    prefabs.Add(prefab);
                }
            }
            
            
            foreach (var dir in dirInfo.GetDirectories())
            {
                prefabs.AddRange(LoadAllPrefabs(dir.FullName));
            }
            
            return prefabs;
        }
    }
}
#endif