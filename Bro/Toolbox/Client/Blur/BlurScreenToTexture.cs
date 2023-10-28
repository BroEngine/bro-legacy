using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Bro.Toolbox.Client
{
    [RequireComponent(typeof(RawImage))]
    public class BlurScreenToTexture : MonoBehaviour
    {
        [SerializeField] private Material BlurMaterial;

        private RenderTexture _tempRenderTexture;
        private RenderTexture _tempRenderTexture2;
        private RawImage _rawImage;
        private Camera _cam;

        private void OnEnable()
        {
            StartCoroutine(RenderRoutine());
        }

        private void OnDestroy()
        {
            if (_cam != null)
                _cam.targetTexture = null;

            if (_tempRenderTexture != null)
            {
                _tempRenderTexture.DiscardContents();
                RenderTexture.ReleaseTemporary(_tempRenderTexture);
                RenderTexture.ReleaseTemporary(_tempRenderTexture2);
            }
        }

        private IEnumerator RenderRoutine()
        {
            _cam = Camera.main;
            if (_cam == null)
                yield break;

            _rawImage = GetComponent<RawImage>();
            if (_rawImage == null)
                yield break;

            _rawImage.enabled = false;

            yield return new WaitForEndOfFrame();

            _tempRenderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
            _tempRenderTexture2 = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
            _cam.targetTexture = _tempRenderTexture;
            _cam.Render();
            _cam.targetTexture = null;
            Graphics.Blit(_tempRenderTexture, _tempRenderTexture2, BlurMaterial, 0);
            Graphics.Blit(_tempRenderTexture2, _tempRenderTexture, BlurMaterial, 1);
            Graphics.Blit(_tempRenderTexture, _tempRenderTexture2, BlurMaterial, 2);
            Graphics.Blit(_tempRenderTexture2, _tempRenderTexture, BlurMaterial, 3);
            Graphics.Blit(_tempRenderTexture, _tempRenderTexture2, BlurMaterial, 4);
            Graphics.Blit(_tempRenderTexture2, _tempRenderTexture, BlurMaterial, 5);

            _rawImage.texture = _tempRenderTexture;
            _rawImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            _rawImage.enabled = true;
        }
    }
}