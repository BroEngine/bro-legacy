#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT )

using UnityEngine;
using UnityEngine.UI;

namespace Bro.Toolbox.Client.UI
{
    public static class ContentFitterRefresh
    {
        public static void RefreshContentFitters(Transform transform)
        {
            if (transform is RectTransform)
            {
                RefreshContentFitter((RectTransform)transform);
            }
        }
 
        private static void RefreshContentFitter(RectTransform transform)
        {
            if (transform == null || !transform.gameObject.activeSelf)
            {
                return;
            }
     
            foreach (var child in transform)
            {
                if (child is RectTransform)
                {
                    RefreshContentFitter((RectTransform) child);
                }
            }
 
            var layoutGroup = transform.GetComponent<LayoutGroup>();
            var contentSizeFitter = transform.GetComponent<ContentSizeFitter>();
            if (layoutGroup != null)
            {
                layoutGroup.SetLayoutHorizontal();
                layoutGroup.SetLayoutVertical();
            }
 
            if (contentSizeFitter != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
            }
        }
    }
}
#endif