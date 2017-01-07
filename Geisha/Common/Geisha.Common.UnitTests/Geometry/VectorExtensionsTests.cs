using Geisha.Common.Geometry;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Geometry
{
    [TestFixture]
    public class VectorExtensionsTests
    {
        [TestCase(1, 2, 3)]
        [TestCase(91.3376, 63.2359, 9.7540)]
        public void AsVector2_ShouldReturnCorrectVector2_WhenVector3Given(double x, double y, double z)
        {
            // Arrange
            var vector3 = new Vector3(x, y, z);

            // Act
            var vector2 = vector3.AsVector2();

            // Assert
            Assert.That(vector2.X, Is.EqualTo(x));
            Assert.That(vector2.Y, Is.EqualTo(y));
        }

        [TestCase(1, 2, 3, 4)]
        [TestCase(91.3376, 63.2359, 9.7540, 27.8498)]
        public void AsVector2_ShouldReturnCorrectVector2_WhenVector4Given(double x, double y, double z, double w)
        {
            // Arrange
            var vector4 = new Vector4(x, y, z, w);

            // Act
            var vector2 = vector4.AsVector2();

            // Assert
            Assert.That(vector2.X, Is.EqualTo(x));
            Assert.That(vector2.Y, Is.EqualTo(y));
        }

        [TestCase(1, 2, 3, 4)]
        [TestCase(91.3376, 63.2359, 9.7540, 27.8498)]
        public void AsVector3_ShouldReturnCorrectVector3_WhenVector4Given(double x, double y, double z, double w)
        {
            // Arrange
            var vector4 = new Vector4(x, y, z, w);

            // Act
            var vector3 = vector4.AsVector3();

            // Assert
            Assert.That(vector3.X, Is.EqualTo(x));
            Assert.That(vector3.Y, Is.EqualTo(y));
        }
    }
}