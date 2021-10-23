using System;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Asset registration info to be used in <see cref="IAssetStore" />.
    /// </summary>
    public readonly struct AssetInfo : IEquatable<AssetInfo>
    {
        private readonly string? _assetFilePath;

        /// <summary>
        ///     Creates new instance of <see cref="AssetInfo" />.
        /// </summary>
        /// <param name="assetId">Id of asset.</param>
        /// <param name="assetType">Type of asset.</param>
        /// <param name="assetFilePath">Path to asset file.</param>
        public AssetInfo(AssetId assetId, AssetType assetType, string assetFilePath)
        {
            AssetId = assetId;
            AssetType = assetType;
            _assetFilePath = assetFilePath ?? throw new ArgumentNullException(nameof(assetFilePath));
        }

        /// <summary>
        ///     Id of asset.
        /// </summary>
        public AssetId AssetId { get; }

        /// <summary>
        ///     Type of asset.
        /// </summary>
        public AssetType AssetType { get; }

        /// <summary>
        ///     Path to asset file.
        /// </summary>
        public string AssetFilePath => _assetFilePath ?? string.Empty;

        /// <summary>
        ///     Converts the value of the current <see cref="AssetInfo" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="AssetInfo" /> object.</returns>
        public override string ToString() => $"{nameof(AssetId)}: {AssetId}, {nameof(AssetType)}: {AssetType}, {nameof(AssetFilePath)}: {AssetFilePath}";

        #region Equality members

        /// <inheritdoc />
        public bool Equals(AssetInfo other) => AssetId.Equals(other.AssetId) && AssetType.Equals(other.AssetType) && AssetFilePath == other.AssetFilePath;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is AssetInfo other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(AssetId, AssetType, AssetFilePath);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="AssetInfo" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="AssetInfo" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(AssetInfo left, AssetInfo right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="AssetInfo" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="AssetInfo" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(AssetInfo left, AssetInfo right) => !left.Equals(right);

        #endregion
    }
}