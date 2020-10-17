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
        private ICoreDiagnosticInfoProvider _coreDiagnosticInfoProvider = null!;
        private IGameTimeProvider _gameTimeProvider = null!;
        private IEngineSystems _engineSystems = null!;
        private ISceneManagerForGameLoop _sceneManager = null!;
        private IPerformanceStatisticsRecorder _performanceStatisticsRecorder = null!;

        private IAnimationSystem _animationSystem = null!;
        private IAudioSystem _audioSystem = null!;
        private IBehaviorSystem _behaviorSystem = null!;
        private IEntityDestructionSystem _entityDestructionSystem = null!;
        private IInputSystem _inputSystem = null!;
        private IPhysicsSystem _physicsSystem = null!;
        private IRenderingSystem _renderingSystem = null!;
        private ICustomSystem _customSystem1 = null!;
        private ICustomSystem _customSystem2 = null!;
        private ICustomSystem _customSystem3 = null!;

        private const string AnimationSystemName = "AnimationSystemName";
        private const string AudioSystemName = "AudioSystem";
        private const string BehaviorSystemName = "BehaviorSystem";
        private const string EntityDestructionSystemName = "EntityDestructionSystem";
        private const string InputSystemName = "InputSystem";
        private const string PhysicsSystemName = "PhysicsSystem";
        private const string RenderingSystemName = "RenderingSystem";
        private const string CustomSystem1Name = "CustomSystem1";
        private const string CustomSystem2Name = "CustomSystem2";
        private const string CustomSystem3Name = "CustomSystem3";

        [SetUp]
        public void SetUp()
        {
            _coreDiagnosticInfoProvider = Substitute.For<ICoreDiagnosticInfoProvider>();
            _gameTimeProvider = Substitute.For<IGameTimeProvider>();
            _engineSystems = Substitute.For<IEngineSystems>();
            _sceneManager = Substitute.For<ISceneManagerForGameLoop>();
            _performanceStatisticsRecorder = Substitute.For<IPerformanceStatisticsRecorder>();

            _animationSystem = Substitute.For<IAnimationSystem>();
            _engineSystems.AnimationSystem.Returns(_animationSystem);
            _engineSystems.AnimationSystemName.Returns(AnimationSystemName);
            _audioSystem = Substitute.For<IAudioSystem>();
            _engineSystems.AudioSystem.Returns(_audioSystem);
            _engineSystems.AudioSystemName.Returns(AudioSystemName);
            _behaviorSystem = Substitute.For<IBehaviorSystem>();
            _engineSystems.BehaviorSystem.Returns(_behaviorSystem);
            _engineSystems.BehaviorSystemName.Returns(BehaviorSystemName);
            _entityDestructionSystem = Substitute.For<IEntityDestructionSystem>();
            _engineSystems.EntityDestructionSystem.Returns(_entityDestructionSystem);
            _engineSystems.EntityDestructionSystemName.Returns(EntityDestructionSystemName);
            _inputSystem = Substitute.For<IInputSystem>();
            _engineSystems.InputSystem.Returns(_inputSystem);
            _engineSystems.InputSystemName.Returns(InputSystemName);
            _physicsSystem = Substitute.For<IPhysicsSystem>();
            _engineSystems.PhysicsSystem.Returns(_physicsSystem);
            _engineSystems.PhysicsSystemName.Returns(PhysicsSystemName);
            _renderingSystem = Substitute.For<IRenderingSystem>();
            _engineSystems.RenderingSystem.Returns(_renderingSystem);
            _engineSystems.RenderingSystemName.Returns(RenderingSystemName);
            _customSystem1 = Substitute.For<ICustomSystem>();
            _customSystem1.Name.Returns(CustomSystem1Name);
            _customSystem2 = Substitute.For<ICustomSystem>();
            _customSystem2.Name.Returns(CustomSystem2Name);
            _customSystem3 = Substitute.For<ICustomSystem>();
            _customSystem3.Name.Returns(CustomSystem3Name);
            _engineSystems.CustomSystems.Returns(new[] {_customSystem1, _customSystem2, _customSystem3});
        }

        private GameLoop GetGameLoop(CoreConfiguration? configuration = null)
        {
            return new GameLoop(
                _coreDiagnosticInfoProvider,
                _gameTimeProvider,
                _engineSystems,
                _sceneManager,
                _performanceStatisticsRecorder,
                configuration ?? CoreConfiguration.CreateBuilder().Build());
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
                _behaviorSystem.Received(1).ProcessBehaviorFixedUpdate(scene);
                _customSystem1.Received(1).ProcessFixedUpdate(scene);
                _customSystem2.Received(1).ProcessFixedUpdate(scene);
                _customSystem3.Received(1).ProcessFixedUpdate(scene);
                _physicsSystem.Received(1).ProcessPhysics(scene);
                _entityDestructionSystem.Received(1).DestroyEntitiesAfterFixedTimeStep(scene);
                _behaviorSystem.Received(1).ProcessBehaviorUpdate(scene, gameTime);
                _customSystem1.Received(1).ProcessUpdate(scene, gameTime);
                _customSystem2.Received(1).ProcessUpdate(scene, gameTime);
                _customSystem3.Received(1).ProcessUpdate(scene, gameTime);
                _audioSystem.Received(1).ProcessAudio(scene);
                _animationSystem.Received(1).ProcessAnimations(scene, gameTime);
                _renderingSystem.Received(1).RenderScene(scene);
                _entityDestructionSystem.Received(1).DestroyEntitiesAfterFullFrame(scene);
            });
        }

        [TestCase(0.05, 0, 0)]
        [TestCase(0.1, 0, 1)]
        [TestCase(0.2, 0, 2)]
        [TestCase(0.78, 0, 7)]
        [TestCase(0.78, 10, 7)]
        [TestCase(0.78, 3, 3)]
        public void Update_ShouldExecuteFixedTimeStepSystemsCorrectNumberOfTimes(double deltaTime, int fixedUpdatesPerFrameLimit, int expectedFixedUpdateCount)
        {
            // Arrange
            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            var gameTime = new GameTime(TimeSpan.FromSeconds(deltaTime));
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var scene = new Scene();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop(CoreConfiguration.CreateBuilder().WithFixedUpdatesPerFrameLimit(fixedUpdatesPerFrameLimit).Build());

            // Act
            gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                for (var i = 0; i < expectedFixedUpdateCount; i++)
                {
                    _inputSystem.Received(1).ProcessInput(scene);
                    _behaviorSystem.Received(1).ProcessBehaviorFixedUpdate(scene);
                    _customSystem1.Received(1).ProcessFixedUpdate(scene);
                    _customSystem2.Received(1).ProcessFixedUpdate(scene);
                    _customSystem3.Received(1).ProcessFixedUpdate(scene);
                    _physicsSystem.Received(1).ProcessPhysics(scene);
                    _entityDestructionSystem.Received(1).DestroyEntitiesAfterFixedTimeStep(scene);
                }
            });
        }

        [Test]
        public void Update_ShouldUpdateDiagnosticsAfterUpdateOfAllSystemsAndAfterRecordingFrame()
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
                _behaviorSystem.Received(1).ProcessBehaviorFixedUpdate(scene);
                _customSystem1.Received(1).ProcessFixedUpdate(scene);
                _customSystem2.Received(1).ProcessFixedUpdate(scene);
                _customSystem3.Received(1).ProcessFixedUpdate(scene);
                _physicsSystem.Received(1).ProcessPhysics(scene);
                _entityDestructionSystem.Received(1).DestroyEntitiesAfterFixedTimeStep(scene);
                _behaviorSystem.Received(1).ProcessBehaviorUpdate(scene, gameTime);
                _customSystem1.Received(1).ProcessUpdate(scene, gameTime);
                _customSystem2.Received(1).ProcessUpdate(scene, gameTime);
                _customSystem3.Received(1).ProcessUpdate(scene, gameTime);
                _audioSystem.Received(1).ProcessAudio(scene);
                _animationSystem.Received(1).ProcessAnimations(scene, gameTime);
                _renderingSystem.Received(1).RenderScene(scene);
                _entityDestructionSystem.Received(1).DestroyEntitiesAfterFullFrame(scene);

                _performanceStatisticsRecorder.RecordFrame();
                _coreDiagnosticInfoProvider.UpdateDiagnostics(scene);
            });
        }

        [Test]
        public void Update_ShouldFirstRecordSystemsThenRecordFrame()
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
                _performanceStatisticsRecorder.RecordSystemExecution(InputSystemName);
                _performanceStatisticsRecorder.RecordSystemExecution(BehaviorSystemName);
                _performanceStatisticsRecorder.RecordSystemExecution(CustomSystem1Name);
                _performanceStatisticsRecorder.RecordSystemExecution(CustomSystem2Name);
                _performanceStatisticsRecorder.RecordSystemExecution(CustomSystem3Name);
                _performanceStatisticsRecorder.RecordSystemExecution(PhysicsSystemName);
                _performanceStatisticsRecorder.RecordSystemExecution(EntityDestructionSystemName);
                _performanceStatisticsRecorder.RecordSystemExecution(BehaviorSystemName);
                _performanceStatisticsRecorder.RecordSystemExecution(CustomSystem1Name);
                _performanceStatisticsRecorder.RecordSystemExecution(CustomSystem2Name);
                _performanceStatisticsRecorder.RecordSystemExecution(CustomSystem3Name);
                _performanceStatisticsRecorder.RecordSystemExecution(AudioSystemName);
                _performanceStatisticsRecorder.RecordSystemExecution(AnimationSystemName);
                _performanceStatisticsRecorder.RecordSystemExecution(RenderingSystemName);
                _performanceStatisticsRecorder.RecordSystemExecution(EntityDestructionSystemName);
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