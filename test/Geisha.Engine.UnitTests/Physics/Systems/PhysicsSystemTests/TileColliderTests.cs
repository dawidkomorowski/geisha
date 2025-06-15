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

    [Test]
    public void TileBody_ShouldHaveCollisionNormalFilterSetToAll_WhenNoSurroundingTiles()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(1, 1)
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
        var entity = CreateTileStaticBody(0, 0);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var body = GetBodyForEntity(physicsSystem, entity);
        Assert.That(body.CollisionNormalFilter, Is.EqualTo(CollisionNormalFilter.All));
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