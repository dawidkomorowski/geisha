using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Spatial;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Spatial;

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
    public void IsValidProxy_ShouldReturnFalse_ForOldId_AfterSlotIsReusedByNewProxy()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20);
        var bounds = new AABB2D(0, 0, 10, 10);
        var oldId = grid.CreateProxy(in bounds, 1);
        grid.DestroyProxy(oldId);

        // Reuse the same slot
        var newId = grid.CreateProxy(in bounds, 2);

        // Act
        var oldIsValid = grid.IsValidProxy(oldId);
        var newIsValid = grid.IsValidProxy(newId);

        // Assert
        Assert.That(oldId.Index, Is.EqualTo(newId.Index)); // same slot
        Assert.That(oldIsValid, Is.False);
        Assert.That(newIsValid, Is.True);
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
    public void CreateProxy_ShouldSucceed_WhenCapacityIsExceeded()
    {
        // Arrange
        // Capacity 4 matches the internal DefaultCapacity so the proxy pool is fully consumed
        // after 4 creates, and the 5th genuinely triggers reallocation.
        const int capacity = 4;
        var grid = new SpatialGrid<int>(20, capacity);
        var bounds = new AABB2D(0, 0, 10, 10);
        for (var i = 0; i < capacity; i++) grid.CreateProxy(in bounds, i);

        // Act
        var id = grid.CreateProxy(in bounds, 99);

        // Assert
        Assert.That(grid.IsValidProxy(id), Is.True);
        Assert.That(grid.GetProxyData(id).Payload, Is.EqualTo(99));
    }

    [Test]
    public void CreateProxy_ShouldSucceed_WhenCapacityIsZero()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20, 0);
        var bounds = new AABB2D(0, 0, 10, 10);

        // Act
        var id = grid.CreateProxy(in bounds, 1);

        // Assert
        Assert.That(grid.IsValidProxy(id), Is.True);
        Assert.That(grid.GetProxyData(id).Payload, Is.EqualTo(1));
    }

    [Test]
    public void CreateProxy_ShouldKeepExistingProxiesValid_WhenReallocationOccurs()
    {
        // Arrange
        // Capacity 4 matches the internal DefaultCapacity so the proxy pool is fully consumed
        // after 4 creates, and the 5th genuinely triggers reallocation.
        const int capacity = 4;
        var grid = new SpatialGrid<int>(20, capacity);
        var bounds = new AABB2D(0, 0, 10, 10);
        var existingIds = new SpatialGridProxyId[capacity];
        for (var i = 0; i < capacity; i++) existingIds[i] = grid.CreateProxy(in bounds, i);

        // Act — triggers reallocation
        var newId = grid.CreateProxy(in bounds, 99);

        // Assert
        for (var i = 0; i < capacity; i++)
        {
            Assert.That(grid.IsValidProxy(existingIds[i]), Is.True);
            Assert.That(grid.GetProxyData(existingIds[i]).Payload, Is.EqualTo(i));
        }

        Assert.That(grid.IsValidProxy(newId), Is.True);
        Assert.That(grid.GetProxyData(newId).Payload, Is.EqualTo(99));
    }

    [Test]
    public void CreateProxy_ShouldSupportMultipleReallocations()
    {
        // Arrange
        var grid = new SpatialGrid<int>(20, 1);
        var bounds = new AABB2D(0, 0, 10, 10);

        // Act — fill well beyond the initial capacity to force multiple reallocations
        var ids = new SpatialGridProxyId[10];
        for (var i = 0; i < ids.Length; i++)
        {
            ids[i] = grid.CreateProxy(in bounds, i);
        }

        // Assert
        for (var i = 0; i < ids.Length; i++)
        {
            Assert.That(grid.IsValidProxy(ids[i]), Is.True);
            Assert.That(grid.GetProxyData(ids[i]).Payload, Is.EqualTo(i));
        }
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
    public void QueryOverlappingPairs_ShouldReturnNoPairs_WhenGridIsEmpty()
    {
        // Arrange
        var grid = new SpatialGrid<int>(10);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        Assert.That(queryResults, Is.Empty);
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnNoPairs_WhenGridHasOnlySingleProxy()
    {
        // Arrange
        var grid = new SpatialGrid<int>(10);
        grid.CreateProxy(new AABB2D(0, 0, 10, 10), 1);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        Assert.That(queryResults, Is.Empty);
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnNoPairs_WhenProxiesDoNotOverlapAndAreInSameCell()
    {
        // Arrange
        var grid = new SpatialGrid<int>(100);
        grid.CreateProxy(new AABB2D(0, 0, 5, 5), 1);
        grid.CreateProxy(new AABB2D(10, 10, 15, 15), 2);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        Assert.That(queryResults, Is.Empty);
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnNoPairs_WhenProxiesDoNotOverlapAndAreInDifferentCells()
    {
        // Arrange
        var grid = new SpatialGrid<int>(10);
        grid.CreateProxy(new AABB2D(0, 0, 5, 5), 1);
        grid.CreateProxy(new AABB2D(100, 100, 105, 105), 2);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        Assert.That(queryResults, Is.Empty);
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnPair_WhenTwoProxiesOverlapWithinSingleCell()
    {
        // Arrange
        var grid = new SpatialGrid<int>(100);
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 10, 10), 1);
        var id2 = grid.CreateProxy(new AABB2D(5, 5, 15, 15), 2);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnPair_WhenProxiesOnlyTouchAtEdge()
    {
        // Arrange
        var grid = new SpatialGrid<int>(100);
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 10, 10), 1);
        var id2 = grid.CreateProxy(new AABB2D(10, 0, 20, 10), 2);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnNoPairs_WhenProxiesAreInAdjacentCellsButDoNotOverlap()
    {
        // Arrange
        var grid = new SpatialGrid<int>(10);
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 9, 9), 1);
        var id2 = grid.CreateProxy(new AABB2D(11, 0, 20, 9), 2);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        Assert.That(queryResults, Is.Empty);
        Assert.That(grid.IsValidProxy(id1), Is.True);
        Assert.That(grid.IsValidProxy(id2), Is.True);
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnPair_WhenProxiesOverlapAcrossMultipleCells()
    {
        // Arrange
        var grid = new SpatialGrid<int>(10);
        // Proxy 1 spans multiple cells (bigger than a single cell).
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 25, 25), 1);
        // Proxy 2 is located in one of the cells spanned by proxy 1, far from its origin cell.
        var id2 = grid.CreateProxy(new AABB2D(20, 20, 30, 30), 2);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnUniquePair_WhenBothProxiesSpanMultipleSharedCells()
    {
        // Arrange
        var grid = new SpatialGrid<int>(10);
        // Both proxies span multiple cells and share several of them, which could cause the pair
        // to be reported multiple times if duplicates are not filtered out.
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 25, 25), 1);
        var id2 = grid.CreateProxy(new AABB2D(5, 5, 30, 30), 2);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnAllPairs_WhenMultipleProxiesOverlap()
    {
        // Arrange
        var grid = new SpatialGrid<int>(100);
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 10, 10), 1);
        var id2 = grid.CreateProxy(new AABB2D(5, 5, 15, 15), 2);
        var id3 = grid.CreateProxy(new AABB2D(8, 8, 18, 18), 3);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2), (id1, id3), (id2, id3));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnOnlyOverlappingPairs_WhenSomeProxiesDoNotOverlap()
    {
        // Arrange
        var grid = new SpatialGrid<int>(100);
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 10, 10), 1);
        var id2 = grid.CreateProxy(new AABB2D(5, 5, 15, 15), 2);
        _ = grid.CreateProxy(new AABB2D(50, 50, 60, 60), 3);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnPair_WhenProxyIsMovedIntoOverlap()
    {
        // Arrange
        var grid = new SpatialGrid<int>(100);
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 10, 10), 1);
        var id2 = grid.CreateProxy(new AABB2D(50, 50, 60, 60), 2);

        // Act
        grid.MoveProxy(id2, new AABB2D(5, 5, 15, 15));
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnNoPairs_WhenProxyIsMovedOutOfOverlap()
    {
        // Arrange
        var grid = new SpatialGrid<int>(100);
        _ = grid.CreateProxy(new AABB2D(0, 0, 10, 10), 1);
        var id2 = grid.CreateProxy(new AABB2D(5, 5, 15, 15), 2);

        // Act
        grid.MoveProxy(id2, new AABB2D(50, 50, 60, 60));
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        Assert.That(queryResults, Is.Empty);
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnPair_WhenProxyIsMovedIntoOverlapAcrossCellBoundary()
    {
        // Arrange
        // Cell size 10: id1 is in cell (0,0), id2 starts in cell (10,10) — no overlap.
        var grid = new SpatialGrid<int>(10);
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 9, 9), 1);
        var id2 = grid.CreateProxy(new AABB2D(100, 100, 109, 109), 2);

        // Act
        // Move id2 across a cell boundary into the cell of id1, overlapping it.
        grid.MoveProxy(id2, new AABB2D(5, 5, 14, 14));
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnNoPairs_WhenProxyIsMovedOutOfOverlapAcrossCellBoundary()
    {
        // Arrange
        // Cell size 10: both proxies start in the same cell and overlap.
        var grid = new SpatialGrid<int>(10);
        _ = grid.CreateProxy(new AABB2D(0, 0, 9, 9), 1);
        var id2 = grid.CreateProxy(new AABB2D(5, 5, 14, 14), 2);

        // Act
        // Move id2 across a cell boundary far away so it no longer overlaps id1.
        grid.MoveProxy(id2, new AABB2D(100, 100, 109, 109));
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        Assert.That(queryResults, Is.Empty);
    }

    [Test]
    public void QueryOverlappingPairs_ShouldNotIncludeDestroyedProxy_WhenItPreviouslyOverlapped()
    {
        // Arrange
        var grid = new SpatialGrid<int>(100);
        _ = grid.CreateProxy(new AABB2D(0, 0, 10, 10), 1);
        var id2 = grid.CreateProxy(new AABB2D(5, 5, 15, 15), 2);

        // Act
        grid.DestroyProxy(id2);
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        Assert.That(queryResults, Is.Empty);
    }

    // Reproduction of bug: CellRange.Enumerator uses MaxX instead of MaxY for Y-axis termination.
    // A proxy that spans more rows than columns (taller than wide in cell-space) exercises this.
    [Test]
    [Description("Reproduction of bug: CellRange.Enumerator uses MaxX instead of MaxY for Y-axis termination.")]
    public void QueryOverlappingPairs_ShouldReturnPair_WhenProxySpansMoreRowsThanColumns()
    {
        // Arrange
        // Cell size 10x10. Proxy 1 spans cells x=[0..0], y=[0..2] (1 column, 3 rows).
        // Proxy 2 sits in cell y=2 — only reachable if the enumerator terminates on MaxY, not MaxX.
        var grid = new SpatialGrid<int>(10);
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 9, 29), 1); // 1 col wide, 3 rows tall
        var id2 = grid.CreateProxy(new AABB2D(0, 25, 9, 34), 2); // overlaps id1 in row y=2

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnPair_WhenProxiesTouchAtCellBoundaryEdge()
    {
        // Arrange
        // Cell size 10. Proxies are in adjacent cells and touch exactly at x=10 (a cell boundary).
        // The intersection is a degenerate vertical line; canonical cell must be one both share.
        var grid = new SpatialGrid<int>(10);
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 10, 9), 1); // in cells x=0 and x=1
        var id2 = grid.CreateProxy(new AABB2D(10, 0, 20, 9), 2); // in cells x=1 and x=2

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldStopReporting_WhenHandlerReturnsFalse()
    {
        // Arrange
        var grid = new SpatialGrid<int>(100);
        grid.CreateProxy(new AABB2D(0, 0, 10, 10), 1);
        grid.CreateProxy(new AABB2D(5, 5, 15, 15), 2);
        grid.CreateProxy(new AABB2D(8, 8, 18, 18), 3);

        // Act
        var queryResults = new List<PairQueryResult>();
        var queryHandler = new LimitedPairListQueryHandler(queryResults, maxPairs: 1);
        grid.QueryOverlappingPairs(ref queryHandler);

        // Assert
        Assert.That(queryResults, Has.Count.EqualTo(1));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnPair_WhenProxiesTouchAtCorner()
    {
        // Arrange
        // Both proxies share only a single corner point at (10, 10).
        var grid = new SpatialGrid<int>(100);
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 10, 10), 1);
        var id2 = grid.CreateProxy(new AABB2D(10, 10, 20, 20), 2);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReportNewProxy_WhenSlotIsReusedAfterDestroy()
    {
        // Arrange
        var grid = new SpatialGrid<int>(100);
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 10, 10), 1);
        var id2 = grid.CreateProxy(new AABB2D(5, 5, 15, 15), 2);
        grid.DestroyProxy(id2);

        // Reuse the slot — new proxy overlaps id1, old id2 must not appear.
        var id3 = grid.CreateProxy(new AABB2D(5, 5, 15, 15), 3);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        Assert.That(grid.IsValidProxy(id2), Is.False);
        AssertPairsEquivalent(queryResults, (id1, id3));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnPair_WhenProxyIsMovedToSameBounds()
    {
        // Arrange
        var grid = new SpatialGrid<int>(100);
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 10, 10), 1);
        var id2 = grid.CreateProxy(new AABB2D(5, 5, 15, 15), 2);

        // Act — no-op move: same bounds
        grid.MoveProxy(id2, new AABB2D(5, 5, 15, 15));
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnPair_WhenMultiCellProxyIsMovedIntoOverlap()
    {
        // Arrange
        // Cell size 10. Proxy 1 spans multiple cells; proxy 2 starts far away and is moved
        // to a multi-cell region that overlaps proxy 1.
        var grid = new SpatialGrid<int>(10);
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 25, 25), 1); // spans cells (0,0)-(2,2)
        var id2 = grid.CreateProxy(new AABB2D(100, 100, 125, 125), 2);

        // Act
        grid.MoveProxy(id2, new AABB2D(20, 20, 45, 45)); // spans cells (2,2)-(4,4), overlaps id1 at (2,2)
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnNoPairs_WhenMultiCellProxyIsMovedOutOfOverlap()
    {
        // Arrange
        var grid = new SpatialGrid<int>(10);
        _ = grid.CreateProxy(new AABB2D(0, 0, 25, 25), 1);
        var id2 = grid.CreateProxy(new AABB2D(20, 20, 45, 45), 2);

        // Act
        grid.MoveProxy(id2, new AABB2D(100, 100, 125, 125));
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        Assert.That(queryResults, Is.Empty);
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnPair_WhenProxiesAreAtNegativeCoordinates()
    {
        // Arrange
        var grid = new SpatialGrid<int>(10);
        var id1 = grid.CreateProxy(new AABB2D(-20, -20, -10, -10), 1);
        var id2 = grid.CreateProxy(new AABB2D(-15, -15, -5, -5), 2);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnNoPairs_WhenProxiesAreAtNegativeCoordinatesAndDoNotOverlap()
    {
        // Arrange
        var grid = new SpatialGrid<int>(10);
        grid.CreateProxy(new AABB2D(-30, -30, -21, -21), 1);
        grid.CreateProxy(new AABB2D(-10, -10, -1, -1), 2);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        Assert.That(queryResults, Is.Empty);
    }

    [Test]
    public void QueryOverlappingPairs_ShouldReturnPair_WhenProxiesSpanNegativeAndPositiveCoordinates()
    {
        // Arrange
        var grid = new SpatialGrid<int>(10);
        var id1 = grid.CreateProxy(new AABB2D(-5, -5, 5, 5), 1);
        var id2 = grid.CreateProxy(new AABB2D(-3, -3, 3, 3), 2);

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    [Test]
    public void CreateProxy_ShouldRegisterInCorrectCells_WhenBoundsMaxIsExactlyOnCellBoundary()
    {
        // Arrange
        // Cell size 10. Proxy max at x=10 maps to cell x=1 via Floor(10/10)=1, so the proxy
        // spans cells x=0 and x=1. A second proxy in cell x=1 should be reported as overlapping.
        var grid = new SpatialGrid<int>(10);
        var id1 = grid.CreateProxy(new AABB2D(0, 0, 10, 9), 1); // spans cells x=0 and x=1
        var id2 = grid.CreateProxy(new AABB2D(10, 0, 19, 9), 2); // in cell x=1

        // Act
        var queryResults = QueryOverlappingPairs(grid);

        // Assert
        AssertPairsEquivalent(queryResults, (id1, id2));
    }

    private static List<PairQueryResult> QueryOverlappingPairs(SpatialGrid<int> grid)
    {
        var queryResults = new List<PairQueryResult>();
        var queryHandler = new PairListQueryHandler(queryResults);
        grid.QueryOverlappingPairs(ref queryHandler);
        return queryResults;
    }

    private static void AssertPairsEquivalent(IReadOnlyList<PairQueryResult> actual,
        params (SpatialGridProxyId ProxyId1, SpatialGridProxyId ProxyId2)[] expectedPairs)
    {
        var actualNormalized = actual.Select(p => NormalizePair(p.ProxyId1, p.ProxyId2));
        var expectedNormalized = expectedPairs.Select(p => NormalizePair(p.ProxyId1, p.ProxyId2));

        Assert.That(actualNormalized, Is.EquivalentTo(expectedNormalized));
    }

    private static (SpatialGridProxyId, SpatialGridProxyId) NormalizePair(SpatialGridProxyId proxyId1, SpatialGridProxyId proxyId2)
    {
        return proxyId1.Index <= proxyId2.Index ? (proxyId1, proxyId2) : (proxyId2, proxyId1);
    }

    private readonly record struct PairQueryResult(SpatialGridProxyId ProxyId1, SpatialGridProxyId ProxyId2);

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

    private readonly struct LimitedPairListQueryHandler : IPairsQueryHandler
    {
        private readonly List<PairQueryResult> _pairs;
        private readonly int _maxPairs;

        public LimitedPairListQueryHandler(List<PairQueryResult> pairs, int maxPairs)
        {
            _pairs = pairs;
            _maxPairs = maxPairs;
        }

        public bool Handle(SpatialGridProxyId proxyId1, SpatialGridProxyId proxyId2)
        {
            _pairs.Add(new PairQueryResult(proxyId1, proxyId2));
            return _pairs.Count < _maxPairs;
        }
    }
}