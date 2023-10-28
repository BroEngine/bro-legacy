using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
namespace Bro.Toolbox.Client
{
	public static class MeshSaverEditor
	{

		[MenuItem("CONTEXT/MeshFilter/Save Mesh...")]
		public static void SaveMeshInPlace(MenuCommand menuCommand)
		{
			MeshFilter mf = menuCommand.context as MeshFilter;
			Mesh m = mf.sharedMesh;
			SaveMesh(m, m.name, false, true);
		}

		[MenuItem("CONTEXT/MeshFilter/Save Mesh As New Instance...")]
		public static void SaveMeshNewInstanceItem(MenuCommand menuCommand)
		{
			MeshFilter mf = menuCommand.context as MeshFilter;
			Mesh m = mf.sharedMesh;
			SaveMesh(m, m.name, true, true);
		}

		public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
		{
			string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
			if (string.IsNullOrEmpty(path)) return;

			path = FileUtil.GetProjectRelativePath(path);

			Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

			if (optimizeMesh)
				MeshUtility.Optimize(meshToSave);

			AssetDatabase.CreateAsset(meshToSave, path);
			AssetDatabase.SaveAssets();
		}


		[MenuItem("CONTEXT/SpriteRenderer/Save as Mesh...")]
		public static void SaveSpriteInPlace(MenuCommand menuCommand)
		{
			SpriteRenderer spriteRenderer = menuCommand.context as SpriteRenderer;
			var sprite = spriteRenderer.sprite;
			var mesh = MeshConvertUtility.SpriteToMesh(sprite);
			SaveMesh(mesh, mesh.name, false, true);
		}
	}
}
#endif