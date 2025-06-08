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
    private const bool EnableVisualOutput = false;
    private protected Scene Scene = null!;
    private protected IDebugRenderer DebugRenderer = null!;
    private IDebugRendererForTests _debugRendererForTests = null!;

    protected const double Epsilon = 1e-6;
    protected static IEqualityComparer<Vector2> Vector2Comparer => CommonEqualityComparer.Vector2(Epsilon);

    // ReSharper disable once InconsistentNaming
    protected static IEqualityComparer<Matrix3x3> Matrix3x3Comparer => CommonEqualityComparer.Matrix3x3(Epsilon);

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
        var physicsConfiguration = new PhysicsConfiguration
        {
            PenetrationTolerance = 0d,
            RenderCollisionGeometry = EnableVisualOutput
        };
        return GetPhysicsSystem(physicsConfiguration);
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
        DebugRenderer.ClearReceivedCalls();
    }


    protected static bool ContactPoint2DComparison(ContactPoint2D p1, ContactPoint2D p2)
    {
        return Vector2Comparer.Equals(p1.WorldPosition, p2.WorldPosition) &&
               Vector2Comparer.Equals(p1.ThisLocalPosition, p2.ThisLocalPosition) &&
               Vector2Comparer.Equals(p1.OtherLocalPosition, p2.OtherLocalPosition);
    }

    protected Entity CreateRectangleKinematicBody(AxisAlignedRectangle rectangle, double rotation = 0d) =>
        CreateRectangleKinematicBody(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height, rotation);

    protected Entity CreateRectangleKinematicBody(double x, double y, double width, double height) => CreateRectangleKinematicBody(x, y, width, height, 0);

    protected Entity CreateRectangleKinematicBody(double x, double y, double width, double height, double rotation)
    {
        var entity = Scene.CreateEntity();
        AddRectangleCollider(entity, x, y, width, height, rotation);
        entity.CreateComponent<KinematicRigidBody2DComponent>();
        return entity;
    }

    protected Entity CreateRectangleStaticBody(AxisAlignedRectangle rectangle, double rotation = 0d) =>
        CreateRectangleStaticBody(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height, rotation);

    protected Entity CreateRectangleStaticBody(double x, double y, double width, double height) => CreateRectangleStaticBody(x, y, width, height, 0);

    protected Entity CreateRectangleStaticBody(double x, double y, double width, double height, double rotation)
    {
        var entity = Scene.CreateEntity();
        AddRectangleCollider(entity, x, y, width, height, rotation);
        return entity;
    }

    protected Entity CreateCircleKinematicBody(Circle circle, double rotation = 0d) =>
        CreateCircleKinematicBody(circle.Center.X, circle.Center.Y, circle.Radius, rotation);

    protected Entity CreateCircleKinematicBody(double x, double y, double radius) => CreateCircleKinematicBody(x, y, radius, 0);

    protected Entity CreateCircleKinematicBody(double x, double y, double radius, double rotation)
    {
        var entity = Scene.CreateEntity();
        AddCircleCollider(entity, x, y, radius, rotation);
        entity.CreateComponent<KinematicRigidBody2DComponent>();
        return entity;
    }

    protected Entity CreateCircleStaticBody(Circle circle, double rotation = 0d) =>
        CreateCircleStaticBody(circle.Center.X, circle.Center.Y, circle.Radius, rotation);

    protected Entity CreateCircleStaticBody(double x, double y, double radius) => CreateCircleStaticBody(x, y, radius, 0);

    private Entity CreateCircleStaticBody(double x, double y, double radius, double rotation)
    {
        var entity = Scene.CreateEntity();
        AddCircleCollider(entity, x, y, radius, rotation);
        return entity;
    }

    protected Entity CreateTileStaticBody(Vector2 position) => CreateTileStaticBody(position.X, position.Y);

    protected Entity CreateTileStaticBody(double x, double y)
    {
        var entity = Scene.CreateEntity();
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(x, y);
        entity.CreateComponent<TileColliderComponent>();
        return entity;
    }

    private static void AddRectangleCollider(Entity entity, double x, double y, double width, double height, double rotation)
    {
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(x, y);
        transform2DComponent.Rotation = rotation;
        transform2DComponent.Scale = Vector2.One;

        var rectangleColliderComponent = entity.CreateComponent<RectangleColliderComponent>();
        rectangleColliderComponent.Dimensions = new Vector2(width, height);
    }

    private static void AddCircleCollider(Entity entity, double x, double y, double radius, double rotation)
    {
        var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
        transform2DComponent.Translation = new Vector2(x, y);
        transform2DComponent.Rotation = rotation;
        transform2DComponent.Scale = Vector2.One;

        var circleColliderComponent = entity.CreateComponent<CircleColliderComponent>();
        circleColliderComponent.Radius = radius;
    }
}