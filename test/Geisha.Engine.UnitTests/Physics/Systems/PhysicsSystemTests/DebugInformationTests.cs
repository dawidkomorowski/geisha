using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

[TestFixture]
public class DebugInformationTests : PhysicsSystemTestsBase
{
    private readonly Color _staticBodyColor = Color.Green;
    private readonly Color _kinematicBodyColor = Color.Blue;
    private readonly Color _contactPointColor = Color.FromArgb(255, 255, 165, 0);
    private readonly Color _contactNormalColor = Color.Black;

    [TestCase(false, 0)]
    [TestCase(true, 1)]
    public void PreparePhysicsDebugInformation_ShouldDrawCircleForCircleStaticBody_WhenCollisionGeometryRenderingIsEnabled(
        bool renderCollisionGeometry, int expectedDrawCallsCount)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(new PhysicsConfiguration { RenderCollisionGeometry = renderCollisionGeometry });

        var circle = new Circle(new Vector2(10, 20), 30);
        CreateCircleStaticBody(circle, Angle.Deg2Rad(30));

        SaveVisualOutput(physicsSystem);
        physicsSystem.ProcessPhysics();

        // Act
        physicsSystem.PreparePhysicsDebugInformation();

        // Assert
        DebugRenderer.Received(expectedDrawCallsCount).DrawCircle(circle, _staticBodyColor);

        var rectangle = new AxisAlignedRectangle(15, 0, 30, 0);
        DebugRenderer.Received(expectedDrawCallsCount)
            .DrawRectangle(rectangle, _staticBodyColor, Matrix3x3.CreateTRS(new Vector2(10, 20), Angle.Deg2Rad(30), Vector2.One));

        Assert.That(DebugRenderer.ReceivedCalls().Count(), Is.EqualTo(expectedDrawCallsCount * 2));
    }

    [TestCase(false, 0)]
    [TestCase(true, 1)]
    public void PreparePhysicsDebugInformation_ShouldDrawRectangleForRectangleStaticBody_WhenCollisionGeometryRenderingIsEnabled(
        bool renderCollisionGeometry, int expectedDrawCallsCount)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(new PhysicsConfiguration { RenderCollisionGeometry = renderCollisionGeometry });
        var entity = CreateRectangleStaticBody(10, 20, 200, 100, Angle.Deg2Rad(30));

        SaveVisualOutput(physicsSystem);
        physicsSystem.ProcessPhysics();

        // Act
        physicsSystem.PreparePhysicsDebugInformation();

        // Assert
        var rectangle = new AxisAlignedRectangle(new Vector2(200, 100));
        var transform = entity.GetComponent<Transform2DComponent>().ToMatrix();
        DebugRenderer.Received(expectedDrawCallsCount).DrawRectangle(rectangle, _staticBodyColor, transform);

        Assert.That(DebugRenderer.ReceivedCalls().Count(), Is.EqualTo(expectedDrawCallsCount));
    }

    [TestCase(false, 0)]
    [TestCase(true, 1)]
    public void PreparePhysicsDebugInformation_ShouldDrawRectangleForTileStaticBody_WhenCollisionGeometryRenderingIsEnabled(
        bool renderCollisionGeometry, int expectedDrawCallsCount)
    {
        // Arrange
        var physicsConfiguration = new PhysicsConfiguration
        {
            RenderCollisionGeometry = renderCollisionGeometry,
            TileSize = new SizeD(200, 100)
        };
        var physicsSystem = GetPhysicsSystem(physicsConfiguration);
        CreateTileStaticBody(400, -100);

        SaveVisualOutput(physicsSystem);
        physicsSystem.ProcessPhysics();

        // Act
        physicsSystem.PreparePhysicsDebugInformation();

        // Assert
        var rectangle = new AxisAlignedRectangle(new Vector2(200, 100));
        var transform = Matrix3x3.CreateTranslation(new Vector2(400, -100));
        DebugRenderer.Received(expectedDrawCallsCount).DrawRectangle(rectangle, _staticBodyColor, transform);

        Assert.That(DebugRenderer.ReceivedCalls().Count(), Is.EqualTo(expectedDrawCallsCount));
    }

    [TestCase(false, 0)]
    [TestCase(true, 1)]
    public void PreparePhysicsDebugInformation_ShouldDrawCircleForCircleKinematicBody_WhenCollisionGeometryRenderingIsEnabled(
        bool renderCollisionGeometry, int expectedDrawCallsCount)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(new PhysicsConfiguration { RenderCollisionGeometry = renderCollisionGeometry });

        var circle = new Circle(new Vector2(10, 20), 30);
        CreateCircleKinematicBody(circle, Angle.Deg2Rad(30));

        SaveVisualOutput(physicsSystem);
        physicsSystem.ProcessPhysics();

        // Act
        physicsSystem.PreparePhysicsDebugInformation();

        // Assert
        DebugRenderer.Received(expectedDrawCallsCount).DrawCircle(circle, _kinematicBodyColor);

        var rectangle = new AxisAlignedRectangle(15, 0, 30, 0);
        DebugRenderer.Received(expectedDrawCallsCount)
            .DrawRectangle(rectangle, _kinematicBodyColor, Matrix3x3.CreateTRS(new Vector2(10, 20), Angle.Deg2Rad(30), Vector2.One));

        Assert.That(DebugRenderer.ReceivedCalls().Count(), Is.EqualTo(expectedDrawCallsCount * 2));
    }

    [TestCase(false, 0)]
    [TestCase(true, 1)]
    public void PreparePhysicsDebugInformation_ShouldDrawRectangleForRectangleKinematicBody_WhenCollisionGeometryRenderingIsEnabled(
        bool renderCollisionGeometry, int expectedDrawCallsCount)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(new PhysicsConfiguration { RenderCollisionGeometry = renderCollisionGeometry });
        var entity = CreateRectangleKinematicBody(10, 20, 200, 100, Angle.Deg2Rad(30));

        SaveVisualOutput(physicsSystem);
        physicsSystem.ProcessPhysics();

        // Act
        physicsSystem.PreparePhysicsDebugInformation();

        // Assert
        var rectangle = new AxisAlignedRectangle(new Vector2(200, 100));
        var transform = entity.GetComponent<Transform2DComponent>().ToMatrix();
        DebugRenderer.Received(expectedDrawCallsCount).DrawRectangle(rectangle, _kinematicBodyColor, transform);

        Assert.That(DebugRenderer.ReceivedCalls().Count(), Is.EqualTo(expectedDrawCallsCount));
    }

    [Test]
    public void PreparePhysicsDebugInformation_ShouldDrawContacts_WhenEntityIsColliding()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(new PhysicsConfiguration { RenderCollisionGeometry = true });
        var circleEntity = CreateCircleKinematicBody(-40, 40, 30, Angle.Deg2Rad(30));
        var rectangleEntity = CreateRectangleStaticBody(-20, -30, 200, 50, Angle.Deg2Rad(-30));

        physicsSystem.ProcessPhysics();
        SaveVisualOutput(physicsSystem);

        // Assume
        Assert.That(circleEntity.GetComponent<CircleColliderComponent>().IsColliding, Is.True);
        Assert.That(rectangleEntity.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);

        // Act
        physicsSystem.PreparePhysicsDebugInformation();

        // Assert
        // Circle
        var circle = new Circle(new Vector2(-40, 40), 30);
        DebugRenderer.Received(1).DrawCircle(circle, _kinematicBodyColor);

        var radius = new AxisAlignedRectangle(15, 0, 30, 0);
        DebugRenderer.Received(1)
            .DrawRectangle(radius, _kinematicBodyColor, Matrix3x3.CreateTRS(new Vector2(-40, 40), Angle.Deg2Rad(30), Vector2.One));

        // Rectangle
        var rectangle = new AxisAlignedRectangle(new Vector2(200, 50));
        var transform = rectangleEntity.GetComponent<Transform2DComponent>().ToMatrix();
        DebugRenderer.Received(1).DrawRectangle(rectangle, _staticBodyColor, transform);

        // Contact point
        var contact2D = circleEntity.GetComponent<CircleColliderComponent>().GetContacts()[0];
        var contactPoint = new Circle(contact2D.ContactPoints[0].WorldPosition, 3);
        DebugRenderer.Received(1).DrawCircle(contactPoint, _contactPointColor);

        // Contact normal
        var contactNormal = new AxisAlignedRectangle(15, 0, 30, 0);
        var expectedTransform = Matrix3x3.CreateTRS(contact2D.ContactPoints[0].WorldPosition, Angle.Deg2Rad(60), Vector2.One);
        DebugRenderer.Received(1).DrawRectangle(contactNormal, _contactNormalColor, Arg.Is<Matrix3x3>(m => Matrix3x3Comparer.Equals(expectedTransform, m)));

        // Total calls
        Assert.That(DebugRenderer.ReceivedCalls().Count(), Is.EqualTo(5));
    }
}