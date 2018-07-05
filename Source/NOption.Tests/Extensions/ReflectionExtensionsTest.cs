namespace NOption.Tests.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using NOption.Extensions;
    using Xunit;

    public class ReflectionExtensionsTest
    {
        [Theory]
        [InlineData(typeof(ICollection), null)]
        [InlineData(typeof(IList), typeof(object))]
        [InlineData(typeof(IList<int>), typeof(int))]
        [InlineData(typeof(IList<string>), typeof(string))]
        [InlineData(typeof(string[]), typeof(string))]
        [InlineData(typeof(List<string>), typeof(string))]
        [InlineData(typeof(Collection<string>), typeof(string))]
        [InlineData(typeof(IStringCollection), typeof(string))]
        [InlineData(typeof(StringCollection), typeof(string))]
        [InlineData(typeof(CustomCollection), typeof(object))]
        [InlineData(typeof(IMultiCollection), null)]
        public void GetListElementType(Type listType, Type expectedElementType)
        {
            Assert.Equal(expectedElementType, listType.GetListElementType());
        }

        private interface IStringCollection : IList<string>
        {
        }

        private interface CustomCollection : IList
        {
        }

        private interface IMultiCollection : IList<string>, IList<int>
        {
        }

        private class StringCollection : IList<string>
        {
            private List<string> items;

            public IEnumerator<string> GetEnumerator()
            {
                return items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)items).GetEnumerator();
            }

            public void Add(string item)
            {
                items.Add(item);
            }

            public void Clear()
            {
                items.Clear();
            }

            public bool Contains(string item)
            {
                return items.Contains(item);
            }

            public void CopyTo(string[] array, int arrayIndex)
            {
                items.CopyTo(array, arrayIndex);
            }

            public bool Remove(string item)
            {
                return items.Remove(item);
            }

            public int Count => items.Count;

            public bool IsReadOnly => ((ICollection<string>)items).IsReadOnly;

            public int IndexOf(string item)
            {
                return items.IndexOf(item);
            }

            public void Insert(int index, string item)
            {
                items.Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                items.RemoveAt(index);
            }

            public string this[int index]
            {
                get => items[index];
                set => items[index] = value;
            }
        }
    }
}
