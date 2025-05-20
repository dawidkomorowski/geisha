using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class KinematicRigidBodyCollisionResponseTests : PhysicsSystemTestsBase
{
    [Test]
    public void CollisionResponseDisabled_KinematicBody_And_StaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 5, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = false;
        CreateRectangleStaticBody(5, 5, 10, 10);

        SaveVisualOutput(physicsSystem, 0, 10);

        // Act
        physicsSystem.ProcessPhysics();

        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(0, 5)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Rotation, Is.Zero);
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().AngularVelocity, Is.EqualTo(0d));
    }

    [Test]
    public void CollisionResponseDisabled_KinematicBody_And_KinematicBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = CreateRectangleKinematicBody(0, 5, 10, 10);
        kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = false;
        var kinematicBody2 = CreateRectangleKinematicBody(5, 5, 10, 10);
        kinematicBody2.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = false;

        SaveVisualOutput(physicsSystem, 0, 10);

        // Act
        physicsSystem.ProcessPhysics();

        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        Assert.That(kinematicBody1.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(0, 5)));
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Rotation, Is.Zero);
        Assert.That(kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().AngularVelocity, Is.EqualTo(0d));

        Assert.That(kinematicBody2.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(5, 5)));
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Rotation, Is.Zero);
        Assert.That(kinematicBody2.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(kinematicBody2.GetComponent<KinematicRigidBody2DComponent>().AngularVelocity, Is.EqualTo(0d));
    }

    [Test]
    public void CollisionResponseEnabled_KinematicBody_And_StaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 5, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        CreateRectangleStaticBody(5, 5, 10, 10);

        SaveVisualOutput(physicsSystem, 0, 10);

        // Act
        physicsSystem.ProcessPhysics();

        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(-5, 5)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Rotation, Is.Zero);
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().AngularVelocity, Is.EqualTo(0d));
    }

    [Test]
    public void CollisionResponseEnabled_KinematicBody_And_MultipleStaticBodies()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 5, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        CreateRectangleStaticBody(0, 0, 10, 10);
        CreateRectangleStaticBody(5, 5, 10, 10);

        SaveVisualOutput(physicsSystem, 0, 10);

        // Act
        physicsSystem.ProcessPhysics();

        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(-5, 10)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Rotation, Is.Zero);
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().AngularVelocity, Is.EqualTo(0d));
    }

    [Test]
    public void CollisionResponseEnabled_ForOneBody_KinematicBody_And_KinematicBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = CreateRectangleKinematicBody(0, 5, 10, 10);
        kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        var kinematicBody2 = CreateRectangleKinematicBody(5, 5, 10, 10);
        kinematicBody2.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = false;

        SaveVisualOutput(physicsSystem, 0, 10);

        // Act
        physicsSystem.ProcessPhysics();

        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        Assert.That(kinematicBody1.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(-5, 5)));
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Rotation, Is.Zero);
        Assert.That(kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().AngularVelocity, Is.EqualTo(0d));

        Assert.That(kinematicBody2.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(5, 5)));
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Rotation, Is.Zero);
        Assert.That(kinematicBody2.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(kinematicBody2.GetComponent<KinematicRigidBody2DComponent>().AngularVelocity, Is.EqualTo(0d));
    }

    [Test]
    public void CollisionResponseEnabled_ForBothBodies_KinematicBody_And_KinematicBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = CreateRectangleKinematicBody(0, 5, 10, 10);
        kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        var kinematicBody2 = CreateRectangleKinematicBody(5, 5, 10, 10);
        kinematicBody2.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;

        SaveVisualOutput(physicsSystem, 0, 10);

        // Act
        physicsSystem.ProcessPhysics();

        SaveVisualOutput(physicsSystem, 1, 10);

        // Assert
        Assert.That(kinematicBody1.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(-2.5, 5)));
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Rotation, Is.Zero);
        Assert.That(kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().AngularVelocity, Is.EqualTo(0d));

        Assert.That(kinematicBody2.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(7.5, 5)));
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Rotation, Is.Zero);
        Assert.That(kinematicBody2.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(kinematicBody2.GetComponent<KinematicRigidBody2DComponent>().AngularVelocity, Is.EqualTo(0d));
    }
}