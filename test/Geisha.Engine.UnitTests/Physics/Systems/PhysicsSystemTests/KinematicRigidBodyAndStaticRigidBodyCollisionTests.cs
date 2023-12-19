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

    #endregion
}