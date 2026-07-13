using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Spatial;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Spatial;

[TestFixture]
internal class SpatialGridTests
{
    [TestCase(20)]
    [TestCase(50)]
    public void Constructor_ShouldCreateGridWithSpecifiedSizeD(double cellSize)
    {
        // Arrange
        // Act
        var grid = new SpatialGrid<int>(cellSize);

        // Assert
        Assert.That(grid.CellSize.Width, Is.EqualTo(cellSize));
        Assert.That(grid.CellSize.Height, Is.EqualTo(cellSize));
    }

    [TestCase(20, 10)]
    [TestCase(50, 50)]
    public void Constructor_ShouldCreateGridWithSpecifiedSizeD(double width, double height)
    {
        // Arrange
        var size = new SizeD(width, height);

        // Act
        var grid = new SpatialGrid<int>(size);

        // Assert
        Assert.That(grid.CellSize, Is.EqualTo(size));
    }
}