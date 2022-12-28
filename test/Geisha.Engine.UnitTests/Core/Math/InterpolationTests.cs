using Geisha.Engine.Core.Math;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math
{
    [TestFixture]
    public class InterpolationTests
    {
        [TestCase(-3, 5, 0, -3)]
        [TestCase(-3, 5, 1, 5)]
        [TestCase(-3, 5, 0.5, 1)]
        [TestCase(-3, 5, 0.25, -1)]
        public void Lerp_Test(double a, double b, double alpha, double expected)
        {
            // Arrange
            // Act
            var actual = Interpolation.Lerp(a, b, alpha);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}