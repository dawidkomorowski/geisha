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
            // Act
            // Assert
            Assert.That(() => _assetStore.GetAsset<object>(Guid.NewGuid()), Throws.TypeOf<GeishaEngineException>());
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturnAsset_WhenAssetWasNotYetLoaded()
        {
            // Arrange
            var assetId = Guid.NewGuid();
            var asset = new object();

            var assetLoader = Substitute.For<IAssetLoader>();
            assetLoader.Load("some file path").Returns(asset);
            _assetLoaderProvider.GetLoaderFor(typeof(object)).Returns(assetLoader);

            _assetStore.RegisterAsset(new AssetInfo(typeof(object), assetId, "some file path"));

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
            var assetId = Guid.NewGuid();
            var asset = new object();

            var assetLoader = Substitute.For<IAssetLoader>();
            assetLoader.Load("some file path").Returns(asset);
            _assetLoaderProvider.GetLoaderFor(typeof(object)).Returns(assetLoader);

            _assetStore.RegisterAsset(new AssetInfo(typeof(object), assetId, "some file path"));
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
            var assetId = Guid.NewGuid();
            var asset = new object();

            var assetLoader = Substitute.For<IAssetLoader>();
            assetLoader.Load("some file path").Returns(asset);
            _assetLoaderProvider.GetLoaderFor(typeof(object)).Returns(assetLoader);

            _assetStore.RegisterAsset(new AssetInfo(typeof(object), assetId, "some file path"));
            _assetStore.GetAsset<object>(assetId);

            // Act
            var actual = _assetStore.GetAssetId(asset);

            // Assert
            Assert.That(actual, Is.EqualTo(assetId));
        }

        #endregion
    }
}