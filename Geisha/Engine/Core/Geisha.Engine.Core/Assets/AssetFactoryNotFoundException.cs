using System;

namespace Geisha.Engine.Core.Assets
{
    public sealed class AssetFactoryNotFoundException : Exception
    {
        public AssetFactoryNotFoundException(AssetInfo assetInfo) : base(
            $"No asset factory found for asset info: {assetInfo}. Single implementation of {nameof(IAssetFactory)} per asset type is required.")
        {
            AssetInfo = assetInfo;
        }

        public AssetInfo AssetInfo { get; }
    }
}