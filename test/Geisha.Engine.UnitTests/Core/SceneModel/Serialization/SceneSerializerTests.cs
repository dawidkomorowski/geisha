using System;
using System.IO;
using System.Linq;
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
        private IComponentSerializerProvider _componentSerializerProvider = null!;
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
            _componentSerializerProvider = Substitute.For<IComponentSerializerProvider>();

            _sceneSerializer = new SceneSerializer(_sceneFactory, _sceneBehaviorFactoryProvider, _componentFactoryProvider, _componentSerializerProvider);
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

            _componentFactoryProvider.Get(TestComponentA.Id).Returns(new TestComponentA.Factory());
            _componentFactoryProvider.Get(TestComponentB.Id).Returns(new TestComponentB.Factory());
            _componentFactoryProvider.Get(TestComponentC.Id).Returns(new TestComponentC.Factory());

            _componentSerializerProvider.Get(TestComponentA.Id).Returns(new TestComponentA.Serializer());
            _componentSerializerProvider.Get(TestComponentB.Id).Returns(new TestComponentB.Serializer());
            _componentSerializerProvider.Get(TestComponentC.Id).Returns(new TestComponentC.Serializer());

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

        private sealed class TestComponentA : IComponent
        {
            public static ComponentId Id { get; } = new ComponentId("TestComponentA");
            public ComponentId ComponentId => Id;

            public string? DataA { get; set; }

            public sealed class Factory : IComponentFactory
            {
                public Type ComponentType { get; } = typeof(TestComponentA);
                public ComponentId ComponentId => Id;
                public IComponent Create() => new TestComponentA();
            }

            public sealed class Serializer : ComponentSerializer<TestComponentA>
            {
                public Serializer() : base(Id)
                {
                }

                protected override void Serialize(TestComponentA component, IComponentDataWriter componentDataWriter)
                {
                    componentDataWriter.WriteStringProperty("DataA", component.DataA);
                }

                protected override void Deserialize(TestComponentA component, IComponentDataReader componentDataReader)
                {
                    component.DataA = componentDataReader.ReadStringProperty("DataA");
                }
            }
        }

        private sealed class TestComponentB : IComponent
        {
            public static ComponentId Id { get; } = new ComponentId("TestComponentB");
            public ComponentId ComponentId => Id;

            public string? DataB { get; set; }

            public sealed class Factory : IComponentFactory
            {
                public Type ComponentType { get; } = typeof(TestComponentB);
                public ComponentId ComponentId => Id;
                public IComponent Create() => new TestComponentB();
            }

            public sealed class Serializer : ComponentSerializer<TestComponentB>
            {
                public Serializer() : base(Id)
                {
                }

                protected override void Serialize(TestComponentB component, IComponentDataWriter componentDataWriter)
                {
                    componentDataWriter.WriteStringProperty("DataB", component.DataB);
                }

                protected override void Deserialize(TestComponentB component, IComponentDataReader componentDataReader)
                {
                    component.DataB = componentDataReader.ReadStringProperty("DataB");
                }
            }
        }

        private sealed class TestComponentC : IComponent
        {
            public static ComponentId Id { get; } = new ComponentId("TestComponentC");
            public ComponentId ComponentId => Id;

            public string? DataC { get; set; }

            public sealed class Factory : IComponentFactory
            {
                public Type ComponentType { get; } = typeof(TestComponentC);
                public ComponentId ComponentId => Id;
                public IComponent Create() => new TestComponentC();
            }

            public sealed class Serializer : ComponentSerializer<TestComponentC>
            {
                public Serializer() : base(Id)
                {
                }

                protected override void Serialize(TestComponentC component, IComponentDataWriter componentDataWriter)
                {
                    componentDataWriter.WriteStringProperty("DataC", component.DataC);
                }

                protected override void Deserialize(TestComponentC component, IComponentDataReader componentDataReader)
                {
                    component.DataC = componentDataReader.ReadStringProperty("DataC");
                }
            }
        }

        #endregion
    }
}