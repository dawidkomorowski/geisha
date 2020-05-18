using System.Linq;
using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Assets;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Assets
{
    [TestFixture]
    public class SpriteManagedAssetFactoryTests
    {
        private IAssetStore _assetStore = null!;
        private IFileSystem _fileSystem = null!;
        private IJsonSerializer _jsonSerializer = null!;
        private SpriteManagedAssetFactory _spriteManagedAssetFactory = null!;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _fileSystem = Substitute.For<IFileSystem>();
            _jsonSerializer = Substitute.For<IJsonSerializer>();
            _spriteManagedAssetFactory = new SpriteManagedAssetFactory(_fileSystem, _jsonSerializer);
        }

        [Test]
        public void Create_ShouldReturnEmpty_GivenAssetInfoWithNotMatchingAssetType()
        {
            // Arrange
            var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(object), "asset file path");

            // Act
            var actual = _spriteManagedAssetFactory.Create(assetInfo, _assetStore);

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Create_ShouldReturnSingleAsset_GivenAssetInfoWithMatchingAssetType()
        {
            // Arrange
            var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(Sprite), "asset file path");

            // Act
            var actual = _spriteManagedAssetFactory.Create(assetInfo, _assetStore);

            // Assert
            Assert.That(actual, Is.Not.Empty);
            var managedAsset = actual.Single();
            Assert.That(managedAsset, Is.TypeOf<SpriteManagedAsset>());
            Assert.That(managedAsset.AssetInfo, Is.EqualTo(assetInfo));
        }
    }
}