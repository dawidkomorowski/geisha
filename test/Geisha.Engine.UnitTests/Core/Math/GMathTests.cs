using System;
using Geisha.Engine.Core.Math;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math
{
    [TestFixture]
    public class GMathTests
    {
        [TestCase(0, 0, true)]
        [TestCase(1d, 1d, true)]
        [TestCase(double.MaxValue, double.MaxValue, true)]
        [TestCase(double.MinValue, double.MinValue, true)]
        [TestCase(double.Epsilon, double.Epsilon, true)]
        [TestCase(double.Epsilon, 2 * double.Epsilon, true)]
        [TestCase(double.Epsilon, 3 * double.Epsilon, false)]
        [TestCase(1e20d, 1e20d + 1e5d, true)]
        [TestCase(1e20d, 1e20d + 1e6d, false)]
        [TestCase(1e-20d, 1e-20d + 1e-36d, true)]
        [TestCase(1e-20d, 1e-20d + 1e-35d, false)]
        public void AlmostEqual_Test(double a, double b, bool expected)
        {
            // Arrange
            // Act
            var actual = GMath.AlmostEqual(a, b);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(-3, 5, 0, -3)]
        [TestCase(-3, 5, 1, 5)]
        [TestCase(-3, 5, 0.5, 1)]
        [TestCase(-3, 5, 0.25, -1)]
        public void Lerp_Test(double a, double b, double alpha, double expected)
        {
            // Arrange
            // Act
            var actual = GMath.Lerp(a, b, alpha);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}