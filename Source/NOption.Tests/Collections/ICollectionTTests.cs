namespace NOption.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    public abstract class ICollectionTTests<T>
    {
        protected abstract ICollection<T> CreateCollection(int entries);
        protected abstract List<T> CreateItems(int entries);
        protected abstract bool IsReadOnly { get; }

        [Fact]
        public void Empty()
        {
            var collection = CreateCollection(0);
            var items = CreateItems(1);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
            Assert.Equal(IsReadOnly, collection.IsReadOnly);
            Assert.False(collection.Contains(items[0]));
        }

        [Fact]
        public void Contains()
        {
            var collection = CreateCollection(2);
            var items = CreateItems(3);
            Assert.Equal(2, collection.Count);
            Assert.Equal(new[] { items[0], items[1] }, collection);
            Assert.Equal(IsReadOnly, collection.IsReadOnly);
            Assert.True(collection.Contains(items[0]));
            Assert.True(collection.Contains(items[1]));
            Assert.False(collection.Contains(items[2]));
        }

        [Fact]
        public void Clear()
        {
            var collection = CreateCollection(3);

            if (IsReadOnly) {
                Assert.Throws<NotSupportedException>(() => collection.Clear());
            } else {
                collection.Clear();
                Assert.Empty(collection);
            }
        }

        [Fact]
        public void Add()
        {
            var collection = CreateCollection(2);
            var items = CreateItems(3);

            if (IsReadOnly) {
                Assert.Throws<NotSupportedException>(() => collection.Add(items[2]));
            } else {
                collection.Add(items[2]);
                Assert.Equal(items, collection);
            }
        }

        [Fact]
        public void Remove()
        {
            var collection = CreateCollection(2);
            var items = CreateItems(3);

            if (IsReadOnly) {
                Assert.Throws<NotSupportedException>(() => collection.Remove(items[1]));
            } else {
                collection.Remove(items[1]);
                Assert.Single(collection, items[0]);
                collection.Remove(items[2]);
                Assert.Single(collection, items[0]);
            }
        }

        [Fact]
        public void CopyTo()
        {
            var collection = CreateCollection(3);
            var items = CreateItems(3);
            Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(new T[3], -1));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new T[2], 0));

            var array = new T[3];
            collection.CopyTo(array, 0);
            Assert.Equal(array, array);

            var array2 = new T[4];
            collection.CopyTo(array2, 1);
            Assert.Equal(new[] { default, array[0], array[1], array[2] }, array2);
        }
    }
}
