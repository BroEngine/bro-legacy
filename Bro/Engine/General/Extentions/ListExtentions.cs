using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bro
{
    public static class ListExtensions
    {
        public static bool Contains<T>(this System.Collections.Generic.IList<T> list, Predicate<T> match)
        {
            for (int i = 0, max = list.Count; i < max; ++i)
            {
                if (match(list[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool FastRemoveFirst<T>(this System.Collections.Generic.IList<T> source, Predicate<T> match)
        {
            for (int i = 0, max = source.Count; i < max; ++i)
            {
                if (match(source[i]))
                {
                    FastRemoveAtIndex(source, i);
                    return true;
                }
            }
            return false;
        }
        
        public static int FastRemoveAll<T>(this System.Collections.Generic.IList<T> source, Predicate<T> match)
        {
            var count = 0;
            for (int i = source.Count - 1; i >= 0; --i)
            {
                if (match(source[i]))
                {
                    ++count;
                    source.FastRemoveAtIndex(i);
                }
            }
            return count;
        }

        public static void FastRemoveAtIndex<T>(this System.Collections.Generic.IList<T> source, int removeElementIndex)
        {
            var lastElemIndex = source.Count - 1;
            source[removeElementIndex] = source[lastElemIndex];
            source.RemoveAt(lastElemIndex);
        }

        public static bool FastRemove<T>(this System.Collections.Generic.IList<T> source, T element) where T : class
        {
            for (int i = 0, max = source.Count; i < max; ++i)
            {
                if (source[i] == element)
                {
                    FastRemoveAtIndex(source, i);
                    return true;
                }
            }

            return false;
        }

        public static void Resize<T>(this System.Collections.Generic.List<T> source, int newSize, T defaultValue)
        {
            int curSize = source.Count;
            if (newSize < curSize)
            {
                source.RemoveRange(newSize, curSize - newSize);
            }
            else if (newSize > curSize)
            {
                if (newSize > source.Capacity)
                {
                    source.Capacity = newSize;
                }

                source.AddRange(System.Linq.Enumerable.Repeat(defaultValue, newSize - curSize));
            }
        }

        public static T Random<T>(this System.Collections.Generic.IList<T> list) => list[Bro.Random.Instance.Range(0, list.Count)];
        
        public static int FastRemoveRange<T>(this System.Collections.Generic.IList<T> source, System.Collections.Generic.IEnumerable<T> itemsToRemove)
        {
            var count = 0;
            for (int i = source.Count - 1; i >= 0; --i)
            {
                if (itemsToRemove.Contains(source[i]))
                {
                    ++count;
                    source.FastRemoveAtIndex(i);
                }
            }
            return count;
        }
        
        public static System.Collections.Generic.List<T> Where<T>(this System.Collections.Generic.IReadOnlyList<T> source, Predicate<T> condition)
        {
            var result = new System.Collections.Generic.List<T>();
            foreach (var item in source)
            {
                if (condition(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }
        
        public static string ToString<T>(this System.Collections.Generic.IReadOnlyList<T> source, Func<T,string> itemString, string separator = null)
        {
            if (separator == null)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var item in source)
                {
                    builder.Append(itemString(item));
                }

                return builder.ToString();
            }
            
            var strings = new string[source.Count];
            for (var index = 0; index < source.Count; index++)
            {
                strings[index] = itemString(source[index]);
            }

            return string.Join(separator, strings);
        }
        
        #if UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
        public static void SortAndSibling<T>(this List<T> list, Comparison<T> comparison, int offset = 0) where T : UnityEngine.MonoBehaviour
        {
            list.Sort(comparison);
            for (var i = 0; i < list.Count; ++i)
            {
                list[i].gameObject.transform.SetSiblingIndex(offset + i);
            }
             
            // examples
            // result: 0,1,2,3,4,5
            // _items.SortAndSibling((a, b) =>
            // {
            //     return  a.Index.CompareTo(b.Index);
            // });
            //
            // result: 5,4,3,2,1,0
            // _items.SortAndSibling((a, b) =>
            // {
            //     return -1 * a.Index.CompareTo(b.Index);
            // });
        }
        #endif
    }
}