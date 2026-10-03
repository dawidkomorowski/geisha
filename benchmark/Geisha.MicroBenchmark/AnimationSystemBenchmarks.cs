using System;
using BenchmarkDotNet.Attributes;
using Geisha.Engine.Animation.Systems;
using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;

namespace Geisha.MicroBenchmark;

[MemoryDiagnoser]
public class AnimationSystemBenchmarks
{
    private Scene _scene = null!;
    private AnimationSystem _animationSystem = null!;
    private readonly TimeStep _timeStep = new(TimeSpan.FromSeconds(1d / 60d));

    private void InitializeAnimationSystem()
    {
        _animationSystem = new AnimationSystem();
        _scene = TestSceneFactory.Create();
        _scene.AddObserver(_animationSystem);
    }

    private void CleanupAnimationSystem()
    {
        _scene.RemoveObserver(_animationSystem);
        _scene = null!;
        _animationSystem = null!;
    }

    [IterationSetup]
    public void IterationSetup()
    {
        InitializeAnimationSystem();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        CleanupAnimationSystem();
    }

    [Benchmark]
    public void ProcessAnimations_10_Seconds_0_Animations()
    {
        // Assuming 60FPS it simulates 10s.
        for (var i = 0; i < 600; i++)
        {
            _animationSystem.ProcessAnimations(_timeStep);
        }
    }
}