using System;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Asset registration info to be used in <see cref="IAssetStore" />.
    /// </summary>
    public class AssetInfo
    {
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
        ///     File path to asset file.
        /// </summary>
        public string AssetFilePath { get; }

        public override string ToString()
        {
            return $"{nameof(AssetType)}: {AssetType}, {nameof(AssetId)}: {AssetId}, {nameof(AssetFilePath)}: {AssetFilePath}";
        }
    }
}