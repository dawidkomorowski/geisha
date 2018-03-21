using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Definition;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel.Definition
{
    [TestFixture]
    public class ComponentDefinitionMapperAdapterTests
    {
        [Test]
        public void ComponentType_ShouldReturnComponentTypeGivenAsTypeParameter()
        {
            // Arrange
            // Act
            var mapper = new TestComponentDefinitionMapper();

            // Assert
            Assert.That(mapper.ComponentType, Is.EqualTo(typeof(TestComponent)));
        }

        [Test]
        public void ComponentDefinitionType_ShouldReturnComponentDefinitionTypeGivenAsTypeParameter()
        {
            // Arrange
            // Act
            var mapper = new TestComponentDefinitionMapper();

            // Assert
            Assert.That(mapper.ComponentDefinitionType, Is.EqualTo(typeof(TestComponentDefinition)));
        }

        [Test]
        public void ToDefinition_ShouldPassArgumentToInternalToDefinition()
        {
            // Arrange
            var mapper = new TestComponentDefinitionMapper();
            var component = new TestComponent();

            // Act
            mapper.ToDefinition(component);

            // Assert
            Assert.That(mapper.ToDefinitionInput, Is.EqualTo(component));
        }

        [Test]
        public void ToDefinition_ShouldReturnValueReturnedFromInternalToDefinition()
        {
            // Arrange
            var mapper = new TestComponentDefinitionMapper();
            var component = new TestComponent();
            var componentDefinition = new TestComponentDefinition();
            mapper.ToDefinitionOutput = componentDefinition;

            // Act
            var actual = mapper.ToDefinition(component);

            // Assert
            Assert.That(actual, Is.EqualTo(componentDefinition));
        }

        [Test]
        public void FromDefinition_ShouldPassArgumentToInternalFromDefinition()
        {
            // Arrange
            var mapper = new TestComponentDefinitionMapper();
            var componentDefinition = new TestComponentDefinition();

            // Act
            mapper.FromDefinition(componentDefinition);

            // Assert
            Assert.That(mapper.FromDefinitionInput, Is.EqualTo(componentDefinition));
        }

        [Test]
        public void FromDefinition_ShouldReturnValueReturnedFromInternalFromDefinition()
        {
            // Arrange
            var mapper = new TestComponentDefinitionMapper();
            var component = new TestComponent();
            var componentDefinition = new TestComponentDefinition();
            mapper.FromDefinitionOutput = component;

            // Act
            var actual = mapper.FromDefinition(componentDefinition);

            // Assert
            Assert.That(actual, Is.EqualTo(component));
        }

        #region Helpers

        private class TestComponent : IComponent
        {
        }

        private class TestComponentDefinition : IComponentDefinition
        {
        }

        private class TestComponentDefinitionMapper : ComponentDefinitionMapperAdapter<TestComponent, TestComponentDefinition>
        {
            public TestComponent ToDefinitionInput { get; set; }
            public TestComponentDefinition ToDefinitionOutput { get; set; }
            public TestComponentDefinition FromDefinitionInput { get; set; }
            public TestComponent FromDefinitionOutput { get; set; }

            protected override TestComponentDefinition ToDefinition(TestComponent component)
            {
                ToDefinitionInput = component;
                return ToDefinitionOutput;
            }

            protected override TestComponent FromDefinition(TestComponentDefinition componentDefinition)
            {
                FromDefinitionInput = componentDefinition;
                return FromDefinitionOutput;
            }
        }

        #endregion
    }
}