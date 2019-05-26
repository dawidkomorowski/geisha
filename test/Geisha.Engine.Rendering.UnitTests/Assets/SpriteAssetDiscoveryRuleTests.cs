using System;
using System.Linq;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;
using Geisha.Framework.FileSystem;
using Geisha.Framework.Rendering;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Rendering.UnitTests.Assets
{
    [TestFixture]
    public class SpriteAssetDiscoveryRuleTests
    {
        private IJsonSerializer _jsonSerializer;
        private SpriteAssetDiscoveryRule _spriteAssetDiscoveryRule;

        [SetUp]
        public void SetUp()
        {
            _jsonSerializer = Substitute.For<IJsonSerializer>();
            _spriteAssetDiscoveryRule = new SpriteAssetDiscoveryRule(_jsonSerializer);
        }

        [Test]
        public void Discover_ShouldReturnEmpty_GivenFileWithNotMatchingExtension()
        {
            // Arrange
            var file = Substitute.For<IFile>();
            file.Extension.Returns(".some_file_type");

            // Act
            var actual = _spriteAssetDiscoveryRule.Discover(file);

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
            file.Extension.Returns(".sprite");
            file.ReadAllText().Returns(json);
            _jsonSerializer.Deserialize<SpriteFileContent>(json).Returns(new SpriteFileContent
            {
                AssetId = assetId.Value,
                TextureAssetId = Guid.NewGuid()
            });

            // Act
            var actual = _spriteAssetDiscoveryRule.Discover(file);

            // Assert
            Assert.That(actual, Is.Not.Empty);
            var assetInfo = actual.Single();
            Assert.That(assetInfo.AssetId, Is.EqualTo(assetId));
            Assert.That(assetInfo.AssetType, Is.EqualTo(typeof(Sprite)));
            Assert.That(assetInfo.AssetFilePath, Is.EqualTo(filePath));
        }
    }
}