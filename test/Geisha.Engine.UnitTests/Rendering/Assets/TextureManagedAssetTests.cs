using System.IO;
using System.Text.Json;
using Geisha.Common.FileSystem;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using Geisha.Engine.Rendering.Backend;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Assets
{
    [TestFixture]
    public class TextureManagedAssetTests
    {
        private IRenderer2D _renderer2D = null!;
        private IFileSystem _fileSystem = null!;
        private TextureManagedAsset _textureManagedAsset = null!;

        private ITexture _texture = null!;

        [SetUp]
        public void SetUp()
        {
            _renderer2D = Substitute.For<IRenderer2D>();
            _fileSystem = Substitute.For<IFileSystem>();

            const string textureFilePath = @"some_directory\texture_file_path";
            const string actualTextureFilePath = @"some_directory\actual_texture_file_path";

            var textureFileJson = JsonSerializer.Serialize(new TextureFileContent
            {
                TextureFilePath = "actual_texture_file_path"
            });

            var stream = Substitute.For<Stream>();
            _texture = Substitute.For<ITexture>();

            var textureFile = Substitute.For<IFile>();
            textureFile.ReadAllText().Returns(textureFileJson);
            var actualTextureFile = Substitute.For<IFile>();
            actualTextureFile.OpenRead().Returns(stream);
            _fileSystem.GetFile(textureFilePath).Returns(textureFile);
            _fileSystem.GetFile(actualTextureFilePath).Returns(actualTextureFile);
            _renderer2D.CreateTexture(stream).Returns(_texture);

            Assert.Fail("TODO");
            //var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(ITexture), textureFilePath);
            //_textureManagedAsset = new TextureManagedAsset(assetInfo, _renderer2D, _fileSystem);
        }

        [Test]
        public void Load_ShouldLoadTextureFromFile()
        {
            // Arrange
            // Act
            _textureManagedAsset.Load();
            var actual = (ITexture?) _textureManagedAsset.AssetInstance;

            // Assert
            Assert.That(actual, Is.EqualTo(_texture));
        }

        [Test]
        public void Unload_ShouldDisposeTexture()
        {
            // Arrange
            _textureManagedAsset.Load();

            // Act
            _textureManagedAsset.Unload();

            // Assert
            _texture.Received().Dispose();
        }
    }
}