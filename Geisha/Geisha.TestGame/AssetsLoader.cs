using System;
using System.ComponentModel.Composition;
using System.IO;
using Geisha.Framework.Audio;

namespace Geisha.TestGame
{
    public interface IAssetsLoader
    {
        ISound Music { get; }
        ISound DotDieSound { get; }
    }

    [Export(typeof(IAssetsLoader))]
    public class AssetsLoader : IAssetsLoader
    {
        private const string ResourcesRootPath = @"Assets\";
        private readonly IAudioProvider _audioProvider;

        // Assets cache
        private readonly Lazy<ISound> _dotDieSound;
        private readonly Lazy<ISound> _music;

        [ImportingConstructor]
        public AssetsLoader(IAudioProvider audioProvider)
        {
            _audioProvider = audioProvider;

            // Init assets cache
            _music = new Lazy<ISound>(LoadMusic);
            _dotDieSound = new Lazy<ISound>(LoadDotDieSound);
        }

        public ISound Music => _music.Value;
        public ISound DotDieSound => _dotDieSound.Value;

        private ISound LoadMusic()
        {
            return LoadSound(@"C:\Users\Dawid Komorowski\Downloads\Heroic_Demise_New_.wav");
        }

        private ISound LoadDotDieSound()
        {
            return LoadSound(@"C:\Users\Dawid Komorowski\Downloads\shimmer_1 (online-audio-converter.com).mp3");
        }

        private ISound LoadSound(string filePath)
        {
            filePath = Path.Combine(ResourcesRootPath, filePath);

            using (Stream stream = new FileStream(filePath, FileMode.Open))
            {
                return _audioProvider.CreateSound(stream, GetFormat(filePath));
            }
        }

        private static SoundFormat GetFormat(string filePath)
        {
            var fileExtension = Path.GetExtension(filePath);

            switch (fileExtension)
            {
                case ".wav":
                    return SoundFormat.Wav;
                case ".mp3":
                    return SoundFormat.Mp3;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}