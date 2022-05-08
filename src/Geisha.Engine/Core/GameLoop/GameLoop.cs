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
                using (_performanceStatisticsRecorder.RecordSystemExecution(_gameLoopSteps.InputSystemName))
                {
                    _gameLoopSteps.InputSystem.ProcessInput();
                }

                using (_performanceStatisticsRecorder.RecordSystemExecution(_gameLoopSteps.BehaviorSystemName))
                {
                    _gameLoopSteps.BehaviorSystem.ProcessBehaviorFixedUpdate();
                }

                foreach (var customSystem in _gameLoopSteps.CustomSystems)
                {
                    using (_performanceStatisticsRecorder.RecordSystemExecution(customSystem.Name))
                    {
                        customSystem.ProcessFixedUpdate();
                    }
                }

                using (_performanceStatisticsRecorder.RecordSystemExecution(_gameLoopSteps.PhysicsSystemName))
                {
                    _gameLoopSteps.PhysicsSystem.ProcessPhysics();
                }

                scene.RemoveEntitiesAfterFixedTimeStep();

                _timeToSimulate -= GameTime.FixedDeltaTime;
                fixedUpdatesPerFrame++;
            }

            using (_performanceStatisticsRecorder.RecordSystemExecution(_gameLoopSteps.BehaviorSystemName))
            {
                _gameLoopSteps.BehaviorSystem.ProcessBehaviorUpdate(gameTime);
            }

            foreach (var customSystem in _gameLoopSteps.CustomSystems)
            {
                using (_performanceStatisticsRecorder.RecordSystemExecution(customSystem.Name))
                {
                    customSystem.ProcessUpdate(gameTime);
                }
            }

            using (_performanceStatisticsRecorder.RecordSystemExecution(_gameLoopSteps.PhysicsSystemName))
            {
                _gameLoopSteps.PhysicsSystem.PreparePhysicsDebugInformation();
            }

            using (_performanceStatisticsRecorder.RecordSystemExecution(_gameLoopSteps.AudioStepName))
            {
                _gameLoopSteps.AudioStep.ProcessAudio();
            }

            using (_performanceStatisticsRecorder.RecordSystemExecution(_gameLoopSteps.AnimationStepName))
            {
                _gameLoopSteps.AnimationStep.ProcessAnimations(gameTime);
            }

            using (_performanceStatisticsRecorder.RecordSystemExecution(_gameLoopSteps.RenderingSystemName))
            {
                _gameLoopSteps.RenderingSystem.RenderScene();
            }

            scene.RemoveEntitiesAfterFullFrame();

            _performanceStatisticsRecorder.RecordFrame();
            _coreDiagnosticInfoProvider.UpdateDiagnostics(scene);
        }
    }
}