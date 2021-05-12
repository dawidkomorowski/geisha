using System;
using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel.Serialization
{
    [TestFixture]
    public class ComponentDataSerializationTests
    {
        private SceneSerializer _sceneSerializer = null!;
        private TestComponent _component = null!;
        private TestComponent.Serializer _serializer = null!;

        [SetUp]
        public void SetUp()
        {
            var sceneFactory = Substitute.For<ISceneFactory>();
            sceneFactory.Create().Returns(ci => TestSceneFactory.Create());

            var sceneBehaviorFactoryProvider = Substitute.For<ISceneBehaviorFactoryProvider>();
            var emptySceneBehaviorFactory = Substitute.For<ISceneBehaviorFactory>();
            emptySceneBehaviorFactory.BehaviorName.Returns(string.Empty);
            emptySceneBehaviorFactory.Create(Arg.Any<Scene>())
                .Returns(ci => SceneBehavior.CreateEmpty(ci.Arg<Scene>()));
            sceneBehaviorFactoryProvider.Get(string.Empty).Returns(emptySceneBehaviorFactory);

            var componentFactoryProvider = Substitute.For<IComponentFactoryProvider>();
            componentFactoryProvider.Get(TestComponent.Id).Returns(new TestComponent.Factory());

            var componentSerializerProvider = Substitute.For<IComponentSerializerProvider>();
            _serializer = new TestComponent.Serializer();
            componentSerializerProvider.Get(TestComponent.Id).Returns(_serializer);

            _sceneSerializer = new SceneSerializer(sceneFactory, sceneBehaviorFactoryProvider, componentFactoryProvider, componentSerializerProvider);

            _component = new TestComponent();
        }

        [TestCase(null)]
        [TestCase("Not null")]
        public void SerializeAndDeserialize_String(string? value)
        {
            // Arrange
            _component.StringProperty = value;

            _serializer.SerializeAction = (component, writer) => writer.WriteStringProperty("StringProperty", component.StringProperty);
            _serializer.DeserializeAction = (component, reader) => component.StringProperty = reader.ReadStringProperty("StringProperty");

            // Act
            var actual = SerializeAndDeserialize();

            // Assert
            Assert.That(actual.StringProperty, Is.EqualTo(_component.StringProperty));
        }

        private TestComponent SerializeAndDeserialize()
        {
            var sceneToSerialize = TestSceneFactory.Create();
            var entity = new Entity();
            entity.AddComponent(_component);
            sceneToSerialize.AddEntity(entity);

            var json = _sceneSerializer.Serialize(sceneToSerialize);
            var deserializedScene = _sceneSerializer.Deserialize(json);

            return deserializedScene.RootEntities.Single().GetComponent<TestComponent>();
        }

        private sealed class TestComponent : IComponent
        {
            public static ComponentId Id { get; } = new ComponentId("TestComponent");
            public ComponentId ComponentId => Id;

            public string? StringProperty { get; set; }

            public sealed class Factory : IComponentFactory
            {
                public Type ComponentType { get; } = typeof(TestComponent);
                public ComponentId ComponentId => Id;
                public IComponent Create() => new TestComponent();
            }

            public sealed class Serializer : ComponentSerializer<TestComponent>
            {
                public Serializer() : base(Id)
                {
                    SerializeAction = (component, writer) => throw new InvalidOperationException($"{nameof(SerializeAction)} was not set.");
                    DeserializeAction = (component, reader) => throw new InvalidOperationException($"{nameof(DeserializeAction)} was not set.");
                }

                public Action<TestComponent, IComponentDataWriter> SerializeAction { get; set; }
                public Action<TestComponent, IComponentDataReader> DeserializeAction { get; set; }

                protected override void Serialize(TestComponent component, IComponentDataWriter componentDataWriter)
                {
                    SerializeAction(component, componentDataWriter);
                }

                protected override void Deserialize(TestComponent component, IComponentDataReader componentDataReader)
                {
                    DeserializeAction(component, componentDataReader);
                }
            }
        }
    }
}