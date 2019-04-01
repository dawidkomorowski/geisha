using System.IO;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Framework.Audio;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Audio.Assets
{
    internal sealed class SoundManagedAsset : ManagedAsset<ISound>
    {
        private readonly IAudioProvider _audioProvider;
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;

        public SoundManagedAsset(AssetInfo assetInfo, IAudioProvider audioProvider, IFileSystem fileSystem, IJsonSerializer jsonSerializer) : base(assetInfo)
        {
            _audioProvider = audioProvider;
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
                return _audioProvider.CreateSound(stream, soundFormat);
            }
        }

        protected override void UnloadAsset(ISound asset)
        {
            throw new System.NotImplementedException();
        }
    }
}