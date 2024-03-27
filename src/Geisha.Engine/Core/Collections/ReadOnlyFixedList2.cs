using System;

namespace Geisha.Engine.Core.Collections;

// TODO Add documentation.
// TODO Add tests.
public readonly struct ReadOnlyFixedList2<T>
{
    private readonly T? _item0;
    private readonly T? _item1;

    public ReadOnlyFixedList2()
    {
        _item0 = default;
        _item1 = default;
        Count = 0;
    }

    public ReadOnlyFixedList2(T? item0)
    {
        _item0 = item0;
        _item1 = default;
        Count = 1;
    }

    public ReadOnlyFixedList2(T? item0, T? item1)
    {
        _item0 = item0;
        _item1 = item1;
        Count = 2;
    }

    public int Count { get; }

    public T this[int index]
    {
        get
        {
            if (index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            return index switch
            {
                0 => _item0!,
                1 => _item1!,
                _ => throw new IndexOutOfRangeException()
            };
        }
    }
}