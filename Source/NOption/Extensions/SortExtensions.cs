namespace NOption.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Collections;

    public static class SortExtensions
    {
        public static List<T> SortBy<T, TResult>(
            this List<T> list, Func<T, TResult> selector) where TResult : IComparable<TResult>
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            list.Sort(CreateSelectorComparison(selector));
            return list;
        }

        public static IList<T> StableSortBy<T, TResult>(
            this IList<T> list, Func<T, TResult> selector) where TResult : IComparable<TResult>
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            return list.StableSort(CreateSelectorComparison(selector));
        }

        private static Comparison<T> CreateSelectorComparison<T, TResult>(Func<T, TResult> selector)
            where TResult : IComparable<TResult>
        {
            return (x, y) => Comparer<TResult>.Default.Compare(selector(x), selector(y));
        }

        public static IList<T> StableSort<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            return UncheckedMergeSort(list, 0, list.Count, Comparer<T>.Default.Compare);
        }

        public static IList<T> StableSort<T>(this IList<T> list, Comparison<T> comparison)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparison == null)
                throw new ArgumentNullException(nameof(comparison));
            return UncheckedMergeSort(list, 0, list.Count, comparison);
        }

        public static IList<T> StableSort<T>(this IList<T> list, IComparer<T> comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            return UncheckedMergeSort(list, 0, list.Count, comparer.Compare);
        }

        public static IList<T> StableSort<T>(
            this IList<T> list, int index, int count)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Contract violated: index >= 0");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Contract violated: count >= 0");
            if (!(index + count <= list.Count))
                throw new ArgumentException("Contract violated: index + count <= list.Count");
            return UncheckedMergeSort(list, index, count, Comparer<T>.Default.Compare);
        }

        public static IList<T> StableSort<T>(
            this IList<T> list, int index, int count, Comparison<T> comparison)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparison == null)
                throw new ArgumentNullException(nameof(comparison));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Contract violated: index >= 0");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Contract violated: count >= 0");
            if (!(index + count <= list.Count))
                throw new ArgumentException("Contract violated: index + count <= list.Count");
            return UncheckedMergeSort(list, index, count, comparison);
        }

        public static IList<T> StableSort<T>(
            this IList<T> list, int index, int count, IComparer<T> comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Contract violated: index >= 0");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Contract violated: count >= 0");
            if (!(index + count <= list.Count))
                throw new ArgumentException("Contract violated: index + count <= list.Count");
            return UncheckedMergeSort(list, index, count, comparer.Compare);
        }

        public static IList<T> MergeSort<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            return UncheckedMergeSort(list, 0, list.Count, Comparer<T>.Default.Compare);
        }

        public static IList<T> MergeSort<T>(this IList<T> list, Comparison<T> comparison)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparison == null)
                throw new ArgumentNullException(nameof(comparison));
            return UncheckedMergeSort(list, 0, list.Count, comparison);
        }

        public static IList<T> MergeSort<T>(this IList<T> list, IComparer<T> comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            return UncheckedMergeSort(list, 0, list.Count, comparer.Compare);
        }

        public static IList<T> MergeSort<T>(
            this IList<T> list, int index, int count)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Contract violated: index >= 0");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Contract violated: count >= 0");
            if (!(index + count <= list.Count))
                throw new ArgumentException("Contract violated: index + count <= list.Count");
            return UncheckedMergeSort(list, index, count, Comparer<T>.Default.Compare);
        }

        public static IList<T> MergeSort<T>(
            this IList<T> list, int index, int count, Comparison<T> comparison)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparison == null)
                throw new ArgumentNullException(nameof(comparison));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Contract violated: index >= 0");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Contract violated: count >= 0");
            if (!(index + count <= list.Count))
                throw new ArgumentException("Contract violated: index + count <= list.Count");
            return UncheckedMergeSort(list, index, count, comparison);
        }

        public static IList<T> MergeSort<T>(
            this IList<T> list, int index, int count, IComparer<T> comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Contract violated: index >= 0");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Contract violated: count >= 0");
            if (!(index + count <= list.Count))
                throw new ArgumentException("Contract violated: index + count <= list.Count");
            return UncheckedMergeSort(list, index, count, comparer.Compare);
        }

        private static IList<T> UncheckedMergeSort<T>(
            this IList<T> list, int index, int count, Comparison<T> compare)
        {
            if (count == 0)
                return list;

            MergeSorter.Sort(list.ToList(), list, index, index + count, compare);
            return list;
        }
    }
}
