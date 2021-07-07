using System.Linq;
using Geisha.Common.FileSystem;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core.Assets;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Audio.Assets
{
    [TestFixture]
    public class SoundManagedAssetFactoryTests
    {
        private IAssetStore _assetStore = null!;
        private IAudioBackend _audioBackend = null!;
        private IFileSystem _fileSystem = null!;
        private SoundManagedAssetFactory _spriteManagedAssetFactory = null!;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _audioBackend = Substitute.For<IAudioBackend>();
            _fileSystem = Substitute.For<IFileSystem>();
            _spriteManagedAssetFactory = new SoundManagedAssetFactory(_audioBackend, _fileSystem);
        }

        [Test]
        public void Create_ShouldReturnEmpty_GivenAssetInfoWithNotMatchingAssetType()
        {
            Assert.Fail("TODO");
            //// Arrange
            //var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(object), "asset file path");

            //// Act
            //var actual = _spriteManagedAssetFactory.Create(assetInfo, _assetStore);

            //// Assert
            //Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Create_ShouldReturnSingleAsset_GivenAssetInfoWithMatchingAssetType()
        {
            Assert.Fail("TODO");
            //// Arrange
            //var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(ISound), "asset file path");

            //// Act
            //var actual = _spriteManagedAssetFactory.Create(assetInfo, _assetStore);

            //// Assert
            //Assert.That(actual, Is.Not.Empty);
            //var managedAsset = actual.Single();
            //Assert.That(managedAsset, Is.TypeOf<SoundManagedAsset>());
            //Assert.That(managedAsset.AssetInfo, Is.EqualTo(assetInfo));
        }
    }
}