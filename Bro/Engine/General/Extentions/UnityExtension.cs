#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII

using System.Collections.Generic;
using UnityEngine;

namespace Bro
{
    public static class UnityExtension
    {
        public static ValType GetSafe<KeyType, ValType>(this IDictionary<KeyType, ValType> self, KeyType key)
        {
            if (self.ContainsKey(key))
            {
                return self[key];
            }
            return default(ValType);
        }

        public static void ClearHierarchy(this Transform transform)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i).gameObject;

                child.SetActive(false);
                if (Application.isPlaying)
                {
                    Object.Destroy(child);
                }
                else
                {
                    Object.DestroyImmediate(child);
                }
            }
        }

        public static bool IsNull(this UnityEngine.Object o)
        {
            return o == null || !o;
        }


        public static void DebugDraw(this UnityEngine.Rect rect)
        {
            Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y ),Color.green);
            Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x , rect.y + rect.height), Color.red);
            Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y), Color.green);
            Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x, rect.y + rect.height), Color.red);
        }
    }
}
#endif