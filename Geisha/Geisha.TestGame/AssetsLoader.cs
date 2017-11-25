using System.ComponentModel.Composition;
using System.IO;
using Geisha.Common.Math;
using Geisha.Framework.Audio;
using Geisha.Framework.Rendering;

namespace Geisha.TestGame
{
    public interface IAssetsLoader
    {
        Sprite CreateDotSprite();
        Sprite CreateBoxSprite();
        Sprite CreateCompassSprite();
        void PlayMusic();
    }

    [Export(typeof(IAssetsLoader))]
    public class AssetsLoader : IAssetsLoader
    {
        private const string ResourcesRootPath = @"Assets\";
        private readonly IRenderer2D _renderer2D;
        private readonly ISoundPlayer _soundPlayer;

        [ImportingConstructor]
        public AssetsLoader(IRenderer2D renderer2D, ISoundPlayer soundPlayer)
        {
            _renderer2D = renderer2D;
            _soundPlayer = soundPlayer;
        }

        public Sprite CreateDotSprite()
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

        public Sprite CreateBoxSprite()
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

        public Sprite CreateCompassSprite()
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

        private ISound _music;

        public void PlayMusic()
        {
            if(_music == null)
            {
                var music = LoadSound(@"C:\Users\Dawid Komorowski\Downloads\Heroic_Demise_New_.wav");
                _music = music;
            }
            _soundPlayer.Play(_music);
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
                return _soundPlayer.CreateSound(stream);
            }
        }
    }
}