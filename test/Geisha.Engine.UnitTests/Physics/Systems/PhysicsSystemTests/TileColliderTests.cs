using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.PhysicsEngine2D;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

public class TileColliderTests : PhysicsSystemTestsBase
{
    [TestCase(0, 0)]
    [TestCase(0, 1)]
    [TestCase(1, 0)]
    [TestCase(-1, -1)]
    [TestCase(1, -1)]
    [TestCase(-1, 1)]
    public void Constructor_ShouldThrowException_GivenInvalidTileSize(double width, double height)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(width, height)
        };

        // Act & Assert
        Assert.That(() => GetPhysicsSystem(physicsConfiguration), Throws.ArgumentException);
    }

    [Test]
    public void Constructor_ShouldNotThrowException_GivenValidTileSize()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(1, 1)
        };

        // Act & Assert
        Assert.That(() => GetPhysicsSystem(physicsConfiguration), Throws.Nothing);
    }

    [TestCase(1, 1, new double[] { }, 0, 0, CollisionNormalFilter.All)]
    [TestCase(1, 1, new double[] { -1, 0 }, 0, 0,
        CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new double[] { 1, 0 }, 0, 0,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new double[] { 0, -1 }, 0, 0,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new double[] { 0, 1 }, 0, 0,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical)]
    [TestCase(1, 1, new double[]
    {
        /*T1*/-1, 0, /*T2*/1, 0
    }, 0, 0, CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new double[]
    {
        /*T1*/0, -1, /*T2*/0, 1
    }, 0, 0, CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal)]
    [TestCase(1, 1, new double[]
    {
        /*T1*/-1, 0, /*T2*/0, 1
    }, 0, 0, CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical)]
    [TestCase(1, 1, new double[]
    {
        /*T1*/1, 0, /*T2*/0, -1
    }, 0, 0, CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new double[]
    {
        /*T1*/-1, 0, /*T2*/1, 0, /*T3*/0, -1, /*T4*/0, 1
    }, 0, 0, CollisionNormalFilter.None)]
    [TestCase(1, 1, new double[]
    {
        /*T1*/-1, 1, /*T2*/1, 1, /*T3*/-1, -1, /*T4*/1, -1
    }, 0, 0, CollisionNormalFilter.All)]
    public void TileBody_ShouldHaveCollisionNormalFilterSet_BasedOnSurroundingTiles(double tw, double th, double[] tiles, double tx, double ty,
        object collisionNormalFilter)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(tw, th)
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        for (var i = 0; i < tiles.Length; i += 2)
        {
            CreateTileStaticBody(tiles[i], tiles[i + 1]);
        }

        var entity = CreateTileStaticBody(tx, ty);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var body = GetBodyForEntity(physicsSystem, entity);
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(collisionNormalFilter));
    }

    [Test]
    public void KinematicBody_ShouldAvoidGhostCollisionWithTileCollider_WhenMovingRight()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(10, 10),
            PenetrationTolerance = 1,
            RenderCollisionGeometry = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(0, 0);
        CreateTileStaticBody(10, 0);
        var kinematicBody = CreateRectangleKinematicBody(-0.5, 9, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(10, 0);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(10, 0)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(1.5, 9)));
    }

    [Test]
    public void KinematicBody_ShouldAvoidGhostCollisionWithTileCollider_WhenMovingLeft()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(10, 10),
            PenetrationTolerance = 1,
            RenderCollisionGeometry = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(0, 0);
        CreateTileStaticBody(10, 0);
        var kinematicBody = CreateRectangleKinematicBody(10.5, 9, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(-10, 0);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(-10, 0)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(8.5, 9)));
    }

    [Test]
    public void KinematicBody_ShouldAvoidGhostCollisionWithTileCollider_WhenMovingUp()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(10, 10),
            PenetrationTolerance = 1,
            RenderCollisionGeometry = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(0, 0);
        CreateTileStaticBody(0, 10);
        var kinematicBody = CreateRectangleKinematicBody(-9, -0.5, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(0, 10);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(0, 10)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(-9, 1.5)));
    }

    [Test]
    public void KinematicBody_ShouldAvoidGhostCollisionWithTileCollider_WhenMovingDown()
    {
        // Arrange
        GameTime.FixedDeltaTime = TimeSpan.FromSeconds(0.1);
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(10, 10),
            PenetrationTolerance = 1,
            RenderCollisionGeometry = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(0, 0);
        CreateTileStaticBody(0, 10);
        var kinematicBody = CreateRectangleKinematicBody(-9, 10.5, 10, 10);
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().EnableCollisionResponse = true;
        kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity = new Vector2(0, -10);

        // Act
        SaveVisualOutput(physicsSystem, 0, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1, 10);
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2, 10);

        // Assert
        Assert.That(kinematicBody.GetComponent<KinematicRigidBody2DComponent>().LinearVelocity, Is.EqualTo(new Vector2(0, -10)));
        Assert.That(kinematicBody.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(-9, 8.5)));
    }
}