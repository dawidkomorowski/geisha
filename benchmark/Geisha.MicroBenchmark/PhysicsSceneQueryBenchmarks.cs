using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.Systems;
using Geisha.TestUtils;

namespace Geisha.MicroBenchmark;

[MemoryDiagnoser]
public class PhysicsSceneQueryBenchmarks
{
    private const int StaticBodiesCount = 10000;
    private const double QueryVisualScale = 0.5;

    private Scene _scene = null!;
    private PhysicsSystem _physicsSystem = null!;
    private IDebugRendererForTests _debugRenderer = null!;

    private readonly AABB2D _aabbToQuery = AABB2D.FromCenterAndSize(0, 0, 700, 500);
    private readonly Collider2DComponent[] _collidersArray = new Collider2DComponent[2048];
    private readonly List<Collider2DComponent> _collidersList = new(2048);

    [GlobalSetup]
    public void GlobalSetup()
    {
        _scene = TestSceneFactory.Create();

        var timeSystem = new TimeSystem(new CoreConfiguration { FixedUpdatesPerSecond = 60 });
        _debugRenderer = TestKit.CreateDebugRenderer();
        var physicsConfiguration = new PhysicsConfiguration
        {
            EnableDebugRendering = true
        };
        _physicsSystem = new PhysicsSystem(physicsConfiguration, timeSystem, _debugRenderer);
        _scene.AddObserver(_physicsSystem);

        ConfigureStaticScene(new SizeD(6000, 6000), StaticBodiesCount);
        _physicsSystem.SynchronizePhysicsState();

        SaveVisualOutput("PhysicsSceneQueryBenchmarks[0]");
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        SaveVisualOutput("PhysicsSceneQueryBenchmarks[1]");

        _scene.RemoveObserver(_physicsSystem);
        _physicsSystem.Dispose();
        _physicsSystem = null!;
        _scene = null!;
        _debugRenderer.Dispose();
    }

    [Benchmark]
    public int QueryBounds_Span()
    {
        return _physicsSystem.QueryBounds(_aabbToQuery, _collidersArray);
    }

    [Benchmark]
    public int QueryBounds_List()
    {
        return _physicsSystem.QueryBounds(_aabbToQuery, _collidersList);
    }

    [Benchmark]
    public int QueryOverlap_AxisAlignedRectangle_Span()
    {
        return _physicsSystem.QueryOverlap(_aabbToQuery.ToAxisAlignedRectangle(), _collidersArray);
    }

    [Benchmark]
    public int QueryOverlap_AxisAlignedRectangle_List()
    {
        return _physicsSystem.QueryOverlap(_aabbToQuery.ToAxisAlignedRectangle(), _collidersList);
    }

    private void ConfigureStaticScene(SizeD bounds, int staticBodies)
    {
        var random = new Random(560);

        for (var i = 0; i < staticBodies; i++)
        {
            var position = RandomPositionInBounds(random, bounds);

            if (random.Next(0, 2) == 0)
            {
                CreateRectangleStaticBody(position, random.Next(5, 50), random.Next(5, 50));
            }
            else
            {
                CreateCircleStaticBody(position, random.Next(5, 25));
            }
        }
    }

    private void CreateRectangleStaticBody(Vector2 position, double width, double height)
    {
        var entity = _scene.CreateEntity();
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = position;

        var rectangleColliderComponent = entity.CreateComponent<RectangleColliderComponent>();
        rectangleColliderComponent.Dimensions = new Vector2(width, height);
    }

    private void CreateCircleStaticBody(Vector2 position, double radius)
    {
        var entity = _scene.CreateEntity();
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = position;

        var circleColliderComponent = entity.CreateComponent<CircleColliderComponent>();
        circleColliderComponent.Radius = radius;
    }

    private void SaveVisualOutput(string fileName)
    {
        _debugRenderer.BeginDraw(QueryVisualScale);
        _physicsSystem.SynchronizePhysicsState();
        _physicsSystem.PreparePhysicsDebugInformation();
        _debugRenderer.DrawRectangle(_aabbToQuery.ToAxisAlignedRectangle(), Color.Red, Matrix3x3.Identity);

        var outputPath = Path.Combine("..", "..", "..", "..");
        _debugRenderer.EndDraw(outputPath, fileName);
    }

    private static Vector2 RandomPositionInBounds(Random random, SizeD bounds)
    {
        return new Vector2(
            random.NextDouble() * bounds.Width - bounds.Width / 2,
            random.NextDouble() * bounds.Height - bounds.Height / 2
        );
    }
}