using System;
using System.Diagnostics;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Systems;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Systems
{
    [TestFixture]
    public class RenderingSystemTests
    {
        private const int ScreenWidth = 200;
        private const int ScreenHeight = 100;
        private IRenderer2D _renderer2D = null!;
        private IRenderingBackend _renderingBackend = null!;
        private IAggregatedDiagnosticInfoProvider _aggregatedDiagnosticInfoProvider = null!;
        private IDebugRendererForRenderingSystem _debugRendererForRenderingSystem = null!;

        [SetUp]
        public void SetUp()
        {
            _renderer2D = Substitute.For<IRenderer2D>();
            _renderer2D.ScreenWidth.Returns(ScreenWidth);
            _renderer2D.ScreenHeight.Returns(ScreenHeight);

            _renderingBackend = Substitute.For<IRenderingBackend>();
            _renderingBackend.Renderer2D.Returns(_renderer2D);
            _aggregatedDiagnosticInfoProvider = Substitute.For<IAggregatedDiagnosticInfoProvider>();
            _debugRendererForRenderingSystem = Substitute.For<IDebugRendererForRenderingSystem>();
        }

        [Test]
        public void RenderScene_Should_BeginRendering_Clear_EndRendering_GivenAnEmptyScene()
        {
            // Arrange
            var (renderingSystem, _) = GetRenderingSystem();

            // Act
            renderingSystem.RenderScene();

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.BeginRendering();
                _renderer2D.Clear(Color.White);
                _renderer2D.EndRendering(false);
            });
        }

        [Test]
        public void RenderScene_ShouldCallInFollowingOrder_BeginRendering_Clear_RenderSprite_EndRendering()
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
                _renderer2D.BeginRendering();
                _renderer2D.Clear(Color.White);
                _renderer2D.RenderSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
                _renderer2D.EndRendering(false);
            });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void RenderScene_Should_EndRendering_WithWaitForVSync_BasedOnRenderingConfiguration(bool enableVSync)
        {
            // Arrange
            var (renderingSystem, _) = GetRenderingSystem(new RenderingConfiguration { EnableVSync = enableVSync });

            // Act
            renderingSystem.RenderScene();

            // Assert
            _renderer2D.Received().EndRendering(enableVSync);
        }

        [Test]
        public void RenderScene_ShouldIgnoreOrderInLayer_WhenEntitiesAreInDifferentSortingLayers()
        {
            // Arrange
            const string otherSortingLayer = "Other";

            var (renderingSystem, renderingScene) = GetRenderingSystem(new RenderingConfiguration
                { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, otherSortingLayer } });
            renderingScene.AddCamera();
            var entity1 = renderingScene.AddSprite(orderInLayer: 0, sortingLayerName: otherSortingLayer);
            var entity2 = renderingScene.AddSprite(orderInLayer: 1);

            // Act
            renderingSystem.RenderScene();

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
                _renderer2D.RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
            });
        }

        [Test]
        public void RenderScene_ShouldNotRenderSprite_WhenSceneContainsEntityWithSpriteRendererAndTransformButDoesNotContainCamera()
        {
            // Arrange
            var (renderingSystem, renderingScene) = GetRenderingSystem();
            renderingScene.AddSprite();

            // Act
            renderingSystem.RenderScene();

            // Assert
            _renderer2D.DidNotReceive().RenderSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
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
            _renderer2D.Received(1).RenderSprite(entity.GetSprite(), Matrix3x3.CreateTranslation(new Vector2(-10, 10)), entity.GetOpacity());
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
            _renderer2D.Received(1).RenderSprite(entity.GetSprite(),
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
            _renderer2D.Received(1).RenderSprite(entity.GetSprite(),
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
            _renderer2D.Received(1).RenderSprite(entity.GetSprite(),
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
                _renderer2D.Clear(Color.White);
                _renderer2D.Clear(Color.Black);
                _renderer2D.SetClippingRectangle(new AxisAlignedRectangle(ScreenHeight, ScreenHeight));
                _renderer2D.Clear(Color.White);
                _renderer2D.Received(1).RenderSprite(entity.GetSprite(),
                    // Sprite transform is half the scale and translation due to camera view rectangle being scaled by height to match
                    new Matrix3x3(m11: 0.5, m12: 0, m13: -5, m21: 0, m22: 0.5, m23: 5, m31: 0, m32: 0, m33: 1),
                    entity.GetOpacity());
                _renderer2D.ClearClipping();
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
                _renderer2D.Clear(Color.White);
                _renderer2D.Clear(Color.Black);
                _renderer2D.SetClippingRectangle(new AxisAlignedRectangle(ScreenWidth, ScreenHeight / 2d));
                _renderer2D.Clear(Color.White);
                _renderer2D.Received(1).RenderSprite(entity.GetSprite(),
                    // Sprite transform is half the scale and translation due to camera view rectangle being scaled by width to match
                    new Matrix3x3(m11: 0.5, m12: 0, m13: -5, m21: 0, m22: 0.5, m23: 5, m31: 0, m32: 0, m33: 1),
                    entity.GetOpacity());
                _renderer2D.ClearClipping();
            });
        }

        [Test]
        public void RenderScene_ShouldRenderDiagnosticInfo_AfterRenderingScene()
        {
            // Arrange
            var diagnosticInfo1 = GetRandomDiagnosticInfo();
            var diagnosticInfo2 = GetRandomDiagnosticInfo();
            var diagnosticInfo3 = GetRandomDiagnosticInfo();

            _aggregatedDiagnosticInfoProvider.GetAllDiagnosticInfo().Returns(new[] { diagnosticInfo1, diagnosticInfo2, diagnosticInfo3 });

            var (renderingSystem, renderingScene) = GetRenderingSystem();
            renderingScene.AddCamera();
            var entity1 = renderingScene.AddSprite(orderInLayer: 0);
            var entity2 = renderingScene.AddSprite(orderInLayer: 1);
            var entity3 = renderingScene.AddSprite(orderInLayer: 2);

            // Act
            renderingSystem.RenderScene();

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
                _renderer2D.RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
                _renderer2D.RenderSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());

                _renderer2D.RenderText(diagnosticInfo1.ToString(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3x3>());
                _renderer2D.RenderText(diagnosticInfo2.ToString(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3x3>());
                _renderer2D.RenderText(diagnosticInfo3.ToString(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3x3>());
            });
        }

        [Test]
        public void RenderScene_ShouldRenderInOrderOf_OrderInLayer_WhenEntitiesAreInTheSameSortingLayer()
        {
            // Arrange
            var (renderingSystem, renderingScene) = GetRenderingSystem();
            renderingScene.AddCamera();
            var entity1 = renderingScene.AddSprite(orderInLayer: 1);
            var entity2 = renderingScene.AddSprite(orderInLayer: -1);
            var entity3 = renderingScene.AddSprite(orderInLayer: 0);

            // Act
            renderingSystem.RenderScene();

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
                _renderer2D.RenderSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());
                _renderer2D.RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
            });
        }

        [Test]
        public void RenderScene_ShouldRenderInSortingLayersOrder_Default_Background_Foreground()
        {
            // Arrange
            const string backgroundSortingLayerName = "Background";
            const string foregroundSortingLayerName = "Foreground";

            var (renderingSystem, renderingScene) = GetRenderingSystem(new RenderingConfiguration
                { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, backgroundSortingLayerName, foregroundSortingLayerName } });
            renderingScene.AddCamera();
            var entity1 = renderingScene.AddSprite(sortingLayerName: foregroundSortingLayerName);
            var entity2 = renderingScene.AddSprite(sortingLayerName: RenderingConfiguration.DefaultSortingLayerName);
            var entity3 = renderingScene.AddSprite(sortingLayerName: backgroundSortingLayerName);

            // Act
            renderingSystem.RenderScene();

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
                _renderer2D.RenderSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());
                _renderer2D.RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
            });
        }

        [Test]
        public void RenderScene_ShouldRenderInSortingLayersOrder_Foreground_Background_Default()
        {
            // Arrange
            const string backgroundSortingLayerName = "Background";
            const string foregroundSortingLayerName = "Foreground";

            var (renderingSystem, renderingScene) = GetRenderingSystem(new RenderingConfiguration
                { SortingLayersOrder = new[] { foregroundSortingLayerName, backgroundSortingLayerName, RenderingConfiguration.DefaultSortingLayerName } });
            renderingScene.AddCamera();
            var entity1 = renderingScene.AddSprite(sortingLayerName: foregroundSortingLayerName);
            var entity2 = renderingScene.AddSprite(sortingLayerName: RenderingConfiguration.DefaultSortingLayerName);
            var entity3 = renderingScene.AddSprite(sortingLayerName: backgroundSortingLayerName);

            // Act
            renderingSystem.RenderScene();

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
                _renderer2D.RenderSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());
                _renderer2D.RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
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
            _renderer2D.Received(1).RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
            _renderer2D.DidNotReceive().RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
        }

        [Test]
        public void RenderScene_ShouldRenderSprite_WhenSceneContainsEntityWithSpriteRendererAndTransform()
        {
            // Arrange
            var (renderingSystem, renderingScene) = GetRenderingSystem();
            renderingScene.AddCamera();
            var entity = renderingScene.AddSprite();

            // Act
            renderingSystem.RenderScene();

            // Assert
            _renderer2D.Received(1).RenderSprite(entity.GetSprite(), entity.Get2DTransformationMatrix(), entity.GetOpacity());
        }

        [Test]
        public void RenderScene_ShouldRenderSprite_WhenOpacityIsNonDefault()
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
            _renderer2D.Received(1).RenderSprite(entity.GetSprite(), entity.Get2DTransformationMatrix(), 0.5);
        }

        [Test]
        public void RenderScene_ShouldRenderText_WhenSceneContainsEntityWithTextRendererAndTransform()
        {
            // Arrange
            var (renderingSystem, renderingScene) = GetRenderingSystem();
            renderingScene.AddCamera();
            var entity = renderingScene.AddText();

            // Act
            renderingSystem.RenderScene();

            // Assert
            var textRenderer = entity.GetComponent<TextRendererComponent>();
            Debug.Assert(textRenderer.Text != null, "textRenderer.Text != null");
            _renderer2D.Received(1).RenderText(textRenderer.Text, textRenderer.FontSize, textRenderer.Color, entity.Get2DTransformationMatrix());
        }

        [Test]
        public void RenderScene_ShouldRenderRectangle_WhenSceneContainsEntityWithRectangleRendererAndTransform()
        {
            // Arrange
            var (renderingSystem, renderingScene) = GetRenderingSystem();
            renderingScene.AddCamera();
            var entity = renderingScene.AddRectangle();

            // Act
            renderingSystem.RenderScene();

            // Assert
            var rectangleRenderer = entity.GetComponent<RectangleRendererComponent>();
            _renderer2D.Received(1).RenderRectangle(Arg.Is<AxisAlignedRectangle>(r =>
                    Math.Abs(r.Width - rectangleRenderer.Dimension.X) < 0.001 && Math.Abs(r.Height - rectangleRenderer.Dimension.Y) < 0.001),
                rectangleRenderer.Color, rectangleRenderer.FillInterior,
                entity.Get2DTransformationMatrix());
        }

        [Test]
        public void RenderScene_ShouldRenderEllipse_WhenSceneContainsEntityWithEllipseRendererAndTransform()
        {
            // Arrange
            var (renderingSystem, renderingScene) = GetRenderingSystem();
            renderingScene.AddCamera();
            var entity = renderingScene.AddEllipse();

            // Act
            renderingSystem.RenderScene();

            // Assert
            var ellipseRenderer = entity.GetComponent<EllipseRendererComponent>();
            _renderer2D.Received(1).RenderEllipse(new Ellipse(ellipseRenderer.RadiusX, ellipseRenderer.RadiusY), ellipseRenderer.Color,
                ellipseRenderer.FillInterior, entity.Get2DTransformationMatrix());
        }

        [Test]
        public void RenderScene_ShouldSetScreenWidthAndScreenHeightOnCameraComponent()
        {
            // Arrange
            const int screenWidth = 123;
            const int screenHeight = 456;
            _renderer2D.ScreenWidth.Returns(screenWidth);
            _renderer2D.ScreenHeight.Returns(screenHeight);

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
            _renderer2D.Received(1).RenderEllipse(new Ellipse(childEllipseRenderer.RadiusX, childEllipseRenderer.RadiusY), childEllipseRenderer.Color,
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
            _renderer2D.Received(1).RenderEllipse(new Ellipse(parentEllipseRenderer.RadiusX, parentEllipseRenderer.RadiusY), parentEllipseRenderer.Color,
                parentEllipseRenderer.FillInterior, parentExpectedTransform);

            var childEllipseRenderer = childEntity.GetComponent<EllipseRendererComponent>();
            _renderer2D.Received(1).RenderEllipse(new Ellipse(childEllipseRenderer.RadiusX, childEllipseRenderer.RadiusY), childEllipseRenderer.Color,
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
                _renderer2D.RenderSprite(entity.GetSprite(), entity.Get2DTransformationMatrix(), entity.GetOpacity());
                _debugRendererForRenderingSystem.Received(1).DrawDebugInformation(_renderer2D, Matrix3x3.Identity);
            });
        }

        [Test]
        public void RenderScene_ShouldNotRenderSprite_WhenTransform2DComponentRemovedFromSpriteEntity()
        {
            // Arrange
            var (renderingSystem, renderingScene) = GetRenderingSystem();
            renderingScene.AddCamera();
            var spriteEntity = renderingScene.AddSprite();

            // Assume
            renderingSystem.RenderScene();
            _renderer2D.Received(1).RenderSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

            _renderer2D.ClearReceivedCalls();

            // Act
            spriteEntity.RemoveComponent(spriteEntity.GetComponent<Transform2DComponent>());
            renderingSystem.RenderScene();

            // Assert
            _renderer2D.DidNotReceive().RenderSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
        }

        [Test]
        public void RenderScene_ShouldNotRenderSprite_WhenTransform2DComponentRemovedFromCameraEntity()
        {
            // Arrange
            var (renderingSystem, renderingScene) = GetRenderingSystem();
            var cameraEntity = renderingScene.AddCamera();
            var spriteEntity = renderingScene.AddSprite();

            // Arrange
            renderingSystem.RenderScene();
            _renderer2D.Received(1).RenderSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

            _renderer2D.ClearReceivedCalls();

            // Act
            cameraEntity.RemoveComponent(cameraEntity.GetComponent<Transform2DComponent>());
            renderingSystem.RenderScene();

            // Assert
            _renderer2D.DidNotReceive().RenderSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
        }

        [Test]
        public void RenderScene_ShouldNotRenderSprite_WhenRenderer2DComponentRemoved()
        {
            // Arrange
            var (renderingSystem, renderingScene) = GetRenderingSystem();
            renderingScene.AddCamera();
            var spriteEntity = renderingScene.AddSprite();

            // Arrange
            renderingSystem.RenderScene();
            _renderer2D.Received(1).RenderSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

            _renderer2D.ClearReceivedCalls();

            // Act
            spriteEntity.RemoveComponent(spriteEntity.GetComponent<SpriteRendererComponent>());
            renderingSystem.RenderScene();

            // Assert
            _renderer2D.DidNotReceive().RenderSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
        }

        [Test]
        public void RenderScene_ShouldNotRenderSprite_WhenCameraComponentRemoved()
        {
            // Arrange
            var (renderingSystem, renderingScene) = GetRenderingSystem();
            var cameraEntity = renderingScene.AddCamera();
            var spriteEntity = renderingScene.AddSprite();

            // Arrange
            renderingSystem.RenderScene();
            _renderer2D.Received(1).RenderSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

            _renderer2D.ClearReceivedCalls();

            // Act
            cameraEntity.RemoveComponent(cameraEntity.GetComponent<CameraComponent>());
            renderingSystem.RenderScene();

            // Assert
            _renderer2D.DidNotReceive().RenderSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
        }

        private (RenderingSystem renderingSystem, RenderingScene renderingScene) GetRenderingSystem()
        {
            return GetRenderingSystem(new RenderingConfiguration());
        }

        private (RenderingSystem renderingSystem, RenderingScene renderingScene) GetRenderingSystem(RenderingConfiguration configuration)
        {
            var renderingSystem = new RenderingSystem(
                _renderingBackend,
                configuration,
                _aggregatedDiagnosticInfoProvider,
                _debugRendererForRenderingSystem
            );

            var renderingScene = new RenderingScene(renderingSystem);

            return (renderingSystem, renderingScene);
        }

        private static DiagnosticInfo GetRandomDiagnosticInfo()
        {
            return new DiagnosticInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        }

        private sealed class RenderingScene
        {
            private readonly Scene _scene = TestSceneFactory.Create();

            public RenderingScene(ISceneObserver observer)
            {
                _scene.AddObserver(observer);
            }

            public Scene Scene => _scene;

            public Entity AddCamera()
            {
                var entity = _scene.CreateEntity();
                entity.CreateComponent<Transform2DComponent>();

                var cameraComponent = entity.CreateComponent<CameraComponent>();
                cameraComponent.ViewRectangle = new Vector2(ScreenWidth, ScreenHeight);

                return entity;
            }

            public Entity AddCamera(Vector2 translation, double rotation, Vector2 scale)
            {
                var entity = AddCamera();
                var transform2DComponent = entity.GetComponent<Transform2DComponent>();
                transform2DComponent.Translation = translation;
                transform2DComponent.Rotation = rotation;
                transform2DComponent.Scale = scale;

                return entity;
            }

            public Entity AddSpriteWithDefaultTransform()
            {
                var entity = _scene.CreateEntity();
                entity.CreateComponent<Transform2DComponent>();

                var spriteRendererComponent = entity.CreateComponent<SpriteRendererComponent>();
                spriteRendererComponent.Sprite = new Sprite(Substitute.For<ITexture>(), Vector2.Zero, Vector2.Zero, Vector2.Zero, 0);

                return entity;
            }

            public Entity AddSprite(int orderInLayer = 0, string sortingLayerName = RenderingConfiguration.DefaultSortingLayerName, bool visible = true)
            {
                var entity = AddSpriteWithDefaultTransform();

                var transformComponent = entity.GetComponent<Transform2DComponent>();
                SetRandomValues(transformComponent);

                var spriteRendererComponent = entity.GetComponent<SpriteRendererComponent>();
                spriteRendererComponent.OrderInLayer = orderInLayer;
                spriteRendererComponent.SortingLayerName = sortingLayerName;
                spriteRendererComponent.Visible = visible;

                return entity;
            }

            public Entity AddText()
            {
                var entity = _scene.CreateEntity();

                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                SetRandomValues(transform2DComponent);

                var textRendererComponent = entity.CreateComponent<TextRendererComponent>();
                textRendererComponent.Text = Utils.Random.GetString();
                textRendererComponent.FontSize = FontSize.FromPoints(Utils.Random.NextDouble());
                textRendererComponent.Color = Color.FromArgb(Utils.Random.Next());

                return entity;
            }

            public Entity AddRectangle()
            {
                var entity = _scene.CreateEntity();

                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                SetRandomValues(transform2DComponent);

                var rectangleRendererComponent = entity.CreateComponent<RectangleRendererComponent>();
                rectangleRendererComponent.Dimension = Utils.RandomVector2();
                rectangleRendererComponent.Color = Color.FromArgb(Utils.Random.Next());
                rectangleRendererComponent.FillInterior = Utils.Random.NextBool();

                return entity;
            }

            public Entity AddEllipse()
            {
                var entity = _scene.CreateEntity();
                CreateEllipse(entity);
                return entity;
            }

            public (Entity parent, Entity child) AddParentEllipseWithChildEllipse()
            {
                var parent = _scene.CreateEntity();
                CreateEllipse(parent);

                var child = parent.CreateChildEntity();
                CreateEllipse(child);

                return (parent, child);
            }

            private static void SetRandomValues(Transform2DComponent transform2DComponent)
            {
                transform2DComponent.Translation = Utils.RandomVector2();
                transform2DComponent.Rotation = Utils.Random.NextDouble();
                transform2DComponent.Scale = Utils.RandomVector2();
            }

            private static void CreateEllipse(Entity entity)
            {
                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                SetRandomValues(transform2DComponent);

                var ellipseRendererComponent = entity.CreateComponent<EllipseRendererComponent>();
                ellipseRendererComponent.RadiusX = Utils.Random.NextDouble();
                ellipseRendererComponent.RadiusY = Utils.Random.NextDouble();
                ellipseRendererComponent.Color = Color.FromArgb(Utils.Random.Next());
                ellipseRendererComponent.FillInterior = Utils.Random.NextBool();
            }
        }
    }

    internal static class EntityExtensions
    {
        public static Sprite GetSprite(this Entity entity) => entity.GetComponent<SpriteRendererComponent>().Sprite ??
                                                              throw new ArgumentException("Entity must have SpriteRendererComponent with non-null Sprite.");

        public static double GetOpacity(this Entity entity) => entity.GetComponent<SpriteRendererComponent>().Opacity;

        public static Matrix3x3 Get2DTransformationMatrix(this Entity entity) => entity.GetComponent<Transform2DComponent>().ToMatrix();
    }
}