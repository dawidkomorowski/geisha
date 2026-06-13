using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

// TODO: Test sensor exact-overlap correctness: AABB overlap without shape overlap must not trigger OnOverlapBegin/OnOverlapEnd.
// TODO: Test sensor overlap lifecycle when a body is removed/disposed during active overlap (define and verify OnOverlapEnd policy; no invalid callbacks).
// TODO: Test runtime IsSensor toggling while overlapping and while separated; verify begin/end transitions are correct.
// TODO: Test runtime Enabled toggling for sensor and visitor bodies; verify no ghost overlaps and correct begin/end transitions.
// TODO: Test runtime CollisionLayer/CollisionMask changes during active overlap; verify pair begin/end follows filter changes.
// TODO: Test sensor events with substepping; ensure exactly one begin/end per logical transition across substeps.
// TODO: Test duplicate callback protection: no repeated begin for continuous overlap and no repeated end for continuous separation.
// TODO: Test callback symmetry/order: both participants receive matching OnOverlapBegin and OnOverlapEnd exactly once per transition.
// TODO: Test sensor overlap cache cleanup/index integrity across frames (stale removal + swap-remove updates do not orphan or corrupt pairs).

[TestFixture]
public class SensorTests : PhysicsSystemTestsBase
{
    [TestCase(true, true, false, false, true)]
    [TestCase(true, false, false, true, true)]
    [TestCase(true, true, true, false, true)]
    [TestCase(true, false, true, true, true)]
    [TestCase(true, true, false, true, true)]
    [TestCase(true, true, true, true, true)]
    [TestCase(false, true, false, true, false)]
    public void Sensor_ShouldProduceNoCollision(bool firstBodyIsKinematic, bool firstBodyIsSensor, bool secondBodyIsKinematic, bool secondBodyIsSensor,
        bool expectSensorEvent)
    {
        var physicsSystem = GetPhysicsSystem();
        var firstBody = CreateBody(firstBodyIsKinematic, 0, 0, 100);
        var secondBody = CreateBody(secondBodyIsKinematic, 150, 0, 100);

        var firstBodyCollider = firstBody.GetComponent<CircleColliderComponent>();
        var secondBodyCollider = secondBody.GetComponent<CircleColliderComponent>();
        firstBodyCollider.IsSensor = firstBodyIsSensor;
        secondBodyCollider.IsSensor = secondBodyIsSensor;

        var overlapBeginFromFirstBody = new List<Collider2DComponent>();
        firstBodyCollider.OnOverlapBegin = overlapBeginFromFirstBody.Add;

        // Assume
        Assert.That(firstBodyCollider.IsColliding, Is.False);
        Assert.That(secondBodyCollider.IsColliding, Is.False);

        Assert.That(firstBodyCollider.Enabled, Is.True);
        Assert.That(secondBodyCollider.Enabled, Is.True);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        SaveVisualOutput(physicsSystem);

        Assert.That(firstBodyCollider.IsColliding, Is.False);
        Assert.That(firstBodyCollider.ContactCount, Is.Zero);
        var firstTransform = firstBody.GetComponent<Transform2DComponent>();
        Assert.That(firstTransform.Translation, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(firstTransform.Rotation, Is.Zero);
        Assert.That(firstTransform.Scale, Is.EqualTo(Vector2.One));

        Assert.That(secondBodyCollider.IsColliding, Is.False);
        Assert.That(secondBodyCollider.ContactCount, Is.Zero);
        var secondTransform = secondBody.GetComponent<Transform2DComponent>();
        Assert.That(secondTransform.Translation, Is.EqualTo(new Vector2(150, 0)));
        Assert.That(secondTransform.Rotation, Is.Zero);
        Assert.That(secondTransform.Scale, Is.EqualTo(Vector2.One));

        if (expectSensorEvent)
        {
            Assert.That(overlapBeginFromFirstBody, Has.Count.EqualTo(1));
            Assert.That(overlapBeginFromFirstBody[0], Is.EqualTo(secondBodyCollider));
        }
        else
        {
            Assert.That(overlapBeginFromFirstBody, Has.Count.Zero);
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public void Sensor_ShouldInvoke_OnOverlapBegin_And_OnOverlapEnd(bool sensorIsKinematic)
    {
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(-100, 0, 100);
        var sensorBody = CreateBody(sensorIsKinematic, 150, 0, 100);

        var overlapBeginFromKinematic = new List<Collider2DComponent>();
        var overlapBeginFromSensor = new List<Collider2DComponent>();
        var overlapEndFromKinematic = new List<Collider2DComponent>();
        var overlapEndFromSensor = new List<Collider2DComponent>();

        var kinematicCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        kinematicCollider.OnOverlapBegin = overlapBeginFromKinematic.Add;
        kinematicCollider.OnOverlapEnd = overlapEndFromKinematic.Add;

        var sensorCollider = sensorBody.GetComponent<CircleColliderComponent>();
        sensorCollider.OnOverlapBegin = overlapBeginFromSensor.Add;
        sensorCollider.OnOverlapEnd = overlapEndFromSensor.Add;
        sensorCollider.IsSensor = true;

        var kinematicTransform = kinematicBody.GetComponent<Transform2DComponent>();

        // Act 0
        physicsSystem.ProcessPhysics(); // No overlap -> should invoke no overlap callbacks.
        SaveVisualOutput(physicsSystem, 0);

        // Assert 0
        Assert.That(overlapBeginFromKinematic, Is.Empty);
        Assert.That(overlapBeginFromSensor, Is.Empty);
        Assert.That(overlapEndFromKinematic, Is.Empty);
        Assert.That(overlapEndFromSensor, Is.Empty);

        // Act 1
        kinematicTransform.Translation = new Vector2(0, 0);

        physicsSystem.ProcessPhysics(); // Overlap detected -> should invoke OnOverlapBegin.
        SaveVisualOutput(physicsSystem, 1);

        // Assert 1
        Assert.That(overlapBeginFromKinematic, Has.Count.EqualTo(1));
        Assert.That(overlapBeginFromKinematic[0], Is.EqualTo(sensorCollider));

        Assert.That(overlapBeginFromSensor, Has.Count.EqualTo(1));
        Assert.That(overlapBeginFromSensor[0], Is.EqualTo(kinematicCollider));

        Assert.That(overlapEndFromKinematic, Is.Empty);
        Assert.That(overlapEndFromSensor, Is.Empty);

        // Act 2
        physicsSystem.ProcessPhysics(); // Overlap stays -> should invoke no overlap callbacks.
        SaveVisualOutput(physicsSystem, 2);

        // Assert 2
        Assert.That(overlapBeginFromKinematic, Has.Count.EqualTo(1));
        Assert.That(overlapBeginFromSensor, Has.Count.EqualTo(1));
        Assert.That(overlapEndFromKinematic, Is.Empty);
        Assert.That(overlapEndFromSensor, Is.Empty);

        // Act 3
        kinematicTransform.Translation = new Vector2(-100, 0);

        physicsSystem.ProcessPhysics(); // Overlap removed -> should invoke OnOverlapEnd.
        SaveVisualOutput(physicsSystem, 3);

        // Assert 3
        Assert.That(overlapBeginFromKinematic, Has.Count.EqualTo(1));
        Assert.That(overlapBeginFromSensor, Has.Count.EqualTo(1));

        Assert.That(overlapEndFromKinematic, Has.Count.EqualTo(1));
        Assert.That(overlapEndFromKinematic[0], Is.EqualTo(sensorCollider));

        Assert.That(overlapEndFromSensor, Has.Count.EqualTo(1));
        Assert.That(overlapEndFromSensor[0], Is.EqualTo(kinematicCollider));

        // Act 4
        physicsSystem.ProcessPhysics(); // No overlap -> should invoke no overlap callbacks.
        SaveVisualOutput(physicsSystem, 4);

        // Assert 4
        Assert.That(overlapBeginFromKinematic, Has.Count.EqualTo(1));
        Assert.That(overlapBeginFromSensor, Has.Count.EqualTo(1));
        Assert.That(overlapEndFromKinematic, Has.Count.EqualTo(1));
        Assert.That(overlapEndFromSensor, Has.Count.EqualTo(1));
    }

    private Entity CreateBody(bool isKinematic, double x, double y, double radius)
    {
        return isKinematic
            ? CreateCircleKinematicBody(x, y, radius)
            : CreateCircleStaticBody(x, y, radius);
    }
}