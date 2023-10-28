#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;


namespace Bro.Toolbox.Client
{
    public static class SpriteAtlasValidation
    {
        public static readonly string GlobalAtlasName = "Global";

        private static List<UnityEngine.U2D.SpriteAtlas> atlases;

        public static IEnumerable<UnityEngine.U2D.SpriteAtlas> Atlases
        {
            get
            {
                if (atlases == null)
                {
                    atlases = FindAllAssetsOfType<UnityEngine.U2D.SpriteAtlas>();
                }

                return atlases;
            }
        }

        private static List<T> FindAllAssetsOfType<T>() where T : UnityEngine.Object
        {
            var assets = new List<T>();
            var searchPattern = typeof(T).ToString().Split('.').Last();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{searchPattern}");
            foreach (var guid in guids)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }
    }
}
#endif