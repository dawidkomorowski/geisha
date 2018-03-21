using Geisha.Common.Math;
using Geisha.Common.Math.Definition;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Math.Definition
{
    [TestFixture]
    public class Vector3DefinitionTests
    {
        [Test]
        public void ToVector3()
        {
            // Arrange
            var definition = new Vector3Definition
            {
                X = 1,
                Y = 2,
                Z = 3
            };

            // Act
            var actual = Vector3Definition.ToVector3(definition);

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
            var actual = Vector3Definition.FromVector3(vector);

            // Assert
            Assert.That(actual.X, Is.EqualTo(1));
            Assert.That(actual.Y, Is.EqualTo(2));
            Assert.That(actual.Z, Is.EqualTo(3));
        }
    }
}