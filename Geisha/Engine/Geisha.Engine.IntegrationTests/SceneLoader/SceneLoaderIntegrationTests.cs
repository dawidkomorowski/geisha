using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Definition;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;
using Geisha.Framework.Input;
using Geisha.Framework.Rendering;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests.SceneLoader
{
    [Export]
    public class SceneLoaderIntegrationTestsSut
    {
        [ImportingConstructor]
        public SceneLoaderIntegrationTestsSut(IAssetStore assetStore, ISceneLoader sceneLoader)
        {
            AssetStore = assetStore;
            SceneLoader = sceneLoader;
        }

        public IAssetStore AssetStore { get; }
        public ISceneLoader SceneLoader { get; }
    }

    [TestFixture]
    public class SceneLoaderIntegrationTests : IntegrationTests<SceneLoaderIntegrationTestsSut>
    {
        private string _sceneFilePath;

        public override void SetUp()
        {
            base.SetUp();
            _sceneFilePath = GetRandomFilePath();
        }

        public override void TearDown()
        {
            base.TearDown();

            if (File.Exists(_sceneFilePath))
            {
                File.Delete(_sceneFilePath);
            }
        }

        #region No components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEmptyEntity()
        {
            // Arrange
            var scene = new Scene();

            var emptyEntity = NewEntityWithRandomName();
            scene.AddEntity(emptyEntity);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            Assert.That(loadedScene, Is.Not.Null);
            Assert.That(loadedScene.RootEntities.Count, Is.EqualTo(scene.RootEntities.Count));
            Assert.That(loadedScene.AllEntities.Count(), Is.EqualTo(scene.AllEntities.Count()));

            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), emptyEntity);
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithChildren()
        {
            // Arrange
            var scene = new Scene();

            var entityWithChildren = NewEntityWithRandomName();
            entityWithChildren.AddChild(NewEntityWithRandomName());
            entityWithChildren.AddChild(NewEntityWithRandomName());
            entityWithChildren.AddChild(NewEntityWithRandomName());
            scene.AddEntity(entityWithChildren);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            Assert.That(loadedScene, Is.Not.Null);
            Assert.That(loadedScene.RootEntities.Count, Is.EqualTo(scene.RootEntities.Count));
            Assert.That(loadedScene.AllEntities.Count(), Is.EqualTo(scene.AllEntities.Count()));

            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithChildren);
        }

        #endregion

        #region Core components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithTransform()
        {
            // Arrange
            var scene = new Scene();

            var entityWithTransform = NewEntityWithRandomName();
            entityWithTransform.AddComponent(new Transform
            {
                Translation = new Vector3(Random.NextDouble(), Random.NextDouble(), Random.NextDouble()),
                Rotation = new Vector3(Random.NextDouble(), Random.NextDouble(), Random.NextDouble()),
                Scale = new Vector3(Random.NextDouble(), Random.NextDouble(), Random.NextDouble())
            });
            scene.AddEntity(entityWithTransform);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            Assert.That(loadedScene, Is.Not.Null);
            Assert.That(loadedScene.RootEntities.Count, Is.EqualTo(scene.RootEntities.Count));
            Assert.That(loadedScene.AllEntities.Count(), Is.EqualTo(scene.AllEntities.Count()));

            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithTransform);
            var transform = entityWithTransform.GetComponent<Transform>();
            var loadedTransform = loadedScene.RootEntities.Single().GetComponent<Transform>();
            Assert.That(loadedTransform.Translation, Is.EqualTo(transform.Translation));
            Assert.That(loadedTransform.Rotation, Is.EqualTo(transform.Rotation));
            Assert.That(loadedTransform.Scale, Is.EqualTo(transform.Scale));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithBehavior()
        {
            // Arrange
            var scene = new Scene();

            var entityWithBehavior = NewEntityWithRandomName();
            entityWithBehavior.AddComponent(new TestBehavior
            {
                IntProperty = Random.Next(),
                DoubleProperty = Random.NextDouble(),
                StringProperty = Random.GetString()
            });
            scene.AddEntity(entityWithBehavior);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            Assert.That(loadedScene, Is.Not.Null);
            Assert.That(loadedScene.RootEntities.Count, Is.EqualTo(scene.RootEntities.Count));
            Assert.That(loadedScene.AllEntities.Count(), Is.EqualTo(scene.AllEntities.Count()));

            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithBehavior);
            var testBehavior = entityWithBehavior.GetComponent<TestBehavior>();
            var loadedTestBehavior = loadedScene.RootEntities.Single().GetComponent<TestBehavior>();
            Assert.That(loadedTestBehavior.IntProperty, Is.EqualTo(testBehavior.IntProperty));
            Assert.That(loadedTestBehavior.DoubleProperty, Is.EqualTo(testBehavior.DoubleProperty));
            Assert.That(loadedTestBehavior.StringProperty, Is.EqualTo(testBehavior.StringProperty));
        }

        #endregion

        #region Physics components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithCircleCollider()
        {
            // Arrange
            var scene = new Scene();

            var entityWithCircleCollider = NewEntityWithRandomName();
            entityWithCircleCollider.AddComponent(new CircleCollider
            {
                Radius = Random.NextDouble()
            });
            scene.AddEntity(entityWithCircleCollider);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            Assert.That(loadedScene, Is.Not.Null);
            Assert.That(loadedScene.RootEntities.Count, Is.EqualTo(scene.RootEntities.Count));
            Assert.That(loadedScene.AllEntities.Count(), Is.EqualTo(scene.AllEntities.Count()));

            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithCircleCollider);
            var circleCollider = entityWithCircleCollider.GetComponent<CircleCollider>();
            var loadedCircleCollider = loadedScene.RootEntities.Single().GetComponent<CircleCollider>();
            Assert.That(loadedCircleCollider.Radius, Is.EqualTo(circleCollider.Radius));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithRectangleCollider()
        {
            // Arrange
            var scene = new Scene();

            var entityWithRectangleCollider = NewEntityWithRandomName();
            entityWithRectangleCollider.AddComponent(new RectangleCollider
            {
                Dimension = NewRandomVector2()
            });
            scene.AddEntity(entityWithRectangleCollider);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            Assert.That(loadedScene, Is.Not.Null);
            Assert.That(loadedScene.RootEntities.Count, Is.EqualTo(scene.RootEntities.Count));
            Assert.That(loadedScene.AllEntities.Count(), Is.EqualTo(scene.AllEntities.Count()));

            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithRectangleCollider);
            var rectangleCollider = entityWithRectangleCollider.GetComponent<RectangleCollider>();
            var loadedRectangleCollider = loadedScene.RootEntities.Single().GetComponent<RectangleCollider>();
            Assert.That(loadedRectangleCollider.Dimension, Is.EqualTo(rectangleCollider.Dimension));
        }

        #endregion

        #region Rendering components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithCamera()
        {
            // Arrange
            var scene = new Scene();

            var entityWithCamera = NewEntityWithRandomName();
            entityWithCamera.AddComponent(new Camera());
            scene.AddEntity(entityWithCamera);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            Assert.That(loadedScene, Is.Not.Null);
            Assert.That(loadedScene.RootEntities.Count, Is.EqualTo(scene.RootEntities.Count));
            Assert.That(loadedScene.AllEntities.Count(), Is.EqualTo(scene.AllEntities.Count()));

            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithCamera);
            Assert.That(loadedScene.RootEntities.Single().HasComponent<Camera>(), Is.True);
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithTextRenderer()
        {
            // Arrange
            var scene = new Scene();

            var entityWithTextRenderer = NewEntityWithRandomName();
            entityWithTextRenderer.AddComponent(new TextRenderer
            {
                Text = Random.GetString(),
                FontSize = Random.Next(),
                Color = Color.FromArgb(Random.Next()),
                Visible = Random.NextBool(),
                SortingLayerName = Random.GetString(),
                OrderInLayer = Random.Next()
            });
            scene.AddEntity(entityWithTextRenderer);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            Assert.That(loadedScene, Is.Not.Null);
            Assert.That(loadedScene.RootEntities.Count, Is.EqualTo(scene.RootEntities.Count));
            Assert.That(loadedScene.AllEntities.Count(), Is.EqualTo(scene.AllEntities.Count()));

            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithTextRenderer);
            var textRenderer = entityWithTextRenderer.GetComponent<TextRenderer>();
            var loadedTextRenderer = loadedScene.RootEntities.Single().GetComponent<TextRenderer>();
            Assert.That(loadedTextRenderer.Text, Is.EqualTo(textRenderer.Text));
            Assert.That(loadedTextRenderer.FontSize, Is.EqualTo(textRenderer.FontSize));
            Assert.That(loadedTextRenderer.Color, Is.EqualTo(textRenderer.Color));
            Assert.That(loadedTextRenderer.Visible, Is.EqualTo(textRenderer.Visible));
            Assert.That(loadedTextRenderer.SortingLayerName, Is.EqualTo(textRenderer.SortingLayerName));
            Assert.That(loadedTextRenderer.OrderInLayer, Is.EqualTo(textRenderer.OrderInLayer));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithSpriteRenderer()
        {
            // Arrange
            var spriteAssetId = Guid.NewGuid();
            SystemUnderTest.AssetStore.RegisterAsset(new AssetInfo(typeof(Sprite), spriteAssetId,
                GetPathUnderTestDirectory(@"SceneLoader\Assets\TestSprite.sprite")));

            var scene = new Scene();

            var entityWithSpriteRenderer = NewEntityWithRandomName();
            entityWithSpriteRenderer.AddComponent(new SpriteRenderer
            {
                Sprite = SystemUnderTest.AssetStore.GetAsset<Sprite>(spriteAssetId),
                Visible = Random.NextBool(),
                SortingLayerName = Random.GetString(),
                OrderInLayer = Random.Next()
            });
            scene.AddEntity(entityWithSpriteRenderer);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            Assert.That(loadedScene, Is.Not.Null);
            Assert.That(loadedScene.RootEntities.Count, Is.EqualTo(scene.RootEntities.Count));
            Assert.That(loadedScene.AllEntities.Count(), Is.EqualTo(scene.AllEntities.Count()));

            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithSpriteRenderer);
            var spriteRenderer = entityWithSpriteRenderer.GetComponent<SpriteRenderer>();
            var loadedSpriteRenderer = loadedScene.RootEntities.Single().GetComponent<SpriteRenderer>();
            Assert.That(loadedSpriteRenderer.Sprite.PixelsPerUnit, Is.EqualTo(spriteRenderer.Sprite.PixelsPerUnit));
            Assert.That(loadedSpriteRenderer.Sprite.Rectangle.LowerLeft, Is.EqualTo(spriteRenderer.Sprite.Rectangle.LowerLeft));
            Assert.That(loadedSpriteRenderer.Sprite.Rectangle.LowerRight, Is.EqualTo(spriteRenderer.Sprite.Rectangle.LowerRight));
            Assert.That(loadedSpriteRenderer.Sprite.Rectangle.UpperLeft, Is.EqualTo(spriteRenderer.Sprite.Rectangle.UpperLeft));
            Assert.That(loadedSpriteRenderer.Sprite.Rectangle.UpperRight, Is.EqualTo(spriteRenderer.Sprite.Rectangle.UpperRight));
            Assert.That(loadedSpriteRenderer.Sprite.SourceAnchor, Is.EqualTo(spriteRenderer.Sprite.SourceAnchor));
            Assert.That(loadedSpriteRenderer.Sprite.SourceDimension, Is.EqualTo(spriteRenderer.Sprite.SourceDimension));
            Assert.That(loadedSpriteRenderer.Sprite.SourceTexture.Dimension, Is.EqualTo(spriteRenderer.Sprite.SourceTexture.Dimension));
            Assert.That(loadedSpriteRenderer.Sprite.SourceUV, Is.EqualTo(spriteRenderer.Sprite.SourceUV));
            Assert.That(loadedSpriteRenderer.Visible, Is.EqualTo(spriteRenderer.Visible));
            Assert.That(loadedSpriteRenderer.SortingLayerName, Is.EqualTo(spriteRenderer.SortingLayerName));
            Assert.That(loadedSpriteRenderer.OrderInLayer, Is.EqualTo(spriteRenderer.OrderInLayer));
        }

        #endregion

        #region Input components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithInputComponent()
        {
            // Arrange
            var inputMappingAssetId = Guid.NewGuid();
            SystemUnderTest.AssetStore.RegisterAsset(new AssetInfo(typeof(InputMapping), inputMappingAssetId,
                GetPathUnderTestDirectory(@"SceneLoader\Assets\TestInputMapping.input")));

            var scene = new Scene();

            var entityWithInputComponent = NewEntityWithRandomName();
            entityWithInputComponent.AddComponent(new InputComponent
            {
                InputMapping = SystemUnderTest.AssetStore.GetAsset<InputMapping>(inputMappingAssetId)
            });
            scene.AddEntity(entityWithInputComponent);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            Assert.That(loadedScene, Is.Not.Null);
            Assert.That(loadedScene.RootEntities.Count, Is.EqualTo(scene.RootEntities.Count));
            Assert.That(loadedScene.AllEntities.Count(), Is.EqualTo(scene.AllEntities.Count()));

            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithInputComponent);
            var inputComponent = entityWithInputComponent.GetComponent<InputComponent>();
            var loadedInputComponent = loadedScene.RootEntities.Single().GetComponent<InputComponent>();
            Assert.That(loadedInputComponent.HardwareInput, Is.EqualTo(HardwareInput.Empty));

            Assert.That(loadedInputComponent.InputMapping.ActionMappings.Count, Is.EqualTo(inputComponent.InputMapping.ActionMappings.Count));
            Assert.That(loadedInputComponent.InputMapping.ActionMappings[0].ActionName, Is.EqualTo(inputComponent.InputMapping.ActionMappings[0].ActionName));
            Assert.That(loadedInputComponent.InputMapping.ActionMappings[0].HardwareActions.Count,
                Is.EqualTo(inputComponent.InputMapping.ActionMappings[0].HardwareActions.Count));
            Assert.That(loadedInputComponent.InputMapping.ActionMappings[0].HardwareActions[0].HardwareInputVariant,
                Is.EqualTo(inputComponent.InputMapping.ActionMappings[0].HardwareActions[0].HardwareInputVariant));

            Assert.That(loadedInputComponent.InputMapping.AxisMappings.Count, Is.EqualTo(inputComponent.InputMapping.AxisMappings.Count));
            // Axis mapping 1
            Assert.That(loadedInputComponent.InputMapping.AxisMappings[0].AxisName, Is.EqualTo(inputComponent.InputMapping.AxisMappings[0].AxisName));
            Assert.That(loadedInputComponent.InputMapping.AxisMappings[0].HardwareAxes.Count,
                Is.EqualTo(inputComponent.InputMapping.AxisMappings[0].HardwareAxes.Count));
            Assert.That(loadedInputComponent.InputMapping.AxisMappings[0].HardwareAxes[0].HardwareInputVariant,
                Is.EqualTo(inputComponent.InputMapping.AxisMappings[0].HardwareAxes[0].HardwareInputVariant));
            Assert.That(loadedInputComponent.InputMapping.AxisMappings[0].HardwareAxes[0].Scale,
                Is.EqualTo(inputComponent.InputMapping.AxisMappings[0].HardwareAxes[0].Scale));
            Assert.That(loadedInputComponent.InputMapping.AxisMappings[0].HardwareAxes[1].HardwareInputVariant,
                Is.EqualTo(inputComponent.InputMapping.AxisMappings[0].HardwareAxes[1].HardwareInputVariant));
            Assert.That(loadedInputComponent.InputMapping.AxisMappings[0].HardwareAxes[1].Scale,
                Is.EqualTo(inputComponent.InputMapping.AxisMappings[0].HardwareAxes[1].Scale));
            // Axis mapping 2
            Assert.That(loadedInputComponent.InputMapping.AxisMappings[1].AxisName, Is.EqualTo(inputComponent.InputMapping.AxisMappings[1].AxisName));
            Assert.That(loadedInputComponent.InputMapping.AxisMappings[1].HardwareAxes.Count,
                Is.EqualTo(inputComponent.InputMapping.AxisMappings[1].HardwareAxes.Count));
            Assert.That(loadedInputComponent.InputMapping.AxisMappings[1].HardwareAxes[0].HardwareInputVariant,
                Is.EqualTo(inputComponent.InputMapping.AxisMappings[1].HardwareAxes[0].HardwareInputVariant));
            Assert.That(loadedInputComponent.InputMapping.AxisMappings[1].HardwareAxes[0].Scale,
                Is.EqualTo(inputComponent.InputMapping.AxisMappings[1].HardwareAxes[0].Scale));
            Assert.That(loadedInputComponent.InputMapping.AxisMappings[1].HardwareAxes[1].HardwareInputVariant,
                Is.EqualTo(inputComponent.InputMapping.AxisMappings[1].HardwareAxes[1].HardwareInputVariant));
            Assert.That(loadedInputComponent.InputMapping.AxisMappings[1].HardwareAxes[1].Scale,
                Is.EqualTo(inputComponent.InputMapping.AxisMappings[1].HardwareAxes[1].Scale));
        }

        #endregion

        #region Helpers

        private Entity NewEntityWithRandomName()
        {
            return new Entity
            {
                Name = Random.GetString()
            };
        }

        private static void AssertEntitiesAreEqual(Entity entity1, Entity entity2)
        {
            Assert.That(entity1.Name, Is.EqualTo(entity2.Name));
            Assert.That(entity1.Components.Count, Is.EqualTo(entity2.Components.Count));
            Assert.That(entity1.IsRoot, Is.EqualTo(entity2.IsRoot));
            Assert.That(entity1.Children.Count, Is.EqualTo(entity2.Children.Count));
            for (var i = 0; i < entity1.Children.Count; i++)
            {
                AssertEntitiesAreEqual(entity1.Children[i], entity2.Children[i]);
            }
        }

        [ComponentDefinition]
        private class TestBehavior : Behavior
        {
            [PropertyDefinition]
            public int IntProperty { get; set; }

            [PropertyDefinition]
            public double DoubleProperty { get; set; }

            [PropertyDefinition]
            public string StringProperty { get; set; }
        }

        #endregion
    }
}