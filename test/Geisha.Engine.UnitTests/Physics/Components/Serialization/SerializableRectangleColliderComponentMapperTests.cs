using Geisha.Common.Math;
using Geisha.Common.Math.Serialization;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.Components.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Components.Serialization
{
    [TestFixture]
    public class SerializableRectangleColliderComponentMapperTests
    {
        [Test]
        public void MapToSerializable()
        {
            // Arrange
            var mapper = new SerializableRectangleColliderComponentMapper();
            var collider = new RectangleColliderComponent
            {
                Dimension = new Vector2(12.34, 56.78)
            };

            // Act
            var actual = (SerializableRectangleColliderComponent) mapper.MapToSerializable(collider);

            // Assert
            Assert.That(actual.Dimension.X, Is.EqualTo(collider.Dimension.X));
            Assert.That(actual.Dimension.Y, Is.EqualTo(collider.Dimension.Y));
        }

        [Test]
        public void MapFromSerializable()
        {
            // Arrange
            var mapper = new SerializableRectangleColliderComponentMapper();
            var serializableCollider = new SerializableRectangleColliderComponent
            {
                Dimension = new SerializableVector2
                {
                    X = 12.34,
                    Y = 56.78
                }
            };

            // Act
            var actual = (RectangleColliderComponent) mapper.MapFromSerializable(serializableCollider);

            // Assert
            Assert.That(actual.Dimension.X, Is.EqualTo(serializableCollider.Dimension.X));
            Assert.That(actual.Dimension.Y, Is.EqualTo(serializableCollider.Dimension.Y));
            Assert.That(actual.IsColliding, Is.False);
            Assert.That(actual.CollidingEntities, Is.Empty);
        }
    }
}