using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel.Serialization
{
    [TestFixture]
    public class AutomaticComponentDefinitionMapperTests
    {
        #region IsApplicableFor

        [Test]
        public void IsApplicableForComponent_ShouldReturnTrueGivenComponentMarkedWith_SerializableComponentAttribute()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            // Assert
            Assert.That(mapper.IsApplicableForComponent(new EmptyTestComponent()), Is.True);
        }

        [Test]
        public void IsApplicableForComponent_ShouldReturnFalseGivenComponentNotMarkedWith_SerializableComponentAttribute()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            // Assert
            Assert.That(mapper.IsApplicableForComponent(Substitute.For<IComponent>()), Is.False);
        }

        [Test]
        public void IsApplicableForSerializableComponent_ShouldReturnTrueGivenSerializableComponentOfType_AutomaticComponentDefinition()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            // Assert
            Assert.That(mapper.IsApplicableForSerializableComponent(new AutomaticComponentDefinition()), Is.True);
        }

        [Test]
        public void IsApplicableForSerializableComponent_ShouldReturnFalseGivenSerializableComponentOfTypeDifferentThan_AutomaticComponentDefinition()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            // Assert
            Assert.That(mapper.IsApplicableForSerializableComponent(Substitute.For<ISerializableComponent>()), Is.False);
        }

        #endregion

        #region MapToSerializable

        [Test]
        public void MapToSerializable_ShouldReturnAutomaticComponentDefinitionWithComponentTypeAsTypeOfGivenComponent()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            var actual = (AutomaticComponentDefinition) mapper.MapToSerializable(new EmptyTestComponent());

            // Assert
            Assert.That(actual.ComponentType, Contains.Substring(typeof(EmptyTestComponent).FullName));
        }

        [Test]
        public void MapToSerializable_ShouldReturnEmptyAutomaticComponentDefinition_GivenEmptyComponent()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            var actual = (AutomaticComponentDefinition) mapper.MapToSerializable(new EmptyTestComponent());

            // Assert
            Assert.That(actual.IntProperties, Is.Empty);
            Assert.That(actual.DoubleProperties, Is.Empty);
            Assert.That(actual.StringProperties, Is.Empty);
        }

        [Test]
        public void MapToSerializable_ShouldReturnAutomaticComponentDefinitionWithIntProperty_GivenComponentWithIntProperty()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var component = new IntPropertyTestComponent
            {
                IntProperty = 17
            };

            // Act
            var actual = (AutomaticComponentDefinition) mapper.MapToSerializable(component);

            // Assert
            Assert.That(actual.IntProperties, Has.Count.EqualTo(1));
            Assert.That(actual.IntProperties, Contains.Key(nameof(IntPropertyTestComponent.IntProperty)));
            Assert.That(actual.IntProperties[nameof(IntPropertyTestComponent.IntProperty)], Is.EqualTo(component.IntProperty));
            Assert.That(actual.DoubleProperties, Is.Empty);
            Assert.That(actual.StringProperties, Is.Empty);
        }

        [Test]
        public void MapToSerializable_ShouldReturnAutomaticComponentDefinitionWithDoubleProperty_GivenComponentWithDoubleProperty()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var component = new DoublePropertyTestComponent
            {
                DoubleProperty = 1.23
            };

            // Act
            var actual = (AutomaticComponentDefinition) mapper.MapToSerializable(component);

            // Assert
            Assert.That(actual.DoubleProperties, Has.Count.EqualTo(1));
            Assert.That(actual.DoubleProperties, Contains.Key(nameof(DoublePropertyTestComponent.DoubleProperty)));
            Assert.That(actual.DoubleProperties[nameof(DoublePropertyTestComponent.DoubleProperty)], Is.EqualTo(component.DoubleProperty));
            Assert.That(actual.IntProperties, Is.Empty);
            Assert.That(actual.StringProperties, Is.Empty);
        }

        [Test]
        public void MapToSerializable_ShouldReturnAutomaticComponentDefinitionWithStringProperty_GivenComponentWithStringProperty()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var component = new StringPropertyTestComponent
            {
                StringProperty = "value"
            };

            // Act
            var actual = (AutomaticComponentDefinition) mapper.MapToSerializable(component);

            // Assert
            Assert.That(actual.StringProperties, Has.Count.EqualTo(1));
            Assert.That(actual.StringProperties, Contains.Key(nameof(StringPropertyTestComponent.StringProperty)));
            Assert.That(actual.StringProperties[nameof(StringPropertyTestComponent.StringProperty)], Is.EqualTo(component.StringProperty));
            Assert.That(actual.IntProperties, Is.Empty);
            Assert.That(actual.DoubleProperties, Is.Empty);
        }

        [Test]
        public void MapToSerializable_ShouldReturnAutomaticComponentDefinitionWithManyProperties_GivenComponentWithManyProperties()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var component = new ManyPropertiesTestComponent
            {
                IntProperty = 17,
                DoubleProperty = 1.23,
                StringProperty = "value"
            };

            // Act
            var actual = (AutomaticComponentDefinition) mapper.MapToSerializable(component);

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
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            // Assert
            Assert.That(() => mapper.MapToSerializable(new UnsupportedPropertyTestComponent()),
                Throws.TypeOf<GeishaEngineException>().With.Message.Contains("Component contains property of unsupported type."));
        }

        [Test]
        public void MapToSerializable_ShouldReturnAutomaticComponentDefinitionOnlyWithPropertiesMarkedWith_PropertyDefinitionAttribute()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var component = new NotMarkedPropertiesTestComponent();

            // Act
            var actual = (AutomaticComponentDefinition) mapper.MapToSerializable(component);

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
        public void MapFromSerializable_ShouldThrowExceptionGivenDefinitionWithInvalidComponentType()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var componentDefinition = new AutomaticComponentDefinition
            {
                ComponentType = "invalid component type"
            };

            // Act
            // Assert
            Assert.That(() => mapper.MapFromSerializable(componentDefinition),
                Throws.InvalidOperationException.With.Message.Contains(componentDefinition.ComponentType));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnComponentOfTypeAsDefinedInComponentType()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var componentDefinition = new AutomaticComponentDefinition
            {
                ComponentType = $"{typeof(EmptyTestComponent).FullName}, {typeof(EmptyTestComponent).Assembly.GetName().Name}"
            };

            // Act
            var actual = mapper.MapFromSerializable(componentDefinition);

            // Assert
            Assert.That(actual, Is.TypeOf<EmptyTestComponent>());
        }

        [Test]
        public void MapFromSerializable_ShouldReturnComponentWithIntProperty_GivenAutomaticComponentDefinitionWithIntProperty()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var componentDefinition = new AutomaticComponentDefinition
            {
                ComponentType = $"{typeof(IntPropertyTestComponent).FullName}, {typeof(IntPropertyTestComponent).Assembly.GetName().Name}",
                IntProperties = new Dictionary<string, int>
                {
                    [$"{nameof(IntPropertyTestComponent.IntProperty)}"] = 17
                }
            };

            // Act
            var actual = (IntPropertyTestComponent) mapper.MapFromSerializable(componentDefinition);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.IntProperty, Is.EqualTo(componentDefinition.IntProperties[nameof(IntPropertyTestComponent.IntProperty)]));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnComponentWithDoubleProperty_GivenAutomaticComponentDefinitionWithDoubleProperty()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var componentDefinition = new AutomaticComponentDefinition
            {
                ComponentType = $"{typeof(DoublePropertyTestComponent).FullName}, {typeof(DoublePropertyTestComponent).Assembly.GetName().Name}",
                DoubleProperties = new Dictionary<string, double>
                {
                    [$"{nameof(DoublePropertyTestComponent.DoubleProperty)}"] = 1.23
                }
            };

            // Act
            var actual = (DoublePropertyTestComponent) mapper.MapFromSerializable(componentDefinition);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.DoubleProperty, Is.EqualTo(componentDefinition.DoubleProperties[nameof(DoublePropertyTestComponent.DoubleProperty)]));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnComponentWithStringProperty_GivenAutomaticComponentDefinitionWithStringProperty()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var componentDefinition = new AutomaticComponentDefinition
            {
                ComponentType = $"{typeof(StringPropertyTestComponent).FullName}, {typeof(StringPropertyTestComponent).Assembly.GetName().Name}",
                StringProperties = new Dictionary<string, string>
                {
                    [$"{nameof(StringPropertyTestComponent.StringProperty)}"] = "value"
                }
            };

            // Act
            var actual = (StringPropertyTestComponent) mapper.MapFromSerializable(componentDefinition);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.StringProperty, Is.EqualTo(componentDefinition.StringProperties[nameof(StringPropertyTestComponent.StringProperty)]));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnComponentWithManyProperties_GivenAutomaticComponentDefinitionWithManyProperties()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var componentDefinition = new AutomaticComponentDefinition
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
            var actual = (ManyPropertiesTestComponent) mapper.MapFromSerializable(componentDefinition);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.IntProperty, Is.EqualTo(componentDefinition.IntProperties[nameof(ManyPropertiesTestComponent.IntProperty)]));
            Assert.That(actual.DoubleProperty, Is.EqualTo(componentDefinition.DoubleProperties[nameof(ManyPropertiesTestComponent.DoubleProperty)]));
            Assert.That(actual.StringProperty, Is.EqualTo(componentDefinition.StringProperties[nameof(ManyPropertiesTestComponent.StringProperty)]));
        }

        [Test]
        public void MapFromSerializable_ShouldThrowException_GivenAutomaticComponentDefinitionWithComponentTypeThatContainsUnsupportedPropertyType()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            // Assert
            Assert.That(
                () => mapper.MapFromSerializable(new AutomaticComponentDefinition
                {
                    ComponentType = $"{typeof(UnsupportedPropertyTestComponent).FullName}, {typeof(UnsupportedPropertyTestComponent).Assembly.GetName().Name}"
                }), Throws.TypeOf<GeishaEngineException>().With.Message.Contains("Component contains property of unsupported type."));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnComponentInitializedOnlyWithPropertiesMarkedWith_PropertyDefinitionAttribute()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var componentDefinition = new AutomaticComponentDefinition
            {
                ComponentType = $"{typeof(NotMarkedPropertiesTestComponent).FullName}, {typeof(NotMarkedPropertiesTestComponent).Assembly.GetName().Name}",
                IntProperties = new Dictionary<string, int>
                {
                    [$"{nameof(NotMarkedPropertiesTestComponent.Property1)}"] = 1,
                    [$"{nameof(NotMarkedPropertiesTestComponent.Property2)}"] = 2,
                    [$"{nameof(NotMarkedPropertiesTestComponent.Property3)}"] = 3
                }
            };

            // Act
            var actual = (NotMarkedPropertiesTestComponent) mapper.MapFromSerializable(componentDefinition);

            // Assert
            Assert.That(actual.Property1, Is.EqualTo(componentDefinition.IntProperties[nameof(NotMarkedPropertiesTestComponent.Property1)]));
            Assert.That(actual.Property2, Is.EqualTo(componentDefinition.IntProperties[nameof(NotMarkedPropertiesTestComponent.Property2)]));
            Assert.That(actual.Property3, Is.EqualTo(componentDefinition.IntProperties[nameof(NotMarkedPropertiesTestComponent.Property3)]));
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
            [PropertyDefinition]
            public int IntProperty { get; set; }
        }

        [SerializableComponent]
        private class DoublePropertyTestComponent : IComponent
        {
            [PropertyDefinition]
            public double DoubleProperty { get; set; }
        }

        [SerializableComponent]
        private class StringPropertyTestComponent : IComponent
        {
            [PropertyDefinition]
            public string StringProperty { get; set; }
        }

        [SerializableComponent]
        private class ManyPropertiesTestComponent : IComponent
        {
            [PropertyDefinition]
            public int IntProperty { get; set; }

            [PropertyDefinition]
            public double DoubleProperty { get; set; }

            [PropertyDefinition]
            public string StringProperty { get; set; }
        }

        [SerializableComponent]
        private class UnsupportedPropertyTestComponent : IComponent
        {
            [PropertyDefinition]
            public object UnsupportedProperty { get; set; }
        }

        [SerializableComponent]
        private class NotMarkedPropertiesTestComponent : IComponent
        {
            [PropertyDefinition]
            public int Property1 { get; set; }

            [PropertyDefinition]
            public int Property2 { get; set; }

            [PropertyDefinition]
            public int Property3 { get; set; }

            public int NotMarkedProperty1 { get; set; }
            public int NotMarkedProperty2 { get; set; }
            public int NotMarkedProperty3 { get; set; }
        }

        #endregion
    }
}