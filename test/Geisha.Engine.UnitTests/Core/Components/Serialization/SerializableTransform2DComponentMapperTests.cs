using Geisha.Common.Math;
using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Components.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Components.Serialization
{
    [TestFixture]
    public class SerializableTransform2DComponentMapperTests
    {
        [Test]
        public void MapToSerializable()
        {
            // Arrange
            var mapper = new SerializableTransform2DComponentMapper();
            var transform = new Transform2DComponent
            {
                Translation = new Vector2(1.23, 2.34),
                Rotation = 3.45,
                Scale = new Vector2(4.56, 5.67)
            };

            // Act
            var actual = (SerializableTransform2DComponent) mapper.MapToSerializable(transform);

            // Assert
            Assert.That(actual.Translation.X, Is.EqualTo(1.23));
            Assert.That(actual.Translation.Y, Is.EqualTo(2.34));

            Assert.That(actual.Rotation, Is.EqualTo(3.45));

            Assert.That(actual.Scale.X, Is.EqualTo(4.56));
            Assert.That(actual.Scale.Y, Is.EqualTo(5.67));
        }

        [Test]
        public void MapFromSerializable()
        {
            // Arrange
            var mapper = new SerializableTransform2DComponentMapper();
            var transform = new SerializableTransform2DComponent
            {
                Translation = new SerializableVector2
                {
                    X = 1.23,
                    Y = 2.34
                },
                Rotation = 3.45,
                Scale = new SerializableVector2
                {
                    X = 4.56,
                    Y = 5.67
                }
            };

            // Act
            var actual = (Transform2DComponent) mapper.MapFromSerializable(transform);

            // Assert
            Assert.That(actual.Translation.X, Is.EqualTo(1.23));
            Assert.That(actual.Translation.Y, Is.EqualTo(2.34));

            Assert.That(actual.Rotation, Is.EqualTo(3.45));

            Assert.That(actual.Scale.X, Is.EqualTo(4.56));
            Assert.That(actual.Scale.Y, Is.EqualTo(5.67));
        }
    }
}