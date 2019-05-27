using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Components.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components.Serialization
{
    [TestFixture]
    public class SerializableCameraComponentMapperTests
    {
        [Test]
        public void MapToSerializable()
        {
            // Arrange
            var mapper = new SerializableCameraComponentMapper();
            var camera = new CameraComponent();

            // Act
            var actual = mapper.MapToSerializable(camera);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.TypeOf<SerializableCameraComponent>());
        }

        [Test]
        public void MapFromSerializable()
        {
            // Arrange
            var mapper = new SerializableCameraComponentMapper();
            var serializableCamera = new SerializableCameraComponent();

            // Act
            var actual = mapper.MapFromSerializable(serializableCamera);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.TypeOf<CameraComponent>());
        }
    }
}