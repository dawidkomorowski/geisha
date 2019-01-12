using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel.Serialization
{
    [TestFixture]
    public class SerializableComponentMapperAdapterTests
    {
        [Test]
        public void IsApplicableForComponent_ShouldReturnTrueGivenComponentOfTypeSameAsTypeParameter()
        {
            // Arrange
            // Act
            var mapper = new SerializableTestComponentMapper();

            // Assert
            Assert.That(mapper.IsApplicableForComponent(new TestComponent()), Is.True);
        }

        [Test]
        public void IsApplicableForComponent_ShouldReturnFalseGivenComponentOfTypeDifferentThanTypeParameter()
        {
            // Arrange
            // Act
            var mapper = new SerializableTestComponentMapper();

            // Assert
            Assert.That(mapper.IsApplicableForComponent(Substitute.For<IComponent>()), Is.False);
        }

        [Test]
        public void IsApplicableForSerializableComponent_ShouldReturnTrueGivenSerializableComponentOfTypeSameAsTypeParameter()
        {
            // Arrange
            // Act
            var mapper = new SerializableTestComponentMapper();

            // Assert
            Assert.That(mapper.IsApplicableForSerializableComponent(new SerializableTestComponent()), Is.True);
        }

        [Test]
        public void IsApplicableForSerializableComponent_ShouldReturnFalseGivenSerializableComponentOfTypeDifferentThanTypeParameter()
        {
            // Arrange
            // Act
            var mapper = new SerializableTestComponentMapper();

            // Assert
            Assert.That(mapper.IsApplicableForSerializableComponent(Substitute.For<ISerializableComponent>()), Is.False);
        }

        [Test]
        public void MapToSerializable_ShouldPassArgumentToInternalMapToSerializable()
        {
            // Arrange
            var mapper = new SerializableTestComponentMapper();
            var component = new TestComponent();

            // Act
            mapper.MapToSerializable(component);

            // Assert
            Assert.That(mapper.MapToSerializableInput, Is.EqualTo(component));
        }

        [Test]
        public void MapToSerializable_ShouldReturnValueReturnedFromInternalMapToSerializable()
        {
            // Arrange
            var mapper = new SerializableTestComponentMapper();
            var component = new TestComponent();
            var serializableComponent = new SerializableTestComponent();
            mapper.MapToSerializableOutput = serializableComponent;

            // Act
            var actual = mapper.MapToSerializable(component);

            // Assert
            Assert.That(actual, Is.EqualTo(serializableComponent));
        }

        [Test]
        public void MapFromSerializable_ShouldPassArgumentToInternalMapFromSerializable()
        {
            // Arrange
            var mapper = new SerializableTestComponentMapper();
            var serializableComponent = new SerializableTestComponent();

            // Act
            mapper.MapFromSerializable(serializableComponent);

            // Assert
            Assert.That(mapper.MapFromSerializableInput, Is.EqualTo(serializableComponent));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnValueReturnedFromInternalMapFromSerializable()
        {
            // Arrange
            var mapper = new SerializableTestComponentMapper();
            var component = new TestComponent();
            var serializableComponent = new SerializableTestComponent();
            mapper.MapFromSerializableOutput = component;

            // Act
            var actual = mapper.MapFromSerializable(serializableComponent);

            // Assert
            Assert.That(actual, Is.EqualTo(component));
        }

        #region Helpers

        private class TestComponent : IComponent
        {
        }

        private class SerializableTestComponent : ISerializableComponent
        {
        }

        private class SerializableTestComponentMapper : SerializableComponentMapperAdapter<TestComponent, SerializableTestComponent>
        {
            public TestComponent MapToSerializableInput { get; set; }
            public SerializableTestComponent MapToSerializableOutput { get; set; }
            public SerializableTestComponent MapFromSerializableInput { get; set; }
            public TestComponent MapFromSerializableOutput { get; set; }

            protected override SerializableTestComponent MapToSerializable(TestComponent component)
            {
                MapToSerializableInput = component;
                return MapToSerializableOutput;
            }

            protected override TestComponent MapFromSerializable(SerializableTestComponent serializableComponent)
            {
                MapFromSerializableInput = serializableComponent;
                return MapFromSerializableOutput;
            }
        }

        #endregion
    }
}