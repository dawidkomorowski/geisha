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
        // Arrange
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
}