using System;
using System.Diagnostics;
using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
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
        private readonly RenderingConfiguration.IBuilder _renderingConfigurationBuilder = RenderingConfiguration.CreateBuilder();

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
            var renderingSystem = GetRenderingSystem();
            var scene = TestSceneFactory.Create();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.BeginRendering();
                _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));
                _renderer2D.EndRendering(false);
            });
        }

        [Test]
        public void RenderScene_ShouldCallInFollowingOrder_BeginRendering_Clear_RenderSprite_EndRendering()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            renderingSceneBuilder.AddSprite();
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.BeginRendering();
                _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));
                _renderer2D.RenderSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>());
                _renderer2D.EndRendering(false);
            });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void RenderScene_Should_EndRendering_WithWaitForVSync_BasedOnRenderingConfiguration(bool enableVSync)
        {
            // Arrange
            SetupVSync(enableVSync);

            var renderingSystem = GetRenderingSystem();
            var scene = TestSceneFactory.Create();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            _renderer2D.Received().EndRendering(enableVSync);
        }

        [Test]
        public void RenderScene_ShouldIgnoreOrderInLayer_WhenEntitiesAreInDifferentSortingLayers()
        {
            // Arrange
            const string otherSortingLayer = "Other";
            SetupSortingLayers(RenderingConfiguration.DefaultSortingLayerName, otherSortingLayer);

            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity1 = renderingSceneBuilder.AddSprite(orderInLayer: 0, sortingLayerName: otherSortingLayer);
            var entity2 = renderingSceneBuilder.AddSprite(orderInLayer: 1);
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix());
            });
        }

        [Test]
        public void RenderScene_ShouldNotRenderSprite_WhenSceneContainsEntityWithSpriteRendererAndTransformButDoesNotContainCamera()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddSprite();
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            _renderer2D.DidNotReceive().RenderSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>());
        }

        [Test]
        public void RenderScene_ShouldPerformCameraTransformationOnEntity_WhenSceneContainsEntityAndCamera()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();

            var cameraTransform = new Transform2DComponent
            {
                Translation = new Vector2(10, -10),
                Rotation = 0,
                Scale = Vector2.One
            };
            renderingSceneBuilder.AddCamera(cameraTransform);
            var entity = renderingSceneBuilder.AddSprite(Transform2DComponent.CreateDefault());
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            _renderer2D.Received(1).RenderSprite(entity.GetSprite(), Matrix3x3.CreateTranslation(new Vector2(-10, 10)));
        }

        [Test]
        public void RenderScene_ShouldApplyViewRectangleOfCamera_WhenSceneContainsEntityAndCamera()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();

            var cameraTransform = new Transform2DComponent
            {
                Translation = new Vector2(10, -10),
                Rotation = 0,
                Scale = Vector2.One
            };
            var cameraEntity = renderingSceneBuilder.AddCamera(cameraTransform);
            var camera = cameraEntity.GetComponent<CameraComponent>();

            // Camera view rectangle is twice the screen resolution
            camera.ViewRectangle = new Vector2(ScreenWidth * 2, ScreenHeight * 2);

            var entity = renderingSceneBuilder.AddSprite(Transform2DComponent.CreateDefault());
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            _renderer2D.Received(1).RenderSprite(entity.GetSprite(),
                // Sprite transform is half the scale and translation due to camera view rectangle 
                new Matrix3x3(m11: 0.5, m12: 0, m13: -5, m21: 0, m22: 0.5, m23: 5, m31: 0, m32: 0, m33: 1));
        }

        [Test]
        public void RenderScene_ShouldApplyViewRectangleOfCameraWithOverscanMatchedByHeight_WhenCameraAndScreenAspectRatioDiffers()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();

            var cameraTransform = new Transform2DComponent
            {
                Translation = new Vector2(10, -10),
                Rotation = 0,
                Scale = Vector2.One
            };
            var cameraEntity = renderingSceneBuilder.AddCamera(cameraTransform);
            var camera = cameraEntity.GetComponent<CameraComponent>();
            camera.AspectRatioBehavior = AspectRatioBehavior.Overscan;

            // Camera view rectangle 4xScreenWidth and 2xScreenHeight
            // Camera view rectangle is 4:1 ratio while screen is 2:1 ratio
            camera.ViewRectangle = new Vector2(ScreenWidth * 4, ScreenHeight * 2);

            var entity = renderingSceneBuilder.AddSprite(Transform2DComponent.CreateDefault());
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            _renderer2D.Received(1).RenderSprite(entity.GetSprite(),
                // Sprite transform is half the scale and translation due to camera view rectangle being scaled by height to match
                new Matrix3x3(m11: 0.5, m12: 0, m13: -5, m21: 0, m22: 0.5, m23: 5, m31: 0, m32: 0, m33: 1));
        }

        [Test]
        public void RenderScene_ShouldApplyViewRectangleOfCameraWithOverscanMatchedByWidth_WhenCameraAndScreenAspectRatioDiffers()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();

            var cameraTransform = new Transform2DComponent
            {
                Translation = new Vector2(10, -10),
                Rotation = 0,
                Scale = Vector2.One
            };
            var cameraEntity = renderingSceneBuilder.AddCamera(cameraTransform);
            var camera = cameraEntity.GetComponent<CameraComponent>();
            camera.AspectRatioBehavior = AspectRatioBehavior.Overscan;

            // Camera view rectangle 2xScreenWidth and 4xScreenHeight
            // Camera view rectangle is 1:1 ratio while screen is 2:1 ratio
            camera.ViewRectangle = new Vector2(ScreenWidth * 2, ScreenHeight * 4);

            var entity = renderingSceneBuilder.AddSprite(Transform2DComponent.CreateDefault());
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            _renderer2D.Received(1).RenderSprite(entity.GetSprite(),
                // Sprite transform is half the scale and translation due to camera view rectangle being scaled by width to match
                new Matrix3x3(m11: 0.5, m12: 0, m13: -5, m21: 0, m22: 0.5, m23: 5, m31: 0, m32: 0, m33: 1));
        }

        [Test]
        public void RenderScene_ShouldApplyViewRectangleOfCameraWithUnderscanMatchedByHeight_WhenCameraAndScreenAspectRatioDiffers()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();

            var cameraTransform = new Transform2DComponent
            {
                Translation = new Vector2(10, -10),
                Rotation = 0,
                Scale = Vector2.One
            };
            var cameraEntity = renderingSceneBuilder.AddCamera(cameraTransform);
            var camera = cameraEntity.GetComponent<CameraComponent>();
            camera.AspectRatioBehavior = AspectRatioBehavior.Underscan;

            // Camera view rectangle 1xScreenWidth and 2xScreenHeight
            // Camera view rectangle is 1:1 ratio while screen is 2:1 ratio
            camera.ViewRectangle = new Vector2(ScreenWidth, ScreenHeight * 2);

            var entity = renderingSceneBuilder.AddSprite(Transform2DComponent.CreateDefault());
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));
                _renderer2D.Clear(Color.FromArgb(255, 0, 0, 0));
                _renderer2D.SetClippingRectangle(new AxisAlignedRectangle(ScreenHeight, ScreenHeight));
                _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));
                _renderer2D.Received(1).RenderSprite(entity.GetSprite(),
                    // Sprite transform is half the scale and translation due to camera view rectangle being scaled by height to match
                    new Matrix3x3(m11: 0.5, m12: 0, m13: -5, m21: 0, m22: 0.5, m23: 5, m31: 0, m32: 0, m33: 1));
                _renderer2D.ClearClipping();
            });
        }

        [Test]
        public void RenderScene_ShouldApplyViewRectangleOfCameraWithUnderscanMatchedByWidth_WhenCameraAndScreenAspectRatioDiffers()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();

            var cameraTransform = new Transform2DComponent
            {
                Translation = new Vector2(10, -10),
                Rotation = 0,
                Scale = Vector2.One
            };
            var cameraEntity = renderingSceneBuilder.AddCamera(cameraTransform);
            var camera = cameraEntity.GetComponent<CameraComponent>();
            camera.AspectRatioBehavior = AspectRatioBehavior.Underscan;

            // Camera view rectangle 2xScreenWidth and 1xScreenHeight
            // Camera view rectangle is 4:1 ratio while screen is 2:1 ratio
            camera.ViewRectangle = new Vector2(ScreenWidth * 2, ScreenHeight);

            var entity = renderingSceneBuilder.AddSprite(Transform2DComponent.CreateDefault());
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));
                _renderer2D.Clear(Color.FromArgb(255, 0, 0, 0));
                _renderer2D.SetClippingRectangle(new AxisAlignedRectangle(ScreenWidth, ScreenHeight / 2d));
                _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));
                _renderer2D.Received(1).RenderSprite(entity.GetSprite(),
                    // Sprite transform is half the scale and translation due to camera view rectangle being scaled by width to match
                    new Matrix3x3(m11: 0.5, m12: 0, m13: -5, m21: 0, m22: 0.5, m23: 5, m31: 0, m32: 0, m33: 1));
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

            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity1 = renderingSceneBuilder.AddSprite(orderInLayer: 0);
            var entity2 = renderingSceneBuilder.AddSprite(orderInLayer: 1);
            var entity3 = renderingSceneBuilder.AddSprite(orderInLayer: 2);
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix());

                _renderer2D.RenderText(diagnosticInfo1.ToString(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3x3>());
                _renderer2D.RenderText(diagnosticInfo2.ToString(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3x3>());
                _renderer2D.RenderText(diagnosticInfo3.ToString(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3x3>());
            });
        }

        [Test]
        public void RenderScene_ShouldRenderInOrderOf_OrderInLayer_WhenEntitiesAreInTheSameSortingLayer()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity1 = renderingSceneBuilder.AddSprite(orderInLayer: 1);
            var entity2 = renderingSceneBuilder.AddSprite(orderInLayer: -1);
            var entity3 = renderingSceneBuilder.AddSprite(orderInLayer: 0);
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix());
            });
        }

        [Test]
        public void RenderScene_ShouldRenderInSortingLayersOrder_Default_Background_Foreground()
        {
            // Arrange
            const string backgroundSortingLayerName = "Background";
            const string foregroundSortingLayerName = "Foreground";
            SetupSortingLayers(RenderingConfiguration.DefaultSortingLayerName, backgroundSortingLayerName, foregroundSortingLayerName);

            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity1 = renderingSceneBuilder.AddSprite(sortingLayerName: foregroundSortingLayerName);
            var entity2 = renderingSceneBuilder.AddSprite(sortingLayerName: RenderingConfiguration.DefaultSortingLayerName);
            var entity3 = renderingSceneBuilder.AddSprite(sortingLayerName: backgroundSortingLayerName);
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix());
            });
        }

        [Test]
        public void RenderScene_ShouldRenderInSortingLayersOrder_Foreground_Background_Default()
        {
            // Arrange
            const string backgroundSortingLayerName = "Background";
            const string foregroundSortingLayerName = "Foreground";
            SetupSortingLayers(foregroundSortingLayerName, backgroundSortingLayerName, RenderingConfiguration.DefaultSortingLayerName);

            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity1 = renderingSceneBuilder.AddSprite(sortingLayerName: foregroundSortingLayerName);
            var entity2 = renderingSceneBuilder.AddSprite(sortingLayerName: RenderingConfiguration.DefaultSortingLayerName);
            var entity3 = renderingSceneBuilder.AddSprite(sortingLayerName: backgroundSortingLayerName);
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix());
            });
        }

        [Test]
        public void RenderScene_ShouldRenderOnlyEntities_ThatHaveVisibleSpriteRenderer()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity1 = renderingSceneBuilder.AddSprite(visible: true);
            var entity2 = renderingSceneBuilder.AddSprite(visible: false);
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            _renderer2D.Received(1).RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix());
            _renderer2D.DidNotReceive().RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix());
        }

        [Test]
        public void RenderScene_ShouldRenderSprite_WhenSceneContainsEntityWithSpriteRendererAndTransform()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity = renderingSceneBuilder.AddSprite();
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            _renderer2D.Received(1).RenderSprite(entity.GetSprite(), entity.Get2DTransformationMatrix());
        }

        [Test]
        public void RenderScene_ShouldRenderText_WhenSceneContainsEntityWithTextRendererAndTransform()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity = renderingSceneBuilder.AddText();
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            var textRenderer = entity.GetComponent<TextRendererComponent>();
            Debug.Assert(textRenderer.Text != null, "textRenderer.Text != null");
            _renderer2D.Received(1).RenderText(textRenderer.Text, textRenderer.FontSize, textRenderer.Color, entity.Get2DTransformationMatrix());
        }

        [Test]
        public void RenderScene_ShouldRenderRectangle_WhenSceneContainsEntityWithRectangleRendererAndTransform()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity = renderingSceneBuilder.AddRectangle();
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

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
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity = renderingSceneBuilder.AddEllipse();
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

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

            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            var cameraEntity = renderingSceneBuilder.AddCamera();
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            var cameraComponent = cameraEntity.GetComponent<CameraComponent>();
            Assert.That(cameraComponent.ScreenWidth, Is.EqualTo(screenWidth));
            Assert.That(cameraComponent.ScreenHeight, Is.EqualTo(screenHeight));
        }

        [Test]
        public void RenderScene_ShouldRenderEntityTransformedWithParentTransform_WhenEntityHasParentWithTransform2DComponent()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var (parentEntity, childEntity) = renderingSceneBuilder.AddParentEllipseWithChildEllipse();
            var scene = renderingSceneBuilder.Build();

            var parentExpectedTransform = parentEntity.Get2DTransformationMatrix();
            var childExpectedTransform = parentExpectedTransform * childEntity.Get2DTransformationMatrix();


            // Act
            renderingSystem.RenderScene(scene);

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
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity = renderingSceneBuilder.AddSprite();
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.RenderScene(scene);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity.GetSprite(), entity.Get2DTransformationMatrix());
                _debugRendererForRenderingSystem.Received(1).DrawDebugInformation(_renderer2D, Matrix3x3.Identity);
            });
        }

        private RenderingSystem GetRenderingSystem()
        {
            return new RenderingSystem(
                _renderingBackend,
                _renderingConfigurationBuilder.Build(),
                _aggregatedDiagnosticInfoProvider,
                _debugRendererForRenderingSystem
            );
        }

        private void SetupVSync(bool enableVSync)
        {
            _renderingConfigurationBuilder.WithEnableVSync(enableVSync);
        }

        private void SetupSortingLayers(params string[] sortingLayers)
        {
            _renderingConfigurationBuilder.WithSortingLayersOrder(sortingLayers);
        }

        private static DiagnosticInfo GetRandomDiagnosticInfo()
        {
            return new DiagnosticInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        }

        private class RenderingSceneBuilder
        {
            private readonly Scene _scene = TestSceneFactory.Create();

            public Entity AddCamera(Transform2DComponent? transformComponent = null)
            {
                var entity = new Entity();
                entity.AddComponent(transformComponent ?? Transform2DComponent.CreateDefault());
                entity.AddComponent(new CameraComponent
                {
                    ViewRectangle = new Vector2(ScreenWidth, ScreenHeight)
                });
                _scene.AddEntity(entity);

                return entity;
            }

            public Entity AddSprite(
                Transform2DComponent? transformComponent = null,
                int orderInLayer = 0,
                string sortingLayerName = RenderingConfiguration.DefaultSortingLayerName,
                bool visible = true)
            {
                var entity = new Entity();
                entity.AddComponent(transformComponent ?? RandomTransform2DComponent());
                entity.AddComponent(new SpriteRendererComponent
                {
                    Sprite = new Sprite(Substitute.For<ITexture>(), Vector2.Zero, Vector2.Zero, Vector2.Zero, 0),
                    OrderInLayer = orderInLayer,
                    SortingLayerName = sortingLayerName,
                    Visible = visible
                });
                _scene.AddEntity(entity);

                return entity;
            }

            public Entity AddText()
            {
                var entity = new Entity();
                entity.AddComponent(RandomTransform2DComponent());
                entity.AddComponent(new TextRendererComponent
                {
                    Text = Utils.Random.GetString(),
                    FontSize = FontSize.FromPoints(Utils.Random.NextDouble()),
                    Color = Color.FromArgb(Utils.Random.Next())
                });
                _scene.AddEntity(entity);

                return entity;
            }

            public Entity AddRectangle()
            {
                var entity = new Entity();
                entity.AddComponent(RandomTransform2DComponent());
                entity.AddComponent(new RectangleRendererComponent
                {
                    Dimension = Utils.RandomVector2(),
                    Color = Color.FromArgb(Utils.Random.Next()),
                    FillInterior = Utils.Random.NextBool()
                });
                _scene.AddEntity(entity);

                return entity;
            }

            public Entity AddEllipse()
            {
                var entity = CreateEllipse();
                _scene.AddEntity(entity);
                return entity;
            }

            public (Entity parent, Entity child) AddParentEllipseWithChildEllipse()
            {
                var parent = CreateEllipse();
                var child = CreateEllipse();
                parent.AddChild(child);
                _scene.AddEntity(parent);

                return (parent, child);
            }

            public Scene Build()
            {
                return _scene;
            }

            private static Transform2DComponent RandomTransform2DComponent()
            {
                return new Transform2DComponent
                {
                    Translation = Utils.RandomVector2(),
                    Rotation = Utils.Random.NextDouble(),
                    Scale = Utils.RandomVector2()
                };
            }

            private static Entity CreateEllipse()
            {
                var entity = new Entity();
                entity.AddComponent(RandomTransform2DComponent());
                entity.AddComponent(new EllipseRendererComponent
                {
                    RadiusX = Utils.Random.NextDouble(),
                    RadiusY = Utils.Random.NextDouble(),
                    Color = Color.FromArgb(Utils.Random.Next()),
                    FillInterior = Utils.Random.NextBool()
                });
                return entity;
            }
        }
    }

    internal static class EntityExtensions
    {
        public static Sprite GetSprite(this Entity entity) => entity.GetComponent<SpriteRendererComponent>().Sprite ??
                                                              throw new ArgumentException("Entity must have SpriteRendererComponent with non-null Sprite.");

        public static Matrix3x3 Get2DTransformationMatrix(this Entity entity) => entity.GetComponent<Transform2DComponent>().ToMatrix();
    }
}