namespace Bro.Toolbox.Client
{
    public static class RectTransformExtension
    {
        public static UnityEngine.Vector3 GetRightTopLocalPosition(this UnityEngine.RectTransform transform)
        {
            var x = transform.sizeDelta.x - transform.pivot.x * transform.sizeDelta.x;
            var y = transform.sizeDelta.y - transform.pivot.y * transform.sizeDelta.y;
            return new UnityEngine.Vector3(x, y, 0);;
        }
        
        public static UnityEngine.Vector3 GetRightMiddleLocalPosition(this UnityEngine.RectTransform transform)
        {
            var x = transform.sizeDelta.x - transform.pivot.x * transform.sizeDelta.x;
            var y = transform.sizeDelta.y * 0.5f - transform.pivot.y * transform.sizeDelta.y;
            return new UnityEngine.Vector3(x, y, 0);;
        }
        
        public static UnityEngine.Vector3 GetRightBottomLocalPosition(this UnityEngine.RectTransform transform)
        {
            var x = transform.sizeDelta.x - transform.pivot.x * transform.sizeDelta.x;
            var y = - transform.pivot.y * transform.sizeDelta.y;
            return new UnityEngine.Vector3(x, y, 0);;
        }
        
        public static UnityEngine.Vector3 GetLeftBottomLocalPosition(this UnityEngine.RectTransform transform)
        {
            var x = - transform.pivot.x * transform.sizeDelta.x;
            var y = - transform.pivot.y * transform.sizeDelta.y;
            return new UnityEngine.Vector3(x, y, 0);;
        }
        
        public static UnityEngine.Vector3 GetLeftTopLocalPosition(this UnityEngine.RectTransform transform)
        {
            var x = - transform.pivot.x * transform.sizeDelta.x;
            var y = transform.sizeDelta.y - transform.pivot.y * transform.sizeDelta.y;
            return new UnityEngine.Vector3(x, y, 0);;
        }
        
        public static UnityEngine.Vector3 GetLeftMiddleLocalPosition(this UnityEngine.RectTransform transform)
        {
            var x = - transform.pivot.x * transform.sizeDelta.x;
            var y = transform.sizeDelta.y * 0.5f - transform.pivot.y * transform.sizeDelta.y;
            return new UnityEngine.Vector3(x, y, 0);;
        }
    }
}