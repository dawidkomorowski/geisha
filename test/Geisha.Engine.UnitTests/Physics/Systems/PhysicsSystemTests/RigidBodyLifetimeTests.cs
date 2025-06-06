using Geisha.Engine.Core.Components;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.PhysicsEngine2D;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class RigidBodyLifetimeTests : PhysicsSystemTestsBase
{
    #region Static body

    [Test]
    public void StaticBody_ShouldBeCreated_WhenEntityIsRootAndHas_Transform2DComponent_And_RectangleColliderComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Act
        var entity = Scene.CreateEntity();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
        entity.CreateComponent<Transform2DComponent>();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
        entity.CreateComponent<RectangleColliderComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.IsRectangleCollider, Is.True);
        Assert.That(body.IsCircleCollider, Is.False);
    }

    [Test]
    public void StaticBody_ShouldBeCreated_WhenEntityIsRootAndHas_Transform2DComponent_And_CircleColliderComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Act
        var entity = Scene.CreateEntity();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
        entity.CreateComponent<Transform2DComponent>();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
        entity.CreateComponent<CircleColliderComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.IsRectangleCollider, Is.False);
        Assert.That(body.IsCircleCollider, Is.True);
    }

    [Test]
    public void StaticBody_ShouldBeCreated_WhenEntityIsNotRootAndHas_Transform2DComponent_And_RectangleColliderComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Act
        var entity = Scene.CreateEntity().CreateChildEntity();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
        entity.CreateComponent<Transform2DComponent>();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
        entity.CreateComponent<RectangleColliderComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.IsRectangleCollider, Is.True);
        Assert.That(body.IsCircleCollider, Is.False);
    }

    [Test]
    public void StaticBody_ShouldBeCreated_WhenEntityIsNotRootAndHas_Transform2DComponent_And_CircleColliderComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Act
        var entity = Scene.CreateEntity().CreateChildEntity();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
        entity.CreateComponent<Transform2DComponent>();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
        entity.CreateComponent<CircleColliderComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.IsRectangleCollider, Is.False);
        Assert.That(body.IsCircleCollider, Is.True);
    }

    [Test]
    public void StaticBody_ShouldNotBeCreated_WhenEntityIsNotRoot_And_RootEntityHasKinematicRigidBody2DComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);

        // Act
        var parent = Scene.CreateEntity();
        parent.CreateComponent<KinematicRigidBody2DComponent>();
        var entity = parent.CreateChildEntity();
        entity.CreateComponent<Transform2DComponent>();
        entity.CreateComponent<RectangleColliderComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
    }

    [Test]
    public void StaticBody_ShouldBeRemoved_WhenTransform2DComponentIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleStaticBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.IsRectangleCollider, Is.True);
        Assert.That(body.IsCircleCollider, Is.False);

        // Act
        entity.RemoveComponent(entity.GetComponent<Transform2DComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
    }

    [Test]
    public void StaticBody_ShouldBeRemoved_WhenCollider2DComponentIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleStaticBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.IsRectangleCollider, Is.True);
        Assert.That(body.IsCircleCollider, Is.False);

        // Act
        entity.RemoveComponent(entity.GetComponent<RectangleColliderComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
    }

    [Test]
    public void StaticBody_ShouldBeRemoved_WhenItBecomesChildOfEntityWithKinematicRigidBody2DComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleStaticBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.IsRectangleCollider, Is.True);
        Assert.That(body.IsCircleCollider, Is.False);

        // Act
        var parent = Scene.CreateEntity();
        parent.CreateComponent<KinematicRigidBody2DComponent>();
        entity.Parent = parent;

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
    }

    [Test]
    public void StaticBody_ShouldBeRecreated_WhenItStopsToBeChildOfEntityWithKinematicRigidBody2DComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleStaticBody(0, 0, 10, 5);
        var parent = Scene.CreateEntity();
        parent.CreateComponent<KinematicRigidBody2DComponent>();
        entity.Parent = parent;

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);

        // Act
        entity.Parent = null;

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.IsRectangleCollider, Is.True);
        Assert.That(body.IsCircleCollider, Is.False);
    }

    [Test]
    public void StaticBody_ShouldBeRemoved_WhenItsParentBecomesChildOfEntityWithKinematicRigidBody2DComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleStaticBody(0, 0, 10, 5);
        var parent = Scene.CreateEntity();
        entity.Parent = parent;

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.IsRectangleCollider, Is.True);
        Assert.That(body.IsCircleCollider, Is.False);

        // Act
        var root = Scene.CreateEntity();
        root.CreateComponent<KinematicRigidBody2DComponent>();
        parent.Parent = root;

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
    }

    [Test]
    public void StaticBody_ShouldBeRecreated_WhenItsParentStopsToBeChildOfEntityWithKinematicRigidBody2DComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleStaticBody(0, 0, 10, 5);
        var parent = Scene.CreateEntity();
        entity.Parent = parent;
        var root = Scene.CreateEntity();
        root.CreateComponent<KinematicRigidBody2DComponent>();
        parent.Parent = root;

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);

        // Act
        parent.Parent = null;

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.IsRectangleCollider, Is.True);
        Assert.That(body.IsCircleCollider, Is.False);
    }

    [Test]
    public void StaticBody_ShouldBeRemoved_WhenItsParentGetsKinematicRigidBody2DComponentAdded()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleStaticBody(0, 0, 10, 5);
        var parent = Scene.CreateEntity();
        entity.Parent = parent;

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.IsRectangleCollider, Is.True);
        Assert.That(body.IsCircleCollider, Is.False);

        // Act
        parent.CreateComponent<KinematicRigidBody2DComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
    }

    [Test]
    public void StaticBody_ShouldBeRecreated_WhenItsParentGetsKinematicRigidBody2DComponentRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleStaticBody(0, 0, 10, 5);
        var parent = Scene.CreateEntity();
        parent.CreateComponent<KinematicRigidBody2DComponent>();
        entity.Parent = parent;

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);

        // Act
        parent.RemoveComponent(parent.GetComponent<KinematicRigidBody2DComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.IsRectangleCollider, Is.True);
        Assert.That(body.IsCircleCollider, Is.False);
    }

    [Test]
    public void StaticBody_ShouldClearContacts_WhenStaticBodyIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var staticBody = CreateRectangleStaticBody(0, 0, 10, 5);
        var kinematicBody = CreateRectangleKinematicBody(5, 0, 10, 5);

        var staticRectangleCollider = staticBody.GetComponent<RectangleColliderComponent>();
        var kinematicRectangleCollider = kinematicBody.GetComponent<RectangleColliderComponent>();

        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(staticRectangleCollider.IsColliding, Is.True);
        Assert.That(kinematicRectangleCollider.IsColliding, Is.True);

        // Act
        staticBody.RemoveComponent(staticBody.GetComponent<Transform2DComponent>());
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(staticRectangleCollider.IsColliding, Is.False);
        Assert.That(staticRectangleCollider.Contacts, Has.Count.Zero);

        Assert.That(kinematicRectangleCollider.IsColliding, Is.False);
        Assert.That(kinematicRectangleCollider.Contacts, Has.Count.Zero);
    }

    #endregion

    #region Kinematic body

    [Test]
    public void KinematicBody_ShouldBeCreated_WhenEntityIsRootAndHas_Transform2DComponent_And_RectangleColliderComponent_And_KinematicRigidBody2DComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);

        // Act
        var entity = Scene.CreateEntity();
        entity.CreateComponent<Transform2DComponent>();
        entity.CreateComponent<RectangleColliderComponent>();
        entity.CreateComponent<KinematicRigidBody2DComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.IsRectangleCollider, Is.True);
        Assert.That(body.IsCircleCollider, Is.False);
    }

    [Test]
    public void KinematicBody_ShouldBeCreated_WhenEntityIsRootAndHas_Transform2DComponent_And_CircleColliderComponent_And_KinematicRigidBody2DComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);

        // Act
        var entity = Scene.CreateEntity();
        entity.CreateComponent<Transform2DComponent>();
        entity.CreateComponent<CircleColliderComponent>();
        entity.CreateComponent<KinematicRigidBody2DComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.IsRectangleCollider, Is.False);
        Assert.That(body.IsCircleCollider, Is.True);
    }

    [Test]
    public void KinematicBody_ShouldNotBeCreated_WhenEntityIsNotRoot()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);

        // Act
        var entity = Scene.CreateEntity().CreateChildEntity();
        entity.CreateComponent<Transform2DComponent>();
        entity.CreateComponent<RectangleColliderComponent>();
        entity.CreateComponent<KinematicRigidBody2DComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
    }

    [Test]
    public void KinematicBody_ShouldBeRemoved_WhenTransform2DComponentIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleKinematicBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.IsRectangleCollider, Is.True);
        Assert.That(body.IsCircleCollider, Is.False);

        // Act
        entity.RemoveComponent(entity.GetComponent<Transform2DComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
    }

    [Test]
    public void KinematicBody_ShouldBeRemoved_WhenCollider2DComponentIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleKinematicBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.IsRectangleCollider, Is.True);
        Assert.That(body.IsCircleCollider, Is.False);

        // Act
        entity.RemoveComponent(entity.GetComponent<RectangleColliderComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
    }

    [Test]
    public void KinematicBody_ShouldBeRemoved_WhenEntityStopsToBeRootEntity()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleKinematicBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.IsRectangleCollider, Is.True);
        Assert.That(body.IsCircleCollider, Is.False);

        // Act
        var parent = Scene.CreateEntity();
        entity.Parent = parent;

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);
    }

    [Test]
    public void KinematicBody_ShouldBeRecreated_WhenEntityBecomesRootEntity()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleKinematicBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var bodyBefore = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(bodyBefore.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(bodyBefore.IsRectangleCollider, Is.True);
        Assert.That(bodyBefore.IsCircleCollider, Is.False);

        var parent = Scene.CreateEntity();
        entity.Parent = parent;

        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.Zero);

        // Act
        entity.Parent = null;

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var bodyAfter = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(bodyAfter.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(bodyAfter.IsRectangleCollider, Is.True);
        Assert.That(bodyAfter.IsCircleCollider, Is.False);
    }

    [Test]
    public void KinematicBody_ShouldBecomeStaticBody_WhenKinematicRigidBody2DComponentIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleKinematicBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var bodyBefore = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(bodyBefore.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(bodyBefore.IsRectangleCollider, Is.True);
        Assert.That(bodyBefore.IsCircleCollider, Is.False);

        // Act
        entity.RemoveComponent(entity.GetComponent<KinematicRigidBody2DComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies, Has.Count.EqualTo(1));
        var bodyAfter = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(bodyAfter.Type, Is.EqualTo(BodyType.Static));
        Assert.That(bodyAfter.IsRectangleCollider, Is.True);
        Assert.That(bodyAfter.IsCircleCollider, Is.False);
    }

    [Test]
    public void KinematicBody_ShouldClearContacts_WhenKinematicBodyIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody1 = CreateRectangleKinematicBody(0, 0, 10, 5);
        var kinematicBody2 = CreateRectangleKinematicBody(5, 0, 10, 5);

        var rectangleCollider1 = kinematicBody1.GetComponent<RectangleColliderComponent>();
        var rectangleCollider2 = kinematicBody2.GetComponent<RectangleColliderComponent>();

        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(rectangleCollider1.IsColliding, Is.True);
        Assert.That(rectangleCollider2.IsColliding, Is.True);

        // Act
        kinematicBody1.RemoveComponent(kinematicBody1.GetComponent<Transform2DComponent>());
        physicsSystem.ProcessPhysics();

        // Assert
        Assert.That(rectangleCollider1.IsColliding, Is.False);
        Assert.That(rectangleCollider1.Contacts, Has.Count.Zero);

        Assert.That(rectangleCollider2.IsColliding, Is.False);
        Assert.That(rectangleCollider2.Contacts, Has.Count.Zero);
    }

    #endregion
}