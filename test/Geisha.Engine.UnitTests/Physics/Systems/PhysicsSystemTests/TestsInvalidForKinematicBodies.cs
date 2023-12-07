using System.Linq;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

// TODO Do something with these tests.
public class TestsInvalidForKinematicBodies : PhysicsSystemTestsBase
{
    [Test]
    [Ignore("This test has no sense for KinematicRigidBody vs KinematicRigidBody collision.")]
    public void ProcessPhysics_ShouldMakeEntityWithParentTransformColliding_WhenItWasNotCollidingButIsCollidingNowDueToParentTransform()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var rectangle1 = AddRectangleCollider(0, 0, 10, 5);
        var rectangle2 = AddRectangleColliderWithParentTransform(15, 10, -7.5, -7.5, 10, 5);

        // Assume
        Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var rectangleCollider1 = rectangle1.GetComponent<RectangleColliderComponent>();
        Assert.That(rectangleCollider1.IsColliding, Is.True);
        Assert.That(rectangleCollider1.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(rectangleCollider1.CollidingEntities.Single(), Is.EqualTo(rectangle2));

        var rectangleCollider2 = rectangle2.GetComponent<RectangleColliderComponent>();
        Assert.That(rectangleCollider2.IsColliding, Is.True);
        Assert.That(rectangleCollider2.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(rectangleCollider2.CollidingEntities.Single(), Is.EqualTo(rectangle1));
    }

    [Test]
    [Ignore("This test has no sense for KinematicRigidBody vs KinematicRigidBody collision.")]
    public void ProcessPhysics_ShouldMakeEntityWithParentTransformNotColliding_WhenItWasCollidingButIsNotCollidingAnymoreDueToParentTransform()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var rectangle1 = AddRectangleCollider(0, 0, 10, 5);
        var rectangle2 = AddRectangleColliderWithParentTransform(10, 10, 5, 2.5, 10, 5);

        rectangle1.GetComponent<RectangleColliderComponent>().AddCollidingEntity(rectangle2);
        rectangle2.GetComponent<RectangleColliderComponent>().AddCollidingEntity(rectangle1);

        // Assume
        Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
    }
}