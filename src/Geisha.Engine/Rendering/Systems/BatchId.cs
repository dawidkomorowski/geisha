using System;
using Geisha.Engine.Core;

namespace Geisha.Engine.Rendering.Systems;

// TODO: Add unit tests.
// TODO: Refine implementation.
// TODO: Use in Renderer.
internal readonly record struct BatchId : IComparable<BatchId>
{
    public static readonly BatchId Invalid = new(RuntimeId.Invalid, 0);

    public BatchId(RuntimeId id, byte flags)
    {
        Id = id;
        Flags = flags;
    }

    public RuntimeId Id { get; init; }
    public byte Flags { get; init; }

    public BatchId WithFlag(int index, bool value)
    {
        return this with { Flags = (byte)(Flags | (value ? (1 << index) : 0)) };
    }

    public int CompareTo(BatchId other)
    {
        var idComparison = Id.CompareTo(other.Id);
        return idComparison != 0 ? idComparison : Flags.CompareTo(other.Flags);
    }
}