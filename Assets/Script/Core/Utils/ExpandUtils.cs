using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Logic.Core.Utils
{
    public static class ExpandUtils
    {
        public static Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(this IEnumerable<TElement> source, Func<TElement, TKey> keySelector)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (keySelector == null)
                throw new ArgumentNullException("keySelector");
            Dictionary<TKey, TElement> dic = new Dictionary<TKey, TElement>();
            foreach (TElement item in source)
            {
                TKey t = keySelector(item);
                if (!dic.ContainsKey(t))
                {
                    dic.Add(t, item);
                }
                else
                {
                    Debug.LogError("An element with the same key already exists in the dictionary, key value is " + t);
                }
            }
            return dic;
        }
    }
}
