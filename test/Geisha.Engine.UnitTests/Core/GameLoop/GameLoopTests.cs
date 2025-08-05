using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.GameLoop
{
    [TestFixture]
    public class GameLoopTests
    {
        private ICoreDiagnosticInfoProvider _coreDiagnosticInfoProvider = null!;
        private IGameTimeProvider _gameTimeProvider = null!;
        private IGameLoopSteps _gameLoopSteps = null!;
        private ISceneManagerInternal _sceneManager = null!;
        private IPerformanceStatisticsRecorder _performanceStatisticsRecorder = null!;

        private IAnimationGameLoopStep _animationStep = null!;
        private IAudioGameLoopStep _audioStep = null!;
        private IBehaviorGameLoopStep _behaviorStep = null!;
        private ICoroutineGameLoopStep _coroutineStep = null!;
        private IInputGameLoopStep _inputStep = null!;
        private IPhysicsGameLoopStep _physicsStep = null!;
        private IRenderingGameLoopStep _renderingStep = null!;
        private ITransformInterpolationGameLoopStep _transformInterpolationStep = null!;
        private ICustomGameLoopStep _customStep1 = null!;
        private ICustomGameLoopStep _customStep2 = null!;
        private ICustomGameLoopStep _customStep3 = null!;

        private const string AnimationStepName = "AnimationStep";
        private const string AudioStepName = "AudioStep";
        private const string BehaviorStepName = "BehaviorStep";
        private const string CoroutineStepName = "CoroutineStep";
        private const string InputStepName = "InputStep";
        private const string PhysicsStepName = "PhysicsStep";
        private const string RenderingStepName = "RenderingStep";
        private const string TransformInterpolationStepName = "TransformInterpolationStep";
        private const string CustomStep1Name = "CustomStep1";
        private const string CustomStep2Name = "CustomStep2";
        private const string CustomStep3Name = "CustomStep3";

        [SetUp]
        public void SetUp()
        {
            _coreDiagnosticInfoProvider = Substitute.For<ICoreDiagnosticInfoProvider>();
            _gameTimeProvider = Substitute.For<IGameTimeProvider>();
            _gameLoopSteps = Substitute.For<IGameLoopSteps>();
            _sceneManager = Substitute.For<ISceneManagerInternal>();
            _performanceStatisticsRecorder = Substitute.For<IPerformanceStatisticsRecorder>();

            _animationStep = Substitute.For<IAnimationGameLoopStep>();
            _gameLoopSteps.AnimationStep.Returns(_animationStep);
            _gameLoopSteps.AnimationStepName.Returns(AnimationStepName);

            _audioStep = Substitute.For<IAudioGameLoopStep>();
            _gameLoopSteps.AudioStep.Returns(_audioStep);
            _gameLoopSteps.AudioStepName.Returns(AudioStepName);

            _behaviorStep = Substitute.For<IBehaviorGameLoopStep>();
            _gameLoopSteps.BehaviorStep.Returns(_behaviorStep);
            _gameLoopSteps.BehaviorStepName.Returns(BehaviorStepName);

            _coroutineStep = Substitute.For<ICoroutineGameLoopStep>();
            _gameLoopSteps.CoroutineStep.Returns(_coroutineStep);
            _gameLoopSteps.CoroutineStepName.Returns(CoroutineStepName);

            _inputStep = Substitute.For<IInputGameLoopStep>();
            _gameLoopSteps.InputStep.Returns(_inputStep);
            _gameLoopSteps.InputStepName.Returns(InputStepName);

            _physicsStep = Substitute.For<IPhysicsGameLoopStep>();
            _gameLoopSteps.PhysicsStep.Returns(_physicsStep);
            _gameLoopSteps.PhysicsStepName.Returns(PhysicsStepName);

            _renderingStep = Substitute.For<IRenderingGameLoopStep>();
            _gameLoopSteps.RenderingStep.Returns(_renderingStep);
            _gameLoopSteps.RenderingStepName.Returns(RenderingStepName);

            _transformInterpolationStep = Substitute.For<ITransformInterpolationGameLoopStep>();
            _gameLoopSteps.TransformInterpolationStep.Returns(_transformInterpolationStep);
            _gameLoopSteps.TransformInterpolationStepName.Returns(TransformInterpolationStepName);

            _customStep1 = Substitute.For<ICustomGameLoopStep>();
            _customStep1.Name.Returns(CustomStep1Name);
            _customStep2 = Substitute.For<ICustomGameLoopStep>();
            _customStep2.Name.Returns(CustomStep2Name);
            _customStep3 = Substitute.For<ICustomGameLoopStep>();
            _customStep3.Name.Returns(CustomStep3Name);
            _gameLoopSteps.CustomSteps.Returns(new[] { _customStep1, _customStep2, _customStep3 });
        }

        private Geisha.Engine.Core.GameLoop.GameLoop GetGameLoop(CoreConfiguration? configuration = null)
        {
            return new Geisha.Engine.Core.GameLoop.GameLoop(
                _coreDiagnosticInfoProvider,
                _gameTimeProvider,
                _gameLoopSteps,
                _sceneManager,
                _performanceStatisticsRecorder,
                configuration ?? new CoreConfiguration());
        }

        [Test]
        public void Update_ShouldExecuteGameLoopStepsInCorrectOrder()
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
                _inputStep.Received(1).ProcessInput();
                _behaviorStep.Received(1).ProcessBehaviorFixedUpdate();
                _coroutineStep.Received(1).ProcessCoroutines();
                _customStep1.Received(1).ProcessFixedUpdate();
                _customStep2.Received(1).ProcessFixedUpdate();
                _customStep3.Received(1).ProcessFixedUpdate();
                _physicsStep.Received(1).ProcessPhysics();
                _transformInterpolationStep.Received(1).SnapshotTransforms();
                _transformInterpolationStep.Received(1).InterpolateTransforms(0.5);
                _behaviorStep.Received(1).ProcessBehaviorUpdate(gameTime);
                _coroutineStep.Received(1).ProcessCoroutines(gameTime);
                _customStep1.Received(1).ProcessUpdate(gameTime);
                _customStep2.Received(1).ProcessUpdate(gameTime);
                _customStep3.Received(1).ProcessUpdate(gameTime);
                _physicsStep.Received(1).PreparePhysicsDebugInformation();
                _audioStep.Received(1).ProcessAudio();
                _animationStep.Received(1).ProcessAnimations(gameTime);
                _renderingStep.Received(1).RenderScene();
            });
        }

        [TestCase(0.05, 0, 0)]
        [TestCase(0.1, 0, 1)]
        [TestCase(0.2, 0, 2)]
        [TestCase(0.78, 0, 7)]
        [TestCase(0.78, 10, 7)]
        [TestCase(0.78, 3, 3)]
        public void Update_ShouldExecuteFixedTimeStepGameLoopStepsCorrectNumberOfTimes(double deltaTime, int fixedUpdatesPerFrameLimit,
            int expectedFixedUpdateCount)
        {
            // Arrange
            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
            var gameTime = new GameTime(TimeSpan.FromSeconds(deltaTime));
            _gameTimeProvider.GetGameTime().Returns(gameTime);

            var scene = TestSceneFactory.Create();
            _sceneManager.CurrentScene.Returns(scene);

            var gameLoop = GetGameLoop(new CoreConfiguration { FixedUpdatesPerFrameLimit = fixedUpdatesPerFrameLimit });

            // Act
            gameLoop.Update();

            // Assert
            Received.InOrder(() =>
            {
                for (var i = 0; i < expectedFixedUpdateCount; i++)
                {
                    _inputStep.Received(1).ProcessInput();
                    _behaviorStep.Received(1).ProcessBehaviorFixedUpdate();
                    _coroutineStep.Received(1).ProcessCoroutines();
                    _customStep1.Received(1).ProcessFixedUpdate();
                    _customStep2.Received(1).ProcessFixedUpdate();
                    _customStep3.Received(1).ProcessFixedUpdate();
                    _physicsStep.Received(1).ProcessPhysics();
                    _transformInterpolationStep.Received(1).SnapshotTransforms();
                }
            });
        }

        [Test]
        public void Update_ShouldHandleInterpolationWhenFixedUpdatesLimitReached()
        {
            Assert.Fail("TODO Test Limit and maybe add tests for different interpolation factors.");
        }

        [Test]
        public void Update_ShouldUpdateDiagnosticsAfterUpdateOfAllGameLoopStepsAndAfterRecordingFrame()
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
                _inputStep.Received(1).ProcessInput();
                _behaviorStep.Received(1).ProcessBehaviorFixedUpdate();
                _coroutineStep.Received(1).ProcessCoroutines();
                _customStep1.Received(1).ProcessFixedUpdate();
                _customStep2.Received(1).ProcessFixedUpdate();
                _customStep3.Received(1).ProcessFixedUpdate();
                _physicsStep.Received(1).ProcessPhysics();
                _transformInterpolationStep.Received(1).SnapshotTransforms();
                _transformInterpolationStep.Received(1).InterpolateTransforms(0.5);
                _behaviorStep.Received(1).ProcessBehaviorUpdate(gameTime);
                _coroutineStep.Received(1).ProcessCoroutines(gameTime);
                _customStep1.Received(1).ProcessUpdate(gameTime);
                _customStep2.Received(1).ProcessUpdate(gameTime);
                _customStep3.Received(1).ProcessUpdate(gameTime);
                _physicsStep.Received(1).PreparePhysicsDebugInformation();
                _audioStep.Received(1).ProcessAudio();
                _animationStep.Received(1).ProcessAnimations(gameTime);
                _renderingStep.Received(1).RenderScene();

                _performanceStatisticsRecorder.RecordFrame();
                _coreDiagnosticInfoProvider.UpdateDiagnostics(scene);
            });
        }

        [Test]
        public void Update_ShouldFirstRecordGameLoopStepsThenRecordFrame()
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
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(InputStepName);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(BehaviorStepName);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(CoroutineStepName);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(CustomStep1Name);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(CustomStep2Name);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(CustomStep3Name);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(PhysicsStepName);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(TransformInterpolationStepName);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(TransformInterpolationStepName);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(BehaviorStepName);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(CoroutineStepName);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(CustomStep1Name);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(CustomStep2Name);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(CustomStep3Name);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(PhysicsStepName);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(AudioStepName);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(AnimationStepName);
                _performanceStatisticsRecorder.BeginStepDuration();
                _performanceStatisticsRecorder.EndStepDuration(RenderingStepName);
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
            Assert.That(scene.AllEntities, Contains.Item(entity));

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
            Assert.That(scene.AllEntities, Contains.Item(entity));

            // Act
            gameLoop.Update();

            // Assert
            Assert.That(scene.AllEntities, Does.Not.Contains(entity));
        }

        #endregion
    }
}