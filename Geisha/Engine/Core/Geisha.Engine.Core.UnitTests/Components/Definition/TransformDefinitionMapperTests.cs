using Geisha.Common.Math;
using Geisha.Common.Math.Definition;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Components.Definition;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Components.Definition
{
    [TestFixture]
    public class TransformDefinitionMapperTests
    {
        [Test]
        public void ToDefinition()
        {
            // Arrange
            var mapper = new TransformDefinitionMapper();
            var transform = new Transform
            {
                Translation = new Vector3(1.23, 2.34, 3.45),
                Rotation = new Vector3(4.56, 5.67, 6.78),
                Scale = new Vector3(7.89, 8.90, 9.00)
            };

            // Act
            var actual = (TransformDefinition) mapper.ToDefinition(transform);

            // Assert
            Assert.That(actual.Translation.X, Is.EqualTo(1.23));
            Assert.That(actual.Translation.Y, Is.EqualTo(2.34));
            Assert.That(actual.Translation.Z, Is.EqualTo(3.45));

            Assert.That(actual.Rotation.X, Is.EqualTo(4.56));
            Assert.That(actual.Rotation.Y, Is.EqualTo(5.67));
            Assert.That(actual.Rotation.Z, Is.EqualTo(6.78));

            Assert.That(actual.Scale.X, Is.EqualTo(7.89));
            Assert.That(actual.Scale.Y, Is.EqualTo(8.90));
            Assert.That(actual.Scale.Z, Is.EqualTo(9.00));
        }

        [Test]
        public void FromDefinition()
        {
            // Arrange
            var mapper = new TransformDefinitionMapper();
            var transform = new TransformDefinition()
            {
                Translation = new Vector3Definition
                {
                    X = 1.23,
                    Y = 2.34,
                    Z = 3.45
                },
                Rotation = new Vector3Definition
                {
                    X = 4.56,
                    Y = 5.67,
                    Z = 6.78
                },
                Scale = new Vector3Definition
                {
                    X = 7.89,
                    Y = 8.90,
                    Z = 9.00
                }
            };

            // Act
            var actual = (Transform) mapper.FromDefinition(transform);

            // Assert
            Assert.That(actual.Translation.X, Is.EqualTo(1.23));
            Assert.That(actual.Translation.Y, Is.EqualTo(2.34));
            Assert.That(actual.Translation.Z, Is.EqualTo(3.45));

            Assert.That(actual.Rotation.X, Is.EqualTo(4.56));
            Assert.That(actual.Rotation.Y, Is.EqualTo(5.67));
            Assert.That(actual.Rotation.Z, Is.EqualTo(6.78));

            Assert.That(actual.Scale.X, Is.EqualTo(7.89));
            Assert.That(actual.Scale.Y, Is.EqualTo(8.90));
            Assert.That(actual.Scale.Z, Is.EqualTo(9.00));
        }
    }
}