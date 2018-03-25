using Geisha.Common.Math;
using Geisha.Common.Math.Definition;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Math.Definition
{
    [TestFixture]
    public class Vector2DefinitionTests
    {
        [Test]
        public void ToVector2()
        {
            // Arrange
            var definition = new Vector2Definition
            {
                X = 1,
                Y = 2
            };

            // Act
            var actual = Vector2Definition.ToVector2(definition);

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
            var actual = Vector2Definition.FromVector2(vector);

            // Assert
            Assert.That(actual.X, Is.EqualTo(1));
            Assert.That(actual.Y, Is.EqualTo(2));
        }
    }
}