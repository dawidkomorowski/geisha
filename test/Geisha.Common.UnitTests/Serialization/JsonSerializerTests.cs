using System;
using System.Collections.Generic;
using Geisha.Common.Serialization;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Serialization
{
    [TestFixture]
    public class JsonSerializerTests
    {
        private JsonSerializer _jsonSerializer = null!;

        [SetUp]
        public void SetUp()
        {
            _jsonSerializer = new JsonSerializer();
        }

        #region Serialize and Deserialize

        [Test]
        public void Serialize_and_Deserialize_ShouldSerializeAndDeserializeSimpleObject()
        {
            // Arrange
            var original = new SimpleObject
            {
                IntValue = 12,
                DoubleValue = -541543.124543,
                StringValue = "Some string value.",
                EnumValue = SimpleEnum.EnumValue2
            };

            // Act
            var json = _jsonSerializer.Serialize(original);
            var deserialized = _jsonSerializer.Deserialize<SimpleObject>(json);

            // Assert
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized, Is.TypeOf<SimpleObject>());
            Assert.That(deserialized.IntValue, Is.EqualTo(original.IntValue));
            Assert.That(deserialized.DoubleValue, Is.EqualTo(original.DoubleValue));
            Assert.That(deserialized.StringValue, Is.EqualTo(original.StringValue));
            Assert.That(deserialized.EnumValue, Is.EqualTo(original.EnumValue));
        }

        [Test]
        public void Serialize_and_Deserialize_ShouldSerializeAndDeserializeSimpleObjectGraph()
        {
            // Arrange
            var original = new SimpleObjectGraph
            {
                NodeName = "Root",
                NodeValue = SimpleObject.CreateRandom(),
                NodeChild1 = new SimpleObjectGraph
                {
                    NodeName = "Child 1",
                    NodeValue = SimpleObject.CreateRandom(),
                    NodeChild1 = null,
                    NodeChild2 = null
                },
                NodeChild2 = new SimpleObjectGraph
                {
                    NodeName = "Child 2",
                    NodeValue = SimpleObject.CreateRandom(),
                    NodeChild1 = null,
                    NodeChild2 = new SimpleObjectGraph
                    {
                        NodeName = "Child 2.2",
                        NodeValue = SimpleObject.CreateRandom(),
                        NodeChild1 = null,
                        NodeChild2 = null
                    }
                }
            };

            // Act
            var json = _jsonSerializer.Serialize(original);
            var deserialized = _jsonSerializer.Deserialize<SimpleObjectGraph>(json);

            // Assert
            var rootOriginal = original;
            var rootDeserialized = deserialized;
            Assert.That(rootDeserialized, Is.Not.Null);
            Assert.That(rootDeserialized, Is.TypeOf<SimpleObjectGraph>());
            Assert.That(rootDeserialized.NodeName, Is.EqualTo(rootOriginal.NodeName));
            Assert.That(rootDeserialized.NodeValue, Is.EqualTo(rootOriginal.NodeValue));

            var child1Original = rootOriginal.NodeChild1;
            var child1Deserialized = rootDeserialized.NodeChild1;
            Assert.That(child1Deserialized, Is.Not.Null);
            Assert.That(child1Deserialized, Is.TypeOf<SimpleObjectGraph>());
            Assert.That(child1Deserialized!.NodeName, Is.EqualTo(child1Original.NodeName));
            Assert.That(child1Deserialized.NodeValue, Is.EqualTo(child1Original.NodeValue));
            Assert.That(child1Deserialized.NodeChild1, Is.Null);
            Assert.That(child1Deserialized.NodeChild2, Is.Null);

            var child2Original = rootOriginal.NodeChild2;
            var child2Deserialized = rootDeserialized.NodeChild2;
            Assert.That(child2Deserialized, Is.Not.Null);
            Assert.That(child2Deserialized, Is.TypeOf<SimpleObjectGraph>());
            Assert.That(child2Deserialized!.NodeName, Is.EqualTo(child2Original.NodeName));
            Assert.That(child2Deserialized.NodeValue, Is.EqualTo(child2Original.NodeValue));
            Assert.That(child2Deserialized.NodeChild1, Is.Null);

            var child22Original = child2Original.NodeChild2;
            var child22Deserialized = child2Deserialized.NodeChild2;
            Assert.That(child22Deserialized, Is.Not.Null);
            Assert.That(child22Deserialized, Is.TypeOf<SimpleObjectGraph>());
            Assert.That(child22Deserialized!.NodeName, Is.EqualTo(child22Original.NodeName));
            Assert.That(child22Deserialized.NodeValue, Is.EqualTo(child22Original.NodeValue));
            Assert.That(child22Deserialized.NodeChild1, Is.Null);
            Assert.That(child22Deserialized.NodeChild2, Is.Null);
        }

        [Test]
        public void Serialize_and_Deserialize_ShouldSerializeAndDeserializeSimpleInterfaceContainer()
        {
            // Arrange
            var original = new SimpleInterfaceContainer
            {
                Implementation1 = new SimpleImplementation1
                {
                    StringValue = "String value of implementation 1",
                    IntValue = 12
                },
                Implementation2 = new SimpleImplementation2
                {
                    StringValue = "String value of implementation 2",
                    DoubleValue = -12345.67809
                },
                Implementation3 = new SimpleImplementation3
                {
                    StringValue = "String value of implementation 3",
                    SimpleObjectValue = SimpleObject.CreateRandom()
                }
            };

            // Act
            var json = _jsonSerializer.Serialize(original);
            var deserialized = _jsonSerializer.Deserialize<SimpleInterfaceContainer>(json);

            // Assert
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized, Is.TypeOf<SimpleInterfaceContainer>());

            var implementation1Original = original.Implementation1;
            var implementation1Deserialized = deserialized.Implementation1;
            Assert.That(implementation1Deserialized, Is.Not.Null);
            Assert.That(implementation1Deserialized, Is.TypeOf<SimpleImplementation1>());
            Assert.That(implementation1Deserialized!.StringValue, Is.EqualTo(implementation1Original.StringValue));
            Assert.That(((SimpleImplementation1) implementation1Deserialized).IntValue,
                Is.EqualTo(((SimpleImplementation1) implementation1Original).IntValue));

            var implementation2Original = original.Implementation2;
            var implementation2Deserialized = deserialized.Implementation2;
            Assert.That(implementation2Deserialized, Is.Not.Null);
            Assert.That(implementation2Deserialized, Is.TypeOf<SimpleImplementation2>());
            Assert.That(implementation2Deserialized!.StringValue, Is.EqualTo(implementation2Original.StringValue));
            Assert.That(((SimpleImplementation2) implementation2Deserialized).DoubleValue,
                Is.EqualTo(((SimpleImplementation2) implementation2Original).DoubleValue));

            var implementation3Original = original.Implementation3;
            var implementation3Deserialized = deserialized.Implementation3;
            Assert.That(implementation3Deserialized, Is.Not.Null);
            Assert.That(implementation3Deserialized, Is.TypeOf<SimpleImplementation3>());
            Assert.That(implementation3Deserialized!.StringValue, Is.EqualTo(implementation3Original.StringValue));
            Assert.That(((SimpleImplementation3) implementation3Deserialized).SimpleObjectValue,
                Is.EqualTo(((SimpleImplementation3) implementation3Original).SimpleObjectValue));
        }

        [Test]
        public void Serialize_and_Deserialize_ShouldSerializeAndDeserializeSimpleInterfaceCollection()
        {
            // Arrange
            var original = new SimpleInterfaceCollection
            {
                Collection = new List<ISimpleInterface>
                {
                    SimpleImplementation1.CreateRandom(),
                    SimpleImplementation2.CreateRandom(),
                    SimpleImplementation3.CreateRandom()
                }
            };

            // Act
            var json = _jsonSerializer.Serialize(original);
            var deserialized = _jsonSerializer.Deserialize<SimpleInterfaceCollection>(json);

            // Assert
            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized, Is.TypeOf<SimpleInterfaceCollection>());
            Assert.That(deserialized.Collection, Is.Not.Null);
            Assert.That(deserialized.Collection, Is.TypeOf<List<ISimpleInterface>>());
            Assert.That(deserialized.Collection, Is.Not.Empty);
            Assert.That(deserialized.Collection, Has.Count.EqualTo(3));
            CollectionAssert.AreEqual(deserialized.Collection, original.Collection);
        }

        [TestCase(SimpleEnum.EnumValue1)]
        [TestCase(SimpleEnum.EnumValue2)]
        [TestCase(SimpleEnum.EnumValue3)]
        public void Serialize_and_Deserialize_ShouldSerializeAndDeserializeEnumAsString(SimpleEnum value)
        {
            // Arrange
            // Act
            var json = _jsonSerializer.Serialize(value);
            var deserialized = _jsonSerializer.Deserialize<SimpleEnum>(json);

            // Assert
            Assert.That(json, Is.EqualTo($"\"{value.ToString()}\""));
            Assert.That(deserialized, Is.EqualTo(value));
        }

        #endregion

        #region DeserializePart

        [Test]
        public void DeserializePart_ShouldDeserializeSimpleProperty_GivenSimplePath()
        {
            // Arrange
            const string json = @"
{
    ""IntProperty"": 42
}
";

            // Act
            var actual = _jsonSerializer.DeserializePart<int>(json, "IntProperty");

            // Assert
            Assert.That(actual, Is.EqualTo(42));
        }

        [Test]
        public void DeserializePart_ShouldDeserializeSimpleProperty_GivenComplexSimplePath()
        {
            // Arrange
            const string json = @"
{
    ""Root"": {
        ""Level1"": {
            ""Level2"": {
               ""IntProperty"": 42
            }
        }
    }
}
";

            // Act
            var actual = _jsonSerializer.DeserializePart<int>(json, "Root.Level1.Level2.IntProperty");

            // Assert
            Assert.That(actual, Is.EqualTo(42));
        }

        [Test]
        public void DeserializePart_ShouldDeserializeSimpleObject_GivenComplexPath()
        {
            // Arrange
            const string json = @"
{
    ""Root"": {
        ""Level1"": {
            ""Level2"": {
               ""SimpleObject"": {
                    ""IntValue"": 12,
                    ""DoubleValue"": -541543.124543,
                    ""StringValue"": ""Some string value."",
                    ""EnumValue"": ""EnumValue2""
                }
            }
        }
    }
}
";

            // Act
            var actual = _jsonSerializer.DeserializePart<SimpleObject>(json, "Root.Level1.Level2.SimpleObject");

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual!.IntValue, Is.EqualTo(12));
            Assert.That(actual.DoubleValue, Is.EqualTo(-541543.124543));
            Assert.That(actual.StringValue, Is.EqualTo("Some string value."));
            Assert.That(actual.EnumValue, Is.EqualTo(SimpleEnum.EnumValue2));
        }

        [Test]
        public void DeserializePart_ShouldReturnDefaultValue_GivenPathToNotExistentElement()
        {
            // Arrange
            const string json = @"
{
    ""Root"": {
        ""Level1"": {
            ""Level2"": {
            }
        }
    }
}
";

            // Act
            var actualValueType = _jsonSerializer.DeserializePart<int>(json, "Root.Level1.Level2.IntProperty");
            var actualReferenceType = _jsonSerializer.DeserializePart<SimpleObject>(json, "Root.Level1.Level2.SimpleObject");

            // Assert
            Assert.That(actualValueType, Is.EqualTo(default(int)));
            Assert.That(actualReferenceType, Is.EqualTo(default(SimpleObject)));
        }

        #endregion

        #region Test classes

        public enum SimpleEnum
        {
            EnumValue1,
            EnumValue2,
            EnumValue3
        }

        private class SimpleObject
        {
            public int IntValue { get; set; }
            public double DoubleValue { get; set; }
            public string? StringValue { get; set; }
            public SimpleEnum EnumValue { get; set; }

            public static SimpleObject CreateRandom()
            {
                var random = new Random();

                return new SimpleObject
                {
                    IntValue = random.Next(),
                    DoubleValue = random.NextDouble(),
                    StringValue = Guid.NewGuid().ToString(),
                    EnumValue = (SimpleEnum) Enum.GetValues(typeof(SimpleEnum)).GetValue(random.Next(Enum.GetValues(typeof(SimpleEnum)).Length))!
                };
            }

            private bool Equals(SimpleObject other)
            {
                return IntValue == other.IntValue && DoubleValue.Equals(other.DoubleValue) && string.Equals(StringValue, other.StringValue) &&
                       EnumValue == other.EnumValue;
            }

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((SimpleObject) obj);
            }

            public override int GetHashCode() => 0;

        }

        private class SimpleObjectGraph
        {
            public string? NodeName { get; set; }
            public SimpleObject? NodeValue { get; set; }
            public SimpleObjectGraph? NodeChild1 { get; set; }
            public SimpleObjectGraph? NodeChild2 { get; set; }
        }

        private interface ISimpleInterface
        {
            string? StringValue { get; set; }
        }

        private class SimpleImplementation1 : ISimpleInterface
        {
            public string? StringValue { get; set; }
            public int IntValue { get; set; }

            public static SimpleImplementation1 CreateRandom()
            {
                var random = new Random();

                return new SimpleImplementation1
                {
                    StringValue = Guid.NewGuid().ToString(),
                    IntValue = random.Next()
                };
            }

            private bool Equals(SimpleImplementation1 other)
            {
                return string.Equals(StringValue, other.StringValue) && IntValue == other.IntValue;
            }

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((SimpleImplementation1) obj);
            }

            public override int GetHashCode() => 0;

        }

        private class SimpleImplementation2 : ISimpleInterface
        {
            public string? StringValue { get; set; }
            public double DoubleValue { get; set; }

            public static SimpleImplementation2 CreateRandom()
            {
                var random = new Random();

                return new SimpleImplementation2
                {
                    StringValue = Guid.NewGuid().ToString(),
                    DoubleValue = random.NextDouble()
                };
            }

            private bool Equals(SimpleImplementation2 other)
            {
                return string.Equals(StringValue, other.StringValue) && DoubleValue.Equals(other.DoubleValue);
            }

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((SimpleImplementation2) obj);
            }

            public override int GetHashCode() => 0;

        }

        private class SimpleImplementation3 : ISimpleInterface
        {
            public string? StringValue { get; set; }
            public SimpleObject? SimpleObjectValue { get; set; }

            public static SimpleImplementation3 CreateRandom()
            {
                return new SimpleImplementation3
                {
                    StringValue = Guid.NewGuid().ToString(),
                    SimpleObjectValue = SimpleObject.CreateRandom()
                };
            }

            private bool Equals(SimpleImplementation3 other)
            {
                return string.Equals(StringValue, other.StringValue) &&
                       Equals(SimpleObjectValue, other.SimpleObjectValue);
            }

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((SimpleImplementation3) obj);
            }

            public override int GetHashCode() => 0;
        }

        private class SimpleInterfaceContainer
        {
            public ISimpleInterface? Implementation1 { get; set; }
            public ISimpleInterface? Implementation2 { get; set; }
            public ISimpleInterface? Implementation3 { get; set; }
        }

        private class SimpleInterfaceCollection
        {
            public List<ISimpleInterface>? Collection { get; set; }
        }

        #endregion
    }
}