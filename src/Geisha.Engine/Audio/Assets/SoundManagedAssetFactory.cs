﻿using Geisha.Common;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Audio.Assets
{
    internal sealed class SoundManagedAssetFactory : IManagedAssetFactory
    {
        private readonly IAudioBackend _audioBackend;
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;

        public SoundManagedAssetFactory(IAudioBackend audioBackend, IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            _audioBackend = audioBackend;
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
        }

        public ISingleOrEmpty<IManagedAsset> Create(AssetInfo assetInfo, IAssetStore assetStore)
        {
            if (assetInfo.AssetType == typeof(ISound))
            {
                var managedAsset = new SoundManagedAsset(assetInfo, _audioBackend, _fileSystem, _jsonSerializer);
                return SingleOrEmpty.Single(managedAsset);
            }
            else
            {
                return SingleOrEmpty.Empty<IManagedAsset>();
            }
        }
    }
}