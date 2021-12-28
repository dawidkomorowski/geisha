using System.Collections.Generic;
using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace Benchmark
{
    internal sealed class BenchmarkSystem : ICustomSystem
    {
        private const int FixedFramesPerBenchmark = 600;
        private readonly IEngineManager _engineManager;
        private readonly ISceneManager _sceneManager;
        private readonly BenchmarkResults _benchmarkResults;
        private readonly List<Benchmark> _benchmarks = new List<Benchmark>();
        private int _currentBenchmarkIndex;
        private int _fixedFramesCounter;
        private int _framesCounter;

        private enum BenchmarkStatus
        {
            Pending,
            Running,
            Complete
        }

        private sealed class Benchmark
        {
            public string Name { get; set; }
            public string SceneBehaviorName { get; set; }
            public BenchmarkStatus Status { get; set; }
        }

        public BenchmarkSystem(IEngineManager engineManager, ISceneManager sceneManager)
        {
            _engineManager = engineManager;
            _sceneManager = sceneManager;
            _benchmarkResults = new BenchmarkResults();

            AddBenchmark("Test1", "BenchmarkSceneBehavior");
            AddBenchmark("Test2", "BenchmarkSceneBehavior");
            AddBenchmark("Test3", "BenchmarkSceneBehavior");
        }

        public string Name => nameof(BenchmarkSystem);
        private Benchmark CurrentBenchmark => _benchmarks[_currentBenchmarkIndex];

        public void ProcessFixedUpdate(Scene scene)
        {
            if (CurrentBenchmark.Status != BenchmarkStatus.Running) return;

            _fixedFramesCounter++;

            if (_fixedFramesCounter >= FixedFramesPerBenchmark)
            {
                CompleteRunningBenchmark();
            }
        }

        public void ProcessUpdate(Scene scene, GameTime gameTime)
        {
            if (CurrentBenchmark.Status == BenchmarkStatus.Pending)
            {
                PreparePendingBenchmark();
            }

            if (CurrentBenchmark.Status == BenchmarkStatus.Running)
            {
                _framesCounter++;
            }

            if (CurrentBenchmark.Status == BenchmarkStatus.Complete)
            {
                SaveBenchmarkResults();
                MoveToNextBenchmark();
            }
        }

        private void AddBenchmark(string name, string sceneBehaviorName)
        {
            _benchmarks.Add(new Benchmark { Name = name, SceneBehaviorName = sceneBehaviorName, Status = BenchmarkStatus.Pending });
        }

        private void PreparePendingBenchmark()
        {
            _sceneManager.LoadEmptyScene(CurrentBenchmark.SceneBehaviorName);
            CurrentBenchmark.Status = BenchmarkStatus.Running;
            _fixedFramesCounter = 0;
            _framesCounter = 0;
        }

        private void CompleteRunningBenchmark()
        {
            CurrentBenchmark.Status = BenchmarkStatus.Complete;
        }

        private void SaveBenchmarkResults()
        {
            var result = new BenchmarkResult
            {
                BenchmarkName = CurrentBenchmark.Name,
                Frames = _framesCounter,
                FixedFrames = _fixedFramesCounter
            };
            _benchmarkResults.AddResult(result);
        }

        private void MoveToNextBenchmark()
        {
            if (_benchmarks.Count - 1 > _currentBenchmarkIndex)
            {
                _currentBenchmarkIndex++;
            }
            else
            {
                _engineManager.ScheduleEngineShutdown();
            }
        }
    }
}