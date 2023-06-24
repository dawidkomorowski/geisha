using System;
using System.Diagnostics;
using System.Linq;
using Autofac;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Geisha.IntegrationTestsData;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests.Core
{
    public sealed class SceneLoaderIntegrationTestsSut
    {
        public SceneLoaderIntegrationTestsSut(IAssetStore assetStore, ISceneFactory sceneFactory, ISceneLoader sceneLoader)
        {
            AssetStore = assetStore;
            SceneFactory = sceneFactory;
            SceneLoader = sceneLoader;
        }

        public IAssetStore AssetStore { get; }
        public ISceneFactory SceneFactory { get; }
        public ISceneLoader SceneLoader { get; }
    }

    [TestFixture]
    public class SceneLoaderIntegrationTests : IntegrationTests<SceneLoaderIntegrationTestsSut>
    {
        private TemporaryDirectory _temporaryDirectory = null!;
        private string _sceneFilePath = null!;

        protected override void RegisterTestComponents(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<TestSceneBehaviorFactory>().As<ISceneBehaviorFactory>().SingleInstance();
            containerBuilder.RegisterType<TestBehaviorComponent.Factory>().As<IComponentFactory>().SingleInstance();
        }

        public override void SetUp()
        {
            base.SetUp();
            _temporaryDirectory = new TemporaryDirectory();
            _sceneFilePath = _temporaryDirectory.GetRandomFilePath();

            SystemUnderTest.AssetStore.RegisterAssets(Utils.GetPathUnderTestDirectory(@"Assets"));
        }

        public override void TearDown()
        {
            base.TearDown();
            _temporaryDirectory.Dispose();
        }

        #region No components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithSceneBehavior()
        {
            // Arrange
            var sceneBehaviorFactory = new TestSceneBehaviorFactory();

            var scene = SystemUnderTest.SceneFactory.Create();
            scene.SceneBehavior = sceneBehaviorFactory.Create(scene);

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
            var scene = SystemUnderTest.SceneFactory.Create();

            var emptyEntity = CreateNewEntityWithRandomName(scene);

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
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithChildren = CreateNewEntityWithRandomName(scene);

            var child1 = CreateNewEntityWithRandomName(scene);
            child1.Parent = entityWithChildren;
            var child2 = CreateNewEntityWithRandomName(scene);
            child2.Parent = entityWithChildren;
            var child3 = CreateNewEntityWithRandomName(scene);
            child3.Parent = entityWithChildren;

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithChildren);
        }

        #endregion

        #region Animation components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithSpriteAnimationComponent()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithSpriteAnimation = CreateNewEntityWithRandomName(scene);
            var spriteAnimationComponent = entityWithSpriteAnimation.CreateComponent<SpriteAnimationComponent>();
            spriteAnimationComponent.AddAnimation("animation", SystemUnderTest.AssetStore.GetAsset<SpriteAnimation>(AssetsIds.TestSpriteAnimation));
            spriteAnimationComponent.PlayAnimation("animation");
            spriteAnimationComponent.Position = 0.7;
            spriteAnimationComponent.PlaybackSpeed = 1.3;
            spriteAnimationComponent.PlayInLoop = true;

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
            Assert.That(loadedSpriteAnimationComponent.CurrentAnimation, Is.Not.Null);
            Debug.Assert(loadedSpriteAnimationComponent.CurrentAnimation != null, "loadedSpriteAnimationComponent.CurrentAnimation != null");
            Assert.That(loadedSpriteAnimationComponent.CurrentAnimation.Value.Name, Is.EqualTo("animation"));
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedSpriteAnimationComponent.CurrentAnimation.Value.Animation),
                Is.EqualTo(AssetsIds.TestSpriteAnimation));
            Assert.That(loadedSpriteAnimationComponent.IsPlaying, Is.True);
            Assert.That(loadedSpriteAnimationComponent.Position, Is.EqualTo(spriteAnimationComponent.Position));
            Assert.That(loadedSpriteAnimationComponent.PlaybackSpeed, Is.EqualTo(spriteAnimationComponent.PlaybackSpeed));
            Assert.That(loadedSpriteAnimationComponent.PlayInLoop, Is.EqualTo(spriteAnimationComponent.PlayInLoop));
        }

        #endregion

        #region Audio components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithAudioSource()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithAudioSource = CreateNewEntityWithRandomName(scene);
            var audioSourceComponent = entityWithAudioSource.CreateComponent<AudioSourceComponent>();
            audioSourceComponent.Sound = SystemUnderTest.AssetStore.GetAsset<ISound>(AssetsIds.TestSound);

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

        #region Core components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithTransform2D()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithTransform = CreateNewEntityWithRandomName(scene);
            var transform2DComponent = entityWithTransform.CreateComponent<Transform2DComponent>();
            transform2DComponent.Translation = Utils.RandomVector2();
            transform2DComponent.Rotation = Utils.Random.NextDouble();
            transform2DComponent.Scale = Utils.RandomVector2();

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
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithTransform = CreateNewEntityWithRandomName(scene);
            var transform3DComponent = entityWithTransform.CreateComponent<Transform3DComponent>();
            transform3DComponent.Translation = Utils.RandomVector3();
            transform3DComponent.Rotation = Utils.RandomVector3();
            transform3DComponent.Scale = Utils.RandomVector3();

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
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithBehavior = CreateNewEntityWithRandomName(scene);
            var testBehaviorComponent = entityWithBehavior.CreateComponent<TestBehaviorComponent>();
            testBehaviorComponent.IntProperty = Utils.Random.Next();
            testBehaviorComponent.DoubleProperty = Utils.Random.NextDouble();
            testBehaviorComponent.StringProperty = Utils.Random.GetString();

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
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithInputComponent = CreateNewEntityWithRandomName(scene);
            var inputComponent = entityWithInputComponent.CreateComponent<InputComponent>();
            inputComponent.InputMapping = SystemUnderTest.AssetStore.GetAsset<InputMapping>(AssetsIds.TestInputMapping);

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
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithCircleCollider = CreateNewEntityWithRandomName(scene);
            var circleColliderComponent = entityWithCircleCollider.CreateComponent<CircleColliderComponent>();
            circleColliderComponent.Radius = Utils.Random.NextDouble();

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
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithRectangleCollider = CreateNewEntityWithRandomName(scene);
            var rectangleColliderComponent = entityWithRectangleCollider.CreateComponent<RectangleColliderComponent>();
            rectangleColliderComponent.Dimensions = Utils.RandomVector2();

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithRectangleCollider);
            var rectangleCollider = entityWithRectangleCollider.GetComponent<RectangleColliderComponent>();
            var loadedRectangleCollider = loadedScene.RootEntities.Single().GetComponent<RectangleColliderComponent>();
            Assert.That(loadedRectangleCollider.Dimensions, Is.EqualTo(rectangleCollider.Dimensions));
        }

        #endregion

        #region Rendering components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithCamera()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithCamera = CreateNewEntityWithRandomName(scene);
            var cameraComponent = entityWithCamera.CreateComponent<CameraComponent>();
            cameraComponent.AspectRatioBehavior = Utils.Random.NextEnum<AspectRatioBehavior>();
            cameraComponent.ViewRectangle = Utils.RandomVector2();

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithCamera);
            var loadedCameraComponent = loadedScene.RootEntities.Single().GetComponent<CameraComponent>();
            Assert.That(loadedCameraComponent.AspectRatioBehavior, Is.EqualTo(cameraComponent.AspectRatioBehavior));
            Assert.That(loadedCameraComponent.ViewRectangle.X, Is.EqualTo(cameraComponent.ViewRectangle.X));
            Assert.That(loadedCameraComponent.ViewRectangle.Y, Is.EqualTo(cameraComponent.ViewRectangle.Y));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithTextRenderer()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithTextRenderer = CreateNewEntityWithRandomName(scene);
            var textRendererComponent = entityWithTextRenderer.CreateComponent<TextRendererComponent>();
            textRendererComponent.Text = Utils.Random.GetString();
            textRendererComponent.FontFamilyName = Utils.Random.NextBool() ? "Arial" : "Calibri";
            textRendererComponent.FontSize = FontSize.FromDips(Utils.Random.Next());
            textRendererComponent.Color = Color.FromArgb(Utils.Random.Next());
            textRendererComponent.MaxWidth = Utils.Random.NextDouble(100, 200);
            textRendererComponent.MaxHeight = Utils.Random.NextDouble(100, 200);
            textRendererComponent.TextAlignment = Utils.Random.NextEnum<TextAlignment>();
            textRendererComponent.ParagraphAlignment = Utils.Random.NextEnum<ParagraphAlignment>();
            textRendererComponent.Pivot = Utils.RandomVector2();
            textRendererComponent.ClipToLayoutBox = Utils.Random.NextBool();
            textRendererComponent.Visible = Utils.Random.NextBool();
            textRendererComponent.SortingLayerName = Utils.Random.GetString();
            textRendererComponent.OrderInLayer = Utils.Random.Next();

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithTextRenderer);
            var textRenderer = entityWithTextRenderer.GetComponent<TextRendererComponent>();
            var loadedTextRenderer = loadedScene.RootEntities.Single().GetComponent<TextRendererComponent>();
            Assert.That(loadedTextRenderer.Text, Is.EqualTo(textRenderer.Text));
            Assert.That(loadedTextRenderer.FontFamilyName, Is.EqualTo(textRenderer.FontFamilyName));
            Assert.That(loadedTextRenderer.FontSize, Is.EqualTo(textRenderer.FontSize));
            Assert.That(loadedTextRenderer.Color, Is.EqualTo(textRenderer.Color));
            Assert.That(loadedTextRenderer.MaxWidth, Is.EqualTo(textRenderer.MaxWidth));
            Assert.That(loadedTextRenderer.MaxHeight, Is.EqualTo(textRenderer.MaxHeight));
            Assert.That(loadedTextRenderer.TextAlignment, Is.EqualTo(textRenderer.TextAlignment));
            Assert.That(loadedTextRenderer.ParagraphAlignment, Is.EqualTo(textRenderer.ParagraphAlignment));
            Assert.That(loadedTextRenderer.Pivot, Is.EqualTo(textRenderer.Pivot));
            Assert.That(loadedTextRenderer.ClipToLayoutBox, Is.EqualTo(textRenderer.ClipToLayoutBox));
            Assert.That(loadedTextRenderer.Visible, Is.EqualTo(textRenderer.Visible));
            Assert.That(loadedTextRenderer.SortingLayerName, Is.EqualTo(textRenderer.SortingLayerName));
            Assert.That(loadedTextRenderer.OrderInLayer, Is.EqualTo(textRenderer.OrderInLayer));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithSpriteRenderer()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithSpriteRenderer = CreateNewEntityWithRandomName(scene);
            var spriteRendererComponent = entityWithSpriteRenderer.CreateComponent<SpriteRendererComponent>();
            spriteRendererComponent.Sprite = SystemUnderTest.AssetStore.GetAsset<Sprite>(AssetsIds.TestSprite);
            spriteRendererComponent.Visible = Utils.Random.NextBool();
            spriteRendererComponent.SortingLayerName = Utils.Random.GetString();
            spriteRendererComponent.OrderInLayer = Utils.Random.Next();

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
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithRectangleRenderer = CreateNewEntityWithRandomName(scene);
            var rectangleRendererComponent = entityWithRectangleRenderer.CreateComponent<RectangleRendererComponent>();
            rectangleRendererComponent.Dimension = Utils.RandomVector2();
            rectangleRendererComponent.FillInterior = Utils.Random.NextBool();
            rectangleRendererComponent.Color = Color.FromArgb(Utils.Random.Next());
            rectangleRendererComponent.Visible = Utils.Random.NextBool();
            rectangleRendererComponent.SortingLayerName = Utils.Random.GetString();
            rectangleRendererComponent.OrderInLayer = Utils.Random.Next();

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
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithEllipseRenderer = CreateNewEntityWithRandomName(scene);
            var ellipseRendererComponent = entityWithEllipseRenderer.CreateComponent<EllipseRendererComponent>();
            ellipseRendererComponent.RadiusX = Utils.Random.NextDouble();
            ellipseRendererComponent.RadiusY = Utils.Random.NextDouble();
            ellipseRendererComponent.FillInterior = Utils.Random.NextBool();
            ellipseRendererComponent.Color = Color.FromArgb(Utils.Random.Next());
            ellipseRendererComponent.Visible = Utils.Random.NextBool();
            ellipseRendererComponent.SortingLayerName = Utils.Random.GetString();
            ellipseRendererComponent.OrderInLayer = Utils.Random.Next();

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

        private static Entity CreateNewEntityWithRandomName(Scene scene)
        {
            var entity = scene.CreateEntity();
            entity.Name = Utils.Random.GetString();
            return entity;
        }

        private static void AssertScenesAreEqual(Scene scene1, Scene scene2)
        {
            Assert.That(scene1.RootEntities.Count, Is.EqualTo(scene2.RootEntities.Count));
            Assert.That(scene1.AllEntities.Count(), Is.EqualTo(scene2.AllEntities.Count()));
            Assert.That(scene1.SceneBehavior.Name, Is.EqualTo(scene2.SceneBehavior.Name));
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

        private class TestBehaviorComponent : BehaviorComponent
        {
            private TestBehaviorComponent(Entity entity) : base(entity)
            {
            }

            public int IntProperty { get; set; }
            public double DoubleProperty { get; set; }
            public string StringProperty { get; set; } = string.Empty;

            protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
            {
                base.Serialize(writer, assetStore);

                writer.WriteInt("IntProperty", IntProperty);
                writer.WriteDouble("DoubleProperty", DoubleProperty);
                writer.WriteString("StringProperty", StringProperty);
            }

            protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
            {
                base.Deserialize(reader, assetStore);

                IntProperty = reader.ReadInt("IntProperty");
                DoubleProperty = reader.ReadDouble("DoubleProperty");
                StringProperty = reader.ReadString("StringProperty") ?? throw new InvalidOperationException();
            }

            public sealed class Factory : ComponentFactory<TestBehaviorComponent>
            {
                protected override TestBehaviorComponent CreateComponent(Entity entity) => new TestBehaviorComponent(entity);
            }
        }

        private sealed class TestSceneBehaviorFactory : ISceneBehaviorFactory
        {
            public const string SceneBehaviorName = "TestSceneBehavior";
            public string BehaviorName { get; } = SceneBehaviorName;
            public SceneBehavior Create(Scene scene) => new TestSceneBehavior(scene);

            private sealed class TestSceneBehavior : SceneBehavior
            {
                public TestSceneBehavior(Scene scene) : base(scene)
                {
                }

                public override string Name { get; } = SceneBehaviorName;

                protected internal override void OnLoaded()
                {
                }
            }
        }

        #endregion
    }
}