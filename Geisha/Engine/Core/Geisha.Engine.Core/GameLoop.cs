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
        private readonly ISceneManager _sceneManager;
        private readonly ISystemsProvider _systemsProvider;
        private readonly IPerformanceStatisticsRecorder _performanceStatisticsRecorder;

        private TimeSpan _notSimulatedTime;

        public GameLoop(ISystemsProvider systemsProvider, IGameTimeProvider gameTimeProvider, ISceneManager sceneManager,
            ICoreDiagnosticInfoProvider coreDiagnosticInfoProvider, IPerformanceStatisticsRecorder performanceStatisticsRecorder)
        {
            _systemsProvider = systemsProvider;
            _gameTimeProvider = gameTimeProvider;
            _sceneManager = sceneManager;
            _coreDiagnosticInfoProvider = coreDiagnosticInfoProvider;
            _performanceStatisticsRecorder = performanceStatisticsRecorder;
        }

        public void Update()
        {
            var scene = _sceneManager.CurrentScene;
            var gameTime = _gameTimeProvider.GetGameTime();
            var variableTimeStepSystems = _systemsProvider.GetVariableTimeStepSystems();
            var fixedTimeStepSystems = _systemsProvider.GetFixedTimeStepSystems();

            _notSimulatedTime += gameTime.DeltaTime;

            while (_notSimulatedTime >= GameTime.FixedDeltaTime)
            {
                foreach (var system in fixedTimeStepSystems)
                {
                    _performanceStatisticsRecorder.RecordSystemExecution(system, () => system.FixedUpdate(scene));
                }

                _notSimulatedTime -= GameTime.FixedDeltaTime;
            }

            foreach (var system in variableTimeStepSystems)
            {
                _performanceStatisticsRecorder.RecordSystemExecution(system, () => system.Update(scene, gameTime));
            }

            _performanceStatisticsRecorder.RecordFrame();
            _coreDiagnosticInfoProvider.UpdateDiagnostics(scene);
        }
    }
}