namespace NOption.Tests.Collections
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public abstract class IReadOnlyDictionaryTests<TKey, TValue>
    {
        protected abstract IReadOnlyDictionary<TKey, TValue> CreateCollection(int entries);
        protected abstract List<KeyValuePair<TKey, TValue>> CreateItems(int entries);

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
        public void ContainsKey()
        {
            var collection = CreateCollection(2);
            var items = CreateItems(3);
            Assert.True(collection.ContainsKey(items[0].Key));
            Assert.True(collection.ContainsKey(items[1].Key));
            Assert.False(collection.ContainsKey(items[2].Key));
        }

        [Fact]
        public void TryGetValue()
        {
            var collection = CreateCollection(2);
            var items = CreateItems(3);
            Assert.True(collection.TryGetValue(items[0].Key, out var val1));
            Assert.True(collection.TryGetValue(items[1].Key, out var val2));
            Assert.False(collection.TryGetValue(items[2].Key, out var val3));
            Assert.Equal(items[0].Value, val1);
            Assert.Equal(items[1].Value, val2);
            Assert.Equal(default, val3);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void Keys(int count)
        {
            var collection = CreateCollection(count);
            var items = CreateItems(count);
            Assert.Equal(items.Select(x => x.Key), collection.Keys);
            Assert.Equal(items.Select(x => x.Value), collection.Values);
        }
    }
}
