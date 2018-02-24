using System;
using System.ComponentModel.Composition;
using System.IO;
using Geisha.Common.Math;
using Geisha.Framework.Audio;
using Geisha.Framework.Rendering;

namespace Geisha.TestGame
{
    public interface IAssetsLoader
    {
        Sprite DotSprite { get; }
        Sprite BoxSprite { get; }
        Sprite CompassSprite { get; }
        ISound Music { get; }
        ISound DotDieSound { get; }
    }

    [Export(typeof(IAssetsLoader))]
    public class AssetsLoader : IAssetsLoader
    {
        private const string ResourcesRootPath = @"Assets\";
        private readonly IAudioProvider _audioProvider;
        private readonly IRenderer2D _renderer2D;

        // Assets cache
        private readonly Lazy<Sprite> _dotSprite;
        private readonly Lazy<Sprite> _boxSprite;
        private readonly Lazy<Sprite> _compassSprite;
        private readonly Lazy<ISound> _music;
        private readonly Lazy<ISound> _dotDieSound;

        [ImportingConstructor]
        public AssetsLoader(IRenderer2D renderer2D, IAudioProvider audioProvider)
        {
            _renderer2D = renderer2D;
            _audioProvider = audioProvider;

            // Init assets cache
            _dotSprite = new Lazy<Sprite>(CreateDotSprite);
            _boxSprite = new Lazy<Sprite>(CreateBoxSprite);
            _compassSprite = new Lazy<Sprite>(CreateCompassSprite);
            _music = new Lazy<ISound>(LoadMusic);
            _dotDieSound = new Lazy<ISound>(LoadDotDieSound);
        }

        public Sprite DotSprite => _dotSprite.Value;
        public Sprite BoxSprite => _boxSprite.Value;
        public Sprite CompassSprite => _compassSprite.Value;
        public ISound Music => _music.Value;
        public ISound DotDieSound => _dotDieSound.Value;

        private Sprite CreateDotSprite()
        {
            var dotTexture = LoadTexture("Dot.png");

            return new Sprite
            {
                SourceTexture = dotTexture,
                SourceUV = Vector2.Zero,
                SourceDimension = dotTexture.Dimension,
                SourceAnchor = dotTexture.Dimension / 2,
                PixelsPerUnit = 1
            };
        }

        private Sprite CreateBoxSprite()
        {
            var boxTexture = LoadTexture("box.jpg");

            return new Sprite
            {
                SourceTexture = boxTexture,
                SourceUV = Vector2.Zero,
                SourceDimension = boxTexture.Dimension,
                SourceAnchor = boxTexture.Dimension / 2,
                PixelsPerUnit = 1
            };
        }

        private Sprite CreateCompassSprite()
        {
            var compassTexture = LoadTexture("compass_texture.png");

            return new Sprite
            {
                SourceTexture = compassTexture,
                SourceUV = Vector2.Zero,
                SourceDimension = compassTexture.Dimension,
                SourceAnchor = compassTexture.Dimension / 2,
                PixelsPerUnit = 1
            };
        }

        private ISound LoadMusic()
        {
            return LoadSound(@"C:\Users\Dawid Komorowski\Downloads\Heroic_Demise_New_.wav");
        }

        private ISound LoadDotDieSound()
        {
            return LoadSound(@"C:\Users\Dawid Komorowski\Downloads\shimmer_1 (online-audio-converter.com).mp3");
        }

        private ITexture LoadTexture(string filePath)
        {
            filePath = Path.Combine(ResourcesRootPath, filePath);
            using (Stream stream = new FileStream(filePath, FileMode.Open))
            {
                return _renderer2D.CreateTexture(stream);
            }
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