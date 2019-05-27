using System;
using System.Collections.Generic;
using Geisha.Common.Math;
using Geisha.Common.TestUtils;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Configuration;
using Geisha.Engine.Rendering.Systems;
using Geisha.Framework.Rendering;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Systems
{
    [TestFixture]
    public class RenderingSystemTests
    {
        private readonly GameTime _gameTime = new GameTime(TimeSpan.FromSeconds(0.1));
        private IRenderer2D _renderer2D;
        private IConfigurationManager _configurationManager;
        private IAggregatedDiagnosticInfoProvider _aggregatedDiagnosticInfoProvider;
        private RenderingConfiguration _renderingConfiguration;

        [SetUp]
        public void SetUp()
        {
            _renderer2D = Substitute.For<IRenderer2D>();
            _configurationManager = Substitute.For<IConfigurationManager>();
            _aggregatedDiagnosticInfoProvider = Substitute.For<IAggregatedDiagnosticInfoProvider>();

            _renderingConfiguration = new RenderingConfiguration();
            _configurationManager.GetConfiguration<RenderingConfiguration>().Returns(_renderingConfiguration);
        }

        [Test]
        public void Update_Should_BeginRendering_Clear_EndRendering_GivenAnEmptyScene()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var scene = new Scene();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.BeginRendering();
                _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));
                _renderer2D.EndRendering(false);
            });
        }

        [Test]
        public void Update_ShouldCallInFollowingOrder_BeginRendering_Clear_RenderSprite_EndRendering()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            renderingSceneBuilder.AddSprite();
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.Update(scene, _gameTime);

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
        public void Update_Should_EndRendering_WithWaitForVSync_BasedOnRenderingConfiguration(bool enableVSync)
        {
            // Arrange
            SetupVSync(enableVSync);

            var renderingSystem = GetRenderingSystem();
            var scene = new Scene();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            _renderer2D.Received().EndRendering(enableVSync);
        }

        [Test]
        public void Update_ShouldIgnoreOrderInLayer_WhenEntitiesAreInDifferentSortingLayers()
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
            renderingSystem.Update(scene, _gameTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix());
            });
        }

        [Test]
        public void Update_ShouldNotRenderSprite_WhenSceneContainsEntityWithSpriteRendererAndTransformButDoesNotContainCamera()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddSprite();
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            _renderer2D.DidNotReceive().RenderSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3x3>());
        }

        [Test]
        public void Update_ShouldPerformCameraTransformationOnEntity_WhenSceneContainsEntityAndCamera()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();

            var cameraTransform = new TransformComponent
            {
                Translation = new Vector3(10, -10, 0),
                Rotation = Vector3.Zero,
                Scale = Vector3.One
            };
            renderingSceneBuilder.AddCamera(cameraTransform);
            var entity = renderingSceneBuilder.AddSprite(TransformComponent.Default);
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            _renderer2D.Received(1).RenderSprite(entity.GetSprite(), Matrix3x3.CreateTranslation(new Vector2(-10, 10)));
        }

        [Test]
        public void Update_ShouldRenderDiagnosticInfo_AfterRenderingScene()
        {
            // Arrange
            var diagnosticInfo1 = GetRandomDiagnosticInfo();
            var diagnosticInfo2 = GetRandomDiagnosticInfo();
            var diagnosticInfo3 = GetRandomDiagnosticInfo();

            _aggregatedDiagnosticInfoProvider.GetAllDiagnosticInfo().Returns(new[] {diagnosticInfo1, diagnosticInfo2, diagnosticInfo3});

            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity1 = renderingSceneBuilder.AddSprite(orderInLayer: 0);
            var entity2 = renderingSceneBuilder.AddSprite(orderInLayer: 1);
            var entity3 = renderingSceneBuilder.AddSprite(orderInLayer: 2);
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.Update(scene, _gameTime);

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
        public void Update_ShouldRenderInOrderOf_OrderInLayer_WhenEntitiesAreInTheSameSortingLayer()
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
            renderingSystem.Update(scene, _gameTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix());
            });
        }

        [Test]
        public void Update_ShouldRenderInSortingLayersOrder_Default_Background_Foreground()
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
            renderingSystem.Update(scene, _gameTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix());
            });
        }

        [Test]
        public void Update_ShouldRenderInSortingLayersOrder_Foreground_Background_Default()
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
            renderingSystem.Update(scene, _gameTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix());
                _renderer2D.RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix());
            });
        }

        [Test]
        public void Update_ShouldRenderOnlyEntities_ThatHaveVisibleSpriteRenderer()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity1 = renderingSceneBuilder.AddSprite(visible: true);
            var entity2 = renderingSceneBuilder.AddSprite(visible: false);
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            _renderer2D.Received(1).RenderSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix());
            _renderer2D.DidNotReceive().RenderSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix());
        }

        [Test]
        public void Update_ShouldRenderSprite_WhenSceneContainsEntityWithSpriteRendererAndTransform()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity = renderingSceneBuilder.AddSprite();
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            _renderer2D.Received(1).RenderSprite(entity.GetSprite(), entity.Get2DTransformationMatrix());
        }

        [Test]
        public void Update_ShouldRenderText_WhenSceneContainsEntityWithTextRendererAndTransform()
        {
            // Arrange
            var renderingSystem = GetRenderingSystem();
            var renderingSceneBuilder = new RenderingSceneBuilder();
            renderingSceneBuilder.AddCamera();
            var entity = renderingSceneBuilder.AddText();
            var scene = renderingSceneBuilder.Build();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            var textRenderer = entity.GetComponent<TextRendererComponent>();
            _renderer2D.Received(1).RenderText(textRenderer.Text, textRenderer.FontSize, textRenderer.Color, entity.Get2DTransformationMatrix());
        }

        private RenderingSystem GetRenderingSystem()
        {
            return new RenderingSystem(_renderer2D, _configurationManager, _aggregatedDiagnosticInfoProvider);
        }

        private void SetupSortingLayers(params string[] sortingLayers)
        {
            _renderingConfiguration.SortingLayersOrder = new List<string>(sortingLayers);
        }

        private void SetupVSync(bool enableVSync)
        {
            _renderingConfiguration.EnableVSync = enableVSync;
        }

        private static DiagnosticInfo GetRandomDiagnosticInfo()
        {
            return new DiagnosticInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        }

        private class RenderingSceneBuilder
        {
            private readonly Scene _scene = new Scene();

            public Entity AddCamera(TransformComponent transformComponent = null)
            {
                var entity = new Entity();
                entity.AddComponent(transformComponent ?? TransformComponent.Default);
                entity.AddComponent(new CameraComponent());
                _scene.AddEntity(entity);

                return entity;
            }

            public Entity AddSprite(
                TransformComponent transformComponent = null,
                int orderInLayer = 0,
                string sortingLayerName = RenderingConfiguration.DefaultSortingLayerName,
                bool visible = true)
            {
                var entity = new Entity();
                entity.AddComponent(transformComponent ?? RandomTransformComponent());
                entity.AddComponent(new SpriteRendererComponent
                {
                    Sprite = new Sprite(),
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
                entity.AddComponent(RandomTransformComponent());
                entity.AddComponent(new TextRendererComponent
                {
                    Text = Utils.Random.GetString(),
                    FontSize = FontSize.FromPoints(Utils.Random.NextDouble()),
                    Color = Color.FromArgb(Utils.Random.Next())
                });
                _scene.AddEntity(entity);

                return entity;
            }

            public Scene Build()
            {
                return _scene;
            }

            private static TransformComponent RandomTransformComponent()
            {
                return new TransformComponent
                {
                    Translation = Utils.RandomVector3(),
                    Rotation = Utils.RandomVector3(),
                    Scale = Utils.RandomVector3()
                };
            }
        }
    }

    internal static class EntityExtensions
    {
        public static Sprite GetSprite(this Entity entity) => entity.GetComponent<SpriteRendererComponent>().Sprite;
        public static Matrix3x3 Get2DTransformationMatrix(this Entity entity) => entity.GetComponent<TransformComponent>().Create2DTransformationMatrix();
    }
}