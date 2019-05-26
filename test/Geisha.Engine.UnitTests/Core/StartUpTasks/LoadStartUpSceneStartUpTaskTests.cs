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
            var configurationManager = Substitute.For<IConfigurationManager>();
            configurationManager.GetConfiguration<CoreConfiguration>().Returns(new CoreConfiguration
            {
                StartUpScene = startUpScene
            });

            var startUpTask = new LoadStartUpSceneStartUpTask(sceneManager, configurationManager);

            // Act
            startUpTask.Run();

            // Assert
            sceneManager.Received().LoadScene(startUpScene);
        }
    }
}