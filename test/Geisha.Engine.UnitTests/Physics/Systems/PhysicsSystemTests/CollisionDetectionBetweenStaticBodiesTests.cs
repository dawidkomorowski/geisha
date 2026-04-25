using System.Collections.Generic;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class CollisionDetectionBetweenStaticBodiesTests : PhysicsSystemTestsBase
{
    [Test]
    public void ProcessPhysics_RectangleStaticBodyShouldNotCollideWithOtherRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var staticBody1 = CreateRectangleStaticBody(0, 0, 10, 5);
        var staticBody2 = CreateRectangleStaticBody(5, 0, 10, 5);

        var staticBody1Collider = staticBody1.GetComponent<RectangleColliderComponent>();
        var staticBody2Collider = staticBody2.GetComponent<RectangleColliderComponent>();

        // Assume
        Assert.That(staticBody1Collider.IsColliding, Is.False);
        Assert.That(staticBody2Collider.IsColliding, Is.False);

        Assert.That(staticBody1Collider.Enabled, Is.True);
        Assert.That(staticBody2Collider.Enabled, Is.True);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var staticBody1Contacts = new List<Contact2D>();
        Assert.That(staticBody1Collider.IsColliding, Is.False);
        Assert.That(staticBody1Collider.GetContacts(staticBody1Contacts), Is.Zero);
        Assert.That(staticBody1Contacts, Has.Count.Zero);

        var staticBody2Contacts = new List<Contact2D>();
        Assert.That(staticBody2Collider.IsColliding, Is.False);
        Assert.That(staticBody2Collider.GetContacts(staticBody2Contacts), Is.Zero);
        Assert.That(staticBody2Contacts, Has.Count.Zero);
    }

    [Test]
    public void ProcessPhysics_CircleStaticBodyShouldNotCollideWithOtherCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var staticBody1 = CreateCircleStaticBody(0, 0, 10);
        var staticBody2 = CreateCircleStaticBody(5, 0, 10);

        var staticBody1Collider = staticBody1.GetComponent<CircleColliderComponent>();
        var staticBody2Collider = staticBody2.GetComponent<CircleColliderComponent>();

        // Assume
        Assert.That(staticBody1Collider.IsColliding, Is.False);
        Assert.That(staticBody2Collider.IsColliding, Is.False);

        Assert.That(staticBody1Collider.Enabled, Is.True);
        Assert.That(staticBody2Collider.Enabled, Is.True);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var staticBody1Contacts = new List<Contact2D>();
        Assert.That(staticBody1Collider.IsColliding, Is.False);
        Assert.That(staticBody1Collider.GetContacts(staticBody1Contacts), Is.Zero);
        Assert.That(staticBody1Contacts, Has.Count.Zero);

        var staticBody2Contacts = new List<Contact2D>();
        Assert.That(staticBody2Collider.IsColliding, Is.False);
        Assert.That(staticBody2Collider.GetContacts(staticBody2Contacts), Is.Zero);
        Assert.That(staticBody2Contacts, Has.Count.Zero);
    }

    [Test]
    public void ProcessPhysics_RectangleStaticBodyShouldNotCollideWithCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var staticBody1 = CreateRectangleStaticBody(0, 0, 10, 5);
        var staticBody2 = CreateCircleStaticBody(5, 0, 10);

        var staticBody1Collider = staticBody1.GetComponent<RectangleColliderComponent>();
        var staticBody2Collider = staticBody2.GetComponent<CircleColliderComponent>();

        // Assume
        Assert.That(staticBody1Collider.IsColliding, Is.False);
        Assert.That(staticBody2Collider.IsColliding, Is.False);

        Assert.That(staticBody1Collider.Enabled, Is.True);
        Assert.That(staticBody2Collider.Enabled, Is.True);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var staticBody1Contacts = new List<Contact2D>();
        Assert.That(staticBody1Collider.IsColliding, Is.False);
        Assert.That(staticBody1Collider.GetContacts(staticBody1Contacts), Is.Zero);
        Assert.That(staticBody1Contacts, Has.Count.Zero);

        var staticBody2Contacts = new List<Contact2D>();
        Assert.That(staticBody2Collider.IsColliding, Is.False);
        Assert.That(staticBody2Collider.GetContacts(staticBody2Contacts), Is.Zero);
        Assert.That(staticBody2Contacts, Has.Count.Zero);
    }
}