using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core
{
    [TestFixture]
    public class GameLoopTests
    {
        private ICoreDiagnosticInfoProvider _coreDiagnosticInfoProvider;
        private IGameTimeProvider _gameTimeProvider;
        private IEngineSystems _engineSystems;
        private ISceneManagerForGameLoop _sceneManager;
        private ISystemsProvider _systemsProvider;
        private IPerformanceStatisticsRecorder _performanceStatisticsRecorder;
        private IConfigurationManager _configurationManager;

        private IAudioSystem _audioSystem;
        private IEntityDestructionSystem _entityDestructionSystem;
        private IInputSystem _inputSystem;
        private IPhysicsSystem _physicsSystem;
        private IRenderingSystem _renderingSystem;

        [SetUp]
        public void SetUp()
        {
            _coreDiagnosticInfoProvider = Substitute.For<ICoreDiagnosticInfoProvider>();
            _gameTimeProvider = Substitute.For<IGameTimeProvider>();
            _engineSystems = Substitute.For<IEngineSystems>();
            _sceneManager = Substitute.For<ISceneManagerForGameLoop>();
            _systemsProvider = Substitute.For<ISystemsProvider>();
            _performanceStatisticsRecorder = Substitute.For<IPerformanceStatisticsRecorder>();
            _performanceStatisticsRecorder.RecordSystemExecution(Arg.Any<IFixedTimeStepSystem>(), Arg.Do<Action>(action => action()));
            _performanceStatisticsRecorder.RecordSystemExecution(Arg.Any<IVariableTimeStepSystem>(), Arg.Do<Action>(action => action()));
            _configurationManager = Substitute.For<IConfigurationManager>();

            _audioSystem = Substitute.For<IAudioSystem>();
            _engineSystems.AudioSystem.Returns(_audioSystem);
            _entityDestructionSystem = Substitute.For<IEntityDestructionSystem>();
            _engineSystems.EntityDestructionSystem.Returns(_entityDestructionSystem);
            _inputSystem = Substitute.For<IInputSystem>();
            _engineSystems.InputSystem.Returns(_inputSystem);
            _physicsSystem = Substitute.For<IPhysicsSystem>();
            _engineSystems.PhysicsSystem.Returns(_physicsSystem);
            _renderingSystem = Substitute.For<IRenderingSystem>();
            _engineSystems.RenderingSystem.Returns(_renderingSystem);
        }

        private GameLoop GetGameLoop(CoreConfiguration configuration = null)
        {
            _configurationManager.GetConfiguration<CoreConfiguration>().Returns(configuration ?? new CoreConfiguration());

            return new GameLoop(
                _coreDiagnosticInfoProvider,
                _gameTimeProvider,
                _engineSystems,
                _sceneManager,
                _systemsProvider,
                _performanceStatisticsRecorder,
                _configurationManager);
        }

        [Test]
        public void Update_ShouldExecuteEngineSystemsInCorrectOrder()
        {
            // Arrange
            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            var gameTime = new GameTime(TimeSpan.FromSeconds(0.15));
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var scene = new Scene();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop();

            // Act
            gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                _inputSystem.Received(1).ProcessInput(scene);
                _physicsSystem.Received(1).ProcessPhysics(scene);
                _entityDestructionSystem.Received(1).DestroyEntities(scene);
                _audioSystem.Received(1).ProcessAudio(scene);
                _renderingSystem.Received(1).RenderScene(scene);
            });
        }

        [TestCase(0.05, 0, 0)]
        [TestCase(0.1, 0, 1)]
        [TestCase(0.2, 0, 2)]
        [TestCase(0.78, 0, 7)]
        [TestCase(0.78, 10, 7)]
        [TestCase(0.78, 3, 3)]
        public void Update_ShouldFixedUpdateFixedTimeStepSystemsCorrectNumberOfTimes(double deltaTime, int fixedUpdatesPerFrameLimit,
            int expectedFixedUpdateCount)
        {
            // Arrange
            var system1 = Substitute.For<IFixedTimeStepSystem>();
            var system2 = Substitute.For<IFixedTimeStepSystem>();
            var system3 = Substitute.For<IFixedTimeStepSystem>();

            _systemsProvider.GetFixedTimeStepSystems().Returns(new[] {system1, system2, system3});

            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            var gameTime = new GameTime(TimeSpan.FromSeconds(deltaTime));
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var scene = new Scene();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop(new CoreConfiguration {FixedUpdatesPerFrameLimit = fixedUpdatesPerFrameLimit});

            // Act
            gameLoop.Update();

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
            var gameTime = new GameTime(TimeSpan.FromSeconds(0.15));
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var scene = new Scene();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop();

            // Act
            gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                system1.FixedUpdate(scene);
                system2.FixedUpdate(scene);
                system3.FixedUpdate(scene);
            });
        }

        [Test]
        public void Update_ShouldUpdateVariableTimeStepSystemsWithCorrectSceneAndGameTime()
        {
            // Arrange
            var system1 = Substitute.For<IVariableTimeStepSystem>();
            var system2 = Substitute.For<IVariableTimeStepSystem>();
            var system3 = Substitute.For<IVariableTimeStepSystem>();

            _systemsProvider.GetVariableTimeStepSystems().Returns(new[] {system1, system2, system3});

            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            var gameTime = new GameTime(TimeSpan.FromSeconds(0.1));
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var scene = new Scene();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop();

            // Act
            gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                system1.Update(scene, gameTime);
                system2.Update(scene, gameTime);
                system3.Update(scene, gameTime);
            });
        }

        [Test]
        public void Update_ShouldUpdateDiagnosticsAfterUpdateOfAllSystemsAndAfterRecordingFrame()
        {
            // Arrange
            var system1 = Substitute.For<IVariableTimeStepSystem>();
            var system2 = Substitute.For<IVariableTimeStepSystem>();
            var system3 = Substitute.For<IFixedTimeStepSystem>();
            var system4 = Substitute.For<IFixedTimeStepSystem>();

            _systemsProvider.GetVariableTimeStepSystems().Returns(new[] {system1, system2});
            _systemsProvider.GetFixedTimeStepSystems().Returns(new[] {system3, system4});

            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            var gameTime = new GameTime(TimeSpan.FromSeconds(0.15));
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var scene = new Scene();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop();

            // Act
            gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                system3.FixedUpdate(scene);
                system4.FixedUpdate(scene);
                system1.Update(scene, gameTime);
                system2.Update(scene, gameTime);
                _performanceStatisticsRecorder.RecordFrame();
                _coreDiagnosticInfoProvider.UpdateDiagnostics(scene);
            });
        }

        [Test]
        public void Update_ShouldFirstRecordSystemsThenRecordFrame()
        {
            // Arrange
            var fixedTimeStepSystem = Substitute.For<IFixedTimeStepSystem>();
            var variableTimeStepSystem = Substitute.For<IVariableTimeStepSystem>();

            _systemsProvider.GetVariableTimeStepSystems().Returns(new[] {variableTimeStepSystem});
            _systemsProvider.GetFixedTimeStepSystems().Returns(new[] {fixedTimeStepSystem});

            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            var gameTime = new GameTime(TimeSpan.FromSeconds(0.15));
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var scene = new Scene();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop();

            // Act
            gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                _performanceStatisticsRecorder.RecordSystemExecution(Arg.Any<IFixedTimeStepSystem>(), Arg.Any<Action>());
                _performanceStatisticsRecorder.RecordSystemExecution(Arg.Any<IVariableTimeStepSystem>(), Arg.Any<Action>());
                _performanceStatisticsRecorder.RecordFrame();
            });
        }

        [Test]
        public void Update_ShouldExecuteSceneManagerOnNextFrameBeforeAccessingCurrentScene()
        {
            // Arrange
            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            var gameTime = new GameTime(TimeSpan.FromSeconds(0.15));
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var sceneBeforeOnNextFrame = new Scene();
            var sceneAfterOnNextFrame = new Scene();
            _sceneManager.CurrentScene.Returns(sceneBeforeOnNextFrame);

            _sceneManager.When(sm => sm.OnNextFrame()).Do(_ => { _sceneManager.CurrentScene.Returns(sceneAfterOnNextFrame); });

            var gameLoop = GetGameLoop();

            // Act
            gameLoop.Update();

            // Assert
            _coreDiagnosticInfoProvider.Received().UpdateDiagnostics(sceneAfterOnNextFrame);
        }
    }
}