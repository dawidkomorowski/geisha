using System.Collections.Generic;
using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Configuration;
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
        private const double DeltaTime = 0.1;
        private IRenderer2D _renderer2D;
        private IConfigurationManager _configurationManager;

        [SetUp]
        public void SetUp()
        {
            _renderer2D = Substitute.For<IRenderer2D>();
            _configurationManager = Substitute.For<IConfigurationManager>();
        }

        [Test]
        public void Update_ShouldClearOnce()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var renderingSystem = new RenderingSystem(_renderer2D, _configurationManager);
            var scene = new Scene();

            // Act
            renderingSystem.Update(scene, DeltaTime);

            // Assert
            _renderer2D.Received(1).Clear();
        }

        [Test]
        public void Update_ShouldRenderSpriteOnce_WhenSceneContainsEntityWithSpriteRendererAndTransform()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var renderingSystem = new RenderingSystem(_renderer2D, _configurationManager);
            var scene = new SceneWithTwoEntitiesButOnlyOneWithSpriteRendererAndTransform();

            // Act
            renderingSystem.Update(scene, DeltaTime);

            // Assert
            var sprite = scene.EntityWithSpriteRendererAndTransformSprite;
            var transform = scene.EntityWithSpriteRendererAndTransformTransformationMatrix;
            _renderer2D.Received(1).Render(sprite, transform);
        }

        [Test]
        public void Update_ShouldFirstClearThenRender()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var renderingSystem = new RenderingSystem(_renderer2D, _configurationManager);
            var scene = new SceneWithTwoEntitiesButOnlyOneWithSpriteRendererAndTransform();

            // Act
            renderingSystem.Update(scene, DeltaTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.Clear();
                _renderer2D.Render(Arg.Any<Sprite>(), Arg.Any<Matrix3>());
            });
        }

        [Test]
        public void Update_ShouldRenderInSortingLayersOrder_Default_Background_Foreground()
        {
            // Arrange
            SetupSortingLayers(RenderingDefaultConfigurationFactory.DefaultSortingLayerName,
                SceneWithEntitiesInDifferentSortingLayers.BackgroundSortingLayerName,
                SceneWithEntitiesInDifferentSortingLayers.ForegroundSortingLayerName);

            var renderingSystem = new RenderingSystem(_renderer2D, _configurationManager);
            var scene = new SceneWithEntitiesInDifferentSortingLayers();

            // Act
            renderingSystem.Update(scene, DeltaTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.Render(scene.EntityInDefaultLayerSprite, scene.EntityInDefaultLayerTransformationMatrix);
                _renderer2D.Render(scene.EntityInBackgroundLayerSprite, scene.EntityInBackgroundLayerTransformationMatrix);
                _renderer2D.Render(scene.EntityInForegroundLayerSprite, scene.EntityInForegroundLayerTransformationMatrix);
            });
        }

        [Test]
        public void Update_ShouldRenderInSortingLayersOrder_Foreground_Background_Default()
        {
            // Arrange
            SetupSortingLayers(SceneWithEntitiesInDifferentSortingLayers.ForegroundSortingLayerName,
                SceneWithEntitiesInDifferentSortingLayers.BackgroundSortingLayerName,
                RenderingDefaultConfigurationFactory.DefaultSortingLayerName);

            var renderingSystem = new RenderingSystem(_renderer2D, _configurationManager);
            var scene = new SceneWithEntitiesInDifferentSortingLayers();

            // Act
            renderingSystem.Update(scene, DeltaTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.Render(scene.EntityInForegroundLayerSprite, scene.EntityInForegroundLayerTransformationMatrix);
                _renderer2D.Render(scene.EntityInBackgroundLayerSprite, scene.EntityInBackgroundLayerTransformationMatrix);
                _renderer2D.Render(scene.EntityInDefaultLayerSprite, scene.EntityInDefaultLayerTransformationMatrix);
            });
        }

        [Test]
        public void Update_ShouldRenderInSortingOrder_WhenEntitiesAreInTheSameSortingLayer()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var renderingSystem = new RenderingSystem(_renderer2D, _configurationManager);
            var scene = new SceneWithThreeEntitiesWithTransformAndSpriteRenderer();

            scene.Entity1SpriteRenderer.SortingOrder = -1;
            scene.Entity2SpriteRenderer.SortingOrder = 0;
            scene.Entity3SpriteRenderer.SortingOrder = 1;

            // Act
            renderingSystem.Update(scene, DeltaTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.Render(scene.Entity1Sprite, scene.Entity1TransformationMatrix);
                _renderer2D.Render(scene.Entity2Sprite, scene.Entity2TransformationMatrix);
                _renderer2D.Render(scene.Entity3Sprite, scene.Entity3TransformationMatrix);
            });
        }

        [Test]
        public void Update_ShouldRenderIgnoreSortingOrder_WhenEntitiesAreInDifferentSortingLayers()
        {
            // Arrange
            const string otherSortingLayer = "Other";
            SetupSortingLayers(RenderingDefaultConfigurationFactory.DefaultSortingLayerName, otherSortingLayer);

            var renderingSystem = new RenderingSystem(_renderer2D, _configurationManager);
            var scene = new SceneWithThreeEntitiesWithTransformAndSpriteRenderer();

            scene.Entity1SpriteRenderer.SortingOrder = 0;
            scene.Entity1SpriteRenderer.SortingLayerName = otherSortingLayer;

            scene.Entity2SpriteRenderer.SortingOrder = 1;

            // Act
            renderingSystem.Update(scene, DeltaTime);

            // Assert
            Received.InOrder(() =>
            {
                _renderer2D.Render(scene.Entity2Sprite, scene.Entity2TransformationMatrix);
                _renderer2D.Render(scene.Entity1Sprite, scene.Entity1TransformationMatrix);
            });
        }

        [Test]
        public void Update_ShouldRenderOnlyEntities_ThatHaveVisibleSpriteRenderer()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var renderingSystem = new RenderingSystem(_renderer2D, _configurationManager);
            var scene = new SceneWithThreeEntitiesWithTransformAndSpriteRenderer();

            scene.Entity1SpriteRenderer.Visible = true;
            scene.Entity2SpriteRenderer.Visible = false;

            // Act
            renderingSystem.Update(scene, DeltaTime);

            // Assert
            _renderer2D.Received(1).Render(scene.Entity1Sprite, scene.Entity1TransformationMatrix);
            _renderer2D.DidNotReceive().Render(scene.Entity2Sprite, scene.Entity2TransformationMatrix);
        }

        [Test]
        public void FixedUpdate_ShouldClearOnce()
        {
            // Arrange
            SetupDefaultSortingLayers();

            var renderingSystem = new RenderingSystem(_renderer2D, _configurationManager);
            var scene = new Scene();

            // Act
            renderingSystem.FixedUpdate(scene);

            // Assert
            _renderer2D.Received(1).Clear();
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

        private class SceneWithTwoEntitiesButOnlyOneWithSpriteRendererAndTransform : Scene
        {
            public Matrix3 EntityWithSpriteRendererAndTransformTransformationMatrix { get; }
            public Sprite EntityWithSpriteRendererAndTransformSprite { get; }

            public SceneWithTwoEntitiesButOnlyOneWithSpriteRendererAndTransform()
            {
                EntityWithSpriteRendererAndTransformSprite = new Sprite();
                var spriteRenderer = new SpriteRenderer {Sprite = EntityWithSpriteRendererAndTransformSprite};

                var transform = new Transform {Translation = new Vector3(1, 2, 3), Rotation = new Vector3(1, 2, 3), Scale = new Vector3(1, 2, 3)};
                EntityWithSpriteRendererAndTransformTransformationMatrix = transform.Create2DTransformationMatrix();

                var entityWithSpriteRendererAndTransform = new Entity();
                entityWithSpriteRendererAndTransform.AddComponent(spriteRenderer);
                entityWithSpriteRendererAndTransform.AddComponent(transform);

                AddEntity(entityWithSpriteRendererAndTransform);
            }
        }

        private class SceneWithEntitiesInDifferentSortingLayers : Scene
        {
            public Matrix3 EntityInDefaultLayerTransformationMatrix { get; }
            public Matrix3 EntityInBackgroundLayerTransformationMatrix { get; }
            public Matrix3 EntityInForegroundLayerTransformationMatrix { get; }

            public Sprite EntityInDefaultLayerSprite { get; }
            public Sprite EntityInBackgroundLayerSprite { get; }
            public Sprite EntityInForegroundLayerSprite { get; }

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
        }

        private class SceneWithThreeEntitiesWithTransformAndSpriteRenderer : Scene
        {
            public Matrix3 Entity1TransformationMatrix { get; }
            public Matrix3 Entity2TransformationMatrix { get; }
            public Matrix3 Entity3TransformationMatrix { get; }

            public Sprite Entity1Sprite { get; }
            public Sprite Entity2Sprite { get; }
            public Sprite Entity3Sprite { get; }

            public SpriteRenderer Entity1SpriteRenderer { get; }
            public SpriteRenderer Entity2SpriteRenderer { get; }
            public SpriteRenderer Entity3SpriteRenderer { get; }

            public SceneWithThreeEntitiesWithTransformAndSpriteRenderer()
            {
                Entity1Sprite = new Sprite();
                Entity2Sprite = new Sprite();
                Entity3Sprite = new Sprite();

                Entity1SpriteRenderer = new SpriteRenderer
                {
                    Sprite = Entity1Sprite,
                };

                Entity2SpriteRenderer = new SpriteRenderer
                {
                    Sprite = Entity2Sprite,
                };

                Entity3SpriteRenderer = new SpriteRenderer
                {
                    Sprite = Entity3Sprite,
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

                Entity1TransformationMatrix = entity1Transform.Create2DTransformationMatrix();
                Entity2TransformationMatrix = entity2Transform.Create2DTransformationMatrix();
                Entity3TransformationMatrix = entity3Transform.Create2DTransformationMatrix();

                var entity1 = new Entity();
                var entity2 = new Entity();
                var entity3 = new Entity();

                entity1.AddComponent(entity1Transform);
                entity2.AddComponent(entity2Transform);
                entity3.AddComponent(entity3Transform);

                entity1.AddComponent(Entity1SpriteRenderer);
                entity2.AddComponent(Entity2SpriteRenderer);
                entity3.AddComponent(Entity3SpriteRenderer);

                AddEntity(entity1);
                AddEntity(entity2);
                AddEntity(entity3);
            }
        }
    }
}