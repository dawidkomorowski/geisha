using System;
using System.Text;

namespace Geisha.Engine.Physics;

public readonly record struct CollisionBitmask
{
    public CollisionBitmask(uint value)
    {
        Value = value;
    }

    public uint Value { get; init; }

    public bool HasBit(int bit) => (Value & 1u << bit) != 0;
    public CollisionBitmask WithBit(int bit) => default;
    public CollisionBitmask WithoutBit(int bit) => default;

    public static CollisionBitmask None { get; } = new(0);
    public static CollisionBitmask All { get; } = new(uint.MaxValue);

    public static CollisionBitmask FromUInt(uint value) => new(value);

    // TODO: This implementation may force the caller to allocate.
    //       When migrated to .NET 9 (C# 13) params collection feature could be used to accept ReadOnlySpan.
    public static CollisionBitmask FromBits(params int[] bits) => FromBits(bits.AsSpan());

    public static CollisionBitmask FromBits(ReadOnlySpan<int> bits)
    {
        uint value = 0;

        foreach (var bit in bits)
        {
            value |= 1u << bit;
        }

        return new CollisionBitmask(value);
    }

    public static CollisionBitmask operator &(CollisionBitmask left, CollisionBitmask right) => default;
    public static CollisionBitmask operator |(CollisionBitmask left, CollisionBitmask right) => default;

    private bool PrintMembers(StringBuilder builder)
    {
        // TODO: Once migrated to .NET 8 use native "b8" format.
        builder.Append($"{nameof(Value)} = ");

        for (var i = 31; i >= 0; i--)
        {
            builder.Append(HasBit(i) ? "1" : "0");
        }

        return true;
    }
}