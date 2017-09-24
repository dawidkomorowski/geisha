using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests
{
    [TestFixture]
    public class GameLoopTests
    {
        [SetUp]
        public void SetUp()
        {
            _systemsProvider = Substitute.For<ISystemsProvider>();
            _deltaTimeProvider = Substitute.For<IDeltaTimeProvider>();
            _fixedDeltaTimeProvider = Substitute.For<IFixedDeltaTimeProvider>();
            _sceneManager = Substitute.For<ISceneManager>();
            _coreDiagnosticsInfoProvider = Substitute.For<ICoreDiagnosticsInfoProvider>();
            _gameLoop = new GameLoop(_systemsProvider, _deltaTimeProvider, _fixedDeltaTimeProvider, _sceneManager, _coreDiagnosticsInfoProvider);
        }

        private ISystemsProvider _systemsProvider;
        private IDeltaTimeProvider _deltaTimeProvider;
        private IFixedDeltaTimeProvider _fixedDeltaTimeProvider;
        private ISceneManager _sceneManager;
        private ICoreDiagnosticsInfoProvider _coreDiagnosticsInfoProvider;
        private GameLoop _gameLoop;

        [TestCase(0.05, 0)]
        [TestCase(0.1, 1)]
        [TestCase(0.2, 2)]
        [TestCase(0.78, 7)]
        public void Update_ShouldFixedUpdateFixedTimeStepSystemsCorrectNumberOfTimes(double deltaTime,
            int expectedFixedUpdateCount)
        {
            // Arrange
            var system1 = Substitute.For<IFixedTimeStepSystem>();
            var system2 = Substitute.For<IFixedTimeStepSystem>();
            var system3 = Substitute.For<IFixedTimeStepSystem>();

            _systemsProvider.GetFixedTimeStepSystems().Returns(new[] {system1, system2, system3});

            _fixedDeltaTimeProvider.GetFixedDeltaTime().Returns(0.1);
            _deltaTimeProvider.GetDeltaTime().Returns(deltaTime);

            var scene = new Scene();
            _sceneManager.CurrentScene.Returns(scene);

            // Act
            _gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                for (var i = 0; i < expectedFixedUpdateCount; i++)
                {
                    system1.FixedUpdate(scene);
                    system2.FixedUpdate(scene);
                    system3.FixedUpdate(scene);
                }
            });
        }

        [Test]
        public void Update_ShouldFixedUpdateFixedTimeStepSystemsWithCorrectScene()
        {
            // Arrange
            var system1 = Substitute.For<IFixedTimeStepSystem>();
            var system2 = Substitute.For<IFixedTimeStepSystem>();
            var system3 = Substitute.For<IFixedTimeStepSystem>();

            _systemsProvider.GetFixedTimeStepSystems().Returns(new[] {system1, system2, system3});

            _fixedDeltaTimeProvider.GetFixedDeltaTime().Returns(0.1);
            _deltaTimeProvider.GetDeltaTime().Returns(0.15);

            var scene = new Scene();
            _sceneManager.CurrentScene.Returns(scene);

            // Act
            _gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                system1.FixedUpdate(scene);
                system2.FixedUpdate(scene);
                system3.FixedUpdate(scene);
            });
        }

        [Test]
        public void Update_ShouldUpdateDiagnosticsAfterUpdateOfAllSystems()
        {
            // Arrange
            var system1 = Substitute.For<IVariableTimeStepSystem>();
            var system2 = Substitute.For<IVariableTimeStepSystem>();
            var system3 = Substitute.For<IFixedTimeStepSystem>();
            var system4 = Substitute.For<IFixedTimeStepSystem>();

            _systemsProvider.GetVariableTimeStepSystems().Returns(new[] {system1, system2});
            _systemsProvider.GetFixedTimeStepSystems().Returns(new[] {system3, system4});

            const double deltaTime = 0.15;
            _fixedDeltaTimeProvider.GetFixedDeltaTime().Returns(0.1);
            _deltaTimeProvider.GetDeltaTime().Returns(deltaTime);

            var scene = new Scene();
            _sceneManager.CurrentScene.Returns(scene);

            // Act
            _gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                system3.FixedUpdate(scene);
                system4.FixedUpdate(scene);
                system1.Update(scene, deltaTime);
                system2.Update(scene, deltaTime);
                _coreDiagnosticsInfoProvider.UpdateDiagnostics(scene);
            });
        }

        [Test]
        public void Update_ShouldUpdateVariableTimeStepSystemsWithCorrectSceneAndDeltaTime()
        {
            // Arrange
            var system1 = Substitute.For<IVariableTimeStepSystem>();
            var system2 = Substitute.For<IVariableTimeStepSystem>();
            var system3 = Substitute.For<IVariableTimeStepSystem>();

            _systemsProvider.GetVariableTimeStepSystems().Returns(new[] {system1, system2, system3});

            const double deltaTime = 0.1;
            _fixedDeltaTimeProvider.GetFixedDeltaTime().Returns(0.1);
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