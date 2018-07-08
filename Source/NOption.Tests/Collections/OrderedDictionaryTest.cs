namespace NOption.Tests.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using NOption.Collections;
    using Xunit;

    public sealed class OrderedDictionaryTest
    {
        [Fact]
        public void Construct()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new OrderedDictionary<string, int>(-1));
            var d1 = new OrderedDictionary<string, int>(null);
            var d2 = new OrderedDictionary<string, int>(10, null);
            var d3 = new OrderedDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var d4 = new OrderedDictionary<string, int>(10, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void Comparer()
        {
            var dictionary = new OrderedDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            dictionary["a"] = 1;
            dictionary["b"] = 2;
            dictionary["A"] = 3;
            Assert.Equal(new[] {
                new KeyValuePair<string, int>("a", 3),
                new KeyValuePair<string, int>("b", 2) },
                dictionary);
            Assert.Equal(2, dictionary["b"]);
            Assert.True(dictionary.ContainsKey("a"));
            Assert.True(dictionary.ContainsKey("A"));
            Assert.True(dictionary.ContainsKey("B"));
            Assert.False(dictionary.ContainsKey("c"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(23)]
        public void Count(int count)
        {
            var dictionary = CreateDict(count);
            Assert.Equal(count, dictionary.Count);
        }

        [Fact]
        public void GetEnumerator()
        {
            var dictionary = new OrderedDictionary<string, int>();
            dictionary.Add("a", 11);
            dictionary.Add("c", 13);
            dictionary.Add("b", 12);

            var pairs = new List<KeyValuePair<string, int>>();
            foreach (var pair in dictionary)
                pairs.Add(pair);

            var actual = pairs.ToArray();
            var expected = new[] {
                    new KeyValuePair<string, int>("a", 11),
                    new KeyValuePair<string, int>("c", 13),
                    new KeyValuePair<string, int>("b", 12),
                };
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Item()
        {
            var dictionary = new OrderedDictionary<string, int>();
            dictionary.Add("a", 11);
            dictionary.Add("c", 13);
            dictionary.Add("b", 12);

            Assert.Equal(11, dictionary["a"]);
            Assert.Equal(13, dictionary["c"]);
            Assert.Equal(12, dictionary["b"]);
        }

        [Fact]
        public void SetItem()
        {
            var dictionary = new OrderedDictionary<string, int>();
            dictionary.Add("a", 11);
            dictionary.Add("c", 13);
            dictionary.Add("b", 12);

            dictionary["c"] = 23;

            Assert.Equal(11, dictionary["a"]);
            Assert.Equal(23, dictionary["c"]);
            Assert.Equal(12, dictionary["b"]);
        }

        [Fact]
        public void GetAt()
        {
            var dictionary = new OrderedDictionary<string, int>();
            dictionary.Add("a", 11);
            dictionary.Add("c", 13);
            dictionary.Add("b", 12);

            Assert.Equal(11, dictionary.GetAt(0));
            Assert.Equal(13, dictionary.GetAt(1));
            Assert.Equal(12, dictionary.GetAt(2));
            Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.GetAt(3));
        }

        [Fact]
        public void SetAt()
        {
            var dictionary = new OrderedDictionary<string, int>();
            dictionary.Add("a", 11);
            dictionary.Add("c", 13);
            dictionary.Add("b", 12);

            dictionary.SetAt(1, 23);
            Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.SetAt(3, 42));

            Assert.Equal(new[] {
                new KeyValuePair<string, int>("a", 11),
                new KeyValuePair<string, int>("c", 23),
                new KeyValuePair<string, int>("b", 12)
            }, dictionary);
        }

        [Fact]
        public void ContainsKey()
        {
            var dictionary = new OrderedDictionary<string, int>();
            dictionary.Add("a", 11);
            dictionary.Add("c", 13);
            dictionary.Add("b", 12);

            Assert.True(dictionary.ContainsKey("a"));
            Assert.True(dictionary.ContainsKey("c"));
            Assert.True(dictionary.ContainsKey("b"));
            Assert.False(dictionary.ContainsKey("d"));
        }

        [Fact]
        public void TryGetValue_Preconditions()
        {
            var dictionary = new OrderedDictionary<string, string>();
            dictionary.Add("a", "11");
            dictionary.Add("c", "13");
            dictionary.Add("b", "12");

            Assert.Throws<ArgumentNullException>(
                () => dictionary.TryGetValue(null, out string _));
        }

        [Fact]
        public void TryGetValue_WithNonExistingKey()
        {
            var dictionary = new OrderedDictionary<string, string>();
            dictionary.Add("a", "11");
            dictionary.Add("c", "13");
            dictionary.Add("b", "12");

            Assert.False(dictionary.TryGetValue("d", out string value));
            Assert.Null(value);
        }

        [Theory]
        [InlineData("a", "11")]
        [InlineData("b", "12")]
        [InlineData("c", "13")]
        public void TryGetValue_WithExistingKey(string key, string value)
        {
            var dictionary = new OrderedDictionary<string, string>();
            dictionary.Add("a", "11");
            dictionary.Add("c", "13");
            dictionary.Add("b", "12");

            Assert.True(dictionary.TryGetValue(key, out string actual));
            Assert.Equal(value, actual);
        }

        [Fact]
        public void Clear()
        {
            var dictionary = new OrderedDictionary<string, string>();
            dictionary.Add("a", "11");
            dictionary.Add("c", "13");
            dictionary.Add("b", "12");

            dictionary.Clear();

            Assert.Empty(dictionary);
            Assert.False(dictionary.ContainsKey("a"));
            Assert.False(dictionary.ContainsKey("b"));
            Assert.False(dictionary.ContainsKey("c"));
            Assert.False(dictionary.TryGetValue("a", out string value));
        }

        [Fact]
        public void Remove_Preconditions()
        {
            var dictionary = new OrderedDictionary<string, string>();
            dictionary.Add("b", "12");

            Assert.Throws<ArgumentNullException>(() => dictionary.Remove(null));
        }

        [Fact]
        public void Remove()
        {
            var dictionary = new OrderedDictionary<string, string>();
            dictionary.Add("a", "11");
            dictionary.Add("c", "13");
            dictionary.Add("b", "12");

            dictionary.Remove("c");

            Assert.Equal(2, dictionary.Count);
            Assert.Equal("11", dictionary["a"]);
            Assert.Equal("12", dictionary["b"]);
            Assert.Equal("11", dictionary.GetAt(0));
            Assert.Equal("12", dictionary.GetAt(1));
            Assert.False(dictionary.ContainsKey("c"));
        }

        [Fact]
        public void Remove_NonExisting()
        {
            var dictionary = new OrderedDictionary<string, string>();
            dictionary.Add("a", "11");
            dictionary.Add("c", "13");
            dictionary.Add("b", "12");

            dictionary.Remove("d");

            Assert.Equal(3, dictionary.Count);
            Assert.Equal("11", dictionary["a"]);
            Assert.Equal("13", dictionary["c"]);
            Assert.Equal("12", dictionary["b"]);
            Assert.Equal("11", dictionary.GetAt(0));
            Assert.Equal("13", dictionary.GetAt(1));
            Assert.Equal("12", dictionary.GetAt(2));
        }

        [Fact]
        public void Insert_Preconditions()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateDict(0).Insert(1, 42, "v"));
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateDict(4).Insert(-1, 42, "v"));
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateDict(5).Insert(6, 42, "v"));
        }

        [Fact]
        public void Insert_Begin()
        {
            var dictionary = new OrderedDictionary<string, int>();
            dictionary.Add("a", 1);
            dictionary.Add("b", 2);

            dictionary.Insert(0, "c", 3);

            Assert.Equal(3, dictionary.Count);
            Assert.Equal(3, dictionary["c"]);
            Assert.Equal(1, dictionary["a"]);
            Assert.Equal(2, dictionary["b"]);
            Assert.Equal(3, dictionary.GetAt(0));
            Assert.Equal(1, dictionary.GetAt(1));
            Assert.Equal(2, dictionary.GetAt(2));
        }

        [Fact]
        public void Insert_Middle()
        {
            var dictionary = new OrderedDictionary<string, int>();
            dictionary.Add("a", 1);
            dictionary.Add("b", 2);

            dictionary.Insert(1, "c", 3);

            Assert.Equal(3, dictionary.Count);
            Assert.Equal(1, dictionary["a"]);
            Assert.Equal(3, dictionary["c"]);
            Assert.Equal(2, dictionary["b"]);
            Assert.Equal(1, dictionary.GetAt(0));
            Assert.Equal(3, dictionary.GetAt(1));
            Assert.Equal(2, dictionary.GetAt(2));
        }

        [Fact]
        public void Insert_End()
        {
            var dictionary = new OrderedDictionary<string, int>();
            dictionary.Add("a", 1);
            dictionary.Add("b", 2);

            dictionary.Insert(2, "c", 3);

            Assert.Equal(3, dictionary.Count);
            Assert.Equal(1, dictionary["a"]);
            Assert.Equal(2, dictionary["b"]);
            Assert.Equal(3, dictionary["c"]);
            Assert.Equal(1, dictionary.GetAt(0));
            Assert.Equal(2, dictionary.GetAt(1));
            Assert.Equal(3, dictionary.GetAt(2));
        }

        [Fact]
        public void Insert_Duplicate()
        {
            var dictionary = new OrderedDictionary<string, int>();
            dictionary.Add("a", 1);
            dictionary.Add("b", 2);

            var ex = Assert.Throws<ArgumentException>(() => dictionary.Insert(1, "a", 3));

            Assert.Contains("same key", ex.Message);
            Assert.Equal(2, dictionary.Count);
            Assert.Equal(1, dictionary["a"]);
            Assert.Equal(2, dictionary["b"]);
            Assert.Equal(1, dictionary.GetAt(0));
            Assert.Equal(2, dictionary.GetAt(1));
        }

        public sealed class Keys : IReadOnlyListTests<int>
        {
            protected override IReadOnlyList<int> CreateCollection(int entries)
            {
                return CreateDict(entries).Keys;
            }

            protected override List<int> CreateItems(int entries)
            {
                return CreateKeys(entries);
            }
        }

        public sealed class Values : IReadOnlyListTests<string>
        {
            protected override IReadOnlyList<string> CreateCollection(int entries)
            {
                return CreateDict(entries).Values;
            }

            protected override List<string> CreateItems(int entries)
            {
                return CreateValues(entries);
            }
        }

        public sealed class AsIReadOnlyDictionary : IReadOnlyDictionaryTests<int, string>
        {
            protected override IReadOnlyDictionary<int, string> CreateCollection(int entries)
            {
                return CreateDict(entries);
            }

            protected override List<KeyValuePair<int, string>> CreateItems(int entries)
            {
                return CreateKeyValues(entries);
            }
        }

        public sealed class AsIDictionaryT
        {
            [Fact]
            public void Empty()
            {
                IDictionary<int, string> dictionary = new OrderedDictionary<int, string>();
                Assert.Empty(dictionary.Keys);
                Assert.Empty(dictionary.Values);
                Assert.False(dictionary.ContainsKey(3));
            }

            [Fact]
            public void Indexer()
            {
                IDictionary<int, string> dictionary = new OrderedDictionary<int, string>();
                dictionary.Add(2, "b");
                dictionary.Add(1, "a");
                Assert.Equal("b", dictionary[2]);
                Assert.Equal("a", dictionary[1]);
                Assert.Throws<KeyNotFoundException>(() => dictionary[3]);
            }

            [Fact]
            public void ContainsKey()
            {
                IDictionary<int, string> dictionary = new OrderedDictionary<int, string>();
                dictionary.Add(2, "b");
                dictionary.Add(1, "a");
                Assert.True(dictionary.ContainsKey(2));
                Assert.True(dictionary.ContainsKey(1));
                Assert.False(dictionary.ContainsKey(3));
            }

            public sealed class Keys : ICollectionTTests<int>
            {
                protected override bool IsReadOnly => true;

                protected override ICollection<int> CreateCollection(int entries)
                {
                    return ((IDictionary<int, string>)CreateDict(entries)).Keys;
                }

                protected override List<int> CreateItems(int entries)
                {
                    return CreateKeys(entries);
                }
            }

            public sealed class Values : ICollectionTTests<string>
            {
                protected override bool IsReadOnly => true;

                protected override ICollection<string> CreateCollection(int entries)
                {
                    return ((IDictionary<int, string>)CreateDict(entries)).Values;
                }

                protected override List<string> CreateItems(int entries)
                {
                    return CreateValues(entries);
                }
            }
        }

        public sealed class AsIDictionary : IDictionaryTests<int, string>
        {
            protected override IDictionary CreateCollection(int entries)
            {
                return CreateDict(entries);
            }

            protected override List<KeyValuePair<int, string>> CreateItems(int entries)
            {
                return CreateKeyValues(entries);
            }

            [Fact]
            public void Properties()
            {
                var dictionary = CreateCollection(3);
                Assert.False(dictionary.IsFixedSize);
                Assert.False(dictionary.IsReadOnly);
            }

            [Fact]
            public void Add_Preconditions()
            {
                Assert.Throws<ArgumentNullException>(
                    () => ((IDictionary)new OrderedDictionary<string, int>()).Add(null, 1));
                Assert.Throws<ArgumentNullException>(
                    () => ((IDictionary)new OrderedDictionary<int, int>()).Add(null, 1));
                Assert.Throws<ArgumentNullException>(
                    () => ((IDictionary)new OrderedDictionary<int, int>()).Add(1, null));
                Assert.Throws<ArgumentException>(
                    () => ((IDictionary)new OrderedDictionary<int, string>()).Add(1, 1));
                Assert.Throws<ArgumentException>(
                    () => ((IDictionary)new OrderedDictionary<int, string>()).Add("1", "1"));
            }

            [Fact]
            public void GetItem_Preconditions()
            {
                IDictionary dictionary = new OrderedDictionary<int, int> { { 1, 2 } };
                Assert.Throws<ArgumentNullException>(() => dictionary[null]);
            }

            [Fact]
            public void SetItem_Preconditions()
            {
                IDictionary dictionary = new OrderedDictionary<int, int> { { 1, 2 } };
                Assert.Throws<ArgumentNullException>(() => dictionary[null] = 3);
                Assert.Throws<ArgumentException>(() => dictionary["a"] = 3);
                Assert.Throws<ArgumentException>(() => dictionary[1] = "a");
            }

            public sealed class Keys : ICollectionTests<int>
            {
                protected override ICollection CreateCollection(int entries)
                {
                    return ((IDictionary)CreateDict(entries)).Keys;
                }

                protected override List<int> CreateItems(int entries)
                {
                    return CreateKeys(entries);
                }
            }

            public sealed class Values : ICollectionTests<string>
            {
                protected override ICollection CreateCollection(int entries)
                {
                    return ((IDictionary)CreateDict(entries)).Values;
                }

                protected override List<string> CreateItems(int entries)
                {
                    return CreateValues(entries);
                }
            }
        }

        public sealed class AsICollectionT : ICollectionTTests<KeyValuePair<int, string>>
        {
            protected override bool IsReadOnly => false;

            protected override ICollection<KeyValuePair<int, string>> CreateCollection(int entries)
            {
                return CreateDict(entries);
            }

            protected override List<KeyValuePair<int, string>> CreateItems(int entries)
            {
                return CreateKeyValues(entries);
            }

            [Fact]
            public void Remove_MismatchedValue()
            {
                var collection = CreateCollection(1);
                var first = collection.First();

                collection.Remove(new KeyValuePair<int, string>(first.Key, first.Value + "x"));

                Assert.Equal(new[] { first }, collection);
            }
        }

        public sealed class AsICollection : ICollectionTests<KeyValuePair<int, string>>
        {
            protected override ICollection CreateCollection(int entries)
            {
                return CreateDict(entries);
            }

            protected override List<KeyValuePair<int, string>> CreateItems(int entries)
            {
                return CreateKeyValues(entries);
            }
        }

        //public sealed class IDictionaryKV
        //{
        //    public sealed class KeysTest : ICollection_Tests<int>
        //    {
        //        protected override ICollection<int> CreateCollection(int entries)
        //        {
        //            return ((IDictionary<int, string>)CreateDict(entries)).Keys;
        //        }

        //        protected override List<int> CreateItems(int entries)
        //        {
        //            return CreateKeys(entries);
        //        }
        //    }
        //}

        private static OrderedDictionary<int, string> CreateDict(int entries)
        {
            var dictionary = new OrderedDictionary<int, string>();
            for (int i = 0; i < entries; ++i)
                dictionary.Add(-i, ((char)('A' + i)).ToString());
            return dictionary;
        }

        private static List<KeyValuePair<int, string>> CreateKeyValues(int entries)
        {
            var values = new List<KeyValuePair<int, string>>();
            for (int i = 0; i < entries; ++i)
                values.Add(new KeyValuePair<int, string>(-i, ((char)('A' + i)).ToString()));
            return values;
        }

        private static List<int> CreateKeys(int entries)
        {
            var values = new List<int>();
            for (int i = 0; i < entries; ++i)
                values.Add(-i);
            return values;
        }

        private static List<string> CreateValues(int entries)
        {
            var values = new List<string>();
            for (int i = 0; i < entries; ++i)
                values.Add(((char)('A' + i)).ToString());
            return values;
        }
    }
}
