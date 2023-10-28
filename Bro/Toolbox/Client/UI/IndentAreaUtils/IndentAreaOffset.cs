using UnityEngine;

namespace Bro.Toolbox.Client.UI
{
    class IndentAreaOffset : IndentAreaBase
    {
        private Vector2 _baseOffsetMin;
        private Vector2 _baseOffsetMax;

        private RectTransform _curRectTransform;

        protected override void Start()
        {
            _curRectTransform = (RectTransform)transform;
            
            _baseOffsetMax = _curRectTransform.offsetMax;
            _baseOffsetMin = _curRectTransform.offsetMin;
            
            base.Start();
        }

        protected override void UpdateOffsets()
        {
            var curOffsetMin = _baseOffsetMin;
            var curOffsetMax = _baseOffsetMax;

            var safeAreaOffset = IndentAreaUtils.GetIndentAreaOffset();

            if(AnchoredSide != IndentAreaUtils.AnchoredSide.Left)
            {
                curOffsetMax = new Vector2(curOffsetMax.x + safeAreaOffset.RightOffset, curOffsetMax.y);
            }

            if (AnchoredSide != IndentAreaUtils.AnchoredSide.Right)
            {
                curOffsetMin = new Vector2(curOffsetMin.x + safeAreaOffset.LeftOffset, curOffsetMax.y);
            }

            _curRectTransform.offsetMax = curOffsetMax;
            _curRectTransform.offsetMin = curOffsetMin;
        }
    }
}