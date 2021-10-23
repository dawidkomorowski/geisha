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
        private IAssetStore _assetStore = null!;
        private TextureAssetLoader _textureAssetLoader = null!;

        private AssetInfo _assetInfo;
        private ITexture _texture = null!;

        [SetUp]
        public void SetUp()
        {
            var renderingBackend = Substitute.For<IRenderingBackend>();
            var renderer2D = Substitute.For<IRenderer2D>();
            var fileSystem = Substitute.For<IFileSystem>();
            _assetStore = Substitute.For<IAssetStore>();

            renderingBackend.Renderer2D.Returns(renderer2D);

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
            fileSystem.GetFile(assetFilePath).Returns(assetFile);

            var stream = Substitute.For<Stream>();
            var textureFile = Substitute.For<IFile>();
            textureFile.OpenRead().Returns(stream);
            fileSystem.GetFile(textureFilePath).Returns(textureFile);

            _texture = Substitute.For<ITexture>();
            renderer2D.CreateTexture(stream).Returns(_texture);

            _textureAssetLoader = new TextureAssetLoader(renderingBackend, fileSystem);
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