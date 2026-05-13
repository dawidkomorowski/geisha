namespace Geisha.Engine.Physics;

public readonly record struct CollisionBitmask
{
    public CollisionBitmask(uint value)
    {
        Value = value;
    }

    public uint Value { get; init; }

    public bool HasBit(int bit) => false;

    public static CollisionBitmask None { get; } = new(uint.MinValue);
    public static CollisionBitmask All { get; } = new(uint.MaxValue);

    public static CollisionBitmask FromUInt(uint value) => new(value);

    // TODO: This implementation may force the caller to allocate.
    //       When migrated to .NET 9 (C# 13) params collection feature could be used to accept ReadOnlySpan.
    public static CollisionBitmask FromBits(params int[] bits)
    {
        uint value = 0;

        foreach (var bit in bits)
        {
            value |= 1u << bit;
        }

        return new CollisionBitmask(value);
    }
}