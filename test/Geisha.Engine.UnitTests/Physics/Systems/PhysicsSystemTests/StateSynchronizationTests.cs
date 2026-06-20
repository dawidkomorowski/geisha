using System;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.PhysicsEngine2D;
using Geisha.Engine.Physics.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
internal abstract class StateSynchronizationTests : PhysicsSystemTestsBase
{
    protected abstract void PerformSynchronization(PhysicsSystem physicsSystem);

    [TestFixture]
    public class StateSynchronizationViaProcessPhysicsTests : StateSynchronizationTests
    {
        protected override void PerformSynchronization(PhysicsSystem physicsSystem)
        {
            TimeSystem.FixedDeltaTime.Returns(TimeSpan.Zero);
            physicsSystem.ProcessPhysics();
        }
    }

    [TestFixture]
    public class StateSynchronizationViaSynchronizePhysicsStateTests : StateSynchronizationTests
    {
        protected override void PerformSynchronization(PhysicsSystem physicsSystem) => physicsSystem.SynchronizePhysicsState();
    }

    [TestFixture]
    public class StateSynchronizationViaCollider2DComponentSynchronizePhysicsStateTests : StateSynchronizationTests
    {
        protected override void PerformSynchronization(PhysicsSystem physicsSystem)
        {
            foreach (var entity in Scene.AllEntities)
            {
                foreach (var collider in entity.Components.OfType<Collider2DComponent>())
                {
                    collider.SynchronizePhysicsState();
                }
            }
        }

        [Test]
        public void SynchronizePhysicsState_ShouldSynchronizeOnlySingleBody()
        {
            // Arrange
            var physicsSystem = GetPhysicsSystem();
            var entity1 = CreateRectangleStaticBody(0, 0, 10, 20);
            var entity2 = CreateRectangleStaticBody(0, 0, 30, 40);

            var transform2DComponent1 = entity1.GetComponent<Transform2DComponent>();
            var collider1 = entity1.GetComponent<RectangleColliderComponent>();

            var transform2DComponent2 = entity2.GetComponent<Transform2DComponent>();

            // Assume
            collider1.SynchronizePhysicsState();
            entity2.GetComponent<RectangleColliderComponent>().SynchronizePhysicsState();

            var body1 = physicsSystem.FindInternalBody(entity1);
            var body2 = physicsSystem.FindInternalBody(entity2);

            Assert.That(body1.Position, Is.EqualTo(new Vector2(0, 0)));
            Assert.That(body2.Position, Is.EqualTo(new Vector2(0, 0)));

            // Act
            transform2DComponent1.Translation = new Vector2(10, 5);
            transform2DComponent2.Translation = new Vector2(20, 15);

            collider1.SynchronizePhysicsState();
            // entity2's collider is intentionally not synced

            // Assert
            Assert.That(body1.Position, Is.EqualTo(new Vector2(10, 5)));
            Assert.That(body2.Position, Is.EqualTo(new Vector2(0, 0)));
        }
    }

    [Test]
    public void ShouldSynchronizeBodyWithComponents_GivenRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleStaticBody(0, 0, 20, 10);

        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        var rectangleColliderComponent = entity.GetComponent<RectangleColliderComponent>();

        rectangleColliderComponent.Enabled = false;

        // Assume
        PerformSynchronization(physicsSystem);

        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body.Rotation, Is.EqualTo(0d));
        Assert.That(body.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body.EnableCollisionResponse, Is.False);
        Assert.That(body.RectangleColliderSize, Is.EqualTo(new SizeD(20, 10)));
        Assert.That(body.EnableCollisionDetection, Is.False);
        Assert.That(body.IsSensor, Is.False);
        Assert.That(body.CollisionLayer, Is.EqualTo(uint.MaxValue));
        Assert.That(body.CollisionMask, Is.EqualTo(uint.MaxValue));

        Assert.That(rectangleColliderComponent.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(0, 0, 20, 10)));

        // Act
        transform2DComponent.Translation = new Vector2(10, 5);
        transform2DComponent.Rotation = Angle.DegreesToRadians(30);
        transform2DComponent.Scale = Vector2.One;

        rectangleColliderComponent.Dimensions = new Vector2(30, 20);
        rectangleColliderComponent.Enabled = true;
        rectangleColliderComponent.IsSensor = true;
        rectangleColliderComponent.CollisionLayer = CollisionBitmask.FromBits(0, 2);
        rectangleColliderComponent.CollisionMask = CollisionBitmask.FromBits(1, 3);

        PerformSynchronization(physicsSystem);

        // Assert
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(10, 5)));
        Assert.That(body.Rotation, Is.EqualTo(Angle.DegreesToRadians(30)));
        Assert.That(body.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body.EnableCollisionResponse, Is.False);
        Assert.That(body.RectangleColliderSize, Is.EqualTo(new SizeD(30, 20)));
        Assert.That(body.EnableCollisionDetection, Is.True);
        Assert.That(body.IsSensor, Is.True);
        Assert.That(body.CollisionLayer, Is.EqualTo(CollisionBitmask.FromBits(0, 2).Value));
        Assert.That(body.CollisionMask, Is.EqualTo(CollisionBitmask.FromBits(1, 3).Value));

        Assert.That(rectangleColliderComponent.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(10, 5, 35.980762, 32.320508))
            .Using<AxisAlignedRectangle>(AxisAlignedRectangleEquality));
    }

    [Test]
    public void ShouldSynchronizeBodyWithComponents_GivenCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateCircleStaticBody(0, 0, 10);

        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        var circleColliderComponent = entity.GetComponent<CircleColliderComponent>();

        circleColliderComponent.Enabled = false;

        // Assume
        PerformSynchronization(physicsSystem);

        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Circle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body.Rotation, Is.EqualTo(0d));
        Assert.That(body.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body.EnableCollisionResponse, Is.False);
        Assert.That(body.CircleColliderRadius, Is.EqualTo(10));
        Assert.That(body.EnableCollisionDetection, Is.False);
        Assert.That(body.IsSensor, Is.False);
        Assert.That(body.CollisionLayer, Is.EqualTo(uint.MaxValue));
        Assert.That(body.CollisionMask, Is.EqualTo(uint.MaxValue));

        Assert.That(circleColliderComponent.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(0, 0, 20, 20)));


        // Act
        transform2DComponent.Translation = new Vector2(10, 5);
        transform2DComponent.Rotation = Angle.DegreesToRadians(30);
        transform2DComponent.Scale = Vector2.One;

        circleColliderComponent.Radius = 20;
        circleColliderComponent.Enabled = true;
        circleColliderComponent.IsSensor = true;
        circleColliderComponent.CollisionLayer = CollisionBitmask.FromBits(0, 2);
        circleColliderComponent.CollisionMask = CollisionBitmask.FromBits(1, 3);

        PerformSynchronization(physicsSystem);

        // Assert
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Circle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(10, 5)));
        Assert.That(body.Rotation, Is.EqualTo(Angle.DegreesToRadians(30)));
        Assert.That(body.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body.EnableCollisionResponse, Is.False);
        Assert.That(body.CircleColliderRadius, Is.EqualTo(20));
        Assert.That(body.EnableCollisionDetection, Is.True);
        Assert.That(body.IsSensor, Is.True);
        Assert.That(body.CollisionLayer, Is.EqualTo(CollisionBitmask.FromBits(0, 2).Value));
        Assert.That(body.CollisionMask, Is.EqualTo(CollisionBitmask.FromBits(1, 3).Value));

        Assert.That(circleColliderComponent.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(10, 5, 40, 40)));
    }

    [TestCase(1, 1, 0, 0, 0, 0)]
    [TestCase(1, 1, 1, 1, 1, 1)]
    [TestCase(1, 1, -1, 1, -1, 1)]
    [TestCase(1, 1, 1, -1, 1, -1)]
    [TestCase(1, 1, -1, -1, -1, -1)]
    [TestCase(1, 1, 3, -6, 3, -6)]
    [TestCase(1, 1, -5, 2, -5, 2)]
    [TestCase(1, 1, 0.4, 0.4, 0, 0)]
    [TestCase(1, 1, 0.6, 0.6, 1, 1)]
    [TestCase(1, 1, 0.4, 0.6, 0, 1)]
    [TestCase(1, 1, 0.6, 0.4, 1, 0)]
    [TestCase(1, 1, -0.4, -0.4, 0, 0)]
    [TestCase(1, 1, -0.6, -0.6, -1, -1)]
    [TestCase(1, 1, -0.4, -0.6, 0, -1)]
    [TestCase(1, 1, -0.6, -0.4, -1, 0)]
    [TestCase(1, 1, -0.4, 0.4, 0, 0)]
    [TestCase(1, 1, 0.4, -0.4, 0, 0)]
    [TestCase(2, 4, 0, 0, 0, 0)]
    [TestCase(2, 4, 2, 4, 2, 4)]
    [TestCase(2, 4, -2, -4, -2, -4)]
    [TestCase(2, 4, 0.9, 1.9, 0, 0)]
    [TestCase(2, 4, 1.1, 2.1, 2, 4)]
    [TestCase(2, 4, 8, 12, 8, 12)]
    [TestCase(2, 4, -8, -12, -8, -12)]
    [TestCase(2, 4, 13.75, 65.25, 14, 64)]
    public void ShouldSynchronizeBodyWithComponents_GivenTileStaticBody(double tw, double th, double x, double y, double ex, double ey)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(new PhysicsConfiguration
        {
            TileSize = new SizeD(tw, th)
        });
        var entity = CreateTileStaticBody(0, 0);

        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        var tileColliderComponent = entity.GetComponent<TileColliderComponent>();

        tileColliderComponent.Enabled = false;

        // Assume
        PerformSynchronization(physicsSystem);

        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Tile));
        Assert.That(body.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body.Rotation, Is.EqualTo(0d));
        Assert.That(body.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body.EnableCollisionResponse, Is.False);
        Assert.That(body.EnableCollisionDetection, Is.False);
        Assert.That(body.IsSensor, Is.False);
        Assert.That(body.CollisionLayer, Is.EqualTo(uint.MaxValue));
        Assert.That(body.CollisionMask, Is.EqualTo(uint.MaxValue));

        Assert.That(tileColliderComponent.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(0, 0, tw, th)));

        // Act
        transform2DComponent.Translation = new Vector2(x, y);
        transform2DComponent.Rotation = Angle.DegreesToRadians(30);
        transform2DComponent.Scale = new Vector2(2, 3);

        tileColliderComponent.Enabled = true;
        tileColliderComponent.IsSensor = true;
        tileColliderComponent.CollisionLayer = CollisionBitmask.FromBits(0, 2);
        tileColliderComponent.CollisionMask = CollisionBitmask.FromBits(1, 3);

        PerformSynchronization(physicsSystem);

        // Assert
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Tile));
        Assert.That(body.Position, Is.EqualTo(new Vector2(ex, ey)));
        Assert.That(body.Rotation, Is.Zero);
        Assert.That(body.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body.EnableCollisionResponse, Is.False);
        Assert.That(body.EnableCollisionDetection, Is.True);
        Assert.That(body.IsSensor, Is.True);
        Assert.That(body.CollisionLayer, Is.EqualTo(CollisionBitmask.FromBits(0, 2).Value));
        Assert.That(body.CollisionMask, Is.EqualTo(CollisionBitmask.FromBits(1, 3).Value));

        Assert.That(transform2DComponent.Translation, Is.EqualTo(new Vector2(ex, ey)));
        Assert.That(transform2DComponent.Rotation, Is.Zero);
        Assert.That(transform2DComponent.Scale, Is.EqualTo(Vector2.One));

        Assert.That(tileColliderComponent.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(ex, ey, tw, th)));
    }

    [Test]
    public void ShouldSynchronizeBodyWithComponents_GivenHierarchyOfStaticBodies()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity1 = CreateRectangleStaticBody(0, 0, 10, 20);
        var entity2 = CreateRectangleStaticBody(0, 0, 30, 40);
        var entity3 = CreateRectangleStaticBody(0, 0, 50, 60);
        entity3.Parent = entity2;
        entity2.Parent = entity1;

        var transform2DComponent1 = entity1.GetComponent<Transform2DComponent>();
        var rectangleColliderComponent1 = entity1.GetComponent<RectangleColliderComponent>();

        var transform2DComponent2 = entity2.GetComponent<Transform2DComponent>();
        var rectangleColliderComponent2 = entity2.GetComponent<RectangleColliderComponent>();

        var transform2DComponent3 = entity3.GetComponent<Transform2DComponent>();
        var rectangleColliderComponent3 = entity3.GetComponent<RectangleColliderComponent>();

        // Assume
        PerformSynchronization(physicsSystem);

        var body1 = physicsSystem.FindInternalBody(entity1);
        var body2 = physicsSystem.FindInternalBody(entity2);
        var body3 = physicsSystem.FindInternalBody(entity3);

        Assert.That(body1.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body1.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body1.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body1.Rotation, Is.EqualTo(0d));
        Assert.That(body1.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body1.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body1.EnableCollisionResponse, Is.False);
        Assert.That(body1.RectangleColliderSize, Is.EqualTo(new SizeD(10, 20)));
        Assert.That(body1.IsSensor, Is.False);
        Assert.That(body1.CollisionLayer, Is.EqualTo(uint.MaxValue));
        Assert.That(body1.CollisionMask, Is.EqualTo(uint.MaxValue));

        Assert.That(rectangleColliderComponent1.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(0, 0, 10, 20)));

        Assert.That(body2.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body2.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body2.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body2.Rotation, Is.EqualTo(0d));
        Assert.That(body2.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body2.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body2.EnableCollisionResponse, Is.False);
        Assert.That(body2.RectangleColliderSize, Is.EqualTo(new SizeD(30, 40)));
        Assert.That(body2.IsSensor, Is.False);
        Assert.That(body2.CollisionLayer, Is.EqualTo(uint.MaxValue));
        Assert.That(body2.CollisionMask, Is.EqualTo(uint.MaxValue));

        Assert.That(rectangleColliderComponent2.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(0, 0, 30, 40)));

        Assert.That(body3.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body3.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body3.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body3.Rotation, Is.EqualTo(0d));
        Assert.That(body3.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body3.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body3.EnableCollisionResponse, Is.False);
        Assert.That(body3.RectangleColliderSize, Is.EqualTo(new SizeD(50, 60)));
        Assert.That(body3.IsSensor, Is.False);
        Assert.That(body3.CollisionLayer, Is.EqualTo(uint.MaxValue));
        Assert.That(body3.CollisionMask, Is.EqualTo(uint.MaxValue));

        Assert.That(rectangleColliderComponent3.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(0, 0, 50, 60)));

        // Act
        // Body 1
        transform2DComponent1.Translation = new Vector2(1, 2);
        transform2DComponent1.Rotation = Angle.DegreesToRadians(10);
        transform2DComponent1.Scale = Vector2.One;

        rectangleColliderComponent1.Dimensions = new Vector2(11, 22);
        rectangleColliderComponent1.IsSensor = true;
        rectangleColliderComponent1.CollisionLayer = CollisionBitmask.FromBits(0, 2);
        rectangleColliderComponent1.CollisionMask = CollisionBitmask.FromBits(1, 3);

        // Body 2
        transform2DComponent2.Translation = new Vector2(3, 4);
        transform2DComponent2.Rotation = Angle.DegreesToRadians(20);
        transform2DComponent2.Scale = Vector2.One;

        rectangleColliderComponent2.Dimensions = new Vector2(33, 44);
        rectangleColliderComponent2.IsSensor = true;
        rectangleColliderComponent2.CollisionLayer = CollisionBitmask.FromBits(1, 4);
        rectangleColliderComponent2.CollisionMask = CollisionBitmask.FromBits(0, 3);

        // Body 3
        transform2DComponent3.Translation = new Vector2(5, 6);
        transform2DComponent3.Rotation = Angle.DegreesToRadians(30);
        transform2DComponent3.Scale = Vector2.One;

        rectangleColliderComponent3.Dimensions = new Vector2(55, 66);
        rectangleColliderComponent3.IsSensor = true;
        rectangleColliderComponent3.CollisionLayer = CollisionBitmask.FromBits(2, 5);
        rectangleColliderComponent3.CollisionMask = CollisionBitmask.FromBits(1, 4);

        PerformSynchronization(physicsSystem);

        // Assert
        Assert.That(body1.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body1.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body1.Position, Is.EqualTo(new Vector2(1, 2)));
        Assert.That(body1.Rotation, Is.EqualTo(Angle.DegreesToRadians(10)));
        Assert.That(body1.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body1.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body1.EnableCollisionResponse, Is.False);
        Assert.That(body1.RectangleColliderSize, Is.EqualTo(new SizeD(11, 22)));
        Assert.That(body1.IsSensor, Is.True);
        Assert.That(body1.CollisionLayer, Is.EqualTo(CollisionBitmask.FromBits(0, 2).Value));
        Assert.That(body1.CollisionMask, Is.EqualTo(CollisionBitmask.FromBits(1, 3).Value));

        Assert.That(rectangleColliderComponent1.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(1, 2, 14.653145, 23.575900))
            .Using<AxisAlignedRectangle>(AxisAlignedRectangleEquality));

        Assert.That(body2.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body2.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        var expectedPosition2 = new Vector2(1, 2) + (Matrix3x3.CreateRotation(Angle.DegreesToRadians(10)) * new Vector2(3, 4).Homogeneous).ToVector2();
        Assert.That(body2.Position, Is.EqualTo(expectedPosition2));
        Assert.That(body2.Rotation, Is.EqualTo(Angle.DegreesToRadians(30)));
        Assert.That(body2.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body2.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body2.EnableCollisionResponse, Is.False);
        Assert.That(body2.RectangleColliderSize, Is.EqualTo(new SizeD(33, 44)));
        Assert.That(body2.IsSensor, Is.True);
        Assert.That(body2.CollisionLayer, Is.EqualTo(CollisionBitmask.FromBits(1, 4).Value));
        Assert.That(body2.CollisionMask, Is.EqualTo(CollisionBitmask.FromBits(0, 3).Value));

        Assert.That(rectangleColliderComponent2.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(expectedPosition2, new Vector2(50.578838, 54.605117)))
            .Using<AxisAlignedRectangle>(AxisAlignedRectangleEquality));

        Assert.That(body3.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body3.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        var expectedPosition3 = expectedPosition2 + (Matrix3x3.CreateRotation(Angle.DegreesToRadians(10 + 20)) * new Vector2(5, 6).Homogeneous).ToVector2();
        Assert.That(body3.Position, Is.EqualTo(expectedPosition3).Using<Vector2>(Vector2Equality));
        Assert.That(body3.Rotation, Is.EqualTo(Angle.DegreesToRadians(60)));
        Assert.That(body3.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body3.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body3.EnableCollisionResponse, Is.False);
        Assert.That(body3.RectangleColliderSize, Is.EqualTo(new SizeD(55, 66)));
        Assert.That(body3.IsSensor, Is.True);
        Assert.That(body3.CollisionLayer, Is.EqualTo(CollisionBitmask.FromBits(2, 5).Value));
        Assert.That(body3.CollisionMask, Is.EqualTo(CollisionBitmask.FromBits(1, 4).Value));

        Assert.That(rectangleColliderComponent3.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(expectedPosition3, new Vector2(84.657676, 80.631397)))
            .Using<AxisAlignedRectangle>(AxisAlignedRectangleEquality));
    }

    [Test]
    public void ShouldSynchronizeBodyWithComponents_GivenRectangleKinematicBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleKinematicBody(0, 0, 20, 10);

        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        var rectangleColliderComponent = entity.GetComponent<RectangleColliderComponent>();
        var kinematicRigidBody2DComponent = entity.GetComponent<KinematicRigidBody2DComponent>();

        rectangleColliderComponent.Enabled = false;

        // Assume
        PerformSynchronization(physicsSystem);

        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body.Rotation, Is.EqualTo(0d));
        Assert.That(body.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body.EnableCollisionResponse, Is.False);
        Assert.That(body.RectangleColliderSize, Is.EqualTo(new SizeD(20, 10)));
        Assert.That(body.EnableCollisionDetection, Is.False);
        Assert.That(body.IsSensor, Is.False);
        Assert.That(body.CollisionLayer, Is.EqualTo(uint.MaxValue));
        Assert.That(body.CollisionMask, Is.EqualTo(uint.MaxValue));

        Assert.That(rectangleColliderComponent.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(0, 0, 20, 10)));

        // Act
        transform2DComponent.Translation = new Vector2(10, 5);
        transform2DComponent.Rotation = Angle.DegreesToRadians(30);
        transform2DComponent.Scale = Vector2.One;

        rectangleColliderComponent.Dimensions = new Vector2(30, 20);
        rectangleColliderComponent.Enabled = true;
        rectangleColliderComponent.IsSensor = true;
        rectangleColliderComponent.CollisionLayer = CollisionBitmask.FromBits(0, 2);
        rectangleColliderComponent.CollisionMask = CollisionBitmask.FromBits(1, 3);

        kinematicRigidBody2DComponent.LinearVelocity = new Vector2(1, 2);
        kinematicRigidBody2DComponent.AngularVelocity = 0.5d;
        kinematicRigidBody2DComponent.EnableCollisionResponse = true;

        PerformSynchronization(physicsSystem);

        // Assert
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(10, 5)));
        Assert.That(body.Rotation, Is.EqualTo(Angle.DegreesToRadians(30)));
        Assert.That(body.LinearVelocity, Is.EqualTo(new Vector2(1, 2)));
        Assert.That(body.AngularVelocity, Is.EqualTo(0.5d));
        Assert.That(body.EnableCollisionResponse, Is.True);
        Assert.That(body.RectangleColliderSize, Is.EqualTo(new SizeD(30, 20)));
        Assert.That(body.EnableCollisionDetection, Is.True);
        Assert.That(body.IsSensor, Is.True);
        Assert.That(body.CollisionLayer, Is.EqualTo(CollisionBitmask.FromBits(0, 2).Value));
        Assert.That(body.CollisionMask, Is.EqualTo(CollisionBitmask.FromBits(1, 3).Value));

        Assert.That(rectangleColliderComponent.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(10, 5, 35.980762, 32.320508))
            .Using<AxisAlignedRectangle>(AxisAlignedRectangleEquality));
    }

    [Test]
    public void ShouldSynchronizeBodyWithComponents_GivenCircleKinematicBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateCircleKinematicBody(0, 0, 10);

        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        var circleColliderComponent = entity.GetComponent<CircleColliderComponent>();
        var kinematicRigidBody2DComponent = entity.GetComponent<KinematicRigidBody2DComponent>();

        circleColliderComponent.Enabled = false;

        // Assume
        PerformSynchronization(physicsSystem);

        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Circle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(0, 0)));
        Assert.That(body.Rotation, Is.EqualTo(0d));
        Assert.That(body.LinearVelocity, Is.EqualTo(Vector2.Zero));
        Assert.That(body.AngularVelocity, Is.EqualTo(0d));
        Assert.That(body.EnableCollisionResponse, Is.False);
        Assert.That(body.CircleColliderRadius, Is.EqualTo(10));
        Assert.That(body.EnableCollisionDetection, Is.False);
        Assert.That(body.IsSensor, Is.False);
        Assert.That(body.CollisionLayer, Is.EqualTo(uint.MaxValue));
        Assert.That(body.CollisionMask, Is.EqualTo(uint.MaxValue));

        Assert.That(circleColliderComponent.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(0, 0, 20, 20)));

        // Act
        transform2DComponent.Translation = new Vector2(10, 5);
        transform2DComponent.Rotation = Angle.DegreesToRadians(30);
        transform2DComponent.Scale = Vector2.One;

        circleColliderComponent.Radius = 20;
        circleColliderComponent.Enabled = true;
        circleColliderComponent.IsSensor = true;
        circleColliderComponent.CollisionLayer = CollisionBitmask.FromBits(0, 2);
        circleColliderComponent.CollisionMask = CollisionBitmask.FromBits(1, 3);

        kinematicRigidBody2DComponent.LinearVelocity = new Vector2(1, 2);
        kinematicRigidBody2DComponent.AngularVelocity = 0.5d;
        kinematicRigidBody2DComponent.EnableCollisionResponse = true;

        PerformSynchronization(physicsSystem);

        // Assert
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Circle));
        Assert.That(body.Position, Is.EqualTo(new Vector2(10, 5)));
        Assert.That(body.Rotation, Is.EqualTo(Angle.DegreesToRadians(30)));
        Assert.That(body.LinearVelocity, Is.EqualTo(new Vector2(1, 2)));
        Assert.That(body.AngularVelocity, Is.EqualTo(0.5d));
        Assert.That(body.EnableCollisionResponse, Is.True);
        Assert.That(body.CircleColliderRadius, Is.EqualTo(20));
        Assert.That(body.EnableCollisionDetection, Is.True);
        Assert.That(body.IsSensor, Is.True);
        Assert.That(body.CollisionLayer, Is.EqualTo(CollisionBitmask.FromBits(0, 2).Value));
        Assert.That(body.CollisionMask, Is.EqualTo(CollisionBitmask.FromBits(1, 3).Value));

        Assert.That(circleColliderComponent.BoundingRectangle, Is.EqualTo(new AxisAlignedRectangle(10, 5, 40, 40)));
    }
}