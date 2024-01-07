using Geisha.Engine.Core.Components;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class StaticRigidBodyTests : PhysicsSystemTestsBase
{
    [Test]
    public void StaticRigidBody_ShouldBeRemoved_WhenTransform2DComponentRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var staticBody = CreateRectangleStaticBody(0, 0, 10, 5);
        var kinematicBody = CreateRectangleKinematicBody(5, 0, 10, 5);

        var staticRectangleCollider = staticBody.GetComponent<RectangleColliderComponent>();
        var kinematicRectangleCollider = kinematicBody.GetComponent<RectangleColliderComponent>();

        physicsSystem.ProcessPhysics();

        // Assume
        Assume.That(staticRectangleCollider.IsColliding, Is.True);
        Assume.That(kinematicRectangleCollider.IsColliding, Is.True);

        // Act
        staticBody.RemoveComponent(staticBody.GetComponent<Transform2DComponent>());
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(staticRectangleCollider.IsColliding, Is.False);
        Assert.That(staticRectangleCollider.CollidingEntities, Has.Count.Zero);

        Assert.That(kinematicRectangleCollider.IsColliding, Is.False);
        Assert.That(kinematicRectangleCollider.CollidingEntities, Has.Count.Zero);
    }

    [Test]
    public void StaticRigidBody_ShouldBeRemoved_WhenCollider2DComponentRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var staticBody = CreateRectangleStaticBody(0, 0, 10, 5);
        var kinematicBody = CreateRectangleKinematicBody(5, 0, 10, 5);

        var staticRectangleCollider = staticBody.GetComponent<RectangleColliderComponent>();
        var kinematicRectangleCollider = kinematicBody.GetComponent<RectangleColliderComponent>();

        physicsSystem.ProcessPhysics();

        // Assume
        Assume.That(staticRectangleCollider.IsColliding, Is.True);
        Assume.That(kinematicRectangleCollider.IsColliding, Is.True);

        // Act
        staticBody.RemoveComponent(staticRectangleCollider);
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(staticRectangleCollider.IsColliding, Is.False);
        Assert.That(staticRectangleCollider.CollidingEntities, Has.Count.Zero);

        Assert.That(kinematicRectangleCollider.IsColliding, Is.False);
        Assert.That(kinematicRectangleCollider.CollidingEntities, Has.Count.Zero);
    }

    [Test]
    public void ProcessPhysics_RectangleStaticBodyShouldNotCollideWithOtherRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var staticBody1 = CreateRectangleStaticBody(0, 0, 10, 5);
        var staticBody2 = CreateRectangleStaticBody(5, 0, 10, 5);

        // Assume
        Assume.That(staticBody1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(staticBody2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var staticBody1Collider = staticBody1.GetComponent<RectangleColliderComponent>();
        Assert.That(staticBody1Collider.IsColliding, Is.False);
        Assert.That(staticBody1Collider.CollidingEntities, Has.Count.Zero);

        var staticBody2Collider = staticBody2.GetComponent<RectangleColliderComponent>();
        Assert.That(staticBody2Collider.IsColliding, Is.False);
        Assert.That(staticBody2Collider.CollidingEntities, Has.Count.Zero);
    }

    [Test]
    public void ProcessPhysics_CircleStaticBodyShouldNotCollideWithOtherCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var staticBody1 = CreateCircleStaticBody(0, 0, 10);
        var staticBody2 = CreateCircleStaticBody(5, 0, 10);

        // Assume
        Assume.That(staticBody1.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assume.That(staticBody2.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var staticBody1Collider = staticBody1.GetComponent<CircleColliderComponent>();
        Assert.That(staticBody1Collider.IsColliding, Is.False);
        Assert.That(staticBody1Collider.CollidingEntities, Has.Count.Zero);

        var staticBody2Collider = staticBody2.GetComponent<CircleColliderComponent>();
        Assert.That(staticBody2Collider.IsColliding, Is.False);
        Assert.That(staticBody2Collider.CollidingEntities, Has.Count.Zero);
    }

    [Test]
    public void ProcessPhysics_RectangleStaticBodyShouldNotCollideWithCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var staticBody1 = CreateRectangleStaticBody(0, 0, 10, 5);
        var staticBody2 = CreateCircleStaticBody(5, 0, 10);

        // Assume
        Assume.That(staticBody1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(staticBody2.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var staticBody1Collider = staticBody1.GetComponent<RectangleColliderComponent>();
        Assert.That(staticBody1Collider.IsColliding, Is.False);
        Assert.That(staticBody1Collider.CollidingEntities, Has.Count.Zero);

        var staticBody2Collider = staticBody2.GetComponent<CircleColliderComponent>();
        Assert.That(staticBody2Collider.IsColliding, Is.False);
        Assert.That(staticBody2Collider.CollidingEntities, Has.Count.Zero);
    }
}