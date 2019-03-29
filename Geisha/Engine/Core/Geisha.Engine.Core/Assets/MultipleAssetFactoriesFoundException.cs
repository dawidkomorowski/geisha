using System;

namespace Geisha.Engine.Core.Assets
{
    public sealed class MultipleAssetFactoriesFoundException : Exception
    {
        public MultipleAssetFactoriesFoundException(AssetInfo assetInfo) : base(
            $"Multiple asset factories found for asset info: {assetInfo}. Single implementation of {nameof(IAssetFactory)} per asset type is required.")
        {
            AssetInfo = assetInfo;
        }

        public AssetInfo AssetInfo { get; }
    }
}