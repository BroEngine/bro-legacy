using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Bro.Toolbox.Client
{
    [CustomEditor(typeof(TilesGenerator))]
    public class TileGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            TilesGenerator tilesGenerator = (TilesGenerator) target;

            DrawDefaultInspector();

            if (GUILayout.Button("Generate"))
            {
                tilesGenerator.GenerateTileMap();
            }
        }
    }
}

#endif