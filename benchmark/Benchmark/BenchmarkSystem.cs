using System;
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
            public Benchmark(string name, string sceneBehaviorName, BenchmarkStatus status)
            {
                Name = name;
                SceneBehaviorName = sceneBehaviorName;
                Status = status;
            }

            public string Name { get; }
            public string SceneBehaviorName { get; }
            public BenchmarkStatus Status { get; set; }
        }

        public BenchmarkSystem(IEngineManager engineManager, ISceneManager sceneManager)
        {
            _engineManager = engineManager;
            _sceneManager = sceneManager;
            _benchmarkResults = new BenchmarkResults();

            AddBenchmark("Empty scene", "EmptyScene");
            AddBenchmark("10K entities with no components", "EntitiesWithNoComponents");
            AddBenchmark("1000 static sprites", "StaticSprites");
            AddBenchmark("1000 moving sprites", "MovingSprites");
            AddBenchmark("1000 animated sprites", "AnimatedSprites");
            AddBenchmark("200 moving colliders", "MovingColliders");
            AddBenchmark("4000 entities spawned/removed per second", "EntitiesThroughput");
        }

        private Benchmark CurrentBenchmark => _benchmarks[_currentBenchmarkIndex];
        private string CurrentBenchmarkOutOfTotal => $"{_currentBenchmarkIndex + 1}/{_benchmarks.Count}";

        #region Implementation of ICustomSystem

        public string Name => nameof(BenchmarkSystem);

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

        public void OnEntityCreated(Entity entity)
        {
            throw new NotImplementedException();
        }

        public void OnEntityRemoved(Entity entity)
        {
            throw new NotImplementedException();
        }

        public void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
        {
            throw new NotImplementedException();
        }

        public void OnComponentCreated(Component component)
        {
            throw new NotImplementedException();
        }

        public void OnComponentRemoved(Component component)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void AddBenchmark(string name, string sceneBehaviorName)
        {
            Console.WriteLine($"Added benchmark to execute: {name}");

            _benchmarks.Add(new Benchmark(name, sceneBehaviorName, BenchmarkStatus.Pending));
        }

        private void PreparePendingBenchmark()
        {
            Console.WriteLine($"Executing benchmark {CurrentBenchmarkOutOfTotal}: {CurrentBenchmark.Name}");

            _sceneManager.LoadEmptyScene(CurrentBenchmark.SceneBehaviorName);
            CurrentBenchmark.Status = BenchmarkStatus.Running;
            _fixedFramesCounter = 0;
            _framesCounter = 0;
        }

        private void CompleteRunningBenchmark()
        {
            Console.WriteLine($"Completed benchmark {CurrentBenchmarkOutOfTotal}: {CurrentBenchmark.Name}");

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

                // Load empty scene to catch up remaining time to simulate before next benchmark.
                _sceneManager.LoadEmptyScene("BenchmarkSceneBehavior");
            }
            else
            {
                _engineManager.ScheduleEngineShutdown();
            }
        }
    }
}