using System;
using System.Collections.Generic;
using Geisha.Common.Math;
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

namespace Geisha.Engine.Rendering.UnitTests.Systems
{
    [TestFixture]
    public class RenderingSystemTests
    {
        [SetUp]
        public void SetUp()
        {
            _renderer2D = Substitute.For<IRenderer2D>();
            _configurationManager = Substitute.For<IConfigurationManager>();
            _aggregatedDiagnosticInfoProvider = Substitute.For<IAggregatedDiagnosticInfoProvider>();
        }

        private readonly GameTime _gameTime = new GameTime(TimeSpan.FromSeconds(0.1));
        private IRenderer2D _renderer2D;
        private IConfigurationManager _configurationManager;
        private IAggregatedDiagnosticInfoProvider _aggregatedDiagnosticInfoProvider;

        private RenderingSystem GetRenderingSystem()
        {
            return new RenderingSystem(_renderer2D, _configurationManager, _aggregatedDiagnosticInfoProvider);
        }

        private void SetupSortingLayers(params string[] sortingLayers)
        {
            _configurationManager.GetConfiguration<RenderingConfiguration>()
                .Returns(new RenderingConfiguration
                {
                    SortingLayersOrder = new List<string>(sortingLayers)
                });
        }

        private void SetupDefaultSortingLayers()
        {
            SetupSortingLayers(RenderingDefaultConfigurationFactory.DefaultSortingLayerName);
        }

        private static DiagnosticInfo GetRandomDiagnosticInfo()
        {
            return new DiagnosticInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        }

        private class SceneWithCamera : Scene
        {
            public void AddCamera(Transform transform = null)
            {
                var cameraEntity = new Entity();
                cameraEntity.AddComponent(transform ?? Transform.Default);
                cameraEntity.AddComponent(new Camera());

                AddEntity(cameraEntity);
            }
        }

        private class SceneWithEntityWithSpriteRendererAndWithEntityWithTextRenderer : SceneWithCamera
        {
            public SceneWithEntityWithSpriteRendererAndWithEntityWithTextRenderer()
            {
                var entityWithSpriteRendererTransform = new Transform
                {
                    Translation = new Vector3(1, 2, 3),
                    Rotation = new Vector3(1, 2, 3),
                    Scale = new Vector3(1, 2, 3)
                };
                EntityWithSpriteRendererAndTransformTransformationMatrix = entityWithSpriteRendererTransform.Create2DTransformationMatrix();

                var entityWithTextRendererTransform = new Transform
                {
                    Translation = new Vector3(2, 3, 4),
                    Rotation = new Vector3(2, 3, 4),
                    Scale = new Vector3(2, 3, 4)
                };
                EntityWithTextRendererAndTransformTransformationMatrix = entityWithTextRendererTransform.Create2DTransformationMatrix();

                EntityWithSpriteRendererAndTransformSprite = new Sprite();
                var spriteRenderer = new SpriteRenderer {Sprite = EntityWithSpriteRendererAndTransformSprite};

                EntityWithTextRendererAndTransformTextRenderer = new TextRenderer
                {
                    Text = nameof(EntityWithTextRendererAndTransformTextRenderer),
                    FontSize = FontSize.FromPoints(24),
                    Color = Color.FromArgb(1234)
                };

                var entityWithSpriteRendererAndTransform = new Entity();
                entityWithSpriteRendererAndTransform.AddComponent(entityWithSpriteRendererTransform);
                entityWithSpriteRendererAndTransform.AddComponent(spriteRenderer);

                var entityWithTextRendererAndTransform = new Entity();
                entityWithTextRendererAndTransform.AddComponent(EntityWithTextRendererAndTransformTextRenderer);
                entityWithTextRendererAndTransform.AddComponent(entityWithTextRendererTransform);

                AddEntity(entityWithSpriteRendererAndTransform);
                AddEntity(entityWithTextRendererAndTransform);
            }

            public Matrix3 EntityWithSpriteRendererAndTransformTransformationMatrix { get; }
            public Matrix3 EntityWithTextRendererAndTransformTransformationMatrix { get; }

            public Sprite EntityWithSpriteRendererAndTransformSprite { get; }

            public TextRenderer EntityWithTextRendererAndTransformTextRenderer { get; }
        }

        private class SceneWithEntitiesInDifferentSortingLayers : SceneWithCamera
        {
            public const string BackgroundSortingLayerName = "Background";
            public const string ForegroundSortingLayerName = "Foreground";

            public SceneWithEntitiesInDifferentSortingLayers()
            {
                EntityInDefaultLayerSprite = new Sprite();
                EntityInBackgroundLayerSprite = new Sprite();
                EntityInForegroundLayerSprite = new Sprite();

                var entityInDefaultLayerSpriteRenderer = new SpriteRenderer
                {
                    Sprite = EntityInDefaultLayerSprite,
                    SortingLayerName = RenderingDefaultConfigurationFactory.DefaultSortingLayerName
                };

                var entityInBackgroundLayerSpriteRenderer = new SpriteRenderer
                {
                    Sprite = EntityInBackgroundLayerSprite,
                    SortingLayerName = BackgroundSortingLayerName
                };

                var entityInForegroundLayerSpriteRenderer = new SpriteRenderer
                {
                    Sprite = EntityInForegroundLayerSprite,
                    SortingLayerName = ForegroundSortingLayerName
                };

                var entityInDefaultLayerTransform = new Transform
                {
                    Translation = new Vector3(1, 2, 3),
                    Rotation = new Vector3(1, 2, 3),
                    Scale = new Vector3(1, 2, 3)
                };

                var entityInBackgroundLayerTransform = new Transform
                {
                    Translation = new Vector3(2, 3, 4),
                    Rotation = new Vector3(2, 3, 4),
                    Scale = new Vector3(2, 3, 4)
                };

                var entityInForegroundLayerTransform = new Transform
                {
                    Translation = new Vector3(3, 4, 5),
                    Rotation = new Vector3(3, 4, 5),
                    Scale = new Vector3(3, 4, 5)
                };

                EntityInDefaultLayerTransformationMatrix = entityInDefaultLayerTransform.Create2DTransformationMatrix();
                EntityInBackgroundLayerTransformationMatrix = entityInBackgroundLayerTransform.Create2DTransformationMatrix();
                EntityInForegroundLayerTransformationMatrix = entityInForegroundLayerTransform.Create2DTransformationMatrix();

                var entityInDefaultLayer = new Entity();
                var entityInBackgroundLayer = new Entity();
                var entityInForegroundLayer = new Entity();

                entityInDefaultLayer.AddComponent(entityInDefaultLayerTransform);
                entityInBackgroundLayer.AddComponent(entityInBackgroundLayerTransform);
                entityInForegroundLayer.AddComponent(entityInForegroundLayerTransform);

                entityInDefaultLayer.AddComponent(entityInDefaultLayerSpriteRenderer);
                entityInBackgroundLayer.AddComponent(entityInBackgroundLayerSpriteRenderer);
                entityInForegroundLayer.AddComponent(entityInForegroundLayerSpriteRenderer);

                AddEntity(entityInDefaultLayer);
                AddEntity(entityInBackgroundLayer);
                AddEntity(entityInForegroundLayer);
            }

            public Matrix3 EntityInDefaultLayerTransformationMatrix { get; }
            public Matrix3 EntityInBackgroundLayerTransformationMatrix { get; }
            public Matrix3 EntityInForegroundLayerTransformationMatrix { get; }

            public Sprite EntityInDefaultLayerSprite { get; }
            public Sprite EntityInBackgroundLayerSprite { get; }
            public Sprite EntityInForegroundLayerSprite { get; }
        }

        private class SceneWithEntitiesWithTransformAndSpriteRenderer : SceneWithCamera
        {
            public SceneWithEntitiesWithTransformAndSpriteRenderer()
            {
                Entity1Sprite = new Sprite();
                Entity2Sprite = new Sprite();
                Entity3Sprite = new Sprite();
                EntityWithDefaultTransformSprite = new Sprite();

                Entity1SpriteRenderer = new SpriteRenderer
                {
                    Sprite = Entity1Sprite
                };

                Entity2SpriteRenderer = new SpriteRenderer
                {
                    Sprite = Entity2Sprite
                };

                Entity3SpriteRenderer = new SpriteRenderer
                {
                    Sprite = Entity3Sprite
                };

                EntityWithDefaultTransformSpriteRenderer = new SpriteRenderer
                {
                    Sprite = EntityWithDefaultTransformSprite
                };

                var entity1Transform = new Transform
                {
                    Translation = new Vector3(1, 2, 3),
                    Rotation = new Vector3(1, 2, 3),
                    Scale = new Vector3(1, 2, 3)
                };

                var entity2Transform = new Transform
                {
                    Translation = new Vector3(2, 3, 4),
                    Rotation = new Vector3(2, 3, 4),
                    Scale = new Vector3(2, 3, 4)
                };

                var entity3Transform = new Transform
                {
                    Translation = new Vector3(3, 4, 5),
                    Rotation = new Vector3(3, 4, 5),
                    Scale = new Vector3(3, 4, 5)
                };

                var entityWithDefaultTransformTransform = Transform.Default;

                Entity1TransformationMatrix = entity1Transform.Create2DTransformationMatrix();
                Entity2TransformationMatrix = entity2Transform.Create2DTransformationMatrix();
                Entity3TransformationMatrix = entity3Transform.Create2DTransformationMatrix();
                EntityWithDefaultTransformMatrix = entityWithDefaultTransformTransform.Create2DTransformationMatrix();

                var entity1 = new Entity();
                var entity2 = new Entity();
                var entity3 = new Entity();
                var entityWithDefaultTransform = new Entity();

                entity1.AddComponent(entity1Transform);
                entity2.AddComponent(entity2Transform);
                entity3.AddComponent(entity3Transform);
                entityWithDefaultTransform.AddComponent(entityWithDefaultTransformTransform);

                entity1.AddComponent(Entity1SpriteRenderer);
                entity2.AddComponent(Entity2SpriteRenderer);
                entity3.AddComponent(Entity3SpriteRenderer);
                entityWithDefaultTransform.AddComponent(EntityWithDefaultTransformSpriteRenderer);

                AddEntity(entity1);
                AddEntity(entity2);
                AddEntity(entity3);
                AddEntity(entityWithDefaultTransform);
            }

            public Matrix3 Entity1TransformationMatrix { get; }
            public Matrix3 Entity2TransformationMatrix { get; }
            public Matrix3 Entity3TransformationMatrix { get; }
            public Matrix3 EntityWithDefaultTransformMatrix { get; }

            public Sprite Entity1Sprite { get; }
            public Sprite Entity2Sprite { get; }
            public Sprite Entity3Sprite { get; }
            public Sprite EntityWithDefaultTransformSprite { get; }

            public SpriteRenderer Entity1SpriteRenderer { get; }
            public SpriteRenderer Entity2SpriteRenderer { get; }
            public SpriteRenderer Entity3SpriteRenderer { get; }
            public SpriteRenderer EntityWithDefaultTransformSpriteRenderer { get; }
        }

        [Test]
        public void Update_Should_BeginRendering_Clear_EndRendering_GivenAnEmptyScene()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var renderingSystem = GetRenderingSystem();
            var scene = new Scene();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.BeginRendering();
                _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));
                _renderer2D.EndRendering();
            });
        }

        [Test]
        public void Update_ShouldCallInFollowingOrder_BeginRendering_Clear_RenderSprite_EndRendering()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var renderingSystem = GetRenderingSystem();
            var scene = new SceneWithEntityWithSpriteRendererAndWithEntityWithTextRenderer();
            scene.AddCamera();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.BeginRendering();
                _renderer2D.Clear(Color.FromArgb(255, 255, 255, 255));
                _renderer2D.RenderSprite(Arg.Any<Sprite>(), Arg.Any<Matrix3>());
                _renderer2D.EndRendering();
            });
        }

        [Test]
        public void Update_ShouldIgnoreOrderInLayer_WhenEntitiesAreInDifferentSortingLayers()
        {
            // Arrange
            const string otherSortingLayer = "Other";
            SetupSortingLayers(RenderingDefaultConfigurationFactory.DefaultSortingLayerName, otherSortingLayer);

            var renderingSystem = GetRenderingSystem();
            var scene = new SceneWithEntitiesWithTransformAndSpriteRenderer();
            scene.AddCamera();

            scene.Entity1SpriteRenderer.OrderInLayer = 0;
            scene.Entity1SpriteRenderer.SortingLayerName = otherSortingLayer;

            scene.Entity2SpriteRenderer.OrderInLayer = 1;

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(scene.Entity2Sprite, scene.Entity2TransformationMatrix);
                _renderer2D.RenderSprite(scene.Entity1Sprite, scene.Entity1TransformationMatrix);
            });
        }

        [Test]
        public void Update_ShouldNotRenderSprite_WhenSceneContainsEntityWithSpriteRendererAndTransformButDoesNotContainCamera()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var renderingSystem = GetRenderingSystem();
            var scene = new SceneWithEntityWithSpriteRendererAndWithEntityWithTextRenderer();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            var sprite = scene.EntityWithSpriteRendererAndTransformSprite;
            _renderer2D.DidNotReceive().RenderSprite(sprite, Arg.Any<Matrix3>());
        }

        [Test]
        public void Update_ShouldPerformCameraTransformationOnEntity_WhenSceneContainsEntityAndCamera()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var renderingSystem = GetRenderingSystem();
            var scene = new SceneWithEntitiesWithTransformAndSpriteRenderer();

            var cameraTransform = new Transform
            {
                Translation = new Vector3(10, -10, 0),
                Rotation = Vector3.Zero,
                Scale = Vector3.One
            };
            scene.AddCamera(cameraTransform);

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            var sprite = scene.EntityWithDefaultTransformSprite;
            _renderer2D.Received(1).RenderSprite(sprite, Matrix3.Translation(new Vector2(-10, 10)));
        }

        [Test]
        public void Update_ShouldRenderDiagnosticInfo_AfterRenderingScene()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var diagnosticInfo1 = GetRandomDiagnosticInfo();
            var diagnosticInfo2 = GetRandomDiagnosticInfo();
            var diagnosticInfo3 = GetRandomDiagnosticInfo();

            _aggregatedDiagnosticInfoProvider.GetAllDiagnosticInfo().Returns(new[] {diagnosticInfo1, diagnosticInfo2, diagnosticInfo3});

            var renderingSystem = GetRenderingSystem();
            var scene = new SceneWithEntitiesWithTransformAndSpriteRenderer();
            scene.AddCamera();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(scene.Entity1Sprite, scene.Entity1TransformationMatrix);
                _renderer2D.RenderSprite(scene.Entity2Sprite, scene.Entity2TransformationMatrix);
                _renderer2D.RenderSprite(scene.Entity3Sprite, scene.Entity3TransformationMatrix);

                _renderer2D.RenderText(diagnosticInfo1.ToString(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3>());
                _renderer2D.RenderText(diagnosticInfo2.ToString(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3>());
                _renderer2D.RenderText(diagnosticInfo3.ToString(), Arg.Any<FontSize>(), Arg.Any<Color>(), Arg.Any<Matrix3>());
            });
        }

        [Test]
        public void Update_ShouldRenderInOrderOf_OrderInLayer_WhenEntitiesAreInTheSameSortingLayer()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var renderingSystem = GetRenderingSystem();
            var scene = new SceneWithEntitiesWithTransformAndSpriteRenderer();
            scene.AddCamera();

            scene.Entity1SpriteRenderer.OrderInLayer = -1;
            scene.Entity2SpriteRenderer.OrderInLayer = 0;
            scene.Entity3SpriteRenderer.OrderInLayer = 1;

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(scene.Entity1Sprite, scene.Entity1TransformationMatrix);
                _renderer2D.RenderSprite(scene.Entity2Sprite, scene.Entity2TransformationMatrix);
                _renderer2D.RenderSprite(scene.Entity3Sprite, scene.Entity3TransformationMatrix);
            });
        }

        [Test]
        public void Update_ShouldRenderInSortingLayersOrder_Default_Background_Foreground()
        {
            // Arrange
            SetupSortingLayers(RenderingDefaultConfigurationFactory.DefaultSortingLayerName,
                SceneWithEntitiesInDifferentSortingLayers.BackgroundSortingLayerName,
                SceneWithEntitiesInDifferentSortingLayers.ForegroundSortingLayerName);

            var renderingSystem = GetRenderingSystem();
            var scene = new SceneWithEntitiesInDifferentSortingLayers();
            scene.AddCamera();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(scene.EntityInDefaultLayerSprite, scene.EntityInDefaultLayerTransformationMatrix);
                _renderer2D.RenderSprite(scene.EntityInBackgroundLayerSprite, scene.EntityInBackgroundLayerTransformationMatrix);
                _renderer2D.RenderSprite(scene.EntityInForegroundLayerSprite, scene.EntityInForegroundLayerTransformationMatrix);
            });
        }

        [Test]
        public void Update_ShouldRenderInSortingLayersOrder_Foreground_Background_Default()
        {
            // Arrange
            SetupSortingLayers(SceneWithEntitiesInDifferentSortingLayers.ForegroundSortingLayerName,
                SceneWithEntitiesInDifferentSortingLayers.BackgroundSortingLayerName,
                RenderingDefaultConfigurationFactory.DefaultSortingLayerName);

            var renderingSystem = GetRenderingSystem();
            var scene = new SceneWithEntitiesInDifferentSortingLayers();
            scene.AddCamera();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.RenderSprite(scene.EntityInForegroundLayerSprite, scene.EntityInForegroundLayerTransformationMatrix);
                _renderer2D.RenderSprite(scene.EntityInBackgroundLayerSprite, scene.EntityInBackgroundLayerTransformationMatrix);
                _renderer2D.RenderSprite(scene.EntityInDefaultLayerSprite, scene.EntityInDefaultLayerTransformationMatrix);
            });
        }

        [Test]
        public void Update_ShouldRenderOnlyEntities_ThatHaveVisibleSpriteRenderer()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var renderingSystem = GetRenderingSystem();
            var scene = new SceneWithEntitiesWithTransformAndSpriteRenderer();
            scene.AddCamera();

            scene.Entity1SpriteRenderer.Visible = true;
            scene.Entity2SpriteRenderer.Visible = false;

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            _renderer2D.Received(1).RenderSprite(scene.Entity1Sprite, scene.Entity1TransformationMatrix);
            _renderer2D.DidNotReceive().RenderSprite(scene.Entity2Sprite, scene.Entity2TransformationMatrix);
        }

        [Test]
        public void Update_ShouldRenderSprite_WhenSceneContainsEntityWithSpriteRendererAndTransform()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var renderingSystem = GetRenderingSystem();
            var scene = new SceneWithEntityWithSpriteRendererAndWithEntityWithTextRenderer();
            scene.AddCamera();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            var sprite = scene.EntityWithSpriteRendererAndTransformSprite;
            var transform = scene.EntityWithSpriteRendererAndTransformTransformationMatrix;
            _renderer2D.Received(1).RenderSprite(sprite, transform);
        }

        [Test]
        public void Update_ShouldRenderText_WhenSceneContainsEntityWithTextRendererAndTransform()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var renderingSystem = GetRenderingSystem();
            var scene = new SceneWithEntityWithSpriteRendererAndWithEntityWithTextRenderer();
            scene.AddCamera();

            // Act
            renderingSystem.Update(scene, _gameTime);

            // Assert
            var textRenderer = scene.EntityWithTextRendererAndTransformTextRenderer;
            var transform = scene.EntityWithTextRendererAndTransformTransformationMatrix;
            _renderer2D.Received(1).RenderText(textRenderer.Text, textRenderer.FontSize, textRenderer.Color, transform);
        }
    }
}