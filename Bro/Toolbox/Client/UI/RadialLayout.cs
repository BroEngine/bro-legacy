#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII

using UnityEngine;
using UnityEngine.UI;

namespace Bro.Toolbox.Client.UI
{
    public class RadialLayout : LayoutGroup
    {
        [Range(0, 360)] public float StartAngle;
        [Range(0, 360)] public float LayoutAngle;
        public float MinRadius;
        
        public override void SetLayoutHorizontal() { }
        public override void SetLayoutVertical() { }
        
        public override void CalculateLayoutInputVertical()
        {
            CalculateLayoutRadial();
        }

        protected override void OnEnable()
        {
            base.OnEnable(); 
            CalculateLayoutRadial();
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            CalculateLayoutRadial();
        }
#endif
        
        private void CalculateLayoutRadial()
        {
            m_Tracker.Clear();
            if (transform.childCount == 0) return;
            
            var offsetAngle = LayoutAngle / (transform.childCount - 1);
            var fAngle = StartAngle;

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = (RectTransform) transform.GetChild(i);
                if (child == null) continue;
                
                m_Tracker.Add(this, child,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.Pivot);

                var vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);
                child.localPosition = vPos * MinRadius;
                child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);

                var rotatable = child.GetComponent<RadialLayoutCell>();
                if (rotatable != null) rotatable.SetRotation(fAngle);
                
                fAngle += offsetAngle;
            }
        }
    }
}
#endif