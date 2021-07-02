using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Rendering.Assets
{
    internal sealed class SpriteManagedAssetFactory : IManagedAssetFactory
    {
        private readonly IFileSystem _fileSystem;

        public SpriteManagedAssetFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public ISingleOrEmpty<IManagedAsset> Create(AssetInfo assetInfo, IAssetStore assetStore)
        {
            if (assetInfo.AssetType == typeof(Sprite))
            {
                var managedAsset = new SpriteManagedAsset(assetInfo, _fileSystem, assetStore);
                return SingleOrEmpty.Single(managedAsset);
            }
            else
            {
                return SingleOrEmpty.Empty<IManagedAsset>();
            }
        }
    }
}