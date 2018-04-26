using System.IO;
using Geisha.Common.Math.Definition;
using Geisha.Common.Serialization;
using Geisha.Engine.Rendering.Assets;
using Geisha.Framework.FileSystem;
using Geisha.Framework.Rendering;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Rendering.UnitTests.Assets
{
    [TestFixture]
    public class SpriteLoaderTests
    {
        private IFileSystem _fileSystem;
        private IRenderer2D _renderer;
        private SpriteLoader _spriteLoader;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _renderer = Substitute.For<IRenderer2D>();
            _spriteLoader = new SpriteLoader(_fileSystem, _renderer);
        }

        [Test]
        public void Load_ShouldReturnSpriteWithDataAsDefinedInSpriteFile()
        {
            // Arrange
            const string filePath = "sprite file path";

            var spriteFile = new SpriteFile
            {
                SourceTextureFilePath = "source texture file path",
                SourceUV = new Vector2Definition {X = 123.456, Y = 234.567},
                SourceDimension = new Vector2Definition {X = 345.456, Y = 456.567},
                SourceAnchor = new Vector2Definition {X = 567.678, Y = 678.789},
                PixelsPerUnit = 123.456
            };

            var stream = Substitute.For<Stream>();
            var texture = Substitute.For<ITexture>();

            _fileSystem.ReadAllTextFromFile(filePath).Returns(Serializer.SerializeJson(spriteFile));
            _fileSystem.OpenFileStreamForReading("source texture file path").Returns(stream);
            _renderer.CreateTexture(stream).Returns(texture);

            // Act
            var actual = (Sprite) _spriteLoader.Load(filePath);

            // Assert
            Assert.That(actual.SourceTexture, Is.EqualTo(texture));
            Assert.That(actual.SourceUV.X, Is.EqualTo(spriteFile.SourceUV.X));
            Assert.That(actual.SourceUV.Y, Is.EqualTo(spriteFile.SourceUV.Y));
            Assert.That(actual.SourceDimension.X, Is.EqualTo(spriteFile.SourceDimension.X));
            Assert.That(actual.SourceDimension.Y, Is.EqualTo(spriteFile.SourceDimension.Y));
            Assert.That(actual.SourceAnchor.X, Is.EqualTo(spriteFile.SourceAnchor.X));
            Assert.That(actual.SourceAnchor.Y, Is.EqualTo(spriteFile.SourceAnchor.Y));
            Assert.That(actual.PixelsPerUnit, Is.EqualTo(spriteFile.PixelsPerUnit));
        }
    }
}