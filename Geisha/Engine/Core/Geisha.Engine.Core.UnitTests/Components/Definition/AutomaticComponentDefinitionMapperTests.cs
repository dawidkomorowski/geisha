using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Definition;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Components.Definition
{
    [TestFixture]
    public class AutomaticComponentDefinitionMapperTests
    {
        #region IsApplicableFor

        [Test]
        public void IsApplicableForComponent_ShouldReturnTrueGivenComponentMarkedWith_UseAutomaticComponentDefinitionAttribute()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            // Assert
            Assert.That(mapper.IsApplicableForComponent(new EmptyTestComponent()), Is.True);
        }

        [Test]
        public void IsApplicableForComponent_ShouldReturnFalseGivenComponentNotMarkedWith_UseAutomaticComponentDefinitionAttribute()
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
        public void ToDefinition_ShouldReturnAutomaticComponentDefinitionWithComponentTypeFullName_GivenSomeComponent()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            var actual = (AutomaticComponentDefinition) mapper.ToDefinition(new EmptyTestComponent());

            // Assert
            Assert.That(actual.ComponentTypeFullName, Is.EqualTo(typeof(EmptyTestComponent).FullName));
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
            Assert.That(() => mapper.ToDefinition(new UnsupportedPropertyTestComponent()), Throws.TypeOf<GeishaEngineException>());
        }

        #endregion

        #region Helpers

        [UseAutomaticComponentDefinition]
        private class EmptyTestComponent : IComponent
        {
        }

        [UseAutomaticComponentDefinition]
        private class IntPropertyTestComponent : IComponent
        {
            public int IntProperty { get; set; }
        }

        [UseAutomaticComponentDefinition]
        private class DoublePropertyTestComponent : IComponent
        {
            public double DoubleProperty { get; set; }
        }

        [UseAutomaticComponentDefinition]
        private class StringPropertyTestComponent : IComponent
        {
            public string StringProperty { get; set; }
        }

        [UseAutomaticComponentDefinition]
        private class ManyPropertiesTestComponent : IComponent
        {
            public int IntProperty { get; set; }
            public double DoubleProperty { get; set; }
            public string StringProperty { get; set; }
        }

        [UseAutomaticComponentDefinition]
        private class UnsupportedPropertyTestComponent : IComponent
        {
            public object UnsupportedProperty { get; set; }
        }

        #endregion
    }
}