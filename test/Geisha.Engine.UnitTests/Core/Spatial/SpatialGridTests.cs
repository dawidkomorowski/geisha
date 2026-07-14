using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Spatial;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Spatial;

// TODO: Some test cases duplicate each other - figure out how to improve that.
[TestFixture]
internal class SpatialGridTests
{
    [TestCase(20)]
    [TestCase(50)]
    public void Constructor_ShouldCreateGridWithSpecifiedSize_GivenScalarSize(double cellSize)
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
    public void Constructor_ShouldCreateGridWithSpecifiedSize_GivenSizeDSize(double width, double height)
    {
        // Arrange
        var size = new SizeD(width, height);

        // Act
        var grid = new SpatialGrid<int>(size);

        // Assert
        Assert.That(grid.CellSize, Is.EqualTo(size));
    }

    [TestCase(20)]
    [TestCase(50)]
    public void Constructor_ShouldCreateGridWithSpecifiedSizeAndCapacity_GivenScalarSize(double cellSize)
    {
        // Arrange
        const int capacity = 10;

        // Act
        var grid = new SpatialGrid<int>(cellSize, capacity);

        // Assert
        Assert.That(grid.CellSize.Width, Is.EqualTo(cellSize));
        Assert.That(grid.CellSize.Height, Is.EqualTo(cellSize));
    }

    [TestCase(20, 10)]
    [TestCase(50, 50)]
    public void Constructor_ShouldCreateGridWithSpecifiedSizeAndCapacity_GivenSizeDSize(double width, double height)
    {
        // Arrange
        var size = new SizeD(width, height);
        const int capacity = 10;

        // Act
        var grid = new SpatialGrid<int>(size, capacity);

        // Assert
        Assert.That(grid.CellSize, Is.EqualTo(size));
    }

    [Test]
    public void Constructor_ShouldCreateEmptyGrid_GivenZeroCapacity()
    {
        // Arrange
        // Act
        var grid = new SpatialGrid<int>(new SizeD(20, 20), 0);

        // Assert
        Assert.That(grid.CellSize, Is.EqualTo(new SizeD(20, 20)));
    }

    [Test]
    public void Constructor_ShouldThrowArgumentException_GivenNegativeCapacity()
    {
        // Arrange
        // Act
        // Assert
        Assert.That(() => new SpatialGrid<int>(new SizeD(20, 20), -1), Throws.ArgumentException);
    }

    [Test]
    public void IsValidProxy_ShouldReturnFalse_GivenNullProxyId()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20);

        // Act
        var isValid = grid.IsValidProxy(SpatialGridProxyId.Null);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    public void IsValidProxy_ShouldReturnTrue_GivenIdOfCreatedProxy()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20);
        var bounds = new AABB2D(0, 0, 10, 10);
        var id = grid.CreateProxy(in bounds, 42);

        // Act
        var isValid = grid.IsValidProxy(id);

        // Assert
        Assert.That(isValid, Is.True);
    }

    [Test]
    public void IsValidProxy_ShouldReturnFalse_GivenIdOfDestroyedProxy()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20);
        var bounds = new AABB2D(0, 0, 10, 10);
        var id = grid.CreateProxy(in bounds, 42);
        grid.DestroyProxy(id);

        // Act
        var isValid = grid.IsValidProxy(id);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    public void CreateProxy_ShouldCreateProxyWithSpecifiedBoundsAndPayload()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20);
        var bounds = new AABB2D(0, 0, 10, 10);

        // Act
        var id = grid.CreateProxy(in bounds, 42);

        // Assert
        var proxyData = grid.GetProxyData(id);
        Assert.That(proxyData.Bounds, Is.EqualTo(bounds));
        Assert.That(proxyData.Payload, Is.EqualTo(42));
    }

    [Test]
    public void CreateProxy_ShouldCreateMultipleProxiesWithDistinctIds()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20, 4);
        var bounds = new AABB2D(0, 0, 10, 10);

        // Act
        var id1 = grid.CreateProxy(in bounds, 1);
        var id2 = grid.CreateProxy(in bounds, 2);
        var id3 = grid.CreateProxy(in bounds, 3);

        // Assert
        Assert.That(id1, Is.Not.EqualTo(id2));
        Assert.That(id2, Is.Not.EqualTo(id3));
        Assert.That(grid.GetProxyData(id1).Payload, Is.EqualTo(1));
        Assert.That(grid.GetProxyData(id2).Payload, Is.EqualTo(2));
        Assert.That(grid.GetProxyData(id3).Payload, Is.EqualTo(3));
    }

    [Test]
    public void CreateProxy_ShouldThrowNotImplementedException_WhenCapacityIsExceeded()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20, 1);
        var bounds = new AABB2D(0, 0, 10, 10);
        grid.CreateProxy(in bounds, 1);

        // Act
        // Assert
        Assert.That(() => grid.CreateProxy(in bounds, 2), Throws.TypeOf<NotImplementedException>());
    }

    [Test]
    public void CreateProxy_ShouldThrowNotImplementedException_WhenCapacityIsZero()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20, 0);
        var bounds = new AABB2D(0, 0, 10, 10);

        // Act
        // Assert
        Assert.That(() => grid.CreateProxy(in bounds, 1), Throws.TypeOf<NotImplementedException>());
    }

    [Test]
    public void CreateProxy_ShouldReuseDestroyedProxySlot()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20, 1);
        var bounds = new AABB2D(0, 0, 10, 10);
        var id1 = grid.CreateProxy(in bounds, 1);
        grid.DestroyProxy(id1);

        // Act
        var id2 = grid.CreateProxy(in bounds, 2);

        // Assert
        Assert.That(id2.Index, Is.EqualTo(id1.Index));
        Assert.That(id2.Version, Is.Not.EqualTo(id1.Version));
        Assert.That(grid.IsValidProxy(id1), Is.False);
        Assert.That(grid.IsValidProxy(id2), Is.True);
    }

    [Test]
    public void DestroyProxy_ShouldMakeProxyIdInvalid()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20);
        var bounds = new AABB2D(0, 0, 10, 10);
        var id = grid.CreateProxy(in bounds, 42);

        // Act
        grid.DestroyProxy(id);

        // Assert
        Assert.That(grid.IsValidProxy(id), Is.False);
    }

    [Test]
    public void DestroyProxy_ShouldThrowInvalidOperationException_GivenNullProxyId()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20);

        // Act
        // Assert
        Assert.That(() => grid.DestroyProxy(SpatialGridProxyId.Null), Throws.InvalidOperationException);
    }

    [Test]
    public void DestroyProxy_ShouldThrowInvalidOperationException_WhenProxyAlreadyDestroyed()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20);
        var bounds = new AABB2D(0, 0, 10, 10);
        var id = grid.CreateProxy(in bounds, 42);
        grid.DestroyProxy(id);

        // Act
        // Assert
        Assert.That(() => grid.DestroyProxy(id), Throws.InvalidOperationException);
    }

    [Test]
    public void GetProxyData_ShouldReturnBoundsAndPayload_GivenValidProxyId()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20);
        var bounds = new AABB2D(1, 2, 3, 4);
        var id = grid.CreateProxy(in bounds, 123);

        // Act
        var proxyData = grid.GetProxyData(id);

        // Assert
        Assert.That(proxyData.Bounds, Is.EqualTo(bounds));
        Assert.That(proxyData.Payload, Is.EqualTo(123));
    }

    [Test]
    public void GetProxyData_ShouldThrowInvalidOperationException_GivenNullProxyId()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20);

        // Act
        // Assert
        Assert.That(() => grid.GetProxyData(SpatialGridProxyId.Null), Throws.InvalidOperationException);
    }

    [Test]
    public void GetProxyData_ShouldThrowInvalidOperationException_GivenIdOfDestroyedProxy()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20);
        var bounds = new AABB2D(0, 0, 10, 10);
        var id = grid.CreateProxy(in bounds, 42);
        grid.DestroyProxy(id);

        // Act
        // Assert
        Assert.That(() => grid.GetProxyData(id), Throws.InvalidOperationException);
    }

    [Test]
    public void MoveProxy_ShouldUpdateBoundsOfProxy()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20);
        var bounds = new AABB2D(0, 0, 10, 10);
        var id = grid.CreateProxy(in bounds, 42);
        var newBounds = new AABB2D(5, 5, 15, 15);

        // Act
        grid.MoveProxy(id, in newBounds);

        // Assert
        var proxyData = grid.GetProxyData(id);
        Assert.That(proxyData.Bounds, Is.EqualTo(newBounds));
        Assert.That(proxyData.Payload, Is.EqualTo(42));
    }

    [Test]
    public void MoveProxy_ShouldThrowInvalidOperationException_GivenNullProxyId()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20);
        var newBounds = new AABB2D(0, 0, 10, 10);

        // Act
        // Assert
        Assert.That(() => grid.MoveProxy(SpatialGridProxyId.Null, in newBounds), Throws.InvalidOperationException);
    }

    [Test]
    public void MoveProxy_ShouldThrowInvalidOperationException_GivenIdOfDestroyedProxy()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20);
        var bounds = new AABB2D(0, 0, 10, 10);
        var id = grid.CreateProxy(in bounds, 42);
        grid.DestroyProxy(id);

        // Act
        // Assert
        Assert.That(() => grid.MoveProxy(id, in bounds), Throws.InvalidOperationException);
    }

    [Test]
    public void QueryOverlappingPairs_HappyPath()
    {
        // Arrange
        var grid = new SpatialGrid<int>(10);
        var id = grid.CreateProxy(new AABB2D(0, 0, 10, 10), 1);

        // Act
        var queryResults = new List<PairQueryResult>();
        var queryHandler = new PairListQueryHandler(queryResults);
        grid.QueryOverlappingPairs(ref queryHandler);

        // Assert
        Assert.That(queryResults, Is.Empty);
    }

    private readonly record struct PairQueryResult
    {
        public PairQueryResult(SpatialGridProxyId proxyId1, SpatialGridProxyId proxyId2)
        {
            ProxyId1 = proxyId1;
            ProxyId2 = proxyId2;
        }

        private SpatialGridProxyId ProxyId1 { get; init; }
        private SpatialGridProxyId ProxyId2 { get; init; }
    }

    private readonly struct PairListQueryHandler : IPairsQueryHandler
    {
        private readonly List<PairQueryResult> _pairs;

        public PairListQueryHandler(List<PairQueryResult> pairs)
        {
            _pairs = pairs;
        }

        public bool Handle(SpatialGridProxyId proxyId1, SpatialGridProxyId proxyId2)
        {
            _pairs.Add(new PairQueryResult(proxyId1, proxyId2));
            return true;
        }
    }
}