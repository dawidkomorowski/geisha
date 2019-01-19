using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.Components.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.Physics.UnitTests.Components.Serialization
{
    [TestFixture]
    public class SerializableCircleColliderComponentMapperTests
    {
        [Test]
        public void MapToSerializable()
        {
            // Arrange
            var mapper = new SerializableCircleColliderComponentMapper();
            var collider = new CircleColliderComponent
            {
                Radius = 123.456
            };

            // Act
            var actual = (SerializableCircleColliderComponent) mapper.MapToSerializable(collider);

            // Assert
            Assert.That(actual.Radius, Is.EqualTo(collider.Radius));
        }

        [Test]
        public void MapFromSerializable()
        {
            // Arrange
            var mapper = new SerializableCircleColliderComponentMapper();
            var serializableCollider = new SerializableCircleColliderComponent
            {
                Radius = 123.456
            };

            // Act
            var actual = (CircleColliderComponent) mapper.MapFromSerializable(serializableCollider);

            // Assert
            Assert.That(actual.Radius, Is.EqualTo(serializableCollider.Radius));
            Assert.That(actual.IsColliding, Is.False);
            Assert.That(actual.CollidingEntities, Is.Empty);
        }
    }
}