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
using static System.Net.Mime.MediaTypeNames;

namespace Geisha.Engine.UnitTests.Rendering.Systems
{
    [TestFixture]
    public class RenderingSystemTests
    {
        private const int ScreenWidth = 200;
        private const int ScreenHeight = 100;
        private IRenderingContext2D _renderingContext2D = null!;
        private IRenderingBackend _renderingBackend = null!;
        private IAggregatedDiagnosticInfoProvider _aggregatedDiagnosticInfoProvider = null!;
        private IDebugRendererForRenderingSystem _debugRendererForRenderingSystem = null!;

        [SetUp]
        public void SetUp()
        {
            _renderingContext2D = Substitute.For<IRenderingContext2D>();
            _renderingContext2D.ScreenWidth.Returns(ScreenWidth);
            _renderingContext2D.ScreenHeight.Returns(ScreenHeight);

            _renderingBackend = Substitute.For<IRenderingBackend>();
            _renderingBackend.Context2D.Returns(_renderingContext2D);
            _aggregatedDiagnosticInfoProvider = Substitute.For<IAggregatedDiagnosticInfoProvider>();
            _debugRendererForRenderingSystem = Substitute.For<IDebugRendererForRenderingSystem>();
        }

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
                _renderingContext2D.BeginDraw();
                _renderingContext2D.Clear(Color.White);
                _renderingContext2D.EndDraw();
                _renderingBackend.Present(false);
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
                _renderingContext2D.BeginDraw();
                _renderingContext2D.Clear(Color.White);
                _renderingContext2D.DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
                _renderingContext2D.EndDraw();
                _renderingBackend.Present(false);
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
            _renderingBackend.Received().Present(enableVSync);
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
                _renderingContext2D.DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
                _renderingContext2D.DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
            });
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
            _renderingContext2D.DidNotReceive().DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
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
            _renderingContext2D.Received(1).DrawSprite(entity.GetSprite(), Matrix3x3.CreateTranslation(new Vector2(-10, 10)), entity.GetOpacity());
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
            _renderingContext2D.Received(1).DrawSprite(entity.GetSprite(),
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
            _renderingContext2D.Received(1).DrawSprite(entity.GetSprite(),
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
            _renderingContext2D.Received(1).DrawSprite(entity.GetSprite(),
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
                _renderingContext2D.Clear(Color.White);
                _renderingContext2D.Clear(Color.Black);
                _renderingContext2D.SetClippingRectangle(new AxisAlignedRectangle(ScreenHeight, ScreenHeight));
                _renderingContext2D.Clear(Color.White);
                _renderingContext2D.Received(1).DrawSprite(entity.GetSprite(),
                    // Sprite transform is half the scale and translation due to camera view rectangle being scaled by height to match
                    new Matrix3x3(m11: 0.5, m12: 0, m13: -5, m21: 0, m22: 0.5, m23: 5, m31: 0, m32: 0, m33: 1),
                    entity.GetOpacity());
                _renderingContext2D.ClearClipping();
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
                _renderingContext2D.Clear(Color.White);
                _renderingContext2D.Clear(Color.Black);
                _renderingContext2D.SetClippingRectangle(new AxisAlignedRectangle(ScreenWidth, ScreenHeight / 2d));
                _renderingContext2D.Clear(Color.White);
                _renderingContext2D.Received(1).DrawSprite(entity.GetSprite(),
                    // Sprite transform is half the scale and translation due to camera view rectangle being scaled by width to match
                    new Matrix3x3(m11: 0.5, m12: 0, m13: -5, m21: 0, m22: 0.5, m23: 5, m31: 0, m32: 0, m33: 1),
                    entity.GetOpacity());
                _renderingContext2D.ClearClipping();
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
                _renderingContext2D.DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
                _renderingContext2D.DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
                _renderingContext2D.DrawSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());

                _renderingContext2D.DrawText(diagnosticInfo1.ToString(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3x3>());
                _renderingContext2D.DrawText(diagnosticInfo2.ToString(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3x3>());
                _renderingContext2D.DrawText(diagnosticInfo3.ToString(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3x3>());
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
                _renderingContext2D.DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
                _renderingContext2D.DrawSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());
                _renderingContext2D.DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
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
                _renderingContext2D.DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
                _renderingContext2D.DrawSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());
                _renderingContext2D.DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
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
                _renderingContext2D.DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
                _renderingContext2D.DrawSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());
                _renderingContext2D.DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
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
            _renderingContext2D.Received(1).DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
            _renderingContext2D.DidNotReceive().DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
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
            _renderingContext2D.Received(1).DrawSprite(entity.GetSprite(), entity.Get2DTransformationMatrix(), entity.GetOpacity());
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
            _renderingContext2D.Received(1).DrawSprite(entity.GetSprite(), entity.Get2DTransformationMatrix(), 0.5);
        }

        [Test]
        public void RenderScene_ShouldDrawTextLayout_WhenSceneContainsEntityWithTextRendererAndTransform()
        {
            // Arrange
            const string text = "Sample text";
            const string fontFamilyName = "Calibri";
            var fontSize = FontSize.FromDips(20);
            var color = Color.Red;
            const double maxWidth = 200;
            const double maxHeight = 400;
            const TextAlignment textAlignment = TextAlignment.Center;
            const ParagraphAlignment paragraphAlignment = ParagraphAlignment.Center;
            var pivot = new Vector2(100, 200);

            var textLayout = Substitute.For<ITextLayout>();
            _renderingContext2D.CreateTextLayout(text, fontFamilyName, fontSize, maxWidth, maxHeight).Returns(textLayout);

            var (renderingSystem, renderingScene) = GetRenderingSystem();
            renderingScene.AddCamera();
            var (entity, textRendererComponent) = renderingScene.AddText();

            textRendererComponent.FontFamilyName = fontFamilyName;
            textRendererComponent.FontSize = fontSize;
            textRendererComponent.Color = color;
            textRendererComponent.MaxWidth = maxWidth;
            textRendererComponent.MaxHeight = maxHeight;
            textRendererComponent.TextAlignment = textAlignment;
            textRendererComponent.ParagraphAlignment = paragraphAlignment;
            textRendererComponent.Pivot = pivot;
            // Force recreation of ITextLayout
            textRendererComponent.Text = text;

            // Act
            renderingSystem.RenderScene();

            // Assert
            textLayout.Received(1).TextAlignment = textAlignment;
            textLayout.Received(1).ParagraphAlignment = paragraphAlignment;
            _renderingContext2D.Received(1).DrawTextLayout(textLayout, color, entity.Get2DTransformationMatrix());
        }

        [Test]
        public void RenderScene_ShouldDrawTextLayout_WhenSceneContainsEntityWithTextRendererAndTransform_AfterLayoutIsUpdated()
        {
            // Arrange
            const string text = "Sample text";
            const string fontFamilyName = "Calibri";
            var fontSize = FontSize.FromDips(20);
            var color = Color.Red;
            const double maxWidth = 200;
            const double maxHeight = 400;
            const TextAlignment textAlignment = TextAlignment.Center;
            const ParagraphAlignment paragraphAlignment = ParagraphAlignment.Center;
            var pivot = new Vector2(100, 200);

            var textLayout = Substitute.For<ITextLayout>();
            _renderingContext2D.CreateTextLayout(text, Arg.Any<string>(), Arg.Any<FontSize>(), Arg.Any<double>(), Arg.Any<double>()).Returns(textLayout);

            var (renderingSystem, renderingScene) = GetRenderingSystem();
            renderingScene.AddCamera();
            var (entity, textRendererComponent) = renderingScene.AddText();

            textRendererComponent.Text = text;
            textRendererComponent.FontFamilyName = fontFamilyName;
            textRendererComponent.FontSize = fontSize;
            textRendererComponent.Color = color;
            textRendererComponent.MaxWidth = maxWidth;
            textRendererComponent.MaxHeight = maxHeight;
            textRendererComponent.TextAlignment = textAlignment;
            textRendererComponent.ParagraphAlignment = paragraphAlignment;
            textRendererComponent.Pivot = pivot;

            // Act
            renderingSystem.RenderScene();

            // Assert
            textLayout.Received(1).FontFamilyName = fontFamilyName;
            textLayout.Received(1).FontSize = fontSize;
            textLayout.Received(1).MaxWidth = maxWidth;
            textLayout.Received(1).MaxHeight = maxHeight;
            textLayout.Received(1).TextAlignment = textAlignment;
            textLayout.Received(1).ParagraphAlignment = paragraphAlignment;
            _renderingContext2D.Received(1).DrawTextLayout(textLayout, color, entity.Get2DTransformationMatrix());
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
            _renderingContext2D.Received(1).DrawRectangle(Arg.Is<AxisAlignedRectangle>(r =>
                    Math.Abs(r.Width - rectangleRenderer.Dimension.X) < 0.001 && Math.Abs(r.Height - rectangleRenderer.Dimension.Y) < 0.001),
                rectangleRenderer.Color, rectangleRenderer.FillInterior,
                entity.Get2DTransformationMatrix());
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
            _renderingContext2D.Received(1).DrawEllipse(new Ellipse(ellipseRenderer.RadiusX, ellipseRenderer.RadiusY), ellipseRenderer.Color,
                ellipseRenderer.FillInterior, entity.Get2DTransformationMatrix());
        }

        [Test]
        public void RenderScene_ShouldSetScreenWidthAndScreenHeightOnCameraComponent()
        {
            // Arrange
            const int screenWidth = 123;
            const int screenHeight = 456;
            _renderingContext2D.ScreenWidth.Returns(screenWidth);
            _renderingContext2D.ScreenHeight.Returns(screenHeight);

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
            _renderingContext2D.Received(1).DrawEllipse(new Ellipse(childEllipseRenderer.RadiusX, childEllipseRenderer.RadiusY), childEllipseRenderer.Color,
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
            _renderingContext2D.Received(1).DrawEllipse(new Ellipse(parentEllipseRenderer.RadiusX, parentEllipseRenderer.RadiusY), parentEllipseRenderer.Color,
                parentEllipseRenderer.FillInterior, parentExpectedTransform);

            var childEllipseRenderer = childEntity.GetComponent<EllipseRendererComponent>();
            _renderingContext2D.Received(1).DrawEllipse(new Ellipse(childEllipseRenderer.RadiusX, childEllipseRenderer.RadiusY), childEllipseRenderer.Color,
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
                _renderingContext2D.DrawSprite(entity.GetSprite(), entity.Get2DTransformationMatrix(), entity.GetOpacity());
                _debugRendererForRenderingSystem.Received(1).DrawDebugInformation(_renderingContext2D, Matrix3x3.Identity);
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
            _renderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

            _renderingContext2D.ClearReceivedCalls();

            // Act
            spriteEntity.RemoveComponent(spriteEntity.GetComponent<Transform2DComponent>());
            renderingSystem.RenderScene();

            // Assert
            _renderingContext2D.DidNotReceive().DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
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
            _renderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

            _renderingContext2D.ClearReceivedCalls();

            // Act
            cameraEntity.RemoveComponent(cameraEntity.GetComponent<Transform2DComponent>());
            renderingSystem.RenderScene();

            // Assert
            _renderingContext2D.DidNotReceive().DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
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
            _renderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

            _renderingContext2D.ClearReceivedCalls();

            // Act
            spriteEntity.RemoveComponent(spriteEntity.GetComponent<SpriteRendererComponent>());
            renderingSystem.RenderScene();

            // Assert
            _renderingContext2D.DidNotReceive().DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
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
            _renderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());

            _renderingContext2D.ClearReceivedCalls();

            // Act
            cameraEntity.RemoveComponent(cameraEntity.GetComponent<CameraComponent>());
            renderingSystem.RenderScene();

            // Assert
            _renderingContext2D.DidNotReceive().DrawSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>(), Arg.Any<double>());
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
            _renderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), Arg.Any<Matrix3x3>(), spriteEntity.GetOpacity());
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
            _renderingContext2D.Received(1).DrawSprite(sprite, spriteEntity.Get2DTransformationMatrix(), opacity);
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
            _renderingContext2D.Received(1).DrawSprite(spriteEntity.GetSprite(), spriteEntity.Get2DTransformationMatrix(), spriteEntity.GetOpacity());
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

            public (Entity entity, TextRendererComponent textRendererComponent) AddText()
            {
                var entity = _scene.CreateEntity();

                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                SetRandomValues(transform2DComponent);

                var textRendererComponent = entity.CreateComponent<TextRendererComponent>();

                return (entity, textRendererComponent);
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