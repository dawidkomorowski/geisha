using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.Systems;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

public abstract class PhysicsSystemTestsBase
{
    private protected Scene Scene = null!;
    private protected IDebugRenderer DebugRenderer = null!;

    [SetUp]
    public void SetUp()
    {
        Scene = TestSceneFactory.Create();
        DebugRenderer = Substitute.For<IDebugRenderer>();
    }

    private protected PhysicsSystem GetPhysicsSystem()
    {
        return GetPhysicsSystem(new PhysicsConfiguration());
    }

    private protected PhysicsSystem GetPhysicsSystem(PhysicsConfiguration configuration)
    {
        var physicsSystem = new PhysicsSystem(configuration, DebugRenderer);
        Scene.AddObserver(physicsSystem);
        return physicsSystem;
    }

    protected Entity CreateRectangleKinematicBody(double entityX, double entityY, double rectangleWidth, double rectangleHeight)
    {
        var entity = Scene.CreateEntity();
        AddRectangleCollider(entity, entityX, entityY, rectangleWidth, rectangleHeight);
        entity.CreateComponent<KinematicRigidBody2DComponent>();
        return entity;
    }

    protected Entity CreateRectangleStaticBody(double entityX, double entityY, double rectangleWidth, double rectangleHeight)
    {
        var entity = Scene.CreateEntity();
        AddRectangleCollider(entity, entityX, entityY, rectangleWidth, rectangleHeight);
        return entity;
    }

    protected Entity CreateCircleKinematicBody(double entityX, double entityY, double radius)
    {
        var entity = Scene.CreateEntity();
        AddCircleCollider(entity, entityX, entityY, radius);
        entity.CreateComponent<KinematicRigidBody2DComponent>();
        return entity;
    }

    protected Entity CreateCircleStaticBody(double entityX, double entityY, double radius)
    {
        var entity = Scene.CreateEntity();
        AddCircleCollider(entity, entityX, entityY, radius);
        return entity;
    }

    protected Entity CreateRectangleStaticBodyWithParentTransform(double parentX, double parentY, double entityX, double entityY, double rectangleWidth,
        double rectangleHeight)
    {
        var parent = Scene.CreateEntity();

        var transform2DComponent = parent.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(parentX, parentY);
        transform2DComponent.Rotation = 0;
        transform2DComponent.Scale = Vector2.One;

        var child = CreateRectangleStaticBody(entityX, entityY, rectangleWidth, rectangleHeight);
        child.Parent = parent;

        return child;
    }

    private static void AddRectangleCollider(Entity entity, double entityX, double entityY, double rectangleWidth, double rectangleHeight)
    {
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(entityX, entityY);
        transform2DComponent.Rotation = 0;
        transform2DComponent.Scale = Vector2.One;

        var rectangleColliderComponent = entity.CreateComponent<RectangleColliderComponent>();
        rectangleColliderComponent.Dimensions = new Vector2(rectangleWidth, rectangleHeight);
    }

    private static void AddCircleCollider(Entity entity, double entityX, double entityY, double radius)
    {
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(entityX, entityY);
        transform2DComponent.Rotation = 0;
        transform2DComponent.Scale = Vector2.One;

        var circleColliderComponent = entity.CreateComponent<CircleColliderComponent>();
        circleColliderComponent.Radius = radius;
    }
}