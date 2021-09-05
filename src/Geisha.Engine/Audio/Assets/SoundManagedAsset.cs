using System;
using System.IO;
using Geisha.Common.FileSystem;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Audio.Assets
{
    internal sealed class SoundManagedAsset : ManagedAsset<ISound>
    {
        private readonly IAudioBackend _audioBackend;
        private readonly IFileSystem _fileSystem;

        public SoundManagedAsset(AssetInfo assetInfo, IAudioBackend audioBackend, IFileSystem fileSystem) : base(assetInfo)
        {
            _audioBackend = audioBackend;
            _fileSystem = fileSystem;
        }

        protected override ISound LoadAsset()
        {
            using var assetFileStream = _fileSystem.GetFile(AssetInfo.AssetFilePath).OpenRead();
            var assetData = AssetData.Load(assetFileStream);
            var soundAssetContent = assetData.ReadJsonContent<SoundAssetContent>();

            var relativeSiblingPath = soundAssetContent.SoundFilePath ??
                                      throw new InvalidOperationException(
                                          $"{nameof(SoundAssetContent)}.{nameof(SoundAssetContent.SoundFilePath)} cannot be null.");

            var soundFilePath = PathUtils.GetSiblingPath(AssetInfo.AssetFilePath, relativeSiblingPath);
            var fileExtension = Path.GetExtension(soundFilePath);
            var soundFormat = SoundFormatParser.ParseFromFileExtension(fileExtension);

            using var soundFileStream = _fileSystem.GetFile(soundFilePath).OpenRead();
            return _audioBackend.CreateSound(soundFileStream, soundFormat);
        }

        protected override void UnloadAsset(ISound asset)
        {
            // Actual resources will be released when all instances of the sound will complete playing.
            asset.Dispose();
        }
    }
}