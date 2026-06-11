using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class SensorTests : PhysicsSystemTestsBase
{
    [Test]
    public void Sensor_ShouldProduceNoCollision()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(0, 0, 100);
        var staticBody = CreateCircleStaticBody(150, 0, 100);

        var kinematicBodyCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        var staticBodyCollider = staticBody.GetComponent<CircleColliderComponent>();
        staticBodyCollider.IsSensor = true;

        // Assume
        Assert.That(kinematicBodyCollider.IsColliding, Is.False);
        Assert.That(staticBodyCollider.IsColliding, Is.False);

        Assert.That(kinematicBodyCollider.Enabled, Is.True);
        Assert.That(staticBodyCollider.Enabled, Is.True);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        SaveVisualOutput(physicsSystem);

        Assert.That(kinematicBodyCollider.IsColliding, Is.False);
        Assert.That(kinematicBodyCollider.ContactCount, Is.Zero);
        var transform = kinematicBody.GetComponent<Transform2DComponent>();
        Assert.That(transform.Translation, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(transform.Rotation, Is.Zero);
        Assert.That(transform.Scale, Is.EqualTo(Vector2.One));

        Assert.That(staticBodyCollider.IsColliding, Is.False);
        Assert.That(staticBodyCollider.ContactCount, Is.Zero);
    }
}