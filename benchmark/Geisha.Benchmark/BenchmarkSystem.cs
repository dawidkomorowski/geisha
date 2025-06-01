using System;
using System.Collections.Generic;
using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering.Backend;

namespace Geisha.Benchmark
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal sealed class BenchmarkSystem : ICustomSystem
    {
        private const int FixedFramesPerBenchmark = 600;
        private readonly IEngineManager _engineManager;
        private readonly ISceneManager _sceneManager;
        private readonly IRenderingBackend _renderingBackend;
        private readonly BenchmarkResults _benchmarkResults;
        private readonly List<Benchmark> _benchmarks = new();
        private int _currentBenchmarkIndex;
        private int _fixedFramesCounter;
        private int _framesCounter;
        private int _drawCallsCounter;

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

        public BenchmarkSystem(IEngineManager engineManager, ISceneManager sceneManager, IRenderingBackend renderingBackend)
        {
            _engineManager = engineManager;
            _sceneManager = sceneManager;
            _renderingBackend = renderingBackend;
            _benchmarkResults = new BenchmarkResults();

            AddBenchmark("Empty scene", "EmptyScene");
            AddBenchmark("10 000 entities with no components", "EntitiesWithNoComponents");
            AddBenchmark("10 000 static primitives in view", "StaticPrimitivesInView");
            AddBenchmark("10 000 static primitives out of view", "StaticPrimitivesOutOfView");
            AddBenchmark("10 000 moving primitives in view", "MovingPrimitivesInView");
            AddBenchmark("10 000 moving primitives out of view", "MovingPrimitivesOutOfView");
            AddBenchmark("10 000 static sprites in view", "StaticSpritesInView");
            AddBenchmark("10 000 static sprites out of view", "StaticSpritesOutOfView");
            AddBenchmark("10 000 moving sprites in view", "MovingSpritesInView");
            AddBenchmark("10 000 moving sprites out of view", "MovingSpritesOutOfView");
            AddBenchmark("10 000 sprites in 10 batches of 1000 each", "SpriteBatch10X1000");
            AddBenchmark("10 000 sprites in 100 batches of 100 each", "SpriteBatch100X100");
            AddBenchmark("10 000 sprites in 1000 batches of 10 each", "SpriteBatch1000X10");
            AddBenchmark("10 000 sprites in 10000 batches of 1 each", "SpriteBatch10000X1");
            AddBenchmark("10 000 sprites in 10 layers of 1000 each", "SpritesInLayers10X1000");
            AddBenchmark("10 000 sprites in 5 layers of 2000 each", "SpritesInLayers5X2000");
            AddBenchmark("10 000 sprites in 2 layers of 5000 each", "SpritesInLayers2X5000");
            AddBenchmark("10 000 animated sprites", "AnimatedSprites");
            AddBenchmark("1000 static texts in view", "StaticTextInView");
            AddBenchmark("1000 static texts out of view", "StaticTextOutOfView");
            AddBenchmark("1000 moving texts in view", "MovingTextInView");
            AddBenchmark("1000 moving texts out of view", "MovingTextOutOfView");
            AddBenchmark("1000 rotating texts", "RotatingText");
            AddBenchmark("1000 changing texts", "ChangingText");
            AddBenchmark("10 000 static bodies", "StaticBodies");
            AddBenchmark("2000 moving kinematic bodies", "MovingKinematicBodies");
            AddBenchmark("2000 kinematic bodies controlled by behavior", "KinematicBodiesControlledByBehavior");
            AddBenchmark("100 kinematic bodies controlled by behavior and 10 000 static bodies", "StaticAndKinematicBodies");
            AddBenchmark("1580 kinematic bodies dropped on a static body", "1580KinematicBodiesDropped");
            AddBenchmark("4000 entities spawned/removed per second", "EntitiesThroughput");
        }

        private Benchmark CurrentBenchmark => _benchmarks[_currentBenchmarkIndex];
        private string CurrentBenchmarkOutOfTotal => $"{_currentBenchmarkIndex + 1}/{_benchmarks.Count}";

        #region Implementation of ICustomSystem

        public string Name => nameof(BenchmarkSystem);

        public void ProcessFixedUpdate()
        {
            if (CurrentBenchmark.Status != BenchmarkStatus.Running) return;

            _fixedFramesCounter++;

            if (_fixedFramesCounter >= FixedFramesPerBenchmark)
            {
                CompleteRunningBenchmark();
            }
        }

        public void ProcessUpdate(GameTime gameTime)
        {
            switch (CurrentBenchmark.Status)
            {
                case BenchmarkStatus.Pending:
                    PreparePendingBenchmark();
                    return;
                case BenchmarkStatus.Running:
                    _framesCounter++;
                    _drawCallsCounter += _renderingBackend.Statistics.DrawCalls;
                    return;
                case BenchmarkStatus.Complete:
                    SaveBenchmarkResults();
                    MoveToNextBenchmark();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnEntityCreated(Entity entity)
        {
        }

        public void OnEntityRemoved(Entity entity)
        {
        }

        public void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
        {
        }

        public void OnComponentCreated(Component component)
        {
        }

        public void OnComponentRemoved(Component component)
        {
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
            _drawCallsCounter = 0;
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
                FixedFrames = _fixedFramesCounter,
                DrawCalls = _drawCallsCounter,
                AvgDrawCallsPerFrame = _drawCallsCounter / _framesCounter
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