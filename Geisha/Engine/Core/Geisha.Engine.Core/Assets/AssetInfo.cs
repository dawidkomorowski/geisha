﻿using System;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Asset registration info to be used in <see cref="IAssetStore" />.
    /// </summary>
    public sealed class AssetInfo : IEquatable<AssetInfo>
    {
        /// <summary>
        ///     Creates new instance of <see cref="AssetInfo" />.
        /// </summary>
        /// <param name="assetId">Id of asset.</param>
        /// <param name="assetType">Type of asset.</param>
        /// <param name="assetFilePath">Path to asset file.</param>
        public AssetInfo(AssetId assetId, Type assetType, string assetFilePath)
        {
            AssetId = assetId;
            AssetType = assetType ?? throw new ArgumentNullException(nameof(assetType));
            AssetFilePath = assetFilePath ?? throw new ArgumentNullException(nameof(assetFilePath));
        }

        /// <summary>
        ///     Id of asset.
        /// </summary>
        public AssetId AssetId { get; }

        /// <summary>
        ///     Type of asset.
        /// </summary>
        public Type AssetType { get; }

        /// <summary>
        ///     Path to asset file.
        /// </summary>
        public string AssetFilePath { get; }

        /// <summary>
        ///     Converts the value of the current <see cref="AssetInfo" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="AssetInfo" /> object.</returns>
        public override string ToString()
        {
            return $"{nameof(AssetId)}: {AssetId}, {nameof(AssetType)}: {AssetType}, {nameof(AssetFilePath)}: {AssetFilePath}";
        }

        #region Equality members

        public bool Equals(AssetInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AssetId.Equals(other.AssetId) && AssetType == other.AssetType && string.Equals(AssetFilePath, other.AssetFilePath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is AssetInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = AssetId.GetHashCode();
                hashCode = (hashCode * 397) ^ AssetType.GetHashCode();
                hashCode = (hashCode * 397) ^ AssetFilePath.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(AssetInfo left, AssetInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AssetInfo left, AssetInfo right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}