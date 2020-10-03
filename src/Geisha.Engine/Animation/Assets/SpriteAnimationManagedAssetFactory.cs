using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Animation.Assets
{
    internal sealed class SpriteAnimationManagedAssetFactory : IManagedAssetFactory
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;

        public SpriteAnimationManagedAssetFactory(IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
        }

        public ISingleOrEmpty<IManagedAsset> Create(AssetInfo assetInfo, IAssetStore assetStore)
        {
            if (assetInfo.AssetType == typeof(SpriteAnimation))
            {
                var managedAsset = new SpriteAnimationManagedAsset(assetInfo, _fileSystem, _jsonSerializer, assetStore);
                return SingleOrEmpty.Single(managedAsset);
            }
            else
            {
                return SingleOrEmpty.Empty<IManagedAsset>();
            }
        }
    }
}