using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Configuration;
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
            var assetStore = Substitute.For<IAssetStore>();
            var configurationManager = Substitute.For<IConfigurationManager>();

            const string assetsPath = "Assets path";
            configurationManager.GetConfiguration<CoreConfiguration>().Returns(new CoreConfiguration {AssetsRootDirectoryPath = assetsPath});

            var startUpTask = new RegisterAssetsAutomaticallyStartUpTask(assetStore, configurationManager);

            // Act
            startUpTask.Run();

            // Assert
            assetStore.Received().RegisterAssets(assetsPath);
        }
    }
}