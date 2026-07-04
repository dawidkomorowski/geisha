using System;
using Geisha.Engine.Core;
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

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class EnableVisualOutputAttribute : PropertyAttribute
{
    public EnableVisualOutputAttribute() : base(true)
    {
    }

    public static bool IsSetForCurrentTest()
    {
        var propertyName = nameof(EnableVisualOutputAttribute).Replace("Attribute", "");
        var hasProperty = TestContext.CurrentContext.Test.Properties.ContainsKey(propertyName);

        if (hasProperty)
        {
            return true;
        }

        var testClassName = TestContext.CurrentContext.Test.ClassName ?? throw new InvalidOperationException("Test.ClassName is null.");
        var testMethodName = TestContext.CurrentContext.Test.MethodName ?? throw new InvalidOperationException("Test.MethodName is null.");

        var testClass = Type.GetType(testClassName) ?? throw new InvalidOperationException("Test class not found.");
        var testMethod = testClass.GetMethod(testMethodName) ?? throw new InvalidOperationException("Test method not found.");

        var testClassHasAttribute = testClass.GetCustomAttributes(typeof(EnableVisualOutputAttribute), true).Length > 0;
        var testMethodHasAttribute = testMethod.GetCustomAttributes(typeof(EnableVisualOutputAttribute), true).Length > 0;

        return testClassHasAttribute || testMethodHasAttribute;
    }
}

public abstract class PhysicsSystemTestsBase
{
    private const bool GlobalEnableVisualOutput = false;
    private static bool EffectiveEnableVisualOutput => EnableVisualOutputAttribute.IsSetForCurrentTest() || GlobalEnableVisualOutput;

    private IDebugRendererForTests _debugRendererForTests = null!;
    private protected Scene Scene = null!;
    private protected ITimeSystem TimeSystem = null!;
    private protected IDebugRenderer DebugRenderer = null!;
    private PhysicsSystem? _physicsSystem;

    private protected const double Epsilon = 1e-6;
    private protected static Func<Vector2, Vector2, bool> Vector2Equality => ToleranceEquality.ForVector2(Epsilon);

    // ReSharper disable once InconsistentNaming
    private protected static Func<Matrix3x3, Matrix3x3, bool> Matrix3x3Equality => ToleranceEquality.ForMatrix3x3(Epsilon);

    // ReSharper disable once InconsistentNaming
    private protected static Func<AABB2D, AABB2D, bool> AABB2DEquality => ToleranceEquality.ForAABB2D(Epsilon);

    [SetUp]
    public void SetUp()
    {
        Scene = TestSceneFactory.Create();
        TimeSystem = Substitute.For<ITimeSystem>();
        TimeSystem.FixedDeltaTime.Returns(TimeSpan.FromSeconds(1.0 / 60.0));
        DebugRenderer = Substitute.For<IDebugRenderer>();
        _debugRendererForTests = TestKit.CreateDebugRenderer(DebugRenderer, EffectiveEnableVisualOutput);
    }

    [TearDown]
    public void TearDown()
    {
        _debugRendererForTests.Dispose();
        _physicsSystem?.Dispose();
        _physicsSystem = null;
    }

    private protected PhysicsSystem GetPhysicsSystem()
    {
        var physicsConfiguration = new PhysicsConfiguration
        {
            PenetrationTolerance = 0d,
            EnableDebugRendering = EffectiveEnableVisualOutput
        };
        return GetPhysicsSystem(physicsConfiguration);
    }

    private protected PhysicsSystem GetPhysicsSystem(PhysicsConfiguration configuration)
    {
        var physicsSystem = new PhysicsSystem(configuration, TimeSystem, _debugRendererForTests);
        _physicsSystem = physicsSystem;

        Scene.AddObserver(physicsSystem);
        return physicsSystem;
    }

    private protected void SaveVisualOutput(PhysicsSystem physicsSystem, int stage = 0, double scale = 1d, Action<IDebugRenderer>? postDrawAction = null)
    {
        _debugRendererForTests.BeginDraw(scale);
        physicsSystem.SynchronizePhysicsState();
        physicsSystem.PreparePhysicsDebugInformation();
        postDrawAction?.Invoke(_debugRendererForTests);
        _debugRendererForTests.EndDrawUsingTestContext(stage);
        DebugRenderer.ClearReceivedCalls();
    }


    private protected static bool ContactPoint2DComparison(ContactPoint2D p1, ContactPoint2D p2)
    {
        return Vector2Equality(p1.WorldPosition, p2.WorldPosition) &&
               Vector2Equality(p1.ThisLocalPosition, p2.ThisLocalPosition) &&
               Vector2Equality(p1.OtherLocalPosition, p2.OtherLocalPosition);
    }

    private protected Entity CreateRectangleKinematicBody(AxisAlignedRectangle rectangle, double rotation = 0d) =>
        CreateRectangleKinematicBody(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height, rotation);

    private protected Entity CreateRectangleKinematicBody(double x, double y, double width, double height) =>
        CreateRectangleKinematicBody(x, y, width, height, 0);

    private protected Entity CreateRectangleKinematicBody(double x, double y, double width, double height, double rotation)
    {
        var entity = Scene.CreateEntity();
        AddRectangleCollider(entity, x, y, width, height, rotation);
        entity.CreateComponent<KinematicRigidBody2DComponent>();
        return entity;
    }

    private protected Entity CreateRectangleStaticBody(AxisAlignedRectangle rectangle, double rotation = 0d) =>
        CreateRectangleStaticBody(rectangle.Center.X, rectangle.Center.Y, rectangle.Width, rectangle.Height, rotation);

    private protected Entity CreateRectangleStaticBody(double x, double y, double width, double height) => CreateRectangleStaticBody(x, y, width, height, 0);

    private protected Entity CreateRectangleStaticBody(double x, double y, double width, double height, double rotation)
    {
        var entity = Scene.CreateEntity();
        AddRectangleCollider(entity, x, y, width, height, rotation);
        return entity;
    }

    private protected Entity CreateCircleKinematicBody(Circle circle, double rotation = 0d) =>
        CreateCircleKinematicBody(circle.Center.X, circle.Center.Y, circle.Radius, rotation);

    private protected Entity CreateCircleKinematicBody(double x, double y, double radius) => CreateCircleKinematicBody(x, y, radius, 0);

    private protected Entity CreateCircleKinematicBody(double x, double y, double radius, double rotation)
    {
        var entity = Scene.CreateEntity();
        AddCircleCollider(entity, x, y, radius, rotation);
        entity.CreateComponent<KinematicRigidBody2DComponent>();
        return entity;
    }

    private protected Entity CreateCircleStaticBody(Circle circle, double rotation = 0d) =>
        CreateCircleStaticBody(circle.Center.X, circle.Center.Y, circle.Radius, rotation);

    private protected Entity CreateCircleStaticBody(double x, double y, double radius) => CreateCircleStaticBody(x, y, radius, 0);

    private Entity CreateCircleStaticBody(double x, double y, double radius, double rotation)
    {
        var entity = Scene.CreateEntity();
        AddCircleCollider(entity, x, y, radius, rotation);
        return entity;
    }

    private protected Entity CreateTileStaticBody(Vector2 position) => CreateTileStaticBody(position.X, position.Y);

    private protected Entity CreateTileStaticBody(double x, double y)
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