using UnityEngine;

namespace Bro.Sketch.Client
{
    public static class RectTransformExtensions
    {
        public static void SetSize(this RectTransform target, UnityEngine.Vector2 size)
        {
            target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }
    }
}

