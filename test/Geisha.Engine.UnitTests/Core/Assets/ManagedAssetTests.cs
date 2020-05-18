using System;
using Geisha.Engine.Core.Assets;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets
{
    [TestFixture]
    public class ManagedAssetTests
    {
        #region Constructor

        [Test]
        public void Constructor_ShouldCreateNewInstanceWithAssetInfoAsSpecifiedInParameters()
        {
            // Arrange
            var assetInfo = CreateNewAssetInfo();

            // Act
            var testManagedAsset = new TestManagedAsset(assetInfo);

            // Assert
            Assert.That(testManagedAsset.AssetInfo, Is.EqualTo(assetInfo));
        }

        [Test]
        public void Constructor_ShouldCreateNewInstanceInUnloadedState()
        {
            // Arrange
            var assetInfo = CreateNewAssetInfo();

            // Act
            var testManagedAsset = new TestManagedAsset(assetInfo);

            // Assert
            Assert.That(testManagedAsset.IsLoaded, Is.False);
        }

        [Test]
        public void Constructor_ShouldCreateNewInstanceWithNullAssetInstance()
        {
            // Arrange
            var assetInfo = CreateNewAssetInfo();

            // Act
            var testManagedAsset = new TestManagedAsset(assetInfo);

            // Assert
            Assert.That(testManagedAsset.AssetInstance, Is.Null);
        }

        #endregion

        #region Load

        [Test]
        public void Load_ShouldChangeAssetStateToLoaded()
        {
            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            var testManagedAsset = new TestManagedAsset(assetInfo) {AssetToReturnByLoadAsset = asset};

            // Act
            testManagedAsset.Load();

            // Assert
            Assert.That(testManagedAsset.IsLoaded, Is.True);
        }

        [Test]
        public void Load_ShouldThrowException_WhenAssetIsAlreadyLoaded()
        {
            // Arrange
            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            var testManagedAsset = new TestManagedAsset(assetInfo) {AssetToReturnByLoadAsset = asset};

            testManagedAsset.Load();

            // Act
            // Assert
            Assert.That(() => { testManagedAsset.Load(); }, Throws.TypeOf<AssetAlreadyLoadedException>());
        }

        [Test]
        public void Load_ShouldCallLoadAssetAndSetAssetInstanceWithReturnedValue()
        {
            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            var testManagedAsset = new TestManagedAsset(assetInfo) {AssetToReturnByLoadAsset = asset};

            // Act
            testManagedAsset.Load();

            // Assert
            Assert.That(testManagedAsset.AssetInstance, Is.EqualTo(asset));
        }

        [Test]
        public void Load_ShouldThrowException_WhenLoadAssetReturnsNull()
        {
            // Arrange
            var assetInfo = CreateNewAssetInfo();
            var testManagedAsset = new TestManagedAsset(assetInfo) {AssetToReturnByLoadAsset = null!};

            // Act
            // Assert
            Assert.That(() => { testManagedAsset.Load(); }, Throws.TypeOf<LoadAssetReturnedNullException>());
        }

        #endregion

        #region Unload

        [Test]
        public void Unload_ShouldChangeAssetStateToUnloaded()
        {
            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            var testManagedAsset = new TestManagedAsset(assetInfo) {AssetToReturnByLoadAsset = asset};

            testManagedAsset.Load();

            // Assume
            Assume.That(testManagedAsset.IsLoaded, Is.True);

            // Act
            testManagedAsset.Unload();

            // Assert
            Assert.That(testManagedAsset.IsLoaded, Is.False);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Unload_ShouldThrowException_WhenAssetIsAlreadyUnloaded(bool loadedInitially)
        {
            // Arrange
            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            var testManagedAsset = new TestManagedAsset(assetInfo) {AssetToReturnByLoadAsset = asset};

            if (loadedInitially)
            {
                testManagedAsset.Load();
                testManagedAsset.Unload();
            }

            // Act
            // Assert
            Assert.That(() => { testManagedAsset.Unload(); }, Throws.TypeOf<AssetAlreadyUnloadedException>());
        }

        [Test]
        public void Unload_ShouldSetAssetInstanceToNull()
        {
            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            var testManagedAsset = new TestManagedAsset(assetInfo) {AssetToReturnByLoadAsset = asset};

            testManagedAsset.Load();

            // Assume
            Assume.That(testManagedAsset.IsLoaded, Is.True);

            // Act
            testManagedAsset.Unload();

            // Assert
            Assert.That(testManagedAsset.AssetInstance, Is.Null);
        }

        [Test]
        public void Unload_ShouldCallUnloadAssetWithAssetInstanceAsParameter()
        {
            var assetInfo = CreateNewAssetInfo();
            var asset = new object();
            var testManagedAsset = new TestManagedAsset(assetInfo) {AssetToReturnByLoadAsset = asset};

            testManagedAsset.Load();

            // Assume
            Assume.That(testManagedAsset.IsLoaded, Is.True);

            // Act
            testManagedAsset.Unload();

            // Assert
            Assert.That(testManagedAsset.AssetReceivedByUnloadAsset, Is.EqualTo(asset));
        }

        #endregion

        private static AssetInfo CreateNewAssetInfo()
        {
            return new AssetInfo(AssetId.CreateUnique(), typeof(object), Guid.NewGuid().ToString());
        }

        private sealed class TestManagedAsset : ManagedAsset<object>
        {
            public object AssetToReturnByLoadAsset { get; set; } = new object();
            public object AssetReceivedByUnloadAsset { get; set; } = new object();

            public TestManagedAsset(AssetInfo assetInfo) : base(assetInfo)
            {
            }

            protected override object LoadAsset()
            {
                return AssetToReturnByLoadAsset;
            }

            protected override void UnloadAsset(object asset)
            {
                AssetReceivedByUnloadAsset = asset;
            }
        }
    }
}