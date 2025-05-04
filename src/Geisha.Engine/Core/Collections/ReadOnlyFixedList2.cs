using System;

namespace Geisha.Engine.Core.Collections;

/// <summary>
///     Represents readonly list of fixed capacity of 2 items. The list can live on stack or be embedded into other types.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
/// <remarks>
///     <see cref="ReadOnlyFixedList2{T}" /> may not be suitable to hold mutable structs in certain scenarios as it does
///     not provide ref indexer.
/// </remarks>
public readonly struct ReadOnlyFixedList2<T>
{
    private readonly T? _item0;
    private readonly T? _item1;

    /// <summary>
    ///     Creates new instance of <see cref="ReadOnlyFixedList2{T}" /> that has zero items.
    /// </summary>
    public ReadOnlyFixedList2()
    {
        _item0 = default;
        _item1 = default;
        Count = 0;
    }

    /// <summary>
    ///     Creates new instance of <see cref="ReadOnlyFixedList2{T}" /> that has one item.
    /// </summary>
    /// <param name="item0">Item to be added to the list.</param>
    public ReadOnlyFixedList2(T? item0)
    {
        _item0 = item0;
        _item1 = default;
        Count = 1;
    }

    /// <summary>
    ///     Creates new instance of <see cref="ReadOnlyFixedList2{T}" /> that has two items.
    /// </summary>
    /// <param name="item0">First item to be added to the list.</param>
    /// <param name="item1">Second item to be added in the list.</param>
    public ReadOnlyFixedList2(T? item0, T? item1)
    {
        _item0 = item0;
        _item1 = item1;
        Count = 2;
    }

    /// <summary>
    ///     Gets the number of elements contained in the <see cref="ReadOnlyFixedList2{T}" />.
    /// </summary>
    public int Count { get; }

    /// <summary>
    ///     Gets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <para><paramref name="index" /> is less than 0.</para>
    ///     <para>-or-</para>
    ///     <para><paramref name="index" /> is equal to or greater than Count.</para>
    /// </exception>
    public T this[int index]
    {
        get
        {
            if (index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return index switch
            {
                0 => _item0!,
                1 => _item1!,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
        }
    }

    /// <summary>
    ///     Creates an array with elements from the <see cref="ReadOnlyFixedList2{T}" />.
    /// </summary>
    /// <returns>
    ///     An array containing the elements of the <see cref="ReadOnlyFixedList2{T}" />. The length of the array is equal to
    ///     the <see cref="Count" /> of the list.
    /// </returns>
    public T[] ToArray()
    {
        var array = new T[Count];
        if (Count > 0)
        {
            array[0] = _item0!;
        }

        if (Count > 1)
        {
            array[1] = _item1!;
        }

        return array;
    }
}