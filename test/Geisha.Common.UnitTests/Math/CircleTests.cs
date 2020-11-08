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

        [TestCase(0, 0, 10)]
        [TestCase(47.196, 75.639, 15.627)]
        public void Transform_ShouldTransformCenterOfCircle(double centerX, double centerY, double radius)
        {
            // Arrange
            var circle = new Circle(new Vector2(centerX, centerY), radius);
            var transform = new Matrix3x3(1, 2, 3, 4, 5, 6, 7, 8, 9);

            var expectedCenter = (transform * circle.Center.Homogeneous).ToVector2();

            // Act
            var actual = circle.Transform(transform);

            // Assert
            Assert.That(actual.Center, Is.EqualTo(expectedCenter));
            Assert.That(actual.Radius, Is.EqualTo(circle.Radius));
        }

        [TestCase( /*C1*/ 0, 0, 1, /*C2*/ 10, 0, 1, /*E*/ false)]
        [TestCase( /*C1*/ 0, 0, 1, /*C2*/ 0, 10, 1, /*E*/ false)]
        [TestCase( /*C1*/ 0, 0, 1, /*C2*/ 10, 10, 1, /*E*/ false)]
        [TestCase( /*C1*/ 0, 0, 1, /*C2*/ 1.42, 1.42, 1, /*E*/ false)]
        [TestCase( /*C1*/ 0, 0, 1, /*C2*/ 2.1, 0, 1, /*E*/ false)]
        [TestCase( /*C1*/ 0, 0, 1, /*C2*/ 0, 2.1, 1, /*E*/ false)]
        [TestCase( /*C1*/ 0, 0, 1, /*C2*/ 2, 0, 1, /*E*/ true)]
        [TestCase( /*C1*/ 0, 0, 1, /*C2*/ 0, 2, 1, /*E*/ true)]
        [TestCase( /*C1*/ 0, 0, 1, /*C2*/ 1.9, 0, 1, /*E*/ true)]
        [TestCase( /*C1*/ 0, 0, 1, /*C2*/ 0, 1.9, 1, /*E*/ true)]
        [TestCase( /*C1*/ 0, 0, 1, /*C2*/ 1.41, 1.41, 1, /*E*/ true)]
        public void Overlaps_WithCircle(double c1X, double c1Y, double r1, double c2X, double c2Y, double r2, bool expected)
        {
            // Arrange
            var circle1 = new Circle(new Vector2(c1X, c1Y), r1);
            var circle2 = new Circle(new Vector2(c2X, c2Y), r2);

            // Act
            var actual1 = circle1.Overlaps(circle2);
            var actual2 = circle2.Overlaps(circle1);

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
        }

        [Test]
        public void ToEllipse_ShouldReturnEllipseRepresentingThisCircle()
        {
            // Arrange
            var circle = new Circle(new Vector2(47.196, 75.639), 15.627);

            // Act
            var ellipse = circle.ToEllipse();

            // Assert
            Assert.That(ellipse.Center, Is.EqualTo(circle.Center));
            Assert.That(ellipse.RadiusX, Is.EqualTo(circle.Radius));
            Assert.That(ellipse.RadiusY, Is.EqualTo(circle.Radius));
        }

        [TestCase(0, 0, 0, "Center: X: 0, Y: 0, Radius: 0")]
        [TestCase(74.025, -27.169, 15.627, "Center: X: 74.025, Y: -27.169, Radius: 15.627")]
        [SetCulture("")]
        public void ToString_Test(double x, double y, double radius, string expected)
        {
            // Arrange
            var circle = new Circle(new Vector2(x, y), radius);

            // Act
            var actual = circle.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        #endregion
    }
}