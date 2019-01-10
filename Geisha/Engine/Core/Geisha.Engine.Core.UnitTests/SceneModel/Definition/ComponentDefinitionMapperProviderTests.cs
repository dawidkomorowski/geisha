using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel.Definition
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

            mapper2.IsApplicableForComponent(component).Returns(true);

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
            Assert.That(() => provider.GetMapperFor(component),
                Throws.TypeOf<GeishaEngineException>().With.Message.Contain("No mapper found for component type"));
        }

        [Test]
        public void GetMapperForComponent_ShouldThrowException_WhenThereAreMultipleMatching()
        {
            // Arrange
            var component = new TestComponent();

            var mapper1 = Substitute.For<IComponentDefinitionMapper>();
            var mapper2 = Substitute.For<IComponentDefinitionMapper>();
            var mapper3 = Substitute.For<IComponentDefinitionMapper>();

            mapper1.IsApplicableForComponent(component).Returns(true);
            mapper2.IsApplicableForComponent(component).Returns(true);

            var provider = new ComponentDefinitionMapperProvider(new[] {mapper1, mapper2, mapper3});

            // Act
            // Assert
            Assert.That(() => provider.GetMapperFor(component),
                Throws.TypeOf<GeishaEngineException>().With.Message.Contain("Multiple mappers found for component type"));
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

            mapper2.IsApplicableForComponentDefinition(componentDefinition).Returns(true);

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
            Assert.That(() => provider.GetMapperFor(componentDefinition),
                Throws.TypeOf<GeishaEngineException>().With.Message.Contain("No mapper found for component definition type"));
        }

        [Test]
        public void GetMapperForComponentDefinition_ShouldThrowException_WhenThereAreMultipleMatching()
        {
            // Arrange
            var componentDefinition = new TestComponentDefinition();

            var mapper1 = Substitute.For<IComponentDefinitionMapper>();
            var mapper2 = Substitute.For<IComponentDefinitionMapper>();
            var mapper3 = Substitute.For<IComponentDefinitionMapper>();

            mapper1.IsApplicableForComponentDefinition(componentDefinition).Returns(true);
            mapper2.IsApplicableForComponentDefinition(componentDefinition).Returns(true);

            var provider = new ComponentDefinitionMapperProvider(new[] {mapper1, mapper2, mapper3});

            // Act
            // Assert
            Assert.That(() => provider.GetMapperFor(componentDefinition),
                Throws.TypeOf<GeishaEngineException>().With.Message.Contain("Multiple mappers found for component definition type"));
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