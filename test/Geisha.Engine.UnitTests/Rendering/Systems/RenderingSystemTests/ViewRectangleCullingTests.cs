using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Systems.RenderingSystemTests;

[TestFixture]
public class ViewRectangleCullingTests : RenderingSystemTestsBase
{
    [TestCase(100, 100, false, 0, 0, 0, 1, 1, true)]
    [TestCase(100, 100, false, 1025, 525, 0, 1, 1, true)]
    [TestCase(100, 100, false, 1050, 550, 0, 1, 1, true)]
    [TestCase(100, 100, false, 1051, 551, 0, 1, 1, false)] // Too far on the top right
    [TestCase(100, 100, false, 1060, 0, 0, 1, 1, false)] // Too far on the right
    [TestCase(100, 100, false, 1060, 0, 45, 1, 1, true)] // Too far on the right - rotation makes it visible
    [TestCase(100, 100, false, 1060, 0, 0, 2, 1, true)] // Too far on the right - scale makes it visible
    [TestCase(100, 100, false, 0, 560, 0, 1, 1, false)] // Too far on the top
    [TestCase(100, 100, false, 0, 560, 45, 1, 1, true)] // Too far on the top - rotation makes it visible
    [TestCase(100, 100, false, 0, 560, 0, 1, 2, true)] // Too far on the top - scale makes it visible
    [TestCase(100, 100, true, 0, 0, 0, 1, 1, true)]
    [TestCase(100, 100, true, 1025, 525, 0, 1, 1, true)]
    [TestCase(100, 100, true, 1050, 550, 0, 1, 1, true)]
    [TestCase(100, 100, true, 1051, 551, 0, 1, 1, false)] // Too far on the top right
    [TestCase(100, 100, true, 1060, 0, 0, 1, 1, false)] // Too far on the right
    [TestCase(100, 100, true, 1060, 0, 45, 1, 1, true)] // Too far on the right - rotation makes it visible
    [TestCase(100, 100, true, 1060, 0, 0, 2, 1, true)] // Too far on the right - scale makes it visible
    [TestCase(100, 100, true, 0, 560, 0, 1, 1, false)] // Too far on the top
    [TestCase(100, 100, true, 0, 560, 45, 1, 1, true)] // Too far on the top - rotation makes it visible
    [TestCase(100, 100, true, 0, 560, 0, 1, 2, true)] // Too far on the top - scale makes it visible
    public void RenderScene_ShouldDrawRectangle_WhenRectangleIsInCameraView(double width, double height, bool transformParent, double tx, double ty, double r,
        double sx, double sy, bool expectedIsRendered)
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();

        var transform = new Transform2D(new Vector2(tx, ty), Angle.Deg2Rad(r), new Vector2(sx, sy));

        var parent = context.Scene.CreateEntity();
        parent.CreateComponent<Transform2DComponent>();

        var entity = context.AddRectangle(new Vector2(width, height), Vector2.Zero, 0, Vector2.One);
        entity.Parent = parent;

        if (transformParent)
        {
            parent.GetComponent<Transform2DComponent>().Transform = transform;
        }
        else
        {
            entity.GetComponent<Transform2DComponent>().Transform = transform;
        }

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        var expectedCalls = expectedIsRendered ? 1 : 0;
        RenderingContext2D.ReceivedWithAnyArgs(expectedCalls)
            .DrawRectangle(Arg.Any<AxisAlignedRectangle>(), Arg.Any<Color>(), Arg.Any<bool>(), Arg.Any<Matrix3x3>());

        var rectangleRenderer = entity.GetComponent<RectangleRendererComponent>();
        RenderingContext2D.Received(expectedCalls).DrawRectangle(new AxisAlignedRectangle(rectangleRenderer.Dimensions), rectangleRenderer.Color,
            rectangleRenderer.FillInterior, entity.GetComponent<Transform2DComponent>().ComputeWorldTransformMatrix());
    }

    [TestCase(50, 50, false, 0, 0, 0, 1, 1, true)]
    [TestCase(50, 50, false, 1025, 525, 0, 1, 1, true)]
    [TestCase(50, 50, false, 1050, 550, 0, 1, 1, true)]
    [TestCase(50, 50, false, 1051, 551, 0, 1, 1, false)] // Too far on the top right
    [TestCase(50, 100, false, 1060, 0, 0, 1, 1, false)] // Too far on the right
    [TestCase(50, 100, false, 1060, 0, 90, 1, 1, true)] // Too far on the right - rotation makes it visible
    [TestCase(50, 100, false, 1060, 0, 0, 2, 1, true)] // Too far on the right - scale makes it visible
    [TestCase(100, 50, false, 0, 560, 0, 1, 1, false)] // Too far on the top
    [TestCase(100, 50, false, 0, 560, 90, 1, 1, true)] // Too far on the top - rotation makes it visible
    [TestCase(100, 50, false, 0, 560, 0, 1, 2, true)] // Too far on the top - scale makes it visible
    [TestCase(50, 50, true, 0, 0, 0, 1, 1, true)]
    [TestCase(50, 50, true, 1025, 525, 0, 1, 1, true)]
    [TestCase(50, 50, true, 1050, 550, 0, 1, 1, true)]
    [TestCase(50, 50, true, 1051, 551, 0, 1, 1, false)] // Too far on the top right
    [TestCase(50, 100, true, 1060, 0, 0, 1, 1, false)] // Too far on the right
    [TestCase(50, 100, true, 1060, 0, 90, 1, 1, true)] // Too far on the right - rotation makes it visible
    [TestCase(50, 100, true, 1060, 0, 0, 2, 1, true)] // Too far on the right - scale makes it visible
    [TestCase(100, 50, true, 0, 560, 0, 1, 1, false)] // Too far on the top
    [TestCase(100, 50, true, 0, 560, 90, 1, 1, true)] // Too far on the top - rotation makes it visible
    [TestCase(100, 50, true, 0, 560, 0, 1, 2, true)] // Too far on the top - scale makes it visible
    public void RenderScene_ShouldDrawEllipse_WhenEllipseIsInCameraView(double radiusX, double radiusY, bool transformParent, double tx, double ty, double r,
        double sx, double sy, bool expectedIsRendered)
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();

        var transform = new Transform2D(new Vector2(tx, ty), Angle.Deg2Rad(r), new Vector2(sx, sy));

        var parent = context.Scene.CreateEntity();
        parent.CreateComponent<Transform2DComponent>();

        var entity = context.AddEllipse(radiusX, radiusY, Vector2.Zero, 0, Vector2.One);
        entity.Parent = parent;

        if (transformParent)
        {
            parent.GetComponent<Transform2DComponent>().Transform = transform;
        }
        else
        {
            entity.GetComponent<Transform2DComponent>().Transform = transform;
        }

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        var expectedCalls = expectedIsRendered ? 1 : 0;
        RenderingContext2D.ReceivedWithAnyArgs(expectedCalls)
            .DrawEllipse(Arg.Any<Ellipse>(), Arg.Any<Color>(), Arg.Any<bool>(), Arg.Any<Matrix3x3>());

        var ellipseRenderer = entity.GetComponent<EllipseRendererComponent>();
        RenderingContext2D.Received(expectedCalls).DrawEllipse(new Ellipse(ellipseRenderer.RadiusX, ellipseRenderer.RadiusY), ellipseRenderer.Color,
            ellipseRenderer.FillInterior, entity.GetComponent<Transform2DComponent>().ComputeWorldTransformMatrix());
    }

    [TestCase(100, 100, false, 0, 0, 0, 1, 1, true)]
    [TestCase(100, 100, false, 1025, 525, 0, 1, 1, true)]
    [TestCase(100, 100, false, 1050, 550, 0, 1, 1, true)]
    [TestCase(100, 100, false, 1051, 551, 0, 1, 1, false)] // Too far on the top right
    [TestCase(100, 100, false, 1060, 0, 0, 1, 1, false)] // Too far on the right
    [TestCase(100, 100, false, 1060, 0, 45, 1, 1, true)] // Too far on the right - rotation makes it visible
    [TestCase(100, 100, false, 1060, 0, 0, 2, 1, true)] // Too far on the right - scale makes it visible
    [TestCase(100, 100, false, 0, 560, 0, 1, 1, false)] // Too far on the top
    [TestCase(100, 100, false, 0, 560, 45, 1, 1, true)] // Too far on the top - rotation makes it visible
    [TestCase(100, 100, false, 0, 560, 0, 1, 2, true)] // Too far on the top - scale makes it visible
    [TestCase(100, 100, true, 0, 0, 0, 1, 1, true)]
    [TestCase(100, 100, true, 1025, 525, 0, 1, 1, true)]
    [TestCase(100, 100, true, 1050, 550, 0, 1, 1, true)]
    [TestCase(100, 100, true, 1051, 551, 0, 1, 1, false)] // Too far on the top right
    [TestCase(100, 100, true, 1060, 0, 0, 1, 1, false)] // Too far on the right
    [TestCase(100, 100, true, 1060, 0, 45, 1, 1, true)] // Too far on the right - rotation makes it visible
    [TestCase(100, 100, true, 1060, 0, 0, 2, 1, true)] // Too far on the right - scale makes it visible
    [TestCase(100, 100, true, 0, 560, 0, 1, 1, false)] // Too far on the top
    [TestCase(100, 100, true, 0, 560, 45, 1, 1, true)] // Too far on the top - rotation makes it visible
    [TestCase(100, 100, true, 0, 560, 0, 1, 2, true)] // Too far on the top - scale makes it visible
    public void RenderScene_ShouldDrawSprite_WhenSpriteIsInCameraView(double width, double height, bool transformParent, double tx, double ty, double r,
        double sx, double sy, bool expectedIsRendered)
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();

        var transform = new Transform2D(new Vector2(tx, ty), Angle.Deg2Rad(r), new Vector2(sx, sy));

        var parent = context.Scene.CreateEntity();
        parent.CreateComponent<Transform2DComponent>();

        var entity = context.AddSprite(new Vector2(width, height), Vector2.Zero, 0, Vector2.One);
        entity.Parent = parent;

        if (transformParent)
        {
            parent.GetComponent<Transform2DComponent>().Transform = transform;
        }
        else
        {
            entity.GetComponent<Transform2DComponent>().Transform = transform;
        }

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        var expectedCalls = expectedIsRendered ? 1 : 0;
        RenderingContext2D.ReceivedWithAnyArgs(expectedCalls).DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());

        RenderingContext2D.Received(expectedCalls)
            .DrawSprite(entity.GetSprite(), entity.GetComponent<Transform2DComponent>().ComputeWorldTransformMatrix(), entity.GetOpacity());
    }

    [TestCase(100, 100, false, 0, 0, 0, 1, 1, true)]
    [TestCase(100, 100, false, 1025, 525, 0, 1, 1, true)]
    [TestCase(100, 100, false, 1050, 550, 0, 1, 1, true)]
    [TestCase(100, 100, false, 1051, 551, 0, 1, 1, false)] // Too far on the top right
    [TestCase(100, 100, false, 1060, 0, 0, 1, 1, false)] // Too far on the right
    [TestCase(100, 100, false, 1060, 0, 45, 1, 1, true)] // Too far on the right - rotation makes it visible
    [TestCase(100, 100, false, 1060, 0, 0, 2, 1, true)] // Too far on the right - scale makes it visible
    [TestCase(100, 100, false, 0, 560, 0, 1, 1, false)] // Too far on the top
    [TestCase(100, 100, false, 0, 560, 45, 1, 1, true)] // Too far on the top - rotation makes it visible
    [TestCase(100, 100, false, 0, 560, 0, 1, 2, true)] // Too far on the top - scale makes it visible
    [TestCase(100, 100, true, 0, 0, 0, 1, 1, true)]
    [TestCase(100, 100, true, 1025, 525, 0, 1, 1, true)]
    [TestCase(100, 100, true, 1050, 550, 0, 1, 1, true)]
    [TestCase(100, 100, true, 1051, 551, 0, 1, 1, false)] // Too far on the top right
    [TestCase(100, 100, true, 1060, 0, 0, 1, 1, false)] // Too far on the right
    [TestCase(100, 100, true, 1060, 0, 45, 1, 1, true)] // Too far on the right - rotation makes it visible
    [TestCase(100, 100, true, 1060, 0, 0, 2, 1, true)] // Too far on the right - scale makes it visible
    [TestCase(100, 100, true, 0, 560, 0, 1, 1, false)] // Too far on the top
    [TestCase(100, 100, true, 0, 560, 45, 1, 1, true)] // Too far on the top - rotation makes it visible
    [TestCase(100, 100, true, 0, 560, 0, 1, 2, true)] // Too far on the top - scale makes it visible
    public void RenderScene_ShouldDrawTextLayout_WhenTextLayoutIsInCameraView(double width, double height, bool transformParent, double tx, double ty, double r,
        double sx, double sy, bool expectedIsRendered)
    {
        // Arrange
        const string text = "Sample text";
        const string fontFamilyName = "Calibri";
        var fontSize = FontSize.FromDips(20);
        var color = Color.Red;
        var maxWidth = width;
        var maxHeight = height;
        var pivot = new Vector2(width / 2, height / 2);
        const bool clipToLayoutBox = true;

        var textLayout = Substitute.For<ITextLayout>();
        textLayout.Text.Returns(text);
        textLayout.Metrics.Returns(new TextMetrics
        {
            Left = 0,
            Top = 0,
            Width = width,
            Height = height
        });
        RenderingContext2D.CreateTextLayout(text, fontFamilyName, fontSize, maxWidth, maxHeight).Returns(textLayout);

        var context = CreateRenderingTestContext();
        context.AddCamera();

        var transform = new Transform2D(new Vector2(tx, ty), Angle.Deg2Rad(r), new Vector2(sx, sy));

        var parent = context.Scene.CreateEntity();
        parent.CreateComponent<Transform2DComponent>();


        var (entity, textRendererComponent) = context.AddText(Vector2.Zero, 0, Vector2.One);
        entity.Parent = parent;

        textRendererComponent.FontFamilyName = fontFamilyName;
        textRendererComponent.FontSize = fontSize;
        textRendererComponent.Color = color;
        textRendererComponent.MaxWidth = maxWidth;
        textRendererComponent.MaxHeight = maxHeight;
        textRendererComponent.Pivot = pivot;
        textRendererComponent.ClipToLayoutBox = clipToLayoutBox;
        // Force recreation of ITextLayout
        textRendererComponent.Text = text;

        if (transformParent)
        {
            parent.GetComponent<Transform2DComponent>().Transform = transform;
        }
        else
        {
            entity.GetComponent<Transform2DComponent>().Transform = transform;
        }

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        var expectedCalls = expectedIsRendered ? 1 : 0;
        RenderingContext2D.ReceivedWithAnyArgs(expectedCalls)
            .DrawTextLayout(Arg.Any<ITextLayout>(), Arg.Any<Color>(), Arg.Any<Vector2>(), Arg.Any<Matrix3x3>(), Arg.Any<bool>());

        RenderingContext2D.Received(expectedCalls)
            .DrawTextLayout(textLayout, color, pivot, entity.GetComponent<Transform2DComponent>().ComputeWorldTransformMatrix(), clipToLayoutBox);
    }

    [TestCase(100, 100, false, 0, 0, 0, 1, 1, true)]
    [TestCase(100, 100, false, 1025, 525, 0, 1, 1, true)]
    [TestCase(100, 100, false, 1050, 550, 0, 1, 1, true)]
    [TestCase(100, 100, false, 1051, 551, 0, 1, 1, false)] // Too far on the top right
    [TestCase(100, 100, false, 1060, 0, 0, 1, 1, false)] // Too far on the right
    [TestCase(100, 100, false, 1060, 0, 45, 1, 1, true)] // Too far on the right - rotation makes it visible
    [TestCase(100, 100, false, 1060, 0, 0, 2, 1, true)] // Too far on the right - scale makes it visible
    [TestCase(100, 100, false, 0, 560, 0, 1, 1, false)] // Too far on the top
    [TestCase(100, 100, false, 0, 560, 45, 1, 1, true)] // Too far on the top - rotation makes it visible
    [TestCase(100, 100, false, 0, 560, 0, 1, 2, true)] // Too far on the top - scale makes it visible
    [TestCase(100, 100, true, 0, 0, 0, 1, 1, true)]
    [TestCase(100, 100, true, 1025, 525, 0, 1, 1, true)]
    [TestCase(100, 100, true, 1050, 550, 0, 1, 1, true)]
    [TestCase(100, 100, true, 1051, 551, 0, 1, 1, false)] // Too far on the top right
    [TestCase(100, 100, true, 1060, 0, 0, 1, 1, false)] // Too far on the right
    [TestCase(100, 100, true, 1060, 0, 45, 1, 1, true)] // Too far on the right - rotation makes it visible
    [TestCase(100, 100, true, 1060, 0, 0, 2, 1, true)] // Too far on the right - scale makes it visible
    [TestCase(100, 100, true, 0, 560, 0, 1, 1, false)] // Too far on the top
    [TestCase(100, 100, true, 0, 560, 45, 1, 1, true)] // Too far on the top - rotation makes it visible
    [TestCase(100, 100, true, 0, 560, 0, 1, 2, true)] // Too far on the top - scale makes it visible
    public void RenderScene_ShouldDrawRectangle_WhenRectangleIsInCameraViewAndCameraIsTransformed(double width, double height, bool transformParent, double tx,
        double ty, double r, double sx, double sy, bool expectedIsRendered)
    {
        // Arrange
        var context = CreateRenderingTestContext();
        var camera = context.AddCamera(Vector2.Zero, 0, Vector2.One);
        var cameraComponent = camera.GetComponent<CameraComponent>();
        cameraComponent.ViewRectangle = new Vector2(width, height);

        var transform = new Transform2D(new Vector2(tx, ty), Angle.Deg2Rad(r), new Vector2(sx, sy));

        var parent = context.Scene.CreateEntity();
        parent.CreateComponent<Transform2DComponent>();

        camera.Parent = parent;

        if (transformParent)
        {
            parent.GetComponent<Transform2DComponent>().Transform = transform;
        }
        else
        {
            camera.GetComponent<Transform2DComponent>().Transform = transform;
        }

        var rectangle = context.AddRectangle(new Vector2(ScreenWidth, ScreenHeight), Vector2.Zero, 0, Vector2.One);

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        var expectedCalls = expectedIsRendered ? 1 : 0;
        RenderingContext2D.ReceivedWithAnyArgs(expectedCalls)
            .DrawRectangle(Arg.Any<AxisAlignedRectangle>(), Arg.Any<Color>(), Arg.Any<bool>(), Arg.Any<Matrix3x3>());

        var rectangleRenderer = rectangle.GetComponent<RectangleRendererComponent>();
        RenderingContext2D.Received(expectedCalls).DrawRectangle(new AxisAlignedRectangle(rectangleRenderer.Dimensions), rectangleRenderer.Color,
            rectangleRenderer.FillInterior, cameraComponent.CreateViewMatrixScaledToScreen() * rectangle.GetTransformMatrix());
    }
}