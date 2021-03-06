﻿using System;
using System.IO;
using System.Linq;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel.Serialization
{
    [TestFixture]
    public abstract class SceneSerializerTests
    {
        private ISceneFactory _sceneFactory = null!;
        private ISceneBehaviorFactoryProvider _sceneBehaviorFactoryProvider = null!;
        private IComponentFactoryProvider _componentFactoryProvider = null!;
        private IAssetStore _assetStore = null!;
        private SceneSerializer _sceneSerializer = null!;

        protected abstract Scene SerializeAndDeserialize(Scene scene);

        [SetUp]
        public void SetUp()
        {
            _sceneFactory = Substitute.For<ISceneFactory>();
            _sceneFactory.Create().Returns(ci => TestSceneFactory.Create());

            _sceneBehaviorFactoryProvider = Substitute.For<ISceneBehaviorFactoryProvider>();
            var emptySceneBehaviorFactory = Substitute.For<ISceneBehaviorFactory>();
            emptySceneBehaviorFactory.BehaviorName.Returns(string.Empty);
            emptySceneBehaviorFactory.Create(Arg.Any<Scene>())
                .Returns(ci => SceneBehavior.CreateEmpty(ci.Arg<Scene>()));
            _sceneBehaviorFactoryProvider.Get(string.Empty).Returns(emptySceneBehaviorFactory);

            _componentFactoryProvider = Substitute.For<IComponentFactoryProvider>();
            _assetStore = Substitute.For<IAssetStore>();

            _sceneSerializer = new SceneSerializer(_sceneFactory, _sceneBehaviorFactoryProvider, _componentFactoryProvider, _assetStore);
        }

        [Test]
        public void Serialize_and_Deserialize_EmptyScene()
        {
            // Arrange
            var scene = TestSceneFactory.Create();

            // Act
            var actual = SerializeAndDeserialize(scene);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RootEntities, Has.Count.Zero);
            Assert.That(actual.SceneBehavior.Name, Is.EqualTo(SceneBehavior.CreateEmpty(actual).Name));
        }

        [Test]
        public void Serialize_and_Deserialize_SceneWithSceneBehavior()
        {
            // Arrange
            var sceneBehaviorName = Guid.NewGuid().ToString();

            var emptyScene = TestSceneFactory.Create();
            _sceneFactory.Create().Returns(emptyScene);

            var sceneBehavior = Substitute.ForPartsOf<SceneBehavior>(emptyScene);
            sceneBehavior.Name.Returns(sceneBehaviorName);

            var sceneBehaviorFactory = Substitute.For<ISceneBehaviorFactory>();
            sceneBehaviorFactory.BehaviorName.Returns(sceneBehaviorName);
            sceneBehaviorFactory.Create(emptyScene).Returns(sceneBehavior);

            _sceneBehaviorFactoryProvider.Get(sceneBehaviorName).Returns(sceneBehaviorFactory);

            // Prepare scene to serialize
            var sceneToSerialize = TestSceneFactory.Create();

            var sceneBehaviorToSerialize = Substitute.ForPartsOf<SceneBehavior>(sceneToSerialize);
            sceneBehaviorToSerialize.Name.Returns(sceneBehaviorName);

            sceneToSerialize.SceneBehavior = sceneBehaviorToSerialize;

            // Act
            var actual = SerializeAndDeserialize(sceneToSerialize);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RootEntities, Has.Count.Zero);
            Assert.That(actual.SceneBehavior.Name, Is.EqualTo(sceneBehaviorName));
        }

        [Test]
        public void Serialize_and_Deserialize_SceneWithRootEntities()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            scene.AddEntity(new Entity());
            scene.AddEntity(new Entity());
            scene.AddEntity(new Entity());

            // Act
            var actual = SerializeAndDeserialize(scene);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RootEntities, Has.Count.EqualTo(3));
        }

        [Test]
        public void Serialize_and_Deserialize_SceneWithEntityWithName()
        {
            // Arrange
            var entity = new Entity
            {
                Name = "Entity Name"
            };

            var scene = TestSceneFactory.Create();
            scene.AddEntity(entity);

            // Act
            var actual = SerializeAndDeserialize(scene);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RootEntities, Has.Count.EqualTo(1));
            Assert.That(actual.RootEntities.Single().Name, Is.EqualTo("Entity Name"));
        }

        [Test]
        public void Serialize_and_Deserialize_SceneWithEntityWithChildren()
        {
            // Arrange
            var entity = new Entity();
            entity.AddChild(new Entity());
            entity.AddChild(new Entity());
            entity.AddChild(new Entity());

            var scene = TestSceneFactory.Create();
            scene.AddEntity(entity);

            // Act
            var actual = SerializeAndDeserialize(scene);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RootEntities, Has.Count.EqualTo(1));
            Assert.That(actual.RootEntities.Single().Children, Has.Count.EqualTo(3));
        }

        [Test]
        public void Serialize_and_Deserialize_SceneWithEntityGraph()
        {
            // Arrange
            var root = new Entity();
            var child1 = new Entity {Parent = root};
            var child2 = new Entity {Parent = root};
            _ = new Entity {Parent = root};

            child1.AddChild(new Entity());
            child1.AddChild(new Entity());
            child2.AddChild(new Entity());

            var scene = TestSceneFactory.Create();
            scene.AddEntity(root);

            // Act
            var actual = SerializeAndDeserialize(scene);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RootEntities, Has.Count.EqualTo(1));

            var actualRoot = actual.RootEntities.Single();
            Assert.That(actualRoot.Children, Has.Count.EqualTo(3));
            Assert.That(actualRoot.Children.ElementAt(0).Children, Has.Count.EqualTo(2));
            Assert.That(actualRoot.Children.ElementAt(1).Children, Has.Count.EqualTo(1));
            Assert.That(actualRoot.Children.ElementAt(2).Children, Has.Count.Zero);
        }

        [Test]
        public void Serialize_and_Deserialize_SceneWithEntityWithComponents()
        {
            // Arrange
            var entity = new Entity();
            entity.AddComponent(new TestComponentA {DataA = "Data A"});
            entity.AddComponent(new TestComponentB {DataB = "Data B"});
            entity.AddComponent(new TestComponentC {DataC = "Data C"});

            var scene = TestSceneFactory.Create();
            scene.AddEntity(entity);

            _componentFactoryProvider.Get(ComponentId.Of<TestComponentA>()).Returns(new TestComponentA.Factory());
            _componentFactoryProvider.Get(ComponentId.Of<TestComponentB>()).Returns(new TestComponentB.Factory());
            _componentFactoryProvider.Get(ComponentId.Of<TestComponentC>()).Returns(new TestComponentC.Factory());

            // Act
            var actual = SerializeAndDeserialize(scene);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RootEntities, Has.Count.EqualTo(1));
            var actualEntity = actual.RootEntities.Single();
            Assert.That(actualEntity.Components, Has.Count.EqualTo(3));
            var actualComponent1 = actualEntity.Components.ElementAt(0);
            var actualComponent2 = actualEntity.Components.ElementAt(1);
            var actualComponent3 = actualEntity.Components.ElementAt(2);
            Assert.That(actualComponent1, Is.TypeOf<TestComponentA>());
            Assert.That(actualComponent2, Is.TypeOf<TestComponentB>());
            Assert.That(actualComponent3, Is.TypeOf<TestComponentC>());
            Assert.That(((TestComponentA) actualComponent1).DataA, Is.EqualTo("Data A"));
            Assert.That(((TestComponentB) actualComponent2).DataB, Is.EqualTo("Data B"));
            Assert.That(((TestComponentC) actualComponent3).DataC, Is.EqualTo("Data C"));
        }

        [Test]
        public void Serialize_and_Deserialize_SceneWithEntityWithComponentAccessingAssetStoreDuringSerialization()
        {
            // Arrange
            var entity = new Entity();
            var componentToSerialize = new AssetStoreTestComponent();
            entity.AddComponent(componentToSerialize);

            var scene = TestSceneFactory.Create();
            scene.AddEntity(entity);

            _componentFactoryProvider.Get(ComponentId.Of<AssetStoreTestComponent>()).Returns(new AssetStoreTestComponent.Factory());

            // Act
            var actual = SerializeAndDeserialize(scene);

            // Assert
            Assert.That(componentToSerialize.SerializeAssetStore, Is.EqualTo(_assetStore));
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.RootEntities, Has.Count.EqualTo(1));
            var actualEntity = actual.RootEntities.Single();
            Assert.That(actualEntity.Components, Has.Count.EqualTo(1));
            var deserializedComponent = actualEntity.Components.ElementAt(0);
            Assert.That(deserializedComponent, Is.TypeOf<AssetStoreTestComponent>());
            Assert.That(((AssetStoreTestComponent) deserializedComponent).DeserializeAssetStore, Is.EqualTo(_assetStore));
        }

        [TestFixture]
        public sealed class SceneSerializerTestsUsingStream : SceneSerializerTests
        {
            protected override Scene SerializeAndDeserialize(Scene scene)
            {
                using var memoryStream = new MemoryStream();
                _sceneSerializer.Serialize(scene, memoryStream);
                memoryStream.Position = 0;
                return _sceneSerializer.Deserialize(memoryStream);
            }
        }

        [TestFixture]
        public sealed class SceneSerializerTestsUsingString : SceneSerializerTests
        {
            protected override Scene SerializeAndDeserialize(Scene scene)
            {
                var json = _sceneSerializer.Serialize(scene);
                return _sceneSerializer.Deserialize(json);
            }
        }

        #region Helpers

        [ComponentId("TestComponentA")]
        private sealed class TestComponentA : Component
        {
            public string? DataA { get; set; }

            protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
            {
                base.Serialize(writer, assetStore);
                writer.WriteString("DataA", DataA);
            }

            protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
            {
                base.Deserialize(reader, assetStore);
                DataA = reader.ReadString("DataA");
            }

            public sealed class Factory : ComponentFactory<TestComponentA>
            {
                protected override TestComponentA CreateComponent() => new TestComponentA();
            }
        }

        [ComponentId("TestComponentB")]
        private sealed class TestComponentB : Component
        {
            public string? DataB { get; set; }

            protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
            {
                base.Serialize(writer, assetStore);
                writer.WriteString("DataB", DataB);
            }

            protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
            {
                base.Deserialize(reader, assetStore);
                DataB = reader.ReadString("DataB");
            }

            public sealed class Factory : ComponentFactory<TestComponentB>
            {
                protected override TestComponentB CreateComponent() => new TestComponentB();
            }
        }

        [ComponentId("TestComponentC")]
        private sealed class TestComponentC : Component
        {
            public string? DataC { get; set; }

            protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
            {
                base.Serialize(writer, assetStore);
                writer.WriteString("DataC", DataC);
            }

            protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
            {
                base.Deserialize(reader, assetStore);
                DataC = reader.ReadString("DataC");
            }

            public sealed class Factory : ComponentFactory<TestComponentC>
            {
                protected override TestComponentC CreateComponent() => new TestComponentC();
            }
        }

        private sealed class AssetStoreTestComponent : Component
        {
            public IAssetStore? SerializeAssetStore { get; set; }
            public IAssetStore? DeserializeAssetStore { get; set; }

            protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
            {
                base.Serialize(writer, assetStore);
                SerializeAssetStore = assetStore;
            }

            protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
            {
                base.Deserialize(reader, assetStore);
                DeserializeAssetStore = assetStore;
            }

            public sealed class Factory : ComponentFactory<AssetStoreTestComponent>
            {
                protected override AssetStoreTestComponent CreateComponent() => new AssetStoreTestComponent();
            }
        }

        #endregion
    }
}