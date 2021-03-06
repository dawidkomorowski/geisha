﻿using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Mapping;

namespace Geisha.Engine.Input.Assets
{
    internal sealed class InputMappingManagedAssetFactory : IManagedAssetFactory
    {
        private readonly IFileSystem _fileSystem;

        public InputMappingManagedAssetFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public ISingleOrEmpty<IManagedAsset> Create(AssetInfo assetInfo, IAssetStore assetStore)
        {
            if (assetInfo.AssetType == typeof(InputMapping))
            {
                var managedAsset = new InputMappingManagedAsset(assetInfo, _fileSystem);
                return SingleOrEmpty.Single(managedAsset);
            }
            else
            {
                return SingleOrEmpty.Empty<IManagedAsset>();
            }
        }
    }
}