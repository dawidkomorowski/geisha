using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class Collider2DComponentTests : PhysicsSystemTestsBase
{
    #region BoundingRectangle

    [TestCase(0, 0, 10, 0, 0, 20, 20)] // Circle at origin
    [TestCase(0, 0, 5, 0, 0, 10, 10)] // Smaller radius
    [TestCase(5, 3, 10, 5, 3, 20, 20)] // Circle offset from origin
    public void BoundingRectangle_CircleCollider_ShouldReturnCorrectAxisAlignedRectangle(
        double cx, double cy, double cr, double brX, double brY, double brW, double brH)
    {
        // Arrange
        var expectedBoundingRectangle = new AxisAlignedRectangle(brX, brY, brW, brH);

        var physicsSystem = GetPhysicsSystem();
        var circle = CreateCircleStaticBody(cx, cy, cr);
        var circleCollider = circle.GetComponent<CircleColliderComponent>();

        physicsSystem.ProcessPhysics();

        // Act
        var boundingRectangle = circleCollider.BoundingRectangle;

        // Assert
        Assert.That(boundingRectangle, Is.EqualTo(expectedBoundingRectangle));
    }

    [TestCase(0, 0, 20, 10, 0, 0, 0, 20, 10)] // Rectangle at origin
    [TestCase(0, 0, 10, 5, 0, 0, 0, 10, 5)] // Smaller dimensions
    [TestCase(5, 3, 20, 10, 0, 5, 3, 20, 10)] // Rectangle offset from origin
    [TestCase(0, 0, 20, 10, Math.PI / 6, 0, 0, 22.320508075688775, 18.660254037844386)] // Rotated 30°
    public void BoundingRectangle_RectangleCollider_ShouldReturnCorrectAxisAlignedRectangle(
        double rx, double ry, double rw, double rh, double rr, double brX, double brY, double brW, double brH)
    {
        // Arrange
        var expectedBoundingRectangle = new AxisAlignedRectangle(brX, brY, brW, brH);

        var physicsSystem = GetPhysicsSystem();
        var rectangle = CreateRectangleStaticBody(rx, ry, rw, rh, rr);
        var rectangleCollider = rectangle.GetComponent<RectangleColliderComponent>();

        physicsSystem.ProcessPhysics();

        // Act
        var boundingRectangle = rectangleCollider.BoundingRectangle;

        // Assert
        Assert.That(boundingRectangle, Is.EqualTo(expectedBoundingRectangle).Using<AxisAlignedRectangle>(ToleranceEquality.ForAxisAlignedRectangle(1e-9)));
    }

    [TestCase(0, 0, 1, 1, 0, 0, 1, 1)] // Tile at origin, default size
    [TestCase(5, 3, 1, 1, 5, 3, 1, 1)] // Tile offset from origin, default size
    [TestCase(0, 0, 2, 3, 0, 0, 2, 3)] // Tile at origin, custom size
    public void BoundingRectangle_TileCollider_ShouldReturnCorrectAxisAlignedRectangle(
        double tx, double ty, double tw, double th, double brX, double brY, double brW, double brH)
    {
        // Arrange
        var expectedBoundingRectangle = new AxisAlignedRectangle(brX, brY, brW, brH);

        var physicsConfiguration = new PhysicsConfiguration { TileSize = new SizeD(tw, th) };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
        var tile = CreateTileStaticBody(tx, ty);
        var tileCollider = tile.GetComponent<TileColliderComponent>();

        physicsSystem.ProcessPhysics();

        // Act
        var boundingRectangle = tileCollider.BoundingRectangle;

        // Assert
        Assert.That(boundingRectangle, Is.EqualTo(expectedBoundingRectangle));
    }

    #endregion

    #region GetContacts

    [TestCase(0, 0, 0)] // No other bodies: 0 contacts, empty buffer → 0 contacts written
    [TestCase(5, 0, 0)] // 5 other bodies (5 contacts), buffer size 0 → 0 contacts written (buffer full)
    [TestCase(5, 2, 2)] // 5 other bodies (5 contacts), buffer size 2 → 2 contacts written (buffer full)
    [TestCase(5, 5, 5)] // 5 other bodies (5 contacts), buffer size 5 → 5 contacts written (exact fit)
    [TestCase(5, 8, 5)] // 5 other bodies (5 contacts), buffer size 8 → 5 contacts written (buffer has room)
    public void GetContacts_ShouldWriteContactsIntoSpan(int bodies, int bufferSize, int contactsWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var circle = CreateCircleKinematicBody(0, 0, 10);
        var circleCollider = circle.GetComponent<CircleColliderComponent>();

        for (var i = 0; i < bodies; i++)
        {
            CreateCircleKinematicBody(0, 0, 10);
        }

        physicsSystem.ProcessPhysics();

        var contacts = new Contact2D[bufferSize];

        // Assume
        Assert.That(circleCollider.ContactCount, Is.EqualTo(bodies));

        // Act
        var contactsCount = circleCollider.GetContacts(contacts);

        // Assert
        Assert.That(contactsCount, Is.EqualTo(contactsWritten));
    }

    [TestCase(0, 0, 0, 0)] // No other bodies: 0 contacts, empty list → 0 contacts written, list stays empty
    [TestCase(5, 0, 5, 5)] // 5 other bodies (5 contacts), empty list → 5 contacts written, list grows to 5
    [TestCase(5, 2, 5, 5)] // 5 other bodies (5 contacts), list size 2 → 5 contacts written, list grows to 5
    [TestCase(5, 5, 5, 5)] // 5 other bodies (5 contacts), list size 5 → 5 contacts written, list stays at 5
    [TestCase(5, 8, 5, 8)] // 5 other bodies (5 contacts), list size 8 → 5 contacts written, list stays at 8
    public void GetContacts_ShouldWriteContactsIntoList(int bodies, int initialListSize, int contactsWritten, int finalListSize)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var circle = CreateCircleKinematicBody(0, 0, 10);
        var circleCollider = circle.GetComponent<CircleColliderComponent>();

        for (var i = 0; i < bodies; i++)
        {
            CreateCircleKinematicBody(0, 0, 10);
        }

        physicsSystem.ProcessPhysics();

        var contacts = new List<Contact2D>(Enumerable.Repeat(new Contact2D(), initialListSize));

        // Assume
        Assert.That(circleCollider.ContactCount, Is.EqualTo(bodies));
        Assert.That(contacts.Count, Is.EqualTo(initialListSize));

        // Act
        var contactsCount = circleCollider.GetContacts(contacts);

        // Assert
        Assert.That(contactsCount, Is.EqualTo(contactsWritten));
        Assert.That(contacts.Count, Is.EqualTo(finalListSize));
    }

    [TestCase(0, 0, 0)] // No other bodies: 0 contacts, empty buffer → 0 contacts written
    [TestCase(5, 0, 0)] // 5 other bodies (5 contacts), buffer size 0 → 0 contacts written (buffer full)
    [TestCase(5, 2, 2)] // 5 other bodies (5 contacts), buffer size 2 → 2 contacts written (buffer full)
    [TestCase(5, 5, 5)] // 5 other bodies (5 contacts), buffer size 5 → 5 contacts written (exact fit)
    [TestCase(5, 8, 5)] // 5 other bodies (5 contacts), buffer size 8 → 5 contacts written (buffer has room)
    public void GetContactsAsSpan_ShouldWriteContactsIntoSpan(int bodies, int bufferSize, int contactsWritten)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var circle = CreateCircleKinematicBody(0, 0, 10);
        var circleCollider = circle.GetComponent<CircleColliderComponent>();

        for (var i = 0; i < bodies; i++)
        {
            CreateCircleKinematicBody(0, 0, 10);
        }

        physicsSystem.ProcessPhysics();

        var contacts = new Contact2D[bufferSize];

        // Assume
        Assert.That(circleCollider.ContactCount, Is.EqualTo(bodies));

        // Act
        var contactsView = circleCollider.GetContactsAsSpan(contacts);

        // Assert
        Assert.That(contactsView.Length, Is.EqualTo(contactsWritten));
        Assert.That(contactsView.ToArray(), Is.EqualTo(contacts.AsSpan(0, contactsWritten).ToArray()));
    }

    [TestCase(0, 0, 0, 0)] // No other bodies: 0 contacts, empty list → 0 contacts written, list stays empty
    [TestCase(5, 0, 5, 5)] // 5 other bodies (5 contacts), empty list → 5 contacts written, list grows to 5
    [TestCase(5, 2, 5, 5)] // 5 other bodies (5 contacts), list size 2 → 5 contacts written, list grows to 5
    [TestCase(5, 5, 5, 5)] // 5 other bodies (5 contacts), list size 5 → 5 contacts written, list stays at 5
    [TestCase(5, 8, 5, 8)] // 5 other bodies (5 contacts), list size 8 → 5 contacts written, list stays at 8
    public void GetContactsAsSpan_ShouldWriteContactsIntoList(int bodies, int initialListSize, int contactsWritten, int finalListSize)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var circle = CreateCircleKinematicBody(0, 0, 10);
        var circleCollider = circle.GetComponent<CircleColliderComponent>();

        for (var i = 0; i < bodies; i++)
        {
            CreateCircleKinematicBody(0, 0, 10);
        }

        physicsSystem.ProcessPhysics();

        var contacts = new List<Contact2D>(Enumerable.Repeat(new Contact2D(), initialListSize));

        // Assume
        Assert.That(circleCollider.ContactCount, Is.EqualTo(bodies));
        Assert.That(contacts.Count, Is.EqualTo(initialListSize));

        // Act
        var contactsView = circleCollider.GetContactsAsSpan(contacts);

        // Assert
        Assert.That(contactsView.Length, Is.EqualTo(contactsWritten));
        Assert.That(contactsView.ToArray(), Is.EqualTo(contacts.ToArray().AsSpan(0, contactsWritten).ToArray()));
        Assert.That(contacts.Count, Is.EqualTo(finalListSize));
    }

    #endregion

    #region ContainsPoint

    [TestCase(0, 0, 10, 0, 0, true)] // Point at center
    [TestCase(0, 0, 10, 10, 0, true)] // Point on edge
    [TestCase(0, 0, 10, 10.0001, 0, false)] // Point outside
    [TestCase(5, -3, 10, 15, -3, true)] // Shifted circle: point on edge
    [TestCase(5, -3, 10, 15.0001, -3, false)] // Shifted circle: point outside
    public void ContainsPoint_CircleCollider_Test(double cx, double cy, double cr, double px, double py, bool expected)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var circle = CreateCircleStaticBody(cx, cy, cr);
        var circleCollider = circle.GetComponent<CircleColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        var pointToTest = new Vector2(px, py);

        SaveVisualOutput(physicsSystem, scale: 10, postDrawAction: renderer => renderer.DrawCircle(new Circle(pointToTest, 0.3), Color.Red));

        // Act
        var actual = circleCollider.ContainsPoint(pointToTest);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(0, 0, 20, 10, 0, 0, 0, true)] // Point at center
    [TestCase(0, 0, 20, 10, 0, 10, 0, true)] // Point on edge
    [TestCase(0, 0, 20, 10, 0, 10.0001, 0, false)] // Point outside
    [TestCase(0, 0, 20, 10, Math.PI / 2, 0, 10, true)] // Rotated 90°: point on edge
    [TestCase(0, 0, 20, 10, Math.PI / 2, 0, 10.0001, false)] // Rotated 90°: point outside
    [TestCase(0, 0, 20, 10, Math.PI / 6, 11, 0, false)] // Rotated 30°: inside AABB but outside rotated rectangle
    [TestCase(0, 0, 20, 10, Math.PI / 6, 9, 0, true)] // Rotated 30°: point inside rotated rectangle
    [TestCase(5, -3, 20, 10, 0, 15, -3, true)] // Shifted rectangle: point on edge
    [TestCase(5, -3, 20, 10, 0, 15.0001, -3, false)] // Shifted rectangle: point outside
    [TestCase(5, -3, 20, 10, Math.PI / 6, 16, -3, false)] // Shifted rotated rectangle: inside AABB but outside rotated rectangle
    public void ContainsPoint_RectangleCollider_Test(double rx, double ry, double rw, double rh, double rr, double px, double py, bool expected)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var rectangle = CreateRectangleStaticBody(rx, ry, rw, rh, rr);
        var rectangleCollider = rectangle.GetComponent<RectangleColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        var pointToTest = new Vector2(px, py);

        SaveVisualOutput(physicsSystem, scale: 10, postDrawAction: renderer => renderer.DrawCircle(new Circle(pointToTest, 0.3), Color.Red));

        // Act
        var actual = rectangleCollider.ContainsPoint(pointToTest);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(0, 0, 1, 1, 0, 0, true)] // Point at center of tile at grid origin
    [TestCase(0, 0, 1, 1, 0.5, 0.5, true)] // Point on upper-right corner of tile at grid origin
    [TestCase(0, 0, 1, 1, 0.5001, 0.5, false)] // Point outside tile at grid origin
    [TestCase(4, 6, 2, 3, 5, 7, true)] // Point inside custom-size tile
    [TestCase(4, 6, 2, 3, 5.0001, 7, false)] // Point outside custom-size tile
    public void ContainsPoint_TileCollider_Test(double tx, double ty, double tw, double th, double px, double py, bool expected)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(tw, th),
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
        var tile = CreateTileStaticBody(tx, ty);
        var tileCollider = tile.GetComponent<TileColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        var pointToTest = new Vector2(px, py);

        SaveVisualOutput(physicsSystem, scale: 40, postDrawAction: renderer => renderer.DrawCircle(new Circle(pointToTest, 0.05), Color.Red));

        // Assume
        Assert.That(tile.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(tx, ty)), "Tile is misaligned.");

        // Act
        var actual = tileCollider.ContainsPoint(pointToTest);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    #endregion

    #region Overlaps AxisAlignedRectangle

    [TestCase(0, 0, 10, 0, 0, 2, 2, true)] // AABB fully inside circle
    [TestCase(0, 0, 10, 11, 0, 2, 2, true)] // AABB touching circle edge
    [TestCase(0, 0, 10, 11.0001, 0, 2, 2, false)] // AABB outside circle
    [TestCase(0, 0, 10, 9, 9, 1, 1, false)] // AABB inside circle bounding box but outside actual circle
    [TestCase(5, -3, 10, 15, -3, 2, 2, true)] // Shifted circle overlap
    [TestCase(5, -3, 10, 16.0001, -3, 2, 2, false)] // Shifted circle no overlap
    public void Overlaps_AxisAlignedRectangle_CircleCollider_Test(double cx, double cy, double cr, double aabbX, double aabbY, double aabbW, double aabbH,
        bool expected)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var circle = CreateCircleStaticBody(cx, cy, cr);
        var circleCollider = circle.GetComponent<CircleColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        var aabbToTest = new AxisAlignedRectangle(aabbX, aabbY, aabbW, aabbH);

        SaveVisualOutput(physicsSystem, scale: 10, postDrawAction: renderer => renderer.DrawRectangle(aabbToTest, Color.Red, Matrix3x3.Identity));

        // Act
        var actual = circleCollider.Overlaps(aabbToTest);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(0, 0, 20, 10, 0, 0, 0, 2, 2, true)] // AABB fully inside rectangle
    [TestCase(0, 0, 20, 10, 0, 11, 0, 2, 2, true)] // AABB touching rectangle edge
    [TestCase(0, 0, 20, 10, 0, 11.0001, 0, 2, 2, false)] // AABB outside rectangle
    [TestCase(0, 0, 20, 10, Math.PI / 6, 11, 0, 1, 1, true)] // Rotated rectangle: AABB touches shape
    [TestCase(0, 0, 20, 10, Math.PI / 6, 11, -2, 1, 1, false)] // Rotated rectangle: AABB inside bounding AABB but outside shape
    [TestCase(5, -3, 20, 10, 0, 15, -3, 2, 2, true)] // Shifted rectangle overlap
    public void Overlaps_AxisAlignedRectangle_RectangleCollider_Test(double rx, double ry, double rw, double rh, double rr, double aabbX, double aabbY,
        double aabbW, double aabbH, bool expected)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var rectangle = CreateRectangleStaticBody(rx, ry, rw, rh, rr);
        var rectangleCollider = rectangle.GetComponent<RectangleColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        var aabbToTest = new AxisAlignedRectangle(aabbX, aabbY, aabbW, aabbH);

        SaveVisualOutput(physicsSystem, scale: 10, postDrawAction: renderer => renderer.DrawRectangle(aabbToTest, Color.Red, Matrix3x3.Identity));

        // Act
        var actual = rectangleCollider.Overlaps(aabbToTest);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(0, 0, 1, 1, 0, 0, 0.2, 0.2, true)] // AABB fully inside tile at grid origin
    [TestCase(0, 0, 1, 1, 1, 0, 1, 1, true)] // AABB touching tile edge at grid origin
    [TestCase(0, 0, 1, 1, 1.0001, 0, 1, 1, false)] // AABB outside tile at grid origin
    [TestCase(4, 6, 2, 3, 5, 7, 0.2, 0.2, true)] // AABB inside custom-size tile aligned to grid
    [TestCase(4, 6, 2, 3, 5.1001, 7, 0.2, 0.2, false)] // AABB outside custom-size tile aligned to grid
    public void Overlaps_AxisAlignedRectangle_TileCollider_Test(double tx, double ty, double tw, double th, double aabbX, double aabbY, double aabbW,
        double aabbH, bool expected)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(tw, th),
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
        var tile = CreateTileStaticBody(tx, ty);
        var tileCollider = tile.GetComponent<TileColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        var aabbToTest = new AxisAlignedRectangle(aabbX, aabbY, aabbW, aabbH);

        SaveVisualOutput(physicsSystem, scale: 40, postDrawAction: renderer => renderer.DrawRectangle(aabbToTest, Color.Red, Matrix3x3.Identity));

        // Assume
        Assert.That(tile.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(tx, ty)), "Tile is misaligned.");

        // Act
        var actual = tileCollider.Overlaps(aabbToTest);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    #endregion

    #region Overlaps Circle

    [TestCase(0, 0, 10, 0, 0, 2, true)] // Circle fully inside collider circle
    [TestCase(0, 0, 10, 12, 0, 2, true)] // Circle touching collider circle edge
    [TestCase(0, 0, 10, 12.0001, 0, 2, false)] // Circle outside collider circle
    [TestCase(5, -3, 10, 15, -3, 2, true)] // Shifted circle overlap
    [TestCase(5, -3, 10, 17.0001, -3, 2, false)] // Shifted circle no overlap
    public void Overlaps_Circle_CircleCollider_Test(double cx, double cy, double cr, double testCx, double testCy, double testCr, bool expected)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var circle = CreateCircleStaticBody(cx, cy, cr);
        var circleCollider = circle.GetComponent<CircleColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        var circleToTest = new Circle(new Vector2(testCx, testCy), testCr);

        SaveVisualOutput(physicsSystem, scale: 10, postDrawAction: renderer => renderer.DrawCircle(circleToTest, Color.Red));

        // Act
        var actual = circleCollider.Overlaps(circleToTest);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(0, 0, 20, 10, 0, 0, 0, 2, true)] // Circle center inside rectangle
    [TestCase(0, 0, 20, 10, 0, 11, 0, 1, true)] // Circle touching rectangle edge
    [TestCase(0, 0, 20, 10, 0, 11.0001, 0, 1, false)] // Circle outside rectangle
    [TestCase(0, 0, 20, 10, Math.PI / 6, 11, 0, 0.5, true)] // Rotated rectangle: circle touching shape
    [TestCase(0, 0, 20, 10, Math.PI / 6, 11, -2, 0.5, false)] // Rotated rectangle: circle inside bounding AABB but outside shape
    [TestCase(5, -3, 20, 10, 0, 15, -3, 1, true)] // Shifted rectangle overlap
    public void Overlaps_Circle_RectangleCollider_Test(double rx, double ry, double rw, double rh, double rr, double testCx, double testCy, double testCr,
        bool expected)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var rectangle = CreateRectangleStaticBody(rx, ry, rw, rh, rr);
        var rectangleCollider = rectangle.GetComponent<RectangleColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        var circleToTest = new Circle(new Vector2(testCx, testCy), testCr);

        SaveVisualOutput(physicsSystem, scale: 10, postDrawAction: renderer => renderer.DrawCircle(circleToTest, Color.Red));

        // Act
        var actual = rectangleCollider.Overlaps(circleToTest);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(0, 0, 1, 1, 0, 0, 0.1, true)] // Circle inside tile at grid origin
    [TestCase(0, 0, 1, 1, 1, 0, 0.5, true)] // Circle touching tile edge at grid origin
    [TestCase(0, 0, 1, 1, 1.0001, 0, 0.5, false)] // Circle outside tile at grid origin
    [TestCase(0, 0, 1, 1, 0.9, 0.9, 0.4, false)] // Circle AABB touches tile AABB, but circle is outside tile shape
    [TestCase(4, 6, 2, 3, 5, 7, 0.1, true)] // Circle inside custom-size tile aligned to grid
    [TestCase(4, 6, 2, 3, 5.1001, 7, 0.1, false)] // Circle outside custom-size tile aligned to grid
    public void Overlaps_Circle_TileCollider_Test(double tx, double ty, double tw, double th, double testCx, double testCy, double testCr, bool expected)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            TileSize = new SizeD(tw, th),
            EnableDebugRendering = true
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
        var tile = CreateTileStaticBody(tx, ty);
        var tileCollider = tile.GetComponent<TileColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        var circleToTest = new Circle(new Vector2(testCx, testCy), testCr);

        SaveVisualOutput(physicsSystem, scale: 40, postDrawAction: renderer => renderer.DrawCircle(circleToTest, Color.Red));

        // Assume
        Assert.That(tile.GetComponent<Transform2DComponent>().Translation, Is.EqualTo(new Vector2(tx, ty)), "Tile is misaligned.");

        // Act
        var actual = tileCollider.Overlaps(circleToTest);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    #endregion
}