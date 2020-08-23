using Geisha.Common.Math;
using Geisha.Common.Math.Serialization;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Components.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components.Serialization
{
    [TestFixture]
    public class SerializableCameraComponentMapperTests
    {
        [TestCase(AspectRatioBehavior.Overscan, "Overscan")]
        [TestCase(AspectRatioBehavior.Underscan, "Underscan")]
        public void MapToSerializable(AspectRatioBehavior givenAspectRatioBehavior, string expectedAspectRatioBehavior)
        {
            // Arrange
            var mapper = new SerializableCameraComponentMapper();
            var camera = new CameraComponent
            {
                AspectRatioBehavior = givenAspectRatioBehavior,
                ViewRectangle = new Vector2(123, 456)
            };

            // Act
            var actual = mapper.MapToSerializable(camera);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.TypeOf<SerializableCameraComponent>());
            var serializableCamera = (SerializableCameraComponent) actual;
            Assert.That(serializableCamera.AspectRatioBehavior, Is.EqualTo(expectedAspectRatioBehavior));
            Assert.That(serializableCamera.ViewRectangle.X, Is.EqualTo(123));
            Assert.That(serializableCamera.ViewRectangle.Y, Is.EqualTo(456));
        }

        [TestCase("Overscan", AspectRatioBehavior.Overscan)]
        [TestCase("Underscan", AspectRatioBehavior.Underscan)]
        public void MapFromSerializable(string givenAspectRatioBehavior, AspectRatioBehavior expectedAspectRatioBehavior)
        {
            // Arrange
            var mapper = new SerializableCameraComponentMapper();
            var serializableCamera = new SerializableCameraComponent
            {
                AspectRatioBehavior = givenAspectRatioBehavior,
                ViewRectangle = new SerializableVector2
                {
                    X = 123,
                    Y = 456
                }
            };

            // Act
            var actual = mapper.MapFromSerializable(serializableCamera);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.TypeOf<CameraComponent>());
            var camera = (CameraComponent) actual;
            Assert.That(camera.AspectRatioBehavior, Is.EqualTo(expectedAspectRatioBehavior));
            Assert.That(camera.ViewRectangle.X, Is.EqualTo(123));
            Assert.That(camera.ViewRectangle.Y, Is.EqualTo(456));
        }

        [TestCase(null)]
        [TestCase("NotExistentValue")]
        public void MapFromSerializable_ShouldThrowArgumentException_GivenInvalidAspectRatioBehavior(string aspectRatioBehavior)
        {
            // Arrange
            var mapper = new SerializableCameraComponentMapper();
            var serializableCamera = new SerializableCameraComponent
            {
                AspectRatioBehavior = aspectRatioBehavior,
                ViewRectangle = new SerializableVector2
                {
                    X = 123,
                    Y = 456
                }
            };

            // Act
            // Assert
            Assert.That(() => mapper.MapFromSerializable(serializableCamera), Throws.ArgumentException);
        }
    }
}