using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class GetContactsTests : PhysicsSystemTestsBase
{
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
        Assert.That(contacts.Count, Is.EqualTo(initialListSize));

        // Act
        var contactsView = circleCollider.GetContactsAsSpan(contacts);

        // Assert
        Assert.That(contactsView.Length, Is.EqualTo(contactsWritten));
        Assert.That(contactsView.ToArray(), Is.EqualTo(contacts.ToArray().AsSpan(0, contactsWritten).ToArray()));
        Assert.That(contacts.Count, Is.EqualTo(finalListSize));
    }
}