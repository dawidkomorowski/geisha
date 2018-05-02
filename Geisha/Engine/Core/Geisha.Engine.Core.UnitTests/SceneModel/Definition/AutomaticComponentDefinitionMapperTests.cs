using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Definition;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel.Definition
{
    [TestFixture]
    public class AutomaticComponentDefinitionMapperTests
    {
        #region IsApplicableFor

        [Test]
        public void IsApplicableForComponent_ShouldReturnTrueGivenComponentMarkedWith_ComponentDefinitionAttribute()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            // Assert
            Assert.That(mapper.IsApplicableForComponent(new EmptyTestComponent()), Is.True);
        }

        [Test]
        public void IsApplicableForComponent_ShouldReturnFalseGivenComponentNotMarkedWith_ComponentDefinitionAttribute()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            // Assert
            Assert.That(mapper.IsApplicableForComponent(Substitute.For<IComponent>()), Is.False);
        }

        [Test]
        public void IsApplicableForComponentDefinition_ShouldReturnTrueGivenComponentDefinitionOfType_AutomaticComponentDefinition()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            // Assert
            Assert.That(mapper.IsApplicableForComponentDefinition(new AutomaticComponentDefinition()), Is.True);
        }

        [Test]
        public void IsApplicableForComponentDefinition_ShouldReturnFalseGivenComponentDefinitionOfTypeDifferentThan_AutomaticComponentDefinition()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            // Assert
            Assert.That(mapper.IsApplicableForComponentDefinition(Substitute.For<IComponentDefinition>()), Is.False);
        }

        #endregion

        #region ToDefinition

        [Test]
        public void ToDefinition_ShouldReturnAutomaticComponentDefinitionWithComponentTypeAsTypeOfGivenComponent()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            var actual = (AutomaticComponentDefinition) mapper.ToDefinition(new EmptyTestComponent());

            // Assert
            Assert.That(actual.ComponentType, Contains.Substring(typeof(EmptyTestComponent).FullName));
        }

        [Test]
        public void ToDefinition_ShouldReturnEmptyAutomaticComponentDefinition_GivenEmptyComponent()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            var actual = (AutomaticComponentDefinition) mapper.ToDefinition(new EmptyTestComponent());

            // Assert
            Assert.That(actual.Properties, Is.Empty);
        }

        [Test]
        public void ToDefinition_ShouldReturnAutomaticComponentDefinitionWithIntProperty_GivenComponentWithIntProperty()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var component = new IntPropertyTestComponent
            {
                IntProperty = 17
            };

            // Act
            var actual = (AutomaticComponentDefinition) mapper.ToDefinition(component);

            // Assert
            Assert.That(actual.Properties, Has.Count.EqualTo(1));
            Assert.That(actual.Properties, Contains.Key(nameof(IntPropertyTestComponent.IntProperty)));
            Assert.That(actual.Properties[nameof(IntPropertyTestComponent.IntProperty)], Is.EqualTo(component.IntProperty));
        }

        [Test]
        public void ToDefinition_ShouldReturnAutomaticComponentDefinitionWithDoubleProperty_GivenComponentWithDoubleProperty()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var component = new DoublePropertyTestComponent
            {
                DoubleProperty = 1.23
            };

            // Act
            var actual = (AutomaticComponentDefinition) mapper.ToDefinition(component);

            // Assert
            Assert.That(actual.Properties, Has.Count.EqualTo(1));
            Assert.That(actual.Properties, Contains.Key(nameof(DoublePropertyTestComponent.DoubleProperty)));
            Assert.That(actual.Properties[nameof(DoublePropertyTestComponent.DoubleProperty)], Is.EqualTo(component.DoubleProperty));
        }

        [Test]
        public void ToDefinition_ShouldReturnAutomaticComponentDefinitionWithStringProperty_GivenComponentWithStringProperty()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var component = new StringPropertyTestComponent
            {
                StringProperty = "value"
            };

            // Act
            var actual = (AutomaticComponentDefinition) mapper.ToDefinition(component);

            // Assert
            Assert.That(actual.Properties, Has.Count.EqualTo(1));
            Assert.That(actual.Properties, Contains.Key(nameof(StringPropertyTestComponent.StringProperty)));
            Assert.That(actual.Properties[nameof(StringPropertyTestComponent.StringProperty)], Is.EqualTo(component.StringProperty));
        }

        [Test]
        public void ToDefinition_ShouldReturnAutomaticComponentDefinitionWithManyProperties_GivenComponentWithManyProperties()
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
            var actual = (AutomaticComponentDefinition) mapper.ToDefinition(component);

            // Assert
            Assert.That(actual.Properties, Has.Count.EqualTo(3));
            Assert.That(actual.Properties, Contains.Key(nameof(IntPropertyTestComponent.IntProperty)));
            Assert.That(actual.Properties[nameof(IntPropertyTestComponent.IntProperty)], Is.EqualTo(component.IntProperty));
            Assert.That(actual.Properties, Contains.Key(nameof(DoublePropertyTestComponent.DoubleProperty)));
            Assert.That(actual.Properties[nameof(DoublePropertyTestComponent.DoubleProperty)], Is.EqualTo(component.DoubleProperty));
            Assert.That(actual.Properties, Contains.Key(nameof(ManyPropertiesTestComponent.StringProperty)));
            Assert.That(actual.Properties[nameof(ManyPropertiesTestComponent.StringProperty)], Is.EqualTo(component.StringProperty));
        }

        [Test]
        public void ToDefinition_ShouldThrowException_GivenComponentWithUnsupportedPropertyType()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            // Assert
            Assert.That(() => mapper.ToDefinition(new UnsupportedPropertyTestComponent()),
                Throws.TypeOf<GeishaEngineException>().With.Message.Contains("Component contains property of unsupported type."));
        }

        [Test]
        public void ToDefinition_ShouldReturnAutomaticComponentDefinitionOnlyWithPropertiesMarkedWith_PropertyDefinitionAttribute()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var component = new NotMarkedPropertiesTestComponent();

            // Act
            var actual = (AutomaticComponentDefinition) mapper.ToDefinition(component);

            // Assert
            Assert.That(actual.Properties, Has.Count.EqualTo(3));
            Assert.That(actual.Properties, Contains.Key(nameof(NotMarkedPropertiesTestComponent.Property1)));
            Assert.That(actual.Properties, Contains.Key(nameof(NotMarkedPropertiesTestComponent.Property2)));
            Assert.That(actual.Properties, Contains.Key(nameof(NotMarkedPropertiesTestComponent.Property3)));
            Assert.That(actual.Properties, Does.Not.ContainKey(nameof(NotMarkedPropertiesTestComponent.NotMarkedProperty1)));
            Assert.That(actual.Properties, Does.Not.ContainKey(nameof(NotMarkedPropertiesTestComponent.NotMarkedProperty2)));
            Assert.That(actual.Properties, Does.Not.ContainKey(nameof(NotMarkedPropertiesTestComponent.NotMarkedProperty3)));
        }

        #endregion

        #region FromDefinition

        [Test]
        public void FromDefinition_ShouldThrowExceptionGivenDefinitionWithInvalidComponentType()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var componentDefinition = new AutomaticComponentDefinition
            {
                ComponentType = "invalid component type"
            };

            // Act
            // Assert
            Assert.That(() => mapper.FromDefinition(componentDefinition),
                Throws.InvalidOperationException.With.Message.Contains(componentDefinition.ComponentType));
        }

        [Test]
        public void FromDefinition_ShouldReturnComponentOfTypeAsDefinedInComponentType()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var componentDefinition = new AutomaticComponentDefinition
            {
                ComponentType = $"{typeof(EmptyTestComponent).FullName}, {typeof(EmptyTestComponent).Assembly.GetName().Name}"
            };

            // Act
            var actual = mapper.FromDefinition(componentDefinition);

            // Assert
            Assert.That(actual, Is.TypeOf<EmptyTestComponent>());
        }

        [Test]
        public void FromDefinition_ShouldReturnComponentWithIntProperty_GivenAutomaticComponentDefinitionWithIntProperty()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var componentDefinition = new AutomaticComponentDefinition
            {
                ComponentType = $"{typeof(IntPropertyTestComponent).FullName}, {typeof(IntPropertyTestComponent).Assembly.GetName().Name}",
                Properties = new Dictionary<string, object>
                {
                    [$"{nameof(IntPropertyTestComponent.IntProperty)}"] = 17
                }
            };

            // Act
            var actual = (IntPropertyTestComponent) mapper.FromDefinition(componentDefinition);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.IntProperty, Is.EqualTo(componentDefinition.Properties[nameof(IntPropertyTestComponent.IntProperty)]));
        }

        [Test]
        public void FromDefinition_ShouldReturnComponentWithDoubleProperty_GivenAutomaticComponentDefinitionWithDoubleProperty()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var componentDefinition = new AutomaticComponentDefinition
            {
                ComponentType = $"{typeof(DoublePropertyTestComponent).FullName}, {typeof(DoublePropertyTestComponent).Assembly.GetName().Name}",
                Properties = new Dictionary<string, object>
                {
                    [$"{nameof(DoublePropertyTestComponent.DoubleProperty)}"] = 1.23
                }
            };

            // Act
            var actual = (DoublePropertyTestComponent) mapper.FromDefinition(componentDefinition);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.DoubleProperty, Is.EqualTo(componentDefinition.Properties[nameof(DoublePropertyTestComponent.DoubleProperty)]));
        }

        [Test]
        public void FromDefinition_ShouldReturnComponentWithStringProperty_GivenAutomaticComponentDefinitionWithStringProperty()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var componentDefinition = new AutomaticComponentDefinition
            {
                ComponentType = $"{typeof(StringPropertyTestComponent).FullName}, {typeof(StringPropertyTestComponent).Assembly.GetName().Name}",
                Properties = new Dictionary<string, object>
                {
                    [$"{nameof(StringPropertyTestComponent.StringProperty)}"] = "value"
                }
            };

            // Act
            var actual = (StringPropertyTestComponent) mapper.FromDefinition(componentDefinition);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.StringProperty, Is.EqualTo(componentDefinition.Properties[nameof(StringPropertyTestComponent.StringProperty)]));
        }

        [Test]
        public void FromDefinition_ShouldReturnComponentWithManyProperties_GivenAutomaticComponentDefinitionWithManyProperties()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var componentDefinition = new AutomaticComponentDefinition
            {
                ComponentType = $"{typeof(ManyPropertiesTestComponent).FullName}, {typeof(ManyPropertiesTestComponent).Assembly.GetName().Name}",
                Properties = new Dictionary<string, object>
                {
                    [$"{nameof(ManyPropertiesTestComponent.IntProperty)}"] = 17,
                    [$"{nameof(ManyPropertiesTestComponent.DoubleProperty)}"] = 1.23,
                    [$"{nameof(ManyPropertiesTestComponent.StringProperty)}"] = "value"
                }
            };

            // Act
            var actual = (ManyPropertiesTestComponent) mapper.FromDefinition(componentDefinition);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.IntProperty, Is.EqualTo(componentDefinition.Properties[nameof(ManyPropertiesTestComponent.IntProperty)]));
            Assert.That(actual.DoubleProperty, Is.EqualTo(componentDefinition.Properties[nameof(ManyPropertiesTestComponent.DoubleProperty)]));
            Assert.That(actual.StringProperty, Is.EqualTo(componentDefinition.Properties[nameof(ManyPropertiesTestComponent.StringProperty)]));
        }

        [Test]
        public void FromDefinition_ShouldThrowException_GivenAutomaticComponentDefinitionWithComponentTypeThatContainsUnsupportedPropertyType()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            // Assert
            Assert.That(
                () => mapper.FromDefinition(new AutomaticComponentDefinition
                {
                    ComponentType = $"{typeof(UnsupportedPropertyTestComponent).FullName}, {typeof(UnsupportedPropertyTestComponent).Assembly.GetName().Name}"
                }), Throws.TypeOf<GeishaEngineException>().With.Message.Contains("Component contains property of unsupported type."));
        }

        [Test]
        public void FromDefinition_ShouldReturnComponentInitializedOnlyWithPropertiesMarkedWith_PropertyDefinitionAttribute()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();
            var componentDefinition = new AutomaticComponentDefinition
            {
                ComponentType = $"{typeof(NotMarkedPropertiesTestComponent).FullName}, {typeof(NotMarkedPropertiesTestComponent).Assembly.GetName().Name}",
                Properties = new Dictionary<string, object>
                {
                    [$"{nameof(NotMarkedPropertiesTestComponent.Property1)}"] = 1,
                    [$"{nameof(NotMarkedPropertiesTestComponent.Property2)}"] = 2,
                    [$"{nameof(NotMarkedPropertiesTestComponent.Property3)}"] = 3
                }
            };

            // Act
            var actual = (NotMarkedPropertiesTestComponent) mapper.FromDefinition(componentDefinition);

            // Assert
            Assert.That(actual.Property1, Is.EqualTo(componentDefinition.Properties[nameof(NotMarkedPropertiesTestComponent.Property1)]));
            Assert.That(actual.Property2, Is.EqualTo(componentDefinition.Properties[nameof(NotMarkedPropertiesTestComponent.Property2)]));
            Assert.That(actual.Property3, Is.EqualTo(componentDefinition.Properties[nameof(NotMarkedPropertiesTestComponent.Property3)]));
            Assert.That(actual.NotMarkedProperty1, Is.EqualTo(default(int)));
            Assert.That(actual.NotMarkedProperty2, Is.EqualTo(default(int)));
            Assert.That(actual.NotMarkedProperty3, Is.EqualTo(default(int)));
        }

        #endregion

        #region Test classes

        [ComponentDefinition]
        private class EmptyTestComponent : IComponent
        {
        }

        [ComponentDefinition]
        private class IntPropertyTestComponent : IComponent
        {
            [PropertyDefinition]
            public int IntProperty { get; set; }
        }

        [ComponentDefinition]
        private class DoublePropertyTestComponent : IComponent
        {
            [PropertyDefinition]
            public double DoubleProperty { get; set; }
        }

        [ComponentDefinition]
        private class StringPropertyTestComponent : IComponent
        {
            [PropertyDefinition]
            public string StringProperty { get; set; }
        }

        [ComponentDefinition]
        private class ManyPropertiesTestComponent : IComponent
        {
            [PropertyDefinition]
            public int IntProperty { get; set; }

            [PropertyDefinition]
            public double DoubleProperty { get; set; }

            [PropertyDefinition]
            public string StringProperty { get; set; }
        }

        [ComponentDefinition]
        private class UnsupportedPropertyTestComponent : IComponent
        {
            [PropertyDefinition]
            public object UnsupportedProperty { get; set; }
        }

        [ComponentDefinition]
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