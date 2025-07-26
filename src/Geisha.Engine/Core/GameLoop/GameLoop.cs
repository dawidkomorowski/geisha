using System;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.GameLoop
{
    internal interface IGameLoop
    {
        void Update();
    }

    internal sealed class GameLoop : IGameLoop
    {
        private readonly ICoreDiagnosticInfoProvider _coreDiagnosticInfoProvider;
        private readonly IGameTimeProvider _gameTimeProvider;
        private readonly IGameLoopSteps _gameLoopSteps;
        private readonly ISceneManagerInternal _sceneManager;
        private readonly IPerformanceStatisticsRecorder _performanceStatisticsRecorder;
        private readonly int _fixedUpdatesPerFrameLimit;

        private TimeSpan _timeToSimulate;

        public GameLoop(
            ICoreDiagnosticInfoProvider coreDiagnosticInfoProvider,
            IGameTimeProvider gameTimeProvider,
            IGameLoopSteps gameLoopSteps,
            ISceneManagerInternal sceneManager,
            IPerformanceStatisticsRecorder performanceStatisticsRecorder,
            CoreConfiguration configuration)
        {
            _coreDiagnosticInfoProvider = coreDiagnosticInfoProvider;
            _gameTimeProvider = gameTimeProvider;
            _gameLoopSteps = gameLoopSteps;
            _sceneManager = sceneManager;
            _performanceStatisticsRecorder = performanceStatisticsRecorder;

            _fixedUpdatesPerFrameLimit = configuration.FixedUpdatesPerFrameLimit;
        }

        public void Update()
        {
            _sceneManager.OnNextFrame();

            var scene = _sceneManager.CurrentScene;
            var gameTime = _gameTimeProvider.GetGameTime();

            _timeToSimulate += gameTime.DeltaTime;
            var fixedUpdatesPerFrame = 0;

            while (_timeToSimulate >= GameTime.FixedDeltaTime && (fixedUpdatesPerFrame < _fixedUpdatesPerFrameLimit || _fixedUpdatesPerFrameLimit == 0))
            {
                _performanceStatisticsRecorder.BeginStepDuration();
                _gameLoopSteps.InputStep.ProcessInput();
                _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.InputStepName);

                _performanceStatisticsRecorder.BeginStepDuration();
                _gameLoopSteps.BehaviorStep.ProcessBehaviorFixedUpdate();
                _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.BehaviorStepName);

                _performanceStatisticsRecorder.BeginStepDuration();
                _gameLoopSteps.CoroutineStep.ProcessCoroutines();
                _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.CoroutineStepName);

                foreach (var customStep in _gameLoopSteps.CustomSteps)
                {
                    _performanceStatisticsRecorder.BeginStepDuration();
                    customStep.ProcessFixedUpdate();
                    _performanceStatisticsRecorder.EndStepDuration(customStep.Name);
                }

                _performanceStatisticsRecorder.BeginStepDuration();
                _gameLoopSteps.PhysicsStep.ProcessPhysics();
                _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.PhysicsStepName);

                scene.RemoveEntitiesAfterFixedTimeStep();

                _timeToSimulate -= GameTime.FixedDeltaTime;
                fixedUpdatesPerFrame++;
            }

            _performanceStatisticsRecorder.BeginStepDuration();
            _gameLoopSteps.BehaviorStep.ProcessBehaviorUpdate(gameTime);
            _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.BehaviorStepName);

            _performanceStatisticsRecorder.BeginStepDuration();
            _gameLoopSteps.CoroutineStep.ProcessCoroutines(gameTime);
            _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.CoroutineStepName);

            foreach (var customStep in _gameLoopSteps.CustomSteps)
            {
                _performanceStatisticsRecorder.BeginStepDuration();
                customStep.ProcessUpdate(gameTime);
                _performanceStatisticsRecorder.EndStepDuration(customStep.Name);
            }

            _performanceStatisticsRecorder.BeginStepDuration();
            _gameLoopSteps.PhysicsStep.PreparePhysicsDebugInformation();
            _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.PhysicsStepName);

            _performanceStatisticsRecorder.BeginStepDuration();
            _gameLoopSteps.AudioStep.ProcessAudio();
            _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.AudioStepName);

            _performanceStatisticsRecorder.BeginStepDuration();
            _gameLoopSteps.AnimationStep.ProcessAnimations(gameTime);
            _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.AnimationStepName);

            _performanceStatisticsRecorder.BeginStepDuration();
            _gameLoopSteps.RenderingStep.RenderScene();
            _performanceStatisticsRecorder.EndStepDuration(_gameLoopSteps.RenderingStepName);

            scene.RemoveEntitiesAfterFullFrame();

            _performanceStatisticsRecorder.RecordFrame();
            _coreDiagnosticInfoProvider.UpdateDiagnostics(scene);
        }
    }
}