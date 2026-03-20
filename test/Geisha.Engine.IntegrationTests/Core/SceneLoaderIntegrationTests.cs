using System;
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

            var emptyEntity = CreateEntity(scene, "EmptyEntity");

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

            var entityWithChildren = CreateEntity(scene, "EntityWithChildren");

            var child1 = CreateEntity(scene, "Child1");
            child1.Parent = entityWithChildren;
            var child2 = CreateEntity(scene, "Child2");
            child2.Parent = entityWithChildren;
            var child3 = CreateEntity(scene, "Child3");
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

            var entity1 = CreateEntity(scene, "Entity1");
            var spriteAnimationComponent1 = entity1.CreateComponent<SpriteAnimationComponent>();
            spriteAnimationComponent1.AddAnimation("animation1", SystemUnderTest.AssetStore.GetAsset<SpriteAnimation>(AssetsIds.TestSpriteAnimation));
            spriteAnimationComponent1.PlayAnimation("animation1");
            spriteAnimationComponent1.Position = 0.7;
            spriteAnimationComponent1.PlaybackSpeed = 1.3;
            spriteAnimationComponent1.PlayInLoop = true;

            var entity2 = CreateEntity(scene, "Entity2");
            var spriteAnimationComponent2 = entity2.CreateComponent<SpriteAnimationComponent>();
            spriteAnimationComponent2.AddAnimation("animation2", SystemUnderTest.AssetStore.GetAsset<SpriteAnimation>(AssetsIds.TestSpriteAnimation));
            spriteAnimationComponent2.PlaybackSpeed = 2.5;
            spriteAnimationComponent2.PlayInLoop = false;

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);

            var loadedEntity1 = loadedScene.RootEntities.Single(e => e.Name == "Entity1");
            AssertEntitiesAreEqual(loadedEntity1, entity1);
            var loadedComponent1 = loadedEntity1.GetComponent<SpriteAnimationComponent>();
            Assert.That(loadedComponent1.Animations, Has.Count.EqualTo(1));
            Assert.That(loadedComponent1.Animations.Single().Key, Is.EqualTo("animation1"));
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedComponent1.Animations.Single().Value),
                Is.EqualTo(AssetsIds.TestSpriteAnimation));
            Assert.That(loadedComponent1.CurrentAnimation, Is.Not.Null);
            Assert.That(loadedComponent1.CurrentAnimation.Value.Name, Is.EqualTo("animation1"));
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedComponent1.CurrentAnimation.Value.Animation),
                Is.EqualTo(AssetsIds.TestSpriteAnimation));
            Assert.That(loadedComponent1.IsPlaying, Is.True);
            Assert.That(loadedComponent1.Position, Is.EqualTo(spriteAnimationComponent1.Position));
            Assert.That(loadedComponent1.PlaybackSpeed, Is.EqualTo(spriteAnimationComponent1.PlaybackSpeed));
            Assert.That(loadedComponent1.PlayInLoop, Is.EqualTo(spriteAnimationComponent1.PlayInLoop));

            var loadedEntity2 = loadedScene.RootEntities.Single(e => e.Name == "Entity2");
            AssertEntitiesAreEqual(loadedEntity2, entity2);
            var loadedComponent2 = loadedEntity2.GetComponent<SpriteAnimationComponent>();
            Assert.That(loadedComponent2.Animations, Has.Count.EqualTo(1));
            Assert.That(loadedComponent2.Animations.Single().Key, Is.EqualTo("animation2"));
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedComponent2.Animations.Single().Value),
                Is.EqualTo(AssetsIds.TestSpriteAnimation));
            Assert.That(loadedComponent2.CurrentAnimation, Is.Null);
            Assert.That(loadedComponent2.IsPlaying, Is.False);
            Assert.That(loadedComponent2.Position, Is.EqualTo(spriteAnimationComponent2.Position));
            Assert.That(loadedComponent2.PlaybackSpeed, Is.EqualTo(spriteAnimationComponent2.PlaybackSpeed));
            Assert.That(loadedComponent2.PlayInLoop, Is.EqualTo(spriteAnimationComponent2.PlayInLoop));
        }

        #endregion

        #region Audio components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithAudioSource()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entityWithAudioSource = CreateEntity(scene, "EntityWithAudioSource");
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
            Assert.That(loadedComponent.Sound, Is.Not.Null);
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedComponent.Sound), Is.EqualTo(AssetsIds.TestSound));
        }

        #endregion

        #region Core components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithTransform2D()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity1 = CreateEntity(scene, "Entity1");
            var transform2DComponent1 = entity1.CreateComponent<Transform2DComponent>();
            transform2DComponent1.Translation = new Vector2(1.5, -2.5);
            transform2DComponent1.Rotation = 0.5;
            transform2DComponent1.Scale = new Vector2(3.0, 2.0);
            transform2DComponent1.IsInterpolated = true;

            var entity2 = CreateEntity(scene, "Entity2");
            var transform2DComponent2 = entity2.CreateComponent<Transform2DComponent>();
            transform2DComponent2.Translation = new Vector2(-3.5, 4.5);
            transform2DComponent2.Rotation = -0.5;
            transform2DComponent2.Scale = new Vector2(0.5, 1.5);
            transform2DComponent2.IsInterpolated = false;

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);

            var loadedEntity1 = loadedScene.RootEntities.Single(e => e.Name == "Entity1");
            AssertEntitiesAreEqual(loadedEntity1, entity1);
            var loadedComponent1 = loadedEntity1.GetComponent<Transform2DComponent>();
            Assert.That(loadedComponent1.Translation, Is.EqualTo(transform2DComponent1.Translation));
            Assert.That(loadedComponent1.Rotation, Is.EqualTo(transform2DComponent1.Rotation));
            Assert.That(loadedComponent1.Scale, Is.EqualTo(transform2DComponent1.Scale));
            Assert.That(loadedComponent1.IsInterpolated, Is.EqualTo(transform2DComponent1.IsInterpolated));

            var loadedEntity2 = loadedScene.RootEntities.Single(e => e.Name == "Entity2");
            AssertEntitiesAreEqual(loadedEntity2, entity2);
            var loadedComponent2 = loadedEntity2.GetComponent<Transform2DComponent>();
            Assert.That(loadedComponent2.Translation, Is.EqualTo(transform2DComponent2.Translation));
            Assert.That(loadedComponent2.Rotation, Is.EqualTo(transform2DComponent2.Rotation));
            Assert.That(loadedComponent2.Scale, Is.EqualTo(transform2DComponent2.Scale));
            Assert.That(loadedComponent2.IsInterpolated, Is.EqualTo(transform2DComponent2.IsInterpolated));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithTransform3D()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity1 = CreateEntity(scene, "Entity1");
            var transform3DComponent1 = entity1.CreateComponent<Transform3DComponent>();
            transform3DComponent1.Translation = new Vector3(1.5, -2.5, 3.5);
            transform3DComponent1.Rotation = new Vector3(0.5, -0.5, 1.0);
            transform3DComponent1.Scale = new Vector3(2.0, 3.0, 0.5);

            var entity2 = CreateEntity(scene, "Entity2");
            var transform3DComponent2 = entity2.CreateComponent<Transform3DComponent>();
            transform3DComponent2.Translation = new Vector3(-4.5, 6.5, -2.5);
            transform3DComponent2.Rotation = new Vector3(-1.0, 2.0, -0.5);
            transform3DComponent2.Scale = new Vector3(0.5, 1.5, 2.5);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);

            var loadedEntity1 = loadedScene.RootEntities.Single(e => e.Name == "Entity1");
            AssertEntitiesAreEqual(loadedEntity1, entity1);
            var loadedComponent1 = loadedEntity1.GetComponent<Transform3DComponent>();
            Assert.That(loadedComponent1.Translation, Is.EqualTo(transform3DComponent1.Translation));
            Assert.That(loadedComponent1.Rotation, Is.EqualTo(transform3DComponent1.Rotation));
            Assert.That(loadedComponent1.Scale, Is.EqualTo(transform3DComponent1.Scale));

            var loadedEntity2 = loadedScene.RootEntities.Single(e => e.Name == "Entity2");
            AssertEntitiesAreEqual(loadedEntity2, entity2);
            var loadedComponent2 = loadedEntity2.GetComponent<Transform3DComponent>();
            Assert.That(loadedComponent2.Translation, Is.EqualTo(transform3DComponent2.Translation));
            Assert.That(loadedComponent2.Rotation, Is.EqualTo(transform3DComponent2.Rotation));
            Assert.That(loadedComponent2.Scale, Is.EqualTo(transform3DComponent2.Scale));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithBehavior()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity1 = CreateEntity(scene, "Entity1");
            var testBehaviorComponent1 = entity1.CreateComponent<TestBehaviorComponent>();
            testBehaviorComponent1.IntProperty = 42;
            testBehaviorComponent1.DoubleProperty = 1.5;
            testBehaviorComponent1.StringProperty = "TestString1";

            var entity2 = CreateEntity(scene, "Entity2");
            var testBehaviorComponent2 = entity2.CreateComponent<TestBehaviorComponent>();
            testBehaviorComponent2.IntProperty = -7;
            testBehaviorComponent2.DoubleProperty = -3.14;
            testBehaviorComponent2.StringProperty = "TestString2";

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);

            var loadedEntity1 = loadedScene.RootEntities.Single(e => e.Name == "Entity1");
            AssertEntitiesAreEqual(loadedEntity1, entity1);
            var loadedComponent1 = loadedEntity1.GetComponent<TestBehaviorComponent>();
            Assert.That(loadedComponent1.IntProperty, Is.EqualTo(testBehaviorComponent1.IntProperty));
            Assert.That(loadedComponent1.DoubleProperty, Is.EqualTo(testBehaviorComponent1.DoubleProperty));
            Assert.That(loadedComponent1.StringProperty, Is.EqualTo(testBehaviorComponent1.StringProperty));

            var loadedEntity2 = loadedScene.RootEntities.Single(e => e.Name == "Entity2");
            AssertEntitiesAreEqual(loadedEntity2, entity2);
            var loadedComponent2 = loadedEntity2.GetComponent<TestBehaviorComponent>();
            Assert.That(loadedComponent2.IntProperty, Is.EqualTo(testBehaviorComponent2.IntProperty));
            Assert.That(loadedComponent2.DoubleProperty, Is.EqualTo(testBehaviorComponent2.DoubleProperty));
            Assert.That(loadedComponent2.StringProperty, Is.EqualTo(testBehaviorComponent2.StringProperty));
        }

        #endregion

        #region Input components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithInputComponent()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity1 = CreateEntity(scene, "Entity1");
            var inputComponent1 = entity1.CreateComponent<InputComponent>();
            inputComponent1.InputMapping = SystemUnderTest.AssetStore.GetAsset<InputMapping>(AssetsIds.TestInputMapping);
            inputComponent1.Enabled = true;

            var entity2 = CreateEntity(scene, "Entity2");
            var inputComponent2 = entity2.CreateComponent<InputComponent>();
            inputComponent2.InputMapping = SystemUnderTest.AssetStore.GetAsset<InputMapping>(AssetsIds.TestInputMapping);
            inputComponent2.Enabled = false;

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);

            var loadedEntity1 = loadedScene.RootEntities.Single(e => e.Name == "Entity1");
            AssertEntitiesAreEqual(loadedEntity1, entity1);
            var loadedComponent1 = loadedEntity1.GetComponent<InputComponent>();
            Assert.That(loadedComponent1.HardwareInput, Is.EqualTo(HardwareInput.Empty));
            Assert.That(loadedComponent1.InputMapping, Is.Not.Null);
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedComponent1.InputMapping), Is.EqualTo(AssetsIds.TestInputMapping));
            Assert.That(loadedComponent1.Enabled, Is.EqualTo(inputComponent1.Enabled));

            var loadedEntity2 = loadedScene.RootEntities.Single(e => e.Name == "Entity2");
            AssertEntitiesAreEqual(loadedEntity2, entity2);
            var loadedComponent2 = loadedEntity2.GetComponent<InputComponent>();
            Assert.That(loadedComponent2.HardwareInput, Is.EqualTo(HardwareInput.Empty));
            Assert.That(loadedComponent2.InputMapping, Is.Not.Null);
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedComponent2.InputMapping), Is.EqualTo(AssetsIds.TestInputMapping));
            Assert.That(loadedComponent2.Enabled, Is.EqualTo(inputComponent2.Enabled));
        }

        #endregion

        #region Physics components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithCircleCollider()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity1 = CreateEntity(scene, "Entity1");
            var circleColliderComponent1 = entity1.CreateComponent<CircleColliderComponent>();
            circleColliderComponent1.Radius = 10.5;
            circleColliderComponent1.Enabled = true;

            var entity2 = CreateEntity(scene, "Entity2");
            var circleColliderComponent2 = entity2.CreateComponent<CircleColliderComponent>();
            circleColliderComponent2.Radius = 20.75;
            circleColliderComponent2.Enabled = false;

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);

            var loadedEntity1 = loadedScene.RootEntities.Single(e => e.Name == "Entity1");
            AssertEntitiesAreEqual(loadedEntity1, entity1);
            var loadedComponent1 = loadedEntity1.GetComponent<CircleColliderComponent>();
            Assert.That(loadedComponent1.Radius, Is.EqualTo(circleColliderComponent1.Radius));
            Assert.That(loadedComponent1.Enabled, Is.EqualTo(circleColliderComponent1.Enabled));

            var loadedEntity2 = loadedScene.RootEntities.Single(e => e.Name == "Entity2");
            AssertEntitiesAreEqual(loadedEntity2, entity2);
            var loadedComponent2 = loadedEntity2.GetComponent<CircleColliderComponent>();
            Assert.That(loadedComponent2.Radius, Is.EqualTo(circleColliderComponent2.Radius));
            Assert.That(loadedComponent2.Enabled, Is.EqualTo(circleColliderComponent2.Enabled));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithRectangleCollider()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity1 = CreateEntity(scene, "Entity1");
            var rectangleColliderComponent1 = entity1.CreateComponent<RectangleColliderComponent>();
            rectangleColliderComponent1.Dimensions = new Vector2(100.5, 50.25);
            rectangleColliderComponent1.Enabled = true;

            var entity2 = CreateEntity(scene, "Entity2");
            var rectangleColliderComponent2 = entity2.CreateComponent<RectangleColliderComponent>();
            rectangleColliderComponent2.Dimensions = new Vector2(25.5, 75.75);
            rectangleColliderComponent2.Enabled = false;

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);

            var loadedEntity1 = loadedScene.RootEntities.Single(e => e.Name == "Entity1");
            AssertEntitiesAreEqual(loadedEntity1, entity1);
            var loadedComponent1 = loadedEntity1.GetComponent<RectangleColliderComponent>();
            Assert.That(loadedComponent1.Dimensions, Is.EqualTo(rectangleColliderComponent1.Dimensions));
            Assert.That(loadedComponent1.Enabled, Is.EqualTo(rectangleColliderComponent1.Enabled));

            var loadedEntity2 = loadedScene.RootEntities.Single(e => e.Name == "Entity2");
            AssertEntitiesAreEqual(loadedEntity2, entity2);
            var loadedComponent2 = loadedEntity2.GetComponent<RectangleColliderComponent>();
            Assert.That(loadedComponent2.Dimensions, Is.EqualTo(rectangleColliderComponent2.Dimensions));
            Assert.That(loadedComponent2.Enabled, Is.EqualTo(rectangleColliderComponent2.Enabled));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithTileCollider()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity1 = CreateEntity(scene, "Entity1");
            var tileColliderComponent1 = entity1.CreateComponent<TileColliderComponent>();
            tileColliderComponent1.Enabled = true;

            var entity2 = CreateEntity(scene, "Entity2");
            var tileColliderComponent2 = entity2.CreateComponent<TileColliderComponent>();
            tileColliderComponent2.Enabled = false;

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);

            var loadedEntity1 = loadedScene.RootEntities.Single(e => e.Name == "Entity1");
            AssertEntitiesAreEqual(loadedEntity1, entity1);
            var loadedComponent1 = loadedEntity1.GetComponent<TileColliderComponent>();
            Assert.That(loadedComponent1.Enabled, Is.EqualTo(tileColliderComponent1.Enabled));

            var loadedEntity2 = loadedScene.RootEntities.Single(e => e.Name == "Entity2");
            AssertEntitiesAreEqual(loadedEntity2, entity2);
            var loadedComponent2 = loadedEntity2.GetComponent<TileColliderComponent>();
            Assert.That(loadedComponent2.Enabled, Is.EqualTo(tileColliderComponent2.Enabled));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithKinematicRigidBody2D()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity1 = CreateEntity(scene, "Entity1");
            var kinematicRigidBody2DComponent1 = entity1.CreateComponent<KinematicRigidBody2DComponent>();
            kinematicRigidBody2DComponent1.LinearVelocity = new Vector2(1.5, -2.5);
            kinematicRigidBody2DComponent1.AngularVelocity = 0.5;
            kinematicRigidBody2DComponent1.EnableCollisionResponse = true;

            var entity2 = CreateEntity(scene, "Entity2");
            var kinematicRigidBody2DComponent2 = entity2.CreateComponent<KinematicRigidBody2DComponent>();
            kinematicRigidBody2DComponent2.LinearVelocity = new Vector2(-3.5, 4.5);
            kinematicRigidBody2DComponent2.AngularVelocity = -0.5;
            kinematicRigidBody2DComponent2.EnableCollisionResponse = false;

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);

            var loadedEntity1 = loadedScene.RootEntities.Single(e => e.Name == "Entity1");
            AssertEntitiesAreEqual(loadedEntity1, entity1);
            var loadedComponent1 = loadedEntity1.GetComponent<KinematicRigidBody2DComponent>();
            Assert.That(loadedComponent1.LinearVelocity, Is.EqualTo(kinematicRigidBody2DComponent1.LinearVelocity));
            Assert.That(loadedComponent1.AngularVelocity, Is.EqualTo(kinematicRigidBody2DComponent1.AngularVelocity));
            Assert.That(loadedComponent1.EnableCollisionResponse, Is.EqualTo(kinematicRigidBody2DComponent1.EnableCollisionResponse));

            var loadedEntity2 = loadedScene.RootEntities.Single(e => e.Name == "Entity2");
            AssertEntitiesAreEqual(loadedEntity2, entity2);
            var loadedComponent2 = loadedEntity2.GetComponent<KinematicRigidBody2DComponent>();
            Assert.That(loadedComponent2.LinearVelocity, Is.EqualTo(kinematicRigidBody2DComponent2.LinearVelocity));
            Assert.That(loadedComponent2.AngularVelocity, Is.EqualTo(kinematicRigidBody2DComponent2.AngularVelocity));
            Assert.That(loadedComponent2.EnableCollisionResponse, Is.EqualTo(kinematicRigidBody2DComponent2.EnableCollisionResponse));
        }

        #endregion

        #region Rendering components

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithCamera()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity1 = CreateEntity(scene, "Entity1");
            var cameraComponent1 = entity1.CreateComponent<CameraComponent>();
            cameraComponent1.AspectRatioBehavior = AspectRatioBehavior.Overscan;
            cameraComponent1.ViewRectangle = new Vector2(1920, 1080);

            var entity2 = CreateEntity(scene, "Entity2");
            var cameraComponent2 = entity2.CreateComponent<CameraComponent>();
            cameraComponent2.AspectRatioBehavior = AspectRatioBehavior.Underscan;
            cameraComponent2.ViewRectangle = new Vector2(1280, 720);

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);

            var loadedEntity1 = loadedScene.RootEntities.Single(e => e.Name == "Entity1");
            AssertEntitiesAreEqual(loadedEntity1, entity1);
            var loadedComponent1 = loadedEntity1.GetComponent<CameraComponent>();
            Assert.That(loadedComponent1.AspectRatioBehavior, Is.EqualTo(cameraComponent1.AspectRatioBehavior));
            Assert.That(loadedComponent1.ViewRectangle, Is.EqualTo(cameraComponent1.ViewRectangle));

            var loadedEntity2 = loadedScene.RootEntities.Single(e => e.Name == "Entity2");
            AssertEntitiesAreEqual(loadedEntity2, entity2);
            var loadedComponent2 = loadedEntity2.GetComponent<CameraComponent>();
            Assert.That(loadedComponent2.AspectRatioBehavior, Is.EqualTo(cameraComponent2.AspectRatioBehavior));
            Assert.That(loadedComponent2.ViewRectangle, Is.EqualTo(cameraComponent2.ViewRectangle));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithTextRenderer()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity1 = CreateEntity(scene, "Entity1");
            var textRendererComponent1 = entity1.CreateComponent<TextRendererComponent>();
            textRendererComponent1.Text = "Sample text 1";
            textRendererComponent1.FontFamilyName = "Arial";
            textRendererComponent1.FontSize = FontSize.FromDips(12);
            textRendererComponent1.Color = Color.Blue;
            textRendererComponent1.MaxWidth = 150.5;
            textRendererComponent1.MaxHeight = 100.5;
            textRendererComponent1.TextAlignment = TextAlignment.Leading;
            textRendererComponent1.ParagraphAlignment = ParagraphAlignment.Near;
            textRendererComponent1.Pivot = new Vector2(1.5, -2.5);
            textRendererComponent1.ClipToLayoutBox = true;
            textRendererComponent1.Visible = true;
            textRendererComponent1.SortingLayerName = "Layer1";
            textRendererComponent1.OrderInLayer = 1;

            var entity2 = CreateEntity(scene, "Entity2");
            var textRendererComponent2 = entity2.CreateComponent<TextRendererComponent>();
            textRendererComponent2.Text = "Sample text 2";
            textRendererComponent2.FontFamilyName = "Calibri";
            textRendererComponent2.FontSize = FontSize.FromDips(16);
            textRendererComponent2.Color = Color.Red;
            textRendererComponent2.MaxWidth = 200.75;
            textRendererComponent2.MaxHeight = 175.25;
            textRendererComponent2.TextAlignment = TextAlignment.Trailing;
            textRendererComponent2.ParagraphAlignment = ParagraphAlignment.Far;
            textRendererComponent2.Pivot = new Vector2(-3.5, 4.5);
            textRendererComponent2.ClipToLayoutBox = false;
            textRendererComponent2.Visible = false;
            textRendererComponent2.SortingLayerName = "Layer2";
            textRendererComponent2.OrderInLayer = -5;

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);

            var loadedEntity1 = loadedScene.RootEntities.Single(e => e.Name == "Entity1");
            AssertEntitiesAreEqual(loadedEntity1, entity1);
            var loadedComponent1 = loadedEntity1.GetComponent<TextRendererComponent>();
            Assert.That(loadedComponent1.Text, Is.EqualTo(textRendererComponent1.Text));
            Assert.That(loadedComponent1.FontFamilyName, Is.EqualTo(textRendererComponent1.FontFamilyName));
            Assert.That(loadedComponent1.FontSize, Is.EqualTo(textRendererComponent1.FontSize));
            Assert.That(loadedComponent1.Color, Is.EqualTo(textRendererComponent1.Color));
            Assert.That(loadedComponent1.MaxWidth, Is.EqualTo(textRendererComponent1.MaxWidth));
            Assert.That(loadedComponent1.MaxHeight, Is.EqualTo(textRendererComponent1.MaxHeight));
            Assert.That(loadedComponent1.TextAlignment, Is.EqualTo(textRendererComponent1.TextAlignment));
            Assert.That(loadedComponent1.ParagraphAlignment, Is.EqualTo(textRendererComponent1.ParagraphAlignment));
            Assert.That(loadedComponent1.Pivot, Is.EqualTo(textRendererComponent1.Pivot));
            Assert.That(loadedComponent1.ClipToLayoutBox, Is.EqualTo(textRendererComponent1.ClipToLayoutBox));
            Assert.That(loadedComponent1.Visible, Is.EqualTo(textRendererComponent1.Visible));
            Assert.That(loadedComponent1.SortingLayerName, Is.EqualTo(textRendererComponent1.SortingLayerName));
            Assert.That(loadedComponent1.OrderInLayer, Is.EqualTo(textRendererComponent1.OrderInLayer));

            var loadedEntity2 = loadedScene.RootEntities.Single(e => e.Name == "Entity2");
            AssertEntitiesAreEqual(loadedEntity2, entity2);
            var loadedComponent2 = loadedEntity2.GetComponent<TextRendererComponent>();
            Assert.That(loadedComponent2.Text, Is.EqualTo(textRendererComponent2.Text));
            Assert.That(loadedComponent2.FontFamilyName, Is.EqualTo(textRendererComponent2.FontFamilyName));
            Assert.That(loadedComponent2.FontSize, Is.EqualTo(textRendererComponent2.FontSize));
            Assert.That(loadedComponent2.Color, Is.EqualTo(textRendererComponent2.Color));
            Assert.That(loadedComponent2.MaxWidth, Is.EqualTo(textRendererComponent2.MaxWidth));
            Assert.That(loadedComponent2.MaxHeight, Is.EqualTo(textRendererComponent2.MaxHeight));
            Assert.That(loadedComponent2.TextAlignment, Is.EqualTo(textRendererComponent2.TextAlignment));
            Assert.That(loadedComponent2.ParagraphAlignment, Is.EqualTo(textRendererComponent2.ParagraphAlignment));
            Assert.That(loadedComponent2.Pivot, Is.EqualTo(textRendererComponent2.Pivot));
            Assert.That(loadedComponent2.ClipToLayoutBox, Is.EqualTo(textRendererComponent2.ClipToLayoutBox));
            Assert.That(loadedComponent2.Visible, Is.EqualTo(textRendererComponent2.Visible));
            Assert.That(loadedComponent2.SortingLayerName, Is.EqualTo(textRendererComponent2.SortingLayerName));
            Assert.That(loadedComponent2.OrderInLayer, Is.EqualTo(textRendererComponent2.OrderInLayer));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithSpriteRenderer()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity1 = CreateEntity(scene, "Entity1");
            var spriteRendererComponent1 = entity1.CreateComponent<SpriteRendererComponent>();
            spriteRendererComponent1.Sprite = SystemUnderTest.AssetStore.GetAsset<Sprite>(AssetsIds.TestSprite);
            spriteRendererComponent1.Opacity = 0.5;
            spriteRendererComponent1.BitmapInterpolationMode = BitmapInterpolationMode.NearestNeighbor;
            spriteRendererComponent1.Visible = true;
            spriteRendererComponent1.SortingLayerName = "Layer1";
            spriteRendererComponent1.OrderInLayer = 1;

            var entity2 = CreateEntity(scene, "Entity2");
            var spriteRendererComponent2 = entity2.CreateComponent<SpriteRendererComponent>();
            spriteRendererComponent2.Sprite = SystemUnderTest.AssetStore.GetAsset<Sprite>(AssetsIds.TestSprite);
            spriteRendererComponent2.Opacity = 0.75;
            spriteRendererComponent2.BitmapInterpolationMode = BitmapInterpolationMode.Linear;
            spriteRendererComponent2.Visible = false;
            spriteRendererComponent2.SortingLayerName = "Layer2";
            spriteRendererComponent2.OrderInLayer = -5;

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);

            var loadedEntity1 = loadedScene.RootEntities.Single(e => e.Name == "Entity1");
            AssertEntitiesAreEqual(loadedEntity1, entity1);
            var loadedComponent1 = loadedEntity1.GetComponent<SpriteRendererComponent>();
            Assert.That(loadedComponent1.Sprite, Is.Not.Null);
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedComponent1.Sprite), Is.EqualTo(AssetsIds.TestSprite));
            Assert.That(loadedComponent1.Opacity, Is.EqualTo(spriteRendererComponent1.Opacity));
            Assert.That(loadedComponent1.BitmapInterpolationMode, Is.EqualTo(spriteRendererComponent1.BitmapInterpolationMode));
            Assert.That(loadedComponent1.Visible, Is.EqualTo(spriteRendererComponent1.Visible));
            Assert.That(loadedComponent1.SortingLayerName, Is.EqualTo(spriteRendererComponent1.SortingLayerName));
            Assert.That(loadedComponent1.OrderInLayer, Is.EqualTo(spriteRendererComponent1.OrderInLayer));

            var loadedEntity2 = loadedScene.RootEntities.Single(e => e.Name == "Entity2");
            AssertEntitiesAreEqual(loadedEntity2, entity2);
            var loadedComponent2 = loadedEntity2.GetComponent<SpriteRendererComponent>();
            Assert.That(loadedComponent2.Sprite, Is.Not.Null);
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(loadedComponent2.Sprite), Is.EqualTo(AssetsIds.TestSprite));
            Assert.That(loadedComponent2.Opacity, Is.EqualTo(spriteRendererComponent2.Opacity));
            Assert.That(loadedComponent2.BitmapInterpolationMode, Is.EqualTo(spriteRendererComponent2.BitmapInterpolationMode));
            Assert.That(loadedComponent2.Visible, Is.EqualTo(spriteRendererComponent2.Visible));
            Assert.That(loadedComponent2.SortingLayerName, Is.EqualTo(spriteRendererComponent2.SortingLayerName));
            Assert.That(loadedComponent2.OrderInLayer, Is.EqualTo(spriteRendererComponent2.OrderInLayer));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithRectangleRenderer()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity1 = CreateEntity(scene, "Entity1");
            var rectangleRendererComponent1 = entity1.CreateComponent<RectangleRendererComponent>();
            rectangleRendererComponent1.Dimensions = new Vector2(100.5, 50.25);
            rectangleRendererComponent1.FillInterior = true;
            rectangleRendererComponent1.Color = Color.Blue;
            rectangleRendererComponent1.Visible = true;
            rectangleRendererComponent1.SortingLayerName = "Layer1";
            rectangleRendererComponent1.OrderInLayer = 1;

            var entity2 = CreateEntity(scene, "Entity2");
            var rectangleRendererComponent2 = entity2.CreateComponent<RectangleRendererComponent>();
            rectangleRendererComponent2.Dimensions = new Vector2(25.5, 75.75);
            rectangleRendererComponent2.FillInterior = false;
            rectangleRendererComponent2.Color = Color.Red;
            rectangleRendererComponent2.Visible = false;
            rectangleRendererComponent2.SortingLayerName = "Layer2";
            rectangleRendererComponent2.OrderInLayer = -5;

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);

            var loadedEntity1 = loadedScene.RootEntities.Single(e => e.Name == "Entity1");
            AssertEntitiesAreEqual(loadedEntity1, entity1);
            var loadedComponent1 = loadedEntity1.GetComponent<RectangleRendererComponent>();
            Assert.That(loadedComponent1.Dimensions, Is.EqualTo(rectangleRendererComponent1.Dimensions));
            Assert.That(loadedComponent1.FillInterior, Is.EqualTo(rectangleRendererComponent1.FillInterior));
            Assert.That(loadedComponent1.Color, Is.EqualTo(rectangleRendererComponent1.Color));
            Assert.That(loadedComponent1.Visible, Is.EqualTo(rectangleRendererComponent1.Visible));
            Assert.That(loadedComponent1.SortingLayerName, Is.EqualTo(rectangleRendererComponent1.SortingLayerName));
            Assert.That(loadedComponent1.OrderInLayer, Is.EqualTo(rectangleRendererComponent1.OrderInLayer));

            var loadedEntity2 = loadedScene.RootEntities.Single(e => e.Name == "Entity2");
            AssertEntitiesAreEqual(loadedEntity2, entity2);
            var loadedComponent2 = loadedEntity2.GetComponent<RectangleRendererComponent>();
            Assert.That(loadedComponent2.Dimensions, Is.EqualTo(rectangleRendererComponent2.Dimensions));
            Assert.That(loadedComponent2.FillInterior, Is.EqualTo(rectangleRendererComponent2.FillInterior));
            Assert.That(loadedComponent2.Color, Is.EqualTo(rectangleRendererComponent2.Color));
            Assert.That(loadedComponent2.Visible, Is.EqualTo(rectangleRendererComponent2.Visible));
            Assert.That(loadedComponent2.SortingLayerName, Is.EqualTo(rectangleRendererComponent2.SortingLayerName));
            Assert.That(loadedComponent2.OrderInLayer, Is.EqualTo(rectangleRendererComponent2.OrderInLayer));
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile_GivenSceneWithEntityWithEllipseRenderer()
        {
            // Arrange
            var scene = SystemUnderTest.SceneFactory.Create();

            var entity1 = CreateEntity(scene, "Entity1");
            var ellipseRendererComponent1 = entity1.CreateComponent<EllipseRendererComponent>();
            ellipseRendererComponent1.RadiusX = 10.5;
            ellipseRendererComponent1.RadiusY = 20.5;
            ellipseRendererComponent1.FillInterior = true;
            ellipseRendererComponent1.Color = Color.Blue;
            ellipseRendererComponent1.Visible = true;
            ellipseRendererComponent1.SortingLayerName = "Layer1";
            ellipseRendererComponent1.OrderInLayer = 1;

            var entity2 = CreateEntity(scene, "Entity2");
            var ellipseRendererComponent2 = entity2.CreateComponent<EllipseRendererComponent>();
            ellipseRendererComponent2.RadiusX = 5.75;
            ellipseRendererComponent2.RadiusY = 8.25;
            ellipseRendererComponent2.FillInterior = false;
            ellipseRendererComponent2.Color = Color.Red;
            ellipseRendererComponent2.Visible = false;
            ellipseRendererComponent2.SortingLayerName = "Layer2";
            ellipseRendererComponent2.OrderInLayer = -5;

            // Act
            SystemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = SystemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            AssertScenesAreEqual(loadedScene, scene);

            var loadedEntity1 = loadedScene.RootEntities.Single(e => e.Name == "Entity1");
            AssertEntitiesAreEqual(loadedEntity1, entity1);
            var loadedComponent1 = loadedEntity1.GetComponent<EllipseRendererComponent>();
            Assert.That(loadedComponent1.RadiusX, Is.EqualTo(ellipseRendererComponent1.RadiusX));
            Assert.That(loadedComponent1.RadiusY, Is.EqualTo(ellipseRendererComponent1.RadiusY));
            Assert.That(loadedComponent1.FillInterior, Is.EqualTo(ellipseRendererComponent1.FillInterior));
            Assert.That(loadedComponent1.Color, Is.EqualTo(ellipseRendererComponent1.Color));
            Assert.That(loadedComponent1.Visible, Is.EqualTo(ellipseRendererComponent1.Visible));
            Assert.That(loadedComponent1.SortingLayerName, Is.EqualTo(ellipseRendererComponent1.SortingLayerName));
            Assert.That(loadedComponent1.OrderInLayer, Is.EqualTo(ellipseRendererComponent1.OrderInLayer));

            var loadedEntity2 = loadedScene.RootEntities.Single(e => e.Name == "Entity2");
            AssertEntitiesAreEqual(loadedEntity2, entity2);
            var loadedComponent2 = loadedEntity2.GetComponent<EllipseRendererComponent>();
            Assert.That(loadedComponent2.RadiusX, Is.EqualTo(ellipseRendererComponent2.RadiusX));
            Assert.That(loadedComponent2.RadiusY, Is.EqualTo(ellipseRendererComponent2.RadiusY));
            Assert.That(loadedComponent2.FillInterior, Is.EqualTo(ellipseRendererComponent2.FillInterior));
            Assert.That(loadedComponent2.Color, Is.EqualTo(ellipseRendererComponent2.Color));
            Assert.That(loadedComponent2.Visible, Is.EqualTo(ellipseRendererComponent2.Visible));
            Assert.That(loadedComponent2.SortingLayerName, Is.EqualTo(ellipseRendererComponent2.SortingLayerName));
            Assert.That(loadedComponent2.OrderInLayer, Is.EqualTo(ellipseRendererComponent2.OrderInLayer));
        }

        #endregion

        #region Helpers

        private static Entity CreateEntity(Scene scene, string name)
        {
            var entity = scene.CreateEntity();
            entity.Name = name;
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