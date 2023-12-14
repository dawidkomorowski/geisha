using System.Linq;
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
        Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

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
        var rectangle2 = CreateRectangleKinematicBody(20, 0, 10, 5);

        rectangle1.GetComponent<RectangleColliderComponent>().AddCollidingEntity(rectangle2);
        rectangle2.GetComponent<RectangleColliderComponent>().AddCollidingEntity(rectangle1);

        // Assume
        Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);

        // Act
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
        Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var rectangleCollider1 = rectangle1.GetComponent<RectangleColliderComponent>();
        Assert.That(rectangleCollider1.IsColliding, Is.True);
        Assert.That(rectangleCollider1.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(rectangleCollider1.CollidingEntities.Single(), Is.EqualTo(rectangle2));

        var rectangleCollider2 = rectangle2.GetComponent<RectangleColliderComponent>();
        Assert.That(rectangleCollider2.IsColliding, Is.True);
        Assert.That(rectangleCollider2.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(rectangleCollider2.CollidingEntities.Single(), Is.EqualTo(rectangle1));
    }

    [Test]
    public void ProcessPhysics_ShouldMakeCircleKinematicBodiesColliding_WhenTheyWereNotCollidingButTheyCollideNow()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var circle1 = CreateCircleKinematicBody(0, 0, 10);
        var circle2 = CreateCircleKinematicBody(5, 0, 10);

        // Assume
        Assume.That(circle1.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assume.That(circle2.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var circleCollider1 = circle1.GetComponent<CircleColliderComponent>();
        Assert.That(circleCollider1.IsColliding, Is.True);
        Assert.That(circleCollider1.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(circleCollider1.CollidingEntities.Single(), Is.EqualTo(circle2));

        var circleCollider2 = circle2.GetComponent<CircleColliderComponent>();
        Assert.That(circleCollider2.IsColliding, Is.True);
        Assert.That(circleCollider2.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(circleCollider2.CollidingEntities.Single(), Is.EqualTo(circle1));
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
        Assume.That(circle1.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assume.That(circle2.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assume.That(circle3.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(rectangle3.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var circleCollider1 = circle1.GetComponent<CircleColliderComponent>();
        Assert.That(circleCollider1.IsColliding, Is.True);
        Assert.That(circleCollider1.CollidingEntities, Has.Count.EqualTo(2));
        Assert.That(circleCollider1.CollidingEntities, Contains.Item(circle2));
        Assert.That(circleCollider1.CollidingEntities, Contains.Item(rectangle1));

        var circleCollider2 = circle2.GetComponent<CircleColliderComponent>();
        Assert.That(circleCollider2.IsColliding, Is.True);
        Assert.That(circleCollider2.CollidingEntities, Has.Count.EqualTo(2));
        Assert.That(circleCollider2.CollidingEntities, Contains.Item(circle1));
        Assert.That(circleCollider2.CollidingEntities, Contains.Item(rectangle1));

        var circleCollider3 = circle3.GetComponent<CircleColliderComponent>();
        Assert.That(circleCollider3.IsColliding, Is.True);
        Assert.That(circleCollider3.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(circleCollider3.CollidingEntities.Single(), Is.EqualTo(rectangle2));

        var rectangleCollider1 = rectangle1.GetComponent<RectangleColliderComponent>();
        Assert.That(rectangleCollider1.IsColliding, Is.True);
        Assert.That(rectangleCollider1.CollidingEntities, Has.Count.EqualTo(2));
        Assert.That(rectangleCollider1.CollidingEntities, Contains.Item(circle1));
        Assert.That(rectangleCollider1.CollidingEntities, Contains.Item(circle2));

        var rectangleCollider2 = rectangle2.GetComponent<RectangleColliderComponent>();
        Assert.That(rectangleCollider2.IsColliding, Is.True);
        Assert.That(rectangleCollider2.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(rectangleCollider2.CollidingEntities, Contains.Item(circle3));

        var rectangleCollider3 = rectangle3.GetComponent<RectangleColliderComponent>();
        Assert.That(rectangleCollider3.IsColliding, Is.False);
        Assert.That(rectangleCollider3.CollidingEntities, Has.Count.EqualTo(0));
    }
}