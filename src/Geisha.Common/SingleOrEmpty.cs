using System.Collections;
using System.Collections.Generic;

namespace Geisha.Common
{
    /// <summary>
    ///     Represents <see cref="IEnumerable{T}" /> that either has exactly one element or is empty.
    /// </summary>
    /// <typeparam name="T">The type of element kept.</typeparam>
    public interface ISingleOrEmpty<out T> : IEnumerable<T>
    {
    }

    /// <summary>
    ///     Provides static factory methods for creating <see cref="ISingleOrEmpty{T}" /> instances.
    /// </summary>
    public static class SingleOrEmpty
    {
        /// <summary>
        ///     Creates <see cref="ISingleOrEmpty{T}" /> with single specified element.
        /// </summary>
        /// <typeparam name="T">The type of element kept by created <see cref="ISingleOrEmpty{T}" /> instance.</typeparam>
        /// <param name="item">Element kept by created <see cref="ISingleOrEmpty{T}" /> instance.</param>
        /// <returns><see cref="ISingleOrEmpty{T}" /> with single element.</returns>
        public static ISingleOrEmpty<T> Single<T>(T item)
        {
            return new SingleOrEmptyImpl<T>(item);
        }

        /// <summary>
        ///     Creates <see cref="ISingleOrEmpty{T}" /> with no elements.
        /// </summary>
        /// <typeparam name="T">The type of element kept by created <see cref="ISingleOrEmpty{T}" /> instance.</typeparam>
        /// <returns><see cref="ISingleOrEmpty{T}" /> with no elements.</returns>
        public static ISingleOrEmpty<T> Empty<T>()
        {
            return new SingleOrEmptyImpl<T>();
        }

        private class SingleOrEmptyImpl<T> : ISingleOrEmpty<T>
        {
            private readonly bool _hasValue;
            private readonly T _value;

            public SingleOrEmptyImpl()
            {
                _hasValue = false;
            }

            public SingleOrEmptyImpl(T value)
            {
                _value = value;
                _hasValue = true;
            }

            public IEnumerator<T> GetEnumerator()
            {
                if (_hasValue)
                    yield return _value;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}