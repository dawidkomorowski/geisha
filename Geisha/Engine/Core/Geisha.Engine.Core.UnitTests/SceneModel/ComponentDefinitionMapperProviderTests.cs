using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel
{
    [TestFixture]
    public class ComponentDefinitionMapperProviderTests
    {
        #region GetMapperForComponent

        [Test]
        public void GetMapperForComponent_ShouldReturnMapper_WhenThereIsSingleMatching()
        {
            // Arrange
            var component = new TestComponent();

            var mapper1 = Substitute.For<IComponentDefinitionMapper>();
            var mapper2 = Substitute.For<IComponentDefinitionMapper>();
            var mapper3 = Substitute.For<IComponentDefinitionMapper>();

            mapper2.ComponentType.Returns(component.GetType());

            var provider = new ComponentDefinitionMapperProvider(new[] {mapper1, mapper2, mapper3});

            // Act
            var actual = provider.GetMapperFor(component);

            // Assert
            Assert.That(actual, Is.EqualTo(mapper2));
        }

        [Test]
        public void GetMapperForComponent_ShouldThrowException_WhenThereIsNoMatching()
        {
            // Arrange
            var component = new TestComponent();

            var mapper1 = Substitute.For<IComponentDefinitionMapper>();
            var mapper2 = Substitute.For<IComponentDefinitionMapper>();
            var mapper3 = Substitute.For<IComponentDefinitionMapper>();

            var provider = new ComponentDefinitionMapperProvider(new[] {mapper1, mapper2, mapper3});

            // Act
            // Assert
            Assert.That(() => provider.GetMapperFor(component), Throws.TypeOf<GeishaEngineException>());
        }

        [Test]
        public void GetMapperForComponent_ShouldThrowException_WhenThereAreMultipleMatching()
        {
            // Arrange
            var component = new TestComponent();

            var mapper1 = Substitute.For<IComponentDefinitionMapper>();
            var mapper2 = Substitute.For<IComponentDefinitionMapper>();
            var mapper3 = Substitute.For<IComponentDefinitionMapper>();

            mapper1.ComponentType.Returns(component.GetType());
            mapper2.ComponentType.Returns(component.GetType());

            var provider = new ComponentDefinitionMapperProvider(new[] {mapper1, mapper2, mapper3});

            // Act
            // Assert
            Assert.That(() => provider.GetMapperFor(component), Throws.TypeOf<GeishaEngineException>());
        }

        #endregion

        #region GetMapperForComponentDefinition

        [Test]
        public void GetMapperForComponentDefinition_ShouldReturnMapper_WhenThereIsSingleMatching()
        {
            // Arrange
            var componentDefinition = new TestComponentDefinition();

            var mapper1 = Substitute.For<IComponentDefinitionMapper>();
            var mapper2 = Substitute.For<IComponentDefinitionMapper>();
            var mapper3 = Substitute.For<IComponentDefinitionMapper>();

            mapper2.ComponentDefinitionType.Returns(componentDefinition.GetType());

            var provider = new ComponentDefinitionMapperProvider(new[] {mapper1, mapper2, mapper3});

            // Act
            var actual = provider.GetMapperFor(componentDefinition);

            // Assert
            Assert.That(actual, Is.EqualTo(mapper2));
        }

        [Test]
        public void GetMapperForComponentDefinition_ShouldThrowException_WhenThereIsNoMatching()
        {
            // Arrange
            var componentDefinition = new TestComponentDefinition();

            var mapper1 = Substitute.For<IComponentDefinitionMapper>();
            var mapper2 = Substitute.For<IComponentDefinitionMapper>();
            var mapper3 = Substitute.For<IComponentDefinitionMapper>();

            var provider = new ComponentDefinitionMapperProvider(new[] {mapper1, mapper2, mapper3});

            // Act
            // Assert
            Assert.That(() => provider.GetMapperFor(componentDefinition), Throws.TypeOf<GeishaEngineException>());
        }

        [Test]
        public void GetMapperForComponentDefinition_ShouldThrowException_WhenThereAreMultipleMatching()
        {
            // Arrange
            var componentDefinition = new TestComponentDefinition();

            var mapper1 = Substitute.For<IComponentDefinitionMapper>();
            var mapper2 = Substitute.For<IComponentDefinitionMapper>();
            var mapper3 = Substitute.For<IComponentDefinitionMapper>();

            mapper1.ComponentDefinitionType.Returns(componentDefinition.GetType());
            mapper2.ComponentDefinitionType.Returns(componentDefinition.GetType());

            var provider = new ComponentDefinitionMapperProvider(new[] {mapper1, mapper2, mapper3});

            // Act
            // Assert
            Assert.That(() => provider.GetMapperFor(componentDefinition), Throws.TypeOf<GeishaEngineException>());
        }

        #endregion

        #region Helpers

        private class TestComponent : IComponent
        {
        }

        private class TestComponentDefinition : IComponentDefinition
        {
        }

        #endregion
    }
}