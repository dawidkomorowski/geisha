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
            var loadedComponent = loadedScene.RootEntities.Single().GetComponent<SpriteAnimationComponent>();
            Assert.That(loadedComponent.Animations, Has.Count.EqualTo(1));
            Assert.That(loadedComponent.Animations.Single().Key, Is.EqualTo("animation"));
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedComponent.Animations.Single().Value),
                Is.EqualTo(AssetsIds.TestSpriteAnimation));
            Assert.That(loadedComponent.CurrentAnimation, Is.Not.Null);
            Debug.Assert(loadedComponent.CurrentAnimation != null, "loadedComponent.CurrentAnimation != null");
            Assert.That(loadedComponent.CurrentAnimation.Value.Name, Is.EqualTo("animation"));
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedComponent.CurrentAnimation.Value.Animation),
                Is.EqualTo(AssetsIds.TestSpriteAnimation));
            Assert.That(loadedComponent.IsPlaying, Is.True);
            Assert.That(loadedComponent.Position, Is.EqualTo(spriteAnimationComponent.Position));
            Assert.That(loadedComponent.PlaybackSpeed, Is.EqualTo(spriteAnimationComponent.PlaybackSpeed));
            Assert.That(loadedComponent.PlayInLoop, Is.EqualTo(spriteAnimationComponent.PlayInLoop));
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
            var loadedComponent = loadedScene.RootEntities.Single().GetComponent<AudioSourceComponent>();
            Assert.That(loadedComponent.IsPlaying, Is.EqualTo(audioSourceComponent.IsPlaying));
            Debug.Assert(loadedComponent.Sound != null, "loadedComponent.Sound != null");
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedComponent.Sound), Is.EqualTo(AssetsIds.TestSound));
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
            transform2DComponent.IsInterpolated = Utils.Random.NextBool();

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entityWithTransform);
            var loadedComponent = loadedScene.RootEntities.Single().GetComponent<Transform2DComponent>();
            Assert.That(loadedComponent.Translation, Is.EqualTo(transform2DComponent.Translation));
            Assert.That(loadedComponent.Rotation, Is.EqualTo(transform2DComponent.Rotation));
            Assert.That(loadedComponent.Scale, Is.EqualTo(transform2DComponent.Scale));
            Assert.That(loadedComponent.IsInterpolated, Is.EqualTo(transform2DComponent.IsInterpolated));
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
            var loadedComponent = loadedScene.RootEntities.Single().GetComponent<Transform3DComponent>();
            Assert.That(loadedComponent.Translation, Is.EqualTo(transform3DComponent.Translation));
            Assert.That(loadedComponent.Rotation, Is.EqualTo(transform3DComponent.Rotation));
            Assert.That(loadedComponent.Scale, Is.EqualTo(transform3DComponent.Scale));
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
            var loadedComponent = loadedScene.RootEntities.Single().GetComponent<TestBehaviorComponent>();
            Assert.That(loadedComponent.IntProperty, Is.EqualTo(testBehaviorComponent.IntProperty));
            Assert.That(loadedComponent.DoubleProperty, Is.EqualTo(testBehaviorComponent.DoubleProperty));
            Assert.That(loadedComponent.StringProperty, Is.EqualTo(testBehaviorComponent.StringProperty));
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
            var loadedComponent = loadedScene.RootEntities.Single().GetComponent<InputComponent>();
            Assert.That(loadedComponent.HardwareInput, Is.EqualTo(HardwareInput.Empty));
            Debug.Assert(loadedComponent.InputMapping != null, "loadedComponent.InputMapping != null");
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedComponent.InputMapping), Is.EqualTo(AssetsIds.TestInputMapping));
        }

        #endregion

        #region Physics components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithCircleCollider()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity = CreateNewEntityWithRandomName(scene);
            var circleColliderComponent = entity.CreateComponent<CircleColliderComponent>();
            circleColliderComponent.Radius = Utils.Random.NextDouble();

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entity);
            var loadedComponent = loadedScene.RootEntities.Single().GetComponent<CircleColliderComponent>();
            Assert.That(loadedComponent.Radius, Is.EqualTo(circleColliderComponent.Radius));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithRectangleCollider()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity = CreateNewEntityWithRandomName(scene);
            var rectangleColliderComponent = entity.CreateComponent<RectangleColliderComponent>();
            rectangleColliderComponent.Dimensions = Utils.RandomVector2();

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entity);
            var loadedComponent = loadedScene.RootEntities.Single().GetComponent<RectangleColliderComponent>();
            Assert.That(loadedComponent.Dimensions, Is.EqualTo(rectangleColliderComponent.Dimensions));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithTileCollider()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity = CreateNewEntityWithRandomName(scene);
            entity.CreateComponent<TileColliderComponent>();

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entity);
            Assert.That(loadedScene.RootEntities.Single().HasComponent<TileColliderComponent>(), Is.True);
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithKinematicRigidBody2D()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity = CreateNewEntityWithRandomName(scene);
            var kinematicRigidBody2DComponent = entity.CreateComponent<KinematicRigidBody2DComponent>();
            kinematicRigidBody2DComponent.LinearVelocity = Utils.RandomVector2();
            kinematicRigidBody2DComponent.AngularVelocity = Utils.Random.NextDouble();
            kinematicRigidBody2DComponent.EnableCollisionResponse = Utils.Random.NextBool();

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);
            AssertEntitiesAreEqual(loadedScene.RootEntities.Single(), entity);
            var loadedComponent = loadedScene.RootEntities.Single().GetComponent<KinematicRigidBody2DComponent>();
            Assert.That(loadedComponent.LinearVelocity, Is.EqualTo(kinematicRigidBody2DComponent.LinearVelocity));
            Assert.That(loadedComponent.AngularVelocity, Is.EqualTo(kinematicRigidBody2DComponent.AngularVelocity));
            Assert.That(loadedComponent.EnableCollisionResponse, Is.EqualTo(kinematicRigidBody2DComponent.EnableCollisionResponse));
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
            var loadedComponent = loadedScene.RootEntities.Single().GetComponent<CameraComponent>();
            Assert.That(loadedComponent.AspectRatioBehavior, Is.EqualTo(cameraComponent.AspectRatioBehavior));
            Assert.That(loadedComponent.ViewRectangle.X, Is.EqualTo(cameraComponent.ViewRectangle.X));
            Assert.That(loadedComponent.ViewRectangle.Y, Is.EqualTo(cameraComponent.ViewRectangle.Y));
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
            var loadedComponent = loadedScene.RootEntities.Single().GetComponent<TextRendererComponent>();
            Assert.That(loadedComponent.Text, Is.EqualTo(textRendererComponent.Text));
            Assert.That(loadedComponent.FontFamilyName, Is.EqualTo(textRendererComponent.FontFamilyName));
            Assert.That(loadedComponent.FontSize, Is.EqualTo(textRendererComponent.FontSize));
            Assert.That(loadedComponent.Color, Is.EqualTo(textRendererComponent.Color));
            Assert.That(loadedComponent.MaxWidth, Is.EqualTo(textRendererComponent.MaxWidth));
            Assert.That(loadedComponent.MaxHeight, Is.EqualTo(textRendererComponent.MaxHeight));
            Assert.That(loadedComponent.TextAlignment, Is.EqualTo(textRendererComponent.TextAlignment));
            Assert.That(loadedComponent.ParagraphAlignment, Is.EqualTo(textRendererComponent.ParagraphAlignment));
            Assert.That(loadedComponent.Pivot, Is.EqualTo(textRendererComponent.Pivot));
            Assert.That(loadedComponent.ClipToLayoutBox, Is.EqualTo(textRendererComponent.ClipToLayoutBox));
            Assert.That(loadedComponent.Visible, Is.EqualTo(textRendererComponent.Visible));
            Assert.That(loadedComponent.SortingLayerName, Is.EqualTo(textRendererComponent.SortingLayerName));
            Assert.That(loadedComponent.OrderInLayer, Is.EqualTo(textRendererComponent.OrderInLayer));
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
            var loadedComponent = loadedScene.RootEntities.Single().GetComponent<SpriteRendererComponent>();
            Debug.Assert(loadedComponent.Sprite != null, "loadedComponent.Sprite != null");
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedComponent.Sprite), Is.EqualTo(AssetsIds.TestSprite));
            Assert.That(loadedComponent.Visible, Is.EqualTo(spriteRendererComponent.Visible));
            Assert.That(loadedComponent.SortingLayerName, Is.EqualTo(spriteRendererComponent.SortingLayerName));
            Assert.That(loadedComponent.OrderInLayer, Is.EqualTo(spriteRendererComponent.OrderInLayer));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithRectangleRenderer()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithRectangleRenderer = CreateNewEntityWithRandomName(scene);
            var rectangleRendererComponent = entityWithRectangleRenderer.CreateComponent<RectangleRendererComponent>();
            rectangleRendererComponent.Dimensions = Utils.RandomVector2();
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
            var loadedComponent = loadedScene.RootEntities.Single().GetComponent<RectangleRendererComponent>();
            Assert.That(loadedComponent.Dimensions, Is.EqualTo(rectangleRendererComponent.Dimensions));
            Assert.That(loadedComponent.FillInterior, Is.EqualTo(rectangleRendererComponent.FillInterior));
            Assert.That(loadedComponent.Color, Is.EqualTo(rectangleRendererComponent.Color));
            Assert.That(loadedComponent.Visible, Is.EqualTo(rectangleRendererComponent.Visible));
            Assert.That(loadedComponent.SortingLayerName, Is.EqualTo(rectangleRendererComponent.SortingLayerName));
            Assert.That(loadedComponent.OrderInLayer, Is.EqualTo(rectangleRendererComponent.OrderInLayer));
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
            var loadedComponent = loadedScene.RootEntities.Single().GetComponent<EllipseRendererComponent>();
            Assert.That(loadedComponent.RadiusX, Is.EqualTo(ellipseRendererComponent.RadiusX));
            Assert.That(loadedComponent.RadiusY, Is.EqualTo(ellipseRendererComponent.RadiusY));
            Assert.That(loadedComponent.FillInterior, Is.EqualTo(ellipseRendererComponent.FillInterior));
            Assert.That(loadedComponent.Color, Is.EqualTo(ellipseRendererComponent.Color));
            Assert.That(loadedComponent.Visible, Is.EqualTo(ellipseRendererComponent.Visible));
            Assert.That(loadedComponent.SortingLayerName, Is.EqualTo(ellipseRendererComponent.SortingLayerName));
            Assert.That(loadedComponent.OrderInLayer, Is.EqualTo(ellipseRendererComponent.OrderInLayer));
        }

        #endregion

        #region Helpers

        private static Entity CreateNewEntityWithRandomName(Scene scene)
        {
            var entity = scene.CreateEntity();
            entity.Name = Utils.Random.GetString();
            return entity;
        }

        private static void AssertScenesAreEqual(Scene actual, Scene expected)
        {
            Assert.That(actual.RootEntities.Count, Is.EqualTo(expected.RootEntities.Count));
            Assert.That(actual.AllEntities.Count, Is.EqualTo(expected.AllEntities.Count));
            Assert.That(actual.SceneBehavior.Name, Is.EqualTo(expected.SceneBehavior.Name));
        }

        private static void AssertEntitiesAreEqual(Entity actual, Entity expected)
        {
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.Components.Count, Is.EqualTo(expected.Components.Count));
            Assert.That(actual.IsRoot, Is.EqualTo(expected.IsRoot));
            Assert.That(actual.Children.Count, Is.EqualTo(expected.Children.Count));
            for (var i = 0; i < actual.Children.Count; i++)
            {
                AssertEntitiesAreEqual(actual.Children[i], expected.Children[i]);
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