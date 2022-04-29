using System;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core
{
    internal interface IGameLoop
    {
        void Update();
    }

    internal sealed class GameLoop : IGameLoop
    {
        private readonly ICoreDiagnosticInfoProvider _coreDiagnosticInfoProvider;
        private readonly IGameTimeProvider _gameTimeProvider;
        private readonly IEngineSystems _engineSystems;
        private readonly ISceneManagerForGameLoop _sceneManager;
        private readonly IPerformanceStatisticsRecorder _performanceStatisticsRecorder;
        private readonly int _fixedUpdatesPerFrameLimit;

        private TimeSpan _timeToSimulate;

        public GameLoop(
            ICoreDiagnosticInfoProvider coreDiagnosticInfoProvider,
            IGameTimeProvider gameTimeProvider,
            IEngineSystems engineSystems,
            ISceneManagerForGameLoop sceneManager,
            IPerformanceStatisticsRecorder performanceStatisticsRecorder,
            CoreConfiguration configuration)
        {
            _coreDiagnosticInfoProvider = coreDiagnosticInfoProvider;
            _gameTimeProvider = gameTimeProvider;
            _engineSystems = engineSystems;
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
                using (_performanceStatisticsRecorder.RecordSystemExecution(_engineSystems.InputSystemName))
                {
                    _engineSystems.InputSystem.ProcessInput();
                }

                using (_performanceStatisticsRecorder.RecordSystemExecution(_engineSystems.BehaviorSystemName))
                {
                    _engineSystems.BehaviorSystem.ProcessBehaviorFixedUpdate();
                }

                foreach (var customSystem in _engineSystems.CustomSystems)
                {
                    using (_performanceStatisticsRecorder.RecordSystemExecution(customSystem.Name))
                    {
                        customSystem.ProcessFixedUpdate(scene);
                    }
                }

                using (_performanceStatisticsRecorder.RecordSystemExecution(_engineSystems.PhysicsSystemName))
                {
                    _engineSystems.PhysicsSystem.ProcessPhysics();
                }

                scene.RemoveEntitiesAfterFixedTimeStep();

                _timeToSimulate -= GameTime.FixedDeltaTime;
                fixedUpdatesPerFrame++;
            }

            using (_performanceStatisticsRecorder.RecordSystemExecution(_engineSystems.BehaviorSystemName))
            {
                _engineSystems.BehaviorSystem.ProcessBehaviorUpdate(gameTime);
            }

            foreach (var customSystem in _engineSystems.CustomSystems)
            {
                using (_performanceStatisticsRecorder.RecordSystemExecution(customSystem.Name))
                {
                    customSystem.ProcessUpdate(scene, gameTime);
                }
            }

            using (_performanceStatisticsRecorder.RecordSystemExecution(_engineSystems.PhysicsSystemName))
            {
                _engineSystems.PhysicsSystem.PreparePhysicsDebugInformation();
            }

            using (_performanceStatisticsRecorder.RecordSystemExecution(_engineSystems.AudioSystemName))
            {
                _engineSystems.AudioSystem.ProcessAudio();
            }

            using (_performanceStatisticsRecorder.RecordSystemExecution(_engineSystems.AnimationSystemName))
            {
                _engineSystems.AnimationSystem.ProcessAnimations(gameTime);
            }

            using (_performanceStatisticsRecorder.RecordSystemExecution(_engineSystems.RenderingSystemName))
            {
                _engineSystems.RenderingSystem.RenderScene(scene);
            }

            scene.RemoveEntitiesAfterFullFrame();

            _performanceStatisticsRecorder.RecordFrame();
            _coreDiagnosticInfoProvider.UpdateDiagnostics(scene);
        }
    }
}