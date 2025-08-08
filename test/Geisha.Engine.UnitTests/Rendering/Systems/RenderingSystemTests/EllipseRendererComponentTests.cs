using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
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

        var (renderingSystem, renderingScene) = GetRenderingSystem(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, sortingLayerName } });

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
        Assert.That(ellipseRendererComponent.IsManagedByRenderingSystem, Is.True);

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

        var (renderingSystem, renderingScene) = GetRenderingSystem(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, sortingLayerName } });
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
        Assert.That(ellipseRendererComponent.IsManagedByRenderingSystem, Is.False);

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
    public void RenderScene_ShouldDrawEllipse_TransformedWithParentTransform_WhenEntityHasParentWithTransform2DComponent()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();

        var parent = renderingScene.Scene.CreateEntity();
        var parentTransform = parent.CreateComponent<Transform2DComponent>();
        parentTransform.Translation = new Vector2(10, 20);
        parentTransform.Rotation = 30;
        parentTransform.Scale = new Vector2(2, 4);

        var child = renderingScene.AddEllipse();
        child.Parent = parent;

        var expectedTransform = parentTransform.ToMatrix() * child.Get2DTransformationMatrix();

        // Act
        renderingSystem.RenderScene();

        // Assert
        var ellipseRenderer = child.GetComponent<EllipseRendererComponent>();
        RenderingContext2D.Received(1).DrawEllipse(new Ellipse(ellipseRenderer.RadiusX, ellipseRenderer.RadiusY), ellipseRenderer.Color,
            ellipseRenderer.FillInterior, expectedTransform);
    }

    [Test]
    public void RenderScene_ShouldDrawEllipse_WhenTransformIsInterpolated()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var entity = renderingScene.AddEllipse(50, 100, new Vector2(10, 20), 30, new Vector2(1, 2));
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
        var ellipseRenderer = entity.GetComponent<EllipseRendererComponent>();
        RenderingContext2D.Received(1).DrawEllipse(new Ellipse(ellipseRenderer.RadiusX, ellipseRenderer.RadiusY), ellipseRenderer.Color,
            ellipseRenderer.FillInterior, transform2DComponent.InterpolatedTransform.ToMatrix());
    }

    [Test]
    public void RenderScene_ShouldDrawEllipse_TransformedWithParentTransform_WhenTransformIsInterpolated()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();

        var parent = renderingScene.Scene.CreateEntity();
        var parentTransform = parent.CreateComponent<Transform2DComponent>();
        parentTransform.IsInterpolated = true;
        parentTransform.Translation = new Vector2(10, 20);
        parentTransform.Rotation = 2;
        parentTransform.Scale = new Vector2(1, 2);

        var child = renderingScene.AddEllipse(50, 100, new Vector2(20, 30), 3, new Vector2(2, 3));
        child.Parent = parent;
        var childTransform = child.GetComponent<Transform2DComponent>();
        childTransform.IsInterpolated = true;

        renderingScene.TransformInterpolationSystem.SnapshotTransforms();

        parentTransform.Translation = new Vector2(20, 40);
        parentTransform.Rotation = 4;
        parentTransform.Scale = new Vector2(2, 4);

        childTransform.Translation = new Vector2(30, 60);
        childTransform.Rotation = 6;
        childTransform.Scale = new Vector2(3, 6);

        renderingScene.TransformInterpolationSystem.SnapshotTransforms();

        renderingScene.TransformInterpolationSystem.InterpolateTransforms(0.5);

        // Assume
        Assert.That(parentTransform.InterpolatedTransform, Is.Not.EqualTo(parentTransform.Transform));
        Assert.That(childTransform.InterpolatedTransform, Is.Not.EqualTo(childTransform.Transform));

        var expectedTransform = parentTransform.InterpolatedTransform.ToMatrix() * childTransform.InterpolatedTransform.ToMatrix();

        // Act
        renderingSystem.RenderScene();

        // Assert
        var ellipseRenderer = child.GetComponent<EllipseRendererComponent>();
        RenderingContext2D.Received(1).DrawEllipse(new Ellipse(ellipseRenderer.RadiusX, ellipseRenderer.RadiusY), ellipseRenderer.Color,
            ellipseRenderer.FillInterior, expectedTransform);
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

    [Test]
    public void EllipseRendererComponent_BoundingRectangle_ShouldReturnComputedValue_WhenTransformIsInterpolated()
    {
        // Arrange
        var (_, renderingScene) = GetRenderingSystem();
        var entity = renderingScene.AddEllipse(50, 100, new Vector2(10, 20), 0, new Vector2(2, 2));
        var ellipseRendererComponent = entity.GetComponent<EllipseRendererComponent>();
        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        transform2DComponent.IsInterpolated = true;

        renderingScene.TransformInterpolationSystem.SnapshotTransforms();

        transform2DComponent.Translation = new Vector2(20, 40);
        transform2DComponent.Scale = new Vector2(4, 4);

        renderingScene.TransformInterpolationSystem.SnapshotTransforms();

        renderingScene.TransformInterpolationSystem.InterpolateTransforms(0.5);

        // Assume
        Assert.That(transform2DComponent.InterpolatedTransform, Is.Not.EqualTo(transform2DComponent.Transform));

        // Act
        var actual = ellipseRendererComponent.BoundingRectangle;

        // Assert
        Assert.That(ellipseRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle(15, 30, 300, 600)));
    }
}