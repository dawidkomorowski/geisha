using System;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Asset registration info to be used in <see cref="IAssetStore" />.
    /// </summary>
    public sealed class AssetInfo
    {
        /// <summary>
        ///     Creates new instance of <see cref="AssetInfo" />.
        /// </summary>
        /// <param name="assetId">Id of asset.</param>
        /// <param name="assetType">Type of asset.</param>
        /// <param name="assetFilePath">Path to asset file.</param>
        public AssetInfo(AssetId assetId, Type assetType, string assetFilePath)
        {
            AssetType = assetType;
            AssetId = assetId;
            AssetFilePath = assetFilePath;
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
    }
}