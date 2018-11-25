using System;
using System.Diagnostics;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core
{
    internal interface IGameLoop
    {
        void Update();
    }

    internal class GameLoop : IGameLoop
    {
        private readonly ICoreDiagnosticsInfoProvider _coreDiagnosticsInfoProvider;
        private readonly IGameTimeProvider _gameTimeProvider;
        private readonly ISceneManager _sceneManager;
        private readonly ISystemsProvider _systemsProvider;

        private TimeSpan _notSimulatedTime;

        public GameLoop(ISystemsProvider systemsProvider, IGameTimeProvider gameTimeProvider, ISceneManager sceneManager,
            ICoreDiagnosticsInfoProvider coreDiagnosticsInfoProvider)
        {
            _systemsProvider = systemsProvider;
            _gameTimeProvider = gameTimeProvider;
            _sceneManager = sceneManager;
            _coreDiagnosticsInfoProvider = coreDiagnosticsInfoProvider;

            PerformanceMonitor.Reset();
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
                    PerformanceMonitor.RecordSystemExecution(system, () => system.FixedUpdate(scene));
                }

                _notSimulatedTime -= GameTime.FixedDeltaTime;
            }

            foreach (var system in variableTimeStepSystems)
            {
                PerformanceMonitor.RecordSystemExecution(system, () => system.Update(scene, gameTime));
            }

            PerformanceMonitor.AddFrame();

            if (PerformanceMonitor.TotalFrames % 100 == 0) PrintPerformanceStatistics();
            _coreDiagnosticsInfoProvider.UpdateDiagnostics(scene);
        }

        private static void PrintPerformanceStatistics()
        {
            // TODO how to present it better?
            Debug.WriteLine($"FPS: {PerformanceMonitor.RealFps}, TotalFrames: {PerformanceMonitor.TotalFrames}");
            Debug.WriteLine("Systems share:");
            foreach (var info in PerformanceMonitor.GetNLastFramesSystemsShare(100))
            {
                Debug.WriteLine($"{info.Key}: {info.Value}%");
            }
        }
    }
}