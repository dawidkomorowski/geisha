using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Geisha.Common.FileSystem;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Assets;
using Geisha.Engine.Input.Assets.Serialization;
using Geisha.Engine.Input.Mapping;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input.Assets
{
    [TestFixture]
    public class InputMappingAssetDiscoveryRuleTests
    {
        private InputMappingAssetDiscoveryRule _inputMappingAssetDiscoveryRule = null!;

        [SetUp]
        public void SetUp()
        {
            _inputMappingAssetDiscoveryRule = new InputMappingAssetDiscoveryRule();
        }

        [Test]
        public void Discover_ShouldReturnEmpty_GivenFileWithNotMatchingExtension()
        {
            // Arrange
            var file = Substitute.For<IFile>();
            file.Extension.Returns(".some_file_type");

            // Act
            var actual = _inputMappingAssetDiscoveryRule.Discover(file);

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Discover_ShouldReturnSingleAssetInfo_GivenFileWithMatchingExtension()
        {
            var assetId = AssetId.CreateUnique();
            const string filePath = "file path";

            var json = JsonSerializer.Serialize(new InputMappingFileContent
            {
                AssetId = assetId.Value,
                ActionMappings = new Dictionary<string, SerializableHardwareAction[]>(),
                AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>()
            });

            var file = Substitute.For<IFile>();
            file.Path.Returns(filePath);
            file.Extension.Returns(".input");
            file.ReadAllText().Returns(json);

            // Act
            var actual = _inputMappingAssetDiscoveryRule.Discover(file);

            // Assert
            Assert.That(actual, Is.Not.Empty);
            var assetInfo = actual.Single();
            Assert.That(assetInfo.AssetId, Is.EqualTo(assetId));
            Assert.That(assetInfo.AssetType, Is.EqualTo(typeof(InputMapping)));
            Assert.That(assetInfo.AssetFilePath, Is.EqualTo(filePath));
        }
    }
}