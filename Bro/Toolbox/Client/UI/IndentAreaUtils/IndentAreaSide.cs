using UnityEngine;

namespace Bro.Toolbox.Client.UI
{
    public class IndentAreaSide : IndentAreaBase
    {
        private Vector2 _baseAnchored;
        private RectTransform _curRectTransform;
        
        protected override void Start()
        {
            _curRectTransform = transform as RectTransform;
            
            _baseAnchored = _curRectTransform.anchoredPosition;
            
            base.Start();
        }

        protected override void UpdateOffsets()
        {
            var safeAreaOffset = IndentAreaUtils.GetIndentAreaOffset();

            var offset = AnchoredSide == IndentAreaUtils.AnchoredSide.Right ? safeAreaOffset.RightOffset : safeAreaOffset.LeftOffset;
            
            _curRectTransform.anchoredPosition = new Vector2(_baseAnchored.x + offset, _baseAnchored.y);
            
        }
    }
}