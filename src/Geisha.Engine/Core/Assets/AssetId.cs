using System;

namespace Geisha.Engine.Core.Assets;

/// <summary>
///     Represents type safe identifier of an asset.
/// </summary>
public readonly struct AssetId : IEquatable<AssetId>
{
    /// <summary>
    ///     Creates new, globally unique, instance of <see cref="AssetId" />.
    /// </summary>
    /// <returns>New, globally unique, instance of <see cref="AssetId" />.</returns>
    /// <remarks>Uniqueness of created <see cref="AssetId" /> instances is based on uniqueness of <see cref="Guid" />.</remarks>
    public static AssetId CreateUnique() => new(Guid.NewGuid());

    /// <summary>
    ///     Creates new instance of <see cref="AssetId" /> with <see cref="Value" /> set as specified by
    ///     <paramref name="value" />.
    /// </summary>
    /// <param name="value"></param>
    public AssetId(Guid value)
    {
        Value = value;
    }

    /// <summary>
    ///     Actual value of identifier.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    ///     Converts the value of the current <see cref="AssetId" /> object to its equivalent string representation.
    /// </summary>
    /// <returns>A string representation of the value of the current <see cref="AssetId" /> object.</returns>
    public override string ToString() => Value.ToString().ToUpper();

    #region Equality members

    /// <inheritdoc />
    public bool Equals(AssetId other) => Value.Equals(other.Value);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is AssetId other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    ///     Determines whether two specified instances of <see cref="AssetId" /> are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
    ///     <see cref="AssetId" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(AssetId left, AssetId right) => left.Equals(right);

    /// <summary>
    ///     Determines whether two specified instances of <see cref="AssetId" /> are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
    ///     <see cref="AssetId" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(AssetId left, AssetId right) => !left.Equals(right);

    #endregion
}