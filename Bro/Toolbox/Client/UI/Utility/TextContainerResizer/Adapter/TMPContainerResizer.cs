#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT )
using TMPro;
using UnityEngine;

namespace Bro.Toolbox.Client.UI
{
    public class TMPContainerResizer : ITextContainerResizer
    {
        private readonly TextMeshProUGUI _textComponent;

        public TMPContainerResizer(TextMeshProUGUI textComponent)
        {
            _textComponent = textComponent;
        }
        
        public RectTransform GetRectTransform()
        {
            return _textComponent.rectTransform;
        }

        public void SetSizeDelta(Vector2 sizeDelta)
        {
            _textComponent.rectTransform.sizeDelta = sizeDelta;
        }

        public void SetText(string text)
        {
            _textComponent.text = text;
        }
    }
}
#endif