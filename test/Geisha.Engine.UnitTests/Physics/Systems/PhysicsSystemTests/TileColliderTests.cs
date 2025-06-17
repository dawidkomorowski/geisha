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

    // Tile size is 1x1 and asserted tile is at (0, 0).
    [TestCase(1, 1, new double[0], 0, 0, CollisionNormalFilter.All)]
    [TestCase(1, 1, new[] { -1d, 0d }, 0, 0,
        CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new[] { 1d, 0d }, 0, 0,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new[] { 0d, -1d }, 0, 0,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new[] { 0d, 1d }, 0, 0,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical)]
    [TestCase(1, 1, new[]
    {
        /*T1*/-1d, 0d, /*T2*/1d, 0d
    }, 0, 0, CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new[]
    {
        /*T1*/0d, -1d, /*T2*/0d, 1d
    }, 0, 0, CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal)]
    [TestCase(1, 1, new[]
    {
        /*T1*/-1d, 0d, /*T2*/0d, 1d
    }, 0, 0, CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical)]
    [TestCase(1, 1, new[]
    {
        /*T1*/1d, 0d, /*T2*/0d, -1d
    }, 0, 0, CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new[]
    {
        /*T1*/-1d, 0d, /*T2*/1d, 0d, /*T3*/0d, -1d, /*T4*/0d, 1d
    }, 0, 0, CollisionNormalFilter.None)]
    [TestCase(1, 1, new[]
    {
        /*T1*/-1d, 1d, /*T2*/1d, 1d, /*T3*/-1d, -1d, /*T4*/1d, -1d
    }, 0, 0, CollisionNormalFilter.All)]
    // Tile size is 2.5x3.5 and asserted tile is at (22.5, 38.5).
    [TestCase(2.5, 3.5, new double[0], 22.5, 38.5, CollisionNormalFilter.All)]
    [TestCase(2.5, 3.5, new[] { 20.0, 38.5 }, 22.5, 38.5,
        CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(2.5, 3.5, new[] { 25.0, 38.5 }, 22.5, 38.5,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(2.5, 3.5, new[] { 22.5, 35.0 }, 22.5, 38.5,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.PositiveVertical)]
    [TestCase(2.5, 3.5, new[] { 22.5, 42.0 }, 22.5, 38.5,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical)]
    [TestCase(2.5, 3.5, new[]
    {
        /*T1*/20.0, 38.5, /*T2*/25.0, 38.5
    }, 22.5, 38.5, CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(2.5, 3.5, new[]
    {
        /*T1*/22.5, 35.0, /*T2*/22.5, 42.0
    }, 22.5, 38.5, CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal)]
    [TestCase(2.5, 3.5, new[]
    {
        /*T1*/20.0, 38.5, /*T2*/22.5, 42.0
    }, 22.5, 38.5, CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical)]
    [TestCase(2.5, 3.5, new[]
    {
        /*T1*/25.0, 38.5, /*T2*/22.5, 35.0
    }, 22.5, 38.5, CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveVertical)]
    [TestCase(2.5, 3.5, new[]
    {
        /*T1*/20.0, 38.5, /*T2*/25.0, 38.5, /*T3*/22.5, 35.0, /*T4*/22.5, 42.0
    }, 22.5, 38.5, CollisionNormalFilter.None)]
    [TestCase(2.5, 3.5, new[]
    {
        /*T1*/20.0, 42.0, /*T2*/25.0, 42.0, /*T3*/20.0, 35.0, /*T4*/25.0, 35.0
    }, 22.5, 38.5, CollisionNormalFilter.All)]
    public void TileBody_CollisionNormalFilterIsUpdated_WhenTileBodyPositionIsChanged(double tw, double th, double[] tiles, double tx, double ty,
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
            var x = tiles[i];
            var y = tiles[i + 1];
            var tile = CreateTileStaticBody(x, y);
            Assert.That(tile.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(x, y)), "Tile is misaligned.");
        }

        var entity = CreateTileStaticBody(tx, ty);
        Assert.That(entity.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(tx, ty)), "Tile is misaligned.");

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var body = GetBodyForEntity(physicsSystem, entity);
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(collisionNormalFilter));
    }

    // Tile size is 1x1 and asserted tile is at (0, 0).
    [TestCase(1, 1, new double[0], 0, 0, CollisionNormalFilter.All)]
    [TestCase(1, 1, new[] { -1d, 0d }, 0, 0,
        CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new[] { 1d, 0d }, 0, 0,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new[] { 0d, -1d }, 0, 0,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new[] { 0d, 1d }, 0, 0,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical)]
    [TestCase(1, 1, new[]
    {
        /*T1*/-1d, 0d, /*T2*/1d, 0d
    }, 0, 0, CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new[]
    {
        /*T1*/0d, -1d, /*T2*/0d, 1d
    }, 0, 0, CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal)]
    [TestCase(1, 1, new[]
    {
        /*T1*/-1d, 0d, /*T2*/0d, 1d
    }, 0, 0, CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical)]
    [TestCase(1, 1, new[]
    {
        /*T1*/1d, 0d, /*T2*/0d, -1d
    }, 0, 0, CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveVertical)]
    [TestCase(1, 1, new[]
    {
        /*T1*/-1d, 0d, /*T2*/1d, 0d, /*T3*/0d, -1d, /*T4*/0d, 1d
    }, 0, 0, CollisionNormalFilter.None)]
    [TestCase(1, 1, new[]
    {
        /*T1*/-1d, 1d, /*T2*/1d, 1d, /*T3*/-1d, -1d, /*T4*/1d, -1d
    }, 0, 0, CollisionNormalFilter.All)]
    // Tile size is 2.5x3.5 and asserted tile is at (22.5, 38.5).
    [TestCase(2.5, 3.5, new double[0], 22.5, 38.5, CollisionNormalFilter.All)]
    [TestCase(2.5, 3.5, new[] { 20.0, 38.5 }, 22.5, 38.5,
        CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(2.5, 3.5, new[] { 25.0, 38.5 }, 22.5, 38.5,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(2.5, 3.5, new[] { 22.5, 35.0 }, 22.5, 38.5,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.PositiveVertical)]
    [TestCase(2.5, 3.5, new[] { 22.5, 42.0 }, 22.5, 38.5,
        CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical)]
    [TestCase(2.5, 3.5, new[]
    {
        /*T1*/20.0, 38.5, /*T2*/25.0, 38.5
    }, 22.5, 38.5, CollisionNormalFilter.NegativeVertical | CollisionNormalFilter.PositiveVertical)]
    [TestCase(2.5, 3.5, new[]
    {
        /*T1*/22.5, 35.0, /*T2*/22.5, 42.0
    }, 22.5, 38.5, CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveHorizontal)]
    [TestCase(2.5, 3.5, new[]
    {
        /*T1*/20.0, 38.5, /*T2*/22.5, 42.0
    }, 22.5, 38.5, CollisionNormalFilter.PositiveHorizontal | CollisionNormalFilter.NegativeVertical)]
    [TestCase(2.5, 3.5, new[]
    {
        /*T1*/25.0, 38.5, /*T2*/22.5, 35.0
    }, 22.5, 38.5, CollisionNormalFilter.NegativeHorizontal | CollisionNormalFilter.PositiveVertical)]
    [TestCase(2.5, 3.5, new[]
    {
        /*T1*/20.0, 38.5, /*T2*/25.0, 38.5, /*T3*/22.5, 35.0, /*T4*/22.5, 42.0
    }, 22.5, 38.5, CollisionNormalFilter.None)]
    [TestCase(2.5, 3.5, new[]
    {
        /*T1*/20.0, 42.0, /*T2*/25.0, 42.0, /*T3*/20.0, 35.0, /*T4*/25.0, 35.0
    }, 22.5, 38.5, CollisionNormalFilter.All)]
    public void TileBody_CollisionNormalFilterIsUpdated_WhenNeighbouringTileBodyPositionIsUpdated(double tw, double th, double[] tiles, double tx, double ty,
        object collisionNormalFilter)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(tw, th)
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        var entity = CreateTileStaticBody(tx, ty);
        Assert.That(entity.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(tx, ty)), "Tile is misaligned.");

        for (var i = 0; i < tiles.Length; i += 2)
        {
            var x = tiles[i];
            var y = tiles[i + 1];
            var tile = CreateTileStaticBody(x, y);
            Assert.That(tile.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(x, y)), "Tile is misaligned.");
        }

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