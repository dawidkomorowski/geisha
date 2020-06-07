using System;

namespace Geisha.Engine.Core.Assets
{
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
        public static AssetId CreateUnique() => new AssetId(Guid.NewGuid());

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

        #region Equality members

        /// <summary>
        ///     Returns a value indicating whether the value of this instance is equal to the value of the specified
        ///     <see cref="AssetId" /> instance.
        /// </summary>
        /// <param name="other">The object to compare to this instance.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="other" /> parameter equals the value of this instance; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(AssetId other) => Value.Equals(other.Value);

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="obj" /> is an instance of <see cref="AssetId" /> and equals the value of this
        ///     instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj) => obj is AssetId other && Equals(other);

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
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

        /// <summary>
        ///     Converts the value of the current <see cref="AssetId" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="AssetId" /> object.</returns>
        public override string ToString() => Value.ToString();
    }
}