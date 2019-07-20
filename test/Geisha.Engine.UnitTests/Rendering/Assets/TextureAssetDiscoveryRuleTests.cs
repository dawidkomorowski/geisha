using System.Linq;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using Geisha.Framework.FileSystem;
using Geisha.Framework.Rendering;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Assets
{
    [TestFixture]
    public class TextureAssetDiscoveryRuleTests
    {
        private IJsonSerializer _jsonSerializer;
        private TextureAssetDiscoveryRule _textureAssetDiscoveryRule;

        [SetUp]
        public void SetUp()
        {
            _jsonSerializer = Substitute.For<IJsonSerializer>();
            _textureAssetDiscoveryRule = new TextureAssetDiscoveryRule(_jsonSerializer);
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
            const string json = "file content";
            var file = Substitute.For<IFile>();
            file.Path.Returns(filePath);
            file.Extension.Returns(".texture");
            file.ReadAllText().Returns(json);
            _jsonSerializer.Deserialize<TextureFileContent>(json).Returns(new TextureFileContent
            {
                AssetId = assetId.Value,
                TextureFilePath = string.Empty
            });

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