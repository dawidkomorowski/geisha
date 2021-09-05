using System.IO;
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

            const string assetFilePath = @"some_directory\texture_file_path";
            const string textureFilePath = @"some_directory\actual_texture_file_path";

            var textureFileContent = new TextureFileContent
            {
                TextureFilePath = "actual_texture_file_path"
            };

            var assetInfo = new AssetInfo(AssetId.CreateUnique(), RenderingAssetTypes.Texture, assetFilePath);
            var assetData = AssetData.CreateWithJsonContent(assetInfo.AssetId, assetInfo.AssetType, textureFileContent);
            var memoryStream = new MemoryStream();
            assetData.Save(memoryStream);
            memoryStream.Position = 0;

            var assetFile = Substitute.For<IFile>();
            assetFile.OpenRead().Returns(memoryStream);
            _fileSystem.GetFile(assetFilePath).Returns(assetFile);

            var stream = Substitute.For<Stream>();
            var textureFile = Substitute.For<IFile>();
            textureFile.OpenRead().Returns(stream);
            _fileSystem.GetFile(textureFilePath).Returns(textureFile);

            _texture = Substitute.For<ITexture>();
            _renderer2D.CreateTexture(stream).Returns(_texture);

            _textureManagedAsset = new TextureManagedAsset(assetInfo, _renderer2D, _fileSystem);
        }

        [Test]
        public void Load_ShouldLoadTextureFromFile()
        {
            // Arrange
            // Act
            _textureManagedAsset.Load();
            var actual = (ITexture?)_textureManagedAsset.AssetInstance;

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