using System;
using BenchmarkDotNet.Attributes;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.Systems;
using Geisha.Engine.Rendering;
using Geisha.TestUtils;

namespace Geisha.MicroBenchmark;

[MemoryDiagnoser]
public class PhysicsSystemBenchmarks
{
    private Scene _scene = null!;
    private PhysicsSystem _physicsSystem = null!;

    [IterationSetup(Target = nameof(SimulatePhysics_10Seconds))]
    public void IterationSetup_ProcessPhysics()
    {
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(1d / 60d);
        _scene = TestSceneFactory.Create();
        _physicsSystem = new PhysicsSystem(new PhysicsConfiguration(), new DebugRenderer());
        _scene.AddObserver(_physicsSystem);

        var random = new Random(0);

        // Create 200 kinematic bodies.
        for (var i = 0; i < 200; i++)
        {
            var x = random.NextDouble() * 10000 - 5000;
            var y = random.NextDouble() * 10000 - 5000;

            if (random.Next(0, 2) == 0)
            {
                CreateRectangleKinematicBody(
                    x: x,
                    y: y,
                    width: random.Next(5, 50),
                    height: random.Next(5, 50),
                    linearVelocity: new Vector2(random.Next(-100, 100), random.Next(-100, 100)),
                    angularVelocity: random.NextDouble() * 4 - 2
                );
            }
            else
            {
                CreateCircleKinematicBody(
                    x: x,
                    y: y,
                    radius: random.Next(5, 25),
                    linearVelocity: new Vector2(random.Next(-100, 100), random.Next(-100, 100)),
                    angularVelocity: random.NextDouble() * 4 - 2
                );
            }
        }

        // Create 1000 static bodies.
        for (var i = 0; i < 1000; i++)
        {
            var x = random.NextDouble() * 10000 - 5000;
            var y = random.NextDouble() * 10000 - 5000;

            if (random.Next(0, 2) == 0)
            {
                CreateRectangleStaticBody(x, y, random.Next(5, 50), random.Next(5, 50));
            }
            else
            {
                CreateCircleStaticBody(x, y, random.Next(5, 25));
            }
        }
    }

    [Benchmark]
    public void SimulatePhysics_10Seconds()
    {
        // Assuming 60FPS it simulates 10s.
        for (var i = 0; i < 600; i++)
        {
            _physicsSystem.ProcessPhysics();
        }
    }

    private void CreateRectangleStaticBody(double x, double y, double width, double height)
    {
        var entity = _scene.CreateEntity();
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(x, y);

        var rectangleColliderComponent = entity.CreateComponent<RectangleColliderComponent>();
        rectangleColliderComponent.Dimensions = new Vector2(width, height);
    }

    private void CreateCircleStaticBody(double x, double y, double radius)
    {
        var entity = _scene.CreateEntity();
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(x, y);

        var circleColliderComponent = entity.CreateComponent<CircleColliderComponent>();
        circleColliderComponent.Radius = radius;
    }

    private void CreateRectangleKinematicBody(double x, double y, double width, double height, Vector2 linearVelocity, double angularVelocity)
    {
        var entity = _scene.CreateEntity();
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(x, y);

        var rectangleColliderComponent = entity.CreateComponent<RectangleColliderComponent>();
        rectangleColliderComponent.Dimensions = new Vector2(width, height);

        var kinematicRigidBody2DComponent = entity.CreateComponent<KinematicRigidBody2DComponent>();
        kinematicRigidBody2DComponent.LinearVelocity = linearVelocity;
        kinematicRigidBody2DComponent.AngularVelocity = angularVelocity;
    }

    private void CreateCircleKinematicBody(double x, double y, double radius, Vector2 linearVelocity, double angularVelocity)
    {
        var entity = _scene.CreateEntity();
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(x, y);

        var circleColliderComponent = entity.CreateComponent<CircleColliderComponent>();
        circleColliderComponent.Radius = radius;

        var kinematicRigidBody2DComponent = entity.CreateComponent<KinematicRigidBody2DComponent>();
        kinematicRigidBody2DComponent.LinearVelocity = linearVelocity;
        kinematicRigidBody2DComponent.AngularVelocity = angularVelocity;
    }
}