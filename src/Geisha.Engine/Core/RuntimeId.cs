using System;
using System.Diagnostics;
using System.Threading;

namespace Geisha.Engine.Core;

/// <summary>
///     Represents general purpose identifier that allows to create unique IDs during single runtime of a process.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="RuntimeId" /> is meant to be used as a faster alternative to <see cref="Guid" />. It is based on
///         internal ID sequence that is incremented each time a new ID is requested. <see cref="RuntimeId" /> is stored
///         internally as 64bit unsigned integer with value zero reserved for invalid ID. It does not provide uniqueness
///         beyond
///         single runtime of a process. Therefore it should not be stored in persistent storage for future references and
///         it should not be used to identify objects created beyond single runtime of a process.
///     </para>
///     <para>
///         <see cref="RuntimeId" /> can be used to represent sortable ID for an instance of an object unlike its managed
///         reference.
///     </para>
/// </remarks>
public readonly struct RuntimeId : IComparable<RuntimeId>, IEquatable<RuntimeId>
{
    private static ulong _sequence;
    private readonly ulong _id;

    /// <summary>
    ///     Returns invalid <see cref="RuntimeId" />. It is ID with internal value equal zero.
    /// </summary>
    public static RuntimeId Invalid = default;

    /// <summary>
    ///     Creates new instance of <see cref="RuntimeId" /> which represents next ID in a sequence.
    /// </summary>
    /// <remarks>
    ///     <see cref="RuntimeId" /> keeps track of ID sequence that is incremented each time <see cref="Next" /> method
    ///     is called. It allows to create unique IDs during single runtime of a process.
    /// </remarks>
    /// <returns>New instance of <see cref="RuntimeId" /> which represents next ID in a sequence.</returns>
    public static RuntimeId Next()
    {
        var id = Interlocked.Increment(ref _sequence);
        var runtimeId = new RuntimeId(id);
        Debug.Assert(runtimeId != Invalid, "runtimeId != Invalid");
        return runtimeId;
    }

    /// <summary>
    ///     Creates new instance of <see cref="RuntimeId" /> that is initialized with specified <paramref name="id" />.
    /// </summary>
    /// <param name="id">Id value.</param>
    public RuntimeId(ulong id)
    {
        _id = id;
    }

    /// <summary>
    ///     Converts the value of the current <see cref="RuntimeId" /> object to its equivalent string representation.
    /// </summary>
    /// <returns>A string representation of the value of the current <see cref="RuntimeId" /> object.</returns>
    public override string ToString() => _id.ToString();

    /// <inheritdoc />
    public int CompareTo(RuntimeId other) => _id.CompareTo(other._id);

    /// <inheritdoc />
    public bool Equals(RuntimeId other) => _id == other._id;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is RuntimeId other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => _id.GetHashCode();

    /// <summary>
    ///     Determines whether two specified instances of <see cref="RuntimeId" /> are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
    ///     <see cref="RuntimeId" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(RuntimeId left, RuntimeId right) => left.Equals(right);

    /// <summary>
    ///     Determines whether two specified instances of <see cref="RuntimeId" /> are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
    ///     <see cref="RuntimeId" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(RuntimeId left, RuntimeId right) => !left.Equals(right);
}