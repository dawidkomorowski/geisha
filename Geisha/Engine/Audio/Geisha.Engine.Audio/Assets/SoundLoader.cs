using System.IO;
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

        public SoundLoader(IAudioProvider audioProvider, IFileSystem fileSystem)
        {
            _audioProvider = audioProvider;
            _fileSystem = fileSystem;
        }

        protected override ISound LoadAsset(string filePath)
        {
            using (var stream = _fileSystem.OpenFileStreamForReading(filePath))
            {
                return _audioProvider.CreateSound(stream, GetSoundFormat(filePath));
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