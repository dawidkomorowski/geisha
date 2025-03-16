using System;

namespace Geisha.Engine.Core.Collections;

/// <summary>
///     Represents list of fixed capacity of 2 items. The list can live on stack or be embedded into other types.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
/// <remarks>
///     <para>
///         <see cref="FixedList2{T}" /> may not be suitable to hold mutable structs in certain scenarios as it does not
///         provide ref indexer.
///     </para>
///     <para>
///         <see cref="FixedList2{T}" /> may not be suitable to be exposed by property in certain scenarios as the copy
///         would be returned making mutations invalid.
///     </para>
/// </remarks>
public struct FixedList2<T>
{
    private T? _item0;
    private T? _item1;

    /// <summary>
    ///     Creates new instance of <see cref="FixedList2{T}" /> that is empty.
    /// </summary>
    public FixedList2()
    {
        _item0 = default;
        _item1 = default;
        Count = 0;
    }

    /// <summary>
    ///     Gets capacity of <see cref="FixedList2{T}" />. The capacity is constant and equal to <c>2</c>.
    /// </summary>
    public static int Capacity => 2;

    /// <summary>
    ///     Gets the number of elements contained in the <see cref="FixedList2{T}" />.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    ///     Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <para><paramref name="index" /> is less than 0.</para>
    ///     <para>-or-</para>
    ///     <para><paramref name="index" /> is equal to or greater than Count.</para>
    /// </exception>
    public T this[int index]
    {
        readonly get
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
        set
        {
            if (index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            switch (index)
            {
                case 0:
                    _item0 = value;
                    break;
                case 1:
                    _item1 = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }

    /// <summary>
    ///     Adds an item to the end of the <see cref="FixedList2{T}" />.
    /// </summary>
    /// <param name="item">The item to be added to the end of the <see cref="FixedList2{T}" />.</param>
    /// <exception cref="InvalidOperationException">Maximum capacity of the <see cref="FixedList2{T}" /> has been reached.</exception>
    public void Add(T? item)
    {
        if (Count == Capacity)
        {
            throw new InvalidOperationException("Maximum capacity of the list has been reached.");
        }

        this[Count++] = item!;
    }

    /// <summary>
    ///     Creates new instance of <see cref="ReadOnlyFixedList2{T}" /> that is initialized with the same items as contained
    ///     in this <see cref="FixedList2{T}" />.
    /// </summary>
    /// <returns><see cref="ReadOnlyFixedList2{T}" /> that contains the same items as this <see cref="FixedList2{T}" />.</returns>
    public readonly ReadOnlyFixedList2<T> ToReadOnly() => Count switch
    {
        0 => new ReadOnlyFixedList2<T>(),
        1 => new ReadOnlyFixedList2<T>(_item0),
        2 => new ReadOnlyFixedList2<T>(_item0, _item1),
        _ => throw new InvalidOperationException("Unreachable code. Use UnreachableException once migrated to .NET 7.")
    };
}