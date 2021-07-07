using System;
using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Animation.Assets
{
    internal sealed class SpriteAnimationManagedAssetFactory : IManagedAssetFactory
    {
        private readonly IFileSystem _fileSystem;

        public SpriteAnimationManagedAssetFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public ISingleOrEmpty<IManagedAsset> Create(AssetInfo assetInfo, IAssetStore assetStore)
        {
            throw new NotImplementedException();
            //if (assetInfo.AssetType == typeof(SpriteAnimation))
            //{
            //    var managedAsset = new SpriteAnimationManagedAsset(assetInfo, _fileSystem, assetStore);
            //    return SingleOrEmpty.Single(managedAsset);
            //}
            //else
            //{
            //    return SingleOrEmpty.Empty<IManagedAsset>();
            //}
        }
    }
}