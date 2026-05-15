using System;
using System.Text;

namespace Geisha.Engine.Physics;

/// <summary>
///     Represents a 32-bit bitmask used for collision filtering in the physics system.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="CollisionBitmask" /> is used by <see cref="Components.Collider2DComponent.CollisionLayer" /> and
///         <see cref="Components.Collider2DComponent.CollisionMask" /> to describe layer membership and allowed collision
///         targets.
///     </para>
///     <para>
///         During collision filtering between colliders A and B, a pair is accepted only when both directional checks pass:
///         (A.Layer &amp; B.Mask) != 0 and (A.Mask &amp; B.Layer) != 0.
///     </para>
/// </remarks>
public readonly record struct CollisionBitmask
{
    /// <summary>
    ///     Initializes a new instance of <see cref="CollisionBitmask" /> with a raw 32-bit value.
    /// </summary>
    /// <param name="value">Raw bitmask value.</param>
    public CollisionBitmask(uint value)
    {
        Value = value;
    }

    /// <summary>
    ///     Gets the raw 32-bit bitmask value.
    /// </summary>
    public uint Value { get; init; }

    /// <summary>
    ///     Determines whether a specified bit is set.
    /// </summary>
    /// <param name="bit">Zero-based bit index in range [0, 31].</param>
    /// <returns><see langword="true" /> when the specified bit is set; otherwise <see langword="false" />.</returns>
    public bool HasBit(int bit) => (Value & 1u << bit) != 0;

    /// <summary>
    ///     Returns a new bitmask with the specified bit set.
    /// </summary>
    /// <param name="bit">Zero-based bit index in range [0, 31].</param>
    /// <returns>New <see cref="CollisionBitmask" /> with the specified bit set.</returns>
    public CollisionBitmask WithBit(int bit) => new(Value | 1u << bit);

    /// <summary>
    ///     Returns a new bitmask with the specified bit cleared.
    /// </summary>
    /// <param name="bit">Zero-based bit index in range [0, 31].</param>
    /// <returns>New <see cref="CollisionBitmask" /> with the specified bit cleared.</returns>
    public CollisionBitmask WithoutBit(int bit) => new(Value & ~(1u << bit));

    /// <summary>
    ///     Gets a bitmask with no bits set.
    /// </summary>
    public static CollisionBitmask None { get; } = new(0);

    /// <summary>
    ///     Gets a bitmask with all bits set.
    /// </summary>
    public static CollisionBitmask All { get; } = new(uint.MaxValue);

    /// <summary>
    ///     Creates a bitmask from a raw 32-bit value.
    /// </summary>
    /// <param name="value">Raw bitmask value.</param>
    /// <returns><see cref="CollisionBitmask" /> instance with the specified value.</returns>
    public static CollisionBitmask FromValue(uint value) => new(value);

    /// <summary>
    ///     Creates a bitmask with exactly one bit set.
    /// </summary>
    /// <param name="bit">Zero-based bit index in range [0, 31].</param>
    /// <returns><see cref="CollisionBitmask" /> instance with a single set bit.</returns>
    public static CollisionBitmask FromBit(int bit) => new(1u << bit);

    // TODO: This implementation may force the caller to allocate.
    //       When migrated to .NET 9 (C# 13) params collection feature could be used to accept ReadOnlySpan.
    /// <summary>
    ///     Creates a bitmask with all specified bits set.
    /// </summary>
    /// <param name="bits">Collection of zero-based bit indices in range [0, 31].</param>
    /// <returns><see cref="CollisionBitmask" /> instance with all specified bits set.</returns>
    public static CollisionBitmask FromBits(params int[] bits) => FromBits(bits.AsSpan());

    /// <summary>
    ///     Creates a bitmask with all specified bits set.
    /// </summary>
    /// <param name="bits">Span of zero-based bit indices in range [0, 31].</param>
    /// <returns><see cref="CollisionBitmask" /> instance with all specified bits set.</returns>
    public static CollisionBitmask FromBits(ReadOnlySpan<int> bits)
    {
        uint value = 0;

        foreach (var bit in bits)
        {
            value |= 1u << bit;
        }

        return new CollisionBitmask(value);
    }

    /// <summary>
    ///     Performs bitwise AND operation on two collision bitmasks.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>New <see cref="CollisionBitmask" /> containing the result of bitwise AND.</returns>
    public static CollisionBitmask operator &(CollisionBitmask left, CollisionBitmask right) => new(left.Value & right.Value);

    /// <summary>
    ///     Performs bitwise OR operation on two collision bitmasks.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>New <see cref="CollisionBitmask" /> containing the result of bitwise OR.</returns>
    public static CollisionBitmask operator |(CollisionBitmask left, CollisionBitmask right) => new(left.Value | right.Value);

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