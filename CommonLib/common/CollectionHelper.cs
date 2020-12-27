using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MvvmCharting.Common
{
    public static class CollectionHelper
    {
        public static void AddRange<T>(this Collection<T> source, IEnumerable<T> list)
        {
            if (list == null)
            {
                return;
            }
            foreach (var item in list)
            {
                source.Add(item);
            }
        }

        public static void RemoveRange<T>(this Collection<T> source, int startIndex, int count)
        {
            if (startIndex > source.Count - 1)
            {
                return;
            }

            int endIndex = startIndex + count;
            endIndex = Math.Min(endIndex, source.Count - 1);

            for (int i = endIndex; i >= startIndex; i--)
            {
                source.RemoveAt(i);
            }
        }

        public static int Remove<T>(this Collection<T> source, Func<T, bool> predicate)
        {
            int ct = 0;
            for (int i = source.Count - 1; i >= 0; i--)
            {
                var item = source[i];
                if (predicate(item))
                {
                    source.RemoveAt(i);
                    ct++;
                }
            }

            return ct;
        }
    }
}