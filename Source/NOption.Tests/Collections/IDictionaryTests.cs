namespace NOption.Tests.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Xunit;

    public abstract class IDictionaryTests<TKey, TValue>
    {
        protected abstract IDictionary CreateCollection(int entries);
        protected abstract List<KeyValuePair<TKey, TValue>> CreateItems(int entries);

        [Fact]
        public void Enumerate()
        {
            var dictionary = CreateCollection(3);
            var items = CreateItems(3);

            IDictionary d = dictionary;
            IDictionaryEnumerator enumerator = d.GetEnumerator();

            foreach (var pair in items) {
                Assert.True(enumerator.MoveNext());
#pragma warning disable DE0006 // API is deprecated
                Assert.Equal(new DictionaryEntry(pair.Key, pair.Value), enumerator.Current);
#pragma warning restore DE0006 // API is deprecated
                Assert.Equal(pair.Key, enumerator.Entry.Key);
                Assert.Equal(pair.Value, enumerator.Entry.Value);
                Assert.Equal(pair.Key, enumerator.Key);
                Assert.Equal(pair.Value, enumerator.Value);
            }

            Assert.False(enumerator.MoveNext());

            enumerator.Reset();
            Assert.True(enumerator.MoveNext());
            Assert.Equal(items[0].Key, enumerator.Key);
        }

        [Fact]
        public void Contains()
        {
            var dictionary = CreateCollection(2);
            var items = CreateItems(3);

            Assert.True(dictionary.Contains(items[0].Key));
            Assert.True(dictionary.Contains(items[1].Key));
            Assert.False(dictionary.Contains(items[2].Key));
            Assert.Throws<ArgumentNullException>(() => dictionary.Contains(null));
        }

        [Fact]
        public void GetItem()
        {
            var dictionary = CreateCollection(2);
            var items = CreateItems(3);

            Assert.Equal(items[0].Value, dictionary[items[0].Key]);
            Assert.Equal(items[1].Value, dictionary[items[1].Key]);
            Assert.Null(dictionary[items[2].Key]);
        }

        [Fact]
        public void SetItem_Add()
        {
            var dictionary = CreateCollection(2);
            var items = CreateItems(3);

            dictionary[items[2].Key] = items[2].Value;

            Assert.Equal(3, dictionary.Count);
            Assert.Equal(items[0].Value, dictionary[items[0].Key]);
            Assert.Equal(items[1].Value, dictionary[items[1].Key]);
            Assert.Equal(items[2].Value, dictionary[items[2].Key]);
        }

        [Fact]
        public void SetItem_Replace()
        {
            var dictionary = CreateCollection(2);
            var items = CreateItems(3);

            dictionary[items[1].Key] = items[2].Value;

            Assert.Equal(2, dictionary.Count);
            Assert.Equal(items[0].Value, dictionary[items[0].Key]);
            Assert.Equal(items[2].Value, dictionary[items[1].Key]);
        }

        [Fact]
        public void Add()
        {
            var dictionary = CreateCollection(0);
            var items = CreateItems(1);

            dictionary.Add(items[0].Key, items[0].Value);

            Assert.Equal(1, dictionary.Count);
            Assert.Equal(items[0].Value, dictionary[items[0].Key]);
        }

        [Fact]
        public void Remove()
        {
            var dictionary = CreateCollection(2);
            var items = CreateItems(3);

            dictionary.Remove(items[2].Key);
            Assert.Equal(items[0].Value, dictionary[items[0].Key]);
            Assert.Equal(items[1].Value, dictionary[items[1].Key]);

            dictionary.Remove(items[0].Key);
            Assert.Equal(1, dictionary.Count);
            Assert.Equal(items[1].Value, dictionary[items[1].Key]);
        }
    }
}