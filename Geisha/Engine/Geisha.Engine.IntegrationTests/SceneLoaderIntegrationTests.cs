using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Definition;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests
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