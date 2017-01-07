using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests
{
    [TestFixture]
    public class GameLoopTests
    {
        private ISystemsProvider _systemsProvider;
        private IDeltaTimeProvider _deltaTimeProvider;
        private ISceneManager _sceneManager;
        private GameLoop _gameLoop;

        [SetUp]
        public void SetUp()
        {
            _systemsProvider = Substitute.For<ISystemsProvider>();
            _deltaTimeProvider = Substitute.For<IDeltaTimeProvider>();
            _sceneManager = Substitute.For<ISceneManager>();
            _gameLoop = new GameLoop(_systemsProvider, _deltaTimeProvider, _sceneManager);
        }

        [Test]
        public void Update_ShouldGetDeltaTime()
        {
            // Arrange
            _deltaTimeProvider.GetDeltaTime().Returns(0.1);

            // Act
            _gameLoop.Update();

            // Assert
            _deltaTimeProvider.Received(1).GetDeltaTime();
        }

        [Test]
        public void Update_ShouldUpdateSystemsUpdatableWithCorrectDeltaTime()
        {
            // Arrange
            var systemsUpdatable = Substitute.For<IUpdatable>();
            _systemsProvider.GetSystemsUpdatableForScene(Arg.Any<Scene>()).Returns(systemsUpdatable);

            const double deltaTime = 0.1;
            _deltaTimeProvider.GetDeltaTime().Returns(deltaTime);

            // Act
            _gameLoop.Update();

            // Assert
            systemsUpdatable.Received(1).Update(deltaTime);
        }

        [Test]
        public void Update_ShouldGetSystemsUpdatableForSceneWithCorrectScene()
        {
            // Arrange
            var scene = new Scene();
            _deltaTimeProvider.GetDeltaTime().Returns(0.1);
            _sceneManager.CurrentScene.Returns(scene);

            // Act
            _gameLoop.Update();

            // Assert
            _systemsProvider.Received(1).GetSystemsUpdatableForScene(scene);
        }
    }
}