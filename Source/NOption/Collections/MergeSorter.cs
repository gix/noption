namespace NOption.Collections
{
    using System;
    using System.Collections.Generic;

    internal static class MergeSorter
    {
        public static void Sort<T>(
            IList<T> src, IList<T> dst, int startIndex, int endIndex, Comparison<T> compare)
        {
            if (endIndex - startIndex < 2)
                return;

            int middle = (startIndex + endIndex) / 2;
            Sort(dst, src, startIndex, middle, compare);
            Sort(dst, src, middle, endIndex, compare);
            Merge(src, dst, startIndex, middle, endIndex, compare);
        }

        private static void Merge<T>(
            IList<T> src, IList<T> dst, int start, int middle, int end, Comparison<T> compare)
        {
            int l = start;
            int r = middle;

            for (int k = start; k < end; ++k) {
                if (l < middle && (r >= end || compare(src[l], src[r]) <= 0))
                    dst[k] = src[l++];
                else
                    dst[k] = src[r++];
            }
        }
    }
}
