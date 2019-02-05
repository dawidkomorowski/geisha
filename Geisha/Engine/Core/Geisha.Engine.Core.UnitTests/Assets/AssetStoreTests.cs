using System;
using Geisha.Engine.Core.Assets;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Assets
{
    [TestFixture]
    public class AssetStoreTests
    {
        private IAssetLoaderProvider _assetLoaderProvider;
        private AssetStore _assetStore;

        [SetUp]
        public void SetUp()
        {
            _assetLoaderProvider = Substitute.For<IAssetLoaderProvider>();
            _assetStore = new AssetStore(_assetLoaderProvider);
        }

        #region GetAsset

        [Test]
        public void GetAsset_ShouldThrowException_WhenAssetWasNotRegistered()
        {
            // Arrange
            var notRegisteredAssetId = new AssetId(Guid.NewGuid());

            // Act
            // Assert
            Assert.That(() => _assetStore.GetAsset<object>(notRegisteredAssetId), Throws.TypeOf<GeishaEngineException>());
        }

        [Test]
        public void GetAsset_ShouldThrowException_WhenThereIsAssetIdMismatch()
        {
            // Arrange
            var assetId = new AssetId(Guid.NewGuid());
            var notRegisteredAssetId = new AssetId(Guid.NewGuid());

            _assetStore.RegisterAsset(new AssetInfo(assetId, typeof(object), "some file path"));

            // Act
            // Assert
            Assert.That(() => _assetStore.GetAsset<object>(notRegisteredAssetId), Throws.TypeOf<GeishaEngineException>());
        }

        [Test]
        public void GetAsset_ShouldThrowException_WhenThereIsAssetTypeMismatch()
        {
            // Arrange
            var assetId = new AssetId(Guid.NewGuid());
            _assetStore.RegisterAsset(new AssetInfo(assetId, typeof(object), "some file path"));

            // Act
            // Assert
            Assert.That(() => _assetStore.GetAsset<int>(assetId), Throws.TypeOf<GeishaEngineException>());
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturnAsset_WhenAssetWasNotYetLoaded()
        {
            // Arrange
            var assetId = new AssetId(Guid.NewGuid());
            var asset = new object();

            var assetLoader = Substitute.For<IAssetLoader>();
            assetLoader.Load("some file path").Returns(asset);
            _assetLoaderProvider.GetLoaderFor(typeof(object)).Returns(assetLoader);

            _assetStore.RegisterAsset(new AssetInfo(assetId, typeof(object), "some file path"));

            // Act
            var actual = _assetStore.GetAsset<object>(assetId);

            // Assert
            Assert.That(actual, Is.EqualTo(asset));
            _assetLoaderProvider.Received(1).GetLoaderFor(typeof(object));
            assetLoader.Received(1).Load("some file path");
        }

        [Test]
        public void GetAsset_ShouldNotLoadAssetAndReturnAssetFromCache_WhenAssetWasAlreadyLoaded()
        {
            // Arrange
            var assetId = new AssetId(Guid.NewGuid());
            var asset = new object();

            var assetLoader = Substitute.For<IAssetLoader>();
            assetLoader.Load("some file path").Returns(asset);
            _assetLoaderProvider.GetLoaderFor(typeof(object)).Returns(assetLoader);

            _assetStore.RegisterAsset(new AssetInfo(assetId, typeof(object), "some file path"));
            _assetStore.GetAsset<object>(assetId);

            _assetLoaderProvider.ClearReceivedCalls();
            assetLoader.ClearReceivedCalls();

            // Act
            var actual = _assetStore.GetAsset<object>(assetId);

            // Assert
            Assert.That(actual, Is.EqualTo(asset));
            _assetLoaderProvider.DidNotReceive().GetLoaderFor(typeof(object));
            assetLoader.DidNotReceive().Load("some file path");
        }

        #endregion

        #region GetAssetId

        [Test]
        public void GetAssetId_ShouldThrowException_GivenAssetNotLoadedByAssetStore()
        {
            // Arrange
            var asset = new object();

            // Act
            // Assert
            Assert.That(() => _assetStore.GetAssetId(asset), Throws.ArgumentException);
        }

        [Test]
        public void GetAssetId_ShouldReturnAssetId_GivenAssetLoadedByAssetStore()
        {
            // Arrange
            var assetId = new AssetId(Guid.NewGuid());
            var asset = new object();

            var assetLoader = Substitute.For<IAssetLoader>();
            assetLoader.Load("some file path").Returns(asset);
            _assetLoaderProvider.GetLoaderFor(typeof(object)).Returns(assetLoader);

            _assetStore.RegisterAsset(new AssetInfo(assetId, typeof(object), "some file path"));
            _assetStore.GetAsset<object>(assetId);

            // Act
            var actual = _assetStore.GetAssetId(asset);

            // Assert
            Assert.That(actual, Is.EqualTo(assetId));
        }

        #endregion

        #region RegisterAsset

        [TestCase(typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path", true)]
        [TestCase(typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "other file path", true)]
        [TestCase(typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            typeof(int), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path", false)]
        [TestCase(typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            typeof(object), "C7CF6FFC-FF65-48D8-BF1B-041E51F8E1C4", "some file path", false)]
        public void RegisterAsset_ShouldOverrideAssetInfo_WhenAssetInfoWasAlreadyRegistered(Type assetType1, string assetIdString1, string assetFilePath1,
            Type assetType2, string assetIdString2, string assetFilePath2, bool overridden)
        {
            // Arrange
            var asset = new object();
            var assetId1 = new AssetId(new Guid(assetIdString1));
            var assetId2 = new AssetId(new Guid(assetIdString2));

            var assetLoader = Substitute.For<IAssetLoader>();
            assetLoader.Load(assetFilePath1).Returns(asset);
            assetLoader.Load(assetFilePath2).Returns(asset);
            _assetLoaderProvider.GetLoaderFor(typeof(object)).Returns(assetLoader);

            _assetStore.RegisterAsset(new AssetInfo(assetId1, assetType1, assetFilePath1));

            // Act
            _assetStore.RegisterAsset(new AssetInfo(assetId2, assetType2, assetFilePath2));
            _assetStore.GetAsset<object>(assetId1);

            // Assert
            assetLoader.Received(1).Load(overridden ? assetFilePath2 : assetFilePath1);
        }

        #endregion
    }
}