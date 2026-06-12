using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class SensorTests : PhysicsSystemTestsBase
{
    [TestCase(true, true, false, false)]
    [TestCase(true, false, false, true)]
    [TestCase(true, true, true, false)]
    [TestCase(true, false, true, true)]
    [TestCase(true, true, false, true)]
    [TestCase(true, true, true, true)]
    public void Sensor_ShouldProduceNoCollision(bool firstBodyIsKinematic, bool firstBodyIsSensor, bool secondBodyIsKinematic, bool secondBodyIsSensor)
    {
        var physicsSystem = GetPhysicsSystem();
        var firstBody = CreateBody(firstBodyIsKinematic, 0, 0, 100);
        var secondBody = CreateBody(secondBodyIsKinematic, 150, 0, 100);

        var firstBodyCollider = firstBody.GetComponent<CircleColliderComponent>();
        var secondBodyCollider = secondBody.GetComponent<CircleColliderComponent>();
        firstBodyCollider.IsSensor = firstBodyIsSensor;
        secondBodyCollider.IsSensor = secondBodyIsSensor;

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
    }

    private Entity CreateBody(bool isKinematic, double x, double y, double radius)
    {
        return isKinematic
            ? CreateCircleKinematicBody(x, y, radius)
            : CreateCircleStaticBody(x, y, radius);
    }

    [Test]
    public void Sensor_ShouldInvoke_OnOverlapBegin_And_OnOverlapEnd()
    {
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(-100, 0, 100);
        var sensorBody = CreateCircleStaticBody(150, 0, 100);

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
}