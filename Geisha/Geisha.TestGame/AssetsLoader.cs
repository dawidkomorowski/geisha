using System.ComponentModel.Composition;
using System.IO;
using Geisha.Common.Geometry;
using Geisha.Framework.Rendering;

namespace Geisha.TestGame
{
    public interface IAssetsLoader
    {
        Sprite CreateDotSprite();
        Sprite CreateBoxSprite();
        Sprite CreateCompassSprite();
    }

    [Export(typeof(IAssetsLoader))]
    public class AssetsesLoader : IAssetsLoader
    {
        private const string ResourcesRootPath = @"Assets\";
        private readonly IRenderer2D _renderer2D;

        [ImportingConstructor]
        public AssetsesLoader(IRenderer2D renderer2D)
        {
            _renderer2D = renderer2D;
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

        private ITexture LoadTexture(string filePath)
        {
            filePath = Path.Combine(ResourcesRootPath, filePath);
            using (Stream stream = new FileStream(filePath, FileMode.Open))
            {
                return _renderer2D.CreateTexture(stream);
            }
        }
    }
}