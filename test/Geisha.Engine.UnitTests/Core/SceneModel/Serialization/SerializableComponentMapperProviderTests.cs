using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel.Serialization
{
    [TestFixture]
    public class SerializableComponentMapperProviderTests
    {
        #region GetMapperForComponent

        [Test]
        public void GetMapperForComponent_ShouldReturnMapper_WhenThereIsSingleMatching()
        {
            // Arrange
            var component = new TestComponent();

            var mapper1 = Substitute.For<ISerializableComponentMapper>();
            var mapper2 = Substitute.For<ISerializableComponentMapper>();
            var mapper3 = Substitute.For<ISerializableComponentMapper>();

            mapper2.IsApplicableForComponent(component).Returns(true);

            var provider = new SerializableComponentMapperProvider(new[] {mapper1, mapper2, mapper3});

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

            var mapper1 = Substitute.For<ISerializableComponentMapper>();
            var mapper2 = Substitute.For<ISerializableComponentMapper>();
            var mapper3 = Substitute.For<ISerializableComponentMapper>();

            var provider = new SerializableComponentMapperProvider(new[] {mapper1, mapper2, mapper3});

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

            var mapper1 = Substitute.For<ISerializableComponentMapper>();
            var mapper2 = Substitute.For<ISerializableComponentMapper>();
            var mapper3 = Substitute.For<ISerializableComponentMapper>();

            mapper1.IsApplicableForComponent(component).Returns(true);
            mapper2.IsApplicableForComponent(component).Returns(true);

            var provider = new SerializableComponentMapperProvider(new[] {mapper1, mapper2, mapper3});

            // Act
            // Assert
            Assert.That(() => provider.GetMapperFor(component),
                Throws.TypeOf<GeishaEngineException>().With.Message.Contain("Multiple mappers found for component type"));
        }

        #endregion

        #region GetMapperForSerializableComponent

        [Test]
        public void GetMapperForSerializableComponent_ShouldReturnMapper_WhenThereIsSingleMatching()
        {
            // Arrange
            var serializableComponent = new SerializableTestComponent();

            var mapper1 = Substitute.For<ISerializableComponentMapper>();
            var mapper2 = Substitute.For<ISerializableComponentMapper>();
            var mapper3 = Substitute.For<ISerializableComponentMapper>();

            mapper2.IsApplicableForSerializableComponent(serializableComponent).Returns(true);

            var provider = new SerializableComponentMapperProvider(new[] {mapper1, mapper2, mapper3});

            // Act
            var actual = provider.GetMapperFor(serializableComponent);

            // Assert
            Assert.That(actual, Is.EqualTo(mapper2));
        }

        [Test]
        public void GetMapperForSerializableComponent_ShouldThrowException_WhenThereIsNoMatching()
        {
            // Arrange
            var serializableComponent = new SerializableTestComponent();

            var mapper1 = Substitute.For<ISerializableComponentMapper>();
            var mapper2 = Substitute.For<ISerializableComponentMapper>();
            var mapper3 = Substitute.For<ISerializableComponentMapper>();

            var provider = new SerializableComponentMapperProvider(new[] {mapper1, mapper2, mapper3});

            // Act
            // Assert
            Assert.That(() => provider.GetMapperFor(serializableComponent),
                Throws.TypeOf<GeishaEngineException>().With.Message.Contain("No mapper found for serializable component type"));
        }

        [Test]
        public void GetMapperForSerializableComponent_ShouldThrowException_WhenThereAreMultipleMatching()
        {
            // Arrange
            var serializableComponent = new SerializableTestComponent();

            var mapper1 = Substitute.For<ISerializableComponentMapper>();
            var mapper2 = Substitute.For<ISerializableComponentMapper>();
            var mapper3 = Substitute.For<ISerializableComponentMapper>();

            mapper1.IsApplicableForSerializableComponent(serializableComponent).Returns(true);
            mapper2.IsApplicableForSerializableComponent(serializableComponent).Returns(true);

            var provider = new SerializableComponentMapperProvider(new[] {mapper1, mapper2, mapper3});

            // Act
            // Assert
            Assert.That(() => provider.GetMapperFor(serializableComponent),
                Throws.TypeOf<GeishaEngineException>().With.Message.Contain("Multiple mappers found for serializable component type"));
        }

        #endregion

        #region Helpers

        private class TestComponent : IComponent
        {
        }

        private class SerializableTestComponent : ISerializableComponent
        {
        }

        #endregion
    }
}