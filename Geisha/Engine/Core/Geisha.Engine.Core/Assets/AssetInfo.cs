using System;

namespace Geisha.Engine.Core.Assets
{
    public class AssetInfo
    {
        public AssetInfo(Type assetType, Guid assetId, string assetFilePath)
        {
            AssetType = assetType;
            AssetId = assetId;
            AssetFilePath = assetFilePath;
        }

        public Type AssetType { get; }
        public Guid AssetId { get; }
        public string AssetFilePath { get; }

        public override string ToString()
        {
            return $"{nameof(AssetType)}: {AssetType}, {nameof(AssetId)}: {AssetId}, {nameof(AssetFilePath)}: {AssetFilePath}";
        }
    }
}