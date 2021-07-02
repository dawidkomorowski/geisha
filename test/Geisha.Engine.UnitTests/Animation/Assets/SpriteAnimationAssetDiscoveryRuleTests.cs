using System;
using System.Linq;
using System.Text.Json;
using Geisha.Common.FileSystem;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Assets;
using Geisha.Engine.Animation.Assets.Serialization;
using Geisha.Engine.Core.Assets;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Animation.Assets
{
    [TestFixture]
    public class SpriteAnimationAssetDiscoveryRuleTests
    {
        private SpriteAnimationAssetDiscoveryRule _spriteAnimationAssetDiscoveryRule = null!;

        [SetUp]
        public void SetUp()
        {
            _spriteAnimationAssetDiscoveryRule = new SpriteAnimationAssetDiscoveryRule();
        }

        [Test]
        public void Discover_ShouldReturnEmpty_GivenFileWithNotMatchingExtension()
        {
            // Arrange
            var file = Substitute.For<IFile>();
            file.Extension.Returns(".some_file_type");

            // Act
            var actual = _spriteAnimationAssetDiscoveryRule.Discover(file);

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Discover_ShouldReturnSingleAssetInfo_GivenFileWithMatchingExtension()
        {
            var assetId = AssetId.CreateUnique();
            const string filePath = "file path";

            var json = JsonSerializer.Serialize(new SpriteAnimationFileContent
            {
                AssetId = assetId.Value,
                Frames = new[]
                {
                    new SpriteAnimationFileContent.Frame
                    {
                        SpriteAssetId = Guid.NewGuid(),
                        Duration = 1
                    },
                    new SpriteAnimationFileContent.Frame
                    {
                        SpriteAssetId = Guid.NewGuid(),
                        Duration = 1.5
                    },
                    new SpriteAnimationFileContent.Frame
                    {
                        SpriteAssetId = Guid.NewGuid(),
                        Duration = 0.5
                    }
                },
                DurationTicks = TimeSpan.FromSeconds(2).Ticks
            });

            var file = Substitute.For<IFile>();
            file.Path.Returns(filePath);
            file.Extension.Returns(AnimationFileExtensions.SpriteAnimation);
            file.ReadAllText().Returns(json);

            // Act
            var actual = _spriteAnimationAssetDiscoveryRule.Discover(file);

            // Assert
            Assert.That(actual, Is.Not.Empty);
            var assetInfo = actual.Single();
            Assert.That(assetInfo.AssetId, Is.EqualTo(assetId));
            Assert.That(assetInfo.AssetType, Is.EqualTo(typeof(SpriteAnimation)));
            Assert.That(assetInfo.AssetFilePath, Is.EqualTo(filePath));
        }
    }
}