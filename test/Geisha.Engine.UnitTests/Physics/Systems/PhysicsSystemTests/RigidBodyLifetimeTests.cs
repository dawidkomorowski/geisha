using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics;
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
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
        entity.CreateComponent<Transform2DComponent>();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
        entity.CreateComponent<RectangleColliderComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));
    }

    [Test]
    public void StaticBody_ShouldBeCreated_WhenEntityIsRootAndHas_Transform2DComponent_And_CircleColliderComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Act
        var entity = Scene.CreateEntity();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
        entity.CreateComponent<Transform2DComponent>();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
        entity.CreateComponent<CircleColliderComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Circle));
    }

    [Test]
    public void StaticBody_ShouldBeCreated_WhenEntityIsRootAndHas_Transform2DComponent_And_TileColliderComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Act
        var entity = Scene.CreateEntity();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
        entity.CreateComponent<Transform2DComponent>();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
        entity.CreateComponent<TileColliderComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Tile));
    }

    [Test]
    public void StaticBody_ShouldBeCreated_WhenEntityIsNotRootAndHas_Transform2DComponent_And_RectangleColliderComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Act
        var entity = Scene.CreateEntity().CreateChildEntity();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
        entity.CreateComponent<Transform2DComponent>();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
        entity.CreateComponent<RectangleColliderComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));
    }

    [Test]
    public void StaticBody_ShouldBeCreated_WhenEntityIsNotRootAndHas_Transform2DComponent_And_CircleColliderComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Act
        var entity = Scene.CreateEntity().CreateChildEntity();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
        entity.CreateComponent<Transform2DComponent>();
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
        entity.CreateComponent<CircleColliderComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Circle));
    }

    [Test]
    public void StaticBody_ShouldNotBeCreated_WhenEntityIsNotRoot_And_RootEntityHasKinematicRigidBody2DComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);

        // Act
        var parent = Scene.CreateEntity();
        parent.CreateComponent<KinematicRigidBody2DComponent>();
        var entity = parent.CreateChildEntity();
        entity.CreateComponent<Transform2DComponent>();
        entity.CreateComponent<RectangleColliderComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
    }

    [Test]
    public void StaticBody_ShouldNotBeCreated_WhenEntityIsNotRootAndHas_Transform2DComponent_And_TileColliderComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);

        // Act
        var parent = Scene.CreateEntity();
        var entity = parent.CreateChildEntity();
        entity.CreateComponent<Transform2DComponent>();
        entity.CreateComponent<TileColliderComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
    }

    [Test]
    public void StaticBody_ShouldBeRemoved_WhenTransform2DComponentIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleStaticBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));

        // Act
        entity.RemoveComponent(entity.GetComponent<Transform2DComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
    }

    [Test]
    public void StaticBody_ShouldBeRemoved_WhenRectangleColliderComponentIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleStaticBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));

        // Act
        entity.RemoveComponent(entity.GetComponent<RectangleColliderComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
    }

    [Test]
    public void StaticBody_ShouldBeRemoved_WhenCircleColliderComponentIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateCircleStaticBody(0, 0, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Circle));

        // Act
        entity.RemoveComponent(entity.GetComponent<CircleColliderComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
    }

    [Test]
    public void StaticBody_ShouldBeRemoved_WhenTileColliderComponentIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = Scene.CreateEntity();
        entity.CreateComponent<Transform2DComponent>();
        entity.CreateComponent<TileColliderComponent>();

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Tile));

        // Act
        entity.RemoveComponent(entity.GetComponent<TileColliderComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
    }

    [Test]
    public void StaticBody_ShouldBeRemoved_WhenItBecomesChildOfEntityWithKinematicRigidBody2DComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleStaticBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));

        // Act
        var parent = Scene.CreateEntity();
        parent.CreateComponent<KinematicRigidBody2DComponent>();
        entity.Parent = parent;

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
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
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);

        // Act
        entity.Parent = null;

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));
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
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));

        // Act
        var root = Scene.CreateEntity();
        root.CreateComponent<KinematicRigidBody2DComponent>();
        parent.Parent = root;

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
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
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);

        // Act
        parent.Parent = null;

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));
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
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));

        // Act
        parent.CreateComponent<KinematicRigidBody2DComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
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
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);

        // Act
        parent.RemoveComponent(parent.GetComponent<KinematicRigidBody2DComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));
    }

    [Test]
    public void StaticBody_ShouldBeRemoved_WhenItHasTileColliderAndEntityStopsToBeRootEntity()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateTileStaticBody(0, 0);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Tile));

        // Act
        var parent = Scene.CreateEntity();
        entity.Parent = parent;

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
    }

    [Test]
    public void StaticBody_ShouldBeRecreated_WhenItHasTileColliderAndEntityBecomesRootEntity()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateTileStaticBody(0, 0);
        var parent = Scene.CreateEntity();
        entity.Parent = parent;

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);

        // Act
        entity.Parent = null;

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Static));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Tile));
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
        var staticRectangleContacts = new List<Contact2D>();
        Assert.That(staticRectangleCollider.IsColliding, Is.False);
        Assert.That(staticRectangleCollider.GetContacts(staticRectangleContacts), Is.Zero);
        Assert.That(staticRectangleContacts, Has.Count.Zero);

        var kinematicRectangleContacts = new List<Contact2D>();
        Assert.That(kinematicRectangleCollider.IsColliding, Is.False);
        Assert.That(kinematicRectangleCollider.GetContacts(kinematicRectangleContacts), Is.Zero);
        Assert.That(kinematicRectangleContacts, Has.Count.Zero);
    }

    #endregion

    #region Kinematic body

    [Test]
    public void KinematicBody_ShouldBeCreated_WhenEntityIsRootAndHas_Transform2DComponent_And_RectangleColliderComponent_And_KinematicRigidBody2DComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);

        // Act
        var entity = Scene.CreateEntity();
        entity.CreateComponent<Transform2DComponent>();
        entity.CreateComponent<RectangleColliderComponent>();
        entity.CreateComponent<KinematicRigidBody2DComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));
    }

    [Test]
    public void KinematicBody_ShouldBeCreated_WhenEntityIsRootAndHas_Transform2DComponent_And_CircleColliderComponent_And_KinematicRigidBody2DComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);

        // Act
        var entity = Scene.CreateEntity();
        entity.CreateComponent<Transform2DComponent>();
        entity.CreateComponent<CircleColliderComponent>();
        entity.CreateComponent<KinematicRigidBody2DComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Circle));
    }

    [Test]
    public void KinematicBody_ShouldNotBeCreated_WhenEntityIsRootAndHas_Transform2DComponent_And_TileColliderComponent_And_KinematicRigidBody2DComponent()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);

        // Act
        var entity = Scene.CreateEntity();
        entity.CreateComponent<Transform2DComponent>();
        entity.CreateComponent<TileColliderComponent>();
        entity.CreateComponent<KinematicRigidBody2DComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
    }

    [Test]
    public void KinematicBody_ShouldNotBeCreated_WhenEntityIsNotRoot()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);

        // Act
        var entity = Scene.CreateEntity().CreateChildEntity();
        entity.CreateComponent<Transform2DComponent>();
        entity.CreateComponent<RectangleColliderComponent>();
        entity.CreateComponent<KinematicRigidBody2DComponent>();

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
    }

    [Test]
    public void KinematicBody_ShouldBeRemoved_WhenTransform2DComponentIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleKinematicBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));

        // Act
        entity.RemoveComponent(entity.GetComponent<Transform2DComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
    }

    [Test]
    public void KinematicBody_ShouldBeRemoved_WhenRectangleColliderComponentIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleKinematicBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));

        // Act
        entity.RemoveComponent(entity.GetComponent<RectangleColliderComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
    }

    [Test]
    public void KinematicBody_ShouldBeRemoved_WhenCircleColliderComponentIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateCircleKinematicBody(0, 0, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Circle));

        // Act
        entity.RemoveComponent(entity.GetComponent<CircleColliderComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
    }

    [Test]
    public void KinematicBody_ShouldBeRemoved_WhenEntityStopsToBeRootEntity()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleKinematicBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var body = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(body.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(body.ColliderType, Is.EqualTo(ColliderType.Rectangle));

        // Act
        var parent = Scene.CreateEntity();
        entity.Parent = parent;

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);
    }

    [Test]
    public void KinematicBody_ShouldBeRecreated_WhenEntityBecomesRootEntity()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleKinematicBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var bodyBefore = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(bodyBefore.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(bodyBefore.ColliderType, Is.EqualTo(ColliderType.Rectangle));

        var parent = Scene.CreateEntity();
        entity.Parent = parent;

        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.Zero);

        // Act
        entity.Parent = null;

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var bodyAfter = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(bodyAfter.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(bodyAfter.ColliderType, Is.EqualTo(ColliderType.Rectangle));
    }

    [Test]
    public void KinematicBody_ShouldBecomeStaticBody_WhenKinematicRigidBody2DComponentIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var entity = CreateRectangleKinematicBody(0, 0, 10, 5);

        // Assume
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var bodyBefore = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(bodyBefore.Type, Is.EqualTo(BodyType.Kinematic));
        Assert.That(bodyBefore.ColliderType, Is.EqualTo(ColliderType.Rectangle));

        // Act
        entity.RemoveComponent(entity.GetComponent<KinematicRigidBody2DComponent>());

        // Assert
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(1));
        var bodyAfter = physicsSystem.PhysicsScene2D.Bodies[0];
        Assert.That(bodyAfter.Type, Is.EqualTo(BodyType.Static));
        Assert.That(bodyAfter.ColliderType, Is.EqualTo(ColliderType.Rectangle));
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
        var rectangleContacts1 = new List<Contact2D>();
        Assert.That(rectangleCollider1.IsColliding, Is.False);
        Assert.That(rectangleCollider1.GetContacts(rectangleContacts1), Is.Zero);
        Assert.That(rectangleContacts1, Has.Count.Zero);

        var rectangleContacts2 = new List<Contact2D>();
        Assert.That(rectangleCollider2.IsColliding, Is.False);
        Assert.That(rectangleCollider2.GetContacts(rectangleContacts2), Is.Zero);
        Assert.That(rectangleContacts2, Has.Count.Zero);
    }

    [Test]
    [Description(
        "Regression test for incorrect contacts removal when removed body has multiple (>=3) contacts and those are the only contacts in physics scene.")]
    public void KinematicBody_ShouldClearContactsWithMultipleBodies_WhenKinematicBodyIsRemoved()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();
        var kinematicBody = CreateRectangleKinematicBody(0, 0, 100, 50);
        var staticBody1 = CreateRectangleStaticBody(-75, 0, 100, 50);
        var staticBody2 = CreateRectangleStaticBody(75, 0, 100, 50);
        var staticBody3 = CreateRectangleStaticBody(0, 60, 50, 100);

        var kinematicCollider = kinematicBody.GetComponent<RectangleColliderComponent>();
        var staticCollider1 = staticBody1.GetComponent<RectangleColliderComponent>();
        var staticCollider2 = staticBody2.GetComponent<RectangleColliderComponent>();
        var staticCollider3 = staticBody3.GetComponent<RectangleColliderComponent>();

        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 0);

        // Assume
        Assert.That(kinematicCollider.IsColliding, Is.True);
        Assert.That(staticCollider1.IsColliding, Is.True);
        Assert.That(staticCollider2.IsColliding, Is.True);
        Assert.That(staticCollider3.IsColliding, Is.True);

        // Act
        kinematicBody.RemoveComponent(kinematicBody.GetComponent<Transform2DComponent>());
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1);

        // Assert
        var kinematicContacts = new List<Contact2D>();
        Assert.That(kinematicCollider.IsColliding, Is.False);
        Assert.That(kinematicCollider.GetContacts(kinematicContacts), Is.Zero);
        Assert.That(kinematicContacts, Has.Count.Zero);

        var staticContacts1 = new List<Contact2D>();
        Assert.That(staticCollider1.IsColliding, Is.False);
        Assert.That(staticCollider1.GetContacts(staticContacts1), Is.Zero);
        Assert.That(staticContacts1, Has.Count.Zero);

        var staticContacts2 = new List<Contact2D>();
        Assert.That(staticCollider2.IsColliding, Is.False);
        Assert.That(staticCollider2.GetContacts(staticContacts2), Is.Zero);
        Assert.That(staticContacts2, Has.Count.Zero);

        var staticContacts3 = new List<Contact2D>();
        Assert.That(staticCollider3.IsColliding, Is.False);
        Assert.That(staticCollider3.GetContacts(staticContacts3), Is.Zero);
        Assert.That(staticContacts3, Has.Count.Zero);
    }

    #endregion

    [Test]
    [Description("This test stresses body removal that forces updates to existing contacts.")]
    public void IntegrityTest_WhenMultipleCollidingBodiesAreCreatedAndDestroyed()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem();

        var kinematicBody1 = CreateRectangleKinematicBody(-75, 0, 100, 100, 0.1);
        var kinematicBody2 = CreateRectangleKinematicBody(0, 75, 100, 100, 0.2);
        var kinematicBody3 = CreateRectangleKinematicBody(75, 0, 100, 100, 0.3);
        var kinematicBody4 = CreateRectangleKinematicBody(0, -75, 100, 100, 0.4);

        var collider1 = kinematicBody1.GetComponent<RectangleColliderComponent>();
        var collider2 = kinematicBody2.GetComponent<RectangleColliderComponent>();
        var collider3 = kinematicBody3.GetComponent<RectangleColliderComponent>();
        var collider4 = kinematicBody4.GetComponent<RectangleColliderComponent>();

        // Act 0
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 0);

        // Assert 0
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(4));

        // Check that components are correctly linked to internal rigid bodies.
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.1));
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.2));
        Assert.That(kinematicBody3.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.3));
        Assert.That(kinematicBody4.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.4));

        Assert.That(collider1.ContactCount, Is.EqualTo(2));
        Assert.That(collider2.ContactCount, Is.EqualTo(2));
        Assert.That(collider3.ContactCount, Is.EqualTo(2));
        Assert.That(collider4.ContactCount, Is.EqualTo(2));

        AssertContacts(collider1, collider2, collider4);
        AssertContacts(collider2, collider1, collider3);
        AssertContacts(collider3, collider2, collider4);
        AssertContacts(collider4, collider1, collider3);

        // Act 1.1
        kinematicBody1.RemoveComponent(kinematicBody1.GetComponent<Transform2DComponent>());

        // Assert 1.1
        Assert.That(collider1.ContactCount, Is.EqualTo(0));
        Assert.That(collider2.ContactCount, Is.EqualTo(1));
        Assert.That(collider3.ContactCount, Is.EqualTo(2));
        Assert.That(collider4.ContactCount, Is.EqualTo(1));

        AssertContacts(collider2, collider3);
        AssertContacts(collider3, collider2, collider4);
        AssertContacts(collider4, collider3);

        // Act 1.2
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 1);

        // Assert 1.2
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(3));

        // Check that components are correctly linked to internal rigid bodies.
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.2));
        Assert.That(kinematicBody3.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.3));
        Assert.That(kinematicBody4.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.4));

        Assert.That(collider1.ContactCount, Is.EqualTo(0));
        Assert.That(collider2.ContactCount, Is.EqualTo(1));
        Assert.That(collider3.ContactCount, Is.EqualTo(2));
        Assert.That(collider4.ContactCount, Is.EqualTo(1));

        AssertContacts(collider2, collider3);
        AssertContacts(collider3, collider2, collider4);
        AssertContacts(collider4, collider3);

        // Act 2
        var transform1 = kinematicBody1.CreateComponent<Transform2DComponent>();
        transform1.Translation = new Vector2(-75, 0);
        transform1.Rotation = 0.1;

        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 2);

        // Assert 2
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(4));

        // Check that components are correctly linked to internal rigid bodies.
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.1));
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.2));
        Assert.That(kinematicBody3.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.3));
        Assert.That(kinematicBody4.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.4));

        Assert.That(collider1.ContactCount, Is.EqualTo(2));
        Assert.That(collider2.ContactCount, Is.EqualTo(2));
        Assert.That(collider3.ContactCount, Is.EqualTo(2));
        Assert.That(collider4.ContactCount, Is.EqualTo(2));

        AssertContacts(collider1, collider2, collider4);
        AssertContacts(collider2, collider1, collider3);
        AssertContacts(collider3, collider2, collider4);
        AssertContacts(collider4, collider1, collider3);

        // Act 3.1
        kinematicBody2.RemoveComponent(kinematicBody2.GetComponent<Transform2DComponent>());

        // Assert 3.1
        Assert.That(collider1.ContactCount, Is.EqualTo(1));
        Assert.That(collider2.ContactCount, Is.EqualTo(0));
        Assert.That(collider3.ContactCount, Is.EqualTo(1));
        Assert.That(collider4.ContactCount, Is.EqualTo(2));

        AssertContacts(collider1, collider4);
        AssertContacts(collider3, collider4);
        AssertContacts(collider4, collider1, collider3);

        // Act 3.2
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 3);

        // Assert 3.2
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(3));

        // Check that components are correctly linked to internal rigid bodies.
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.1));
        Assert.That(kinematicBody3.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.3));
        Assert.That(kinematicBody4.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.4));

        Assert.That(collider1.ContactCount, Is.EqualTo(1));
        Assert.That(collider2.ContactCount, Is.EqualTo(0));
        Assert.That(collider3.ContactCount, Is.EqualTo(1));
        Assert.That(collider4.ContactCount, Is.EqualTo(2));

        AssertContacts(collider1, collider4);
        AssertContacts(collider3, collider4);
        AssertContacts(collider4, collider1, collider3);

        // Act 4
        var transform2 = kinematicBody2.CreateComponent<Transform2DComponent>();
        transform2.Translation = new Vector2(0, 75);
        transform2.Rotation = 0.2;

        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 4);

        // Assert 4
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(4));

        // Check that components are correctly linked to internal rigid bodies.
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.1));
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.2));
        Assert.That(kinematicBody3.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.3));
        Assert.That(kinematicBody4.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.4));

        Assert.That(collider1.ContactCount, Is.EqualTo(2));
        Assert.That(collider2.ContactCount, Is.EqualTo(2));
        Assert.That(collider3.ContactCount, Is.EqualTo(2));
        Assert.That(collider4.ContactCount, Is.EqualTo(2));

        AssertContacts(collider1, collider2, collider4);
        AssertContacts(collider2, collider1, collider3);
        AssertContacts(collider3, collider2, collider4);
        AssertContacts(collider4, collider1, collider3);

        // Act 5.1
        kinematicBody3.RemoveComponent(kinematicBody3.GetComponent<Transform2DComponent>());

        // Assert 5.1
        Assert.That(collider1.ContactCount, Is.EqualTo(2));
        Assert.That(collider2.ContactCount, Is.EqualTo(1));
        Assert.That(collider3.ContactCount, Is.EqualTo(0));
        Assert.That(collider4.ContactCount, Is.EqualTo(1));

        AssertContacts(collider1, collider2, collider4);
        AssertContacts(collider2, collider1);
        AssertContacts(collider4, collider1);

        // Act 5.2
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 5);

        // Assert 5.2
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(3));

        // Check that components are correctly linked to internal rigid bodies.
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.1));
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.2));
        Assert.That(kinematicBody4.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.4));

        Assert.That(collider1.ContactCount, Is.EqualTo(2));
        Assert.That(collider2.ContactCount, Is.EqualTo(1));
        Assert.That(collider3.ContactCount, Is.EqualTo(0));
        Assert.That(collider4.ContactCount, Is.EqualTo(1));

        AssertContacts(collider1, collider2, collider4);
        AssertContacts(collider2, collider1);
        AssertContacts(collider4, collider1);

        // Act 6
        var transform3 = kinematicBody3.CreateComponent<Transform2DComponent>();
        transform3.Translation = new Vector2(75, 0);
        transform3.Rotation = 0.3;

        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 6);

        // Assert 6
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(4));

        // Check that components are correctly linked to internal rigid bodies.
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.1));
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.2));
        Assert.That(kinematicBody3.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.3));
        Assert.That(kinematicBody4.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.4));

        Assert.That(collider1.ContactCount, Is.EqualTo(2));
        Assert.That(collider2.ContactCount, Is.EqualTo(2));
        Assert.That(collider3.ContactCount, Is.EqualTo(2));
        Assert.That(collider4.ContactCount, Is.EqualTo(2));

        AssertContacts(collider1, collider2, collider4);
        AssertContacts(collider2, collider1, collider3);
        AssertContacts(collider3, collider2, collider4);
        AssertContacts(collider4, collider1, collider3);

        // Act 7.1
        kinematicBody4.RemoveComponent(kinematicBody4.GetComponent<Transform2DComponent>());

        // Assert 7.1
        Assert.That(collider1.ContactCount, Is.EqualTo(1));
        Assert.That(collider2.ContactCount, Is.EqualTo(2));
        Assert.That(collider3.ContactCount, Is.EqualTo(1));
        Assert.That(collider4.ContactCount, Is.EqualTo(0));

        AssertContacts(collider1, collider2);
        AssertContacts(collider2, collider1, collider3);
        AssertContacts(collider3, collider2);

        // Act 7.2
        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 7);

        // Assert 7.2
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(3));

        // Check that components are correctly linked to internal rigid bodies.
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.1));
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.2));
        Assert.That(kinematicBody3.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.3));

        Assert.That(collider1.ContactCount, Is.EqualTo(1));
        Assert.That(collider2.ContactCount, Is.EqualTo(2));
        Assert.That(collider3.ContactCount, Is.EqualTo(1));
        Assert.That(collider4.ContactCount, Is.EqualTo(0));

        AssertContacts(collider1, collider2);
        AssertContacts(collider2, collider1, collider3);
        AssertContacts(collider3, collider2);

        // Act 8
        var transform4 = kinematicBody4.CreateComponent<Transform2DComponent>();
        transform4.Translation = new Vector2(0, -75);
        transform4.Rotation = 0.4;

        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem, 8);

        // Assert 8
        Assert.That(physicsSystem.PhysicsScene2D.Bodies.Count, Is.EqualTo(4));

        // Check that components are correctly linked to internal rigid bodies.
        Assert.That(kinematicBody1.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.1));
        Assert.That(kinematicBody2.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.2));
        Assert.That(kinematicBody3.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.3));
        Assert.That(kinematicBody4.GetComponent<Transform2DComponent>().Rotation, Is.EqualTo(0.4));

        Assert.That(collider1.ContactCount, Is.EqualTo(2));
        Assert.That(collider2.ContactCount, Is.EqualTo(2));
        Assert.That(collider3.ContactCount, Is.EqualTo(2));
        Assert.That(collider4.ContactCount, Is.EqualTo(2));

        AssertContacts(collider1, collider2, collider4);
        AssertContacts(collider2, collider1, collider3);
        AssertContacts(collider3, collider2, collider4);
        AssertContacts(collider4, collider1, collider3);

        return;

        static void AssertContacts(Collider2DComponent collider, params Collider2DComponent[] collidersInContact)
        {
            var contacts = new List<Contact2D>();
            collider.GetContacts(contacts);
            Assert.That(contacts.Select(c => c.OtherCollider), Is.EquivalentTo(collidersInContact));
        }
    }
}