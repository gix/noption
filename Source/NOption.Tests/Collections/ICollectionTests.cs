namespace NOption.Tests.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public abstract class ICollectionTests<T>
    {
        protected abstract ICollection CreateCollection(int entries);
        protected abstract List<T> CreateItems(int entries);

        [Fact]
        public void Empty()
        {
            ICollection collection = CreateCollection(0);
            Assert.Equal(0, collection.Count);
            Assert.False(collection.IsSynchronized);
            Assert.NotNull(collection.SyncRoot);
        }

        [Fact]
        public void Filled()
        {
            var collection = CreateCollection(3);
            var items = CreateItems(3);
            Assert.Equal(3, collection.Count);
            Assert.Equal(items, collection);
            Assert.False(collection.IsSynchronized);
            Assert.NotNull(collection.SyncRoot);
        }

        [Fact]
        public void CopyTo()
        {
            var collection = CreateCollection(3);
            Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null, 0));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new T[3, 3], 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(new T[3], -1));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new T[2], 0));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new byte[3], 0));
            Assert.Throws<ArgumentException>(() => collection.CopyTo(new X[3], 0));

            var array = new T[3];
            collection.CopyTo(array, 0);
            Assert.Equal(array, array);

            var array2 = new T[4];
            collection.CopyTo(array2, 1);
            Assert.Equal(new[] { default, array[0], array[1], array[2] }, array2);

            var array3 = new object[3];
            collection.CopyTo(array3, 0);
            Assert.Equal(array.Cast<object>(), array3);
        }

        private class X
        {
        }
    }
}
