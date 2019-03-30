using Geisha.Common;

namespace Geisha.Engine.Core.Assets
{
    // TODO Add XML documentation.
    public interface IManagedAssetFactory
    {
        ISingleOrEmpty<IManagedAsset> Create(AssetInfo assetInfo, IAssetStore assetStore);
    }
}