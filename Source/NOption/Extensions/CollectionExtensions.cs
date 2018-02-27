namespace NOption.Extensions
{
    using System;
    using System.Collections.Generic;

    internal static class CollectionExtensions
    {
        public static IList<T> AddRange<T>(this IList<T> list, IEnumerable<T> range)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (range == null)
                return list;

            foreach (var item in range)
                list.Add(item);

            return list;
        }

        public static ISet<T> AddRange<T>(this ISet<T> set, IEnumerable<T> range)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            if (range == null)
                return set;

            foreach (var item in range)
                set.Add(item);

            return set;
        }

        public static int WeakPredecessor<T>(
            this IReadOnlyList<T> list, T value)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            Contract.Ensures(Contract.Result<int>() >= 0);

            return WeakPredecessor(list, 0, list.Count, value, Comparer<T>.Default.Compare);
        }

        public static int WeakPredecessor<T>(
            this IReadOnlyList<T> list, T value, IComparer<T> comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            Contract.Ensures(Contract.Result<int>() >= 0);

            return WeakPredecessor(list, 0, list.Count, value, comparer.Compare);
        }

        public static int WeakPredecessor<T, TValue>(
            this IReadOnlyList<T> list, TValue value, Func<T, TValue, int> comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            Contract.Ensures(Contract.Result<int>() >= 0);

            return WeakPredecessor(list, 0, list.Count, value, comparer);
        }

        public static int WeakPredecessor<T, TValue>(
            this IReadOnlyList<T> list, int index, int count, TValue value,
            Func<T, TValue, int> comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Contract violated: index >= 0");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Contract violated: count >= 0");
            if (!(list.Count - index >= count))
                throw new ArgumentException("Contract violated: list.Count - index >= count");
            Contract.Ensures(Contract.Result<int>() >= 0);

            while (count > 0) {
                int half = count / 2;
                int mid = index + half;

                if (comparer(list[mid], value) < 0) {
                    // Value in upper half.
                    index = mid + 1;
                    count -= half + 1;
                } else {
                    // Value in lower half.
                    count = half;
                }
            }

            return index;
        }

        public static int WeakSuccessor<T>(
            this IReadOnlyList<T> list, T value)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            Contract.Ensures(Contract.Result<int>() >= 0);
            Contract.Ensures(Contract.Result<int>() <= list.Count);

            return WeakSuccessor(list, 0, list.Count, value, Comparer<T>.Default.Compare);
        }

        public static int WeakSuccessor<T>(
            this IReadOnlyList<T> list, T value, IComparer<T> comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            Contract.Ensures(Contract.Result<int>() >= 0);
            Contract.Ensures(Contract.Result<int>() <= list.Count);

            return WeakSuccessor(list, 0, list.Count, value, comparer.Compare);
        }

        public static int WeakSuccessor<T, TValue>(
            this IReadOnlyList<T> list, TValue value, Func<T, TValue, int> comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            Contract.Ensures(Contract.Result<int>() >= 0);
            Contract.Ensures(Contract.Result<int>() <= list.Count);

            return WeakSuccessor(list, 0, list.Count, value, comparer);
        }

        public static int WeakSuccessor<T, TValue>(
            this IReadOnlyList<T> list, int index, int count, TValue value,
            Func<T, TValue, int> comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Contract violated: index >= 0");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Contract violated: count >= 0");
            if (!(list.Count - index >= count))
                throw new ArgumentException("Contract violated: list.Count - index >= count");
            Contract.Ensures(Contract.Result<int>() >= index);
            Contract.Ensures(Contract.Result<int>() <= index + count);

            while (count > 0) {
                int half = count / 2;
                int mid = index + half;

                if (comparer(list[mid], value) <= 0) {
                    // Value in upper half.
                    index = mid + 1;
                    count -= half + 1;
                } else {
                    // Value in lower half.
                    count = half;
                }
            }

            return index;
        }

        public static IEnumerable<T> EqualRange<T>(
            this IReadOnlyList<T> list, T value)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return EqualRange(list, 0, list.Count, value, Comparer<T>.Default.Compare);
        }

        public static IEnumerable<T> EqualRange<T>(
            this IReadOnlyList<T> list, T value, IComparer<T> comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return EqualRange(list, 0, list.Count, value, comparer.Compare);
        }

        public static IEnumerable<T> EqualRange<T, TValue>(
            this IReadOnlyList<T> list, TValue value, Func<T, TValue, int> comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return EqualRange(list, 0, list.Count, value, comparer);
        }

        public static IEnumerable<T> EqualRange<T, TValue>(
            this IReadOnlyList<T> list, int index, int count, TValue value,
            Func<T, TValue, int> comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Contract violated: index >= 0");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Contract violated: count >= 0");
            if (list.Count - index < count)
                throw new ArgumentException("Contract violated: list.Count - index >= count");
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            int idx = list.WeakPredecessor(index, count, value, comparer);
            int end = list.WeakSuccessor(index, count, value, comparer);

            // Move the iterator block out of this method so that the contract
            // is checked now instead of when iterating.
            return Iterate(list, idx, end);
        }

        private static IEnumerable<T> Iterate<T>(IReadOnlyList<T> list, int idx, int end)
        {
            for (; idx < end; ++idx)
                yield return list[idx];
        }
    }
}
