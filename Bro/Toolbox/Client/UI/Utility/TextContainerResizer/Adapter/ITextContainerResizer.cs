using UnityEngine;

namespace Bro.Toolbox.Client.UI
{
    public interface ITextContainerResizer
    {
        RectTransform GetRectTransform();
        void SetSizeDelta(Vector2 sizeDelta);
        void SetText(string text);
    }
}