using System;
using System.Linq;
using Geisha.Common.Math;
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

        [TestCase(true)]
        [TestCase(false)]
        public void SerializeAndDeserialize_Bool(bool value)
        {
            // Arrange
            _component.BoolProperty = value;

            _serializer.SerializeAction = (component, writer) => writer.WriteBoolProperty("BoolProperty", component.BoolProperty);
            _serializer.DeserializeAction = (component, reader) => component.BoolProperty = reader.ReadBoolProperty("BoolProperty");

            // Act
            var actual = SerializeAndDeserialize();

            // Assert
            Assert.That(actual.BoolProperty, Is.EqualTo(_component.BoolProperty));
        }

        [Test]
        public void SerializeAndDeserialize_Int()
        {
            // Arrange
            _component.IntProperty = 123;

            _serializer.SerializeAction = (component, writer) => writer.WriteIntProperty("IntProperty", component.IntProperty);
            _serializer.DeserializeAction = (component, reader) => component.IntProperty = reader.ReadIntProperty("IntProperty");

            // Act
            var actual = SerializeAndDeserialize();

            // Assert
            Assert.That(actual.IntProperty, Is.EqualTo(_component.IntProperty));
        }

        [Test]
        public void SerializeAndDeserialize_Double()
        {
            // Arrange
            _component.DoubleProperty = 123.456;

            _serializer.SerializeAction = (component, writer) => writer.WriteDoubleProperty("DoubleProperty", component.DoubleProperty);
            _serializer.DeserializeAction = (component, reader) => component.DoubleProperty = reader.ReadDoubleProperty("DoubleProperty");

            // Act
            var actual = SerializeAndDeserialize();

            // Assert
            Assert.That(actual.DoubleProperty, Is.EqualTo(_component.DoubleProperty));
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

        [TestCase(DateTimeKind.Local)]
        [TestCase(DateTimeKind.Utc)]
        public void SerializeAndDeserialize_Enum(DateTimeKind value)
        {
            // Arrange
            _component.EnumProperty = value;

            _serializer.SerializeAction = (component, writer) => writer.WriteEnumProperty("EnumProperty", component.EnumProperty);
            _serializer.DeserializeAction = (component, reader) => component.EnumProperty = reader.ReadEnumProperty<DateTimeKind>("EnumProperty");

            // Act
            var actual = SerializeAndDeserialize();

            // Assert
            Assert.That(actual.EnumProperty, Is.EqualTo(_component.EnumProperty));
        }

        [Test]
        public void SerializeAndDeserialize_Vector2()
        {
            // Arrange
            _component.Vector2Property = new Vector2(12.34, 56.78);

            _serializer.SerializeAction = (component, writer) => writer.WriteVector2Property("Vector2Property", component.Vector2Property);
            _serializer.DeserializeAction = (component, reader) => component.Vector2Property = reader.ReadVector2Property("Vector2Property");

            // Act
            var actual = SerializeAndDeserialize();

            // Assert
            Assert.That(actual.Vector2Property, Is.EqualTo(_component.Vector2Property));
        }

        [Test]
        public void SerializeAndDeserialize_Vector3()
        {
            // Arrange
            _component.Vector3Property = new Vector3(12.3, 45.6, 78.9);

            _serializer.SerializeAction = (component, writer) => writer.WriteVector3Property("Vector3Property", component.Vector3Property);
            _serializer.DeserializeAction = (component, reader) => component.Vector3Property = reader.ReadVector3Property("Vector3Property");

            // Act
            var actual = SerializeAndDeserialize();

            // Assert
            Assert.That(actual.Vector3Property, Is.EqualTo(_component.Vector3Property));
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

            public bool BoolProperty { get; set; }
            public int IntProperty { get; set; }
            public double DoubleProperty { get; set; }
            public string? StringProperty { get; set; }
            public DateTimeKind EnumProperty { get; set; }
            public Vector2 Vector2Property { get; set; }
            public Vector3 Vector3Property { get; set; }

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