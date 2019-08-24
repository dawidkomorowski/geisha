using System.IO;
using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Audio.Assets
{
    internal sealed class SoundManagedAsset : ManagedAsset<ISound>
    {
        private readonly IAudioBackend _audioBackend;
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;

        public SoundManagedAsset(AssetInfo assetInfo, IAudioBackend audioBackend, IFileSystem fileSystem, IJsonSerializer jsonSerializer) : base(assetInfo)
        {
            _audioBackend = audioBackend;
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
        }

        protected override ISound LoadAsset()
        {
            var soundFile = _fileSystem.GetFile(AssetInfo.AssetFilePath);
            var soundFileContent = _jsonSerializer.Deserialize<SoundFileContent>(soundFile.ReadAllText());

            var soundFilePath = PathUtils.GetSiblingPath(AssetInfo.AssetFilePath, soundFileContent.SoundFilePath);
            var fileExtension = Path.GetExtension(soundFilePath);
            var soundFormat = SoundFormatParser.ParseFromFileExtension(fileExtension);

            using (var stream = _fileSystem.GetFile(soundFilePath).OpenRead())
            {
                return _audioBackend.CreateSound(stream, soundFormat);
            }
        }

        protected override void UnloadAsset(ISound asset)
        {
            // Actual resources will be released when all instances of the sound will complete playing.
            asset.Dispose();
        }
    }
}