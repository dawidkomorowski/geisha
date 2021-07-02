using System;
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
    public class SpriteAssetDiscoveryRuleTests
    {
        private SpriteAssetDiscoveryRule _spriteAssetDiscoveryRule = null!;

        [SetUp]
        public void SetUp()
        {
            _spriteAssetDiscoveryRule = new SpriteAssetDiscoveryRule();
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

            var json = JsonSerializer.Serialize(new SpriteFileContent
            {
                AssetId = assetId.Value,
                TextureAssetId = Guid.NewGuid()
            });

            var file = Substitute.For<IFile>();
            file.Path.Returns(filePath);
            file.Extension.Returns(RenderingFileExtensions.Sprite);
            file.ReadAllText().Returns(json);

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