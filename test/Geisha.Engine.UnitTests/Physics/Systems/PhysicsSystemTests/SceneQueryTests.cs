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

    [TestCase(0, 0, 0, 0)] // No colliders: 0 hits, empty list -> 0 written, list stays empty
    [TestCase(5, 0, 5, 5)] // 5 hits, empty list -> 5 written, list grows to 5
    [TestCase(5, 2, 5, 5)] // 5 hits, list size 2 -> 5 written, list grows to 5
    [TestCase(5, 5, 5, 5)] // 5 hits, list size 5 -> 5 written, list stays at 5
    [TestCase(5, 8, 5, 8)] // 5 hits, list size 8 -> 5 written, list stays at 8
    public void QueryPoint_ShouldWriteCollidersIntoList(int collidersCount, int initialListSize, int expectedWritten, int expectedFinalListSize)
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

        var colliders = new List<Collider2DComponent>();
        for (var i = 0; i < initialListSize; i++)
        {
            colliders.Add(fillerCollider);
        }

        // Act
        var written = physicsSystem.QueryPoint(pointToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(expectedWritten));
        Assert.That(colliders.Count, Is.EqualTo(expectedFinalListSize));

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

    [TestCase(0, 0, 0, 0)] // No colliders: 0 hits, empty list -> empty span, list stays empty
    [TestCase(5, 0, 5, 5)] // 5 hits, empty list -> span length 5, list grows to 5
    [TestCase(5, 2, 5, 5)] // 5 hits, list size 2 -> span length 5, list grows to 5
    [TestCase(5, 5, 5, 5)] // 5 hits, list size 5 -> span length 5, list stays at 5
    [TestCase(5, 8, 5, 8)] // 5 hits, list size 8 -> span length 5, list stays at 8
    public void QueryPointAsSpan_ShouldWriteCollidersIntoList(int collidersCount, int initialListSize, int expectedWritten, int expectedFinalListSize)
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

        var colliders = new List<Collider2DComponent>();
        for (var i = 0; i < initialListSize; i++)
        {
            colliders.Add(fillerCollider);
        }

        // Act
        var view = physicsSystem.QueryPointAsSpan(pointToQuery, colliders);

        // Assert
        Assert.That(view.Length, Is.EqualTo(expectedWritten));
        Assert.That(colliders.Count, Is.EqualTo(expectedFinalListSize));

        for (var i = 0; i < view.Length; i++)
        {
            Assert.That(view[i].ContainsPoint(pointToQuery), Is.True);
        }
    }

    private static IEnumerable<TestCaseData> QueryPointGeometryTestCases()
    {
        // Circle: x, y, radius(a), ignored(b), rotation, pointX, pointY, expected
        yield return new TestCaseData("Circle", 0d, 0d, 10d, 0d, 0d, 0d, 0d, true).SetName("QueryPoint_Geometry_Circle_PointAtCenter");
        yield return new TestCaseData("Circle", 0d, 0d, 10d, 0d, 0d, 10d, 0d, true).SetName("QueryPoint_Geometry_Circle_PointOnEdge");
        yield return new TestCaseData("Circle", 0d, 0d, 10d, 0d, 0d, 10.0001d, 0d, false).SetName("QueryPoint_Geometry_Circle_PointOutside");
        yield return new TestCaseData("Circle", 5d, -3d, 10d, 0d, 0d, 15d, -3d, true).SetName("QueryPoint_Geometry_Circle_ShiftedPointOnEdge");
        yield return new TestCaseData("Circle", 5d, -3d, 10d, 0d, 0d, 15.0001d, -3d, false).SetName("QueryPoint_Geometry_Circle_ShiftedPointOutside");

        // Rectangle: x, y, width(a), height(b), rotation, pointX, pointY, expected
        yield return new TestCaseData("Rectangle", 0d, 0d, 20d, 10d, 0d, 0d, 0d, true).SetName("QueryPoint_Geometry_Rectangle_PointAtCenter");
        yield return new TestCaseData("Rectangle", 0d, 0d, 20d, 10d, 0d, 10d, 0d, true).SetName("QueryPoint_Geometry_Rectangle_PointOnEdge");
        yield return new TestCaseData("Rectangle", 0d, 0d, 20d, 10d, 0d, 10.0001d, 0d, false).SetName("QueryPoint_Geometry_Rectangle_PointOutside");
        yield return new TestCaseData("Rectangle", 0d, 0d, 20d, 10d, Math.PI / 2, 0d, 10d, true).SetName("QueryPoint_Geometry_Rectangle_Rotated90_PointOnEdge");
        yield return new TestCaseData("Rectangle", 0d, 0d, 20d, 10d, Math.PI / 2, 0d, 10.0001d, false).SetName(
            "QueryPoint_Geometry_Rectangle_Rotated90_PointOutside");
        yield return new TestCaseData("Rectangle", 0d, 0d, 20d, 10d, Math.PI / 6, 11d, 0d, false).SetName(
            "QueryPoint_Geometry_Rectangle_Rotated30_InsideAabbOutsideShape");
        yield return new TestCaseData("Rectangle", 0d, 0d, 20d, 10d, Math.PI / 6, 9d, 0d, true).SetName("QueryPoint_Geometry_Rectangle_Rotated30_InsideShape");
        yield return new TestCaseData("Rectangle", 5d, -3d, 20d, 10d, 0d, 15d, -3d, true).SetName("QueryPoint_Geometry_Rectangle_ShiftedPointOnEdge");
        yield return new TestCaseData("Rectangle", 5d, -3d, 20d, 10d, 0d, 15.0001d, -3d, false).SetName("QueryPoint_Geometry_Rectangle_ShiftedPointOutside");
        yield return new TestCaseData("Rectangle", 5d, -3d, 20d, 10d, Math.PI / 6, 16d, -3d, false)
            .SetName("QueryPoint_Geometry_Rectangle_ShiftedRotated_InsideAabbOutsideShape");

        // Tile: x, y, tileWidth(a), tileHeight(b), ignored(rotation), pointX, pointY, expected
        yield return new TestCaseData("Tile", 0d, 0d, 1d, 1d, 0d, 0d, 0d, true).SetName("QueryPoint_Geometry_Tile_PointAtCenter");
        yield return new TestCaseData("Tile", 0d, 0d, 1d, 1d, 0d, 0.5d, 0.5d, true).SetName("QueryPoint_Geometry_Tile_PointOnCorner");
        yield return new TestCaseData("Tile", 0d, 0d, 1d, 1d, 0d, 0.5001d, 0.5d, false).SetName("QueryPoint_Geometry_Tile_PointOutside");
        yield return new TestCaseData("Tile", 4d, 6d, 2d, 3d, 0d, 5d, 7d, true).SetName("QueryPoint_Geometry_Tile_CustomSize_PointInside");
        yield return new TestCaseData("Tile", 4d, 6d, 2d, 3d, 0d, 5.0001d, 7d, false).SetName("QueryPoint_Geometry_Tile_CustomSize_PointOutside");
    }

    [TestCaseSource(nameof(QueryPointGeometryTestCases))]
    public void QueryPoint_ShouldReturnCollidersMatchingGeometry(
        string colliderType,
        double x,
        double y,
        double a,
        double b,
        double rotation,
        double pointX,
        double pointY,
        bool expectedHit)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            EnableDebugRendering = true,
            TileSize = new SizeD(a <= 0 ? 1 : a, b <= 0 ? 1 : b)
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);

        Collider2DComponent collider = colliderType switch
        {
            "Circle" => CreateCircleStaticBody(x, y, a).GetComponent<CircleColliderComponent>(),
            "Rectangle" => CreateRectangleStaticBody(x, y, a, b, rotation).GetComponent<RectangleColliderComponent>(),
            "Tile" => CreateTileStaticBody(x, y).GetComponent<TileColliderComponent>(),
            _ => throw new ArgumentOutOfRangeException(nameof(colliderType), colliderType, "Unsupported collider type.")
        };

        physicsSystem.SynchronizePhysicsState();

        var pointToQuery = new Vector2(pointX, pointY);
        var colliders = new Collider2DComponent[1];

        var scale = colliderType == "Tile" ? 40 : 10;
        SaveVisualOutput(physicsSystem, scale: scale,
            postDrawAction: renderer => renderer.DrawCircle(new Circle(pointToQuery, colliderType == "Tile" ? 0.05 : 0.3), Color.Red));

        // Act
        var written = physicsSystem.QueryPoint(pointToQuery, colliders);

        // Assert
        Assert.That(written, Is.EqualTo(expectedHit ? 1 : 0));

        if (expectedHit)
        {
            Assert.That(colliders[0], Is.EqualTo(collider));
        }
    }

    #endregion

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
}