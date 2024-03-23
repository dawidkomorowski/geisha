using System;

namespace Geisha.Engine.Core.Collections;

// TODO How useful would it be? It should work fine with variables and fields but poorly with properties.
// TODO Add documentation.
// TODO Add tests.
public struct FixedList2<T>
{
    private T? _item0;
    private T? _item1;

    public FixedList2()
    {
        _item0 = default;
        _item1 = default;
        Count = 0;
    }

    public static int Capacity => 2;
    public int Count { get; private set; }

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
        set
        {
            if (index >= Count)
            {
                throw new IndexOutOfRangeException();
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
                    throw new IndexOutOfRangeException();
            }
        }
    }

    public void Add(T? item)
    {
        if (Count == Capacity)
        {
            throw new InvalidOperationException("TODO");
        }

        this[Count++] = item!;
    }

    public ReadOnlyFixedList2<T> ToReadOnly() => Count switch
    {
        0 => new ReadOnlyFixedList2<T>(),
        1 => new ReadOnlyFixedList2<T>(_item0),
        2 => new ReadOnlyFixedList2<T>(_item0, _item1),
        _ => throw new IndexOutOfRangeException()
    };
}