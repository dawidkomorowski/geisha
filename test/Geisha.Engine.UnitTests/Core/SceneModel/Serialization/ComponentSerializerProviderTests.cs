using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel.Serialization
{
    [TestFixture]
    public class ComponentSerializerProviderTests
    {
        [Test]
        public void Constructor_ThrowsArgumentException_GivenSerializersWithDuplicatedComponentId()
        {
            // Arrange
            var serializer1 = CreateSerializer("Component 1");
            var serializer2 = CreateSerializer("Component 1");
            var serializer3 = CreateSerializer("Component 2");

            // Act
            // Assert
            Assert.That(() => new ComponentSerializerProvider(new[] {serializer1, serializer2, serializer3}),
                Throws.ArgumentException.With.Message.Contains("Component 1"));
        }

        [Test]
        public void Get_ThrowsException_GivenComponentIdForWhichThereIsNoSerializerAvailable()
        {
            // Arrange
            var serializer = CreateSerializer("Component 1");

            var serializerProvider = new ComponentSerializerProvider(new[] {serializer});

            // Act
            // Assert
            Assert.That(() => serializerProvider.Get(new ComponentId("Component 2")),
                Throws.TypeOf<ComponentSerializerNotFoundException>().With.Message.Contains("Component 2").And.With.Message.Contains("Component 1"));
        }

        [Test]
        public void Get_ShouldReturnSerializer_GivenComponentId()
        {
            // Arrange
            var serializer1 = CreateSerializer("Component 1");
            var serializer2 = CreateSerializer("Component 2");
            var serializer3 = CreateSerializer("Component 3");

            var serializerProvider = new ComponentSerializerProvider(new[] {serializer1, serializer2, serializer3});

            // Act
            var actual = serializerProvider.Get(new ComponentId("Component 2"));

            // Assert
            Assert.That(actual, Is.EqualTo(serializer2));
        }

        #region Helpers

        private static IComponentSerializer CreateSerializer(string componentId)
        {
            var serializer = Substitute.For<IComponentSerializer>();
            serializer.ComponentId.Returns(new ComponentId(componentId));
            return serializer;
        }

        #endregion
    }
}