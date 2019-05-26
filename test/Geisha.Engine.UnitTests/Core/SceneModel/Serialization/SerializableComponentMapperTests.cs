using System.Collections.Generic;
using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel.Serialization
{
    [TestFixture]
    public class SerializableComponentMapperTests
    {
        #region IsApplicableFor

        [Test]
        public void IsApplicableForComponent_ShouldReturnTrue_GivenComponentMarkedWith_SerializableComponentAttribute()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();

            // Act
            // Assert
            Assert.That(mapper.IsApplicableForComponent(new EmptyTestComponent()), Is.True);
        }

        [Test]
        public void IsApplicableForComponent_ShouldReturnFalse_GivenComponentNotMarkedWith_SerializableComponentAttribute()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();

            // Act
            // Assert
            Assert.That(mapper.IsApplicableForComponent(Substitute.For<IComponent>()), Is.False);
        }

        [Test]
        public void IsApplicableForSerializableComponent_ShouldReturnTrue_GivenSerializableOfType_SerializableComponent()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();

            // Act
            // Assert
            Assert.That(mapper.IsApplicableForSerializableComponent(new SerializableComponent()), Is.True);
        }

        [Test]
        public void IsApplicableForSerializableComponent_ShouldReturnFalse_GivenSerializableOfTypeDifferentThan_SerializableComponent()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();

            // Act
            // Assert
            Assert.That(mapper.IsApplicableForSerializableComponent(Substitute.For<ISerializableComponent>()), Is.False);
        }

        #endregion

        #region MapToSerializable

        [Test]
        public void MapToSerializable_ShouldReturnSerializableComponentWithComponentTypeAsTypeOfGivenComponent()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();

            // Act
            var actual = (SerializableComponent) mapper.MapToSerializable(new EmptyTestComponent());

            // Assert
            Assert.That(actual.ComponentType, Contains.Substring(typeof(EmptyTestComponent).FullName));
        }

        [Test]
        public void MapToSerializable_ShouldReturnEmptySerializableComponent_GivenEmptyComponent()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();

            // Act
            var actual = (SerializableComponent) mapper.MapToSerializable(new EmptyTestComponent());

            // Assert
            Assert.That(actual.IntProperties, Is.Empty);
            Assert.That(actual.DoubleProperties, Is.Empty);
            Assert.That(actual.StringProperties, Is.Empty);
        }

        [Test]
        public void MapToSerializable_ShouldReturnSerializableComponentWithIntProperty_GivenComponentWithIntProperty()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();
            var component = new IntPropertyTestComponent
            {
                IntProperty = 17
            };

            // Act
            var actual = (SerializableComponent) mapper.MapToSerializable(component);

            // Assert
            Assert.That(actual.IntProperties, Has.Count.EqualTo(1));
            Assert.That(actual.IntProperties, Contains.Key(nameof(IntPropertyTestComponent.IntProperty)));
            Assert.That(actual.IntProperties[nameof(IntPropertyTestComponent.IntProperty)], Is.EqualTo(component.IntProperty));
            Assert.That(actual.DoubleProperties, Is.Empty);
            Assert.That(actual.StringProperties, Is.Empty);
        }

        [Test]
        public void MapToSerializable_ShouldReturnSerializableComponentWithDoubleProperty_GivenComponentWithDoubleProperty()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();
            var component = new DoublePropertyTestComponent
            {
                DoubleProperty = 1.23
            };

            // Act
            var actual = (SerializableComponent) mapper.MapToSerializable(component);

            // Assert
            Assert.That(actual.DoubleProperties, Has.Count.EqualTo(1));
            Assert.That(actual.DoubleProperties, Contains.Key(nameof(DoublePropertyTestComponent.DoubleProperty)));
            Assert.That(actual.DoubleProperties[nameof(DoublePropertyTestComponent.DoubleProperty)], Is.EqualTo(component.DoubleProperty));
            Assert.That(actual.IntProperties, Is.Empty);
            Assert.That(actual.StringProperties, Is.Empty);
        }

        [Test]
        public void MapToSerializable_ShouldReturnSerializableComponentWithStringProperty_GivenComponentWithStringProperty()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();
            var component = new StringPropertyTestComponent
            {
                StringProperty = "value"
            };

            // Act
            var actual = (SerializableComponent) mapper.MapToSerializable(component);

            // Assert
            Assert.That(actual.StringProperties, Has.Count.EqualTo(1));
            Assert.That(actual.StringProperties, Contains.Key(nameof(StringPropertyTestComponent.StringProperty)));
            Assert.That(actual.StringProperties[nameof(StringPropertyTestComponent.StringProperty)], Is.EqualTo(component.StringProperty));
            Assert.That(actual.IntProperties, Is.Empty);
            Assert.That(actual.DoubleProperties, Is.Empty);
        }

        [Test]
        public void MapToSerializable_ShouldReturnSerializableComponentWithManyProperties_GivenComponentWithManyProperties()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();
            var component = new ManyPropertiesTestComponent
            {
                IntProperty = 17,
                DoubleProperty = 1.23,
                StringProperty = "value"
            };

            // Act
            var actual = (SerializableComponent) mapper.MapToSerializable(component);

            // Assert
            Assert.That(actual.IntProperties, Has.Count.EqualTo(1));
            Assert.That(actual.IntProperties, Contains.Key(nameof(IntPropertyTestComponent.IntProperty)));
            Assert.That(actual.IntProperties[nameof(IntPropertyTestComponent.IntProperty)], Is.EqualTo(component.IntProperty));
            Assert.That(actual.DoubleProperties, Has.Count.EqualTo(1));
            Assert.That(actual.DoubleProperties, Contains.Key(nameof(DoublePropertyTestComponent.DoubleProperty)));
            Assert.That(actual.DoubleProperties[nameof(DoublePropertyTestComponent.DoubleProperty)], Is.EqualTo(component.DoubleProperty));
            Assert.That(actual.StringProperties, Has.Count.EqualTo(1));
            Assert.That(actual.StringProperties, Contains.Key(nameof(ManyPropertiesTestComponent.StringProperty)));
            Assert.That(actual.StringProperties[nameof(ManyPropertiesTestComponent.StringProperty)], Is.EqualTo(component.StringProperty));
        }

        [Test]
        public void MapToSerializable_ShouldThrowException_GivenComponentWithUnsupportedPropertyType()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();

            // Act
            // Assert
            Assert.That(() => mapper.MapToSerializable(new UnsupportedPropertyTestComponent()),
                Throws.TypeOf<GeishaEngineException>().With.Message.Contains("Component contains property of unsupported type."));
        }

        [Test]
        public void MapToSerializable_ShouldReturnSerializableComponentOnlyWithPropertiesMarkedWith_SerializablePropertyAttribute()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();
            var component = new NotMarkedPropertiesTestComponent();

            // Act
            var actual = (SerializableComponent) mapper.MapToSerializable(component);

            // Assert
            Assert.That(actual.IntProperties, Has.Count.EqualTo(3));
            Assert.That(actual.IntProperties, Contains.Key(nameof(NotMarkedPropertiesTestComponent.Property1)));
            Assert.That(actual.IntProperties, Contains.Key(nameof(NotMarkedPropertiesTestComponent.Property2)));
            Assert.That(actual.IntProperties, Contains.Key(nameof(NotMarkedPropertiesTestComponent.Property3)));
            Assert.That(actual.IntProperties, Does.Not.ContainKey(nameof(NotMarkedPropertiesTestComponent.NotMarkedProperty1)));
            Assert.That(actual.IntProperties, Does.Not.ContainKey(nameof(NotMarkedPropertiesTestComponent.NotMarkedProperty2)));
            Assert.That(actual.IntProperties, Does.Not.ContainKey(nameof(NotMarkedPropertiesTestComponent.NotMarkedProperty3)));
        }

        #endregion

        #region MapFromSerializable

        [Test]
        public void MapFromSerializable_ShouldThrowException_GivenSerializableComponentWithInvalidComponentType()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();
            var serializableComponent = new SerializableComponent
            {
                ComponentType = "invalid component type"
            };

            // Act
            // Assert
            Assert.That(() => mapper.MapFromSerializable(serializableComponent),
                Throws.InvalidOperationException.With.Message.Contains(serializableComponent.ComponentType));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnComponentOfTypeAsDefinedInComponentType()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();
            var serializableComponent = new SerializableComponent
            {
                ComponentType = $"{typeof(EmptyTestComponent).FullName}, {typeof(EmptyTestComponent).Assembly.GetName().Name}"
            };

            // Act
            var actual = mapper.MapFromSerializable(serializableComponent);

            // Assert
            Assert.That(actual, Is.TypeOf<EmptyTestComponent>());
        }

        [Test]
        public void MapFromSerializable_ShouldReturnComponentWithIntProperty_GivenSerializableComponentWithIntProperty()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();
            var serializableComponent = new SerializableComponent
            {
                ComponentType = $"{typeof(IntPropertyTestComponent).FullName}, {typeof(IntPropertyTestComponent).Assembly.GetName().Name}",
                IntProperties = new Dictionary<string, int>
                {
                    [$"{nameof(IntPropertyTestComponent.IntProperty)}"] = 17
                }
            };

            // Act
            var actual = (IntPropertyTestComponent) mapper.MapFromSerializable(serializableComponent);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.IntProperty, Is.EqualTo(serializableComponent.IntProperties[nameof(IntPropertyTestComponent.IntProperty)]));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnComponentWithDoubleProperty_GivenSerializableComponentWithDoubleProperty()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();
            var serializableComponent = new SerializableComponent
            {
                ComponentType = $"{typeof(DoublePropertyTestComponent).FullName}, {typeof(DoublePropertyTestComponent).Assembly.GetName().Name}",
                DoubleProperties = new Dictionary<string, double>
                {
                    [$"{nameof(DoublePropertyTestComponent.DoubleProperty)}"] = 1.23
                }
            };

            // Act
            var actual = (DoublePropertyTestComponent) mapper.MapFromSerializable(serializableComponent);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.DoubleProperty, Is.EqualTo(serializableComponent.DoubleProperties[nameof(DoublePropertyTestComponent.DoubleProperty)]));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnComponentWithStringProperty_GivenSerializableComponentWithStringProperty()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();
            var serializableComponent = new SerializableComponent
            {
                ComponentType = $"{typeof(StringPropertyTestComponent).FullName}, {typeof(StringPropertyTestComponent).Assembly.GetName().Name}",
                StringProperties = new Dictionary<string, string>
                {
                    [$"{nameof(StringPropertyTestComponent.StringProperty)}"] = "value"
                }
            };

            // Act
            var actual = (StringPropertyTestComponent) mapper.MapFromSerializable(serializableComponent);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.StringProperty, Is.EqualTo(serializableComponent.StringProperties[nameof(StringPropertyTestComponent.StringProperty)]));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnComponentWithManyProperties_GivenSerializableComponentWithManyProperties()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();
            var serializableComponent = new SerializableComponent
            {
                ComponentType = $"{typeof(ManyPropertiesTestComponent).FullName}, {typeof(ManyPropertiesTestComponent).Assembly.GetName().Name}",
                IntProperties = new Dictionary<string, int>
                {
                    [$"{nameof(ManyPropertiesTestComponent.IntProperty)}"] = 17
                },
                DoubleProperties = new Dictionary<string, double>
                {
                    [$"{nameof(ManyPropertiesTestComponent.DoubleProperty)}"] = 1.23
                },
                StringProperties = new Dictionary<string, string>
                {
                    [$"{nameof(ManyPropertiesTestComponent.StringProperty)}"] = "value"
                }
            };

            // Act
            var actual = (ManyPropertiesTestComponent) mapper.MapFromSerializable(serializableComponent);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.IntProperty, Is.EqualTo(serializableComponent.IntProperties[nameof(ManyPropertiesTestComponent.IntProperty)]));
            Assert.That(actual.DoubleProperty, Is.EqualTo(serializableComponent.DoubleProperties[nameof(ManyPropertiesTestComponent.DoubleProperty)]));
            Assert.That(actual.StringProperty, Is.EqualTo(serializableComponent.StringProperties[nameof(ManyPropertiesTestComponent.StringProperty)]));
        }

        [Test]
        public void MapFromSerializable_ShouldThrowException_GivenSerializableComponentWithComponentTypeThatContainsUnsupportedPropertyType()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();

            // Act
            // Assert
            Assert.That(
                () => mapper.MapFromSerializable(new SerializableComponent
                {
                    ComponentType = $"{typeof(UnsupportedPropertyTestComponent).FullName}, {typeof(UnsupportedPropertyTestComponent).Assembly.GetName().Name}"
                }), Throws.TypeOf<GeishaEngineException>().With.Message.Contains("Component contains property of unsupported type."));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnComponentInitializedOnlyWithPropertiesMarkedWith_SerializablePropertyAttribute()
        {
            // Arrange
            var mapper = new SerializableComponentMapper();
            var serializableComponent = new SerializableComponent
            {
                ComponentType = $"{typeof(NotMarkedPropertiesTestComponent).FullName}, {typeof(NotMarkedPropertiesTestComponent).Assembly.GetName().Name}",
                IntProperties = new Dictionary<string, int>
                {
                    [$"{nameof(NotMarkedPropertiesTestComponent.Property1)}"] = 1,
                    [$"{nameof(NotMarkedPropertiesTestComponent.Property2)}"] = 2,
                    [$"{nameof(NotMarkedPropertiesTestComponent.Property3)}"] = 3,
                    [$"{nameof(NotMarkedPropertiesTestComponent.NotMarkedProperty1)}"] = 1,
                    [$"{nameof(NotMarkedPropertiesTestComponent.NotMarkedProperty2)}"] = 2,
                    [$"{nameof(NotMarkedPropertiesTestComponent.NotMarkedProperty3)}"] = 3
                }
            };

            // Act
            var actual = (NotMarkedPropertiesTestComponent) mapper.MapFromSerializable(serializableComponent);

            // Assert
            Assert.That(actual.Property1, Is.EqualTo(serializableComponent.IntProperties[nameof(NotMarkedPropertiesTestComponent.Property1)]));
            Assert.That(actual.Property2, Is.EqualTo(serializableComponent.IntProperties[nameof(NotMarkedPropertiesTestComponent.Property2)]));
            Assert.That(actual.Property3, Is.EqualTo(serializableComponent.IntProperties[nameof(NotMarkedPropertiesTestComponent.Property3)]));
            Assert.That(actual.NotMarkedProperty1, Is.EqualTo(default(int)));
            Assert.That(actual.NotMarkedProperty2, Is.EqualTo(default(int)));
            Assert.That(actual.NotMarkedProperty3, Is.EqualTo(default(int)));
        }

        #endregion

        #region Test classes

        [SerializableComponent]
        private class EmptyTestComponent : IComponent
        {
        }

        [SerializableComponent]
        private class IntPropertyTestComponent : IComponent
        {
            [SerializableProperty]
            public int IntProperty { get; set; }
        }

        [SerializableComponent]
        private class DoublePropertyTestComponent : IComponent
        {
            [SerializableProperty]
            public double DoubleProperty { get; set; }
        }

        [SerializableComponent]
        private class StringPropertyTestComponent : IComponent
        {
            [SerializableProperty]
            public string StringProperty { get; set; }
        }

        [SerializableComponent]
        private class ManyPropertiesTestComponent : IComponent
        {
            [SerializableProperty]
            public int IntProperty { get; set; }

            [SerializableProperty]
            public double DoubleProperty { get; set; }

            [SerializableProperty]
            public string StringProperty { get; set; }
        }

        [SerializableComponent]
        private class UnsupportedPropertyTestComponent : IComponent
        {
            [SerializableProperty]
            public object UnsupportedProperty { get; set; }
        }

        [SerializableComponent]
        private class NotMarkedPropertiesTestComponent : IComponent
        {
            [SerializableProperty]
            public int Property1 { get; set; }

            [SerializableProperty]
            public int Property2 { get; set; }

            [SerializableProperty]
            public int Property3 { get; set; }

            public int NotMarkedProperty1 { get; set; }
            public int NotMarkedProperty2 { get; set; }
            public int NotMarkedProperty3 { get; set; }
        }

        #endregion
    }
}