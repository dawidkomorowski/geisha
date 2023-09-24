using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Systems.RenderingSystemTests;

[TestFixture]
public class CommonTests : RenderingSystemTestsBase
{
    [Test]
    public void RenderScene_ShouldCallInFollowingOrder_BeginDraw_Clear_EndDraw_Present_GivenAnEmptyScene()
    {
        // Arrange
        var (renderingSystem, _) = GetRenderingSystem();

        // Act
        renderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.BeginDraw();
            RenderingContext2D.Clear(Color.White);
            RenderingContext2D.EndDraw();
            RenderingBackend.Present(false);
        });
    }

    [Test]
    public void RenderScene_ShouldCallInFollowingOrder_BeginDraw_Clear_DrawSprite_EndDraw_Present_GivenSceneWithCameraAndSprite()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        renderingScene.AddSprite();

        // Act
        renderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.BeginDraw();
            RenderingContext2D.Clear(Color.White);
            RenderingContext2D.DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
            RenderingContext2D.EndDraw();
            RenderingBackend.Present(false);
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void RenderScene_Should_Present_WithWaitForVSync_BasedOnRenderingConfiguration(bool enableVSync)
    {
        // Arrange
        var (renderingSystem, _) = GetRenderingSystem(new RenderingConfiguration { EnableVSync = enableVSync });

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingBackend.Received().Present(enableVSync);
    }

    [Test]
    public void RenderScene_ShouldNotDrawSprite_WhenSceneContainsEntityWithSpriteRendererAndTransformButDoesNotContainCamera()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddSprite();

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.DidNotReceive().DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
    }

    [Test]
    public void RenderScene_ShouldRenderDiagnosticInfo_AfterRenderingScene_And_AfterUpdatingRenderingDiagnostics()
    {
        // Arrange
        var diagnosticInfo1 = GetRandomDiagnosticInfo();
        var diagnosticInfo2 = GetRandomDiagnosticInfo();
        var diagnosticInfo3 = GetRandomDiagnosticInfo();

        AggregatedDiagnosticInfoProvider.GetAllDiagnosticInfo().Returns(new[] { diagnosticInfo1, diagnosticInfo2, diagnosticInfo3 });

        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var entity1 = renderingScene.AddSprite(orderInLayer: 0);
        var entity2 = renderingScene.AddSprite(orderInLayer: 1);
        var entity3 = renderingScene.AddSprite(orderInLayer: 2);

        var renderingStatistics = new RenderingStatistics
        {
            DrawCalls = 123
        };

        RenderingBackend.Statistics.Returns(renderingStatistics);

        // Act
        renderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
            RenderingContext2D.DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
            RenderingContext2D.DrawSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());

            RenderingDiagnosticInfoProvider.UpdateDiagnostics(renderingStatistics);

            RenderingContext2D.DrawText(diagnosticInfo1.ToString(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3x3>());
            RenderingContext2D.DrawText(diagnosticInfo2.ToString(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3x3>());
            RenderingContext2D.DrawText(diagnosticInfo3.ToString(), Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3x3>());
        });
    }

    [Test]
    public void RenderScene_ShouldRenderOnlyEntities_ThatHaveVisibleSpriteRenderer()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var entity1 = renderingScene.AddSprite(visible: true);
        var entity2 = renderingScene.AddSprite(visible: false);

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
        RenderingContext2D.DidNotReceive().DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
    }

    [Test]
    public void RenderScene_ShouldDrawSprite_WhenSceneContainsEntityWithSpriteRendererAndTransform()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var entity = renderingScene.AddSprite();

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(entity.GetSprite(), entity.Get2DTransformationMatrix(), entity.GetOpacity());
    }

    [Test]
    public void RenderScene_ShouldDrawSprite_WhenOpacityIsNonDefault()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var entity = renderingScene.AddSprite();
        var spriteRendererComponent = entity.GetComponent<SpriteRendererComponent>();
        spriteRendererComponent.Opacity = 0.5;

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(entity.GetSprite(), entity.Get2DTransformationMatrix(), 0.5);
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
        RenderingContext2D.Received(1).DrawRectangle(new AxisAlignedRectangle(rectangleRenderer.Dimension), rectangleRenderer.Color,
            rectangleRenderer.FillInterior, entity.Get2DTransformationMatrix());
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
    public void RenderScene_ShouldRenderEntityTransformedWithParentIdentityTransform_WhenEntityHasParentWithoutTransform2DComponent()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var parentEntity = renderingScene.Scene.CreateEntity();
        var childEntity = renderingScene.AddEllipse();
        childEntity.Parent = parentEntity;

        var childExpectedTransform = childEntity.Get2DTransformationMatrix();


        // Act
        renderingSystem.RenderScene();

        // Assert
        var childEllipseRenderer = childEntity.GetComponent<EllipseRendererComponent>();
        RenderingContext2D.Received(1).DrawEllipse(new Ellipse(childEllipseRenderer.RadiusX, childEllipseRenderer.RadiusY), childEllipseRenderer.Color,
            childEllipseRenderer.FillInterior, childExpectedTransform);
    }

    [Test]
    public void RenderScene_ShouldRenderEntityTransformedWithParentTransform_WhenEntityHasParentWithTransform2DComponent()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var (parentEntity, childEntity) = renderingScene.AddParentEllipseWithChildEllipse();

        var parentExpectedTransform = parentEntity.Get2DTransformationMatrix();
        var childExpectedTransform = parentExpectedTransform * childEntity.Get2DTransformationMatrix();


        // Act
        renderingSystem.RenderScene();

        // Assert
        var parentEllipseRenderer = parentEntity.GetComponent<EllipseRendererComponent>();
        RenderingContext2D.Received(1).DrawEllipse(new Ellipse(parentEllipseRenderer.RadiusX, parentEllipseRenderer.RadiusY), parentEllipseRenderer.Color,
            parentEllipseRenderer.FillInterior, parentExpectedTransform);

        var childEllipseRenderer = childEntity.GetComponent<EllipseRendererComponent>();
        RenderingContext2D.Received(1).DrawEllipse(new Ellipse(childEllipseRenderer.RadiusX, childEllipseRenderer.RadiusY), childEllipseRenderer.Color,
            childEllipseRenderer.FillInterior, childExpectedTransform);
    }

    [Test]
    public void RenderScene_ShouldDrawDebugInformation()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var entity = renderingScene.AddSprite();

        // Act
        renderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.DrawSprite(entity.GetSprite(), entity.Get2DTransformationMatrix(), entity.GetOpacity());
            DebugRendererForRenderingSystem.Received(1).DrawDebugInformation(RenderingContext2D, Matrix3x3.Identity);
        });
    }

    [Test]
    public void RenderScene_ShouldNotDrawSprite_WhenTransform2DComponentRemovedFromSpriteEntity()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var spriteEntity = renderingScene.AddSprite();

        // Assume
        renderingSystem.RenderScene();
        RenderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

        RenderingContext2D.ClearReceivedCalls();

        // Act
        spriteEntity.RemoveComponent(spriteEntity.GetComponent<Transform2DComponent>());
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.DidNotReceive().DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
    }

    [Test]
    public void RenderScene_ShouldNotDrawSprite_WhenTransform2DComponentRemovedFromCameraEntity()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        var cameraEntity = renderingScene.AddCamera();
        var spriteEntity = renderingScene.AddSprite();

        // Arrange
        renderingSystem.RenderScene();
        RenderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

        RenderingContext2D.ClearReceivedCalls();

        // Act
        cameraEntity.RemoveComponent(cameraEntity.GetComponent<Transform2DComponent>());
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.DidNotReceive().DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
    }

    [Test]
    public void RenderScene_ShouldNotDrawSprite_WhenRenderer2DComponentRemoved()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var spriteEntity = renderingScene.AddSprite();

        // Arrange
        renderingSystem.RenderScene();
        RenderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

        RenderingContext2D.ClearReceivedCalls();

        // Act
        spriteEntity.RemoveComponent(spriteEntity.GetComponent<SpriteRendererComponent>());
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.DidNotReceive().DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
    }

    [Test]
    public void RenderScene_ShouldNotDrawSprite_WhenCameraComponentRemoved()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        var cameraEntity = renderingScene.AddCamera();
        var spriteEntity = renderingScene.AddSprite();

        // Arrange
        renderingSystem.RenderScene();
        RenderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

        RenderingContext2D.ClearReceivedCalls();

        // Act
        cameraEntity.RemoveComponent(cameraEntity.GetComponent<CameraComponent>());
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.DidNotReceive().DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
    }

    [Test]
    public void RenderingSystem_ShouldNotDuplicateRendererComponent_WhenCameraComponentIsAddedToEntity_AndEntityAlreadyHasRendererComponent()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        var spriteEntity = renderingScene.AddSprite();

        // Act
        var cameraComponent = spriteEntity.CreateComponent<CameraComponent>();
        cameraComponent.ViewRectangle = new Vector2(ScreenWidth, ScreenHeight);
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), Arg.Any<Matrix3x3>(), spriteEntity.GetOpacity());
    }

    [Test]
    public void RenderingSystem_ShouldAllowToAddRendererComponentToEntity_WhenEntityAlreadyHasCameraComponent()
    {
        // Arrange
        var (_, renderingScene) = GetRenderingSystem();
        var entity = renderingScene.AddCamera();

        // Act
        entity.CreateComponent<SpriteRendererComponent>();

        // Assert
        Assert.That(entity.HasComponent<CameraComponent>());
        Assert.That(entity.HasComponent<SpriteRendererComponent>());
    }

    [Test]
    public void RenderingSystem_ShouldDrawRendererComponent_WhenRendererComponentRemovedFromEntity_AndThenAddedToEntity()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var spriteEntity = renderingScene.AddSprite();

        // Act
        var sprite = spriteEntity.GetSprite();
        var opacity = spriteEntity.GetOpacity();

        spriteEntity.RemoveComponent(spriteEntity.GetComponent<SpriteRendererComponent>());

        var newSpriteRendererComponent = spriteEntity.CreateComponent<SpriteRendererComponent>();
        newSpriteRendererComponent.Sprite = sprite;
        newSpriteRendererComponent.Opacity = opacity;

        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(sprite, spriteEntity.Get2DTransformationMatrix(), opacity);
    }

    [Test]
    public void RenderingSystem_ShouldDrawRendererComponent_WhenCameraComponentRemovedFromEntity_AndThenAddedToEntity()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        var cameraEntity = renderingScene.AddCamera();
        var spriteEntity = renderingScene.AddSprite();

        // Act
        var cameraComponent = cameraEntity.GetComponent<CameraComponent>();
        var viewRectangle = cameraComponent.ViewRectangle;

        cameraEntity.RemoveComponent(cameraComponent);

        var newCameraComponent = cameraEntity.CreateComponent<CameraComponent>();
        newCameraComponent.ViewRectangle = viewRectangle;

        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());
    }
}