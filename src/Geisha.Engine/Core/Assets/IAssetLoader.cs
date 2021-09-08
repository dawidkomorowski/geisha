using System;

namespace Geisha.Engine.Core.Assets
{
    public interface IAssetLoader
    {
        AssetType AssetType { get; }
        Type AssetClassType { get; }

        object LoadAsset(AssetInfo assetInfo, IAssetStore assetStore);
        void UnloadAsset(object asset);
    }
}