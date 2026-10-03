using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Animation.Systems;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Geisha.TestUtils;

namespace Geisha.MicroBenchmark;

[MemoryDiagnoser]
public class AnimationSystemBenchmarks
{
    private Scene _scene = null!;
    private AnimationSystem _animationSystem = null!;
    private readonly TimeStep _timeStep = new(TimeSpan.FromSeconds(1d / 60d));
    private readonly ITexture _texture = new FakeTexture();

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

    private void CreateAnimations()
    {
        for (var i = 0; i < 10_000; i++)
        {
            CreateAnimationEntity();
        }
    }

    [IterationSetup]
    public void IterationSetup()
    {
        InitializeAnimationSystem();
        CreateAnimations();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        CleanupAnimationSystem();
    }

    [Benchmark]
    public void ProcessAnimations_10_Seconds_10_000_Animations()
    {
        // Assuming 60FPS it simulates 10s.
        for (var i = 0; i < 600; i++)
        {
            _animationSystem.ProcessAnimations(_timeStep);
        }
    }

    private void CreateAnimationEntity()
    {
        var entity = _scene.CreateEntity();
        entity.CreateComponent<SpriteRendererComponent>();
        var spriteAnimationComponent = entity.CreateComponent<SpriteAnimationComponent>();

        spriteAnimationComponent.PlayInLoop = true;
        spriteAnimationComponent.AddAnimation("BenchmarkAnimation", CreateAnimation());
        spriteAnimationComponent.PlayAnimation("BenchmarkAnimation");
    }

    private SpriteAnimation CreateAnimation()
    {
        var frames = ImmutableArray.CreateBuilder<SpriteAnimationFrame>();

        for (var i = 0; i < 10; i++)
        {
            frames.Add(new SpriteAnimationFrame(CreateSprite(), 1));
        }

        return new SpriteAnimation(frames.ToImmutable(), TimeSpan.FromSeconds(1));
    }

    private Sprite CreateSprite()
    {
        return new Sprite(
            _texture,
            new Vector2(0, 0),
            new Vector2(64, 64),
            new Vector2(32, 32),
            1
        );
    }

    private sealed class FakeTexture : ITexture
    {
        public Vector2 Dimensions { get; } = new(64, 64);
        public RuntimeId RuntimeId { get; } = RuntimeId.Next();

        public void Dispose()
        {
        }
    }
}