using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.Sprites;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public static class MeshConvertUtility
    {
        public static Mesh SpriteToMesh(Sprite sprite, bool isAtlas = false)
        {
            var mesh = new Mesh();
            try
            {
                var uv = !isAtlas ? sprite.uv : SpriteUtility.GetSpriteUVs(sprite, true);
                mesh.SetVertices(Array.ConvertAll(sprite.vertices, i => (Vector3) i).ToList());
                mesh.SetUVs(0, uv.ToList());
                mesh.SetTriangles(Array.ConvertAll(sprite.triangles, i => (int) i), 0);
            }
            catch (Exception e)
            {
                Debug.LogError("sprite = " + sprite.name + "; " + e);
            }


            return mesh;
        }
    }
}
#endif