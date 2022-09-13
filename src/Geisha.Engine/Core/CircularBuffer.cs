using System;
using System.Collections;
using System.Collections.Generic;

namespace Geisha.Engine.Core
{
    /// <summary>
    ///     Represents a strongly typed, fixed size, buffer of objects that can be added to the end of the buffer by replacing
    ///     objects at the beginning of the buffer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the circular buffer.</typeparam>
    /// <remarks>
    ///     The <see cref="CircularBuffer{T}" /> class implements contiguous, fixed size buffer hence it does only single
    ///     memory allocation of internal buffer itself at construction time. It does not move elements, rather it properly
    ///     indexes internal buffer hence it is value types friendly (no copying).
    /// </remarks>
    public sealed class CircularBuffer<T> : IReadOnlyList<T>
    {
        private readonly T[] _data;
        private int _head = -1;
        private int _version;

        /// <summary>
        ///     Creates new instance of <see cref="CircularBuffer{T}" /> with specified number of elements initialized with default
        ///     values.
        /// </summary>
        /// <param name="count">Number of elements for which the buffer allocates memory.</param>
        public CircularBuffer(int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), count, $"{nameof(count)} parameter must be positive.");
            Count = count;
            _data = new T[count];
        }

        /// <summary>
        ///     The number of elements contained in the <see cref="CircularBuffer{T}" />. It is equal to the fixed size of the
        ///     <see cref="CircularBuffer{T}" />.
        /// </summary>
        public int Count { get; }

        /// <summary>
        ///     Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException($"Index must be in range [0,{Count - 1}].");
                return GetItem(index);
            }
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the <see cref="CircularBuffer{T}" />.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the <see cref="CircularBuffer{T}" />.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            var enumeratorVersion = _version;
            for (var i = 0; i < Count; i++)
            {
                if (enumeratorVersion != _version) throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                yield return GetItem(i);
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Adds an object to the end of the <see cref="CircularBuffer{T}" /> at the same time replacing the object at the
        ///     beginning.
        /// </summary>
        /// <param name="item">
        ///     The object to be added to the end of the <see cref="CircularBuffer{T}" />. The value can be null for
        ///     reference types.
        /// </param>
        public void Add(T item)
        {
            _version++;
            _head = (_head + 1) % Count;
            _data[_head] = item;
        }

        private T GetItem(int index)
        {
            return _data[((_head - (Count - 1 - index)) % Count + Count) % Count];
        }
    }
}