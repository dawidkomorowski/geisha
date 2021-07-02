using System.Linq;
using System.Text.Json;
using Geisha.Common.FileSystem;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Audio.Assets
{
    [TestFixture]
    public class SoundAssetDiscoveryRuleTests
    {
        private SoundAssetDiscoveryRule _soundAssetDiscoveryRule = null!;

        [SetUp]
        public void SetUp()
        {
            _soundAssetDiscoveryRule = new SoundAssetDiscoveryRule();
        }

        [Test]
        public void Discover_ShouldReturnEmpty_GivenFileWithNotMatchingExtension()
        {
            // Arrange
            var file = Substitute.For<IFile>();
            file.Extension.Returns(".some_file_type");

            // Act
            var actual = _soundAssetDiscoveryRule.Discover(file);

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Discover_ShouldReturnSingleAssetInfo_GivenFileWithMatchingExtension()
        {
            var assetId = AssetId.CreateUnique();
            const string filePath = "file path";

            var json = JsonSerializer.Serialize(new SoundFileContent
            {
                AssetId = assetId.Value,
                SoundFilePath = string.Empty
            });

            var file = Substitute.For<IFile>();
            file.Path.Returns(filePath);
            file.Extension.Returns(AudioFileExtensions.Sound);
            file.ReadAllText().Returns(json);

            // Act
            var actual = _soundAssetDiscoveryRule.Discover(file);

            // Assert
            Assert.That(actual, Is.Not.Empty);
            var assetInfo = actual.Single();
            Assert.That(assetInfo.AssetId, Is.EqualTo(assetId));
            Assert.That(assetInfo.AssetType, Is.EqualTo(typeof(ISound)));
            Assert.That(assetInfo.AssetFilePath, Is.EqualTo(filePath));
        }
    }
}