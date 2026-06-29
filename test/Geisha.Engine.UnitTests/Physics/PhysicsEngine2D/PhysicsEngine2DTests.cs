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
        Assert.That(scene.Substeps, Is.EqualTo(scene.Substeps));
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
}