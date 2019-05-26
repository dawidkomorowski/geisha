using Geisha.Engine.Core;
using Geisha.Engine.Core.StartUpTasks;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core
{
    [TestFixture]
    public class EngineTests
    {
        private IGameLoop _gameLoop;
        private IEngineManager _engineManager;
        private IRegisterDiagnosticInfoProvidersStartUpTask _registerDiagnosticInfoProvidersStartUpTask;
        private IRegisterAssetsAutomaticallyStarUpTask _registerAssetsAutomaticallyStarUpTask;
        private ILoadStartUpSceneStartUpTask _loadStartUpSceneStartUpTask;

        [SetUp]
        public void SetUp()
        {
            _gameLoop = Substitute.For<IGameLoop>();
            _engineManager = Substitute.For<IEngineManager>();
            _registerDiagnosticInfoProvidersStartUpTask = Substitute.For<IRegisterDiagnosticInfoProvidersStartUpTask>();
            _registerAssetsAutomaticallyStarUpTask = Substitute.For<IRegisterAssetsAutomaticallyStarUpTask>();
            _loadStartUpSceneStartUpTask = Substitute.For<ILoadStartUpSceneStartUpTask>();
        }

        private Engine.Core.Engine CreateEngine()
        {
            return new Engine.Core.Engine(
                _gameLoop,
                _engineManager,
                _registerDiagnosticInfoProvidersStartUpTask,
                _registerAssetsAutomaticallyStarUpTask,
                _loadStartUpSceneStartUpTask
            );
        }

        [Test]
        public void Constructor_ShouldRunStartUpTasks()
        {
            // Arrange
            // Act
            var actual = CreateEngine();

            // Assert
            _registerDiagnosticInfoProvidersStartUpTask.Received().Run();
            _registerAssetsAutomaticallyStarUpTask.Received().Run();
            _loadStartUpSceneStartUpTask.Received().Run();
        }


        [TestCase(false)]
        [TestCase(true)]
        public void IsScheduledForShutdown_ShouldReturnValueProvidedByEngineManager(bool isScheduledForShutdown)
        {
            // Arrange
            _engineManager.IsEngineScheduledForShutdown.Returns(isScheduledForShutdown);

            var engine = CreateEngine();

            // Act
            var actual = engine.IsScheduledForShutdown;

            // Assert
            Assert.That(actual, Is.EqualTo(isScheduledForShutdown));
        }

        [Test]
        public void Update_ShouldUpdateGameLoop()
        {
            // Arrange
            var engine = CreateEngine();

            // Act
            engine.Update();

            // Assert
            _gameLoop.Received(1).Update();
        }
    }
}