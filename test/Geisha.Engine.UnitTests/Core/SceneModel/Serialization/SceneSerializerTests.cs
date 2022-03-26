using System;
using System.Collections.Generic;
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
        private IAssetStore _assetStore = null!;
        private SceneSerializer _sceneSerializer = null!;

        protected abstract Scene SerializeAndDeserialize(Scene scene);

        [SetUp]
        public void SetUp()
        {
            _sceneFactory = Substitute.For<ISceneFactory>();
            _sceneFactory.Create().Returns(ci => TestSceneFactory.Create(GetCustomComponentFactories()));

            _sceneBehaviorFactoryProvider = Substitute.For<ISceneBehaviorFactoryProvider>();
            _sceneBehaviorFactoryProvider.Get(string.Empty).Returns(new EmptySceneBehaviorFactory());

            _assetStore = Substitute.For<IAssetStore>();

            _sceneSerializer = new SceneSerializer(_sceneFactory, _sceneBehaviorFactoryProvider, _assetStore);
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
            _ = scene.CreateEntity();
            _ = scene.CreateEntity();
            _ = scene.CreateEntity();

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
            var scene = TestSceneFactory.Create();
            var entity = scene.CreateEntity();
            entity.Name = "Entity Name";

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
            var scene = TestSceneFactory.Create();
            var entity = scene.CreateEntity();
            _ = entity.CreateChildEntity();
            _ = entity.CreateChildEntity();
            _ = entity.CreateChildEntity();

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
            var scene = TestSceneFactory.Create();
            var root = scene.CreateEntity();
            var child1 = root.CreateChildEntity();
            var child2 = root.CreateChildEntity();
            _ = root.CreateChildEntity();

            _ = child1.CreateChildEntity();
            _ = child1.CreateChildEntity();
            _ = child2.CreateChildEntity();

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
            var scene = TestSceneFactory.Create(GetCustomComponentFactories());
            var entity = scene.CreateEntity();

            var testComponentA = entity.CreateComponent<TestComponentA>();
            testComponentA.DataA = "Data A";
            var testComponentB = entity.CreateComponent<TestComponentB>();
            testComponentB.DataB = "Data B";
            var testComponentC = entity.CreateComponent<TestComponentC>();
            testComponentC.DataC = "Data C";

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
            Assert.That(((TestComponentA)actualComponent1).DataA, Is.EqualTo("Data A"));
            Assert.That(((TestComponentB)actualComponent2).DataB, Is.EqualTo("Data B"));
            Assert.That(((TestComponentC)actualComponent3).DataC, Is.EqualTo("Data C"));
        }

        [Test]
        public void Serialize_and_Deserialize_SceneWithEntityWithComponentAccessingAssetStoreDuringSerialization()
        {
            // Arrange
            var scene = TestSceneFactory.Create(GetCustomComponentFactories());
            var entity = scene.CreateEntity();
            var componentToSerialize = entity.CreateComponent<AssetStoreTestComponent>();

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
            Assert.That(((AssetStoreTestComponent)deserializedComponent).DeserializeAssetStore, Is.EqualTo(_assetStore));
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

        private static IEnumerable<IComponentFactory> GetCustomComponentFactories() => new IComponentFactory[]
        {
            new TestComponentA.Factory(),
            new TestComponentB.Factory(),
            new TestComponentC.Factory(),
            new AssetStoreTestComponent.Factory()
        };

        #endregion
    }
}