using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel.Serialization
{
    [TestFixture]
    public class ComponentDataSerializationTests : ComponentSerializationTestsBase
    {
        private TestComponent _component = null!;

        protected override IComponentFactory ComponentFactory => new TestComponent.Factory();

        [SetUp]
        public void SetUp()
        {
            _component = new TestComponent();
        }

        [Test]
        public void SerializeAndDeserialize_Defined()
        {
            // Arrange
            var actual = false;

            TestComponent.SerializeAction = (component, writer) => writer.WriteString("DefinedProperty", "defined");
            TestComponent.DeserializeAction = (component, reader) => actual = reader.IsDefined("DefinedProperty");

            // Act
            SerializeAndDeserialize(_component);

            // Assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void SerializeAndDeserialize_Undefined()
        {
            // Arrange
            var actual = true;

            TestComponent.SerializeAction = (component, writer) => { };
            TestComponent.DeserializeAction = (component, reader) => actual = reader.IsDefined("UndefinedProperty");

            // Act
            SerializeAndDeserialize(_component);

            // Assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void SerializeAndDeserialize_Null()
        {
            // Arrange
            var actual = false;

            TestComponent.SerializeAction = (component, writer) => writer.WriteNull("NullProperty");
            TestComponent.DeserializeAction = (component, reader) => actual = reader.IsNull("NullProperty");

            // Act
            SerializeAndDeserialize(_component);

            // Assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void SerializeAndDeserialize_NotNull()
        {
            // Arrange
            var actual = true;

            TestComponent.SerializeAction = (component, writer) => writer.WriteString("NotNullProperty", "not null");
            TestComponent.DeserializeAction = (component, reader) => actual = reader.IsNull("NotNullProperty");

            // Act
            SerializeAndDeserialize(_component);

            // Assert
            Assert.That(actual, Is.False);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SerializeAndDeserialize_Bool(bool value)
        {
            // Arrange
            _component.BoolProperty = value;

            TestComponent.SerializeAction = (component, writer) => writer.WriteBool("BoolProperty", component.BoolProperty);
            TestComponent.DeserializeAction = (component, reader) => component.BoolProperty = reader.ReadBool("BoolProperty");

            // Act
            var actual = SerializeAndDeserialize(_component);

            // Assert
            Assert.That(actual.BoolProperty, Is.EqualTo(_component.BoolProperty));
        }

        [Test]
        public void SerializeAndDeserialize_Int()
        {
            // Arrange
            _component.IntProperty = 123;

            TestComponent.SerializeAction = (component, writer) => writer.WriteInt("IntProperty", component.IntProperty);
            TestComponent.DeserializeAction = (component, reader) => component.IntProperty = reader.ReadInt("IntProperty");

            // Act
            var actual = SerializeAndDeserialize(_component);

            // Assert
            Assert.That(actual.IntProperty, Is.EqualTo(_component.IntProperty));
        }

        [Test]
        public void SerializeAndDeserialize_Double()
        {
            // Arrange
            _component.DoubleProperty = 123.456;

            TestComponent.SerializeAction = (component, writer) => writer.WriteDouble("DoubleProperty", component.DoubleProperty);
            TestComponent.DeserializeAction = (component, reader) => component.DoubleProperty = reader.ReadDouble("DoubleProperty");

            // Act
            var actual = SerializeAndDeserialize(_component);

            // Assert
            Assert.That(actual.DoubleProperty, Is.EqualTo(_component.DoubleProperty));
        }

        [TestCase(null)]
        [TestCase("Not null")]
        public void SerializeAndDeserialize_String(string? value)
        {
            // Arrange
            _component.StringProperty = value;

            TestComponent.SerializeAction = (component, writer) => writer.WriteString("StringProperty", component.StringProperty);
            TestComponent.DeserializeAction = (component, reader) => component.StringProperty = reader.ReadString("StringProperty");

            // Act
            var actual = SerializeAndDeserialize(_component);

            // Assert
            Assert.That(actual.StringProperty, Is.EqualTo(_component.StringProperty));
        }

        [TestCase(DateTimeKind.Local)]
        [TestCase(DateTimeKind.Utc)]
        public void SerializeAndDeserialize_Enum(DateTimeKind value)
        {
            // Arrange
            _component.EnumProperty = value;

            TestComponent.SerializeAction = (component, writer) => writer.WriteEnum("EnumProperty", component.EnumProperty);
            TestComponent.DeserializeAction = (component, reader) => component.EnumProperty = reader.ReadEnum<DateTimeKind>("EnumProperty");

            // Act
            var actual = SerializeAndDeserialize(_component);

            // Assert
            Assert.That(actual.EnumProperty, Is.EqualTo(_component.EnumProperty));
        }

        [Test]
        public void SerializeAndDeserialize_Vector2()
        {
            // Arrange
            _component.Vector2Property = new Vector2(12.34, 56.78);

            TestComponent.SerializeAction = (component, writer) => writer.WriteVector2("Vector2Property", component.Vector2Property);
            TestComponent.DeserializeAction = (component, reader) => component.Vector2Property = reader.ReadVector2("Vector2Property");

            // Act
            var actual = SerializeAndDeserialize(_component);

            // Assert
            Assert.That(actual.Vector2Property, Is.EqualTo(_component.Vector2Property));
        }

        [Test]
        public void SerializeAndDeserialize_Vector3()
        {
            // Arrange
            _component.Vector3Property = new Vector3(12.3, 45.6, 78.9);

            TestComponent.SerializeAction = (component, writer) => writer.WriteVector3("Vector3Property", component.Vector3Property);
            TestComponent.DeserializeAction = (component, reader) => component.Vector3Property = reader.ReadVector3("Vector3Property");

            // Act
            var actual = SerializeAndDeserialize(_component);

            // Assert
            Assert.That(actual.Vector3Property, Is.EqualTo(_component.Vector3Property));
        }

        [Test]
        public void SerializeAndDeserialize_AssetId()
        {
            // Arrange
            _component.AssetIdProperty = AssetId.CreateUnique();

            TestComponent.SerializeAction = (component, writer) => writer.WriteAssetId("AssetIdProperty", component.AssetIdProperty);
            TestComponent.DeserializeAction = (component, reader) => component.AssetIdProperty = reader.ReadAssetId("AssetIdProperty");

            // Act
            var actual = SerializeAndDeserialize(_component);

            // Assert
            Assert.That(actual.AssetIdProperty, Is.EqualTo(_component.AssetIdProperty));
        }

        [Test]
        public void SerializeAndDeserialize_Color()
        {
            // Arrange
            _component.ColorProperty = Color.FromArgb(1, 2, 3, 4);

            TestComponent.SerializeAction = (component, writer) => writer.WriteColor("ColorProperty", component.ColorProperty);
            TestComponent.DeserializeAction = (component, reader) => component.ColorProperty = reader.ReadColor("ColorProperty");

            // Act
            var actual = SerializeAndDeserialize(_component);

            // Assert
            Assert.That(actual.ColorProperty, Is.EqualTo(_component.ColorProperty));
        }

        [Test]
        public void SerializeAndDeserialize_Object()
        {
            // Arrange
            _component.ObjectProperty = new TestComponent.CustomData
            {
                BoolProperty = true,
                IntProperty = 123,
                DoubleProperty = 123.456,
                StringProperty = "value",
                EnumProperty = DateTimeKind.Utc,
                Vector2Property = new Vector2(12.34, 56.78),
                Vector3Property = new Vector3(1.23, 4.56, 7.89),
                AssetIdProperty = AssetId.CreateUnique(),
                ColorProperty = Color.FromArgb(1, 2, 3, 4),
                ObjectProperty = new TestComponent.CustomNestedData
                {
                    IntProperty = 321,
                    DoubleProperty = 654.321
                }
            };

            TestComponent.SerializeAction = (component, writer) => writer.WriteObject("ObjectProperty", component.ObjectProperty, (customData, objectWriter) =>
            {
                objectWriter.WriteNull("NullProperty");
                objectWriter.WriteBool("BoolProperty", customData.BoolProperty);
                objectWriter.WriteInt("IntProperty", customData.IntProperty);
                objectWriter.WriteDouble("DoubleProperty", customData.DoubleProperty);
                objectWriter.WriteString("StringProperty", customData.StringProperty);
                objectWriter.WriteEnum("EnumProperty", customData.EnumProperty);
                objectWriter.WriteVector2("Vector2Property", customData.Vector2Property);
                objectWriter.WriteVector3("Vector3Property", customData.Vector3Property);
                objectWriter.WriteAssetId("AssetIdProperty", customData.AssetIdProperty);
                objectWriter.WriteColor("ColorProperty", customData.ColorProperty);
                objectWriter.WriteObject("ObjectProperty", customData.ObjectProperty, (customNestedData, nestedWriter) =>
                {
                    nestedWriter.WriteInt("IntProperty", customNestedData.IntProperty);
                    nestedWriter.WriteDouble("DoubleProperty", customNestedData.DoubleProperty);
                });
            });

            var actualDefined = false;
            var actualUndefined = true;
            var actualNull = false;
            var actualNotNull = true;

            TestComponent.DeserializeAction = (component, reader) => component.ObjectProperty = reader.ReadObject("ObjectProperty", objectReader =>
            {
                actualDefined = objectReader.IsDefined("BoolProperty");
                actualUndefined = objectReader.IsDefined("UndefinedProperty");
                actualNull = objectReader.IsNull("NullProperty");
                actualNotNull = objectReader.IsNull("BoolProperty");

                return new TestComponent.CustomData
                {
                    BoolProperty = objectReader.ReadBool("BoolProperty"),
                    IntProperty = objectReader.ReadInt("IntProperty"),
                    DoubleProperty = objectReader.ReadDouble("DoubleProperty"),
                    StringProperty = objectReader.ReadString("StringProperty"),
                    EnumProperty = objectReader.ReadEnum<DateTimeKind>("EnumProperty"),
                    Vector2Property = objectReader.ReadVector2("Vector2Property"),
                    Vector3Property = objectReader.ReadVector3("Vector3Property"),
                    AssetIdProperty = objectReader.ReadAssetId("AssetIdProperty"),
                    ColorProperty = objectReader.ReadColor("ColorProperty"),
                    ObjectProperty = objectReader.ReadObject("ObjectProperty", nestedReader => new TestComponent.CustomNestedData
                    {
                        IntProperty = nestedReader.ReadInt("IntProperty"),
                        DoubleProperty = nestedReader.ReadDouble("DoubleProperty")
                    })
                };
            });

            // Act
            var actual = SerializeAndDeserialize(_component);

            // Assert
            Assert.That(actualDefined, Is.True);
            Assert.That(actualUndefined, Is.False);
            Assert.That(actualNull, Is.True);
            Assert.That(actualNotNull, Is.False);
            Assert.That(actual.ObjectProperty.BoolProperty, Is.EqualTo(_component.ObjectProperty.BoolProperty));
            Assert.That(actual.ObjectProperty.IntProperty, Is.EqualTo(_component.ObjectProperty.IntProperty));
            Assert.That(actual.ObjectProperty.DoubleProperty, Is.EqualTo(_component.ObjectProperty.DoubleProperty));
            Assert.That(actual.ObjectProperty.StringProperty, Is.EqualTo(_component.ObjectProperty.StringProperty));
            Assert.That(actual.ObjectProperty.EnumProperty, Is.EqualTo(_component.ObjectProperty.EnumProperty));
            Assert.That(actual.ObjectProperty.Vector2Property, Is.EqualTo(_component.ObjectProperty.Vector2Property));
            Assert.That(actual.ObjectProperty.Vector3Property, Is.EqualTo(_component.ObjectProperty.Vector3Property));
            Assert.That(actual.ObjectProperty.AssetIdProperty, Is.EqualTo(_component.ObjectProperty.AssetIdProperty));
            Assert.That(actual.ObjectProperty.ColorProperty, Is.EqualTo(_component.ObjectProperty.ColorProperty));
            Assert.That(actual.ObjectProperty.ObjectProperty.IntProperty, Is.EqualTo(_component.ObjectProperty.ObjectProperty.IntProperty));
            Assert.That(actual.ObjectProperty.ObjectProperty.DoubleProperty, Is.EqualTo(_component.ObjectProperty.ObjectProperty.DoubleProperty));
        }

        private sealed class TestComponent : Component
        {
            public bool BoolProperty { get; set; }
            public int IntProperty { get; set; }
            public double DoubleProperty { get; set; }
            public string? StringProperty { get; set; }
            public DateTimeKind EnumProperty { get; set; }
            public Vector2 Vector2Property { get; set; }
            public Vector3 Vector3Property { get; set; }
            public AssetId AssetIdProperty { get; set; }
            public Color ColorProperty { get; set; }
            public CustomData ObjectProperty { get; set; } = new CustomData();

            public sealed class CustomData
            {
                public bool BoolProperty { get; set; }
                public int IntProperty { get; set; }
                public double DoubleProperty { get; set; }
                public string? StringProperty { get; set; }
                public DateTimeKind EnumProperty { get; set; }
                public Vector2 Vector2Property { get; set; }
                public Vector3 Vector3Property { get; set; }
                public AssetId AssetIdProperty { get; set; }
                public Color ColorProperty { get; set; }
                public CustomNestedData ObjectProperty { get; set; } = new CustomNestedData();
            }

            public sealed class CustomNestedData
            {
                public int IntProperty { get; set; }
                public double DoubleProperty { get; set; }
            }

            public static Action<TestComponent, IComponentDataWriter> SerializeAction { get; set; } = (component, writer) =>
                throw new InvalidOperationException($"{nameof(SerializeAction)} was not set.");

            public static Action<TestComponent, IComponentDataReader> DeserializeAction { get; set; } = (component, reader) =>
                throw new InvalidOperationException($"{nameof(DeserializeAction)} was not set.");

            protected internal override void Serialize(IComponentDataWriter componentDataWriter, IAssetStore assetStore)
            {
                base.Serialize(componentDataWriter, assetStore);
                SerializeAction(this, componentDataWriter);
            }

            protected internal override void Deserialize(IComponentDataReader componentDataReader, IAssetStore assetStore)
            {
                base.Deserialize(componentDataReader, assetStore);
                DeserializeAction(this, componentDataReader);
            }

            public sealed class Factory : ComponentFactory<TestComponent>
            {
                protected override TestComponent CreateComponent() => new TestComponent();
            }
        }
    }
}