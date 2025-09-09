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

        var context = CreateRenderingTestContext(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, sortingLayerName } });

        var entity = context.AddEllipse();
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
        context.Scene.RemoveObserver(context.RenderingSystem);

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

        var context = CreateRenderingTestContext(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, sortingLayerName } });
        context.Scene.RemoveObserver(context.RenderingSystem);

        var entity = context.AddEllipse();
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
        context.Scene.AddObserver(context.RenderingSystem);

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
        var context = CreateRenderingTestContext();
        context.AddCamera();
        var entity = context.AddEllipse();

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        var ellipseRenderer = entity.GetComponent<EllipseRendererComponent>();
        RenderingContext2D.Received(1).DrawEllipse(new Ellipse(ellipseRenderer.RadiusX, ellipseRenderer.RadiusY), ellipseRenderer.Color,
            ellipseRenderer.FillInterior, entity.GetTransformMatrix());
    }

    [Test]
    public void RenderScene_ShouldDrawEllipse_TransformedWithParentTransform_WhenEntityHasParentWithTransform2DComponent()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();

        var parent = context.Scene.CreateEntity();
        var parentTransform = parent.CreateComponent<Transform2DComponent>();
        parentTransform.Translation = new Vector2(10, 20);
        parentTransform.Rotation = 30;
        parentTransform.Scale = new Vector2(2, 4);

        var child = context.AddEllipse();
        child.Parent = parent;

        var expectedTransform = parentTransform.ToMatrix() * child.GetTransformMatrix();

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        var ellipseRenderer = child.GetComponent<EllipseRendererComponent>();
        RenderingContext2D.Received(1).DrawEllipse(new Ellipse(ellipseRenderer.RadiusX, ellipseRenderer.RadiusY), ellipseRenderer.Color,
            ellipseRenderer.FillInterior, expectedTransform);
    }

    [Test]
    public void RenderScene_ShouldDrawEllipse_WhenTransformIsInterpolated()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();
        var entity = context.AddEllipse(50, 100, new Vector2(10, 20), 30, new Vector2(1, 2));
        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        transform2DComponent.IsInterpolated = true;

        context.TransformInterpolationSystem.SnapshotTransforms();

        transform2DComponent.Translation = new Vector2(20, 40);
        transform2DComponent.Rotation = 60;
        transform2DComponent.Scale = new Vector2(2, 4);

        context.TransformInterpolationSystem.SnapshotTransforms();

        context.TransformInterpolationSystem.InterpolateTransforms(0.5);

        // Assume
        Assert.That(transform2DComponent.InterpolatedTransform, Is.Not.EqualTo(transform2DComponent.Transform));

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        var ellipseRenderer = entity.GetComponent<EllipseRendererComponent>();
        RenderingContext2D.Received(1).DrawEllipse(new Ellipse(ellipseRenderer.RadiusX, ellipseRenderer.RadiusY), ellipseRenderer.Color,
            ellipseRenderer.FillInterior, transform2DComponent.InterpolatedTransform.ToMatrix());
    }

    [Test]
    public void RenderScene_ShouldDrawEllipse_TransformedWithParentTransform_WhenTransformIsInterpolated()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();

        var parent = context.Scene.CreateEntity();
        var parentTransform = parent.CreateComponent<Transform2DComponent>();
        parentTransform.IsInterpolated = true;
        parentTransform.Translation = new Vector2(10, 20);
        parentTransform.Rotation = 2;
        parentTransform.Scale = new Vector2(1, 2);

        var child = context.AddEllipse(50, 100, new Vector2(20, 30), 3, new Vector2(2, 3));
        child.Parent = parent;
        var childTransform = child.GetComponent<Transform2DComponent>();
        childTransform.IsInterpolated = true;

        context.TransformInterpolationSystem.SnapshotTransforms();

        parentTransform.Translation = new Vector2(20, 40);
        parentTransform.Rotation = 4;
        parentTransform.Scale = new Vector2(2, 4);

        childTransform.Translation = new Vector2(30, 60);
        childTransform.Rotation = 6;
        childTransform.Scale = new Vector2(3, 6);

        context.TransformInterpolationSystem.SnapshotTransforms();

        context.TransformInterpolationSystem.InterpolateTransforms(0.5);

        // Assume
        Assert.That(parentTransform.InterpolatedTransform, Is.Not.EqualTo(parentTransform.Transform));
        Assert.That(childTransform.InterpolatedTransform, Is.Not.EqualTo(childTransform.Transform));

        var expectedTransform = parentTransform.InterpolatedTransform.ToMatrix() * childTransform.InterpolatedTransform.ToMatrix();

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        var ellipseRenderer = child.GetComponent<EllipseRendererComponent>();
        RenderingContext2D.Received(1).DrawEllipse(new Ellipse(ellipseRenderer.RadiusX, ellipseRenderer.RadiusY), ellipseRenderer.Color,
            ellipseRenderer.FillInterior, expectedTransform);
    }

    [Test]
    public void EllipseRendererComponent_BoundingRectangle_ShouldReturnDefaultValue_WhenRenderingSystemIsNotAddedToSceneObservers()
    {
        // Arrange
        var context = CreateRenderingTestContext();

        var entity = context.AddEllipse(50, 100, new Vector2(10, 20), 0, new Vector2(2, 2));
        var ellipseRendererComponent = entity.GetComponent<EllipseRendererComponent>();
        context.Scene.RemoveObserver(context.RenderingSystem);

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
        var context = CreateRenderingTestContext();
        var entity = context.AddEllipse(50, 100, new Vector2(10, 20), 0, new Vector2(2, 2));
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
        var context = CreateRenderingTestContext();
        var entity = context.AddEllipse(50, 100, new Vector2(10, 20), 0, new Vector2(2, 2));
        var ellipseRendererComponent = entity.GetComponent<EllipseRendererComponent>();
        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        transform2DComponent.IsInterpolated = true;

        context.TransformInterpolationSystem.SnapshotTransforms();

        transform2DComponent.Translation = new Vector2(20, 40);
        transform2DComponent.Scale = new Vector2(4, 4);

        context.TransformInterpolationSystem.SnapshotTransforms();

        context.TransformInterpolationSystem.InterpolateTransforms(0.5);

        // Assume
        Assert.That(transform2DComponent.InterpolatedTransform, Is.Not.EqualTo(transform2DComponent.Transform));

        // Act
        var actual = ellipseRendererComponent.BoundingRectangle;

        // Assert
        Assert.That(ellipseRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle(15, 30, 300, 600)));
    }
}