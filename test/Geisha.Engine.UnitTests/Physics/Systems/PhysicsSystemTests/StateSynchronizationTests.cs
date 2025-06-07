using System;
using System.Linq;
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
    public void ProcessPhysics_ShouldSynchronizeBodyWithComponents_GivenHierarchyOfStaticBodies()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity1 = CreateRectangleStaticBody(0, 0, 10, 20);
        var entity2 = CreateRectangleStaticBody(0, 0, 30, 40);
        var entity3 = CreateRectangleStaticBody(0, 0, 50, 60);
        entity3.Parent = entity2;
        entity2.Parent = entity1;

        // Assume
        physicsSystem.ProcessPhysics();

        var body1 = physicsSystem.PhysicsScene2D.Bodies.Single(b => b.RectangleCollider == new AxisAlignedRectangle(0, 0, 10, 20));
        var body2 = physicsSystem.PhysicsScene2D.Bodies.Single(b => b.RectangleCollider == new AxisAlignedRectangle(0, 0, 30, 40));
        var body3 = physicsSystem.PhysicsScene2D.Bodies.Single(b => b.RectangleCollider == new AxisAlignedRectangle(0, 0, 50, 60));

        Assert.That(body1.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body1.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body1.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body1.Rotation, Is.EqualTo(0d));
        Assert.That(body1.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body1.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body1.EnableCollisionResponse, Is.False);
        Assert.That(body1.RectangleCollider, Is.EqualTo(new AxisAlignedRectangle(0, 0, 10, 20)));

        Assert.That(body2.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body2.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body2.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body2.Rotation, Is.EqualTo(0d));
        Assert.That(body2.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body2.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body2.EnableCollisionResponse, Is.False);
        Assert.That(body2.RectangleCollider, Is.EqualTo(new AxisAlignedRectangle(0, 0, 30, 40)));

        Assert.That(body3.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body3.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body3.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body3.Rotation, Is.EqualTo(0d));
        Assert.That(body3.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body3.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body3.EnableCollisionResponse, Is.False);
        Assert.That(body3.RectangleCollider, Is.EqualTo(new AxisAlignedRectangle(0, 0, 50, 60)));

        // Act
        // Body 1
        var transform2DComponent = entity1.GetComponent<Transform2DComponent>();
        var rectangleColliderComponent = entity1.GetComponent<RectangleColliderComponent>();

        transform2DComponent.Translation = new Vector2(1, 2);
        transform2DComponent.Rotation = Angle.Deg2Rad(10);
        transform2DComponent.Scale = Vector2.One;

        rectangleColliderComponent.Dimensions = new Vector2(11, 22);

        // Body 2
        transform2DComponent = entity2.GetComponent<Transform2DComponent>();
        rectangleColliderComponent = entity2.GetComponent<RectangleColliderComponent>();

        transform2DComponent.Translation = new Vector2(3, 4);
        transform2DComponent.Rotation = Angle.Deg2Rad(20);
        transform2DComponent.Scale = Vector2.One;

        rectangleColliderComponent.Dimensions = new Vector2(33, 44);

        // Body 3
        transform2DComponent = entity3.GetComponent<Transform2DComponent>();
        rectangleColliderComponent = entity3.GetComponent<RectangleColliderComponent>();

        transform2DComponent.Translation = new Vector2(5, 6);
        transform2DComponent.Rotation = Angle.Deg2Rad(30);
        transform2DComponent.Scale = Vector2.One;

        rectangleColliderComponent.Dimensions = new Vector2(55, 66);

        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(body1.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body1.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body1.Position, Is.EqualTo(new Vector2(1, 2)));
        Assert.That(body1.Rotation, Is.EqualTo(Angle.Deg2Rad(10)));
        Assert.That(body1.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body1.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body1.EnableCollisionResponse, Is.False);
        Assert.That(body1.RectangleCollider, Is.EqualTo(new AxisAlignedRectangle(0, 0, 11, 22)));

        Assert.That(body2.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body2.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        var expectedPosition2 = new Vector2(1, 2) + (Matrix3x3.CreateRotation(Angle.Deg2Rad(10)) * new Vector2(3, 4).Homogeneous).ToVector2();
        Assert.That(body2.Position, Is.EqualTo(expectedPosition2));
        Assert.That(body2.Rotation, Is.EqualTo(Angle.Deg2Rad(30)));
        Assert.That(body2.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body2.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body2.EnableCollisionResponse, Is.False);
        Assert.That(body2.RectangleCollider, Is.EqualTo(new AxisAlignedRectangle(0, 0, 33, 44)));

        Assert.That(body3.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body3.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        var expectedPosition3 = expectedPosition2 + (Matrix3x3.CreateRotation(Angle.Deg2Rad(10 + 20)) * new Vector2(5, 6).Homogeneous).ToVector2();
        Assert.That(body3.Position, Is.EqualTo(expectedPosition3).Using(Vector2Comparer));
        Assert.That(body3.Rotation, Is.EqualTo(Angle.Deg2Rad(60)));
        Assert.That(body3.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body3.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body3.EnableCollisionResponse, Is.False);
        Assert.That(body3.RectangleCollider, Is.EqualTo(new AxisAlignedRectangle(0, 0, 55, 66)));
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