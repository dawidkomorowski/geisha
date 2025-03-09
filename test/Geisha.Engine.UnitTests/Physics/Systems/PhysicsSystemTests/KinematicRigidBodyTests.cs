using Geisha.Engine.Core.Components;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

// TODO Once kinematic movement is implemented it will be a better way to test if entity represents KinematicRigidBody than testing for collisions.
// TODO However cleanup of colliding entities makes only sense when considering collisions.
[TestFixture]
public class KinematicRigidBodyTests : PhysicsSystemTestsBase
{
    [Test]
    public void KinematicRigidBody_ShouldBeRemoved_WhenTransform2DComponentRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = CreateRectangleKinematicBody(0, 0, 10, 5);
        var kinematicBody2 = CreateRectangleKinematicBody(5, 0, 10, 5);

        var rectangleCollider1 = kinematicBody1.GetComponent<RectangleColliderComponent>();
        var rectangleCollider2 = kinematicBody2.GetComponent<RectangleColliderComponent>();

        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(rectangleCollider1.IsColliding, Is.True);
        Assert.That(rectangleCollider2.IsColliding, Is.True);

        // Act
        kinematicBody1.RemoveComponent(kinematicBody1.GetComponent<Transform2DComponent>());
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangleCollider1.IsColliding, Is.False);
        Assert.That(rectangleCollider1.Contacts, Has.Count.Zero);

        Assert.That(rectangleCollider2.IsColliding, Is.False);
        Assert.That(rectangleCollider2.Contacts, Has.Count.Zero);
    }

    [Test]
    public void KinematicRigidBody_ShouldBeRemoved_WhenCollider2DComponentRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = CreateRectangleKinematicBody(0, 0, 10, 5);
        var kinematicBody2 = CreateRectangleKinematicBody(5, 0, 10, 5);

        var rectangleCollider1 = kinematicBody1.GetComponent<RectangleColliderComponent>();
        var rectangleCollider2 = kinematicBody2.GetComponent<RectangleColliderComponent>();

        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(rectangleCollider1.IsColliding, Is.True);
        Assert.That(rectangleCollider2.IsColliding, Is.True);

        // Act
        kinematicBody1.RemoveComponent(rectangleCollider1);
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangleCollider1.IsColliding, Is.False);
        Assert.That(rectangleCollider1.Contacts, Has.Count.Zero);

        Assert.That(rectangleCollider2.IsColliding, Is.False);
        Assert.That(rectangleCollider2.Contacts, Has.Count.Zero);
    }

    [Test]
    public void KinematicRigidBody_ShouldBeRemoved_WhenKinematicRigidBody2DComponentRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = CreateRectangleKinematicBody(0, 0, 10, 5);
        var kinematicBody2 = CreateRectangleKinematicBody(5, 0, 10, 5);

        var rectangleCollider1 = kinematicBody1.GetComponent<RectangleColliderComponent>();
        var rectangleCollider2 = kinematicBody2.GetComponent<RectangleColliderComponent>();

        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(rectangleCollider1.IsColliding, Is.True);
        Assert.That(rectangleCollider2.IsColliding, Is.True);

        // Act
        kinematicBody1.RemoveComponent(kinematicBody1.GetComponent<KinematicRigidBody2DComponent>());
        kinematicBody2.RemoveComponent(kinematicBody2.GetComponent<KinematicRigidBody2DComponent>());
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangleCollider1.IsColliding, Is.False);
        Assert.That(rectangleCollider1.Contacts, Has.Count.Zero);

        Assert.That(rectangleCollider2.IsColliding, Is.False);
        Assert.That(rectangleCollider2.Contacts, Has.Count.Zero);
    }

    [Test]
    public void KinematicRigidBody_ShouldBeRemoved_WhenEntityStopsToBeRootEntity()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = CreateRectangleKinematicBody(0, 0, 10, 5);
        var kinematicBody2 = CreateRectangleKinematicBody(5, 0, 10, 5);

        var rectangleCollider1 = kinematicBody1.GetComponent<RectangleColliderComponent>();
        var rectangleCollider2 = kinematicBody2.GetComponent<RectangleColliderComponent>();

        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(rectangleCollider1.IsColliding, Is.True);
        Assert.That(rectangleCollider2.IsColliding, Is.True);

        // Act
        var parent = Scene.CreateEntity();
        kinematicBody1.Parent = parent;

        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangleCollider1.IsColliding, Is.False);
        Assert.That(rectangleCollider1.Contacts, Has.Count.Zero);

        Assert.That(rectangleCollider2.IsColliding, Is.False);
        Assert.That(rectangleCollider2.Contacts, Has.Count.Zero);
    }

    [Test]
    public void KinematicRigidBody_ShouldBeRecreated_WhenEntityBecomesRootEntity()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = CreateRectangleKinematicBody(0, 0, 10, 5);
        var kinematicBody2 = CreateRectangleKinematicBody(5, 0, 10, 5);

        var rectangleCollider1 = kinematicBody1.GetComponent<RectangleColliderComponent>();
        var rectangleCollider2 = kinematicBody2.GetComponent<RectangleColliderComponent>();

        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(rectangleCollider1.IsColliding, Is.True);
        Assert.That(rectangleCollider2.IsColliding, Is.True);

        var parent = Scene.CreateEntity();
        kinematicBody1.Parent = parent;

        physicsSystem.ProcessPhysics();

        Assert.That(rectangleCollider1.IsColliding, Is.False);
        Assert.That(rectangleCollider2.IsColliding, Is.False);

        // Act
        kinematicBody1.Parent = null;
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangleCollider1.IsColliding, Is.True);
        Assert.That(rectangleCollider2.IsColliding, Is.True);
    }
}