using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
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
    [TestFixture]
    public class SceneLoaderIntegrationTests
    {
        private SystemUnderTest _systemUnderTest;
        private string _sceneFilePath;
        private readonly Random _random = new Random();

        [SetUp]
        public void SetUp()
        {
            var compositionContainer = new CompositionContainer(new ApplicationCatalog());
            _systemUnderTest = compositionContainer.GetExportedValue<SystemUnderTest>();
            _sceneFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, Path.GetRandomFileName());
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_sceneFilePath))
            {
                File.Delete(_sceneFilePath);
            }
        }

        [Test]
        public void SaveAndLoad_ShouldSaveSceneToFileAndThenLoadItFromFile()
        {
            // Arrange
            var scene = new Scene();

            var emptyEntity = NewEntityWithRandomName();
            scene.AddEntity(emptyEntity);

            var entityWithChildren = NewEntityWithRandomName();
            entityWithChildren.AddChild(NewEntityWithRandomName());
            entityWithChildren.AddChild(NewEntityWithRandomName());
            entityWithChildren.AddChild(NewEntityWithRandomName());
            scene.AddEntity(entityWithChildren);

            var entityWithTransform = NewEntityWithRandomName();
            entityWithTransform.AddComponent(new Transform
            {
                Translation = new Vector3(RandomDouble(), RandomDouble(), RandomDouble()),
                Rotation = new Vector3(RandomDouble(), RandomDouble(), RandomDouble()),
                Scale = new Vector3(RandomDouble(), RandomDouble(), RandomDouble())
            });
            scene.AddEntity(entityWithTransform);

            var entityWithBehavior = NewEntityWithRandomName();
            entityWithBehavior.AddComponent(new TestBehavior
            {
                IntProperty = RandomInt(),
                DoubleProperty = RandomDouble(),
                StringProperty = RandomString()
            });
            scene.AddEntity(entityWithBehavior);

            // Act
            _systemUnderTest.SceneLoader.Save(scene, _sceneFilePath);
            var loadedScene = _systemUnderTest.SceneLoader.Load(_sceneFilePath);

            // Assert
            Assert.That(loadedScene, Is.Not.Null);
            Assert.That(loadedScene.RootEntities.Count, Is.EqualTo(scene.RootEntities.Count));
            Assert.That(loadedScene.AllEntities.Count(), Is.EqualTo(scene.AllEntities.Count()));

            // Empty entity
            AssertEntitiesAreEqual(loadedScene.RootEntities[0], emptyEntity);

            // Entity with children
            AssertEntitiesAreEqual(loadedScene.RootEntities[1], entityWithChildren);

            // Entity with transform
            AssertEntitiesAreEqual(loadedScene.RootEntities[2], entityWithTransform);
            var transform = entityWithTransform.GetComponent<Transform>();
            var loadedTransform = loadedScene.RootEntities[2].GetComponent<Transform>();
            Assert.That(loadedTransform.Translation, Is.EqualTo(transform.Translation));
            Assert.That(loadedTransform.Rotation, Is.EqualTo(transform.Rotation));
            Assert.That(loadedTransform.Scale, Is.EqualTo(transform.Scale));

            // Entity with behavior
            AssertEntitiesAreEqual(loadedScene.RootEntities[3], entityWithBehavior);
            var testBehavior = entityWithBehavior.GetComponent<TestBehavior>();
            var loadedTestBehavior = loadedScene.RootEntities[3].GetComponent<TestBehavior>();
            Assert.That(loadedTestBehavior.IntProperty, Is.EqualTo(testBehavior.IntProperty));
            Assert.That(loadedTestBehavior.DoubleProperty, Is.EqualTo(testBehavior.DoubleProperty));
            Assert.That(loadedTestBehavior.StringProperty, Is.EqualTo(testBehavior.StringProperty));
        }

        private int RandomInt()
        {
            return _random.Next();
        }

        private double RandomDouble()
        {
            return _random.NextDouble();
        }

        private static string RandomString()
        {
            return Guid.NewGuid().ToString();
        }

        private static Entity NewEntityWithRandomName()
        {
            return new Entity
            {
                Name = RandomString()
            };
        }

        private void AssertEntitiesAreEqual(Entity entity1, Entity entity2)
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

        [Export]
        private class SystemUnderTest
        {
            [ImportingConstructor]
            public SystemUnderTest(IAssetStore assetStore, ISceneLoader sceneLoader)
            {
                AssetStore = assetStore;
                SceneLoader = sceneLoader;
            }

            public IAssetStore AssetStore { get; }
            public ISceneLoader SceneLoader { get; }
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
    }
}