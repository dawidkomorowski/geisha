using Geisha.Common;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Audio.Assets
{
    internal sealed class SoundManagedAssetFactory : IManagedAssetFactory
    {
        private readonly IAudioProvider _audioProvider;
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;

        public SoundManagedAssetFactory(IAudioProvider audioProvider, IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            _audioProvider = audioProvider;
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
        }

        public ISingleOrEmpty<IManagedAsset> Create(AssetInfo assetInfo, IAssetStore assetStore)
        {
            if (assetInfo.AssetType == typeof(ISound))
            {
                var managedAsset = new SoundManagedAsset(assetInfo, _audioProvider, _fileSystem, _jsonSerializer);
                return SingleOrEmpty.Single(managedAsset);
            }
            else
            {
                return SingleOrEmpty.Empty<IManagedAsset>();
            }
        }
    }
}