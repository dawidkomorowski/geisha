using System.ComponentModel.Composition;
using System.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core
{
    [Export(typeof(IGameLoop))]
    public class GameLoop : IGameLoop
    {
        private readonly ISystemsProvider _systemsProvider;
        private readonly IDeltaTimeProvider _deltaTimeProvider;
        private readonly IFixedDeltaTimeProvider _fixedDeltaTimeProvider;
        private readonly ISceneManager _sceneManager;

        private double _accumulator = 0;

        [ImportingConstructor]
        public GameLoop(ISystemsProvider systemsProvider, IDeltaTimeProvider deltaTimeProvider,
            IFixedDeltaTimeProvider fixedDeltaTimeProvider, ISceneManager sceneManager)
        {
            _systemsProvider = systemsProvider;
            _deltaTimeProvider = deltaTimeProvider;
            _sceneManager = sceneManager;
            _fixedDeltaTimeProvider = fixedDeltaTimeProvider;

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
                {
                    PerformanceMonitor.RecordFixedSystemExecution(system, () => system.FixedUpdate(scene));
                }

                _accumulator -= fixedDeltaTime;
            }

            foreach (var system in variableUpdateSystems)
            {
                PerformanceMonitor.RecordVariableSystemExecution(system, () => system.Update(scene, deltaTime));
            }

            PerformanceMonitor.AddFrame();

            if (PerformanceMonitor.TotalFrames % 100 == 0) PrintPerformanceStatistics();
        }

        private static void PrintPerformanceStatistics()
        {
            Debug.WriteLine($"FPS: {PerformanceMonitor.RealFps}, TotalFrames: {PerformanceMonitor.TotalFrames}");
            Debug.WriteLine($"Systems share:");
            foreach (var info in PerformanceMonitor.GetTotalSystemsShare())
            {
                Debug.WriteLine($"{info.Key}: {info.Value}%");
            }
        }
    }
}