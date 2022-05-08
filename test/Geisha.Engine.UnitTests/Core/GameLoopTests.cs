using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.TestUtils;
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
        private ISceneManagerInternal _sceneManager = null!;
        private IPerformanceStatisticsRecorder _performanceStatisticsRecorder = null!;

        private IAnimationSystem _animationSystem = null!;
        private IAudioSystem _audioSystem = null!;
        private IBehaviorSystem _behaviorSystem = null!;
        private IInputSystem _inputSystem = null!;
        private IPhysicsSystem _physicsSystem = null!;
        private IRenderingSystem _renderingSystem = null!;
        private ICustomSystem _customSystem1 = null!;
        private ICustomSystem _customSystem2 = null!;
        private ICustomSystem _customSystem3 = null!;

        private const string AnimationSystemName = "AnimationSystemName";
        private const string AudioSystemName = "AudioSystem";
        private const string BehaviorSystemName = "BehaviorSystem";
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
            _sceneManager = Substitute.For<ISceneManagerInternal>();
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
            _engineSystems.CustomSystems.Returns(new[] { _customSystem1, _customSystem2, _customSystem3 });
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

            var scene = TestSceneFactory.Create();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop();

            // Act
            gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                _inputSystem.Received(1).ProcessInput();
                _behaviorSystem.Received(1).ProcessBehaviorFixedUpdate();
                _customSystem1.Received(1).ProcessFixedUpdate();
                _customSystem2.Received(1).ProcessFixedUpdate();
                _customSystem3.Received(1).ProcessFixedUpdate();
                _physicsSystem.Received(1).ProcessPhysics();
                _behaviorSystem.Received(1).ProcessBehaviorUpdate(gameTime);
                _customSystem1.Received(1).ProcessUpdate(gameTime);
                _customSystem2.Received(1).ProcessUpdate(gameTime);
                _customSystem3.Received(1).ProcessUpdate(gameTime);
                _physicsSystem.Received(1).PreparePhysicsDebugInformation();
                _audioSystem.Received(1).ProcessAudio();
                _animationSystem.Received(1).ProcessAnimations(gameTime);
                _renderingSystem.Received(1).RenderScene();
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

            var scene = TestSceneFactory.Create();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop(CoreConfiguration.CreateBuilder().WithFixedUpdatesPerFrameLimit(fixedUpdatesPerFrameLimit).Build());

            // Act
            gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                for (var i = 0; i < expectedFixedUpdateCount; i++)
                {
                    _inputSystem.Received(1).ProcessInput();
                    _behaviorSystem.Received(1).ProcessBehaviorFixedUpdate();
                    _customSystem1.Received(1).ProcessFixedUpdate();
                    _customSystem2.Received(1).ProcessFixedUpdate();
                    _customSystem3.Received(1).ProcessFixedUpdate();
                    _physicsSystem.Received(1).ProcessPhysics();
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

            var scene = TestSceneFactory.Create();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop();

            // Act
            gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                _inputSystem.Received(1).ProcessInput();
                _behaviorSystem.Received(1).ProcessBehaviorFixedUpdate();
                _customSystem1.Received(1).ProcessFixedUpdate();
                _customSystem2.Received(1).ProcessFixedUpdate();
                _customSystem3.Received(1).ProcessFixedUpdate();
                _physicsSystem.Received(1).ProcessPhysics();
                _behaviorSystem.Received(1).ProcessBehaviorUpdate(gameTime);
                _customSystem1.Received(1).ProcessUpdate(gameTime);
                _customSystem2.Received(1).ProcessUpdate(gameTime);
                _customSystem3.Received(1).ProcessUpdate(gameTime);
                _physicsSystem.Received(1).PreparePhysicsDebugInformation();
                _audioSystem.Received(1).ProcessAudio();
                _animationSystem.Received(1).ProcessAnimations(gameTime);
                _renderingSystem.Received(1).RenderScene();

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

            var scene = TestSceneFactory.Create();
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
                _performanceStatisticsRecorder.RecordSystemExecution(BehaviorSystemName);
                _performanceStatisticsRecorder.RecordSystemExecution(CustomSystem1Name);
                _performanceStatisticsRecorder.RecordSystemExecution(CustomSystem2Name);
                _performanceStatisticsRecorder.RecordSystemExecution(CustomSystem3Name);
                _performanceStatisticsRecorder.RecordSystemExecution(PhysicsSystemName);
                _performanceStatisticsRecorder.RecordSystemExecution(AudioSystemName);
                _performanceStatisticsRecorder.RecordSystemExecution(AnimationSystemName);
                _performanceStatisticsRecorder.RecordSystemExecution(RenderingSystemName);
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

            var sceneBeforeOnNextFrame = TestSceneFactory.Create();
            var sceneAfterOnNextFrame = TestSceneFactory.Create();
            _sceneManager.CurrentScene.Returns(sceneBeforeOnNextFrame);

            _sceneManager.When(sm => sm.OnNextFrame()).Do(_ => { _sceneManager.CurrentScene.Returns(sceneAfterOnNextFrame); });

            var gameLoop = GetGameLoop();

            // Act
            gameLoop.Update();

            // Assert
            _coreDiagnosticInfoProvider.Received().UpdateDiagnostics(sceneAfterOnNextFrame);
        }

        #region Entities removal

        [Test]
        public void Update_ShouldNotRemoveEntityFromScene_WhenNoRemoveMethodIsExecutedForEntity()
        {
            // Arrange
            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            var gameTime = new GameTime(TimeSpan.FromSeconds(0.15));
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var scene = TestSceneFactory.Create();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop();

            var entity = scene.CreateEntity();

            // Assume
            Assert.That(scene.AllEntities, Contains.Item(entity));

            // Act
            gameLoop.Update();

            // Assert
            Assert.That(scene.AllEntities, Contains.Item(entity));
        }

        [Test]
        public void Update_ShouldNotRemoveEntityFromScene_WhenRemoveAfterFixedTimeStepIsExecutedForEntityButFixedTimeStepDoesNotHappen()
        {
            // Arrange
            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            var gameTime = new GameTime(TimeSpan.FromSeconds(0.05));
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var scene = TestSceneFactory.Create();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop();

            var entity = scene.CreateEntity();
            entity.RemoveAfterFixedTimeStep();

            // Assume
            Assert.That(scene.AllEntities, Contains.Item(entity));

            // Act
            gameLoop.Update();

            // Assert
            Assert.That(scene.AllEntities, Contains.Item(entity));
        }

        [Test]
        public void Update_ShouldRemoveEntityFromScene_WhenRemoveAfterFixedTimeStepIsExecutedForEntityAndFixedTimeStepHappens()
        {
            // Arrange
            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            var gameTime = new GameTime(TimeSpan.FromSeconds(0.15));
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var scene = TestSceneFactory.Create();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop();

            var entity = scene.CreateEntity();
            entity.RemoveAfterFixedTimeStep();

            // Assume
            Assume.That(scene.AllEntities, Contains.Item(entity));

            // Act
            gameLoop.Update();

            // Assert
            Assert.That(scene.AllEntities, Does.Not.Contains(entity));
        }

        [Test]
        public void Update_ShouldRemoveEntityFromScene_WhenRemoveAfterFullFrameIsExecutedForEntityDespiteFixedTimeStepDoesNotHappen()
        {
            // Arrange
            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            var gameTime = new GameTime(TimeSpan.FromSeconds(0.05));
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var scene = TestSceneFactory.Create();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop();

            var entity = scene.CreateEntity();
            entity.RemoveAfterFullFrame();

            // Assume
            Assume.That(scene.AllEntities, Contains.Item(entity));

            // Act
            gameLoop.Update();

            // Assert
            Assert.That(scene.AllEntities, Does.Not.Contains(entity));
        }

        #endregion
    }
}