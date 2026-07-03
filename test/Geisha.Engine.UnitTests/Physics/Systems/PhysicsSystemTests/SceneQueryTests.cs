using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class SceneQueryTests : PhysicsSystemTestsBase
{
    #region QueryPoint

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty buffer -> 0 written
    [TestCase(5, 0, 0)] // 5 hits, buffer size 0 -> 0 written
    [TestCase(5, 2, 2)] // 5 hits, buffer size 2 -> 2 written
    [TestCase(5, 5, 5)] // 5 hits, buffer size 5 -> 5 written
    [TestCase(5, 8, 5)] // 5 hits, buffer size 8 -> 5 written
    public void QueryPoint_ShouldWriteCollidersIntoSpan(int collidersCount, int bufferSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var pointToQuery = Vector2.Zero;
        var colliders = new Collider2DComponent[bufferSize];

        // Act
        var written = physicsSystem.QueryPoint(pointToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(expectedWritten));

        for (var i = 0; i < written; i++)
        {
            Assert.That(colliders[i].ContainsPoint(pointToQuery), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty list -> 0 written, capacity unchanged
    [TestCase(5, 0, 5)] // 5 hits, empty list -> 5 written, capacity grows
    [TestCase(5, 2, 5)] // 5 hits, list size 2 -> 5 written, capacity grows
    [TestCase(5, 5, 5)] // 5 hits, list size 5 -> 5 written, capacity unchanged
    [TestCase(5, 8, 5)] // 5 hits, list size 8 -> 5 written, capacity unchanged
    public void QueryPoint_ShouldWriteCollidersIntoList(int collidersCount, int initialListSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var pointToQuery = Vector2.Zero;

        var fillerCollider = CreateCircleStaticBody(100, 100, 10).GetComponent<CircleColliderComponent>();
        Assert.That(fillerCollider.ContainsPoint(pointToQuery), Is.False);

        var colliders = new List<Collider2DComponent>(initialListSize);
        for (var i = 0; i < initialListSize; i++)
        {
            colliders.Add(fillerCollider);
        }

        var initialCapacity = colliders.Capacity;

        // Act
        var written = physicsSystem.QueryPoint(pointToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(expectedWritten));
        Assert.That(colliders.Count, Is.EqualTo(written));
        AssertListCapacityBehavior(initialCapacity, written, colliders.Capacity);

        for (var i = 0; i < written; i++)
        {
            Assert.That(colliders[i].ContainsPoint(pointToQuery), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty buffer -> empty span
    [TestCase(5, 0, 0)] // 5 hits, buffer size 0 -> empty span
    [TestCase(5, 2, 2)] // 5 hits, buffer size 2 -> span length 2
    [TestCase(5, 5, 5)] // 5 hits, buffer size 5 -> span length 5
    [TestCase(5, 8, 5)] // 5 hits, buffer size 8 -> span length 5
    public void QueryPointAsSpan_ShouldWriteCollidersIntoSpan(int collidersCount, int bufferSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var pointToQuery = Vector2.Zero;
        var colliders = new Collider2DComponent[bufferSize];

        // Act
        var view = physicsSystem.QueryPointAsSpan(pointToQuery, colliders);

        // Assert
        Assert.That(view.Length, Is.EqualTo(expectedWritten));

        foreach (var collider in view)
        {
            Assert.That(collider.ContainsPoint(pointToQuery), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty list -> empty span, capacity unchanged
    [TestCase(5, 0, 5)] // 5 hits, empty list -> span length 5, capacity grows
    [TestCase(5, 2, 5)] // 5 hits, list size 2 -> span length 5, capacity grows
    [TestCase(5, 5, 5)] // 5 hits, list size 5 -> span length 5, capacity unchanged
    [TestCase(5, 8, 5)] // 5 hits, list size 8 -> span length 5, capacity unchanged
    public void QueryPointAsSpan_ShouldWriteCollidersIntoList(int collidersCount, int initialListSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var pointToQuery = Vector2.Zero;

        var fillerCollider = CreateCircleStaticBody(100, 100, 10).GetComponent<CircleColliderComponent>();
        Assert.That(fillerCollider.ContainsPoint(pointToQuery), Is.False);

        var colliders = new List<Collider2DComponent>(initialListSize);
        for (var i = 0; i < initialListSize; i++)
        {
            colliders.Add(fillerCollider);
        }

        var initialCapacity = colliders.Capacity;

        // Act
        var view = physicsSystem.QueryPointAsSpan(pointToQuery, colliders);

        // Assert
        Assert.That(view.Length, Is.EqualTo(expectedWritten));
        Assert.That(colliders.Count, Is.EqualTo(view.Length));
        AssertListCapacityBehavior(initialCapacity, view.Length, colliders.Capacity);

        foreach (var collider in view)
        {
            Assert.That(collider.ContainsPoint(pointToQuery), Is.True);
        }
    }

    public sealed record QueryPointGeometryTestCase(
        string Name,
        Func<SceneQueryTests, Collider2DComponent> CreateCollider,
        PhysicsConfiguration PhysicsConfiguration,
        Vector2 PointToQuery,
        bool ExpectedHit,
        double VisualScale,
        double PointMarkerRadius
    );

    private static IEnumerable<QueryPointGeometryTestCase> QueryPointGeometryCases()
    {
        // Circle
        yield return new QueryPointGeometryTestCase("QueryPoint_01_Geometry_Circle_PointAtCenter",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(0, 0), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("QueryPoint_02_Geometry_Circle_PointOnEdge",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(10, 0), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("QueryPoint_03_Geometry_Circle_PointOutside",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(10.0001, 0), false, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("QueryPoint_04_Geometry_Circle_ShiftedPointOnEdge",
            t => t.CreateCircleStaticBody(5, -3, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(15, -3), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("QueryPoint_05_Geometry_Circle_ShiftedPointOutside",
            t => t.CreateCircleStaticBody(5, -3, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(15.0001, -3), false, 10d, 0.3d);
        // Rectangle
        yield return new QueryPointGeometryTestCase("QueryPoint_06_Geometry_Rectangle_PointAtCenter",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(0, 0), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("QueryPoint_07_Geometry_Rectangle_PointOnEdge",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(10, 0), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("QueryPoint_08_Geometry_Rectangle_PointOutside",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(10.0001, 0), false, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("QueryPoint_09_Geometry_Rectangle_Rotated90_PointOnEdge",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 2).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(0, 10), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("QueryPoint_10_Geometry_Rectangle_Rotated90_PointOutside",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 2).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(0, 10.0001), false, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("QueryPoint_11_Geometry_Rectangle_Rotated30_InsideAabbOutsideShape",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(11, 0), false, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("QueryPoint_12_Geometry_Rectangle_Rotated30_InsideShape",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(9, 0), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("QueryPoint_13_Geometry_Rectangle_ShiftedPointOnEdge",
            t => t.CreateRectangleStaticBody(5, -3, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(15, -3), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("QueryPoint_14_Geometry_Rectangle_ShiftedPointOutside",
            t => t.CreateRectangleStaticBody(5, -3, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(15.0001, -3), false, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("QueryPoint_15_Geometry_Rectangle_ShiftedRotated_InsideAabbOutsideShape",
            t => t.CreateRectangleStaticBody(5, -3, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(16, -3), false, 10d, 0.3d);
        // Tile
        yield return new QueryPointGeometryTestCase("QueryPoint_16_Geometry_Tile_PointAtCenter",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            new Vector2(0, 0), true, 40d, 0.05d);
        yield return new QueryPointGeometryTestCase("QueryPoint_17_Geometry_Tile_PointOnCorner",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            new Vector2(0.5, 0.5), true, 40d, 0.05d);
        yield return new QueryPointGeometryTestCase("QueryPoint_18_Geometry_Tile_PointOutside",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            new Vector2(0.5001, 0.5), false, 40d, 0.05d);
        yield return new QueryPointGeometryTestCase("QueryPoint_19_Geometry_Tile_CustomSize_PointInside",
            t => t.CreateTileStaticBody(4, 6).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(2, 3),
            new Vector2(5, 7), true, 40d, 0.05d);
        yield return new QueryPointGeometryTestCase("QueryPoint_20_Geometry_Tile_CustomSize_PointOutside",
            t => t.CreateTileStaticBody(4, 6).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(2, 3),
            new Vector2(5.0001, 7), false, 40d, 0.05d);
    }

    private static IEnumerable<TestCaseData> QueryPointGeometryTestCases() =>
        QueryPointGeometryCases().Select(testCase => new TestCaseData(testCase).SetName(testCase.Name));

    [TestCaseSource(nameof(QueryPointGeometryTestCases))]
    public void QueryPoint_ShouldReturnCollidersMatchingGeometry(QueryPointGeometryTestCase testCase)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(testCase.PhysicsConfiguration);
        var collider = testCase.CreateCollider(this);
        var queryShapeColor = testCase.ExpectedHit ? Color.Red : Color.Black;

        physicsSystem.SynchronizePhysicsState();
        var colliders = new Collider2DComponent[1];

        SaveVisualOutput(physicsSystem, scale: testCase.VisualScale,
            postDrawAction: renderer => renderer.DrawCircle(new Circle(testCase.PointToQuery, testCase.PointMarkerRadius), queryShapeColor));

        // Act
        var written = physicsSystem.QueryPoint(testCase.PointToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(testCase.ExpectedHit ? 1 : 0));

        if (testCase.ExpectedHit)
        {
            Assert.That(colliders[0], Is.EqualTo(collider));
        }
    }

    #endregion

    #region QueryBounds

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty buffer -> 0 written
    [TestCase(5, 0, 0)] // 5 hits, buffer size 0 -> 0 written
    [TestCase(5, 2, 2)] // 5 hits, buffer size 2 -> 2 written
    [TestCase(5, 5, 5)] // 5 hits, buffer size 5 -> 5 written
    [TestCase(5, 8, 5)] // 5 hits, buffer size 8 -> 5 written
    public void QueryBounds_ShouldWriteCollidersIntoSpan(int collidersCount, int bufferSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var aabbToQuery = AABB2D.FromCenterAndSize(0, 0, 1, 1);
        var colliders = new Collider2DComponent[bufferSize];

        // Act
        var written = physicsSystem.QueryBounds(aabbToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(expectedWritten));

        for (var i = 0; i < written; i++)
        {
            Assert.That(colliders[i].BoundingRectangle.Overlaps(aabbToQuery.ToAxisAlignedRectangle()), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty list -> 0 written, capacity unchanged
    [TestCase(5, 0, 5)] // 5 hits, empty list -> 5 written, capacity grows
    [TestCase(5, 2, 5)] // 5 hits, list size 2 -> 5 written, capacity grows
    [TestCase(5, 5, 5)] // 5 hits, list size 5 -> 5 written, capacity unchanged
    [TestCase(5, 8, 5)] // 5 hits, list size 8 -> 5 written, capacity unchanged
    public void QueryBounds_ShouldWriteCollidersIntoList(int collidersCount, int initialListSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var aabbToQuery = AABB2D.FromCenterAndSize(0, 0, 1, 1);

        var fillerCollider = CreateCircleStaticBody(100, 100, 10).GetComponent<CircleColliderComponent>();
        Assert.That(fillerCollider.BoundingRectangle.Overlaps(aabbToQuery.ToAxisAlignedRectangle()), Is.False);

        var colliders = new List<Collider2DComponent>(initialListSize);
        for (var i = 0; i < initialListSize; i++)
        {
            colliders.Add(fillerCollider);
        }

        var initialCapacity = colliders.Capacity;

        // Act
        var written = physicsSystem.QueryBounds(aabbToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(expectedWritten));
        Assert.That(colliders.Count, Is.EqualTo(written));
        AssertListCapacityBehavior(initialCapacity, written, colliders.Capacity);

        for (var i = 0; i < written; i++)
        {
            Assert.That(colliders[i].BoundingRectangle.Overlaps(aabbToQuery.ToAxisAlignedRectangle()), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty buffer -> empty span
    [TestCase(5, 0, 0)] // 5 hits, buffer size 0 -> empty span
    [TestCase(5, 2, 2)] // 5 hits, buffer size 2 -> span length 2
    [TestCase(5, 5, 5)] // 5 hits, buffer size 5 -> span length 5
    [TestCase(5, 8, 5)] // 5 hits, buffer size 8 -> span length 5
    public void QueryBoundsAsSpan_ShouldWriteCollidersIntoSpan(int collidersCount, int bufferSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var aabbToQuery = AABB2D.FromCenterAndSize(0, 0, 1, 1);
        var colliders = new Collider2DComponent[bufferSize];

        // Act
        var view = physicsSystem.QueryBoundsAsSpan(aabbToQuery, colliders);

        // Assert
        Assert.That(view.Length, Is.EqualTo(expectedWritten));

        foreach (var collider in view)
        {
            Assert.That(collider.BoundingRectangle.Overlaps(aabbToQuery.ToAxisAlignedRectangle()), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty list -> empty span, capacity unchanged
    [TestCase(5, 0, 5)] // 5 hits, empty list -> span length 5, capacity grows
    [TestCase(5, 2, 5)] // 5 hits, list size 2 -> span length 5, capacity grows
    [TestCase(5, 5, 5)] // 5 hits, list size 5 -> span length 5, capacity unchanged
    [TestCase(5, 8, 5)] // 5 hits, list size 8 -> span length 5, capacity unchanged
    public void QueryBoundsAsSpan_ShouldWriteCollidersIntoList(int collidersCount, int initialListSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var aabbToQuery = AABB2D.FromCenterAndSize(0, 0, 1, 1);

        var fillerCollider = CreateCircleStaticBody(100, 100, 10).GetComponent<CircleColliderComponent>();
        Assert.That(fillerCollider.BoundingRectangle.Overlaps(aabbToQuery.ToAxisAlignedRectangle()), Is.False);

        var colliders = new List<Collider2DComponent>(initialListSize);
        for (var i = 0; i < initialListSize; i++)
        {
            colliders.Add(fillerCollider);
        }

        var initialCapacity = colliders.Capacity;

        // Act
        var view = physicsSystem.QueryBoundsAsSpan(aabbToQuery, colliders);

        // Assert
        Assert.That(view.Length, Is.EqualTo(expectedWritten));
        Assert.That(colliders.Count, Is.EqualTo(view.Length));
        AssertListCapacityBehavior(initialCapacity, view.Length, colliders.Capacity);

        foreach (var collider in view)
        {
            Assert.That(collider.BoundingRectangle.Overlaps(aabbToQuery.ToAxisAlignedRectangle()), Is.True);
        }
    }

    public sealed record QueryBoundsGeometryTestCase(
        string Name,
        Func<SceneQueryTests, Collider2DComponent> CreateCollider,
        PhysicsConfiguration PhysicsConfiguration,
        AABB2D AabbToQuery,
        bool ExpectedHit,
        double VisualScale
    );

    private static IEnumerable<QueryBoundsGeometryTestCase> QueryBoundsGeometryCases()
    {
        // Circle
        yield return new QueryBoundsGeometryTestCase("QueryBounds_01_Geometry_Circle_AabbFullyInside",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            AABB2D.FromCenterAndSize(0, 0, 2, 2), true, 10d);
        yield return new QueryBoundsGeometryTestCase("QueryBounds_02_Geometry_Circle_AabbTouchingEdge",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            AABB2D.FromCenterAndSize(11, 0, 2, 2), true, 10d);
        yield return new QueryBoundsGeometryTestCase("QueryBounds_03_Geometry_Circle_AabbOutside",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            AABB2D.FromCenterAndSize(11.0001, 0, 2, 2), false, 10d);
        yield return new QueryBoundsGeometryTestCase("QueryBounds_04_Geometry_Circle_AabbOverlapOutsideShape",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            AABB2D.FromCenterAndSize(9, 9, 1, 1), true, 10d);

        // Rectangle
        yield return new QueryBoundsGeometryTestCase("QueryBounds_05_Geometry_Rectangle_AabbFullyInside",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            AABB2D.FromCenterAndSize(0, 0, 2, 2), true, 10d);
        yield return new QueryBoundsGeometryTestCase("QueryBounds_06_Geometry_Rectangle_AabbOutside",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            AABB2D.FromCenterAndSize(11.0001, 0, 2, 2), false, 10d);
        yield return new QueryBoundsGeometryTestCase("QueryBounds_07_Geometry_Rectangle_RotatedAabbOverlapOutsideShape",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            AABB2D.FromCenterAndSize(11, -2, 1, 1), true, 10d);

        // Tile
        yield return new QueryBoundsGeometryTestCase("QueryBounds_08_Geometry_Tile_AabbInside",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            AABB2D.FromCenterAndSize(0, 0, 0.2, 0.2), true, 40d);
        yield return new QueryBoundsGeometryTestCase("QueryBounds_09_Geometry_Tile_AabbOutside",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            AABB2D.FromCenterAndSize(1.0001, 0, 1, 1), false, 40d);
    }

    private static IEnumerable<TestCaseData> QueryBoundsGeometryTestCases() =>
        QueryBoundsGeometryCases().Select(testCase => new TestCaseData(testCase).SetName(testCase.Name));

    [TestCaseSource(nameof(QueryBoundsGeometryTestCases))]
    public void QueryBounds_ShouldReturnCollidersMatchingBoundingRectangles(QueryBoundsGeometryTestCase testCase)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(testCase.PhysicsConfiguration);
        var collider = testCase.CreateCollider(this);
        var queryShapeColor = testCase.ExpectedHit ? Color.Red : Color.Black;

        physicsSystem.SynchronizePhysicsState();
        var colliders = new Collider2DComponent[1];

        SaveVisualOutput(physicsSystem, scale: testCase.VisualScale,
            postDrawAction: renderer =>
            {
                renderer.DrawRectangle(testCase.AabbToQuery.ToAxisAlignedRectangle(), queryShapeColor, Matrix3x3.Identity);
                renderer.DrawRectangle(collider.BoundingRectangle, Color.Gray, Matrix3x3.Identity);
            });

        // Act
        var written = physicsSystem.QueryBounds(testCase.AabbToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(testCase.ExpectedHit ? 1 : 0));

        if (testCase.ExpectedHit)
        {
            Assert.That(colliders[0], Is.EqualTo(collider));
        }
    }

    #endregion

    #region QueryOverlap AxisAlignedRectangle

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty buffer -> 0 written
    [TestCase(5, 0, 0)] // 5 hits, buffer size 0 -> 0 written
    [TestCase(5, 2, 2)] // 5 hits, buffer size 2 -> 2 written
    [TestCase(5, 5, 5)] // 5 hits, buffer size 5 -> 5 written
    [TestCase(5, 8, 5)] // 5 hits, buffer size 8 -> 5 written
    public void QueryOverlap_AxisAlignedRectangle_ShouldWriteCollidersIntoSpan(int collidersCount, int bufferSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var rectangleToQuery = new AxisAlignedRectangle(0, 0, 1, 1);
        var colliders = new Collider2DComponent[bufferSize];

        // Act
        var written = physicsSystem.QueryOverlap(rectangleToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(expectedWritten));

        for (var i = 0; i < written; i++)
        {
            Assert.That(colliders[i].Overlaps(rectangleToQuery), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty list -> 0 written, capacity unchanged
    [TestCase(5, 0, 5)] // 5 hits, empty list -> 5 written, capacity grows
    [TestCase(5, 2, 5)] // 5 hits, list size 2 -> 5 written, capacity grows
    [TestCase(5, 5, 5)] // 5 hits, list size 5 -> 5 written, capacity unchanged
    [TestCase(5, 8, 5)] // 5 hits, list size 8 -> 5 written, capacity unchanged
    public void QueryOverlap_AxisAlignedRectangle_ShouldWriteCollidersIntoList(int collidersCount, int initialListSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var rectangleToQuery = new AxisAlignedRectangle(0, 0, 1, 1);

        var fillerCollider = CreateCircleStaticBody(100, 100, 10).GetComponent<CircleColliderComponent>();
        Assert.That(fillerCollider.Overlaps(rectangleToQuery), Is.False);

        var colliders = new List<Collider2DComponent>(initialListSize);
        for (var i = 0; i < initialListSize; i++)
        {
            colliders.Add(fillerCollider);
        }

        var initialCapacity = colliders.Capacity;

        // Act
        var written = physicsSystem.QueryOverlap(rectangleToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(expectedWritten));
        Assert.That(colliders.Count, Is.EqualTo(written));
        AssertListCapacityBehavior(initialCapacity, written, colliders.Capacity);

        for (var i = 0; i < written; i++)
        {
            Assert.That(colliders[i].Overlaps(rectangleToQuery), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty buffer -> empty span
    [TestCase(5, 0, 0)] // 5 hits, buffer size 0 -> empty span
    [TestCase(5, 2, 2)] // 5 hits, buffer size 2 -> span length 2
    [TestCase(5, 5, 5)] // 5 hits, buffer size 5 -> span length 5
    [TestCase(5, 8, 5)] // 5 hits, buffer size 8 -> span length 5
    public void QueryOverlapAsSpan_AxisAlignedRectangle_ShouldWriteCollidersIntoSpan(int collidersCount, int bufferSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var rectangleToQuery = new AxisAlignedRectangle(0, 0, 1, 1);
        var colliders = new Collider2DComponent[bufferSize];

        // Act
        var view = physicsSystem.QueryOverlapAsSpan(rectangleToQuery, colliders);

        // Assert
        Assert.That(view.Length, Is.EqualTo(expectedWritten));

        foreach (var collider in view)
        {
            Assert.That(collider.Overlaps(rectangleToQuery), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty list -> empty span, capacity unchanged
    [TestCase(5, 0, 5)] // 5 hits, empty list -> span length 5, capacity grows
    [TestCase(5, 2, 5)] // 5 hits, list size 2 -> span length 5, capacity grows
    [TestCase(5, 5, 5)] // 5 hits, list size 5 -> span length 5, capacity unchanged
    [TestCase(5, 8, 5)] // 5 hits, list size 8 -> span length 5, capacity unchanged
    public void QueryOverlapAsSpan_AxisAlignedRectangle_ShouldWriteCollidersIntoList(int collidersCount, int initialListSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var rectangleToQuery = new AxisAlignedRectangle(0, 0, 1, 1);

        var fillerCollider = CreateCircleStaticBody(100, 100, 10).GetComponent<CircleColliderComponent>();
        Assert.That(fillerCollider.Overlaps(rectangleToQuery), Is.False);

        var colliders = new List<Collider2DComponent>(initialListSize);
        for (var i = 0; i < initialListSize; i++)
        {
            colliders.Add(fillerCollider);
        }

        var initialCapacity = colliders.Capacity;

        // Act
        var view = physicsSystem.QueryOverlapAsSpan(rectangleToQuery, colliders);

        // Assert
        Assert.That(view.Length, Is.EqualTo(expectedWritten));
        Assert.That(colliders.Count, Is.EqualTo(view.Length));
        AssertListCapacityBehavior(initialCapacity, view.Length, colliders.Capacity);

        foreach (var collider in view)
        {
            Assert.That(collider.Overlaps(rectangleToQuery), Is.True);
        }
    }

    public sealed record QueryOverlapAxisAlignedRectangleGeometryTestCase(
        string Name,
        Func<SceneQueryTests, Collider2DComponent> CreateCollider,
        PhysicsConfiguration PhysicsConfiguration,
        AxisAlignedRectangle RectangleToQuery,
        bool ExpectedHit,
        double VisualScale
    );

    private static IEnumerable<QueryOverlapAxisAlignedRectangleGeometryTestCase> QueryOverlapAxisAlignedRectangleGeometryCases()
    {
        // Circle
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_01_Geometry_Circle_AabbFullyInside",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new AxisAlignedRectangle(0, 0, 2, 2), true, 10d);
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_02_Geometry_Circle_AabbTouchingEdge",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new AxisAlignedRectangle(11, 0, 2, 2), true, 10d);
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_03_Geometry_Circle_AabbOutside",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new AxisAlignedRectangle(11.0001, 0, 2, 2), false, 10d);
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_04_Geometry_Circle_InsideBoundingBoxOutsideShape",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new AxisAlignedRectangle(9, 9, 1, 1), false, 10d);
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_05_Geometry_Circle_ShiftedOverlap",
            t => t.CreateCircleStaticBody(5, -3, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new AxisAlignedRectangle(15, -3, 2, 2), true, 10d);
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_06_Geometry_Circle_ShiftedNoOverlap",
            t => t.CreateCircleStaticBody(5, -3, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new AxisAlignedRectangle(16.0001, -3, 2, 2), false, 10d);

        // Rectangle
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_07_Geometry_Rectangle_AabbFullyInside",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new AxisAlignedRectangle(0, 0, 2, 2), true, 10d);
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_08_Geometry_Rectangle_AabbTouchingEdge",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new AxisAlignedRectangle(11, 0, 2, 2), true, 10d);
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_09_Geometry_Rectangle_AabbOutside",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new AxisAlignedRectangle(11.0001, 0, 2, 2), false, 10d);
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_10_Geometry_Rectangle_RotatedTouchingShape",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new AxisAlignedRectangle(11, 0, 1, 1), true, 10d);
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase(
            "QueryOverlap_AxisAlignedRectangle_11_Geometry_Rectangle_RotatedInsideAabbOutsideShape",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new AxisAlignedRectangle(11, -2, 1, 1), false, 10d);
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_12_Geometry_Rectangle_ShiftedOverlap",
            t => t.CreateRectangleStaticBody(5, -3, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new AxisAlignedRectangle(15, -3, 2, 2), true, 10d);

        // Tile
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_13_Geometry_Tile_AabbFullyInside",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            new AxisAlignedRectangle(0, 0, 0.2, 0.2), true, 40d);
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_14_Geometry_Tile_AabbTouchingEdge",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            new AxisAlignedRectangle(1, 0, 1, 1), true, 40d);
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_15_Geometry_Tile_AabbOutside",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            new AxisAlignedRectangle(1.0001, 0, 1, 1), false, 40d);
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_16_Geometry_Tile_CustomSizeInside",
            t => t.CreateTileStaticBody(4, 6).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(2, 3),
            new AxisAlignedRectangle(5, 7, 0.2, 0.2), true, 40d);
        yield return new QueryOverlapAxisAlignedRectangleGeometryTestCase("QueryOverlap_AxisAlignedRectangle_17_Geometry_Tile_CustomSizeOutside",
            t => t.CreateTileStaticBody(4, 6).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(2, 3),
            new AxisAlignedRectangle(5.1001, 7, 0.2, 0.2), false, 40d);
    }

    private static IEnumerable<TestCaseData> QueryOverlapAxisAlignedRectangleGeometryTestCases() =>
        QueryOverlapAxisAlignedRectangleGeometryCases().Select(testCase => new TestCaseData(testCase).SetName(testCase.Name));

    [TestCaseSource(nameof(QueryOverlapAxisAlignedRectangleGeometryTestCases))]
    public void QueryOverlap_AxisAlignedRectangle_ShouldReturnCollidersMatchingGeometry(QueryOverlapAxisAlignedRectangleGeometryTestCase testCase)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(testCase.PhysicsConfiguration);
        var collider = testCase.CreateCollider(this);
        var queryShapeColor = testCase.ExpectedHit ? Color.Red : Color.Black;

        physicsSystem.SynchronizePhysicsState();
        var colliders = new Collider2DComponent[1];

        SaveVisualOutput(physicsSystem, scale: testCase.VisualScale,
            postDrawAction: renderer => renderer.DrawRectangle(testCase.RectangleToQuery, queryShapeColor, Matrix3x3.Identity));

        // Act
        var written = physicsSystem.QueryOverlap(testCase.RectangleToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(testCase.ExpectedHit ? 1 : 0));

        if (testCase.ExpectedHit)
        {
            Assert.That(colliders[0], Is.EqualTo(collider));
        }
    }

    #endregion

    #region QueryOverlap Circle

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty buffer -> 0 written
    [TestCase(5, 0, 0)] // 5 hits, buffer size 0 -> 0 written
    [TestCase(5, 2, 2)] // 5 hits, buffer size 2 -> 2 written
    [TestCase(5, 5, 5)] // 5 hits, buffer size 5 -> 5 written
    [TestCase(5, 8, 5)] // 5 hits, buffer size 8 -> 5 written
    public void QueryOverlap_Circle_ShouldWriteCollidersIntoSpan(int collidersCount, int bufferSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var circleToQuery = new Circle(Vector2.Zero, 1);
        var colliders = new Collider2DComponent[bufferSize];

        // Act
        var written = physicsSystem.QueryOverlap(circleToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(expectedWritten));

        for (var i = 0; i < written; i++)
        {
            Assert.That(colliders[i].Overlaps(circleToQuery), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty list -> 0 written, capacity unchanged
    [TestCase(5, 0, 5)] // 5 hits, empty list -> 5 written, capacity grows
    [TestCase(5, 2, 5)] // 5 hits, list size 2 -> 5 written, capacity grows
    [TestCase(5, 5, 5)] // 5 hits, list size 5 -> 5 written, capacity unchanged
    [TestCase(5, 8, 5)] // 5 hits, list size 8 -> 5 written, capacity unchanged
    public void QueryOverlap_Circle_ShouldWriteCollidersIntoList(int collidersCount, int initialListSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var circleToQuery = new Circle(Vector2.Zero, 1);

        var fillerCollider = CreateCircleStaticBody(100, 100, 10).GetComponent<CircleColliderComponent>();
        Assert.That(fillerCollider.Overlaps(circleToQuery), Is.False);

        var colliders = new List<Collider2DComponent>(initialListSize);
        for (var i = 0; i < initialListSize; i++)
        {
            colliders.Add(fillerCollider);
        }

        var initialCapacity = colliders.Capacity;

        // Act
        var written = physicsSystem.QueryOverlap(circleToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(expectedWritten));
        Assert.That(colliders.Count, Is.EqualTo(written));
        AssertListCapacityBehavior(initialCapacity, written, colliders.Capacity);

        for (var i = 0; i < written; i++)
        {
            Assert.That(colliders[i].Overlaps(circleToQuery), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty buffer -> empty span
    [TestCase(5, 0, 0)] // 5 hits, buffer size 0 -> empty span
    [TestCase(5, 2, 2)] // 5 hits, buffer size 2 -> span length 2
    [TestCase(5, 5, 5)] // 5 hits, buffer size 5 -> span length 5
    [TestCase(5, 8, 5)] // 5 hits, buffer size 8 -> span length 5
    public void QueryOverlapAsSpan_Circle_ShouldWriteCollidersIntoSpan(int collidersCount, int bufferSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var circleToQuery = new Circle(Vector2.Zero, 1);
        var colliders = new Collider2DComponent[bufferSize];

        // Act
        var view = physicsSystem.QueryOverlapAsSpan(circleToQuery, colliders);

        // Assert
        Assert.That(view.Length, Is.EqualTo(expectedWritten));

        foreach (var collider in view)
        {
            Assert.That(collider.Overlaps(circleToQuery), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty list -> empty span, capacity unchanged
    [TestCase(5, 0, 5)] // 5 hits, empty list -> span length 5, capacity grows
    [TestCase(5, 2, 5)] // 5 hits, list size 2 -> span length 5, capacity grows
    [TestCase(5, 5, 5)] // 5 hits, list size 5 -> span length 5, capacity unchanged
    [TestCase(5, 8, 5)] // 5 hits, list size 8 -> span length 5, capacity unchanged
    public void QueryOverlapAsSpan_Circle_ShouldWriteCollidersIntoList(int collidersCount, int initialListSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var circleToQuery = new Circle(Vector2.Zero, 1);

        var fillerCollider = CreateCircleStaticBody(100, 100, 10).GetComponent<CircleColliderComponent>();
        Assert.That(fillerCollider.Overlaps(circleToQuery), Is.False);

        var colliders = new List<Collider2DComponent>(initialListSize);
        for (var i = 0; i < initialListSize; i++)
        {
            colliders.Add(fillerCollider);
        }

        var initialCapacity = colliders.Capacity;

        // Act
        var view = physicsSystem.QueryOverlapAsSpan(circleToQuery, colliders);

        // Assert
        Assert.That(view.Length, Is.EqualTo(expectedWritten));
        Assert.That(colliders.Count, Is.EqualTo(view.Length));
        AssertListCapacityBehavior(initialCapacity, view.Length, colliders.Capacity);

        foreach (var collider in view)
        {
            Assert.That(collider.Overlaps(circleToQuery), Is.True);
        }
    }

    public sealed record QueryOverlapCircleGeometryTestCase(
        string Name,
        Func<SceneQueryTests, Collider2DComponent> CreateCollider,
        PhysicsConfiguration PhysicsConfiguration,
        Circle CircleToQuery,
        bool ExpectedHit,
        double VisualScale
    );

    private static IEnumerable<QueryOverlapCircleGeometryTestCase> QueryOverlapCircleGeometryCases()
    {
        // Circle
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_01_Geometry_Circle_FullyInside",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Circle(new Vector2(0, 0), 2), true, 10d);
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_02_Geometry_Circle_TouchingEdge",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Circle(new Vector2(12, 0), 2), true, 10d);
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_03_Geometry_Circle_Outside",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Circle(new Vector2(12.0001, 0), 2), false, 10d);
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_04_Geometry_Circle_ShiftedOverlap",
            t => t.CreateCircleStaticBody(5, -3, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Circle(new Vector2(15, -3), 2), true, 10d);
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_05_Geometry_Circle_ShiftedNoOverlap",
            t => t.CreateCircleStaticBody(5, -3, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Circle(new Vector2(17.0001, -3), 2), false, 10d);

        // Rectangle
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_06_Geometry_Rectangle_CenterInside",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Circle(new Vector2(0, 0), 2), true, 10d);
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_07_Geometry_Rectangle_TouchingEdge",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Circle(new Vector2(11, 0), 1), true, 10d);
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_08_Geometry_Rectangle_Outside",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Circle(new Vector2(11.0001, 0), 1), false, 10d);
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_09_Geometry_Rectangle_RotatedTouchingShape",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Circle(new Vector2(11, 0), 0.5), true, 10d);
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_10_Geometry_Rectangle_RotatedInsideAabbOutsideShape",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Circle(new Vector2(11, -2), 0.5), false, 10d);
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_11_Geometry_Rectangle_ShiftedOverlap",
            t => t.CreateRectangleStaticBody(5, -3, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Circle(new Vector2(15, -3), 1), true, 10d);

        // Tile
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_12_Geometry_Tile_Inside",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            new Circle(new Vector2(0, 0), 0.1), true, 40d);
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_13_Geometry_Tile_TouchingEdge",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            new Circle(new Vector2(1, 0), 0.5), true, 40d);
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_14_Geometry_Tile_Outside",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            new Circle(new Vector2(1.0001, 0), 0.5), false, 40d);
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_15_Geometry_Tile_AabbTouchOnlyOutsideShape",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            new Circle(new Vector2(0.83, 0.83), 0.4), false, 40d);
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_16_Geometry_Tile_CustomSizeInside",
            t => t.CreateTileStaticBody(4, 6).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(2, 3),
            new Circle(new Vector2(5, 7), 0.1), true, 40d);
        yield return new QueryOverlapCircleGeometryTestCase("QueryOverlap_Circle_17_Geometry_Tile_CustomSizeOutside",
            t => t.CreateTileStaticBody(4, 6).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(2, 3),
            new Circle(new Vector2(5.1001, 7), 0.1), false, 40d);
    }

    private static IEnumerable<TestCaseData> QueryOverlapCircleGeometryTestCases() =>
        QueryOverlapCircleGeometryCases().Select(testCase => new TestCaseData(testCase).SetName(testCase.Name));

    [TestCaseSource(nameof(QueryOverlapCircleGeometryTestCases))]
    public void QueryOverlap_Circle_ShouldReturnCollidersMatchingGeometry(QueryOverlapCircleGeometryTestCase testCase)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(testCase.PhysicsConfiguration);
        var collider = testCase.CreateCollider(this);
        var queryShapeColor = testCase.ExpectedHit ? Color.Red : Color.Black;

        physicsSystem.SynchronizePhysicsState();
        var colliders = new Collider2DComponent[1];

        SaveVisualOutput(physicsSystem, scale: testCase.VisualScale,
            postDrawAction: renderer => renderer.DrawCircle(testCase.CircleToQuery, queryShapeColor));

        // Act
        var written = physicsSystem.QueryOverlap(testCase.CircleToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(testCase.ExpectedHit ? 1 : 0));

        if (testCase.ExpectedHit)
        {
            Assert.That(colliders[0], Is.EqualTo(collider));
        }
    }

    #endregion

    #region QueryOverlap Rectangle

    private sealed record QueryRectangleData(Rectangle QueryRectangle, AxisAlignedRectangle RectangleToDraw, Matrix3x3 RectangleDrawTransform);

    private static QueryRectangleData CreateQueryRectangleData(double x, double y, double width, double height, double rotation)
    {
        var transform = new Transform2D(new Vector2(x, y), rotation, Vector2.One).ToMatrix();
        var rectangleToQuery = new Rectangle(new Vector2(width, height)).Transform(transform);
        var rectangleToDraw = new AxisAlignedRectangle(width, height);

        return new QueryRectangleData(rectangleToQuery, rectangleToDraw, transform);
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty buffer -> 0 written
    [TestCase(5, 0, 0)] // 5 hits, buffer size 0 -> 0 written
    [TestCase(5, 2, 2)] // 5 hits, buffer size 2 -> 2 written
    [TestCase(5, 5, 5)] // 5 hits, buffer size 5 -> 5 written
    [TestCase(5, 8, 5)] // 5 hits, buffer size 8 -> 5 written
    public void QueryOverlap_Rectangle_ShouldWriteCollidersIntoSpan(int collidersCount, int bufferSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var rectangleToQuery = CreateQueryRectangleData(0, 0, 1, 1, 0).QueryRectangle;
        var colliders = new Collider2DComponent[bufferSize];

        // Act
        var written = physicsSystem.QueryOverlap(rectangleToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(expectedWritten));

        for (var i = 0; i < written; i++)
        {
            Assert.That(colliders[i].Overlaps(rectangleToQuery), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty list -> 0 written, capacity unchanged
    [TestCase(5, 0, 5)] // 5 hits, empty list -> 5 written, capacity grows
    [TestCase(5, 2, 5)] // 5 hits, list size 2 -> 5 written, capacity grows
    [TestCase(5, 5, 5)] // 5 hits, list size 5 -> 5 written, capacity unchanged
    [TestCase(5, 8, 5)] // 5 hits, list size 8 -> 5 written, capacity unchanged
    public void QueryOverlap_Rectangle_ShouldWriteCollidersIntoList(int collidersCount, int initialListSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var rectangleToQuery = CreateQueryRectangleData(0, 0, 1, 1, 0).QueryRectangle;

        var fillerCollider = CreateCircleStaticBody(100, 100, 10).GetComponent<CircleColliderComponent>();
        Assert.That(fillerCollider.Overlaps(rectangleToQuery), Is.False);

        var colliders = new List<Collider2DComponent>(initialListSize);
        for (var i = 0; i < initialListSize; i++)
        {
            colliders.Add(fillerCollider);
        }

        var initialCapacity = colliders.Capacity;

        // Act
        var written = physicsSystem.QueryOverlap(rectangleToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(expectedWritten));
        Assert.That(colliders.Count, Is.EqualTo(written));
        AssertListCapacityBehavior(initialCapacity, written, colliders.Capacity);

        for (var i = 0; i < written; i++)
        {
            Assert.That(colliders[i].Overlaps(rectangleToQuery), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty buffer -> empty span
    [TestCase(5, 0, 0)] // 5 hits, buffer size 0 -> empty span
    [TestCase(5, 2, 2)] // 5 hits, buffer size 2 -> span length 2
    [TestCase(5, 5, 5)] // 5 hits, buffer size 5 -> span length 5
    [TestCase(5, 8, 5)] // 5 hits, buffer size 8 -> span length 5
    public void QueryOverlapAsSpan_Rectangle_ShouldWriteCollidersIntoSpan(int collidersCount, int bufferSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var rectangleToQuery = CreateQueryRectangleData(0, 0, 1, 1, 0).QueryRectangle;
        var colliders = new Collider2DComponent[bufferSize];

        // Act
        var view = physicsSystem.QueryOverlapAsSpan(rectangleToQuery, colliders);

        // Assert
        Assert.That(view.Length, Is.EqualTo(expectedWritten));

        foreach (var collider in view)
        {
            Assert.That(collider.Overlaps(rectangleToQuery), Is.True);
        }
    }

    [TestCase(0, 0, 0)] // No colliders: 0 hits, empty list -> empty span, capacity unchanged
    [TestCase(5, 0, 5)] // 5 hits, empty list -> span length 5, capacity grows
    [TestCase(5, 2, 5)] // 5 hits, list size 2 -> span length 5, capacity grows
    [TestCase(5, 5, 5)] // 5 hits, list size 5 -> span length 5, capacity unchanged
    [TestCase(5, 8, 5)] // 5 hits, list size 8 -> span length 5, capacity unchanged
    public void QueryOverlapAsSpan_Rectangle_ShouldWriteCollidersIntoList(int collidersCount, int initialListSize, int expectedWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(CreatePhysicsConfiguration(2, 2));
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var rectangleToQuery = CreateQueryRectangleData(0, 0, 1, 1, 0).QueryRectangle;

        var fillerCollider = CreateCircleStaticBody(100, 100, 10).GetComponent<CircleColliderComponent>();
        Assert.That(fillerCollider.Overlaps(rectangleToQuery), Is.False);

        var colliders = new List<Collider2DComponent>(initialListSize);
        for (var i = 0; i < initialListSize; i++)
        {
            colliders.Add(fillerCollider);
        }

        var initialCapacity = colliders.Capacity;

        // Act
        var view = physicsSystem.QueryOverlapAsSpan(rectangleToQuery, colliders);

        // Assert
        Assert.That(view.Length, Is.EqualTo(expectedWritten));
        Assert.That(colliders.Count, Is.EqualTo(view.Length));
        AssertListCapacityBehavior(initialCapacity, view.Length, colliders.Capacity);

        foreach (var collider in view)
        {
            Assert.That(collider.Overlaps(rectangleToQuery), Is.True);
        }
    }

    public sealed record QueryOverlapRectangleGeometryTestCase(
        string Name,
        Func<SceneQueryTests, Collider2DComponent> CreateCollider,
        PhysicsConfiguration PhysicsConfiguration,
        Rectangle RectangleToQuery,
        AxisAlignedRectangle RectangleToDraw,
        Matrix3x3 RectangleDrawTransform,
        bool ExpectedHit,
        double VisualScale
    );

    private static QueryOverlapRectangleGeometryTestCase CreateQueryOverlapRectangleGeometryTestCase(string name,
        Func<SceneQueryTests, Collider2DComponent> createCollider, PhysicsConfiguration physicsConfiguration, double queryX, double queryY, double queryWidth,
        double queryHeight, double queryRotation, bool expectedHit, double visualScale)
    {
        var queryRectangleData = CreateQueryRectangleData(queryX, queryY, queryWidth, queryHeight, queryRotation);

        return new QueryOverlapRectangleGeometryTestCase(name, createCollider, physicsConfiguration, queryRectangleData.QueryRectangle,
            queryRectangleData.RectangleToDraw, queryRectangleData.RectangleDrawTransform, expectedHit, visualScale);
    }

    private static IEnumerable<QueryOverlapRectangleGeometryTestCase> QueryOverlapRectangleGeometryCases()
    {
        // Circle
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_01_Geometry_Circle_FullyInside",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            0, 0, 2, 2, 0, true, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_02_Geometry_Circle_TouchingEdge",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            11, 0, 2, 2, 0, true, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_03_Geometry_Circle_Outside",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            11.0001, 0, 2, 2, 0, false, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_04_Geometry_Circle_InsideAabbOutsideShape",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            8, 8, 0.5, 0.5, 0, false, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_05_Geometry_Circle_ShiftedOverlap",
            t => t.CreateCircleStaticBody(5, -3, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            15, -3, 2, 2, 0, true, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_06_Geometry_Circle_ShiftedNoOverlap",
            t => t.CreateCircleStaticBody(5, -3, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            16.0001, -3, 2, 2, 0, false, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_07_Geometry_Circle_RotatedSlightOverlap",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            10.8, 0, 2, 1, Math.PI / 4, true, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_08_Geometry_Circle_RotatedOutside",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            11.8, 0, 2, 1, Math.PI / 4, false, 10d);

        // Rectangle
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_09_Geometry_Rectangle_FullyInside",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            0, 0, 2, 2, 0, true, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_10_Geometry_Rectangle_TouchingEdge",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            11, 0, 2, 2, 0, true, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_11_Geometry_Rectangle_Outside",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            11.0001, 0, 2, 2, 0, false, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_12_Geometry_Rectangle_QueryRotatedTouchingShape",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            10.6, 0, 2, 1, Math.PI / 4, true, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_13_Geometry_Rectangle_QueryRotatedOutside",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            11.6, 0, 2, 1, Math.PI / 4, false, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_14_Geometry_Rectangle_SceneRotatedTouchingShape",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            11, 0, 1, 1, 0, true, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_15_Geometry_Rectangle_SceneRotatedInsideAabbOutsideShape",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            11, -2, 1, 1, 0, false, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_16_Geometry_Rectangle_BothRotatedTouchingShape",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            11, 0, 1, 1, Math.PI / 4, true, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_17_Geometry_Rectangle_BothRotatedInsideAabbOutsideShape",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            11, -2, 1, 1, Math.PI / 4, false, 10d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_18_Geometry_Rectangle_ShiftedOverlap",
            t => t.CreateRectangleStaticBody(5, -3, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            15, -3, 2, 2, 0, true, 10d);

        // Tile
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_19_Geometry_Tile_Inside",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            0, 0, 0.2, 0.2, 0, true, 40d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_20_Geometry_Tile_TouchingEdge",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            1, 0, 1, 1, 0, true, 40d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_21_Geometry_Tile_Outside",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            1.0001, 0, 1, 1, 0, false, 40d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_22_Geometry_Tile_RotatedHit",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            0.6, 0.6, 0.8, 0.4, Math.PI / 4, true, 40d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_23_Geometry_Tile_RotatedInsideAabbOutsideShape",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            0.8, 0.8, 0.5, 0.5, Math.PI / 4, false, 40d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_24_Geometry_Tile_CustomSizeInside",
            t => t.CreateTileStaticBody(4, 6).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(2, 3),
            5, 7, 0.2, 0.2, 0, true, 40d);
        yield return CreateQueryOverlapRectangleGeometryTestCase("QueryOverlap_Rectangle_25_Geometry_Tile_CustomSizeOutside",
            t => t.CreateTileStaticBody(4, 6).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(2, 3),
            5.1001, 7, 0.2, 0.2, 0, false, 40d);
    }

    private static IEnumerable<TestCaseData> QueryOverlapRectangleGeometryTestCases() =>
        QueryOverlapRectangleGeometryCases().Select(testCase => new TestCaseData(testCase).SetName(testCase.Name));

    [TestCaseSource(nameof(QueryOverlapRectangleGeometryTestCases))]
    public void QueryOverlap_Rectangle_ShouldReturnCollidersMatchingGeometry(QueryOverlapRectangleGeometryTestCase testCase)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(testCase.PhysicsConfiguration);
        var collider = testCase.CreateCollider(this);
        var queryShapeColor = testCase.ExpectedHit ? Color.Red : Color.Black;

        physicsSystem.SynchronizePhysicsState();
        var colliders = new Collider2DComponent[1];

        SaveVisualOutput(physicsSystem, scale: testCase.VisualScale,
            postDrawAction: renderer => renderer.DrawRectangle(testCase.RectangleToDraw, queryShapeColor, testCase.RectangleDrawTransform));

        // Act
        var written = physicsSystem.QueryOverlap(testCase.RectangleToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(testCase.ExpectedHit ? 1 : 0));

        if (testCase.ExpectedHit)
        {
            Assert.That(colliders[0], Is.EqualTo(collider));
        }
    }

    #endregion

    private static PhysicsConfiguration CreatePhysicsConfiguration(double tileWidth = 1, double tileHeight = 1)
    {
        return new PhysicsConfiguration
        {
            EnableDebugRendering = true,
            TileSize = new SizeD(tileWidth, tileHeight)
        };
    }

    private void CreateCollidersAtOrigin(int collidersCount)
    {
        for (var i = 0; i < collidersCount; i++)
        {
            switch (i % 3)
            {
                case 0:
                    CreateCircleStaticBody(0, 0, 10 + i);
                    break;
                case 1:
                    CreateRectangleStaticBody(0, 0, 20 + i, 10 + i, i % 2 == 0 ? 0 : Math.PI / 6);
                    break;
                default:
                    CreateTileStaticBody(0, 0);
                    break;
            }
        }
    }

    private static void AssertListCapacityBehavior(int initialCapacity, int requiredCount, int finalCapacity)
    {
        if (initialCapacity < requiredCount)
        {
            Assert.That(finalCapacity, Is.GreaterThanOrEqualTo(requiredCount));
            Assert.That(finalCapacity, Is.GreaterThan(initialCapacity));
        }
        else
        {
            Assert.That(finalCapacity, Is.EqualTo(initialCapacity));
        }
    }
}