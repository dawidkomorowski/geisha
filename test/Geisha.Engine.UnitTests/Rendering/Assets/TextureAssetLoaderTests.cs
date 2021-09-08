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
    public class TextureAssetLoaderTests
    {
        private IRenderer2D _renderer2D = null!;
        private IFileSystem _fileSystem = null!;
        private IAssetStore _assetStore = null!;
        private TextureAssetLoader _textureAssetLoader = null!;

        private AssetInfo _assetInfo;
        private ITexture _texture = null!;

        [SetUp]
        public void SetUp()
        {
            _renderer2D = Substitute.For<IRenderer2D>();
            _fileSystem = Substitute.For<IFileSystem>();
            _assetStore = Substitute.For<IAssetStore>();

            const string assetFilePath = @"some_directory\texture_file_path";
            const string textureFilePath = @"some_directory\actual_texture_file_path";

            var textureAssetContent = new TextureAssetContent
            {
                TextureFilePath = "actual_texture_file_path"
            };

            _assetInfo = new AssetInfo(AssetId.CreateUnique(), RenderingAssetTypes.Texture, assetFilePath);
            var assetData = AssetData.CreateWithJsonContent(_assetInfo.AssetId, _assetInfo.AssetType, textureAssetContent);
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

            _textureAssetLoader = new TextureAssetLoader(_renderer2D, _fileSystem);
        }

        [Test]
        public void LoadAsset_ShouldLoadTextureFromFile()
        {
            // Arrange
            // Act
            var actual = (ITexture)_textureAssetLoader.LoadAsset(_assetInfo, _assetStore);

            // Assert
            Assert.That(actual, Is.EqualTo(_texture));
        }

        [Test]
        public void UnloadAsset_ShouldDisposeTexture()
        {
            // Arrange
            // Act
            _textureAssetLoader.UnloadAsset(_texture);

            // Assert
            _texture.Received().Dispose();
        }
    }
}