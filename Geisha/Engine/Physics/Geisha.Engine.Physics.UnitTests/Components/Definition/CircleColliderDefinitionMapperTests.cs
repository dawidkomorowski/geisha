using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.Components.Definition;
using NUnit.Framework;

namespace Geisha.Engine.Physics.UnitTests.Components.Definition
{
    [TestFixture]
    public class CircleColliderDefinitionMapperTests
    {
        [Test]
        public void ToDefinition()
        {
            // Arrange
            var mapper = new CircleColliderDefinitionMapper();
            var collider = new CircleCollider
            {
                Radius = 123.456
            };

            // Act
            var actual = (CircleColliderDefinition) mapper.MapToSerializable(collider);

            // Assert
            Assert.That(actual.Radius, Is.EqualTo(collider.Radius));
        }

        [Test]
        public void FromDefinition()
        {
            // Arrange
            var mapper = new CircleColliderDefinitionMapper();
            var colliderDefinition = new CircleColliderDefinition
            {
                Radius = 123.456
            };

            // Act
            var actual = (CircleCollider) mapper.MapFromSerializable(colliderDefinition);

            // Assert
            Assert.That(actual.Radius, Is.EqualTo(colliderDefinition.Radius));
            Assert.That(actual.IsColliding, Is.False);
            Assert.That(actual.CollidingEntities, Is.Empty);
        }
    }
}