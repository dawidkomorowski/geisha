using System.Collections.Generic;
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
    private const bool EnableVisualOutput = true;
    private protected Scene Scene = null!;
    private protected IDebugRenderer DebugRenderer = null!;
    private IDebugRendererForTests _debugRendererForTests = null!;

    protected const double Epsilon = 1e-6;
    protected static IEqualityComparer<Vector2> Vector2Comparer => CommonEqualityComparer.Vector2(Epsilon);


    [SetUp]
    public void SetUp()
    {
        Scene = TestSceneFactory.Create();
        DebugRenderer = Substitute.For<IDebugRenderer>();
        _debugRendererForTests = TestKit.CreateDebugRenderer(DebugRenderer, EnableVisualOutput);
    }

    [TearDown]
    public void TearDown()
    {
        _debugRendererForTests.Dispose();
    }

    private protected PhysicsSystem GetPhysicsSystem()
    {
        return GetPhysicsSystem(new PhysicsConfiguration { RenderCollisionGeometry = EnableVisualOutput });
    }

    private protected PhysicsSystem GetPhysicsSystem(PhysicsConfiguration configuration)
    {
        var physicsSystem = new PhysicsSystem(configuration, _debugRendererForTests);
        Scene.AddObserver(physicsSystem);
        return physicsSystem;
    }

    private protected void SaveVisualOutput(PhysicsSystem physicsSystem, int stage = 0, double scale = 1d)
    {
        _debugRendererForTests.BeginDraw(scale);
        physicsSystem.SynchronizeBodies();
        physicsSystem.PreparePhysicsDebugInformation();
        _debugRendererForTests.EndDraw(stage);
    }


    protected static bool ContactPoint2DComparison(ContactPoint2D p1, ContactPoint2D p2)
    {
        return Vector2Comparer.Equals(p1.WorldPosition, p2.WorldPosition) &&
               Vector2Comparer.Equals(p1.ThisLocalPosition, p2.ThisLocalPosition) &&
               Vector2Comparer.Equals(p1.OtherLocalPosition, p2.OtherLocalPosition);
    }

    protected Entity CreateRectangleKinematicBody(AxisAlignedRectangle rectangle, double rotation = 0d)
    {
        return CreateRectangleKinematicBody(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height, rotation);
    }

    protected Entity CreateRectangleKinematicBody(double entityX, double entityY, double rectangleWidth, double rectangleHeight)
    {
        return CreateRectangleKinematicBody(entityX, entityY, rectangleWidth, rectangleHeight, 0);
    }

    private Entity CreateRectangleKinematicBody(double entityX, double entityY, double rectangleWidth, double rectangleHeight, double rotation)
    {
        var entity = Scene.CreateEntity();
        AddRectangleCollider(entity, entityX, entityY, rectangleWidth, rectangleHeight, rotation);
        entity.CreateComponent<KinematicRigidBody2DComponent>();
        return entity;
    }

    protected Entity CreateRectangleStaticBody(AxisAlignedRectangle rectangle, double rotation = 0d)
    {
        return CreateRectangleStaticBody(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height, rotation);
    }

    protected Entity CreateRectangleStaticBody(double entityX, double entityY, double rectangleWidth, double rectangleHeight)
    {
        return CreateRectangleStaticBody(entityX, entityY, rectangleWidth, rectangleHeight, 0);
    }

    private Entity CreateRectangleStaticBody(double entityX, double entityY, double rectangleWidth, double rectangleHeight, double rotation)
    {
        var entity = Scene.CreateEntity();
        AddRectangleCollider(entity, entityX, entityY, rectangleWidth, rectangleHeight, rotation);
        return entity;
    }

    protected Entity CreateCircleKinematicBody(Circle circle, double rotation = 0d)
    {
        return CreateCircleKinematicBody(circle.Center.X, circle.Center.Y, circle.Radius, rotation);
    }

    protected Entity CreateCircleKinematicBody(double entityX, double entityY, double radius)
    {
        return CreateCircleKinematicBody(entityX, entityY, radius, 0);
    }

    private Entity CreateCircleKinematicBody(double entityX, double entityY, double radius, double rotation)
    {
        var entity = Scene.CreateEntity();
        AddCircleCollider(entity, entityX, entityY, radius, rotation);
        entity.CreateComponent<KinematicRigidBody2DComponent>();
        return entity;
    }

    protected Entity CreateCircleStaticBody(Circle circle, double rotation = 0d)
    {
        return CreateCircleStaticBody(circle.Center.X, circle.Center.Y, circle.Radius, rotation);
    }

    protected Entity CreateCircleStaticBody(double entityX, double entityY, double radius)
    {
        return CreateCircleStaticBody(entityX, entityY, radius, 0);
    }

    private Entity CreateCircleStaticBody(double entityX, double entityY, double radius, double rotation)
    {
        var entity = Scene.CreateEntity();
        AddCircleCollider(entity, entityX, entityY, radius, rotation);
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

    private static void AddRectangleCollider(Entity entity, double entityX, double entityY, double rectangleWidth, double rectangleHeight, double rotation)
    {
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(entityX, entityY);
        transform2DComponent.Rotation = rotation;
        transform2DComponent.Scale = Vector2.One;

        var rectangleColliderComponent = entity.CreateComponent<RectangleColliderComponent>();
        rectangleColliderComponent.Dimensions = new Vector2(rectangleWidth, rectangleHeight);
    }

    private static void AddCircleCollider(Entity entity, double entityX, double entityY, double radius, double rotation)
    {
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(entityX, entityY);
        transform2DComponent.Rotation = rotation;
        transform2DComponent.Scale = Vector2.One;

        var circleColliderComponent = entity.CreateComponent<CircleColliderComponent>();
        circleColliderComponent.Radius = radius;
    }
}