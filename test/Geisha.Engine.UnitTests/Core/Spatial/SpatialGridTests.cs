using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Spatial;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Spatial;

[TestFixture]
internal class SpatialGridTests
{
    [Test]
    public void Constructor_ShouldCreateGridWithSpecifiedSize()
    {
        // Arrange
        var size = new SizeD(20, 10);

        // Act
        var grid = new SpatialGrid(size);

        // Assert
        Assert.That(grid.CellSize, Is.EqualTo(size));
    }
}