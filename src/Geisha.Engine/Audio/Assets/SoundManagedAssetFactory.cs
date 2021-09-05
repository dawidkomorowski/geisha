using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Audio.Assets
{
    internal sealed class SoundManagedAssetFactory : IManagedAssetFactory
    {
        private readonly IAudioBackend _audioBackend;
        private readonly IFileSystem _fileSystem;

        public SoundManagedAssetFactory(IAudioBackend audioBackend, IFileSystem fileSystem)
        {
            _audioBackend = audioBackend;
            _fileSystem = fileSystem;
        }

        public ISingleOrEmpty<IManagedAsset> Create(AssetInfo assetInfo, IAssetStore assetStore)
        {
            if (assetInfo.AssetType == AudioAssetTypes.Sound)
            {
                var managedAsset = new SoundManagedAsset(assetInfo, _audioBackend, _fileSystem);
                return SingleOrEmpty.Single(managedAsset);
            }
            else
            {
                return SingleOrEmpty.Empty<IManagedAsset>();
            }
        }
    }
}