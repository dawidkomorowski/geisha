using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.PhysicsEngine2D;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class StateSynchronizationTests : PhysicsSystemTestsBase
{
    [Test]
    public void ProcessPhysics_ShouldSynchronizeBodyWithComponents_GivenRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleStaticBody(0, 0, 20, 10);

        // Assume
        physicsSystem.ProcessPhysics();

        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body.Rotation, Is.EqualTo(0d));
        Assert.That(body.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body.EnableCollisionResponse, Is.False);
        Assert.That(body.RectangleCollider, Is.EqualTo(new AxisAlignedRectangle(0, 0, 20, 10)));

        // Act
        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        var rectangleColliderComponent = entity.GetComponent<RectangleColliderComponent>();

        transform2DComponent.Translation = new Vector2(10, 5);
        transform2DComponent.Rotation = Angle.Deg2Rad(30);
        transform2DComponent.Scale = Vector2.One;

        rectangleColliderComponent.Dimensions = new Vector2(30, 20);

        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(10, 5)));
        Assert.That(body.Rotation, Is.EqualTo(Angle.Deg2Rad(30)));
        Assert.That(body.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body.EnableCollisionResponse, Is.False);
        Assert.That(body.RectangleCollider, Is.EqualTo(new AxisAlignedRectangle(0, 0, 30, 20)));
    }

    [Test]
    public void ProcessPhysics_ShouldSynchronizeBodyWithComponents_GivenCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateCircleStaticBody(0, 0, 10);

        // Assume
        physicsSystem.ProcessPhysics();

        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Circle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body.Rotation, Is.EqualTo(0d));
        Assert.That(body.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body.EnableCollisionResponse, Is.False);
        Assert.That(body.CircleCollider, Is.EqualTo(new Circle(10)));

        // Act
        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        var circleColliderComponent = entity.GetComponent<CircleColliderComponent>();

        transform2DComponent.Translation = new Vector2(10, 5);
        transform2DComponent.Rotation = Angle.Deg2Rad(30);
        transform2DComponent.Scale = Vector2.One;

        circleColliderComponent.Radius = 20;

        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Circle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(10, 5)));
        Assert.That(body.Rotation, Is.EqualTo(Angle.Deg2Rad(30)));
        Assert.That(body.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body.EnableCollisionResponse, Is.False);
        Assert.That(body.CircleCollider, Is.EqualTo(new Circle(20)));
    }

    [Test]
    public void ProcessPhysics_ShouldSynchronizeBodyWithComponents_GivenRectangleKinematicBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleKinematicBody(0, 0, 20, 10);

        // Assume
        physicsSystem.ProcessPhysics();

        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body.Rotation, Is.EqualTo(0d));
        Assert.That(body.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body.EnableCollisionResponse, Is.False);
        Assert.That(body.RectangleCollider, Is.EqualTo(new AxisAlignedRectangle(0, 0, 20, 10)));

        // Act
        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        var rectangleColliderComponent = entity.GetComponent<RectangleColliderComponent>();
        var kinematicRigidBody2DComponent = entity.GetComponent<KinematicRigidBody2DComponent>();

        transform2DComponent.Translation = new Vector2(10, 5);
        transform2DComponent.Rotation = Angle.Deg2Rad(30);
        transform2DComponent.Scale = Vector2.One;

        rectangleColliderComponent.Dimensions = new Vector2(30, 20);

        kinematicRigidBody2DComponent.LinearVelocity = new Vector2(1, 2);
        kinematicRigidBody2DComponent.AngularVelocity = 0.5d;
        kinematicRigidBody2DComponent.EnableCollisionResponse = true;

        GameTime.FixedDeltaTime = TimeSpan.Zero;
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(10, 5)));
        Assert.That(body.Rotation, Is.EqualTo(Angle.Deg2Rad(30)));
        Assert.That(body.LinearVelocity, Is.EqualTo(new Vector2(1, 2)));
        Assert.That(body.AngularVelocity, Is.EqualTo(0.5d));
        Assert.That(body.EnableCollisionResponse, Is.True);
        Assert.That(body.RectangleCollider, Is.EqualTo(new AxisAlignedRectangle(0, 0, 30, 20)));
    }

    [Test]
    public void ProcessPhysics_ShouldSynchronizeBodyWithComponents_GivenCircleKinematicBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateCircleKinematicBody(0, 0, 10);

        // Assume
        physicsSystem.ProcessPhysics();

        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Circle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body.Rotation, Is.EqualTo(0d));
        Assert.That(body.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body.EnableCollisionResponse, Is.False);
        Assert.That(body.CircleCollider, Is.EqualTo(new Circle(10)));

        // Act
        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        var circleColliderComponent = entity.GetComponent<CircleColliderComponent>();
        var kinematicRigidBody2DComponent = entity.GetComponent<KinematicRigidBody2DComponent>();

        transform2DComponent.Translation = new Vector2(10, 5);
        transform2DComponent.Rotation = Angle.Deg2Rad(30);
        transform2DComponent.Scale = Vector2.One;

        circleColliderComponent.Radius = 20;

        kinematicRigidBody2DComponent.LinearVelocity = new Vector2(1, 2);
        kinematicRigidBody2DComponent.AngularVelocity = 0.5d;
        kinematicRigidBody2DComponent.EnableCollisionResponse = true;

        GameTime.FixedDeltaTime = TimeSpan.Zero;
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Circle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(10, 5)));
        Assert.That(body.Rotation, Is.EqualTo(Angle.Deg2Rad(30)));
        Assert.That(body.LinearVelocity, Is.EqualTo(new Vector2(1, 2)));
        Assert.That(body.AngularVelocity, Is.EqualTo(0.5d));
        Assert.That(body.EnableCollisionResponse, Is.True);
        Assert.That(body.CircleCollider, Is.EqualTo(new Circle(20)));
    }
}