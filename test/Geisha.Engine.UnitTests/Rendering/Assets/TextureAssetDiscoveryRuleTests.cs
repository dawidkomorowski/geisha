using System.Linq;
using System.Text.Json;
using Geisha.Common.FileSystem;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Assets
{
    [TestFixture]
    public class TextureAssetDiscoveryRuleTests
    {
        private TextureAssetDiscoveryRule _textureAssetDiscoveryRule = null!;

        [SetUp]
        public void SetUp()
        {
            _textureAssetDiscoveryRule = new TextureAssetDiscoveryRule();
        }

        [Test]
        public void Discover_ShouldReturnEmpty_GivenFileWithNotMatchingExtension()
        {
            // Arrange
            var file = Substitute.For<IFile>();
            file.Extension.Returns(".some_file_type");

            // Act
            var actual = _textureAssetDiscoveryRule.Discover(file);

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Discover_ShouldReturnSingleAssetInfo_GivenFileWithMatchingExtension()
        {
            var assetId = AssetId.CreateUnique();
            const string filePath = "file path";

            var json = JsonSerializer.Serialize(new TextureFileContent
            {
                AssetId = assetId.Value,
                TextureFilePath = string.Empty
            });

            var file = Substitute.For<IFile>();
            file.Path.Returns(filePath);
            file.Extension.Returns(RenderingFileExtensions.Texture);
            file.ReadAllText().Returns(json);

            // Act
            var actual = _textureAssetDiscoveryRule.Discover(file);

            // Assert
            Assert.That(actual, Is.Not.Empty);
            var assetInfo = actual.Single();
            Assert.That(assetInfo.AssetId, Is.EqualTo(assetId));
            Assert.That(assetInfo.AssetType, Is.EqualTo(typeof(ITexture)));
            Assert.That(assetInfo.AssetFilePath, Is.EqualTo(filePath));
        }
    }
}