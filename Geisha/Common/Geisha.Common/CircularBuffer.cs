using System;
using System.Collections;
using System.Collections.Generic;

namespace Geisha.Common
{
    public sealed class CircularBuffer<T> : IEnumerable<T>
    {
        private readonly T[] _data;
        private int _head = -1;
        private int _version;

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
                return GetItem(index);
            }
        }

        public void Add(T item)
        {
            _version++;
            _head = (_head + 1) % Size;
            _data[_head] = item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var enumeratorVersion = _version;
            for (var i = 0; i < Size; i++)
            {
                if (enumeratorVersion != _version) throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                yield return GetItem(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private T GetItem(int index)
        {
            return _data[((_head - (Size - 1 - index)) % Size + Size) % Size];
        }
    }
}