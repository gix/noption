namespace NOption.Tests.Extensions
{
    using System;
    using System.Collections.Generic;
    using NOption.Extensions;
    using Xunit;

    public class SortExtensionsTest
    {
        private IComparer<int> IntComparer
        {
            get { return new FunctorComparer<int>((l, r) => r.CompareTo(l)); }
        }

        private Comparison<int> IntComparison
        {
            get { return (l, r) => r.CompareTo(l); }
        }

        private class X
        {
            public X(int a, string b)
            {
                A = a;
                B = b;
            }

            public int A { get; }
            public string B { get; }
        }

        [Fact]
        public void SortBy()
        {
            var input = new List<X> {
                new X(0, "30"),
                new X(1, "20"),
                new X(2, null),
                new X(3, "10"),
            };

            var actual = SortExtensions.SortBy(input, x => x.B);
            Assert.Equal(2, input[0].A);
            Assert.Equal(3, input[1].A);
            Assert.Equal(1, input[2].A);
            Assert.Equal(0, input[3].A);
            Assert.Same(input, actual);
        }

        [Fact]
        public void StableSortBy()
        {
            var input = new List<X> {
                new X(0, "30"),
                new X(1, "20"),
                new X(2, null),
                new X(3, "10"),
            };

            var actual = SortExtensions.StableSortBy(input, x => x.B);
            Assert.Equal(2, input[0].A);
            Assert.Equal(3, input[1].A);
            Assert.Equal(1, input[2].A);
            Assert.Equal(0, input[3].A);
            Assert.Same(input, actual);
        }

        [Theory]
        [InlineData(new int[0], new int[0])]
        [InlineData(new[] { 2 }, new[] { 2 })]
        [InlineData(new[] { 2, 1, 4 }, new[] { 1, 2, 4 })]
        [InlineData(new[] { 2, 1, 4, 3, 5 }, new[] { 1, 2, 3, 4, 5 })]
        public void StableSort(int[] input, int[] expected)
        {
            var actual = SortExtensions.StableSort(input);
            Assert.Equal(expected, actual);
            Assert.Same(input, actual);
        }

        [Fact]
        public void StableSort_Preconditions()
        {
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.StableSort((IList<int>)null));
        }

        [Theory]
        [InlineData(new int[0], new int[0])]
        [InlineData(new[] { 2 }, new[] { 2 })]
        [InlineData(new[] { 2, 1, 4 }, new[] { 4, 2, 1 })]
        [InlineData(new[] { 2, 1, 4, 3, 5 }, new[] { 5, 4, 3, 2, 1 })]
        public void StableSort_Comparer(int[] input, int[] expected)
        {
            var actual = SortExtensions.StableSort(input, IntComparer);
            Assert.Equal(expected, actual);
            Assert.Same(input, actual);
        }

        [Fact]
        public void StableSort_Comparer_Preconditions()
        {
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.StableSort(null, IntComparer));
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.StableSort(new[] { 1 }, (IComparer<int>)null));
        }

        [Theory]
        [InlineData(new int[0], new int[0])]
        [InlineData(new[] { 2 }, new[] { 2 })]
        [InlineData(new[] { 2, 1, 4 }, new[] { 4, 2, 1 })]
        [InlineData(new[] { 2, 1, 4, 3, 5 }, new[] { 5, 4, 3, 2, 1 })]
        public void StableSort_Comparison(int[] input, int[] expected)
        {
            var actual = SortExtensions.StableSort(input, (l, r) => r.CompareTo(l));
            Assert.Equal(expected, actual);
            Assert.Same(input, actual);
        }

        [Fact]
        public void StableSort_Comparison_Preconditions()
        {
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.StableSort(null, IntComparison));
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.StableSort(new[] { 1 }, (Comparison<int>)null));
        }

        [Theory]
        [InlineData(new int[0], 0, 0, new int[0])]
        [InlineData(new[] { 2 }, 0, 1, new[] { 2 })]
        [InlineData(new[] { 5, 4, 3, 2, 1 }, 0, 3, new[] { 3, 4, 5, 2, 1 })]
        [InlineData(new[] { 5, 4, 3, 2, 1 }, 1, 3, new[] { 5, 2, 3, 4, 1 })]
        [InlineData(new[] { 5, 4, 3, 2, 1 }, 2, 3, new[] { 5, 4, 1, 2, 3 })]
        [InlineData(new[] { 5, 4, 3, 2, 1 }, 0, 5, new[] { 1, 2, 3, 4, 5 })]
        public void PartialStableSort(int[] input, int index, int count, int[] expected)
        {
            var actual = SortExtensions.StableSort(input, index, count);
            Assert.Equal(expected, actual);
            Assert.Same(input, actual);
        }

        [Fact]
        public void PartialStableSort_Preconditions()
        {
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.StableSort((IList<int>)null, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => SortExtensions.StableSort(new[] { 1, 2 }, -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => SortExtensions.StableSort(new[] { 1, 2 }, 0, -1));
            Assert.Throws<ArgumentException>(
                () => SortExtensions.StableSort(new[] { 1, 2 }, 2, 1));
            Assert.Throws<ArgumentException>(
                () => SortExtensions.StableSort(new[] { 1, 2 }, 0, 3));
        }

        [Theory]
        [InlineData(new int[0], 0, 0, new int[0])]
        [InlineData(new[] { 2 }, 0, 1, new[] { 2 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 0, 3, new[] { 3, 2, 1, 4, 5 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 1, 3, new[] { 1, 4, 3, 2, 5 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 2, 3, new[] { 1, 2, 5, 4, 3 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 0, 5, new[] { 5, 4, 3, 2, 1 })]
        public void PartialStableSort_Comparer(int[] input, int index, int count, int[] expected)
        {
            var actual = SortExtensions.StableSort(input, index, count, IntComparer);
            Assert.Equal(expected, actual);
            Assert.Same(input, actual);
        }

        [Fact]
        public void PartialStableSort_Comparer_Preconditions()
        {
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.StableSort(null, 0, 0, IntComparer));
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.StableSort(new[] { 1, 2 }, 0, 0, (IComparer<int>)null));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => SortExtensions.StableSort(new[] { 1, 2 }, -1, 1, IntComparer));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => SortExtensions.StableSort(new[] { 1, 2 }, 0, -1, IntComparer));
            Assert.Throws<ArgumentException>(
                () => SortExtensions.StableSort(new[] { 1, 2 }, 2, 1, IntComparer));
            Assert.Throws<ArgumentException>(
                () => SortExtensions.StableSort(new[] { 1, 2 }, 0, 3, IntComparer));
        }

        [Theory]
        [InlineData(new int[0], 0, 0, new int[0])]
        [InlineData(new[] { 2 }, 0, 1, new[] { 2 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 0, 3, new[] { 3, 2, 1, 4, 5 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 1, 3, new[] { 1, 4, 3, 2, 5 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 2, 3, new[] { 1, 2, 5, 4, 3 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 0, 5, new[] { 5, 4, 3, 2, 1 })]
        public void PartialStableSort_Comparison(int[] input, int index, int count, int[] expected)
        {
            var actual = SortExtensions.StableSort(input, index, count, IntComparison);
            Assert.Equal(expected, actual);
            Assert.Same(input, actual);
        }

        [Fact]
        public void PartialStableSort_Comparison_Preconditions()
        {
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.StableSort(null, 0, 0, IntComparison));
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.StableSort(new[] { 1, 2 }, 0, 1, (Comparison<int>)null));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => SortExtensions.StableSort(new[] { 1, 2 }, -1, 1, IntComparison));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => SortExtensions.StableSort(new[] { 1, 2 }, 0, -1, IntComparison));
            Assert.Throws<ArgumentException>(
                () => SortExtensions.StableSort(new[] { 1, 2 }, 2, 1, IntComparison));
            Assert.Throws<ArgumentException>(
                () => SortExtensions.StableSort(new[] { 1, 2 }, 0, 3, IntComparison));
        }

        [Theory]
        [InlineData(new int[0], new int[0])]
        [InlineData(new[] { 2 }, new[] { 2 })]
        [InlineData(new[] { 2, 1, 4 }, new[] { 1, 2, 4 })]
        [InlineData(new[] { 2, 1, 4, 3, 5 }, new[] { 1, 2, 3, 4, 5 })]
        public void MergeSort(int[] input, int[] expected)
        {
            var actual = SortExtensions.MergeSort(input);
            Assert.Equal(expected, actual);
            Assert.Same(input, actual);
        }

        [Fact]
        public void MergeSort_Preconditions()
        {
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.MergeSort((IList<int>)null));
        }

        [Theory]
        [InlineData(new int[0], new int[0])]
        [InlineData(new[] { 2 }, new[] { 2 })]
        [InlineData(new[] { 2, 1, 4 }, new[] { 4, 2, 1 })]
        [InlineData(new[] { 2, 1, 4, 3, 5 }, new[] { 5, 4, 3, 2, 1 })]
        public void MergeSort_Comparer(int[] input, int[] expected)
        {
            var actual = SortExtensions.MergeSort(input, IntComparer);
            Assert.Equal(expected, actual);
            Assert.Same(input, actual);
        }

        [Fact]
        public void MergeSort_Comparer_Preconditions()
        {
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.MergeSort(null, IntComparer));
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.MergeSort(new[] { 1 }, (IComparer<int>)null));
        }

        [Theory]
        [InlineData(new int[0], new int[0])]
        [InlineData(new[] { 2 }, new[] { 2 })]
        [InlineData(new[] { 2, 1, 4 }, new[] { 4, 2, 1 })]
        [InlineData(new[] { 2, 1, 4, 3, 5 }, new[] { 5, 4, 3, 2, 1 })]
        public void MergeSort_Comparison(int[] input, int[] expected)
        {
            var actual = SortExtensions.MergeSort(input, (l, r) => r.CompareTo(l));
            Assert.Equal(expected, actual);
            Assert.Same(input, actual);
        }

        [Fact]
        public void MergeSort_Comparison_Preconditions()
        {
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.MergeSort(null, IntComparison));
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.MergeSort(new[] { 1 }, (Comparison<int>)null));
        }

        [Theory]
        [InlineData(new int[0], 0, 0, new int[0])]
        [InlineData(new[] { 2 }, 0, 1, new[] { 2 })]
        [InlineData(new[] { 5, 4, 3, 2, 1 }, 0, 3, new[] { 3, 4, 5, 2, 1 })]
        [InlineData(new[] { 5, 4, 3, 2, 1 }, 1, 3, new[] { 5, 2, 3, 4, 1 })]
        [InlineData(new[] { 5, 4, 3, 2, 1 }, 2, 3, new[] { 5, 4, 1, 2, 3 })]
        [InlineData(new[] { 5, 4, 3, 2, 1 }, 0, 5, new[] { 1, 2, 3, 4, 5 })]
        public void PartialMergeSort(int[] input, int index, int count, int[] expected)
        {
            var actual = SortExtensions.MergeSort(input, index, count);
            Assert.Equal(expected, actual);
            Assert.Same(input, actual);
        }

        [Fact]
        public void PartialMergeSort_Preconditions()
        {
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.MergeSort((IList<int>)null, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => SortExtensions.MergeSort(new[] { 1, 2 }, -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => SortExtensions.MergeSort(new[] { 1, 2 }, 0, -1));
            Assert.Throws<ArgumentException>(
                () => SortExtensions.MergeSort(new[] { 1, 2 }, 2, 1));
            Assert.Throws<ArgumentException>(
                () => SortExtensions.MergeSort(new[] { 1, 2 }, 0, 3));
        }

        [Theory]
        [InlineData(new int[0], 0, 0, new int[0])]
        [InlineData(new[] { 2 }, 0, 1, new[] { 2 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 0, 3, new[] { 3, 2, 1, 4, 5 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 1, 3, new[] { 1, 4, 3, 2, 5 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 2, 3, new[] { 1, 2, 5, 4, 3 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 0, 5, new[] { 5, 4, 3, 2, 1 })]
        public void PartialMergeSort_Comparer(int[] input, int index, int count, int[] expected)
        {
            var actual = SortExtensions.MergeSort(input, index, count, IntComparer);
            Assert.Equal(expected, actual);
            Assert.Same(input, actual);
        }

        [Fact]
        public void PartialMergeSort_Comparer_Preconditions()
        {
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.MergeSort(null, 0, 0, IntComparer));
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.MergeSort(new[] { 1, 2 }, 0, 0, (IComparer<int>)null));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => SortExtensions.MergeSort(new[] { 1, 2 }, -1, 1, IntComparer));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => SortExtensions.MergeSort(new[] { 1, 2 }, 0, -1, IntComparer));
            Assert.Throws<ArgumentException>(
                () => SortExtensions.MergeSort(new[] { 1, 2 }, 2, 1, IntComparer));
            Assert.Throws<ArgumentException>(
                () => SortExtensions.MergeSort(new[] { 1, 2 }, 0, 3, IntComparer));
        }

        [Theory]
        [InlineData(new int[0], 0, 0, new int[0])]
        [InlineData(new[] { 2 }, 0, 1, new[] { 2 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 0, 3, new[] { 3, 2, 1, 4, 5 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 1, 3, new[] { 1, 4, 3, 2, 5 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 2, 3, new[] { 1, 2, 5, 4, 3 })]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 0, 5, new[] { 5, 4, 3, 2, 1 })]
        public void PartialMergeSort_Comparison(int[] input, int index, int count, int[] expected)
        {
            var actual = SortExtensions.MergeSort(input, index, count, IntComparison);
            Assert.Equal(expected, actual);
            Assert.Same(input, actual);
        }

        [Fact]
        public void PartialMergeSort_Comparison_Preconditions()
        {
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.MergeSort(null, 0, 0, IntComparison));
            Assert.Throws<ArgumentNullException>(
                () => SortExtensions.MergeSort(new[] { 1, 2 }, 0, 1, (Comparison<int>)null));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => SortExtensions.MergeSort(new[] { 1, 2 }, -1, 1, IntComparison));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => SortExtensions.MergeSort(new[] { 1, 2 }, 0, -1, IntComparison));
            Assert.Throws<ArgumentException>(
                () => SortExtensions.MergeSort(new[] { 1, 2 }, 2, 1, IntComparison));
            Assert.Throws<ArgumentException>(
                () => SortExtensions.MergeSort(new[] { 1, 2 }, 0, 3, IntComparison));
        }
    }
}
