using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.StartUpTasks;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.StartUpTasks
{
    [TestFixture]
    public class LoadStartUpSceneStartUpTaskTests
    {
        [Test]
        public void Run_ShouldLoadSceneFromStartUpScenePathInConfiguration()
        {
            // Arrange
            const string startUpScene = "start up scene";

            var sceneManager = Substitute.For<ISceneManager>();
            var coreConfiguration = CoreConfiguration.CreateBuilder().WithStartUpScene(startUpScene).Build();

            var startUpTask = new LoadStartUpSceneStartUpTask(sceneManager, coreConfiguration);

            // Act
            startUpTask.Run();

            // Assert
            sceneManager.Received().LoadScene(startUpScene);
        }
    }
}