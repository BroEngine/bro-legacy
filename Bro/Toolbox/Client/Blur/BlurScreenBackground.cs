using UnityEngine;
using UnityEngine.UI;

namespace Bro.Toolbox.Client
{
    public class BlurScreenBackground : MonoBehaviour
    {
        [SerializeField] private Material _material;
        [SerializeField] private RawImage _rawImage;

        private RenderTexture _mainRenderTexture;
        private RenderTexture _tempRenderTexture;
        private RenderTexture _tempRenderTexture2;
        private Camera _cam;

        private void OnEnable()
        {
            _cam = Camera.main;
            _mainRenderTexture = new RenderTexture(Screen.width, Screen.height, 16);
            _tempRenderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
            _tempRenderTexture2 = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
            _cam.targetTexture = _mainRenderTexture;
            _cam.forceIntoRenderTexture = true;
            _rawImage.texture = _tempRenderTexture2;
        }

        private void OnDisable()
        {
            if (_cam != null)
                _cam.targetTexture = null;

            if (_tempRenderTexture != null)
            {
                _tempRenderTexture.DiscardContents();
                _tempRenderTexture.Release();
                RenderTexture.ReleaseTemporary(_tempRenderTexture);
            }
        }

        private void Update()
        {
            Graphics.Blit(_mainRenderTexture, _tempRenderTexture, _material, 0);
            Graphics.Blit(_mainRenderTexture, _tempRenderTexture2, _material, 1);
        }
    }
}