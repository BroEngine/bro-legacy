using UnityEngine;
using UnityEngine.U2D;

namespace Bro.Toolbox.Client
{
    public class MeshMaterialLoader : MonoBehaviour
    {
        [SerializeField] public MeshRenderer MeshRenderer;
        [SerializeField] public SpriteAtlas SpriteAtlas;
        [SerializeField] public string SpriteName;

        private void Awake()
        {
            MeshRenderer.material.mainTexture = SpriteAtlas.GetSprite(SpriteName).texture;
        }
    }
}


