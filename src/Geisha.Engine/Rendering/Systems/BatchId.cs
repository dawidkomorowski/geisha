using System;
using Geisha.Engine.Core;

namespace Geisha.Engine.Rendering.Systems;

// TODO: Use in Renderer.
// TODO: Convert RuntimeId to record struct and use compiler generated comparison and equality members if possible.
internal readonly record struct BatchId(RuntimeId ResourceId, byte Flags) : IComparable<BatchId>
{
    public static readonly BatchId Empty = new(RuntimeId.Invalid, 0);

    public BatchId WithFlag(int index, bool value)
    {
        if (index is < 0 or > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 7.");
        }

        return this with { Flags = (byte)(value ? Flags | (1 << index) : Flags & ~(1 << index)) };
    }

    public int CompareTo(BatchId other)
    {
        var idComparison = ResourceId.CompareTo(other.ResourceId);
        return idComparison != 0 ? idComparison : Flags.CompareTo(other.Flags);
    }
}