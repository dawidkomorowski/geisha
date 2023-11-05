using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
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
    private Entity _entity = null!;

    [Params(1, 3, 5, 7)]
    public int ComponentsCount { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _scene = TestSceneFactory.Create();
        _entity = _scene.CreateEntity();

        switch (ComponentsCount)
        {
            case 1:
                _entity.CreateComponent<Transform2DComponent>();
                break;
            case 3:
                _entity.CreateComponent<SpriteRendererComponent>();
                _entity.CreateComponent<SpriteAnimationComponent>();
                _entity.CreateComponent<Transform2DComponent>();
                break;
            case 5:
                _entity.CreateComponent<SpriteRendererComponent>();
                _entity.CreateComponent<SpriteAnimationComponent>();
                _entity.CreateComponent<AudioSourceComponent>();
                _entity.CreateComponent<RectangleColliderComponent>();
                _entity.CreateComponent<Transform2DComponent>();
                break;
            case 7:
                _entity.CreateComponent<SpriteRendererComponent>();
                _entity.CreateComponent<SpriteAnimationComponent>();
                _entity.CreateComponent<AudioSourceComponent>();
                _entity.CreateComponent<RectangleColliderComponent>();
                _entity.CreateComponent<CameraComponent>();
                _entity.CreateComponent<InputComponent>();
                _entity.CreateComponent<Transform2DComponent>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(ComponentsCount), "Unsupported number of components.");
        }
    }

    //[Benchmark(Baseline = true)]
    //[Benchmark]
    //[BenchmarkCategory("HasComponent")]
    //public bool HasComponent_Baseline()
    //{
    //    return _entity.HasComponentBaseline<Transform2DComponent>();
    //}

    //[Benchmark]
    //[BenchmarkCategory("HasComponent")]
    //public bool HasComponent_Loop()
    //{
    //    return _entity.HasComponentLoop<Transform2DComponent>();
    //}

    //[Benchmark]
    //[BenchmarkCategory("HasComponent")]
    //public bool HasComponent_Dict()
    //{
    //    return _entity.HasComponent<Transform2DComponent>();
    //}

    ////[Benchmark(Baseline = true)]
    //[Benchmark]
    //[BenchmarkCategory("GetComponent")]
    //public Transform2DComponent GetComponent_Baseline()
    //{
    //    return _entity.GetComponentBaseline<Transform2DComponent>();
    //}

    //[Benchmark]
    //[BenchmarkCategory("GetComponent")]
    //public Transform2DComponent GetComponent_Loop()
    //{
    //    return _entity.GetComponentLoop<Transform2DComponent>();
    //}

    //[Benchmark]
    //[BenchmarkCategory("GetComponent")]
    //public Transform2DComponent GetComponent_Dict()
    //{
    //    return _entity.GetComponent<Transform2DComponent>();
    //}

    //[Benchmark]
    //[BenchmarkCategory("GetComponents")]
    //public IEnumerable<Transform2DComponent> GetComponents_Baseline()
    //{
    //    return _entity.GetComponentsBaseline<Transform2DComponent>();
    //}

    //[Benchmark]
    //[BenchmarkCategory("GetComponents")]
    //public IEnumerable<Transform2DComponent> GetComponents_Loop()
    //{
    //    return _entity.GetComponentsLoop<Transform2DComponent>();
    //}

    //[Benchmark]
    //[BenchmarkCategory("GetComponents")]
    //public IEnumerable<Transform2DComponent> GetComponents_Dict()
    //{
    //    return _entity.GetComponents<Transform2DComponent>();
    //}

    [Benchmark]
    [BenchmarkCategory("GetComponentsAndIterate")]
    public int GetComponentsAndIterate_Baseline()
    {
        var i = 0;
        foreach (var transform2DComponent in _entity.GetComponentsBaseline<Transform2DComponent>())
        {
            i++;
        }

        return i;
    }

    [Benchmark]
    [BenchmarkCategory("GetComponentsAndIterate")]
    public int GetComponentsAndIterate_Loop()
    {
        var i = 0;
        foreach (var transform2DComponent in _entity.GetComponentsLoop<Transform2DComponent>())
        {
            i++;
        }

        return i;
    }

    [Benchmark]
    [BenchmarkCategory("GetComponentsAndIterate")]
    public int GetComponentsAndIterate_Dict()
    {
        var i = 0;
        foreach (var transform2DComponent in _entity.GetComponents<Transform2DComponent>())
        {
            i++;
        }

        return i;
    }
}