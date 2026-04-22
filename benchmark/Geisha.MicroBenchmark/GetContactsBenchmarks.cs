using System;
using System.Buffers;
using System.Collections.Generic;
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

/// <summary>
///     Focused micro benchmark for <see cref="Collider2DComponent.GetContacts" /> API.
///     The scene is intentionally dense so every collider has multiple contacts.
///     This benchmark is temporary and targets the memory-allocation optimization tracked
///     in issue 649-optimize-memory-allocation-of-collider2dcomponentgetcontacts.
/// </summary>
[MemoryDiagnoser]
public class GetContactsBenchmarks
{
    // Grid layout: GridSize x GridSize circles placed with spacing < 2*Radius so they all overlap.
    private const int GridSize = 20;
    private const double Radius = 10.0;
    private const double Spacing = 15.0; // < 2*Radius => guaranteed overlap with neighbours

    private Scene _scene = null!;
    private PhysicsSystem _physicsSystem = null!;
    private readonly List<Collider2DComponent> _colliders = new();

    [GlobalSetup]
    public void GlobalSetup()
    {
        _colliders.Clear();

        _scene = TestSceneFactory.Create();
        var timeSystem = new TimeSystem(new CoreConfiguration { FixedUpdatesPerSecond = 60 });
        var debugRenderer = TestKit.CreateDebugRenderer();
        var physicsConfiguration = new PhysicsConfiguration { EnableDebugRendering = false };
        _physicsSystem = new PhysicsSystem(physicsConfiguration, timeSystem, debugRenderer);
        _scene.AddObserver(_physicsSystem);

        // Build a dense grid of overlapping circle kinematic bodies.
        // Kinematic vs kinematic collision is detected, guaranteeing contacts are generated.
        for (var row = 0; row < GridSize; row++)
        {
            for (var col = 0; col < GridSize; col++)
            {
                var x = col * Spacing;
                var y = row * Spacing;

                var entity = _scene.CreateEntity();
                var transform = entity.CreateComponent<Transform2DComponent>();
                transform.Translation = new Vector2(x, y);

                var collider = entity.CreateComponent<CircleColliderComponent>();
                collider.Radius = Radius;

                entity.CreateComponent<KinematicRigidBody2DComponent>();

                _colliders.Add(collider);
            }
        }

        // Run one physics step to generate contacts between all overlapping colliders.
        _physicsSystem.ProcessPhysics();
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _scene.RemoveObserver(_physicsSystem);
        _physicsSystem = null!;
        _scene = null!;
    }

    /// <summary>
    ///     Baseline: calls <see cref="Collider2DComponent.GetContacts()" /> on every collider in the scene once.
    /// </summary>
    [Benchmark(Baseline = true)]
    public int GetContacts_AllColliders()
    {
        var totalContacts = 0;
        foreach (var collider in _colliders)
        {
            totalContacts += collider.GetContacts().Length;
        }

        return totalContacts;
    }

    [Benchmark]
    public int GetContacts_AllColliders_Array()
    {
        var maxContacts = 0;

        foreach (var collider in _colliders)
        {
            maxContacts = Math.Max(maxContacts, collider.ContactCount);
        }

        var contacts = new Contact2D[maxContacts];

        var totalContacts = 0;
        foreach (var collider in _colliders)
        {
            collider.GetContacts(contacts);
            totalContacts += contacts.Length;
        }

        return totalContacts;
    }

    [Benchmark]
    public int GetContacts_AllColliders_List()
    {
        var maxContacts = 0;

        var contacts = new List<Contact2D>();

        var totalContacts = 0;
        foreach (var collider in _colliders)
        {
            collider.GetContacts(contacts);
            totalContacts += contacts.Count;
        }

        return totalContacts;
    }
}