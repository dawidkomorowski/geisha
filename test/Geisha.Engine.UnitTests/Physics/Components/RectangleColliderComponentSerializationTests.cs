using Geisha.Common.Math;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Components
{
    [TestFixture]
    public class RectangleColliderComponentSerializationTests : ComponentSerializationTestsBase
    {
        [Test]
        public void SerializeAndDeserialize()
        {
            // Arrange
            var dimension = new Vector2(12.34, 56.78);

            // Act
            var actual = SerializeAndDeserialize<RectangleColliderComponent>(component => { component.Dimension = dimension; });

            // Assert
            Assert.That(actual.Dimension, Is.EqualTo(dimension));
            Assert.That(actual.IsColliding, Is.False);
            Assert.That(actual.CollidingEntities, Is.Empty);
        }
    }
}