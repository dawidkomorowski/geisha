using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.PhysicsEngine2D;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.PhysicsEngine2D;

[TestFixture]
public class PhysicsEngine2DTests
{
    [Test]
    public void PhysicsScene_IsInitializedAccordingToDefinition()
    {
        // Arrange
        var sceneDefinition = new PhysicsScene2DDefinition
        {
            Substeps = 123,
            PositionIterations = 456,
            VelocityIterations = 789,
            PenetrationTolerance = 123.456,
            TileSize = new SizeD(12, 34)
        };

        // Act
        var scene = PhysicsScene2D.Create(sceneDefinition);

        // Assert
        Assert.That(scene.Substeps, Is.EqualTo(sceneDefinition.Substeps));
        Assert.That(scene.PositionIterations, Is.EqualTo(sceneDefinition.PositionIterations));
        Assert.That(scene.VelocityIterations, Is.EqualTo(sceneDefinition.VelocityIterations));
        Assert.That(scene.PenetrationTolerance, Is.EqualTo(sceneDefinition.PenetrationTolerance));
        Assert.That(scene.TileSize, Is.EqualTo(sceneDefinition.TileSize));
    }

    [Test]
    public void PhysicsScene_SimulationParameters_CanBeSetAndGet()
    {
        // Arrange
        var scene = PhysicsScene2D.Create(default);

        // Act
        scene.Substeps = 123;
        scene.PositionIterations = 456;
        scene.VelocityIterations = 789;
        scene.PenetrationTolerance = 123.456;

        // Assert
        Assert.That(scene.Substeps, Is.EqualTo(123));
        Assert.That(scene.PositionIterations, Is.EqualTo(456));
        Assert.That(scene.VelocityIterations, Is.EqualTo(789));
        Assert.That(scene.PenetrationTolerance, Is.EqualTo(123.456));
    }

    [Test]
    public void PhysicsScene_DefaultHandle_IsNotValid()
    {
        // Arrange
        var scene = default(PhysicsScene2D);

        // Act & Assert
        Assert.That(scene.IsValid, Is.False);
    }

    [Test]
    public void PhysicsScene_CreatedScene_IsValid()
    {
        // Arrange & Act
        var scene = PhysicsScene2D.Create(default);

        // Assert
        Assert.That(scene.IsValid, Is.True);
    }

    [Test]
    public void PhysicsScene_AfterDestroy_IsNotValid()
    {
        // Arrange
        var scene = PhysicsScene2D.Create(default);

        // Act
        PhysicsScene2D.Destroy(scene);

        // Assert
        Assert.That(scene.IsValid, Is.False);
    }

    [Test]
    public void PhysicsScene_StaleHandleAfterRecreation_IsNotValid()
    {
        // Arrange
        var staleScene = PhysicsScene2D.Create(default);
        PhysicsScene2D.Destroy(staleScene);

        // Act - creating a new scene reuses the freed slot (LIFO free list)
        var newScene = PhysicsScene2D.Create(default);

        // Assert
        Assert.That(staleScene.IsValid, Is.False);
        Assert.That(newScene.IsValid, Is.True);
    }

    [Test]
    public void RigidBody_DefaultHandle_IsNotValid()
    {
        // Arrange
        var body = default(RigidBody2D);

        // Act & Assert
        Assert.That(body.IsValid, Is.False);
    }

    [Test]
    public void RigidBody_CreatedBody_IsValid()
    {
        // Arrange
        var scene = PhysicsScene2D.Create(default);

        // Act
        var body = scene.CreateBody(BodyType.Static, 1.0);

        // Assert
        Assert.That(body.IsValid, Is.True);
    }

    [Test]
    public void RigidBody_AfterDestroyBody_IsNotValid()
    {
        // Arrange
        var scene = PhysicsScene2D.Create(default);
        var body = scene.CreateBody(BodyType.Static, 1.0);

        // Act
        scene.DestroyBody(body);

        // Assert
        Assert.That(body.IsValid, Is.False);
    }

    [Test]
    public void RigidBody_StaleHandleAfterBodyRecreation_IsNotValid()
    {
        // Arrange
        var scene = PhysicsScene2D.Create(default);
        var staleBody = scene.CreateBody(BodyType.Static, 1.0);
        scene.DestroyBody(staleBody);

        // Act - creating a new body reuses the freed body slot (LIFO free list)
        var newBody = scene.CreateBody(BodyType.Static, 1.0);

        // Assert
        Assert.That(staleBody.IsValid, Is.False);
        Assert.That(newBody.IsValid, Is.True);
    }

    [Test]
    public void RigidBody_AfterDestroyScene_IsNotValid()
    {
        // Arrange
        var scene = PhysicsScene2D.Create(default);
        var body = scene.CreateBody(BodyType.Static, 1.0);

        // Act
        PhysicsScene2D.Destroy(scene);

        // Assert
        Assert.That(body.IsValid, Is.False);
    }
}