#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT )
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bro.Toolbox.Client.UI
{
    [RequireComponent(typeof(ITextContainerResizer))]
    public class TextContainerResizer : MonoBehaviour
    {
        [SerializeField] private RectTransform _resizeRectTransform;
        [SerializeField] private MonoBehaviour _textComponent;
        [SerializeField] private ContentSizeFitter _sizeFitter;

        [SerializeField] private LayoutData _heightData;
        [SerializeField] private LayoutData _widthData;
        
        
        private ITextContainerResizer _adapter;

        private void Awake()
        {
            if (_textComponent.GetType() == typeof(TextMeshProUGUI))
            {
                _adapter = new TMPContainerResizer((TextMeshProUGUI) _textComponent);
            }
            else
            {
                Bro.Log.Error("text container resizer :: add text component");
            }
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            _sizeFitter = GetComponent<ContentSizeFitter>(); 
            if (_sizeFitter == null)
            {
                _sizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            }

            _sizeFitter.verticalFit = _heightData.FitMode;
            _sizeFitter.horizontalFit = _widthData.FitMode;
        }
#endif

        public string Text
        {
            set
            {
                //Bro.Log.Error("_adapter: " + _adapter);
                if (_adapter != null)
                {
                    _adapter.SetText(value); ResizeContent(); 
                }
            }
        }

        private void ResizeContent()
        {
            if (_resizeRectTransform == null)
            {
                return;
            }
            
            _sizeFitter.horizontalFit = _widthData.FitMode;

            var rectTransform = GetTextElementRectTransform();
            // x resize
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            var sizeDelta = rectTransform.sizeDelta;
            var newSizeDelta = new Vector2(_widthData.UpdateSize(sizeDelta.x), sizeDelta.y);
            _sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            _adapter.SetSizeDelta(newSizeDelta);

            _resizeRectTransform.sizeDelta = newSizeDelta;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            sizeDelta = rectTransform.sizeDelta;
            //y resize
            newSizeDelta = new Vector2(newSizeDelta.x, _heightData.UpdateSize(sizeDelta.y));
            _resizeRectTransform.sizeDelta = newSizeDelta;
        }

        public RectTransform GetTextElementRectTransform()
        {
            return _adapter.GetRectTransform();
        }

        public float GetMaxContainerHeight()
        {
            return _heightData.MaxContainerSize;
        }

        public float GetMinContainerHeight()
        {
            return _widthData.MinContainerSize;
        }
        
        [Serializable]
        private class LayoutData
        {
            public ContentSizeFitter.FitMode FitMode;
            
            public float TextSize;
            public float MinContainerSize;
            public float MaxContainerSize;

            public float UpdateSize(float curTextSize)
            {
                var preferredSize = MinContainerSize + curTextSize - TextSize;

                if (MaxContainerSize > 0)
                {
                    if (preferredSize > MaxContainerSize)
                    {
                        preferredSize = MaxContainerSize;
                    }
                }
                return FitMode == ContentSizeFitter.FitMode.Unconstrained ? MinContainerSize : preferredSize;
            }
        }
    }
}
#endif