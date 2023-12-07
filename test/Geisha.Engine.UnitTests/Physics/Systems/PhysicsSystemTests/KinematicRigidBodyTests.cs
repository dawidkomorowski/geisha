using Geisha.Engine.Core.Components;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

// TODO Once kinematic movement is implemented it will be a better way to test if entity represents KinematicRigidBody than testing for collisions.
[TestFixture]
public class KinematicRigidBodyTests : PhysicsSystemTestsBase
{
    [Test]
    public void KinematicRigidBody_ShouldBeRemoved_WhenTransform2DComponentRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = AddRectangleKinematicBody(0, 0, 10, 5);
        var kinematicBody2 = AddRectangleKinematicBody(5, 0, 10, 5);

        var rectangleCollider1 = kinematicBody1.GetComponent<RectangleColliderComponent>();
        var rectangleCollider2 = kinematicBody2.GetComponent<RectangleColliderComponent>();

        kinematicBody1.RemoveComponent(kinematicBody1.GetComponent<Transform2DComponent>());

        // Assume
        Assume.That(rectangleCollider1.IsColliding, Is.False);
        Assume.That(rectangleCollider2.IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangleCollider1.IsColliding, Is.False);
        Assert.That(rectangleCollider1.CollidingEntities, Has.Count.Zero);

        Assert.That(rectangleCollider2.IsColliding, Is.False);
        Assert.That(rectangleCollider2.CollidingEntities, Has.Count.Zero);
    }

    [Test]
    public void KinematicRigidBody_ShouldBeRemoved_WhenCollider2DComponentRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = AddRectangleKinematicBody(0, 0, 10, 5);
        var kinematicBody2 = AddRectangleKinematicBody(5, 0, 10, 5);

        var rectangleCollider1 = kinematicBody1.GetComponent<RectangleColliderComponent>();
        var rectangleCollider2 = kinematicBody2.GetComponent<RectangleColliderComponent>();

        kinematicBody1.RemoveComponent(rectangleCollider1);

        // Assume
        Assume.That(rectangleCollider1.IsColliding, Is.False);
        Assume.That(rectangleCollider2.IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangleCollider1.IsColliding, Is.False);
        Assert.That(rectangleCollider1.CollidingEntities, Has.Count.Zero);

        Assert.That(rectangleCollider2.IsColliding, Is.False);
        Assert.That(rectangleCollider2.CollidingEntities, Has.Count.Zero);
    }

    [Test]
    public void KinematicRigidBody_ShouldBeRemoved_WhenKinematicRigidBody2DComponentRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = AddRectangleKinematicBody(0, 0, 10, 5);
        var kinematicBody2 = AddRectangleKinematicBody(5, 0, 10, 5);

        var rectangleCollider1 = kinematicBody1.GetComponent<RectangleColliderComponent>();
        var rectangleCollider2 = kinematicBody2.GetComponent<RectangleColliderComponent>();

        kinematicBody1.RemoveComponent(kinematicBody1.GetComponent<KinematicRigidBody2DComponent>());

        // Assume
        Assume.That(rectangleCollider1.IsColliding, Is.False);
        Assume.That(rectangleCollider2.IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangleCollider1.IsColliding, Is.False);
        Assert.That(rectangleCollider1.CollidingEntities, Has.Count.Zero);

        Assert.That(rectangleCollider2.IsColliding, Is.False);
        Assert.That(rectangleCollider2.CollidingEntities, Has.Count.Zero);
    }

    [Test]
    public void KinematicRigidBody_ShouldBeRemoved_WhenEntityStopsToBeRootEntity()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = AddRectangleKinematicBody(0, 0, 10, 5);
        var kinematicBody2 = AddRectangleKinematicBody(5, 0, 10, 5);

        var rectangleCollider1 = kinematicBody1.GetComponent<RectangleColliderComponent>();
        var rectangleCollider2 = kinematicBody2.GetComponent<RectangleColliderComponent>();

        var parent = Scene.CreateEntity();
        kinematicBody1.Parent = parent;

        // Assume
        Assume.That(rectangleCollider1.IsColliding, Is.False);
        Assume.That(rectangleCollider2.IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangleCollider1.IsColliding, Is.False);
        Assert.That(rectangleCollider1.CollidingEntities, Has.Count.Zero);

        Assert.That(rectangleCollider2.IsColliding, Is.False);
        Assert.That(rectangleCollider2.CollidingEntities, Has.Count.Zero);
    }

    [Test]
    public void KinematicRigidBody_ShouldBeRecreated_WhenEntityBecomesRootEntity()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = AddRectangleKinematicBody(0, 0, 10, 5);
        var kinematicBody2 = AddRectangleKinematicBody(5, 0, 10, 5);

        var rectangleCollider1 = kinematicBody1.GetComponent<RectangleColliderComponent>();
        var rectangleCollider2 = kinematicBody2.GetComponent<RectangleColliderComponent>();

        var parent = Scene.CreateEntity();
        kinematicBody1.Parent = parent;

        physicsSystem.ProcessPhysics();

        // Assume
        Assume.That(rectangleCollider1.IsColliding, Is.False);
        Assume.That(rectangleCollider2.IsColliding, Is.False);

        // Act
        kinematicBody1.Parent = null;
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangleCollider1.IsColliding, Is.True);

        Assert.That(rectangleCollider2.IsColliding, Is.True);
    }
}