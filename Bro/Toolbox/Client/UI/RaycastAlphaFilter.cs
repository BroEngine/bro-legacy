#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII

using UnityEngine;
using UnityEngine.UI;

namespace Bro.Toolbox.Client.UI
{
    /// <summary>
    /// Sprite Texture Type must be "Advanced" and "Read/Write" enabled.
    /// </summary>
    public class RaycastAlphaFilter : MonoBehaviour, ICanvasRaycastFilter
    {
        private Sprite _sprite;
 
        void Start ()
        {
            _sprite = GetComponent<ExtendedImage>().sprite;
        }
   
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            var rectTransform = (RectTransform)transform;
            Vector2 local;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) transform, sp, eventCamera,
                out local);
            
            var normalized = new Vector2(
                (local.x + rectTransform.pivot.x * rectTransform.rect.width) / rectTransform.rect.width,
                (local.y + rectTransform.pivot.y * rectTransform.rect.height) / rectTransform.rect.width);
            var rect = _sprite.textureRect;
            var x = Mathf.FloorToInt(rect.x + rect.width * normalized.x);
            var y = Mathf.FloorToInt(rect.y + rect.height * normalized.y);
            
            return _sprite.texture.GetPixel(x,y).a > 0;
        }
    }
}
#endif