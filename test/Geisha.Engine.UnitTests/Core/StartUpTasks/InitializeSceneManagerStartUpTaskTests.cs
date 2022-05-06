using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.StartUpTasks;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.StartUpTasks
{
    [TestFixture]
    public class InitializeSceneManagerStartUpTaskTests
    {
        [Test]
        public void Run_ShouldInitializeSceneManager()
        {
            // Arrange
            var sceneManager = Substitute.For<ISceneManagerInit>();
            var sceneObservers = Enumerable.Empty<ISceneObserver>();

            // ReSharper disable once PossibleMultipleEnumeration
            var startUpTask = new InitializeSceneManagerStartUpTask(sceneManager, sceneObservers);

            // Act
            startUpTask.Run();

            // Assert
            // ReSharper disable once PossibleMultipleEnumeration
            sceneManager.Received(1).Initialize(sceneObservers);
        }
    }
}