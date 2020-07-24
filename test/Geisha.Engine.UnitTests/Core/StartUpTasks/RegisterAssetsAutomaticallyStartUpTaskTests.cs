using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.StartUpTasks;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.StartUpTasks
{
    [TestFixture]
    public class RegisterAssetsAutomaticallyStartUpTaskTests
    {
        [Test]
        public void Run_ShouldRegisterAllDiscoveredAssetsInAssetStore()
        {
            // Arrange
            var assetStore = Substitute.For<IAssetStore>();

            const string assetsPath = "Assets path";
            var coreConfiguration = CoreConfiguration.CreateBuilder().WithAssetsRootDirectoryPath(assetsPath).Build();

            var startUpTask = new RegisterAssetsAutomaticallyStartUpTask(assetStore, coreConfiguration);

            // Act
            startUpTask.Run();

            // Assert
            assetStore.Received().RegisterAssets(assetsPath);
        }
    }
}