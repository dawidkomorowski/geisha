using Geisha.Common.Math;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math
{
    [TestFixture]
    public class AngleTests
    {
        [TestCase(0, 0)]
        [TestCase(90, System.Math.PI / 2)]
        [TestCase(180, System.Math.PI)]
        [TestCase(360, 2 * System.Math.PI)]
        [TestCase(720, 4 * System.Math.PI)]
        [TestCase(37.375612, 0.65232748934790286)]
        public void Deg2Rad_And_Rag2Deg(double degrees, double radians)
        {
            // Arrange
            // Act
            var actualRadians = Angle.Deg2Rad(degrees);
            var actualDegrees = Angle.Rad2Deg(radians);

            // Assert
            Assert.That(actualRadians, Is.EqualTo(radians));
            Assert.That(actualDegrees, Is.EqualTo(degrees));
        }
    }
}