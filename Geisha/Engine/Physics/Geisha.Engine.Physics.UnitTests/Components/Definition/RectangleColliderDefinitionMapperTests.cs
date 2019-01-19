﻿using Geisha.Common.Math;
using Geisha.Common.Math.Serialization;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.Components.Definition;
using NUnit.Framework;

namespace Geisha.Engine.Physics.UnitTests.Components.Definition
{
    [TestFixture]
    public class RectangleColliderDefinitionMapperTests
    {
        [Test]
        public void ToDefinition()
        {
            // Arrange
            var mapper = new RectangleColliderDefinitionMapper();
            var collider = new RectangleColliderComponent
            {
                Dimension = new Vector2(12.34, 56.78)
            };

            // Act
            var actual = (RectangleColliderDefinition) mapper.MapToSerializable(collider);

            // Assert
            Assert.That(actual.Dimension.X, Is.EqualTo(collider.Dimension.X));
            Assert.That(actual.Dimension.Y, Is.EqualTo(collider.Dimension.Y));
        }

        [Test]
        public void FromDefinition()
        {
            // Arrange
            var mapper = new RectangleColliderDefinitionMapper();
            var colliderDefinition = new RectangleColliderDefinition
            {
                Dimension = new SerializableVector2
                {
                    X = 12.34,
                    Y = 56.78
                }
            };

            // Act
            var actual = (RectangleColliderComponent) mapper.MapFromSerializable(colliderDefinition);

            // Assert
            Assert.That(actual.Dimension.X, Is.EqualTo(colliderDefinition.Dimension.X));
            Assert.That(actual.Dimension.Y, Is.EqualTo(colliderDefinition.Dimension.Y));
            Assert.That(actual.IsColliding, Is.False);
            Assert.That(actual.CollidingEntities, Is.Empty);
        }
    }
}