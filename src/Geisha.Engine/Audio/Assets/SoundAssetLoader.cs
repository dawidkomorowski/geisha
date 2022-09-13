using System;
using System.IO;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.FileSystem;

namespace Geisha.Engine.Audio.Assets
{
    internal sealed class SoundAssetLoader : IAssetLoader
    {
        private readonly IAudioBackend _audioBackend;
        private readonly IFileSystem _fileSystem;

        public SoundAssetLoader(IAudioBackend audioBackend, IFileSystem fileSystem)
        {
            _audioBackend = audioBackend;
            _fileSystem = fileSystem;
        }

        public AssetType AssetType => AudioAssetTypes.Sound;
        public Type AssetClassType { get; } = typeof(ISound);

        public object LoadAsset(AssetInfo assetInfo, IAssetStore assetStore)
        {
            using var assetFileStream = _fileSystem.GetFile(assetInfo.AssetFilePath).OpenRead();
            var assetData = AssetData.Load(assetFileStream);
            var soundAssetContent = assetData.ReadJsonContent<SoundAssetContent>();

            var relativeSiblingPath = soundAssetContent.SoundFilePath ??
                                      throw new InvalidOperationException(
                                          $"{nameof(SoundAssetContent)}.{nameof(SoundAssetContent.SoundFilePath)} cannot be null.");

            var soundFilePath = PathUtils.GetSiblingPath(assetInfo.AssetFilePath, relativeSiblingPath);
            var fileExtension = Path.GetExtension(soundFilePath);
            var soundFormat = SoundFormatParser.ParseFromFileExtension(fileExtension);

            using var soundFileStream = _fileSystem.GetFile(soundFilePath).OpenRead();
            return _audioBackend.CreateSound(soundFileStream, soundFormat);
        }

        public void UnloadAsset(object asset)
        {
            var sound = (ISound)asset;
            // Actual resources will be released when all instances of the sound will complete playing.
            sound.Dispose();
        }
    }
}