using System.Diagnostics;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class KinematicRigidBodyAndStaticRigidBodyCollisionTests : PhysicsSystemTestsBase
{
    #region BasicCollisionsTests

    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldCollideWithRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 5);
        var staticBody = CreateRectangleStaticBody(5, 0, 10, 5);

        // Assume
        Assume.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        Assert.That(kinematicBodyCollider.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(kinematicBodyCollider.CollidingEntities.Single(), Is.EqualTo(staticBody));

        var staticBodyCollider = staticBody.GetComponent<RectangleColliderComponent>();
        Assert.That(staticBodyCollider.IsColliding, Is.True);
        Assert.That(staticBodyCollider.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(staticBodyCollider.CollidingEntities.Single(), Is.EqualTo(kinematicBody));
    }

    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldCollideWithCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 5);
        var staticBody = CreateCircleStaticBody(5, 0, 10);

        // Assume
        Assume.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(staticBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        Assert.That(kinematicBodyCollider.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(kinematicBodyCollider.CollidingEntities.Single(), Is.EqualTo(staticBody));

        var staticBodyCollider = staticBody.GetComponent<CircleColliderComponent>();
        Assert.That(staticBodyCollider.IsColliding, Is.True);
        Assert.That(staticBodyCollider.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(staticBodyCollider.CollidingEntities.Single(), Is.EqualTo(kinematicBody));
    }

    [Test]
    public void ProcessPhysics_CircleKinematicBodyShouldCollideWithRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(0, 0, 10);
        var staticBody = CreateRectangleStaticBody(5, 0, 10, 5);

        // Assume
        Assume.That(kinematicBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assume.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        Assert.That(kinematicBodyCollider.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(kinematicBodyCollider.CollidingEntities.Single(), Is.EqualTo(staticBody));

        var staticBodyCollider = staticBody.GetComponent<RectangleColliderComponent>();
        Assert.That(staticBodyCollider.IsColliding, Is.True);
        Assert.That(staticBodyCollider.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(staticBodyCollider.CollidingEntities.Single(), Is.EqualTo(kinematicBody));
    }

    [Test]
    public void ProcessPhysics_CircleKinematicBodyShouldCollideWithCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(0, 0, 10);
        var staticBody = CreateCircleStaticBody(5, 0, 10);

        // Assume
        Assume.That(kinematicBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assume.That(staticBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        Assert.That(kinematicBodyCollider.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(kinematicBodyCollider.CollidingEntities.Single(), Is.EqualTo(staticBody));

        var staticBodyCollider = staticBody.GetComponent<CircleColliderComponent>();
        Assert.That(staticBodyCollider.IsColliding, Is.True);
        Assert.That(staticBodyCollider.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(staticBodyCollider.CollidingEntities.Single(), Is.EqualTo(kinematicBody));
    }

    #endregion

    #region CollisionShouldNotOccur

    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldNotCollideWithRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 5);
        var staticBody = CreateRectangleStaticBody(11, 0, 10, 5);

        // Assume
        Assume.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.False);
        Assert.That(kinematicBodyCollider.CollidingEntities, Has.Count.Zero);

        var staticBodyCollider = staticBody.GetComponent<RectangleColliderComponent>();
        Assert.That(staticBodyCollider.IsColliding, Is.False);
        Assert.That(staticBodyCollider.CollidingEntities, Has.Count.Zero);
    }

    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldNotCollideWithCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 5);
        var staticBody = CreateCircleStaticBody(16, 0, 10);

        // Assume
        Assume.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(staticBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.False);
        Assert.That(kinematicBodyCollider.CollidingEntities, Has.Count.Zero);

        var staticBodyCollider = staticBody.GetComponent<CircleColliderComponent>();
        Assert.That(staticBodyCollider.IsColliding, Is.False);
        Assert.That(staticBodyCollider.CollidingEntities, Has.Count.Zero);
    }

    [Test]
    public void ProcessPhysics_CircleKinematicBodyShouldNotCollideWithRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(0, 0, 10);
        var staticBody = CreateRectangleStaticBody(16, 0, 10, 5);

        // Assume
        Assume.That(kinematicBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assume.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.False);
        Assert.That(kinematicBodyCollider.CollidingEntities, Has.Count.Zero);

        var staticBodyCollider = staticBody.GetComponent<RectangleColliderComponent>();
        Assert.That(staticBodyCollider.IsColliding, Is.False);
        Assert.That(staticBodyCollider.CollidingEntities, Has.Count.Zero);
    }

    [Test]
    public void ProcessPhysics_CircleKinematicBodyShouldNotCollideWithCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(0, 0, 10);
        var staticBody = CreateCircleStaticBody(21, 0, 10);

        // Assume
        Assume.That(kinematicBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assume.That(staticBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.False);
        Assert.That(kinematicBodyCollider.CollidingEntities, Has.Count.Zero);

        var staticBodyCollider = staticBody.GetComponent<CircleColliderComponent>();
        Assert.That(staticBodyCollider.IsColliding, Is.False);
        Assert.That(staticBodyCollider.CollidingEntities, Has.Count.Zero);
    }

    #endregion

    #region StaticBodyHierarchy

    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldCollideWithRectangleStaticBodyThatHasParentTransformDueToParentTransform()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 5);
        var staticBody = CreateRectangleStaticBodyWithParentTransform(0, 0, -20, -12.5, 10, 5);

        physicsSystem.ProcessPhysics();

        // Assume
        Assume.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        Debug.Assert(staticBody.Parent != null, "staticBody.Parent != null");
        staticBody.Parent.GetComponent<Transform2DComponent>().Translation = new Vector2(15, 10);

        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        Assert.That(kinematicBodyCollider.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(kinematicBodyCollider.CollidingEntities.Single(), Is.EqualTo(staticBody));

        var staticBodyCollider = staticBody.GetComponent<RectangleColliderComponent>();
        Assert.That(staticBodyCollider.IsColliding, Is.True);
        Assert.That(staticBodyCollider.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(staticBodyCollider.CollidingEntities.Single(), Is.EqualTo(kinematicBody));
    }

    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldNotCollideWithRectangleStaticBodyThatHasParentTransformDueToParentTransform()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 5);
        var staticBody = CreateRectangleStaticBodyWithParentTransform(0, 0, 5, 2.5, 10, 5);

        physicsSystem.ProcessPhysics();

        // Assume
        Assume.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assume.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);

        // Act
        Debug.Assert(staticBody.Parent != null, "staticBody.Parent != null");
        staticBody.Parent.GetComponent<Transform2DComponent>().Translation = new Vector2(10, 10);

        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
    }

    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldCollideWithRectangleStaticBodyAndItsChildRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 6);

        var parentStaticBody = CreateRectangleStaticBody(20, 20, 10, 6);
        var childStaticBody = CreateRectangleStaticBody(0, -6, 10, 6);
        childStaticBody.Parent = parentStaticBody;

        physicsSystem.ProcessPhysics();

        // Assume
        Assume.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(parentStaticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(childStaticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        parentStaticBody.GetComponent<Transform2DComponent>().Translation = new Vector2(9, 5);

        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        Assert.That(kinematicBodyCollider.CollidingEntities, Has.Count.EqualTo(2));
        Assert.That(kinematicBodyCollider.CollidingEntities, Contains.Item(parentStaticBody).And.Contains(childStaticBody));

        var parentStaticBodyCollider = parentStaticBody.GetComponent<RectangleColliderComponent>();
        Assert.That(parentStaticBodyCollider.IsColliding, Is.True);
        Assert.That(parentStaticBodyCollider.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(parentStaticBodyCollider.CollidingEntities.Single(), Is.EqualTo(kinematicBody));

        var childStaticBodyCollider = childStaticBody.GetComponent<RectangleColliderComponent>();
        Assert.That(childStaticBodyCollider.IsColliding, Is.True);
        Assert.That(childStaticBodyCollider.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(childStaticBodyCollider.CollidingEntities.Single(), Is.EqualTo(kinematicBody));
    }

    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldCollideWithRectangleStaticBodyAndItsSiblingRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 6);

        var parent = Scene.CreateEntity();
        parent.CreateComponent<Transform2DComponent>().Translation = new Vector2(20, 20);

        var childStaticBody1 = CreateRectangleStaticBody(-5, 3, 10, 6);
        var childStaticBody2 = CreateRectangleStaticBody(5, -3, 10, 6);
        childStaticBody1.Parent = parent;
        childStaticBody2.Parent = parent;

        physicsSystem.ProcessPhysics();

        // Assume
        Assume.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(childStaticBody1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assume.That(childStaticBody2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        parent.GetComponent<Transform2DComponent>().Translation = new Vector2(0, 0);

        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        Assert.That(kinematicBodyCollider.CollidingEntities, Has.Count.EqualTo(2));
        Assert.That(kinematicBodyCollider.CollidingEntities, Contains.Item(childStaticBody1).And.Contains(childStaticBody2));

        var childStaticBodyCollider1 = childStaticBody1.GetComponent<RectangleColliderComponent>();
        Assert.That(childStaticBodyCollider1.IsColliding, Is.True);
        Assert.That(childStaticBodyCollider1.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(childStaticBodyCollider1.CollidingEntities.Single(), Is.EqualTo(kinematicBody));

        var childStaticBodyCollider2 = childStaticBody2.GetComponent<RectangleColliderComponent>();
        Assert.That(childStaticBodyCollider2.IsColliding, Is.True);
        Assert.That(childStaticBodyCollider2.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(childStaticBodyCollider2.CollidingEntities.Single(), Is.EqualTo(kinematicBody));
    }

    #endregion
}