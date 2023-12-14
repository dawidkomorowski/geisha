using System.Linq;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class KinematicRigidBodyAndStaticRigidBodyCollisionTests : PhysicsSystemTestsBase
{
    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldCollideWithRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 5);
        var staticBody = CreateRectangleStaticBody(5, 0, 10, 5);

        // Assume
        Assume.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        Assert.That(kinematicBodyCollider.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(kinematicBodyCollider.CollidingEntities.Single(), Is.EqualTo(staticBody));

        var staticBodyCollider = staticBody.GetComponent<RectangleColliderComponent>();
        Assert.That(staticBodyCollider.IsColliding, Is.True);
        Assert.That(staticBodyCollider.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(staticBodyCollider.CollidingEntities.Single(), Is.EqualTo(kinematicBody));
    }
}