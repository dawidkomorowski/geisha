using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Math.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math.Serialization
{
    [TestFixture]
    public class SerializableVector2Tests
    {
        [Test]
        public void ToVector2()
        {
            // Arrange
            var serializable = new SerializableVector2
            {
                X = 1,
                Y = 2
            };

            // Act
            var actual = SerializableVector2.ToVector2(serializable);

            // Assert
            Assert.That(actual.X, Is.EqualTo(1));
            Assert.That(actual.Y, Is.EqualTo(2));
        }

        [Test]
        public void FromVector2()
        {
            // Arrange
            var vector = new Vector2(1, 2);

            // Act
            var actual = SerializableVector2.FromVector2(vector);

            // Assert
            Assert.That(actual.X, Is.EqualTo(1));
            Assert.That(actual.Y, Is.EqualTo(2));
        }
    }
}