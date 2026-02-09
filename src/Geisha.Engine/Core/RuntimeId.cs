using System;
using System.Diagnostics;
using System.Threading;

namespace Geisha.Engine.Core;

/// <summary>
///     Represents general purpose identifier that allows to create unique IDs for a duration of a process runtime.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="RuntimeId" /> is meant to be used as a faster alternative to <see cref="Guid" />. It is based on
///         internal ID sequence that is incremented each time a new ID is requested. <see cref="RuntimeId" /> is stored
///         internally as 64bit unsigned integer with value zero reserved for invalid ID. It does not provide uniqueness
///         beyond a single runtime of a process. Therefore, it should not be stored in persistent storage for future
///         references, and it should not be used to identify objects created beyond single runtime of a process.
///     </para>
///     <para>
///         <see cref="RuntimeId" /> can be used to represent sortable ID for an instance of an object unlike its managed
///         reference.
///     </para>
/// </remarks>
public readonly record struct RuntimeId(ulong Id) : IComparable<RuntimeId>
{
    private static ulong _sequence;

    /// <summary>
    ///     Returns invalid <see cref="RuntimeId" />. It is ID with value equal zero.
    /// </summary>
    public static RuntimeId Invalid = default;

    /// <summary>
    ///     Creates new instance of <see cref="RuntimeId" /> which represents next ID in a sequence.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="RuntimeId" /> keeps track of ID sequence that is incremented each time <see cref="Next" /> method
    ///         is called. It allows to create unique IDs for a duration of a process runtime.
    ///     </para>
    ///     <para>
    ///         It is safe to call this method from multiple threads.
    ///     </para>
    /// </remarks>
    /// <returns>New instance of <see cref="RuntimeId" /> which represents next ID in a sequence.</returns>
    public static RuntimeId Next()
    {
        var id = Interlocked.Increment(ref _sequence);
        var runtimeId = new RuntimeId(id);
        Debug.Assert(runtimeId != Invalid, "runtimeId != Invalid");
        return runtimeId;
    }

    /// <inheritdoc />
    public int CompareTo(RuntimeId other) => Id.CompareTo(other.Id);
}