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
        public void Update_ShouldUpdateSystemsWithCorrectSceneAndDeltaTime()
        {
            // Arrange
            var system1 = Substitute.For<ISystem>();
            var system2 = Substitute.For<ISystem>();
            var system3 = Substitute.For<ISystem>();

            _systemsProvider.GetSystems().Returns(new[] {system1, system2, system3});

            const double deltaTime = 0.1;
            _deltaTimeProvider.GetDeltaTime().Returns(deltaTime);

            var scene = new Scene();
            _sceneManager.CurrentScene.Returns(scene);

            // Act
            _gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                system1.Update(scene, deltaTime);
                system2.Update(scene, deltaTime);
                system3.Update(scene, deltaTime);
            });
        }
    }
}