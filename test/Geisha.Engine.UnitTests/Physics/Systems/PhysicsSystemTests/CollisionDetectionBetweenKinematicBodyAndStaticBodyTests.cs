using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using NUnit.Framework;
using System.Diagnostics;
using System.Linq;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class CollisionDetectionBetweenKinematicBodyAndStaticBodyTests : PhysicsSystemTestsBase
{
    #region Basic collision

    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldCollideWithRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 5);
        var staticBody = CreateRectangleStaticBody(5, 0, 10, 5);

        // Assume
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        var staticBodyCollider = staticBody.GetComponent<RectangleColliderComponent>();

        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        var kinematicBodyContacts = kinematicBodyCollider.GetContacts();
        Assert.That(kinematicBodyContacts, Has.Length.EqualTo(1));
        Assert.That(kinematicBodyContacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        var staticBodyContacts = staticBodyCollider.GetContacts();
        Assert.That(staticBodyContacts, Has.Length.EqualTo(1));
        Assert.That(staticBodyContacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyContacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
    }

    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldCollideWithCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 5);
        var staticBody = CreateCircleStaticBody(5, 0, 10);

        // Assume
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        var staticBodyCollider = staticBody.GetComponent<CircleColliderComponent>();

        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        var kinematicBodyContacts = kinematicBodyCollider.GetContacts();
        Assert.That(kinematicBodyContacts, Has.Length.EqualTo(1));
        Assert.That(kinematicBodyContacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        var staticBodyContacts = staticBodyCollider.GetContacts();
        Assert.That(staticBodyContacts, Has.Length.EqualTo(1));
        Assert.That(staticBodyContacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyContacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
    }

    [Test]
    public void ProcessPhysics_CircleKinematicBodyShouldCollideWithRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(0, 0, 10);
        var staticBody = CreateRectangleStaticBody(5, 0, 10, 5);

        // Assume
        Assert.That(kinematicBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        var staticBodyCollider = staticBody.GetComponent<RectangleColliderComponent>();

        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        var kinematicBodyContacts = kinematicBodyCollider.GetContacts();
        Assert.That(kinematicBodyContacts, Has.Length.EqualTo(1));
        Assert.That(kinematicBodyContacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        var staticBodyContacts = staticBodyCollider.GetContacts();
        Assert.That(staticBodyContacts, Has.Length.EqualTo(1));
        Assert.That(staticBodyContacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyContacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
    }

    [Test]
    public void ProcessPhysics_CircleKinematicBodyShouldCollideWithCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(0, 0, 10);
        var staticBody = CreateCircleStaticBody(5, 0, 10);

        // Assume
        Assert.That(kinematicBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        var staticBodyCollider = staticBody.GetComponent<CircleColliderComponent>();

        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        var kinematicBodyContacts = kinematicBodyCollider.GetContacts();
        Assert.That(kinematicBodyContacts, Has.Length.EqualTo(1));
        Assert.That(kinematicBodyContacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        var staticBodyContacts = staticBodyCollider.GetContacts();
        Assert.That(staticBodyContacts, Has.Length.EqualTo(1));
        Assert.That(staticBodyContacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyContacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
    }

    #endregion

    #region Collision should not occur

    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldNotCollideWithRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 5);
        var staticBody = CreateRectangleStaticBody(11, 0, 10, 5);

        // Assume
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.False);
        Assert.That(kinematicBodyCollider.GetContacts(), Has.Length.Zero);

        var staticBodyCollider = staticBody.GetComponent<RectangleColliderComponent>();
        Assert.That(staticBodyCollider.IsColliding, Is.False);
        Assert.That(staticBodyCollider.GetContacts(), Has.Length.Zero);
    }

    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldNotCollideWithCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 5);
        var staticBody = CreateCircleStaticBody(16, 0, 10);

        // Assume
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.False);
        Assert.That(kinematicBodyCollider.GetContacts(), Has.Length.Zero);

        var staticBodyCollider = staticBody.GetComponent<CircleColliderComponent>();
        Assert.That(staticBodyCollider.IsColliding, Is.False);
        Assert.That(staticBodyCollider.GetContacts(), Has.Length.Zero);
    }

    [Test]
    public void ProcessPhysics_CircleKinematicBodyShouldNotCollideWithRectangleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(0, 0, 10);
        var staticBody = CreateRectangleStaticBody(16, 0, 10, 5);

        // Assume
        Assert.That(kinematicBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.False);
        Assert.That(kinematicBodyCollider.GetContacts(), Has.Length.Zero);

        var staticBodyCollider = staticBody.GetComponent<RectangleColliderComponent>();
        Assert.That(staticBodyCollider.IsColliding, Is.False);
        Assert.That(staticBodyCollider.GetContacts(), Has.Length.Zero);
    }

    [Test]
    public void ProcessPhysics_CircleKinematicBodyShouldNotCollideWithCircleStaticBody()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateCircleKinematicBody(0, 0, 10);
        var staticBody = CreateCircleStaticBody(21, 0, 10);

        // Assume
        Assert.That(kinematicBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

        // Act
        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<CircleColliderComponent>();
        Assert.That(kinematicBodyCollider.IsColliding, Is.False);
        Assert.That(kinematicBodyCollider.GetContacts(), Has.Length.Zero);

        var staticBodyCollider = staticBody.GetComponent<CircleColliderComponent>();
        Assert.That(staticBodyCollider.IsColliding, Is.False);
        Assert.That(staticBodyCollider.GetContacts(), Has.Length.Zero);
    }

    #endregion

    #region Static body hierarchy

    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldCollideWithRectangleStaticBodyThatHasParentTransformDueToParentTransform()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 5);

        var parent = Scene.CreateEntity();
        var transform2DComponent = parent.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(0, 0);
        transform2DComponent.Rotation = 0;
        transform2DComponent.Scale = Vector2.One;

        var staticBody = CreateRectangleStaticBody(-20, -12.5, 10, 5);
        staticBody.Parent = parent;

        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        Debug.Assert(staticBody.Parent != null, "staticBody.Parent != null");
        staticBody.Parent.GetComponent<Transform2DComponent>().Translation = new Vector2(15, 10);

        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        var staticBodyCollider = staticBody.GetComponent<RectangleColliderComponent>();

        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        var kinematicBodyContacts = kinematicBodyCollider.GetContacts();
        Assert.That(kinematicBodyContacts, Has.Length.EqualTo(1));
        Assert.That(kinematicBodyContacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts[0].OtherCollider, Is.EqualTo(staticBodyCollider));

        Assert.That(staticBodyCollider.IsColliding, Is.True);
        var staticBodyContacts = staticBodyCollider.GetContacts();
        Assert.That(staticBodyContacts, Has.Length.EqualTo(1));
        Assert.That(staticBodyContacts[0].ThisCollider, Is.EqualTo(staticBodyCollider));
        Assert.That(staticBodyContacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
    }

    [Test]
    public void ProcessPhysics_RectangleKinematicBodyShouldNotCollideWithRectangleStaticBodyThatHasParentTransformDueToParentTransform()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 10, 5);

        var parent = Scene.CreateEntity();

        var transform2DComponent = parent.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(0, 0);
        transform2DComponent.Rotation = 0;
        transform2DComponent.Scale = Vector2.One;

        var staticBody = CreateRectangleStaticBody(5, 2.5, 10, 5);
        staticBody.Parent = parent;

        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
        Assert.That(staticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);

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
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(parentStaticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(childStaticBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        parentStaticBody.GetComponent<Transform2DComponent>().Translation = new Vector2(9, 5);

        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        var parentStaticBodyCollider = parentStaticBody.GetComponent<RectangleColliderComponent>();
        var childStaticBodyCollider = childStaticBody.GetComponent<RectangleColliderComponent>();

        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        var kinematicBodyContacts = kinematicBodyCollider.GetContacts();
        Assert.That(kinematicBodyContacts, Has.Length.EqualTo(2));
        Assert.That(kinematicBodyContacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts[1].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts.Any(c => c.OtherCollider == parentStaticBodyCollider), Is.True);
        Assert.That(kinematicBodyContacts.Any(c => c.OtherCollider == childStaticBodyCollider), Is.True);

        Assert.That(parentStaticBodyCollider.IsColliding, Is.True);
        var parentStaticBodyContacts = parentStaticBodyCollider.GetContacts();
        Assert.That(parentStaticBodyContacts, Has.Length.EqualTo(1));
        Assert.That(parentStaticBodyContacts[0].ThisCollider, Is.EqualTo(parentStaticBodyCollider));
        Assert.That(parentStaticBodyContacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));

        Assert.That(childStaticBodyCollider.IsColliding, Is.True);
        var childStaticBodyContacts = childStaticBodyCollider.GetContacts();
        Assert.That(childStaticBodyContacts, Has.Length.EqualTo(1));
        Assert.That(childStaticBodyContacts[0].ThisCollider, Is.EqualTo(childStaticBodyCollider));
        Assert.That(childStaticBodyContacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
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
        Assert.That(kinematicBody.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(childStaticBody1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        Assert.That(childStaticBody2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

        // Act
        parent.GetComponent<Transform2DComponent>().Translation = new Vector2(0, 0);

        physicsSystem.ProcessPhysics();

        // Assert
        var kinematicBodyCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        var childStaticBodyCollider1 = childStaticBody1.GetComponent<RectangleColliderComponent>();
        var childStaticBodyCollider2 = childStaticBody2.GetComponent<RectangleColliderComponent>();

        Assert.That(kinematicBodyCollider.IsColliding, Is.True);
        var kinematicBodyContacts = kinematicBodyCollider.GetContacts();
        Assert.That(kinematicBodyContacts, Has.Length.EqualTo(2));
        Assert.That(kinematicBodyContacts[0].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts[1].ThisCollider, Is.EqualTo(kinematicBodyCollider));
        Assert.That(kinematicBodyContacts.Any(c => c.OtherCollider == childStaticBodyCollider1));
        Assert.That(kinematicBodyContacts.Any(c => c.OtherCollider == childStaticBodyCollider2));

        Assert.That(childStaticBodyCollider1.IsColliding, Is.True);
        var childStaticBody1Contacts = childStaticBodyCollider1.GetContacts();
        Assert.That(childStaticBody1Contacts, Has.Length.EqualTo(1));
        Assert.That(childStaticBody1Contacts[0].ThisCollider, Is.EqualTo(childStaticBodyCollider1));
        Assert.That(childStaticBody1Contacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));

        Assert.That(childStaticBodyCollider2.IsColliding, Is.True);
        var childStaticBody2Contacts = childStaticBodyCollider2.GetContacts();
        Assert.That(childStaticBody2Contacts, Has.Length.EqualTo(1));
        Assert.That(childStaticBody2Contacts[0].ThisCollider, Is.EqualTo(childStaticBodyCollider2));
        Assert.That(childStaticBody2Contacts[0].OtherCollider, Is.EqualTo(kinematicBodyCollider));
    }

    #endregion
}