using Geisha.Engine.Core;
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
        public void Run_ShouldLoadSceneFromStartUpScenePathInConfiguration_WhenStartUpSceneIsNonEmpty()
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

        [Test]
        public void Run_ShouldLoadEmptySceneWithSceneBehaviorFromStartUpSceneBehaviorInConfiguration_WhenStartUpSceneIsEmpty()
        {
            // Arrange
            const string startUpSceneBehavior = "start up scene behavior";

            var sceneManager = Substitute.For<ISceneManager>();
            var coreConfiguration = CoreConfiguration.CreateBuilder().WithStartUpSceneBehavior(startUpSceneBehavior).Build();

            var startUpTask = new LoadStartUpSceneStartUpTask(sceneManager, coreConfiguration);

            // Act
            startUpTask.Run();

            // Assert
            sceneManager.Received().LoadEmptyScene(startUpSceneBehavior);
        }
    }
}