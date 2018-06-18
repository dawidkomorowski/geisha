using System;
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
            _gameTimeProvider = Substitute.For<IGameTimeProvider>();
            _sceneManager = Substitute.For<ISceneManager>();
            _coreDiagnosticsInfoProvider = Substitute.For<ICoreDiagnosticsInfoProvider>();
            _gameLoop = new GameLoop(_systemsProvider, _gameTimeProvider, _sceneManager, _coreDiagnosticsInfoProvider);
        }

        private ISystemsProvider _systemsProvider;
        private IGameTimeProvider _gameTimeProvider;
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

            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            _gameTimeProvider.GetGameTime().Returns(new GameTime(TimeSpan.FromSeconds(deltaTime)));

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

            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            _gameTimeProvider.GetGameTime().Returns(new GameTime(TimeSpan.FromSeconds(0.15)));

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

            var gameTime = new GameTime(TimeSpan.FromSeconds(0.15));
            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var scene = new Scene();
            _sceneManager.CurrentScene.Returns(scene);

            // Act
            _gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                system3.FixedUpdate(scene);
                system4.FixedUpdate(scene);
                system1.Update(scene, gameTime);
                system2.Update(scene, gameTime);
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

            var gameTime = new GameTime(TimeSpan.FromSeconds(0.1));
            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var scene = new Scene();
            _sceneManager.CurrentScene.Returns(scene);

            // Act
            _gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                system1.Update(scene, gameTime);
                system2.Update(scene, gameTime);
                system3.Update(scene, gameTime);
            });
        }
    }
}