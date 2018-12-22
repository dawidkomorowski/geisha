using System;
using System.Collections;
using System.Collections.Generic;

namespace Geisha.Common
{
    public sealed class CircularBuffer<T> : IEnumerable<T>
    {
        private readonly T[] _data;
        private int _head = -1;

        public CircularBuffer(int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), size, $"{nameof(size)} parameter must be positive.");
            Size = size;
            _data = new T[size];
        }

        public int Size { get; }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Size) throw new IndexOutOfRangeException($"Index must be in range [0,{Size - 1}].");
                return _data[((_head - (Size - 1 - index)) % Size + Size) % Size];
            }
        }

        public void Add(T item)
        {
            _head = (_head + 1) % Size;
            _data[_head] = item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}