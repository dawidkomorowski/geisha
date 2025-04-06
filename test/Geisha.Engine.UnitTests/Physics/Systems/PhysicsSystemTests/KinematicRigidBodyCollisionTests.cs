using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class KinematicRigidBodyCollisionTests : PhysicsSystemTestsBase
{
    [Test]
    public void ProcessPhysics_ShouldLeaveKinematicBodiesNotColliding_WhenTheyWereNotCollidingAndTheyStillNotCollide()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var rectangle1 = CreateRectangleKinematicBody(0, 0, 10, 5);
        var rectangle2 = CreateRectangleKinematicBody(20, 0, 10, 5);

        // Assume
        Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
    }

    [Test]
    public void ProcessPhysics_ShouldMakeKinematicBodiesNotColliding_WhenTheyWereCollidingButTheyNotCollideAnymore()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var rectangle1 = CreateRectangleKinematicBody(0, 0, 10, 5);
        var rectangle2 = CreateRectangleKinematicBody(0, 0, 10, 5);

        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);

        // Act
        rectangle2.GetComponent<Transform2DComponent>().Translation = new Vector2(20, 0);
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
    }

    [Test]
    public void ProcessPhysics_ShouldMakeKinematicBodiesColliding_WhenTheyWereNotCollidingButTheyCollideNow()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var rectangle1 = CreateRectangleKinematicBody(0, 0, 10, 5);
        var rectangle2 = CreateRectangleKinematicBody(5, 0, 10, 5);

        // Assume
        Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var rectangleCollider1 = rectangle1.GetComponent<RectangleColliderComponent>();
        var rectangleCollider2 = rectangle2.GetComponent<RectangleColliderComponent>();

        Assert.That(rectangleCollider1.IsColliding, Is.True);
        Assert.That(rectangleCollider1.Contacts, Has.Count.EqualTo(1));
        Assert.That(rectangleCollider1.Contacts[0].ThisCollider, Is.EqualTo(rectangleCollider1));
        Assert.That(rectangleCollider1.Contacts[0].OtherCollider, Is.EqualTo(rectangleCollider2));

        Assert.That(rectangleCollider2.IsColliding, Is.True);
        Assert.That(rectangleCollider2.Contacts, Has.Count.EqualTo(1));
        Assert.That(rectangleCollider2.Contacts[0].ThisCollider, Is.EqualTo(rectangleCollider2));
        Assert.That(rectangleCollider2.Contacts[0].OtherCollider, Is.EqualTo(rectangleCollider1));
    }

    [Test]
    public void ProcessPhysics_ShouldMakeCircleKinematicBodiesColliding_WhenTheyWereNotCollidingButTheyCollideNow()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var circle1 = CreateCircleKinematicBody(0, 0, 10);
        var circle2 = CreateCircleKinematicBody(5, 0, 10);

        // Assume
        Assert.That(circle1.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assert.That(circle2.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var circleCollider1 = circle1.GetComponent<CircleColliderComponent>();
        var circleCollider2 = circle2.GetComponent<CircleColliderComponent>();

        Assert.That(circleCollider1.IsColliding, Is.True);
        Assert.That(circleCollider1.Contacts, Has.Count.EqualTo(1));
        Assert.That(circleCollider1.Contacts[0].ThisCollider, Is.EqualTo(circleCollider1));
        Assert.That(circleCollider1.Contacts[0].OtherCollider, Is.EqualTo(circleCollider2));

        Assert.That(circleCollider2.IsColliding, Is.True);
        Assert.That(circleCollider2.Contacts, Has.Count.EqualTo(1));
        Assert.That(circleCollider2.Contacts[0].ThisCollider, Is.EqualTo(circleCollider2));
        Assert.That(circleCollider2.Contacts[0].OtherCollider, Is.EqualTo(circleCollider1));
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

        // Assume
        Assert.That(circle1.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assert.That(circle2.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assert.That(circle3.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(rectangle3.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var circleCollider1 = circle1.GetComponent<CircleColliderComponent>();
        var circleCollider2 = circle2.GetComponent<CircleColliderComponent>();
        var circleCollider3 = circle3.GetComponent<CircleColliderComponent>();
        var rectangleCollider1 = rectangle1.GetComponent<RectangleColliderComponent>();
        var rectangleCollider2 = rectangle2.GetComponent<RectangleColliderComponent>();
        var rectangleCollider3 = rectangle3.GetComponent<RectangleColliderComponent>();

        Assert.That(circleCollider1.IsColliding, Is.True);
        Assert.That(circleCollider1.Contacts, Has.Count.EqualTo(2));
        Assert.That(circleCollider1.Contacts[0].ThisCollider, Is.EqualTo(circleCollider1));
        Assert.That(circleCollider1.Contacts[1].ThisCollider, Is.EqualTo(circleCollider1));
        Assert.That(circleCollider1.Contacts.Any(c => c.OtherCollider == circleCollider2), Is.True);
        Assert.That(circleCollider1.Contacts.Any(c => c.OtherCollider == rectangleCollider1), Is.True);

        Assert.That(circleCollider2.IsColliding, Is.True);
        Assert.That(circleCollider2.Contacts, Has.Count.EqualTo(2));
        Assert.That(circleCollider2.Contacts[0].ThisCollider, Is.EqualTo(circleCollider2));
        Assert.That(circleCollider2.Contacts[1].ThisCollider, Is.EqualTo(circleCollider2));
        Assert.That(circleCollider2.Contacts.Any(c => c.OtherCollider == circleCollider1), Is.True);
        Assert.That(circleCollider2.Contacts.Any(c => c.OtherCollider == rectangleCollider1), Is.True);

        Assert.That(circleCollider3.IsColliding, Is.True);
        Assert.That(circleCollider3.Contacts, Has.Count.EqualTo(1));
        Assert.That(circleCollider3.Contacts[0].ThisCollider, Is.EqualTo(circleCollider3));
        Assert.That(circleCollider3.Contacts[0].OtherCollider, Is.EqualTo(rectangleCollider2));

        Assert.That(rectangleCollider1.IsColliding, Is.True);
        Assert.That(rectangleCollider1.Contacts, Has.Count.EqualTo(2));
        Assert.That(rectangleCollider1.Contacts[0].ThisCollider, Is.EqualTo(rectangleCollider1));
        Assert.That(rectangleCollider1.Contacts[1].ThisCollider, Is.EqualTo(rectangleCollider1));
        Assert.That(rectangleCollider1.Contacts.Any(c => c.OtherCollider == circleCollider1), Is.True);
        Assert.That(rectangleCollider1.Contacts.Any(c => c.OtherCollider == circleCollider2), Is.True);

        Assert.That(rectangleCollider2.IsColliding, Is.True);
        Assert.That(rectangleCollider2.Contacts, Has.Count.EqualTo(1));
        Assert.That(rectangleCollider2.Contacts[0].ThisCollider, Is.EqualTo(rectangleCollider2));
        Assert.That(rectangleCollider2.Contacts[0].OtherCollider, Is.EqualTo(circleCollider3));

        Assert.That(rectangleCollider3.IsColliding, Is.False);
        Assert.That(rectangleCollider3.Contacts, Has.Count.EqualTo(0));
    }
}