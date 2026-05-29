using System;
using System.Collections.Generic;
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
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(2, 2)
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
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
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(2, 2)
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
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
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(2, 2)
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
        CreateCollidersAtOrigin(collidersCount);
        physicsSystem.SynchronizePhysicsState();

        var pointToQuery = Vector2.Zero;
        var colliders = new Collider2DComponent[bufferSize];

        // Act
        var view = physicsSystem.QueryPointAsSpan(pointToQuery, colliders);

        // Assert
        Assert.That(view.Length, Is.EqualTo(expectedWritten));

        for (var i = 0; i < view.Length; i++)
        {
            Assert.That(view[i].ContainsPoint(pointToQuery), Is.True);
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
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(2, 2)
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
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

        for (var i = 0; i < view.Length; i++)
        {
            Assert.That(view[i].ContainsPoint(pointToQuery), Is.True);
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
        yield return new QueryPointGeometryTestCase("01_QueryPoint_Geometry_Circle_PointAtCenter",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(0, 0), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("02_QueryPoint_Geometry_Circle_PointOnEdge",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(10, 0), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("03_QueryPoint_Geometry_Circle_PointOutside",
            t => t.CreateCircleStaticBody(0, 0, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(10.0001, 0), false, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("04_QueryPoint_Geometry_Circle_ShiftedPointOnEdge",
            t => t.CreateCircleStaticBody(5, -3, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(15, -3), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("05_QueryPoint_Geometry_Circle_ShiftedPointOutside",
            t => t.CreateCircleStaticBody(5, -3, 10).GetComponent<CircleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(15.0001, -3), false, 10d, 0.3d);
        // Rectangle
        yield return new QueryPointGeometryTestCase("06_QueryPoint_Geometry_Rectangle_PointAtCenter",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(0, 0), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("07_QueryPoint_Geometry_Rectangle_PointOnEdge",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(10, 0), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("08_QueryPoint_Geometry_Rectangle_PointOutside",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(10.0001, 0), false, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("09_QueryPoint_Geometry_Rectangle_Rotated90_PointOnEdge",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 2).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(0, 10), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("10_QueryPoint_Geometry_Rectangle_Rotated90_PointOutside",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 2).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(0, 10.0001), false, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("11_QueryPoint_Geometry_Rectangle_Rotated30_InsideAabbOutsideShape",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(11, 0), false, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("12_QueryPoint_Geometry_Rectangle_Rotated30_InsideShape",
            t => t.CreateRectangleStaticBody(0, 0, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(9, 0), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("13_QueryPoint_Geometry_Rectangle_ShiftedPointOnEdge",
            t => t.CreateRectangleStaticBody(5, -3, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(15, -3), true, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("14_QueryPoint_Geometry_Rectangle_ShiftedPointOutside",
            t => t.CreateRectangleStaticBody(5, -3, 20, 10, 0).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(15.0001, -3), false, 10d, 0.3d);
        yield return new QueryPointGeometryTestCase("15_QueryPoint_Geometry_Rectangle_ShiftedRotated_InsideAabbOutsideShape",
            t => t.CreateRectangleStaticBody(5, -3, 20, 10, Math.PI / 6).GetComponent<RectangleColliderComponent>(), CreatePhysicsConfiguration(),
            new Vector2(16, -3), false, 10d, 0.3d);
        // Tile
        yield return new QueryPointGeometryTestCase("16_QueryPoint_Geometry_Tile_PointAtCenter",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            new Vector2(0, 0), true, 40d, 0.05d);
        yield return new QueryPointGeometryTestCase("17_QueryPoint_Geometry_Tile_PointOnCorner",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            new Vector2(0.5, 0.5), true, 40d, 0.05d);
        yield return new QueryPointGeometryTestCase("18_QueryPoint_Geometry_Tile_PointOutside",
            t => t.CreateTileStaticBody(0, 0).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(1, 1),
            new Vector2(0.5001, 0.5), false, 40d, 0.05d);
        yield return new QueryPointGeometryTestCase("19_QueryPoint_Geometry_Tile_CustomSize_PointInside",
            t => t.CreateTileStaticBody(4, 6).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(2, 3),
            new Vector2(5, 7), true, 40d, 0.05d);
        yield return new QueryPointGeometryTestCase("20_QueryPoint_Geometry_Tile_CustomSize_PointOutside",
            t => t.CreateTileStaticBody(4, 6).GetComponent<TileColliderComponent>(), CreatePhysicsConfiguration(2, 3),
            new Vector2(5.0001, 7), false, 40d, 0.05d);
    }

    private static IEnumerable<TestCaseData> QueryPointGeometryTestCases()
    {
        foreach (var testCase in QueryPointGeometryCases())
        {
            yield return new TestCaseData(testCase).SetName(testCase.Name);
        }
    }

    [TestCaseSource(nameof(QueryPointGeometryTestCases))]
    public void QueryPoint_ShouldReturnCollidersMatchingGeometry(QueryPointGeometryTestCase testCase)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(testCase.PhysicsConfiguration);
        var collider = testCase.CreateCollider(this);

        physicsSystem.SynchronizePhysicsState();
        var colliders = new Collider2DComponent[1];

        SaveVisualOutput(physicsSystem, scale: testCase.VisualScale,
            postDrawAction: renderer => renderer.DrawCircle(new Circle(testCase.PointToQuery, testCase.PointMarkerRadius), Color.Red));

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