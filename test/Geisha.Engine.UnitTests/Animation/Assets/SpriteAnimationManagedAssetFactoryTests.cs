using System.Linq;
using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Assets;
using Geisha.Engine.Core.Assets;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Animation.Assets
{
    [TestFixture]
    public class SpriteAnimationManagedAssetFactoryTests
    {
        private IFileSystem _fileSystem = null!;
        private IJsonSerializer _jsonSerializer = null!;
        private IAssetStore _assetStore = null!;
        private SpriteAnimationManagedAssetFactory _spriteAnimationManagedAssetFactory = null!;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _jsonSerializer = Substitute.For<IJsonSerializer>();
            _assetStore = Substitute.For<IAssetStore>();
            _spriteAnimationManagedAssetFactory = new SpriteAnimationManagedAssetFactory(_fileSystem, _jsonSerializer);
        }

        [Test]
        public void Create_ShouldReturnEmpty_GivenAssetInfoWithNotMatchingAssetType()
        {
            // Arrange
            var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(object), "asset file path");

            // Act
            var actual = _spriteAnimationManagedAssetFactory.Create(assetInfo, _assetStore);

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Create_ShouldReturnSingleAsset_GivenAssetInfoWithMatchingAssetType()
        {
            // Arrange
            var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(SpriteAnimation), "asset file path");

            // Act
            var actual = _spriteAnimationManagedAssetFactory.Create(assetInfo, _assetStore);

            // Assert
            Assert.That(actual, Is.Not.Empty);
            var managedAsset = actual.Single();
            Assert.That(managedAsset, Is.TypeOf<SpriteAnimationManagedAsset>());
            Assert.That(managedAsset.AssetInfo, Is.EqualTo(assetInfo));
        }
    }
}