using System;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Represents type information of an asset independent of actual asset class.
    /// </summary>
    public readonly struct AssetType : IEquatable<AssetType>
    {
        private readonly string? _value;

        /// <summary>
        ///     Creates new instance of <see cref="AssetType" /> given <see cref="string" /> value representing the asset type.
        /// </summary>
        /// <param name="value">
        ///     Value representing the asset type. It can be any kind of unique identifier of the asset type but it
        ///     may be useful to have it human readable.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value" /> is <c>null</c>.</exception>
        public AssetType(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        ///     Actual representation of asset type information. Default value is empty <see cref="string" />.
        /// </summary>
        public string Value => _value ?? string.Empty;

        /// <summary>
        ///     Returns <see cref="Value" /> of this <see cref="AssetType" /> instance.
        /// </summary>
        /// <returns><see cref="Value" /> of this <see cref="AssetType" /> instance.</returns>
        public override string ToString() => Value;

        #region Equality members

        /// <inheritdoc />
        public bool Equals(AssetType other) => Value == other.Value;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is AssetType other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        ///     Determines whether two specified instances of <see cref="AssetType" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="AssetType" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(AssetType left, AssetType right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="AssetType" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="AssetType" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(AssetType left, AssetType right) => !left.Equals(right);

        #endregion
    }
}