using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class CollisionDetectionBetweenKinematicBodiesTests : PhysicsSystemTestsBase
{
    [Test]
    public void ProcessPhysics_ShouldLeaveKinematicBodiesNotColliding_WhenTheyWereNotCollidingAndTheyStillNotCollide()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var rectangle1 = CreateRectangleKinematicBody(0, 0, 10, 5);
        var rectangle2 = CreateRectangleKinematicBody(20, 0, 10, 5);

        var rectangle1Collider = rectangle1.GetComponent<RectangleColliderComponent>();
        var rectangle2Collider = rectangle2.GetComponent<RectangleColliderComponent>();

        // Assume
        Assert.That(rectangle1Collider.IsColliding, Is.False);
        Assert.That(rectangle2Collider.IsColliding, Is.False);

        Assert.That(rectangle1Collider.Enabled, Is.True);
        Assert.That(rectangle2Collider.Enabled, Is.True);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangle1Collider.IsColliding, Is.False);
        Assert.That(rectangle1Collider.GetContacts(), Has.Length.Zero);

        Assert.That(rectangle2Collider.IsColliding, Is.False);
        Assert.That(rectangle2Collider.GetContacts(), Has.Length.Zero);
    }

    [Test]
    public void ProcessPhysics_ShouldMakeKinematicBodiesNotColliding_WhenTheyWereCollidingButTheyNotCollideAnymore()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var rectangle1 = CreateRectangleKinematicBody(0, 0, 10, 5);
        var rectangle2 = CreateRectangleKinematicBody(0, 0, 10, 5);

        var rectangle1Collider = rectangle1.GetComponent<RectangleColliderComponent>();
        var rectangle2Collider = rectangle2.GetComponent<RectangleColliderComponent>();

        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(rectangle1Collider.IsColliding, Is.True);
        Assert.That(rectangle2Collider.IsColliding, Is.True);

        Assert.That(rectangle1Collider.Enabled, Is.True);
        Assert.That(rectangle2Collider.Enabled, Is.True);

        // Act
        rectangle2.GetComponent<Transform2DComponent>().Translation = new Vector2(20, 0);
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangle1Collider.IsColliding, Is.False);
        Assert.That(rectangle1Collider.GetContacts(), Has.Length.Zero);

        Assert.That(rectangle2Collider.IsColliding, Is.False);
        Assert.That(rectangle2Collider.GetContacts(), Has.Length.Zero);
    }

    [Test]
    public void ProcessPhysics_ShouldMakeKinematicBodiesColliding_WhenTheyWereNotCollidingButTheyCollideNow()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var rectangle1 = CreateRectangleKinematicBody(0, 0, 10, 5);
        var rectangle2 = CreateRectangleKinematicBody(5, 0, 10, 5);

        var rectangle1Collider = rectangle1.GetComponent<RectangleColliderComponent>();
        var rectangle2Collider = rectangle2.GetComponent<RectangleColliderComponent>();

        // Assume
        Assert.That(rectangle1Collider.IsColliding, Is.False);
        Assert.That(rectangle2Collider.IsColliding, Is.False);

        Assert.That(rectangle1Collider.Enabled, Is.True);
        Assert.That(rectangle2Collider.Enabled, Is.True);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangle1Collider.IsColliding, Is.True);
        var r1Contacts = rectangle1Collider.GetContacts();
        Assert.That(r1Contacts, Has.Length.EqualTo(1));
        Assert.That(r1Contacts[0].ThisCollider, Is.EqualTo(rectangle1Collider));
        Assert.That(r1Contacts[0].OtherCollider, Is.EqualTo(rectangle2Collider));

        Assert.That(rectangle2Collider.IsColliding, Is.True);
        var r2Contacts = rectangle2Collider.GetContacts();
        Assert.That(r2Contacts, Has.Length.EqualTo(1));
        Assert.That(r2Contacts[0].ThisCollider, Is.EqualTo(rectangle2Collider));
        Assert.That(r2Contacts[0].OtherCollider, Is.EqualTo(rectangle1Collider));
    }

    [Test]
    public void ProcessPhysics_ShouldMakeCircleKinematicBodiesColliding_WhenTheyWereNotCollidingButTheyCollideNow()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var circle1 = CreateCircleKinematicBody(0, 0, 10);
        var circle2 = CreateCircleKinematicBody(5, 0, 10);

        var circle1Collider = circle1.GetComponent<CircleColliderComponent>();
        var circle2Collider = circle2.GetComponent<CircleColliderComponent>();

        // Assume
        Assert.That(circle1Collider.IsColliding, Is.False);
        Assert.That(circle2Collider.IsColliding, Is.False);

        Assert.That(circle1Collider.Enabled, Is.True);
        Assert.That(circle2Collider.Enabled, Is.True);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(circle1Collider.IsColliding, Is.True);
        var c1Contacts = circle1Collider.GetContacts();
        Assert.That(c1Contacts, Has.Length.EqualTo(1));
        Assert.That(c1Contacts[0].ThisCollider, Is.EqualTo(circle1Collider));
        Assert.That(c1Contacts[0].OtherCollider, Is.EqualTo(circle2Collider));

        Assert.That(circle2Collider.IsColliding, Is.True);
        var c2Contacts = circle2Collider.GetContacts();
        Assert.That(c2Contacts, Has.Length.EqualTo(1));
        Assert.That(c2Contacts[0].ThisCollider, Is.EqualTo(circle2Collider));
        Assert.That(c2Contacts[0].OtherCollider, Is.EqualTo(circle1Collider));
    }

    [Test]
    public void ProcessPhysics_ShouldMakeKinematicBodiesCollidingAndNotCollidingWithOtherKinematicBodies_WhenThereAreManyCirclesAndRectangles()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var circle1 = CreateCircleKinematicBody(0, 0, 10);
        var circle2 = CreateCircleKinematicBody(15, 0, 10);
        var circle3 = CreateCircleKinematicBody(50, 50, 10);
        var rectangle1 = CreateRectangleKinematicBody(0, 0, 20, 10);
        var rectangle2 = CreateRectangleKinematicBody(45, 45, 10, 5);
        var rectangle3 = CreateRectangleKinematicBody(150, 100, 10, 5);

        var circle1Collider = circle1.GetComponent<CircleColliderComponent>();
        var circle2Collider = circle2.GetComponent<CircleColliderComponent>();
        var circle3Collider = circle3.GetComponent<CircleColliderComponent>();
        var rectangle1Collider = rectangle1.GetComponent<RectangleColliderComponent>();
        var rectangle2Collider = rectangle2.GetComponent<RectangleColliderComponent>();
        var rectangle3Collider = rectangle3.GetComponent<RectangleColliderComponent>();

        // Assume
        Assert.That(circle1Collider.IsColliding, Is.False);
        Assert.That(circle2Collider.IsColliding, Is.False);
        Assert.That(circle3Collider.IsColliding, Is.False);
        Assert.That(rectangle1Collider.IsColliding, Is.False);
        Assert.That(rectangle2Collider.IsColliding, Is.False);
        Assert.That(rectangle3Collider.IsColliding, Is.False);

        Assert.That(circle1Collider.Enabled, Is.True);
        Assert.That(circle2Collider.Enabled, Is.True);
        Assert.That(circle3Collider.Enabled, Is.True);
        Assert.That(rectangle1Collider.Enabled, Is.True);
        Assert.That(rectangle2Collider.Enabled, Is.True);
        Assert.That(rectangle3Collider.Enabled, Is.True);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(circle1Collider.IsColliding, Is.True);
        var c1Contacts = circle1Collider.GetContacts();
        Assert.That(c1Contacts, Has.Length.EqualTo(2));
        Assert.That(c1Contacts[0].ThisCollider, Is.EqualTo(circle1Collider));
        Assert.That(c1Contacts[1].ThisCollider, Is.EqualTo(circle1Collider));
        Assert.That(c1Contacts.Any(c => c.OtherCollider == circle2Collider), Is.True);
        Assert.That(c1Contacts.Any(c => c.OtherCollider == rectangle1Collider), Is.True);

        Assert.That(circle2Collider.IsColliding, Is.True);
        var c2Contacts = circle2Collider.GetContacts();
        Assert.That(c2Contacts, Has.Length.EqualTo(2));
        Assert.That(c2Contacts[0].ThisCollider, Is.EqualTo(circle2Collider));
        Assert.That(c2Contacts[1].ThisCollider, Is.EqualTo(circle2Collider));
        Assert.That(c2Contacts.Any(c => c.OtherCollider == circle1Collider), Is.True);
        Assert.That(c2Contacts.Any(c => c.OtherCollider == rectangle1Collider), Is.True);

        Assert.That(circle3Collider.IsColliding, Is.True);
        var c3Contacts = circle3Collider.GetContacts();
        Assert.That(c3Contacts, Has.Length.EqualTo(1));
        Assert.That(c3Contacts[0].ThisCollider, Is.EqualTo(circle3Collider));
        Assert.That(c3Contacts[0].OtherCollider, Is.EqualTo(rectangle2Collider));

        Assert.That(rectangle1Collider.IsColliding, Is.True);
        var r1Contacts = rectangle1Collider.GetContacts();
        Assert.That(r1Contacts, Has.Length.EqualTo(2));
        Assert.That(r1Contacts[0].ThisCollider, Is.EqualTo(rectangle1Collider));
        Assert.That(r1Contacts[1].ThisCollider, Is.EqualTo(rectangle1Collider));
        Assert.That(r1Contacts.Any(c => c.OtherCollider == circle1Collider), Is.True);
        Assert.That(r1Contacts.Any(c => c.OtherCollider == circle2Collider), Is.True);

        Assert.That(rectangle2Collider.IsColliding, Is.True);
        var r2Contacts = rectangle2Collider.GetContacts();
        Assert.That(r2Contacts, Has.Length.EqualTo(1));
        Assert.That(r2Contacts[0].ThisCollider, Is.EqualTo(rectangle2Collider));
        Assert.That(r2Contacts[0].OtherCollider, Is.EqualTo(circle3Collider));

        Assert.That(rectangle3Collider.IsColliding, Is.False);
        Assert.That(rectangle3Collider.GetContacts(), Has.Length.Zero);
    }

    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void ProcessPhysics_KinematicBodiesShouldNotCollide_WhenColliderComponentIsDisabled(bool collider1Enabled, bool collider2Enabled)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var rectangle1 = CreateRectangleKinematicBody(0, 0, 10, 5);
        var rectangle2 = CreateRectangleKinematicBody(5, 0, 10, 5);

        var rectangle1Collider = rectangle1.GetComponent<RectangleColliderComponent>();
        var rectangle2Collider = rectangle2.GetComponent<RectangleColliderComponent>();

        rectangle1Collider.Enabled = collider1Enabled;
        rectangle2Collider.Enabled = collider2Enabled;

        // Assume
        Assert.That(rectangle1Collider.IsColliding, Is.False);
        Assert.That(rectangle2Collider.IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangle1Collider.IsColliding, Is.False);
        Assert.That(rectangle1Collider.GetContacts(), Has.Length.Zero);

        Assert.That(rectangle2Collider.IsColliding, Is.False);
        Assert.That(rectangle2Collider.GetContacts(), Has.Length.Zero);
    }
}