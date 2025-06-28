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
public class PhysicsSystemBenchmarks
{
    private Scene _scene = null!;
    private PhysicsSystem _physicsSystem = null!;
    private IDebugRendererForTests _debugRenderer = null!;
    private readonly List<KinematicRigidBody2DComponent> _kinematicComponents = new();

    private void InitializePhysicsSystem()
    {
        _kinematicComponents.Clear();

        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(1d / 60d);
        _scene = TestSceneFactory.Create();
        _debugRenderer = TestKit.CreateDebugRenderer();
        var physicsConfiguration = new PhysicsConfiguration
        {
            RenderCollisionGeometry = true
        };
        _physicsSystem = new PhysicsSystem(physicsConfiguration, _debugRenderer);
        _scene.AddObserver(_physicsSystem);
    }

    private void CleanupPhysicsSystem()
    {
        _scene.RemoveObserver(_physicsSystem);
        _physicsSystem = null!;
        _scene = null!;
        _debugRenderer.Dispose();
    }

    [IterationSetup(Target = nameof(SimulatePhysics_10Seconds_200K_1000S_Sparse_CR_Disabled))]
    public void IterationSetup_SimulatePhysics_10Seconds_200K_1000S_Sparse_CR_Disabled()
    {
        InitializePhysicsSystem();
        ConfigureUniformRandomScene(new SizeD(10000, 10000), 200, 1000);
        SaveVisualOutput(_physicsSystem, $"{nameof(SimulatePhysics_10Seconds_200K_1000S_Sparse_CR_Disabled)}[0]", 0.5);
    }

    [IterationCleanup(Target = nameof(SimulatePhysics_10Seconds_200K_1000S_Sparse_CR_Disabled))]
    public void IterationCleanup_SimulatePhysics_10Seconds_200K_1000S_Sparse_CR_Disabled()
    {
        SaveVisualOutput(_physicsSystem, $"{nameof(SimulatePhysics_10Seconds_200K_1000S_Sparse_CR_Disabled)}[1]", 0.5);
        CleanupPhysicsSystem();
    }

    [Benchmark]
    public void SimulatePhysics_10Seconds_200K_1000S_Sparse_CR_Disabled()
    {
        // Assuming 60FPS it simulates 10s.
        for (var i = 0; i < 600; i++)
        {
            _physicsSystem.ProcessPhysics();
        }
    }

    [IterationSetup(Target = nameof(SimulatePhysics_10Seconds_200K_1000S_Dense_CR_Disabled))]
    public void IterationSetup_SimulatePhysics_10Seconds_200K_1000S_Dense_CR_Disabled()
    {
        InitializePhysicsSystem();
        ConfigureUniformRandomScene(new SizeD(1000, 1000), 200, 1000);

        SaveVisualOutput(_physicsSystem, $"{nameof(SimulatePhysics_10Seconds_200K_1000S_Dense_CR_Disabled)}[0]", 0.5);
    }

    [IterationCleanup(Target = nameof(SimulatePhysics_10Seconds_200K_1000S_Dense_CR_Disabled))]
    public void IterationCleanup_SimulatePhysics_10Seconds_200K_1000S_Dense_CR_Disabled()
    {
        SaveVisualOutput(_physicsSystem, $"{nameof(SimulatePhysics_10Seconds_200K_1000S_Dense_CR_Disabled)}[1]", 0.5);
        CleanupPhysicsSystem();
    }

    [Benchmark]
    public void SimulatePhysics_10Seconds_200K_1000S_Dense_CR_Disabled()
    {
        // Assuming 60FPS it simulates 10s.
        for (var i = 0; i < 600; i++)
        {
            _physicsSystem.ProcessPhysics();
        }
    }

    [IterationSetup(Target = nameof(SimulatePhysics_10Seconds_638K_Fall_CR_Enabled))]
    public void IterationSetup_SimulatePhysics_10Seconds_638K_Fall_CR_Enabled()
    {
        InitializePhysicsSystem();

        CreateRectangleStaticBody(new Vector2(0, -300), 1200, 50);
        CreateRectangleStaticBody(new Vector2(-600, 0), 50, 600);
        CreateRectangleStaticBody(new Vector2(600, 0), 50, 600);

        for (var iy = 0; iy < 15; iy++)
        {
            for (var ix = 0; ix < 43; ix++)
            {
                if (ix == 0 && iy % 2 == 1)
                {
                    continue;
                }

                var x = -525 + ix * 25 - (25d / 2d * (iy % 2));
                var y = -200 + iy * 30;

                CreateRectangleKinematicBody(new Vector2(x, y), 20, 20, Vector2.Zero, 0);
            }
        }

        foreach (var kinematicRigidBody2DComponent in _kinematicComponents)
        {
            kinematicRigidBody2DComponent.EnableCollisionResponse = true;
        }

        //Console.WriteLine($"Kinematic bodies: {_kinematicComponents.Count}");

        SaveVisualOutput(_physicsSystem, $"{nameof(SimulatePhysics_10Seconds_638K_Fall_CR_Enabled)}[0]");
    }

    [IterationCleanup(Target = nameof(SimulatePhysics_10Seconds_638K_Fall_CR_Enabled))]
    public void IterationCleanup_SimulatePhysics_10Seconds_638K_Fall_CR_Enabled()
    {
        SaveVisualOutput(_physicsSystem, $"{nameof(SimulatePhysics_10Seconds_638K_Fall_CR_Enabled)}[1]");
        CleanupPhysicsSystem();
    }

    [Benchmark]
    public void SimulatePhysics_10Seconds_638K_Fall_CR_Enabled()
    {
        var gravity = new Vector2(0, -981.0);
        var dt = GameTime.FixedDeltaTime.TotalSeconds;

        // Assuming 60FPS it simulates 10s.
        for (var i = 0; i < 600; i++)
        {
            foreach (var kinematicRigidBody2DComponent in _kinematicComponents)
            {
                kinematicRigidBody2DComponent.LinearVelocity += gravity * dt;
            }

            _physicsSystem.ProcessPhysics();
        }
    }

    private void ConfigureUniformRandomScene(SizeD bounds, int kinematicBodies, int staticBodies)
    {
        var random = new Random(0);

        // Create 200 kinematic bodies.
        for (var i = 0; i < kinematicBodies; i++)
        {
            var position = RandomPositionInBounds(random, bounds);

            if (random.Next(0, 2) == 0)
            {
                CreateRectangleKinematicBody(
                    position,
                    width: random.Next(5, 50),
                    height: random.Next(5, 50),
                    linearVelocity: new Vector2(random.Next(-100, 100), random.Next(-100, 100)),
                    angularVelocity: random.NextDouble() * 4 - 2
                );
            }
            else
            {
                CreateCircleKinematicBody(
                    position,
                    radius: random.Next(5, 25),
                    linearVelocity: new Vector2(random.Next(-100, 100), random.Next(-100, 100)),
                    angularVelocity: random.NextDouble() * 4 - 2
                );
            }
        }

        // Create 1000 static bodies.
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

    private void CreateRectangleKinematicBody(Vector2 position, double width, double height, Vector2 linearVelocity, double angularVelocity)
    {
        var entity = _scene.CreateEntity();
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = position;

        var rectangleColliderComponent = entity.CreateComponent<RectangleColliderComponent>();
        rectangleColliderComponent.Dimensions = new Vector2(width, height);

        var kinematicRigidBody2DComponent = entity.CreateComponent<KinematicRigidBody2DComponent>();
        kinematicRigidBody2DComponent.LinearVelocity = linearVelocity;
        kinematicRigidBody2DComponent.AngularVelocity = angularVelocity;

        _kinematicComponents.Add(kinematicRigidBody2DComponent);
    }

    private void CreateCircleKinematicBody(Vector2 position, double radius, Vector2 linearVelocity, double angularVelocity)
    {
        var entity = _scene.CreateEntity();
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = position;

        var circleColliderComponent = entity.CreateComponent<CircleColliderComponent>();
        circleColliderComponent.Radius = radius;

        var kinematicRigidBody2DComponent = entity.CreateComponent<KinematicRigidBody2DComponent>();
        kinematicRigidBody2DComponent.LinearVelocity = linearVelocity;
        kinematicRigidBody2DComponent.AngularVelocity = angularVelocity;

        _kinematicComponents.Add(kinematicRigidBody2DComponent);
    }

    private void SaveVisualOutput(PhysicsSystem physicsSystem, string fileName, double scale = 1d)
    {
        _debugRenderer.BeginDraw(scale);
        physicsSystem.SynchronizeBodies();
        physicsSystem.PreparePhysicsDebugInformation();

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