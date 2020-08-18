﻿using Geisha.Common.Math;
using Geisha.Common.Math.Serialization;
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
            var camera = new CameraComponent {ViewRectangle = new Vector2(123, 456)};

            // Act
            var actual = mapper.MapToSerializable(camera);

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.TypeOf<SerializableCameraComponent>());
            var serializableCamera = (SerializableCameraComponent) actual;
            Assert.That(serializableCamera.ViewRectangle.X, Is.EqualTo(123));
            Assert.That(serializableCamera.ViewRectangle.Y, Is.EqualTo(456));
        }

        [Test]
        public void MapFromSerializable()
        {
            // Arrange
            var mapper = new SerializableCameraComponentMapper();
            var serializableCamera = new SerializableCameraComponent
            {
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
            Assert.That(camera.ViewRectangle.X, Is.EqualTo(123));
            Assert.That(camera.ViewRectangle.Y, Is.EqualTo(456));
        }
    }
}