using System;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Asset registration info to be used in <see cref="IAssetStore" />.
    /// </summary>
    public class AssetInfo
    {
        /// <summary>
        ///     Creates new instance of <see cref="AssetInfo" />.
        /// </summary>
        /// <param name="assetType">Type of asset.</param>
        /// <param name="assetId">Id of asset.</param>
        /// <param name="assetFilePath">Path to asset file.</param>
        public AssetInfo(Type assetType, Guid assetId, string assetFilePath)
        {
            AssetType = assetType;
            AssetId = assetId;
            AssetFilePath = assetFilePath;
        }

        /// <summary>
        ///     Type of asset.
        /// </summary>
        public Type AssetType { get; }

        /// <summary>
        ///     Id of asset.
        /// </summary>
        public Guid AssetId { get; }

        /// <summary>
        ///     Path to asset file.
        /// </summary>
        public string AssetFilePath { get; }

        /// <summary>
        ///     Returns textual representation of <see cref="AssetInfo" />.
        /// </summary>
        /// <returns>Textual representation of <see cref="AssetInfo" />.</returns>
        public override string ToString()
        {
            return $"{nameof(AssetType)}: {AssetType}, {nameof(AssetId)}: {AssetId}, {nameof(AssetFilePath)}: {AssetFilePath}";
        }
    }
}