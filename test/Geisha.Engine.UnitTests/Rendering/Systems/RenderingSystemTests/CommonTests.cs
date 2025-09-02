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
        var context = CreateRenderingTestContext();

        // Act
        context.RenderingSystem.RenderScene();

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
        var context = CreateRenderingTestContext();
        context.AddCamera();
        context.AddSprite();

        // Act
        context.RenderingSystem.RenderScene();

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
        var context = CreateRenderingTestContext(new RenderingConfiguration { EnableVSync = enableVSync });

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingBackend.Received().Present(enableVSync);
    }

    [Test]
    public void RenderScene_ShouldNotDrawRendererComponent_WhenSceneContainsEntityWithRendererComponentAndTransformButDoesNotContainCamera()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddSprite();

        // Act
        context.RenderingSystem.RenderScene();

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

        var context = CreateRenderingTestContext();
        context.AddCamera();
        var entity1 = context.AddSprite(orderInLayer: 0);
        var entity2 = context.AddSprite(orderInLayer: 1);
        var entity3 = context.AddSprite(orderInLayer: 2);

        var renderingStatistics = new RenderingStatistics
        {
            DrawCalls = 123
        };

        RenderingBackend.Statistics.Returns(renderingStatistics);

        // Act
        context.RenderingSystem.RenderScene();

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
        var context = CreateRenderingTestContext();
        context.AddCamera();
        var entity1 = context.AddSprite(visible: true);
        var entity2 = context.AddSprite(visible: false);

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
        RenderingContext2D.DidNotReceive().DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
    }

    [Test]
    public void RenderScene_ShouldDrawDebugInformation()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();
        var entity = context.AddSprite();

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.DrawSprite(entity.GetSprite(), entity.Get2DTransformationMatrix(), entity.GetOpacity());
            DebugRendererForRenderingSystem.Received(1).DrawDebugInformation(RenderingContext2D, Matrix3x3.Identity);
        });
    }

    [Test]
    public void RenderScene_ShouldNotDrawRendererComponent_WhenTransform2DComponentRemovedFromRendererEntity()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();
        var rendererEntity = context.AddSprite();

        // Assume
        context.RenderingSystem.RenderScene();
        RenderingContext2D.Received(1).DrawSprite(rendererEntity.GetSprite(), rendererEntity.Get2DTransformationMatrix(), rendererEntity.GetOpacity());

        RenderingContext2D.ClearReceivedCalls();

        // Act
        rendererEntity.RemoveComponent(rendererEntity.GetComponent<Transform2DComponent>());
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.DidNotReceive().DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
    }

    [Test]
    public void RenderScene_ShouldNotDrawRendererComponent_WhenTransform2DComponentRemovedFromCameraEntity()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        var cameraEntity = context.AddCamera();
        var spriteEntity = context.AddSprite();

        // Arrange
        context.RenderingSystem.RenderScene();
        RenderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

        RenderingContext2D.ClearReceivedCalls();

        // Act
        cameraEntity.RemoveComponent(cameraEntity.GetComponent<Transform2DComponent>());
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.DidNotReceive().DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
    }

    [Test]
    public void RenderScene_ShouldNotDrawRendererComponent_WhenRenderer2DComponentRemoved()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();
        var spriteEntity = context.AddSprite();

        // Arrange
        context.RenderingSystem.RenderScene();
        RenderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

        RenderingContext2D.ClearReceivedCalls();

        // Act
        spriteEntity.RemoveComponent(spriteEntity.GetComponent<SpriteRendererComponent>());
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.DidNotReceive().DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
    }

    [Test]
    public void RenderScene_ShouldNotDrawRendererComponent_WhenCameraComponentRemoved()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        var cameraEntity = context.AddCamera();
        var spriteEntity = context.AddSprite();

        // Arrange
        context.RenderingSystem.RenderScene();
        RenderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

        RenderingContext2D.ClearReceivedCalls();

        // Act
        cameraEntity.RemoveComponent(cameraEntity.GetComponent<CameraComponent>());
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.DidNotReceive().DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
    }

    [Test]
    public void RenderingSystem_ShouldNotDuplicateRendererComponent_WhenCameraComponentIsAddedToEntity_AndEntityAlreadyHasRendererComponent()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        var spriteEntity = context.AddSprite();

        // Act
        var cameraComponent = spriteEntity.CreateComponent<CameraComponent>();
        cameraComponent.ViewRectangle = new Vector2(ScreenWidth, ScreenHeight);
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), Arg.Any<Matrix3x3>(), spriteEntity.GetOpacity());
    }

    [Test]
    public void RenderingSystem_ShouldAllowToAddRendererComponentToEntity_WhenEntityAlreadyHasCameraComponent()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        var entity = context.AddCamera();

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
        var context = CreateRenderingTestContext();
        context.AddCamera();
        var spriteEntity = context.AddSprite();

        // Act
        var sprite = spriteEntity.GetSprite();
        var opacity = spriteEntity.GetOpacity();

        spriteEntity.RemoveComponent(spriteEntity.GetComponent<SpriteRendererComponent>());

        var newSpriteRendererComponent = spriteEntity.CreateComponent<SpriteRendererComponent>();
        newSpriteRendererComponent.Sprite = sprite;
        newSpriteRendererComponent.Opacity = opacity;

        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(sprite, spriteEntity.Get2DTransformationMatrix(), opacity);
    }

    [Test]
    public void RenderingSystem_ShouldDrawRendererComponent_WhenCameraComponentRemovedFromEntity_AndThenAddedToEntity()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        var cameraEntity = context.AddCamera();
        var spriteEntity = context.AddSprite();

        // Act
        var cameraComponent = cameraEntity.GetComponent<CameraComponent>();
        var viewRectangle = cameraComponent.ViewRectangle;

        cameraEntity.RemoveComponent(cameraComponent);

        var newCameraComponent = cameraEntity.CreateComponent<CameraComponent>();
        newCameraComponent.ViewRectangle = viewRectangle;

        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());
    }
}