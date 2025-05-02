using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems.PhysicsSystemTests;

// TODO How to easily test more complex layout?
// How to test that there were no unexpected calls?
// Maybe save reference rendered image and compare visual output? How does it impact Git LFS?
[TestFixture]
public class DebugInformationTests : PhysicsSystemTestsBase
{
    private readonly Color _staticBodyColor = Color.Green;
    private readonly Color _kinematicBodyColor = Color.Blue;

    [TestCase(false, 0)]
    [TestCase(true, 1)]
    public void PreparePhysicsDebugInformation_ShouldDrawCircleForCircleKinematicBody_WhenCollisionGeometryRenderingIsEnabled(
        bool renderCollisionGeometry, int expectedDrawCallsCount)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(new PhysicsConfiguration { RenderCollisionGeometry = renderCollisionGeometry });
        CreateCircleKinematicBody(10, 20, 30);

        SaveVisualOutput(physicsSystem);
        physicsSystem.ProcessPhysics();

        // Act
        physicsSystem.PreparePhysicsDebugInformation();

        // Assert
        var circle = new Circle(new Vector2(10, 20), 30);
        DebugRenderer.Received(expectedDrawCallsCount).DrawCircle(circle, _kinematicBodyColor);
    }

    [TestCase(false, 0)]
    [TestCase(true, 1)]
    public void PreparePhysicsDebugInformation_ShouldDrawRectangleForRectangleKinematicBody_WhenCollisionGeometryRenderingIsEnabled(
        bool renderCollisionGeometry, int expectedDrawCallsCount)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(new PhysicsConfiguration { RenderCollisionGeometry = renderCollisionGeometry });
        var entity = CreateRectangleKinematicBody(10, 20, 100, 200);

        SaveVisualOutput(physicsSystem);
        physicsSystem.ProcessPhysics();

        // Act
        physicsSystem.PreparePhysicsDebugInformation();

        // Assert
        var rectangle = new AxisAlignedRectangle(new Vector2(100, 200));
        var transform = entity.GetComponent<Transform2DComponent>().ToMatrix();
        DebugRenderer.Received(expectedDrawCallsCount).DrawRectangle(rectangle, _kinematicBodyColor, transform);
    }

    [TestCase(false, 0)]
    [TestCase(true, 1)]
    public void PreparePhysicsDebugInformation_ShouldDrawCircleForCircleStaticBody_WhenCollisionGeometryRenderingIsEnabled(
        bool renderCollisionGeometry, int expectedDrawCallsCount)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(new PhysicsConfiguration { RenderCollisionGeometry = renderCollisionGeometry });
        CreateCircleStaticBody(10, 20, 30);

        SaveVisualOutput(physicsSystem);
        physicsSystem.ProcessPhysics();

        // Act
        physicsSystem.PreparePhysicsDebugInformation();

        // Assert
        var circle = new Circle(new Vector2(10, 20), 30);
        DebugRenderer.Received(expectedDrawCallsCount).DrawCircle(circle, _staticBodyColor);
    }

    [TestCase(false, 0)]
    [TestCase(true, 1)]
    public void PreparePhysicsDebugInformation_ShouldDrawRectangleForRectangleStaticBody_WhenCollisionGeometryRenderingIsEnabled(
        bool renderCollisionGeometry, int expectedDrawCallsCount)
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(new PhysicsConfiguration { RenderCollisionGeometry = renderCollisionGeometry });
        var entity = CreateRectangleStaticBody(10, 20, 100, 200);

        SaveVisualOutput(physicsSystem);
        physicsSystem.ProcessPhysics();

        // Act
        physicsSystem.PreparePhysicsDebugInformation();

        // Assert
        var rectangle = new AxisAlignedRectangle(new Vector2(100, 200));
        var transform = entity.GetComponent<Transform2DComponent>().ToMatrix();
        DebugRenderer.Received(expectedDrawCallsCount).DrawRectangle(rectangle, _staticBodyColor, transform);
    }

    [Test]
    public void PreparePhysicsDebugInformation_ShouldDrawContacts_WhenEntityIsColliding()
    {
        // Arrange
        var physicsSystem = GetPhysicsSystem(new PhysicsConfiguration { RenderCollisionGeometry = true });
        var circleEntity = CreateCircleKinematicBody(10, 20, 30);
        var rectangleEntity = CreateRectangleKinematicBody(10, 20, 100, 200);

        SaveVisualOutput(physicsSystem);
        physicsSystem.ProcessPhysics();

        // Assume
        Assert.That(circleEntity.GetComponent<CircleColliderComponent>().IsColliding, Is.True);
        Assert.That(rectangleEntity.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);

        // Act
        physicsSystem.PreparePhysicsDebugInformation();

        // Assert
        var circle = new Circle(new Vector2(10, 20), 30);
        DebugRenderer.Received(1).DrawCircle(circle, _kinematicBodyColor);

        var rectangle = new AxisAlignedRectangle(new Vector2(100, 200));
        var transform = rectangleEntity.GetComponent<Transform2DComponent>().ToMatrix();
        DebugRenderer.Received(1).DrawRectangle(rectangle, _kinematicBodyColor, transform);

        // TODO Assert drawing contacts?
    }
}