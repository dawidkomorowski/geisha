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
                using (_performanceStatisticsRecorder.RecordStepDuration(_gameLoopSteps.InputStepName))
                {
                    _gameLoopSteps.InputStep.ProcessInput();
                }

                using (_performanceStatisticsRecorder.RecordStepDuration(_gameLoopSteps.BehaviorStepName))
                {
                    _gameLoopSteps.BehaviorStep.ProcessBehaviorFixedUpdate();
                }

                foreach (var customStep in _gameLoopSteps.CustomSteps)
                {
                    using (_performanceStatisticsRecorder.RecordStepDuration(customStep.Name))
                    {
                        customStep.ProcessFixedUpdate();
                    }
                }

                using (_performanceStatisticsRecorder.RecordStepDuration(_gameLoopSteps.PhysicsStepName))
                {
                    _gameLoopSteps.PhysicsStep.ProcessPhysics();
                }

                scene.RemoveEntitiesAfterFixedTimeStep();

                _timeToSimulate -= GameTime.FixedDeltaTime;
                fixedUpdatesPerFrame++;
            }

            using (_performanceStatisticsRecorder.RecordStepDuration(_gameLoopSteps.BehaviorStepName))
            {
                _gameLoopSteps.BehaviorStep.ProcessBehaviorUpdate(gameTime);
            }

            foreach (var customStep in _gameLoopSteps.CustomSteps)
            {
                using (_performanceStatisticsRecorder.RecordStepDuration(customStep.Name))
                {
                    customStep.ProcessUpdate(gameTime);
                }
            }

            using (_performanceStatisticsRecorder.RecordStepDuration(_gameLoopSteps.PhysicsStepName))
            {
                _gameLoopSteps.PhysicsStep.PreparePhysicsDebugInformation();
            }

            using (_performanceStatisticsRecorder.RecordStepDuration(_gameLoopSteps.AudioStepName))
            {
                _gameLoopSteps.AudioStep.ProcessAudio();
            }

            using (_performanceStatisticsRecorder.RecordStepDuration(_gameLoopSteps.AnimationStepName))
            {
                _gameLoopSteps.AnimationStep.ProcessAnimations(gameTime);
            }

            using (_performanceStatisticsRecorder.RecordStepDuration(_gameLoopSteps.RenderingStepName))
            {
                _gameLoopSteps.RenderingStep.RenderScene();
            }

            scene.RemoveEntitiesAfterFullFrame();

            _performanceStatisticsRecorder.RecordFrame();
            _coreDiagnosticInfoProvider.UpdateDiagnostics(scene);
        }
    }
}