using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;



namespace Bro.Toolbox.Client
{

        public class SpriteToMeshConverter : MonoBehaviour
        {
#if UNITY_EDITOR
                [SerializeField] private List<SpriteRenderer> _convertRenderers;
                [SerializeField] private string _meshName = string.Empty;
                [SerializeField] private string _modelDirectory = "Assets/Models/WorldMap";
                [SerializeField] private string _materialDirectory = "Assets/Materials/WorldMap";
                [SerializeField] private string _prefabDirectory = "Assets/Prefabs/WorldMap";
                [SerializeField] private SpriteAtlas _spriteAtlas;
                
                public enum PathType
                {
                        Model,
                        Material,
                        Prefab
                }
                public void SetDirectory(PathType pathType)
                {
                        var path = EditorUtility.OpenFolderPanel("Select need location", "Assets/", "");
                        if (string.IsNullOrEmpty(path)) return;

                        path = FileUtil.GetProjectRelativePath(path);

                        switch (pathType)
                        {
                                case PathType.Model:
                                        _modelDirectory = path;
                                        break;
                                case PathType.Material:
                                        _materialDirectory = path;
                                        break;
                                case PathType.Prefab:
                                        _prefabDirectory = path;
                                        break;
                        }
                }
                
                
                public void LoadChildrenObject()
                {
                        if (_convertRenderers == null)
                        {
                                _convertRenderers = new List<SpriteRenderer>();
                        }

                        var spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>().ToList();
                        _convertRenderers.AddRange(spriteRenderers);
                }

                public void ConvertObjects()
                {
                        if (_convertRenderers.Count == 0)
                        {
                                Debug.LogError("sprite to mesh converter :: Convert Objects is empty");
                                return;
                        }
                        
                        var material = new Material(Shader.Find("Bro/Mesh2d/SimpleUnlit"));
                        var combineInstances = new List<CombineInstance>();

                        foreach (var renderer in _convertRenderers)
                        {
                                var combineInstance = new CombineInstance();

                                if (renderer.sprite == null)
                                {
                                        Debug.LogError("sprite to mesh converter :: cant find sprite on " + renderer);
                                }
                                
                                var mesh = MeshConvertUtility.SpriteToMesh(renderer.sprite, true);
                                if (mesh == null)
                                {
                                        Debug.LogError("sprite to mesh converter :: can't create mesh: " + renderer.sprite.name);
                                        return;
                                }
                                combineInstance.mesh = mesh;
                                combineInstance.transform = renderer.gameObject.transform.localToWorldMatrix;
                                combineInstances.Add(combineInstance);
                        }

                        var newMesh = new Mesh();
                        var name = _meshName != string.Empty ? _meshName : gameObject.name;
                        var path = _prefabDirectory + "/" + name + ".prefab";

                        newMesh.CombineMeshes(combineInstances.ToArray());
                        newMesh.Optimize();
                        
                        SaveElement(material, _materialDirectory, ".mat");
                        SaveElement(newMesh, _modelDirectory, ".asset");
                        
                        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        MeshRenderer meshRenderer = null;
                        MeshFilter meshFilter = null;
                        MeshMaterialLoader meshMaterialLoader = null;
                        if (prefab == null)
                        {
                                prefab = new GameObject();
                                meshRenderer = prefab.AddComponent<MeshRenderer>();
                                meshFilter = prefab.AddComponent<MeshFilter>();
                                meshMaterialLoader = prefab.AddComponent<MeshMaterialLoader>();
                        }
                        else
                        {
                                prefab = PrefabUtility.LoadPrefabContents(path);
                                meshRenderer = prefab.GetComponent<MeshRenderer>();
                                meshFilter = prefab.GetComponent<MeshFilter>();
                                meshMaterialLoader = prefab.GetComponent<MeshMaterialLoader>();
                        }
                        
                        meshFilter.sharedMesh = newMesh;
                        meshRenderer.material = material;
                        prefab.name = name;
                        meshMaterialLoader.MeshRenderer = meshRenderer;
                        meshMaterialLoader.SpriteName = _convertRenderers[0].sprite.texture.name;
                        
                        if (_spriteAtlas != null)
                        {
                               meshMaterialLoader.SpriteAtlas = _spriteAtlas;  
                        }
                        else
                        {
                                Debug.LogError("sprite to mesh converter :: add sprite atlas");
                        }
                        
                        var prefabAsset = PrefabUtility.SaveAsPrefabAsset(prefab, path);
                        DestroyImmediate(prefab);
                        
                        prefab = PrefabUtility.LoadPrefabContents(path);
                        PrefabUtility.UnloadPrefabContents(prefab);
                        PrefabUtility.InstantiatePrefab(prefabAsset);
                }

                private void SaveElement(Object element, string directory, string extension)
                {
                        var name = _meshName != string.Empty ? _meshName : gameObject.name;
                        var path = directory + "/" + name + extension;

                        AssetDatabase.CreateAsset(element, path);
                        AssetDatabase.SaveAssets(); 
                }
#endif
        }
}