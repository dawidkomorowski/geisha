using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.StartUpTasks;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.StartUpTasks
{
    [TestFixture]
    public class RegisterAssetsAutomaticallyStartUpTaskTests
    {
        [Test]
        public void Run_ShouldRegisterAllDiscoveredAssetsInAssetStore()
        {
            // Arrange
            var assetsDiscoveryEngine = Substitute.For<IAssetsDiscoveryEngine>();
            var assetStore = Substitute.For<IAssetStore>();

            var assetInfo1 = CreateNewAssetInfo();
            var assetInfo2 = CreateNewAssetInfo();
            var assetInfo3 = CreateNewAssetInfo();

            assetsDiscoveryEngine.DiscoverAssets().Returns(new[] {assetInfo1, assetInfo2, assetInfo3});

            var startUpTask = new RegisterAssetsAutomaticallyStartUpTask(assetsDiscoveryEngine, assetStore);

            // Act
            startUpTask.Run();

            // Assert
            assetStore.Received().RegisterAsset(assetInfo1);
            assetStore.Received().RegisterAsset(assetInfo2);
            assetStore.Received().RegisterAsset(assetInfo3);
        }

        private static AssetInfo CreateNewAssetInfo()
        {
            return new AssetInfo(new AssetId(Guid.NewGuid()), typeof(int), "asset.int");
        }
    }
}