﻿using Geisha.Common;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Rendering.Assets
{
    internal sealed class SpriteManagedAssetFactory : IManagedAssetFactory
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;

        public SpriteManagedAssetFactory(IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
        }

        public ISingleOrEmpty<IManagedAsset> Create(AssetInfo assetInfo, IAssetStore assetStore)
        {
            if (assetInfo.AssetType == typeof(Sprite))
            {
                var managedAsset = new SpriteManagedAsset(assetInfo, _fileSystem, _jsonSerializer, assetStore);
                return SingleOrEmpty.Single(managedAsset);
            }
            else
            {
                return SingleOrEmpty.Empty<IManagedAsset>();
            }
        }
    }
}