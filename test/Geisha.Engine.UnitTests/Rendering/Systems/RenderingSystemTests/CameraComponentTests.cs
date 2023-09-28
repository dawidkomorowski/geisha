using System;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Systems.RenderingSystemTests;

[TestFixture]
public class CameraComponentTests : RenderingSystemTestsBase
{
    [Test]
    public void RenderingSystem_ShouldKeepCameraComponentState_WhenRenderingSystemIsRemovedFromSceneObserversOfScene()
    {
        // Arrange
        const AspectRatioBehavior aspectRatioBehavior = AspectRatioBehavior.Underscan;
        var viewRectangle = new Vector2(16, 9);

        var (renderingSystem, renderingScene) = GetRenderingSystem();

        var entity = renderingScene.AddCamera();
        var cameraComponent = entity.GetComponent<CameraComponent>();

        cameraComponent.AspectRatioBehavior = aspectRatioBehavior;
        cameraComponent.ViewRectangle = viewRectangle;

        // Assume
        Assume.That(cameraComponent.IsManagedByRenderingSystem, Is.True);

        // Act
        renderingScene.Scene.RemoveObserver(renderingSystem);

        // Assert
        Assert.That(cameraComponent.IsManagedByRenderingSystem, Is.False);
        Assert.That(cameraComponent.AspectRatioBehavior, Is.EqualTo(aspectRatioBehavior));
        Assert.That(cameraComponent.ViewRectangle, Is.EqualTo(viewRectangle));
    }

    [Test]
    public void RenderingSystem_ShouldKeepCameraComponentState_WhenRenderingSystemIsAddedToSceneObserversOfScene()
    {
        // Arrange
        const AspectRatioBehavior aspectRatioBehavior = AspectRatioBehavior.Underscan;
        var viewRectangle = new Vector2(16, 9);

        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.Scene.RemoveObserver(renderingSystem);

        var entity = renderingScene.AddCamera();
        var cameraComponent = entity.GetComponent<CameraComponent>();

        cameraComponent.AspectRatioBehavior = aspectRatioBehavior;
        cameraComponent.ViewRectangle = viewRectangle;

        // Assume
        Assume.That(cameraComponent.IsManagedByRenderingSystem, Is.False);

        // Act
        renderingScene.Scene.AddObserver(renderingSystem);

        // Assert
        Assert.That(cameraComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(cameraComponent.AspectRatioBehavior, Is.EqualTo(aspectRatioBehavior));
        Assert.That(cameraComponent.ViewRectangle, Is.EqualTo(viewRectangle));
    }

    [Test]
    public void RenderScene_ShouldPerformCameraTransformationOnEntity_WhenSceneContainsEntityAndCamera()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();

        renderingScene.AddCamera(new Vector2(10, -10), 0, Vector2.One);
        var entity = renderingScene.AddSpriteWithDefaultTransform();

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(entity.GetSprite(), Matrix3x3.CreateTranslation(new Vector2(-10, 10)), entity.GetOpacity());
    }

    [Test]
    public void RenderScene_ShouldSetScreenWidthAndScreenHeightOnCameraComponent()
    {
        // Arrange
        const int screenWidth = 123;
        const int screenHeight = 456;
        RenderingContext2D.ScreenWidth.Returns(screenWidth);
        RenderingContext2D.ScreenHeight.Returns(screenHeight);

        var (renderingSystem, renderingScene) = GetRenderingSystem();
        var cameraEntity = renderingScene.AddCamera();

        // Act
        renderingSystem.RenderScene();

        // Assert
        var cameraComponent = cameraEntity.GetComponent<CameraComponent>();
        Assert.That(cameraComponent.ScreenWidth, Is.EqualTo(screenWidth));
        Assert.That(cameraComponent.ScreenHeight, Is.EqualTo(screenHeight));
    }

    [Test]
    public void RenderScene_ShouldApplyViewRectangleOfCamera_WhenSceneContainsEntityAndCamera()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();

        var cameraEntity = renderingScene.AddCamera(new Vector2(10, -10), 0, Vector2.One);
        var camera = cameraEntity.GetComponent<CameraComponent>();

        // Camera view rectangle is twice the screen resolution
        camera.ViewRectangle = new Vector2(ScreenWidth * 2, ScreenHeight * 2);

        var entity = renderingScene.AddSpriteWithDefaultTransform();

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(entity.GetSprite(),
            // Sprite transform is half the scale and translation due to camera view rectangle 
            new Matrix3x3(m11: 0.5, m12: 0, m13: -5, m21: 0, m22: 0.5, m23: 5, m31: 0, m32: 0, m33: 1),
            entity.GetOpacity());
    }

    [Test]
    public void RenderScene_ShouldApplyViewRectangleOfCameraWithOverscanMatchedByHeight_WhenCameraAndScreenAspectRatioDiffers()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();

        var cameraEntity = renderingScene.AddCamera(new Vector2(10, -10), 0, Vector2.One);
        var camera = cameraEntity.GetComponent<CameraComponent>();
        camera.AspectRatioBehavior = AspectRatioBehavior.Overscan;

        // Camera view rectangle 4xScreenWidth and 2xScreenHeight
        // Camera view rectangle is 4:1 ratio while screen is 2:1 ratio
        camera.ViewRectangle = new Vector2(ScreenWidth * 4, ScreenHeight * 2);

        var entity = renderingScene.AddSpriteWithDefaultTransform();

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(entity.GetSprite(),
            // Sprite transform is half the scale and translation due to camera view rectangle being scaled by height to match
            new Matrix3x3(m11: 0.5, m12: 0, m13: -5, m21: 0, m22: 0.5, m23: 5, m31: 0, m32: 0, m33: 1),
            entity.GetOpacity());
    }

    [Test]
    public void RenderScene_ShouldApplyViewRectangleOfCameraWithOverscanMatchedByWidth_WhenCameraAndScreenAspectRatioDiffers()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();

        var cameraEntity = renderingScene.AddCamera(new Vector2(10, -10), 0, Vector2.One);
        var camera = cameraEntity.GetComponent<CameraComponent>();
        camera.AspectRatioBehavior = AspectRatioBehavior.Overscan;

        // Camera view rectangle 2xScreenWidth and 4xScreenHeight
        // Camera view rectangle is 1:1 ratio while screen is 2:1 ratio
        camera.ViewRectangle = new Vector2(ScreenWidth * 2, ScreenHeight * 4);

        var entity = renderingScene.AddSpriteWithDefaultTransform();

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(entity.GetSprite(),
            // Sprite transform is half the scale and translation due to camera view rectangle being scaled by width to match
            new Matrix3x3(m11: 0.5, m12: 0, m13: -5, m21: 0, m22: 0.5, m23: 5, m31: 0, m32: 0, m33: 1),
            entity.GetOpacity());
    }

    [Test]
    public void RenderScene_ShouldApplyViewRectangleOfCameraWithUnderscanMatchedByHeight_WhenCameraAndScreenAspectRatioDiffers()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();

        var cameraEntity = renderingScene.AddCamera(new Vector2(10, -10), 0, Vector2.One);
        var camera = cameraEntity.GetComponent<CameraComponent>();
        camera.AspectRatioBehavior = AspectRatioBehavior.Underscan;

        // Camera view rectangle 1xScreenWidth and 2xScreenHeight
        // Camera view rectangle is 1:1 ratio while screen is 2:1 ratio
        camera.ViewRectangle = new Vector2(ScreenWidth, ScreenHeight * 2);

        var entity = renderingScene.AddSpriteWithDefaultTransform();

        // Act
        renderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.Clear(Color.White);
            RenderingContext2D.Clear(Color.Black);
            RenderingContext2D.SetClippingRectangle(new AxisAlignedRectangle(ScreenHeight, ScreenHeight));
            RenderingContext2D.Clear(Color.White);
            RenderingContext2D.Received(1).DrawSprite(entity.GetSprite(),
                // Sprite transform is half the scale and translation due to camera view rectangle being scaled by height to match
                new Matrix3x3(m11: 0.5, m12: 0, m13: -5, m21: 0, m22: 0.5, m23: 5, m31: 0, m32: 0, m33: 1),
                entity.GetOpacity());
            RenderingContext2D.ClearClipping();
        });
    }

    [Test]
    public void RenderScene_ShouldApplyViewRectangleOfCameraWithUnderscanMatchedByWidth_WhenCameraAndScreenAspectRatioDiffers()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();

        var cameraEntity = renderingScene.AddCamera(new Vector2(10, -10), 0, Vector2.One);
        var camera = cameraEntity.GetComponent<CameraComponent>();
        camera.AspectRatioBehavior = AspectRatioBehavior.Underscan;

        // Camera view rectangle 2xScreenWidth and 1xScreenHeight
        // Camera view rectangle is 4:1 ratio while screen is 2:1 ratio
        camera.ViewRectangle = new Vector2(ScreenWidth * 2, ScreenHeight);

        var entity = renderingScene.AddSpriteWithDefaultTransform();

        // Act
        renderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.Clear(Color.White);
            RenderingContext2D.Clear(Color.Black);
            RenderingContext2D.SetClippingRectangle(new AxisAlignedRectangle(ScreenWidth, ScreenHeight / 2d));
            RenderingContext2D.Clear(Color.White);
            RenderingContext2D.Received(1).DrawSprite(entity.GetSprite(),
                // Sprite transform is half the scale and translation due to camera view rectangle being scaled by width to match
                new Matrix3x3(m11: 0.5, m12: 0, m13: -5, m21: 0, m22: 0.5, m23: 5, m31: 0, m32: 0, m33: 1),
                entity.GetOpacity());
            RenderingContext2D.ClearClipping();
        });
    }

    [Test]
    public void CameraComponent_ScreenWidth_And_ScreenHeight_ShouldReturnDefaultValue_WhenRenderingSystemIsNotAddedToSceneObservers()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();

        var entity = renderingScene.AddCamera();
        var cameraComponent = entity.GetComponent<CameraComponent>();
        renderingScene.Scene.RemoveObserver(renderingSystem);

        // Act
        var screenWidth = cameraComponent.ScreenWidth;
        var screenHeight = cameraComponent.ScreenHeight;

        // Assert
        Assert.That(cameraComponent.IsManagedByRenderingSystem, Is.False);
        Assert.That(screenWidth, Is.Zero);
        Assert.That(screenHeight, Is.Zero);
    }

    [Test]
    public void CameraComponent_ScreenWidth_And_ScreenHeight_ShouldReturnActualValue_WhenRenderingSystemIsAddedToSceneObservers()
    {
        // Arrange
        RenderingContext2D.ScreenWidth.Returns(1920);
        RenderingContext2D.ScreenHeight.Returns(1080);

        var (renderingSystem, renderingScene) = GetRenderingSystem();

        var entity = renderingScene.AddCamera();
        var cameraComponent = entity.GetComponent<CameraComponent>();

        // Act
        var screenWidth = cameraComponent.ScreenWidth;
        var screenHeight = cameraComponent.ScreenHeight;

        // Assert
        Assert.That(cameraComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(screenWidth, Is.EqualTo(1920));
        Assert.That(screenHeight, Is.EqualTo(1080));
    }

    [Test]
    public void CameraComponent_ScreenPointToWorld2DPoint_ShouldReturnDefaultValue_WhenRenderingSystemIsNotAddedToSceneObservers()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();

        var entity = renderingScene.AddCamera(Vector2.Zero, 0, Vector2.One);
        var cameraComponent = entity.GetComponent<CameraComponent>();
        renderingScene.Scene.RemoveObserver(renderingSystem);

        // Act
        var actual = cameraComponent.ScreenPointToWorld2DPoint(new Vector2(10, 20));

        // Assert
        Assert.That(cameraComponent.IsManagedByRenderingSystem, Is.False);
        Assert.That(actual, Is.EqualTo(new Vector2()));
    }

    [TestCase(0, 0, 0, 1, 1, 1920, 1080, AspectRatioBehavior.Overscan, 0, 0, -960, 540)]
    [TestCase(0, 0, 0, 1, 1, 1920, 1080, AspectRatioBehavior.Overscan, 960, 540, 0, 0)]
    [TestCase(0, 0, 0, 1, 1, 1920, 1080, AspectRatioBehavior.Overscan, 1920, 1080, 960, -540)]
    [TestCase(0, 0, 0, 1, 1, 1920, 1080, AspectRatioBehavior.Overscan, 200, 100, -760, 440)]
    [TestCase(200, 100, 0, 1, 1, 1920, 1080, AspectRatioBehavior.Overscan, 200, 100, -560, 540)]
    [TestCase(0, 0, 0, 2, 2, 1920, 1080, AspectRatioBehavior.Overscan, 200, 100, -1520, 880)]
    [TestCase(0, 0, Math.PI / 2, 1, 1, 1920, 1080, AspectRatioBehavior.Overscan, 200, 100, -440, -760)]
    [TestCase(0, 0, 0, 1, 1, 192, 108, AspectRatioBehavior.Overscan, 200, 100, -76, 44)]
    [TestCase(0, 0, 0, 1, 1, 3840, 1080, AspectRatioBehavior.Overscan, 200, 100, -760, 440)]
    [TestCase(0, 0, 0, 1, 1, 1920, 2160, AspectRatioBehavior.Overscan, 200, 100, -760, 440)]
    [TestCase(0, 0, 0, 1, 1, 192, 108, AspectRatioBehavior.Underscan, 200, 100, -76, 44)]
    [TestCase(0, 0, 0, 1, 1, 3840, 1080, AspectRatioBehavior.Underscan, 200, 100, -1520, 880)]
    [TestCase(0, 0, 0, 1, 1, 1920, 2160, AspectRatioBehavior.Underscan, 200, 100, -1520, 880)]
    public void CameraComponent_ScreenPointToWorld2DPoint_ShouldReturnComputedValue_WhenRenderingSystemIsAddedToSceneObservers(double tx, double ty, double r,
        double sx, double sy, double vx, double vy, AspectRatioBehavior arb, double px, double py, double wx, double wy)
    {
        // Arrange
        RenderingContext2D.ScreenWidth.Returns(1920);
        RenderingContext2D.ScreenHeight.Returns(1080);

        var (_, renderingScene) = GetRenderingSystem();
        var entity = renderingScene.AddCamera(new Vector2(tx, ty), r, new Vector2(sx, sy));
        var cameraComponent = entity.GetComponent<CameraComponent>();
        cameraComponent.ViewRectangle = new Vector2(vx, vy);
        cameraComponent.AspectRatioBehavior = arb;

        // Act
        var actual = cameraComponent.ScreenPointToWorld2DPoint(new Vector2(px, py));

        // Assert
        Assert.That(cameraComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(actual, Is.EqualTo(new Vector2(wx, wy)).Using(CommonEqualityComparer.Vector2(0.000001)));
    }

    [Test]
    public void TODO()
    {
        // TODO Add API World2DPointToScreenPoint
        // TODO Add tests for Create2DWorldToScreenMatrix
        // TODO Add tests for GetBoundingRectangleOfView
        // TODO Rename RectangleRendererComponent.Dimension to Dimensions
        // TODO Update CameraComponent documentation to inform about IsManagedByRenderingSystem
        Assert.Fail("TODO");
    }
}