namespace NOption.Tests.Collections
{
    using System.Collections.Generic;
    using Xunit;

    public abstract class IReadOnlyListTests<T>
    {
        protected abstract IReadOnlyList<T> CreateCollection(int entries);
        protected abstract List<T> CreateItems(int entries);

        [Fact]
        public void Empty()
        {
            var collection = CreateCollection(0);
            Assert.Equal(0, collection.Count);
            Assert.Empty(collection);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Count(int count)
        {
            var collection = CreateCollection(count);
            Assert.Equal(count, collection.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Enumerate(int count)
        {
            var collection = CreateCollection(count);
            var items = CreateItems(count);
            Assert.Equal(items, collection);
        }

        [Fact]
        public void Indexer()
        {
            var collection = CreateCollection(2);
            var items = CreateItems(2);
            Assert.Equal(items[0], collection[0]);
            Assert.Equal(items[1], collection[1]);
        }
    }
}
