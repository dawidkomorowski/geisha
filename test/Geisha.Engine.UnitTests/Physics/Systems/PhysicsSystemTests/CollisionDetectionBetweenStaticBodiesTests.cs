using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class CollisionDetectionBetweenStaticBodiesTests : PhysicsSystemTestsBase
{
    [Test]
    public void ProcessPhysics_RectangleStaticBodyShouldNotCollideWithOtherRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var staticBody1 = CreateRectangleStaticBody(0, 0, 10, 5);
        var staticBody2 = CreateRectangleStaticBody(5, 0, 10, 5);

        // Assume
        Assert.That(staticBody1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var staticBody1Collider = staticBody1.GetComponent<RectangleColliderComponent>();
        Assert.That(staticBody1Collider.IsColliding, Is.False);
        Assert.That(staticBody1Collider.GetContacts(), Has.Count.Zero);

        var staticBody2Collider = staticBody2.GetComponent<RectangleColliderComponent>();
        Assert.That(staticBody2Collider.IsColliding, Is.False);
        Assert.That(staticBody2Collider.GetContacts(), Has.Count.Zero);
    }

    [Test]
    public void ProcessPhysics_CircleStaticBodyShouldNotCollideWithOtherCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var staticBody1 = CreateCircleStaticBody(0, 0, 10);
        var staticBody2 = CreateCircleStaticBody(5, 0, 10);

        // Assume
        Assert.That(staticBody1.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody2.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var staticBody1Collider = staticBody1.GetComponent<CircleColliderComponent>();
        Assert.That(staticBody1Collider.IsColliding, Is.False);
        Assert.That(staticBody1Collider.GetContacts(), Has.Count.Zero);

        var staticBody2Collider = staticBody2.GetComponent<CircleColliderComponent>();
        Assert.That(staticBody2Collider.IsColliding, Is.False);
        Assert.That(staticBody2Collider.GetContacts(), Has.Count.Zero);
    }

    [Test]
    public void ProcessPhysics_RectangleStaticBodyShouldNotCollideWithCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var staticBody1 = CreateRectangleStaticBody(0, 0, 10, 5);
        var staticBody2 = CreateCircleStaticBody(5, 0, 10);

        // Assume
        Assert.That(staticBody1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody2.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var staticBody1Collider = staticBody1.GetComponent<RectangleColliderComponent>();
        Assert.That(staticBody1Collider.IsColliding, Is.False);
        Assert.That(staticBody1Collider.GetContacts(), Has.Count.Zero);

        var staticBody2Collider = staticBody2.GetComponent<CircleColliderComponent>();
        Assert.That(staticBody2Collider.IsColliding, Is.False);
        Assert.That(staticBody2Collider.GetContacts(), Has.Count.Zero);
    }
}