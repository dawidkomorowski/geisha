using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class CollisionResponseWhenKinematicBodyMovingTests : PhysicsSystemTestsBase
{
    [Test]
    public void CollisionResponseDisabled_KinematicBody_And_StaticBody()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);

        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(-6, 5, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = false;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(20, 0);
        CreateRectangleStaticBody(5, 5, 10, 10);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(-2, 5)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Rotation, Is.Zero);
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(20, 0)));
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().AngularVelocity, Is.EqualTo(0d));
    }

    [Test]
    public void CollisionResponseDisabled_KinematicBody_And_KinematicBody()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);

        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = CreateRectangleKinematicBody(-6, 5, 10, 10);
        kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = false;
        kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(20, 0);
        var kinematicBody2 = CreateRectangleKinematicBody(5, 5, 10, 10);
        kinematicBody2.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = false;

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody1.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(-2, 5)));
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Rotation, Is.Zero);
        Assert.That(kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(20, 0)));
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
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);

        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(-6, 5, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(20, 0);
        CreateRectangleStaticBody(5, 5, 10, 10);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

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
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);

        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(-6, 11, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(20, -20);
        CreateRectangleStaticBody(0, 0, 10, 10);
        CreateRectangleStaticBody(5, 5, 10, 10);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

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
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);

        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = CreateRectangleKinematicBody(-6, 5, 10, 10);
        kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(20, 0);
        var kinematicBody2 = CreateRectangleKinematicBody(5, 5, 10, 10);
        kinematicBody2.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = false;

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

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
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);

        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = CreateRectangleKinematicBody(-6, 5, 10, 10);
        kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(20, 0);
        var kinematicBody2 = CreateRectangleKinematicBody(5, 5, 10, 10);
        kinematicBody2.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody1.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(-3.5, 5)));
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Rotation, Is.Zero);
        Assert.That(kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(10, 0)));
        Assert.That(kinematicBody1.GetComponent<KinematicRigidBody2DComponent>().AngularVelocity, Is.EqualTo(0d));

        Assert.That(kinematicBody2.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(6.5, 5)));
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Rotation, Is.Zero);
        Assert.That(kinematicBody2.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(10, 0)));
        Assert.That(kinematicBody2.GetComponent<KinematicRigidBody2DComponent>().AngularVelocity, Is.EqualTo(0d));
    }
}