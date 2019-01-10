using Geisha.Common.Math;
using Geisha.Common.Math.Serialization;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Math.Serialization
{
    [TestFixture]
    public class SerializableVector3Tests
    {
        [Test]
        public void ToVector3()
        {
            // Arrange
            var serializable = new SerializableVector3
            {
                X = 1,
                Y = 2,
                Z = 3
            };

            // Act
            var actual = SerializableVector3.ToVector3(serializable);

            // Assert
            Assert.That(actual.X, Is.EqualTo(1));
            Assert.That(actual.Y, Is.EqualTo(2));
            Assert.That(actual.Z, Is.EqualTo(3));
        }

        [Test]
        public void FromVector3()
        {
            // Arrange
            var vector = new Vector3(1, 2, 3);

            // Act
            var actual = SerializableVector3.FromVector3(vector);

            // Assert
            Assert.That(actual.X, Is.EqualTo(1));
            Assert.That(actual.Y, Is.EqualTo(2));
            Assert.That(actual.Z, Is.EqualTo(3));
        }
    }
}