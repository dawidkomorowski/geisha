using System.IO;
using Geisha.Common.Serialization;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Framework.Audio;
using Geisha.Framework.FileSystem;

namespace Geisha.Engine.Audio.Assets
{
    /// <summary>
    ///     Provides functionality to load <see cref="ISound" /> from sound file.
    /// </summary>
    internal sealed class SoundLoader : AssetLoaderAdapter<ISound>
    {
        private readonly IAudioProvider _audioProvider;
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;

        public SoundLoader(IAudioProvider audioProvider, IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            _audioProvider = audioProvider;
            _fileSystem = fileSystem;
            _jsonSerializer = jsonSerializer;
        }

        protected override ISound LoadAsset(string filePath)
        {
            var soundFile = _fileSystem.GetFile(filePath);
            var soundFileContent = _jsonSerializer.Deserialize<SoundFileContent>(soundFile.ReadAllText());

            var soundFilePath = PathUtils.GetSiblingPath(filePath, soundFileContent.SoundFilePath);
            using (var stream = _fileSystem.GetFile(soundFilePath).OpenRead())
            {
                return _audioProvider.CreateSound(stream, GetSoundFormat(soundFilePath));
            }
        }

        private static SoundFormat GetSoundFormat(string filePath)
        {
            var fileExtension = Path.GetExtension(filePath);

            switch (fileExtension)
            {
                case ".wav":
                    return SoundFormat.Wav;
                case ".mp3":
                    return SoundFormat.Mp3;
                default:
                    throw new GeishaEngineException($"Unsupported sound file format: {fileExtension}.");
            }
        }
    }
}