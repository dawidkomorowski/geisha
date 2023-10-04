using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Components;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Systems.RenderingSystemTests;

[TestFixture]
public class EllipseRendererComponentTests : RenderingSystemTestsBase
{
    [Test]
    public void RenderingSystem_ShouldKeepEllipseRendererComponentState_WhenRenderingSystemIsRemovedFromSceneObserversOfScene()
    {
        // Arrange
        // Renderer2DComponent
        const bool visible = false;
        const string sortingLayerName = "some sorting layer";
        const int orderInLayer = 12;
        // EllipseRendererComponent
        const double radiusX = 10;
        const double radiusY = 20;
        var color = Color.Red;
        const bool fillInterior = true;

        var (renderingSystem, renderingScene) = GetRenderingSystem();

        var entity = renderingScene.AddEllipse();
        var ellipseRendererComponent = entity.GetComponent<EllipseRendererComponent>();

        // Renderer2DComponent
        ellipseRendererComponent.Visible = visible;
        ellipseRendererComponent.SortingLayerName = sortingLayerName;
        ellipseRendererComponent.OrderInLayer = orderInLayer;
        // EllipseRendererComponent
        ellipseRendererComponent.RadiusX = radiusX;
        ellipseRendererComponent.RadiusY = radiusY;
        ellipseRendererComponent.Color = color;
        ellipseRendererComponent.FillInterior = fillInterior;

        // Assume
        Assume.That(ellipseRendererComponent.IsManagedByRenderingSystem, Is.True);

        // Act
        renderingScene.Scene.RemoveObserver(renderingSystem);

        // Assert
        // Renderer2DComponent
        Assert.That(ellipseRendererComponent.IsManagedByRenderingSystem, Is.False);
        Assert.That(ellipseRendererComponent.Visible, Is.EqualTo(visible));
        Assert.That(ellipseRendererComponent.SortingLayerName, Is.EqualTo(sortingLayerName));
        Assert.That(ellipseRendererComponent.OrderInLayer, Is.EqualTo(orderInLayer));
        // EllipseRendererComponent
        Assert.That(ellipseRendererComponent.RadiusX, Is.EqualTo(radiusX));
        Assert.That(ellipseRendererComponent.RadiusY, Is.EqualTo(radiusY));
        Assert.That(ellipseRendererComponent.Color, Is.EqualTo(color));
        Assert.That(ellipseRendererComponent.FillInterior, Is.EqualTo(fillInterior));
    }

    [Test]
    public void RenderingSystem_ShouldKeepEllipseRendererComponentState_WhenRenderingSystemIsAddedToSceneObserversOfScene()
    {
        // Arrange
        // Renderer2DComponent
        const bool visible = false;
        const string sortingLayerName = "some sorting layer";
        const int orderInLayer = 12;
        // EllipseRendererComponent
        const double radiusX = 10;
        const double radiusY = 20;
        var color = Color.Red;
        const bool fillInterior = true;

        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.Scene.RemoveObserver(renderingSystem);

        var entity = renderingScene.AddEllipse();
        var ellipseRendererComponent = entity.GetComponent<EllipseRendererComponent>();

        // Renderer2DComponent
        ellipseRendererComponent.Visible = visible;
        ellipseRendererComponent.SortingLayerName = sortingLayerName;
        ellipseRendererComponent.OrderInLayer = orderInLayer;
        // EllipseRendererComponent
        ellipseRendererComponent.RadiusX = radiusX;
        ellipseRendererComponent.RadiusY = radiusY;
        ellipseRendererComponent.Color = color;
        ellipseRendererComponent.FillInterior = fillInterior;

        // Assume
        Assume.That(ellipseRendererComponent.IsManagedByRenderingSystem, Is.False);

        // Act
        renderingScene.Scene.AddObserver(renderingSystem);

        // Assert
        // Renderer2DComponent
        Assert.That(ellipseRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(ellipseRendererComponent.Visible, Is.EqualTo(visible));
        Assert.That(ellipseRendererComponent.SortingLayerName, Is.EqualTo(sortingLayerName));
        Assert.That(ellipseRendererComponent.OrderInLayer, Is.EqualTo(orderInLayer));
        // EllipseRendererComponent
        Assert.That(ellipseRendererComponent.RadiusX, Is.EqualTo(radiusX));
        Assert.That(ellipseRendererComponent.RadiusY, Is.EqualTo(radiusY));
        Assert.That(ellipseRendererComponent.Color, Is.EqualTo(color));
        Assert.That(ellipseRendererComponent.FillInterior, Is.EqualTo(fillInterior));
    }

    [Test]
    public void RenderScene_ShouldDrawEllipse_WhenSceneContainsEntityWithEllipseRendererAndTransform()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var entity = renderingScene.AddEllipse();

        // Act
        renderingSystem.RenderScene();

        // Assert
        var ellipseRenderer = entity.GetComponent<EllipseRendererComponent>();
        RenderingContext2D.Received(1).DrawEllipse(new Ellipse(ellipseRenderer.RadiusX, ellipseRenderer.RadiusY), ellipseRenderer.Color,
            ellipseRenderer.FillInterior, entity.Get2DTransformationMatrix());
    }

    [Test]
    public void EllipseRendererComponent_BoundingRectangle_ShouldReturnDefaultValue_WhenRenderingSystemIsNotAddedToSceneObservers()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();

        var entity = renderingScene.AddEllipse(50, 100, new Vector2(10, 20), 0, new Vector2(2, 2));
        var ellipseRendererComponent = entity.GetComponent<EllipseRendererComponent>();
        renderingScene.Scene.RemoveObserver(renderingSystem);

        // Act
        var actual = ellipseRendererComponent.BoundingRectangle;

        // Assert
        Assert.That(ellipseRendererComponent.IsManagedByRenderingSystem, Is.False);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle()));
    }

    [Test]
    public void EllipseRendererComponent_BoundingRectangle_ShouldReturnComputedValue_WhenRenderingSystemIsAddedToSceneObservers()
    {
        // Arrange
        var (_, renderingScene) = GetRenderingSystem();
        var entity = renderingScene.AddEllipse(50, 100, new Vector2(10, 20), 0, new Vector2(2, 2));
        var ellipseRendererComponent = entity.GetComponent<EllipseRendererComponent>();

        // Act
        var actual = ellipseRendererComponent.BoundingRectangle;

        // Assert
        Assert.That(ellipseRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle(10, 20, 200, 400)));
    }
}