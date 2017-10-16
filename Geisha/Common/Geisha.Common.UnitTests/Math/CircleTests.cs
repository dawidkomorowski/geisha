using Geisha.Common.Math;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Math
{
    [TestFixture]
    public class CircleTests
    {
        #region Constructors

        [TestCase(0)]
        [TestCase(10)]
        [TestCase(45.173)]
        public void Constructor_FromRadius_ShouldCreateCircleAtOriginWithGivenRadius(double radius)
        {
            // Arrange
            // Act
            var circle = new Circle(radius);

            // Assert
            Assert.That(circle.Center, Is.EqualTo(Vector2.Zero));
            Assert.That(circle.Radius, Is.EqualTo(radius));
        }

        [TestCase(0, 0, 0)]
        [TestCase(0, 0, 10)]
        [TestCase(5, -5, 10)]
        [TestCase(58.373, -82.671, 45.654)]
        public void Constructor_FromCenterAndRadius_ShouldCreateCircleWithGivenCenterAndRadius(double centerX, double centerY, double radius)
        {
            // Arrange
            // Act
            var circle = new Circle(new Vector2(centerX, centerY), radius);

            // Assert
            Assert.That(circle.Center.X, Is.EqualTo(centerX));
            Assert.That(circle.Center.Y, Is.EqualTo(centerY));
            Assert.That(circle.Radius, Is.EqualTo(radius));
        }

        #endregion

        #region Methods

        [TestCase(0, 0, 1, 10, 0, 1, false)]
        [TestCase(0, 0, 1, 0, 10, 1, false)]
        [TestCase(0, 0, 1, 10, 10, 1, false)]
        [TestCase(0, 0, 1, 2, 0, 1, false)]
        [TestCase(0, 0, 1, 0, 2, 1, false)]
        [TestCase(0, 0, 1, 1.42, 1.42, 1, false)]
        [TestCase(0, 0, 1, 1.9, 0, 1, true)]
        [TestCase(0, 0, 1, 0, 1.9, 1, true)]
        [TestCase(0, 0, 1, 1.41, 1.41, 1, true)]
        public void Overlaps_WithCircle(double c1x, double c1y, double r1, double c2x, double c2y, double r2, bool expected)
        {
            // Arrange
            var circle1 = new Circle(new Vector2(c1x, c1y), r1);
            var circle2 = new Circle(new Vector2(c2x, c2y), r2);

            // Act
            var actual1 = circle1.Overlaps(circle2);
            var actual2 = circle2.Overlaps(circle1);

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
        }

        #endregion
    }
}