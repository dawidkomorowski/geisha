using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Systems.RenderingSystemTests;

[TestFixture]
public class RectangleRendererComponentTests : RenderingSystemTestsBase
{
    [Test]
    public void RenderingSystem_ShouldKeepRectangleRendererComponentState_WhenRenderingSystemIsRemovedFromSceneObserversOfScene()
    {
        // Arrange
        // Renderer2DComponent
        const bool visible = false;
        const string sortingLayerName = "some sorting layer";
        const int orderInLayer = 12;
        // RectangleRendererComponent
        var dimensions = new Vector2(1, 2);
        var color = Color.Red;
        const bool fillInterior = true;

        var (renderingSystem, renderingScene) = GetRenderingSystem(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, sortingLayerName } });

        var entity = renderingScene.AddRectangle();
        var rectangleRendererComponent = entity.GetComponent<RectangleRendererComponent>();

        // Renderer2DComponent
        rectangleRendererComponent.Visible = visible;
        rectangleRendererComponent.SortingLayerName = sortingLayerName;
        rectangleRendererComponent.OrderInLayer = orderInLayer;
        // RectangleRendererComponent
        rectangleRendererComponent.Dimensions = dimensions;
        rectangleRendererComponent.Color = color;
        rectangleRendererComponent.FillInterior = fillInterior;

        // Assume
        Assert.That(rectangleRendererComponent.IsManagedByRenderingSystem, Is.True);

        // Act
        renderingScene.Scene.RemoveObserver(renderingSystem);

        // Assert
        // Renderer2DComponent
        Assert.That(rectangleRendererComponent.IsManagedByRenderingSystem, Is.False);
        Assert.That(rectangleRendererComponent.Visible, Is.EqualTo(visible));
        Assert.That(rectangleRendererComponent.SortingLayerName, Is.EqualTo(sortingLayerName));
        Assert.That(rectangleRendererComponent.OrderInLayer, Is.EqualTo(orderInLayer));
        // RectangleRendererComponent
        Assert.That(rectangleRendererComponent.Dimensions, Is.EqualTo(dimensions));
        Assert.That(rectangleRendererComponent.Color, Is.EqualTo(color));
        Assert.That(rectangleRendererComponent.FillInterior, Is.EqualTo(fillInterior));
    }

    [Test]
    public void RenderingSystem_ShouldKeepRectangleRendererComponentState_WhenRenderingSystemIsAddedToSceneObserversOfScene()
    {
        // Arrange
        // Renderer2DComponent
        const bool visible = false;
        const string sortingLayerName = "some sorting layer";
        const int orderInLayer = 12;
        // RectangleRendererComponent
        var dimensions = new Vector2(1, 2);
        var color = Color.Red;
        const bool fillInterior = true;

        var (renderingSystem, renderingScene) = GetRenderingSystem(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, sortingLayerName } });
        renderingScene.Scene.RemoveObserver(renderingSystem);

        var entity = renderingScene.AddRectangle();
        var rectangleRendererComponent = entity.GetComponent<RectangleRendererComponent>();

        // Renderer2DComponent
        rectangleRendererComponent.Visible = visible;
        rectangleRendererComponent.SortingLayerName = sortingLayerName;
        rectangleRendererComponent.OrderInLayer = orderInLayer;
        // RectangleRendererComponent
        rectangleRendererComponent.Dimensions = dimensions;
        rectangleRendererComponent.Color = color;
        rectangleRendererComponent.FillInterior = fillInterior;

        // Assume
        Assert.That(rectangleRendererComponent.IsManagedByRenderingSystem, Is.False);

        // Act
        renderingScene.Scene.AddObserver(renderingSystem);

        // Assert
        // Renderer2DComponent
        Assert.That(rectangleRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(rectangleRendererComponent.Visible, Is.EqualTo(visible));
        Assert.That(rectangleRendererComponent.SortingLayerName, Is.EqualTo(sortingLayerName));
        Assert.That(rectangleRendererComponent.OrderInLayer, Is.EqualTo(orderInLayer));
        // RectangleRendererComponent
        Assert.That(rectangleRendererComponent.Dimensions, Is.EqualTo(dimensions));
        Assert.That(rectangleRendererComponent.Color, Is.EqualTo(color));
        Assert.That(rectangleRendererComponent.FillInterior, Is.EqualTo(fillInterior));
    }

    [Test]
    public void RenderScene_ShouldDrawRectangle_WhenSceneContainsEntityWithRectangleRendererAndTransform()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var entity = renderingScene.AddRectangle();

        // Act
        renderingSystem.RenderScene();

        // Assert
        var rectangleRenderer = entity.GetComponent<RectangleRendererComponent>();
        RenderingContext2D.Received(1).DrawRectangle(new AxisAlignedRectangle(rectangleRenderer.Dimensions), rectangleRenderer.Color,
            rectangleRenderer.FillInterior, entity.Get2DTransformationMatrix());
    }

    [Test]
    public void RenderScene_ShouldDrawRectangle_WhenTransformIsInterpolated()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var entity = renderingScene.AddRectangle(new Vector2(100, 200), new Vector2(10, 20), 30, new Vector2(1, 2));
        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        transform2DComponent.IsInterpolated = true;

        renderingScene.TransformInterpolationSystem.SnapshotTransforms();

        transform2DComponent.Translation = new Vector2(20, 40);
        transform2DComponent.Rotation = 60;
        transform2DComponent.Scale = new Vector2(2, 4);

        renderingScene.TransformInterpolationSystem.SnapshotTransforms();

        renderingScene.TransformInterpolationSystem.InterpolateTransforms(0.5);

        // Assume
        Assert.That(transform2DComponent.InterpolatedTransform, Is.Not.EqualTo(transform2DComponent.Transform));

        // Act
        renderingSystem.RenderScene();

        // Assert
        var rectangleRenderer = entity.GetComponent<RectangleRendererComponent>();
        RenderingContext2D.Received(1).DrawRectangle(new AxisAlignedRectangle(rectangleRenderer.Dimensions), rectangleRenderer.Color,
            rectangleRenderer.FillInterior, transform2DComponent.InterpolatedTransform.ToMatrix());
    }

    // TODO Add more tests for transform hierarchy. Currently, it is tested in CommonTests for only one type of render node.

    [Test]
    public void RectangleRendererComponent_BoundingRectangle_ShouldReturnDefaultValue_WhenRenderingSystemIsNotAddedToSceneObservers()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();

        var entity = renderingScene.AddRectangle(new Vector2(100, 200), new Vector2(10, 20), 0, new Vector2(2, 2));
        var rectangleRendererComponent = entity.GetComponent<RectangleRendererComponent>();
        renderingScene.Scene.RemoveObserver(renderingSystem);

        // Act
        var actual = rectangleRendererComponent.BoundingRectangle;

        // Assert
        Assert.That(rectangleRendererComponent.IsManagedByRenderingSystem, Is.False);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle()));
    }

    [Test]
    public void RectangleRendererComponent_BoundingRectangle_ShouldReturnComputedValue_WhenRenderingSystemIsAddedToSceneObservers()
    {
        // Arrange
        var (_, renderingScene) = GetRenderingSystem();
        var entity = renderingScene.AddRectangle(new Vector2(100, 200), new Vector2(10, 20), 0, new Vector2(2, 2));
        var rectangleRendererComponent = entity.GetComponent<RectangleRendererComponent>();

        // Act
        var actual = rectangleRendererComponent.BoundingRectangle;

        // Assert
        Assert.That(rectangleRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle(10, 20, 200, 400)));
    }
}