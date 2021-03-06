namespace NOption.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading;

    /// <summary>
    ///   Represents a generic indexed collection of key/value pairs.
    /// </summary>
    /// <typeparam name="TKey">
    ///   The type of the keys in the dictionary.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///   The type of the values in the dictionary.
    /// </typeparam>
    [ComVisible(false)]
    [DebuggerTypeProxy(typeof(OrderedDictionary<,>.DebuggerProxy))]
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    internal sealed class OrderedDictionary<TKey, TValue>
        : IOrderedDictionary<TKey, TValue>
        , IDictionary
    {
        private readonly Dictionary<TKey, TValue> objectsTable;
        private readonly List<KeyValuePair<TKey, TValue>> objectsArray;
        private readonly IEqualityComparer<TKey> comparer;
        private object syncRoot;

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref="OrderedDictionary{TKey,TValue}"/> class that is empty,
        ///   has the default initial capacity, and uses the default equality
        ///   comparer for the key type.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Every key in a <see cref="OrderedDictionary{TKey,TValue}"/> must
        ///     be unique according to the default equality comparer.
        ///   </para>
        ///   <para>
        ///     <see cref="OrderedDictionary{TKey,TValue}"/> requires an equality
        ///     implementation to determine whether keys are equal. This constructor
        ///     uses the default generic equality comparer,
        ///     <see cref="EqualityComparer{T}.Default"/>. If type
        ///     <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/>
        ///     generic interface, the default equality comparer uses that
        ///     implementation. Alternatively, you can specify an implementation
        ///     of the <see cref="IEqualityComparer{T}"/> generic interface by
        ///     using a constructor that accepts a comparer parameter.
        ///   </para>
        /// </remarks>
        public OrderedDictionary()
            : this(0, null)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref="OrderedDictionary{TKey,TValue}"/> class that is empty,
        ///   has the specified initial capacity, and uses the default equality
        ///   comparer for the key type.
        /// </summary>
        /// <param name="capacity">
        ///   The initial number of elements that the
        ///   <see cref="OrderedDictionary{TKey,TValue}"/> can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="capacity"/> is less than 0.
        /// </exception>
        /// <remarks>
        ///   <para>
        ///     Every key in a <see cref="OrderedDictionary{TKey,TValue}"/> must
        ///     be unique according to the default equality comparer.
        ///   </para>
        ///   <para>
        ///     The capacity of a <see cref="OrderedDictionary{TKey,TValue}"/>
        ///     is the number of elements that can be added to the
        ///     <see cref="OrderedDictionary{TKey,TValue}"/> before resizing is
        ///     necessary. As elements are added to a
        ///     <see cref="OrderedDictionary{TKey,TValue}"/>, the capacity is
        ///     automatically increased as required by reallocating the internal
        ///     array.
        ///   </para>
        ///   <para>
        ///     If the size of the collection can be estimated, specifying the
        ///     initial capacity eliminates the need to perform a number of resizing
        ///     operations while adding elements to the
        ///     <see cref="OrderedDictionary{TKey,TValue}"/>.
        ///   </para>
        ///   <para>
        ///     <see cref="OrderedDictionary{TKey,TValue}"/> requires an equality
        ///     implementation to determine whether keys are equal. This constructor
        ///     uses the default generic equality comparer,
        ///     <see cref="EqualityComparer{T}.Default"/>. If type
        ///     <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/>
        ///     generic interface, the default equality comparer uses that
        ///     implementation. Alternatively, you can specify an implementation
        ///     of the <see cref="IEqualityComparer{T}"/> generic interface by
        ///     using a constructor that accepts a comparer parameter.
        ///   </para>
        /// </remarks>
        public OrderedDictionary(int capacity)
            : this(capacity, null)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref="OrderedDictionary{TKey,TValue}"/> class that is empty,
        ///   has the default initial capacity, and uses the specified
        ///   <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{T}"/> implementation to use when
        ///   comparing keys, or <see langword="null"/> to use the default
        ///   <see cref="EqualityComparer{T}"/> for the type of the key.
        /// </param>
        /// <remarks>
        ///   <para>
        ///     Every key in a <see cref="OrderedDictionary{TKey,TValue}"/> must
        ///     be unique according to the default equality comparer.
        ///   </para>
        ///   <para>
        ///     The capacity of a <see cref="OrderedDictionary{TKey,TValue}"/>
        ///     is the number of elements that can be added to the
        ///     <see cref="OrderedDictionary{TKey,TValue}"/> before resizing is
        ///     necessary. As elements are added to a
        ///     <see cref="OrderedDictionary{TKey,TValue}"/>, the capacity is
        ///     automatically increased as required by reallocating the internal
        ///     array.
        ///   </para>
        ///   <para>
        ///     <see cref="OrderedDictionary{TKey,TValue}"/> requires an equality
        ///     implementation to determine whether keys are equal. If
        ///     <paramref name="comparer"/> is <see langword="null"/>, this constructor
        ///     uses the default generic equality comparer,
        ///     <see cref="EqualityComparer{T}.Default"/>. If type
        ///     <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/>
        ///     generic interface, the default equality comparer uses that
        ///     implementation.
        ///   </para>
        /// </remarks>
        public OrderedDictionary(IEqualityComparer<TKey> comparer)
            : this(0, comparer)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref="OrderedDictionary{TKey,TValue}"/> class that is empty,
        ///   has the specified initial capacity, and uses the specified
        ///   <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="capacity">
        ///   The initial number of elements that the
        ///   <see cref="OrderedDictionary{TKey,TValue}"/> can contain.
        /// </param>
        /// <param name="comparer">
        ///   The <see cref="IEqualityComparer{T}"/> implementation to use when
        ///   comparing keys, or <see langword="null"/> to use the default
        ///   <see cref="EqualityComparer{T}"/> for the type of the key.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="capacity"/> is less than 0.
        /// </exception>
        /// <remarks>
        ///   <para>
        ///     Every key in a <see cref="OrderedDictionary{TKey,TValue}"/> must
        ///     be unique according to the default equality comparer.
        ///   </para>
        ///   <para>
        ///     The capacity of a <see cref="OrderedDictionary{TKey,TValue}"/>
        ///     is the number of elements that can be added to the
        ///     <see cref="OrderedDictionary{TKey,TValue}"/> before resizing is
        ///     necessary. As elements are added to a
        ///     <see cref="OrderedDictionary{TKey,TValue}"/>, the capacity is
        ///     automatically increased as required by reallocating the internal
        ///     array.
        ///   </para>
        ///   <para>
        ///     If the size of the collection can be estimated, specifying the
        ///     initial capacity eliminates the need to perform a number of resizing
        ///     operations while adding elements to the
        ///     <see cref="OrderedDictionary{TKey,TValue}"/>.
        ///   </para>
        ///   <para>
        ///     <see cref="OrderedDictionary{TKey,TValue}"/> requires an equality
        ///     implementation to determine whether keys are equal. If
        ///     <paramref name="comparer"/> is <see langword="null"/>, this constructor
        ///     uses the default generic equality comparer,
        ///     <see cref="EqualityComparer{T}.Default"/>. If type
        ///     <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/>
        ///     generic interface, the default equality comparer uses that
        ///     implementation.
        ///   </para>
        /// </remarks>
        public OrderedDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Contract violated: capacity >= 0");
            this.comparer = comparer;
            objectsTable = new Dictionary<TKey, TValue>(capacity, comparer);
            objectsArray = new List<KeyValuePair<TKey, TValue>>(capacity);
        }

        /// <summary>
        ///   Gets an <see cref="IList{T}"/> containing the keys of the
        ///   <see cref="IOrderedDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <returns>
        ///   An <see cref="IList{T}"/> containing the keys of the object that
        ///   implements <see cref="IOrderedDictionary{TKey,TValue}"/>.
        /// </returns>
        public IReadOnlyList<TKey> Keys => new KeyCollection(this);

        /// <summary>
        ///   Gets an <see cref="IList{T}"/> containing the values in the
        ///   <see cref="IOrderedDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <returns>
        ///   An <see cref="IList{T}"/> containing the values in the object that
        ///   implements <see cref="IOrderedDictionary{TKey,TValue}"/>.
        /// </returns>
        public IReadOnlyList<TValue> Values => new ValueCollection(this);

        /// <summary>
        ///   Gets the element at the specified index.
        /// </summary>
        /// <returns>
        ///   The element with the specified key.
        /// </returns>
        /// <param name="index">
        ///   The index of the element to get or set.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="index"/> is not in [0, <see cref="Count"/>).
        /// </exception>
        public TValue GetAt(int index)
        {
            if (!(index >= 0 && index < Count))
                throw new ArgumentOutOfRangeException(nameof(index));

            return objectsArray[index].Value;
        }

        /// <summary>
        ///   Sets the element at the specified index.
        /// </summary>
        /// <returns>
        ///   The element with the specified key.
        /// </returns>
        /// <param name="index">
        ///   The index of the element to get or set.
        /// </param>
        /// <param name="value">
        ///   The new value.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="index"/> is not in [0, <see cref="Count"/>).
        /// </exception>
        public void SetAt(int index, TValue value)
        {
            if (!(index >= 0 && index < Count))
                throw new ArgumentOutOfRangeException(nameof(index));

            KeyValuePair<TKey, TValue> entry = objectsArray[index];
            TKey key = entry.Key;
            objectsArray[index] = new KeyValuePair<TKey, TValue>(key, value);
            objectsTable[key] = value;
        }

        /// <summary>
        ///   Adds an element with the provided key and value to the
        ///   <see cref="OrderedDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="index">
        ///   The zero-based index at which the element should be inserted.
        /// </param>
        /// <param name="key">
        ///   The object to use as the key of the element to add.
        /// </param>
        /// <param name="value">
        ///   The object to use as the value of the element to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   An element with the same key already exists in the
        ///   <see cref="OrderedDictionary{TKey,TValue}"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///   The <see cref="OrderedDictionary{TKey,TValue}"/> is read-only.
        /// </exception>
        public void Insert(int index, TKey key, TValue value)
        {
            if (!(index >= 0 && index <= Count))
                throw new ArgumentOutOfRangeException(nameof(index));

            objectsTable.Add(key, value);
            objectsArray.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        ///   Determines whether the <see cref="OrderedDictionary{TKey,TValue}"/>
        ///   contains a specific value.
        /// </summary>
        /// <param name="value">
        ///   The value to locate in the <see cref="OrderedDictionary{TKey,TValue}"/>.
        ///   The value can be null for reference types.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the <see cref="OrderedDictionary{TKey,TValue}"/>
        ///   contains an element with the specified value; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        /// <remarks>
        ///   <para>
        ///     This method determines equality using the default equality comparer
        ///     <see cref="EqualityComparer{T}.Default"/> for
        ///     <typeparamref name="TValue"/>, the type of values in the dictionary.
        ///   </para>
        ///   <para>
        ///     This method performs a linear search; therefore, the average
        ///     execution time is proportional to <see cref="Count"/>. That is,
        ///     this method is an <c>O(n)</c> operation, where <c>n</c> is
        ///     <see cref="Count"/>.
        ///   </para>
        /// </remarks>
        public bool ContainsValue(TValue value)
        {
            return objectsTable.ContainsValue(value);
        }

        #region Implementation of IDictionary<TKey, TValue>

        /// <summary>
        ///   Gets the number of elements contained in the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <returns>
        ///   The number of elements contained in the <see cref="ICollection{T}"/>.
        /// </returns>
        public int Count => objectsArray.Count;

        /// <summary>
        ///   Gets an <see cref="ICollection{T}"/> containing the keys of the
        ///   <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <returns>
        ///   An <see cref="ICollection{T}"/> containing the keys of the object
        ///   that implements <see cref="IDictionary{TKey,TValue}"/>.
        /// </returns>
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => new KeyCollection(this);

        /// <summary>
        ///   Gets an <see cref="ICollection{T}"/> containing the values in the
        ///   <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <returns>
        ///   An <see cref="ICollection{T}"/> containing the values in the object
        ///   that implements <see cref="IDictionary{TKey,TValue}"/>.
        /// </returns>
        ICollection<TValue> IDictionary<TKey, TValue>.Values => new ValueCollection(this);

        /// <summary>
        ///   Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        ///   The element with the specified key.
        /// </returns>
        /// <param name="key">
        ///   The key of the element to get or set.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        ///   The property is retrieved and <paramref name="key"/> is not found.
        /// </exception>
        public TValue this[TKey key]
        {
            get => objectsTable[key];
            set
            {
                if (objectsTable.ContainsKey(key)) {
                    int index = IndexOfKey(key);
                    objectsTable[key] = value;
                    objectsArray[index] = new KeyValuePair<TKey, TValue>(objectsArray[index].Key, value);
                } else {
                    Add(key, value);
                }
            }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///   A <see cref="IEnumerator{T}"/> that can be used to iterate through
        ///   the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return objectsArray.GetEnumerator();
        }

        /// <summary>
        ///   Determines whether the <see cref="IDictionary{TKey,TValue}"/> contains
        ///   an element with the specified key.
        /// </summary>
        /// <returns>
        ///   true if the <see cref="IDictionary{TKey,TValue}"/> contains an
        ///   element with the key; otherwise, false.
        /// </returns>
        /// <param name="key">
        ///   The key to locate in the <see cref="IDictionary{TKey,TValue}"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        public bool ContainsKey(TKey key)
        {
            return objectsTable.ContainsKey(key);
        }

        /// <summary>
        ///   Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        ///   true if the object that implements <see cref="IDictionary{TKey,TValue}"/>
        ///   contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">
        ///   The key whose value to get.
        /// </param>
        /// <param name="value">
        ///   When this method returns, the value associated with the specified
        ///   key, if the key is found; otherwise, the default value for the
        ///   type of the <paramref name="value"/> parameter. This parameter is
        ///   passed uninitialized.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return objectsTable.TryGetValue(key, out value);
        }

        /// <summary>
        ///   Removes all items from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   The <see cref="ICollection{T}"/> is read-only.
        /// </exception>
        public void Clear()
        {
            objectsTable.Clear();
            objectsArray.Clear();
        }

        /// <summary>
        ///   Adds an element with the provided key and value to the
        ///   <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="key">
        ///   The object to use as the key of the element to add.
        /// </param>
        /// <param name="value">
        ///   The object to use as the value of the element to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   An element with the same key already exists in the
        ///   <see cref="IDictionary{TKey,TValue}"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///   The <see cref="IDictionary{TKey,TValue}"/> is read-only.
        /// </exception>
        public void Add(TKey key, TValue value)
        {
            objectsTable.Add(key, value);
            objectsArray.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        ///   Removes the element with the specified key from the
        ///   <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <returns>
        ///   true if the element is successfully removed; otherwise, false.
        ///   This method also returns false if <paramref name="key"/> was not
        ///   found in the original <see cref="IDictionary{TKey,TValue}"/>.
        /// </returns>
        /// <param name="key">
        ///   The key of the element to remove.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///   The <see cref="IDictionary{TKey,TValue}"/> is read-only.
        /// </exception>
        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            int index = IndexOfKey(key);
            if (index < 0)
                return false;

            objectsTable.Remove(key);
            objectsArray.RemoveAt(index);
            return true;
        }

        #endregion

        #region Implementation of IDictionary<TKey, TValue>

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        #endregion

        #region Implementation of IDictionary

        bool IDictionary.IsFixedSize => false;

        bool IDictionary.IsReadOnly => false;

        ICollection IDictionary.Keys => new KeyCollection(this);

        ICollection IDictionary.Values => new ValueCollection(this);

        object IDictionary.this[object key]
        {
            get => ((IDictionary)objectsTable)[key];
            set
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                ThrowIfIllegalNull<TValue>(value, "value");

                if (!(key is TKey k))
                    throw CreateWrongKeyTypeException(key, typeof(TKey));
                if (!(value is TValue v))
                    throw CreateWrongValueTypeException(value, typeof(TValue));

                this[k] = v;
            }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(GetEnumerator());
        }

        bool IDictionary.Contains(object key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            return key is TKey k && ContainsKey(k);
        }

        void IDictionary.Add(object key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            ThrowIfIllegalNull<TValue>(value, "value");

            if (!(key is TKey k))
                throw CreateWrongKeyTypeException(key, typeof(TKey));
            if (value != null && !(value is TValue))
                throw CreateWrongValueTypeException(value, typeof(TValue));

            Add(k, (TValue)value);
        }

        void IDictionary.Remove(object key)
        {
            if (key is TKey k)
                Remove(k);
        }

        #endregion

        #region Implementation of ICollection

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot
        {
            get
            {
                if (syncRoot == null)
                    Interlocked.CompareExchange<object>(ref syncRoot, new object(), null);
                return syncRoot;
            }
        }

        /// <summary>
        ///   Copies the elements of the <see cref="ICollection"/> to an
        ///   <see cref="Array"/>, starting at a particular <see cref="Array"/>
        ///   index.
        /// </summary>
        /// <param name="array">
        ///   The one-dimensional <see cref="Array"/> that is the destination of
        ///   the elements copied from <see cref="ICollection"/>. The <see cref="Array"/>
        ///   must have zero-based indexing.
        /// </param>
        /// <param name="index">
        ///   The zero-based index in <paramref name="array"/> at which copying
        ///   begins.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="array"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="index"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="array"/> is multidimensional. -or- The number of
        ///   elements in the source <see cref="ICollection"/> is greater than
        ///   the available space from <paramref name="index"/> to the end of
        ///   the destination <paramref name="array"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The type of the source <see cref="ICollection"/> cannot be cast
        ///   automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)objectsArray).CopyTo(array, index);
        }

        #endregion

        #region Implementation of ICollection<KeyValuePair<TKey, TValue>>

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)objectsTable).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            objectsArray.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            int index = IndexOfKey(item.Key);
            if (index < 0 || index >= Count)
                return false;

            if (!EqualityComparer<TValue>.Default.Equals(objectsArray[index].Value, item.Value))
                return false;

            objectsTable.Remove(objectsArray[index].Key);
            objectsArray.RemoveAt(index);
            return true;
        }

        #endregion

        #region Implementation of IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private static void ThrowIfIllegalNull<T>(object value, string argumentName)
        {
            if (value == null && default(T) != null)
                throw new ArgumentNullException(argumentName);
        }

        private int IndexOfKey(TKey key)
        {
            for (int i = 0; i < objectsArray.Count; ++i) {
                var entry = objectsArray[i];
                TKey entryKey = entry.Key;
                if (comparer != null) {
                    if (comparer.Equals(entryKey, key))
                        return i;
                } else if (entryKey.Equals(key)) {
                    return i;
                }
            }

            return -1;
        }

        private static Exception CreateWrongKeyTypeException(object key, Type targetType)
        {
            string message = string.Format(
                CultureInfo.CurrentCulture, Strings.Arg_WrongType, key, targetType);
            return new ArgumentException(message, nameof(key));
        }

        private static Exception CreateWrongValueTypeException(object value, Type targetType)
        {
            string message = string.Format(
                CultureInfo.CurrentCulture, Strings.Arg_WrongType, value, targetType);
            return new ArgumentException(message, nameof(value));
        }

        private sealed class Enumerator : IDictionaryEnumerator
        {
            private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;

            public Enumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
            {
                this.enumerator = enumerator;
            }

            public object Current => Entry;

            public object Key => enumerator.Current.Key;

            public object Value => enumerator.Current.Value;

#pragma warning disable DE0006 // API is deprecated
            public DictionaryEntry Entry
            {
                get
                {
                    var current = enumerator.Current;
                    return new DictionaryEntry(current.Key, current.Value);
                }
            }
#pragma warning restore DE0006 // API is deprecated

            public bool MoveNext()
            {
                return enumerator.MoveNext();
            }

            public void Reset()
            {
                enumerator.Reset();
            }
        }

        [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
        [DebuggerTypeProxy(typeof(CollectionDebuggerProxy<>))]
        private sealed class KeyCollection : IReadOnlyList<TKey>, ICollection<TKey>, ICollection
        {
            private readonly OrderedDictionary<TKey, TValue> dictionary;

            public KeyCollection(OrderedDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }

            public int Count => dictionary.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => ((ICollection)dictionary).SyncRoot;

            public IEnumerator<TKey> GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            public bool Contains(TKey item)
            {
                return dictionary.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (array.Length - index < dictionary.objectsArray.Count)
                    throw new ArgumentException(nameof(array), "Not enough space in target array");
                foreach (var entry in dictionary.objectsArray)
                    array[index++] = entry.Key;
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException();
            }

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException();
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (array.Rank != 1)
                    throw new ArgumentException(nameof(array), Strings.Arg_RankMultiDimNotSupported);
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (array.Length - index < dictionary.objectsArray.Count)
                    throw new ArgumentException(nameof(array), "Not enough space in target array");

                switch (array) {
                    case TKey[] keys:
                        foreach (var entry in dictionary.objectsArray)
                            keys[index++] = entry.Key;
                        break;

                    case object[] objs:
                        try {
                            foreach (var entry in dictionary.objectsArray)
                                objs[index++] = entry.Key;
                        } catch (ArrayTypeMismatchException) {
                            throw new ArgumentException(Strings.Arg_InvalidArrayType);
                        }
                        break;

                    default:
                        throw new ArgumentException(Strings.Arg_InvalidArrayType);
                }
            }

            public TKey this[int index] => dictionary.objectsArray[index].Key;

            [StructLayout(LayoutKind.Sequential)]
            private struct Enumerator : IEnumerator<TKey>
            {
                private readonly IEnumerator<KeyValuePair<TKey, TValue>> arrayEnumerator;

                internal Enumerator(OrderedDictionary<TKey, TValue> dictionary)
                {
                    arrayEnumerator = dictionary.objectsArray.GetEnumerator();
                }

                public TKey Current => arrayEnumerator.Current.Key;

                object IEnumerator.Current => Current;

                public void Dispose()
                {
                    arrayEnumerator.Dispose();
                }

                public bool MoveNext()
                {
                    return arrayEnumerator.MoveNext();
                }

                void IEnumerator.Reset()
                {
                    arrayEnumerator.Reset();
                }
            }
        }

        [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
        [DebuggerTypeProxy(typeof(CollectionDebuggerProxy<>))]
        private sealed class ValueCollection : IReadOnlyList<TValue>, ICollection<TValue>, ICollection
        {
            private readonly OrderedDictionary<TKey, TValue> dictionary;

            public ValueCollection(OrderedDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }

            public int Count => dictionary.Count;

            bool ICollection<TValue>.IsReadOnly => true;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => ((ICollection)dictionary).SyncRoot;

            public IEnumerator<TValue> GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            public bool Contains(TValue item)
            {
                return dictionary.ContainsValue(item);
            }

            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (array.Length - index < dictionary.objectsArray.Count)
                    throw new ArgumentException(nameof(array), "Not enough space in target array");
                foreach (var entry in dictionary.objectsArray)
                    array[index++] = entry.Value;
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (array.Rank != 1)
                    throw new ArgumentException(nameof(array), Strings.Arg_RankMultiDimNotSupported);
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (array.Length - index < dictionary.objectsArray.Count)
                    throw new ArgumentException(nameof(array), "Not enough space in target array");

                switch (array) {
                    case TValue[] values:
                        foreach (var entry in dictionary.objectsArray)
                            values[index++] = entry.Value;
                        break;

                    case object[] objs:
                        try {
                            foreach (var entry in dictionary.objectsArray)
                                objs[index++] = entry.Value;
                        } catch (ArrayTypeMismatchException) {
                            throw new ArgumentException(Strings.Arg_InvalidArrayType);
                        }
                        break;

                    default:
                        throw new ArgumentException(Strings.Arg_InvalidArrayType);
                }
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException();
            }

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException();
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public TValue this[int index] => dictionary.objectsArray[index].Value;

            [StructLayout(LayoutKind.Sequential)]
            private struct Enumerator : IEnumerator<TValue>
            {
                private readonly IEnumerator<KeyValuePair<TKey, TValue>> arrayEnumerator;

                internal Enumerator(OrderedDictionary<TKey, TValue> dictionary)
                {
                    arrayEnumerator = dictionary.objectsArray.GetEnumerator();
                }

                public TValue Current => arrayEnumerator.Current.Value;

                object IEnumerator.Current => Current;

                public void Dispose()
                {
                    arrayEnumerator.Dispose();
                }

                public bool MoveNext()
                {
                    return arrayEnumerator.MoveNext();
                }

                void IEnumerator.Reset()
                {
                    arrayEnumerator.Reset();
                }
            }
        }

        private sealed class CollectionDebuggerProxy<T>
        {
            private readonly ICollection<T> collection;

            public CollectionDebuggerProxy(ICollection<T> collection)
            {
                this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public T[] Items
            {
                get
                {
                    var items = new T[collection.Count];
                    collection.CopyTo(items, 0);
                    return items;
                }
            }
        }

        private sealed class DebuggerProxy
        {
            private readonly IDictionary<TKey, TValue> dictionary;

            public DebuggerProxy(IDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public KeyValuePair<TKey, TValue>[] Items
            {
                get
                {
                    var items = new KeyValuePair<TKey, TValue>[dictionary.Count];
                    dictionary.CopyTo(items, 0);
                    return items;
                }
            }
        }
    }
}
