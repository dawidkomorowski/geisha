using System.ComponentModel.Composition;
using System.Diagnostics;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core
{
    public interface IGameLoop
    {
        void Update();
    }

    [Export(typeof(IGameLoop))]
    public class GameLoop : IGameLoop
    {
        private readonly ICoreDiagnosticsInfoProvider _coreDiagnosticsInfoProvider;
        private readonly IDeltaTimeProvider _deltaTimeProvider;
        private readonly IFixedDeltaTimeProvider _fixedDeltaTimeProvider;
        private readonly ISceneManager _sceneManager;
        private readonly ISystemsProvider _systemsProvider;

        private double _accumulator;

        [ImportingConstructor]
        public GameLoop(ISystemsProvider systemsProvider, IDeltaTimeProvider deltaTimeProvider,
            IFixedDeltaTimeProvider fixedDeltaTimeProvider, ISceneManager sceneManager, ICoreDiagnosticsInfoProvider coreDiagnosticsInfoProvider)
        {
            _systemsProvider = systemsProvider;
            _deltaTimeProvider = deltaTimeProvider;
            _fixedDeltaTimeProvider = fixedDeltaTimeProvider;
            _sceneManager = sceneManager;
            _coreDiagnosticsInfoProvider = coreDiagnosticsInfoProvider;

            PerformanceMonitor.Reset();
        }

        public void Update()
        {
            var scene = _sceneManager.CurrentScene;
            var deltaTime = _deltaTimeProvider.GetDeltaTime();
            var fixedDeltaTime = _fixedDeltaTimeProvider.GetFixedDeltaTime();
            var variableUpdateSystems = _systemsProvider.GetVariableUpdateSystems();
            var fixedUpdateSystems = _systemsProvider.GetFixedUpdateSystems();

            _accumulator += deltaTime;

            while (_accumulator >= fixedDeltaTime)
            {
                foreach (var system in fixedUpdateSystems)
                    PerformanceMonitor.RecordFixedSystemExecution(system, () => system.FixedUpdate(scene));

                _accumulator -= fixedDeltaTime;
            }

            foreach (var system in variableUpdateSystems)
                PerformanceMonitor.RecordVariableSystemExecution(system, () => system.Update(scene, deltaTime));

            PerformanceMonitor.AddFrame();

            if (PerformanceMonitor.TotalFrames % 100 == 0) PrintPerformanceStatistics();
            _coreDiagnosticsInfoProvider.UpdateDiagnostics(scene);
        }

        private static void PrintPerformanceStatistics()
        {
            Debug.WriteLine($"FPS: {PerformanceMonitor.RealFps}, TotalFrames: {PerformanceMonitor.TotalFrames}");
            Debug.WriteLine($"Systems share:");
            foreach (var info in PerformanceMonitor.GetTotalSystemsShare())
                Debug.WriteLine($"{info.Key}: {info.Value}%");
        }
    }
}