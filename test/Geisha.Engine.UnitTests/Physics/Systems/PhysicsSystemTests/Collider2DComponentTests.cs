using System;
using System.Collections.Generic;
using System.Linq;
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
    public void ContainsPoint_CircleCollider_Test(double cx, double cy, double cr, double px, double py, bool expected)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var circle = CreateCircleStaticBody(cx, cy, cr);
        var circleCollider = circle.GetComponent<CircleColliderComponent>();
        physicsSystem.SynchronizePhysicsState();

        var pointToTest = new Vector2(px, py);

        // Act
        var actual = circleCollider.ContainsPoint(pointToTest);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ContainsPoint_RectangleCollider_Test()
    {
        Assert.Fail("TODO");
    }

    [Test]
    public void ContainsPoint_TileCollider_Test()
    {
        Assert.Fail("TODO");
    }

    #endregion
}