using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bro
{
    public static class DictionaryExtensions
    {

        public static TValue FindValue<TKey, TValue>(this Dictionary<TKey, TValue> source, Predicate<TValue> matchPredicate)
        {
            foreach (var item in source)
            {
                if (matchPredicate(item.Value))
                {
                    return item.Value;
                }
            }
            return default(TValue);
        }
        public static List<TValue> FindValues<TKey, TValue>(this Dictionary<TKey, TValue> source, Predicate<TValue> matchPredicate)
        {
            var result = new List<TValue>();
            foreach (var item in source)
            {
                if (matchPredicate(item.Value))
                {
                    result.Add(item.Value);
                }
            }
            return result;
        }
        public static Dictionary<TKey, TValue> AddRange<TKey, TValue>(this Dictionary<TKey, TValue> mergeTarget, Dictionary<TKey, TValue> mergeSource)
        {
            foreach (var val in mergeSource)
            {
                if (mergeTarget.ContainsKey(val.Key))
                {
                    Log.Error("Cant merge dictionaries cause have not resolved key");
                    continue;
                }

                mergeTarget.Add(val.Key, val.Value);
            }

            return mergeTarget;
        }

        public static bool TryAddValue<TKey, TValue>(this Dictionary<TKey, TValue> target, TKey key, TValue value)
        {
            if (target.ContainsKey(key))
            {
                return false;
            }
            
            target.Add(key, value);
            return true;
        }

        public static void RemoveRange<TKey, TValue>(this Dictionary<TKey, TValue> target, IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                target.Remove(key);
            }
        }

        /*** FastTryGetValue ***/
        /* да это быстрее */
        /* да это лучшее */
        /* да тут 0 аллокаций */   /* неа,  target.GetEnumerator(); - вот тут есть аллокации =) */ 
        /* да только для моно */
        /* посмотри на меня, а теперь на свой try get, а теперь опять на меня.. да, я на коне */

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static bool FastTryGetValue<TValue>(this IDictionary<int, TValue> target, int key, out TValue value)
//        {
//            var enumerator = target.GetEnumerator();
//            try
//            {
//                while (enumerator.MoveNext())
//                {
//                    var element = enumerator.Current;
//                    if (element.Key == key)
//                    {
//                        value = element.Value;
//                        return true;
//                    }
//                }
//            }
//            finally
//            {
//                enumerator.Dispose();
//            }
//
//            value = default(TValue);
//            return false;
//        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastTryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> target, TKey key, out TValue value)
        {
            if (target.ContainsKey(key))
            {
                value = target[key];
                return true;
            }
            value = default(TValue);
            return false;
//            var enumerator = target.GetEnumerator();
//            try
//            {
//                while (enumerator.MoveNext())
//                {
//                    var element = enumerator.Current;
//                    if (key.Equals(element.Key))
//                    {
//                        value = element.Value;
//                        return true;
//                    }
//                }
//            }
//            finally
//            {
//                enumerator.Dispose();
//            }
//
//            value = default(TValue);
//            return false;
        }
    }
}