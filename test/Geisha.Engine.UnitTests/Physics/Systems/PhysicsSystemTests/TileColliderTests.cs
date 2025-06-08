using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics;
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

    // TODO
    [Test]
    public void TODO()
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(1, 1)
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        CreateTileStaticBody(0, 0);
        // Act
        // Assert
    }
}