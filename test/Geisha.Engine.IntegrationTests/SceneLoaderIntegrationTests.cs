using System.Diagnostics;
using System.IO;
using System.Linq;
using Geisha.Common.TestUtils;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Geisha.IntegrationTestsData;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests
{
    public sealed class SceneLoaderIntegrationTestsSut
    {
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
        private string _sceneFilePath = null!;

        public override void SetUp()
        {
            base.SetUp();
            _sceneFilePath = Utils.GetRandomFilePath();

            SystemUnderTest.AssetStore.RegisterAssets(Utils.GetPathUnderTestDirectory(@"Assets"));
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
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithConstructionScript()
        {
            // Arrange
            var scene = new Scene {ConstructionScript = Utils.Random.GetString()};

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEmptyEntity()
        {
            // Arrange
            var scene = new Scene();

            var emptyEntity = CreateNewEntityWithRandomName();
            scene.AddEntity(emptyEntity);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), emptyEntity);
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithChildren()
        {
            // Arrange
            var scene = new Scene();

            var entityWithChildren = CreateNewEntityWithRandomName();
            entityWithChildren.AddChild(CreateNewEntityWithRandomName());
            entityWithChildren.AddChild(CreateNewEntityWithRandomName());
            entityWithChildren.AddChild(CreateNewEntityWithRandomName());
            scene.AddEntity(entityWithChildren);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithChildren);
        }

        #endregion

        #region Audio components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithAudioSource()
        {
            // Arrange
            var scene = new Scene();

            var entityWithAudioSource = CreateNewEntityWithRandomName();
            entityWithAudioSource.AddComponent(new AudioSourceComponent
            {
                Sound = SystemUnderTest.AssetStore.GetAsset<ISound>(AssetsIds.TestSound)
            });
            scene.AddEntity(entityWithAudioSource);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithAudioSource);
            var audioSource = entityWithAudioSource.GetComponent<AudioSourceComponent>();
            var loadedAudioSource = loadedScene.RootEntities.Single().GetComponent<AudioSourceComponent>();
            Assert.That(loadedAudioSource.IsPlaying, Is.EqualTo(audioSource.IsPlaying));
            Debug.Assert(loadedAudioSource.Sound != null, "loadedAudioSource.Sound != null");
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedAudioSource.Sound), Is.EqualTo(AssetsIds.TestSound));
        }

        #endregion

        #region Animation components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithSpriteAnimationComponent()
        {
            // Arrange
            var scene = new Scene();

            var entityWithSpriteAnimation = CreateNewEntityWithRandomName();
            var spriteAnimationComponent = new SpriteAnimationComponent();
            entityWithSpriteAnimation.AddComponent(spriteAnimationComponent);
            scene.AddEntity(entityWithSpriteAnimation);

            spriteAnimationComponent.AddAnimation("animation", SystemUnderTest.AssetStore.GetAsset<SpriteAnimation>(AssetsIds.TestSpriteAnimation));
            spriteAnimationComponent.Position = 0.7;
            spriteAnimationComponent.PlaybackSpeed = 1.3;
            spriteAnimationComponent.PlayAnimation("animation");

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithSpriteAnimation);
            var loadedSpriteAnimationComponent = loadedScene.RootEntities.Single().GetComponent<SpriteAnimationComponent>();
            Assert.That(loadedSpriteAnimationComponent.Animations, Has.Count.EqualTo(1));
            Assert.That(loadedSpriteAnimationComponent.Animations.Single().Key, Is.EqualTo("animation"));
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedSpriteAnimationComponent.Animations.Single().Value),
                Is.EqualTo(AssetsIds.TestSpriteAnimation));
            Assert.That(loadedSpriteAnimationComponent.Position, Is.EqualTo(spriteAnimationComponent.Position));
            Assert.That(loadedSpriteAnimationComponent.PlaybackSpeed, Is.EqualTo(spriteAnimationComponent.PlaybackSpeed));
            Debug.Assert(loadedSpriteAnimationComponent.CurrentAnimation != null, "loadedSpriteAnimationComponent.CurrentAnimation != null");
            Assert.That(loadedSpriteAnimationComponent.CurrentAnimation.Value.Name, Is.EqualTo("animation"));
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedSpriteAnimationComponent.CurrentAnimation.Value.Animation),
                Is.EqualTo(AssetsIds.TestSpriteAnimation));
        }

        #endregion

        #region Core components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithTransform2D()
        {
            // Arrange
            var scene = new Scene();

            var entityWithTransform = CreateNewEntityWithRandomName();
            entityWithTransform.AddComponent(new Transform2DComponent
            {
                Translation = Utils.RandomVector2(),
                Rotation = Utils.Random.NextDouble(),
                Scale = Utils.RandomVector2()
            });
            scene.AddEntity(entityWithTransform);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithTransform);
            var transform = entityWithTransform.GetComponent<Transform2DComponent>();
            var loadedTransform = loadedScene.RootEntities.Single().GetComponent<Transform2DComponent>();
            Assert.That(loadedTransform.Translation, Is.EqualTo(transform.Translation));
            Assert.That(loadedTransform.Rotation, Is.EqualTo(transform.Rotation));
            Assert.That(loadedTransform.Scale, Is.EqualTo(transform.Scale));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithTransform3D()
        {
            // Arrange
            var scene = new Scene();

            var entityWithTransform = CreateNewEntityWithRandomName();
            entityWithTransform.AddComponent(new Transform3DComponent
            {
                Translation = Utils.RandomVector3(),
                Rotation = Utils.RandomVector3(),
                Scale = Utils.RandomVector3()
            });
            scene.AddEntity(entityWithTransform);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithTransform);
            var transform = entityWithTransform.GetComponent<Transform3DComponent>();
            var loadedTransform = loadedScene.RootEntities.Single().GetComponent<Transform3DComponent>();
            Assert.That(loadedTransform.Translation, Is.EqualTo(transform.Translation));
            Assert.That(loadedTransform.Rotation, Is.EqualTo(transform.Rotation));
            Assert.That(loadedTransform.Scale, Is.EqualTo(transform.Scale));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithBehavior()
        {
            // Arrange
            var scene = new Scene();

            var entityWithBehavior = CreateNewEntityWithRandomName();
            entityWithBehavior.AddComponent(new TestBehaviorComponent
            {
                IntProperty = Utils.Random.Next(),
                DoubleProperty = Utils.Random.NextDouble(),
                StringProperty = Utils.Random.GetString()
            });
            scene.AddEntity(entityWithBehavior);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithBehavior);
            var testBehavior = entityWithBehavior.GetComponent<TestBehaviorComponent>();
            var loadedTestBehavior = loadedScene.RootEntities.Single().GetComponent<TestBehaviorComponent>();
            Assert.That(loadedTestBehavior.IntProperty, Is.EqualTo(testBehavior.IntProperty));
            Assert.That(loadedTestBehavior.DoubleProperty, Is.EqualTo(testBehavior.DoubleProperty));
            Assert.That(loadedTestBehavior.StringProperty, Is.EqualTo(testBehavior.StringProperty));
        }

        #endregion

        #region Input components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithInputComponent()
        {
            // Arrange
            var scene = new Scene();

            var entityWithInputComponent = CreateNewEntityWithRandomName();
            entityWithInputComponent.AddComponent(new InputComponent
            {
                InputMapping = SystemUnderTest.AssetStore.GetAsset<InputMapping>(AssetsIds.TestInputMapping)
            });
            scene.AddEntity(entityWithInputComponent);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithInputComponent);
            var loadedInputComponent = loadedScene.RootEntities.Single().GetComponent<InputComponent>();
            Assert.That(loadedInputComponent.HardwareInput, Is.EqualTo(HardwareInput.Empty));
            Debug.Assert(loadedInputComponent.InputMapping != null, "loadedInputComponent.InputMapping != null");
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedInputComponent.InputMapping), Is.EqualTo(AssetsIds.TestInputMapping));
        }

        #endregion

        #region Physics components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithCircleCollider()
        {
            // Arrange
            var scene = new Scene();

            var entityWithCircleCollider = CreateNewEntityWithRandomName();
            entityWithCircleCollider.AddComponent(new CircleColliderComponent
            {
                Radius = Utils.Random.NextDouble()
            });
            scene.AddEntity(entityWithCircleCollider);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithCircleCollider);
            var circleCollider = entityWithCircleCollider.GetComponent<CircleColliderComponent>();
            var loadedCircleCollider = loadedScene.RootEntities.Single().GetComponent<CircleColliderComponent>();
            Assert.That(loadedCircleCollider.Radius, Is.EqualTo(circleCollider.Radius));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithRectangleCollider()
        {
            // Arrange
            var scene = new Scene();

            var entityWithRectangleCollider = CreateNewEntityWithRandomName();
            entityWithRectangleCollider.AddComponent(new RectangleColliderComponent
            {
                Dimension = Utils.RandomVector2()
            });
            scene.AddEntity(entityWithRectangleCollider);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithRectangleCollider);
            var rectangleCollider = entityWithRectangleCollider.GetComponent<RectangleColliderComponent>();
            var loadedRectangleCollider = loadedScene.RootEntities.Single().GetComponent<RectangleColliderComponent>();
            Assert.That(loadedRectangleCollider.Dimension, Is.EqualTo(rectangleCollider.Dimension));
        }

        #endregion

        #region Rendering components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithCamera()
        {
            // Arrange
            var scene = new Scene();

            var entityWithCamera = CreateNewEntityWithRandomName();
            entityWithCamera.AddComponent(new CameraComponent
            {
                AspectRatioBehavior = Utils.Random.NextEnum<AspectRatioBehavior>(),
                ViewRectangle = Utils.RandomVector2()
            });
            scene.AddEntity(entityWithCamera);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithCamera);
            var cameraComponent = entityWithCamera.GetComponent<CameraComponent>();
            var loadedCameraComponent = loadedScene.RootEntities.Single().GetComponent<CameraComponent>();
            Assert.That(loadedCameraComponent.AspectRatioBehavior, Is.EqualTo(cameraComponent.AspectRatioBehavior));
            Assert.That(loadedCameraComponent.ViewRectangle.X, Is.EqualTo(cameraComponent.ViewRectangle.X));
            Assert.That(loadedCameraComponent.ViewRectangle.Y, Is.EqualTo(cameraComponent.ViewRectangle.Y));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithTextRenderer()
        {
            // Arrange
            var scene = new Scene();

            var entityWithTextRenderer = CreateNewEntityWithRandomName();
            entityWithTextRenderer.AddComponent(new TextRendererComponent
            {
                Text = Utils.Random.GetString(),
                FontSize = FontSize.FromPoints(Utils.Random.Next()),
                Color = Color.FromArgb(Utils.Random.Next()),
                Visible = Utils.Random.NextBool(),
                SortingLayerName = Utils.Random.GetString(),
                OrderInLayer = Utils.Random.Next()
            });
            scene.AddEntity(entityWithTextRenderer);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithTextRenderer);
            var textRenderer = entityWithTextRenderer.GetComponent<TextRendererComponent>();
            var loadedTextRenderer = loadedScene.RootEntities.Single().GetComponent<TextRendererComponent>();
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
            var scene = new Scene();

            var entityWithSpriteRenderer = CreateNewEntityWithRandomName();
            entityWithSpriteRenderer.AddComponent(new SpriteRendererComponent
            {
                Sprite = SystemUnderTest.AssetStore.GetAsset<Sprite>(AssetsIds.TestSprite),
                Visible = Utils.Random.NextBool(),
                SortingLayerName = Utils.Random.GetString(),
                OrderInLayer = Utils.Random.Next()
            });
            scene.AddEntity(entityWithSpriteRenderer);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            Assert.That(loadedScene.RootEntities.Count, Is.EqualTo(scene.RootEntities.Count));
            Assert.That(loadedScene.AllEntities.Count(), Is.EqualTo(scene.AllEntities.Count()));

            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithSpriteRenderer);
            var spriteRenderer = entityWithSpriteRenderer.GetComponent<SpriteRendererComponent>();
            var loadedSpriteRenderer = loadedScene.RootEntities.Single().GetComponent<SpriteRendererComponent>();
            Debug.Assert(loadedSpriteRenderer.Sprite != null, "loadedSpriteRenderer.Sprite != null");
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedSpriteRenderer.Sprite), Is.EqualTo(AssetsIds.TestSprite));
            Assert.That(loadedSpriteRenderer.Visible, Is.EqualTo(spriteRenderer.Visible));
            Assert.That(loadedSpriteRenderer.SortingLayerName, Is.EqualTo(spriteRenderer.SortingLayerName));
            Assert.That(loadedSpriteRenderer.OrderInLayer, Is.EqualTo(spriteRenderer.OrderInLayer));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithRectangleRenderer()
        {
            // Arrange
            var scene = new Scene();

            var entityWithRectangleRenderer = CreateNewEntityWithRandomName();
            entityWithRectangleRenderer.AddComponent(new RectangleRendererComponent
            {
                Dimension = Utils.RandomVector2(),
                FillInterior = Utils.Random.NextBool(),
                Color = Color.FromArgb(Utils.Random.Next()),
                Visible = Utils.Random.NextBool(),
                SortingLayerName = Utils.Random.GetString(),
                OrderInLayer = Utils.Random.Next()
            });
            scene.AddEntity(entityWithRectangleRenderer);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithRectangleRenderer);
            var rectangleRenderer = entityWithRectangleRenderer.GetComponent<RectangleRendererComponent>();
            var loadedRectangleRenderer = loadedScene.RootEntities.Single().GetComponent<RectangleRendererComponent>();
            Assert.That(loadedRectangleRenderer.Dimension, Is.EqualTo(rectangleRenderer.Dimension));
            Assert.That(loadedRectangleRenderer.FillInterior, Is.EqualTo(rectangleRenderer.FillInterior));
            Assert.That(loadedRectangleRenderer.Color, Is.EqualTo(rectangleRenderer.Color));
            Assert.That(loadedRectangleRenderer.Visible, Is.EqualTo(rectangleRenderer.Visible));
            Assert.That(loadedRectangleRenderer.SortingLayerName, Is.EqualTo(rectangleRenderer.SortingLayerName));
            Assert.That(loadedRectangleRenderer.OrderInLayer, Is.EqualTo(rectangleRenderer.OrderInLayer));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithEllipseRenderer()
        {
            // Arrange
            var scene = new Scene();

            var entityWithEllipseRenderer = CreateNewEntityWithRandomName();
            entityWithEllipseRenderer.AddComponent(new EllipseRendererComponent
            {
                RadiusX = Utils.Random.NextDouble(),
                RadiusY = Utils.Random.NextDouble(),
                FillInterior = Utils.Random.NextBool(),
                Color = Color.FromArgb(Utils.Random.Next()),
                Visible = Utils.Random.NextBool(),
                SortingLayerName = Utils.Random.GetString(),
                OrderInLayer = Utils.Random.Next()
            });
            scene.AddEntity(entityWithEllipseRenderer);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithEllipseRenderer);
            var ellipseRenderer = entityWithEllipseRenderer.GetComponent<EllipseRendererComponent>();
            var loadedEllipseRenderer = loadedScene.RootEntities.Single().GetComponent<EllipseRendererComponent>();
            Assert.That(loadedEllipseRenderer.RadiusX, Is.EqualTo(ellipseRenderer.RadiusX));
            Assert.That(loadedEllipseRenderer.RadiusY, Is.EqualTo(ellipseRenderer.RadiusY));
            Assert.That(loadedEllipseRenderer.FillInterior, Is.EqualTo(ellipseRenderer.FillInterior));
            Assert.That(loadedEllipseRenderer.Color, Is.EqualTo(ellipseRenderer.Color));
            Assert.That(loadedEllipseRenderer.Visible, Is.EqualTo(ellipseRenderer.Visible));
            Assert.That(loadedEllipseRenderer.SortingLayerName, Is.EqualTo(ellipseRenderer.SortingLayerName));
            Assert.That(loadedEllipseRenderer.OrderInLayer, Is.EqualTo(ellipseRenderer.OrderInLayer));
        }

        #endregion

        #region Helpers

        private static Entity CreateNewEntityWithRandomName()
        {
            return new Entity
            {
                Name = Utils.Random.GetString()
            };
        }

        private static void AssertScenesAreEqual(Scene scene1, Scene scene2)
        {
            Assert.That(scene1.RootEntities.Count, Is.EqualTo(scene2.RootEntities.Count));
            Assert.That(scene1.AllEntities.Count(), Is.EqualTo(scene2.AllEntities.Count()));
            Assert.That(scene1.ConstructionScript, Is.EqualTo(scene2.ConstructionScript));
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

        [SerializableComponent]
        private class TestBehaviorComponent : BehaviorComponent
        {
            [SerializableProperty]
            public int IntProperty { get; set; }

            [SerializableProperty]
            public double DoubleProperty { get; set; }

            [SerializableProperty]
            public string StringProperty { get; set; } = string.Empty;
        }

        #endregion
    }
}