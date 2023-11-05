using System;
using BenchmarkDotNet.Attributes;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;
using Geisha.TestUtils;

namespace Geisha.MicroBenchmark;

[MemoryDiagnoser]
public class EntityBenchmarks
{
    private Scene _scene = null!;
    protected Entity Entity = null!;

    //[Params(1, 3, 5, 7)]
    [Params(1)]
    public int ComponentsCount { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _scene = TestSceneFactory.Create();
        Entity = _scene.CreateEntity();

        switch (ComponentsCount)
        {
            case 1:
                Entity.CreateComponent<Transform2DComponent>();
                break;
            case 3:
                Entity.CreateComponent<SpriteRendererComponent>();
                Entity.CreateComponent<SpriteAnimationComponent>();
                Entity.CreateComponent<Transform2DComponent>();
                break;
            case 5:
                Entity.CreateComponent<SpriteRendererComponent>();
                Entity.CreateComponent<SpriteAnimationComponent>();
                Entity.CreateComponent<AudioSourceComponent>();
                Entity.CreateComponent<RectangleColliderComponent>();
                Entity.CreateComponent<Transform2DComponent>();
                break;
            case 7:
                Entity.CreateComponent<SpriteRendererComponent>();
                Entity.CreateComponent<SpriteAnimationComponent>();
                Entity.CreateComponent<AudioSourceComponent>();
                Entity.CreateComponent<RectangleColliderComponent>();
                Entity.CreateComponent<CameraComponent>();
                Entity.CreateComponent<InputComponent>();
                Entity.CreateComponent<Transform2DComponent>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(ComponentsCount), "Unsupported number of components.");
        }
    }
}

[MemoryDiagnoser]
public class HasComponentEntityBenchmarks : EntityBenchmarks
{
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("HasComponent")]
    public bool HasComponent_Baseline()
    {
        return Entity.HasComponentBaseline<Transform2DComponent>();
    }

    [Benchmark]
    [BenchmarkCategory("HasComponent")]
    public bool HasComponent_Loop()
    {
        return Entity.HasComponentLoop<Transform2DComponent>();
    }

    [Benchmark]
    [BenchmarkCategory("HasComponent")]
    public bool HasComponent_Dict()
    {
        return Entity.HasComponent<Transform2DComponent>();
    }
}

[MemoryDiagnoser]
public class GetComponentEntityBenchmarks : EntityBenchmarks
{
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("GetComponent")]
    public Transform2DComponent GetComponent_Baseline()
    {
        return Entity.GetComponentBaseline<Transform2DComponent>();
    }

    [Benchmark]
    [BenchmarkCategory("GetComponent")]
    public Transform2DComponent GetComponent_Loop()
    {
        return Entity.GetComponentLoop<Transform2DComponent>();
    }

    [Benchmark]
    [BenchmarkCategory("GetComponent")]
    public Transform2DComponent GetComponent_Dict()
    {
        return Entity.GetComponent<Transform2DComponent>();
    }
}

[MemoryDiagnoser]
public class GetComponentsEntityBenchmarks : EntityBenchmarks
{
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("GetComponents")]
    public int GetComponentsAndIterate_Baseline()
    {
        var i = 0;
        foreach (var transform2DComponent in Entity.GetComponentsBaseline<Transform2DComponent>())
        {
            i++;
        }

        return i;
    }

    [Benchmark]
    [BenchmarkCategory("GetComponents")]
    public int GetComponentsAndIterate_Loop()
    {
        var i = 0;
        foreach (var transform2DComponent in Entity.GetComponentsLoop<Transform2DComponent>())
        {
            i++;
        }

        return i;
    }

    [Benchmark]
    [BenchmarkCategory("GetComponents")]
    public int GetComponentsAndIterate_Dict()
    {
        var i = 0;
        foreach (var transform2DComponent in Entity.GetComponents<Transform2DComponent>())
        {
            i++;
        }

        return i;
    }
}

[MemoryDiagnoser]
public class ChildrenEntityBenchmarks : EntityBenchmarks
{
    [Benchmark(Baseline = true)]
    public int Children_Baseline()
    {
        var limit = 100;
        while (Entity.ComponentsBaseline.Count != 0)
        {
            limit--;

            if(limit == 0) break;
        }

        return Entity.ComponentsBaseline.Count;
    }

    [Benchmark]
    public int Children_Fast()
    {
        var limit = 100;
        while (Entity.Components.Count != 0)
        {
            limit--;

            if (limit == 0) break;
        }

        return Entity.Components.Count;
    }
}