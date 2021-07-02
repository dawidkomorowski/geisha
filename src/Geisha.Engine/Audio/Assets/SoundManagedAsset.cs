using System;
using System.IO;
using System.Text.Json;
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
            var soundFile = _fileSystem.GetFile(AssetInfo.AssetFilePath);
            var soundFileContent = JsonSerializer.Deserialize<SoundFileContent>(soundFile.ReadAllText());

            if (soundFileContent is null) throw new InvalidOperationException($"{nameof(SoundFileContent)} cannot be null.");

            var relativeSiblingPath = soundFileContent.SoundFilePath ??
                                      throw new InvalidOperationException(
                                          $"{nameof(SoundFileContent)}.{nameof(SoundFileContent.SoundFilePath)} cannot be null.");

            var soundFilePath = PathUtils.GetSiblingPath(AssetInfo.AssetFilePath, relativeSiblingPath);
            var fileExtension = Path.GetExtension(soundFilePath);
            var soundFormat = SoundFormatParser.ParseFromFileExtension(fileExtension);

            using var stream = _fileSystem.GetFile(soundFilePath).OpenRead();
            return _audioBackend.CreateSound(stream, soundFormat);
        }

        protected override void UnloadAsset(ISound asset)
        {
            // Actual resources will be released when all instances of the sound will complete playing.
            asset.Dispose();
        }
    }
}