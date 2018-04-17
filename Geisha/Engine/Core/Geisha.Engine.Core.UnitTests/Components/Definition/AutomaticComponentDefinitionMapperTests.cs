using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Definition;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Components.Definition
{
    [TestFixture]
    public class AutomaticComponentDefinitionMapperTests
    {
        [Test]
        public void IsApplicableForComponent_ShouldReturnTrueGivenComponentMarkedWith_UseAutomaticComponentDefinitionAttribute()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            // Assert
            Assert.That(mapper.IsApplicableForComponent(new EmptyAutomaticTestComponent()), Is.True);
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

        [Test]
        public void ToDefinition_ShouldReturnAutomaticComponentDefinitionWithComponentTypeFullName_GivenSomeComponent()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            var actual = (AutomaticComponentDefinition) mapper.ToDefinition(new EmptyAutomaticTestComponent());

            // Assert
            Assert.That(actual.ComponentTypeFullName, Is.EqualTo(typeof(EmptyAutomaticTestComponent).FullName));
        }

        [Test]
        public void ToDefinition_ShouldReturnEmptyAutomaticComponentDefinition_GivenEmptyComponent()
        {
            // Arrange
            var mapper = new AutomaticComponentDefinitionMapper();

            // Act
            var actual = (AutomaticComponentDefinition) mapper.ToDefinition(new EmptyAutomaticTestComponent());

            // Assert
            Assert.That(actual.Properties, Is.Empty);
        }

        #region Helpers

        [UseAutomaticComponentDefinition]
        private class EmptyAutomaticTestComponent : IComponent
        {
        }

        [UseAutomaticComponentDefinition]
        private class PropertyBasedAutomaticTestComponent : IComponent
        {
            public int IntProperty { get; set; }
            public double DoubleProperty { get; set; }
            public string StringProperty { get; set; }
        }

        #endregion
    }
}