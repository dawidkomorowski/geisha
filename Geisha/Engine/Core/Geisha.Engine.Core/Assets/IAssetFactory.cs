using Geisha.Common;

namespace Geisha.Engine.Core.Assets
{
    // TODO Add XML documentation.
    public interface IAssetFactory
    {
        ISingleOrEmpty<IAsset> Create(AssetInfo assetInfo, IAssetStore assetStore);
    }
}