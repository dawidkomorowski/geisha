using Geisha.Common.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Math
{
    [TestFixture]
    public class EllipseTests
    {
        [Test]
        public void GetBoundingRectangle_ShouldReturnMinimalAxisAlignedRectangleContainingThisEllipse()
        {
            // Arrange
            var ellipse = new Ellipse(new Vector2(47.196, 75.639), 15.627, 45.654);

            // Act
            var boundingRectangle = ellipse.GetBoundingRectangle();

            // Assert
            Assert.That(boundingRectangle.Center, Is.EqualTo(new Vector2(47.196, 75.639)));
            Assert.That(boundingRectangle.Dimensions, Is.EqualTo(new Vector2(31.254, 91.308)));
        }

        [TestCase(0, 0, 0, 0, "Center: X: 0, Y: 0, RadiusX: 0, RadiusY: 0")]
        [TestCase(74.025, -27.169, 15.627, 45.654, "Center: X: 74.025, Y: -27.169, RadiusX: 15.627, RadiusY: 45.654")]
        [SetCulture("")]
        public void ToString_Test(double x, double y, double radiusX, double radiusY, string expected)
        {
            // Arrange
            var ellipse = new Ellipse(new Vector2(x, y), radiusX, radiusY);

            // Act
            var actual = ellipse.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase( /*E1*/0, 0, 0, 0, /*E2*/ 0, 0, 0, 0, /*E*/ true)]
        [TestCase( /*E1*/1, 2, 3, 4, /*E2*/ 1, 2, 3, 4, /*E*/ true)]
        [TestCase( /*E1*/1, 2, 3, 4, /*E2*/ 0, 2, 3, 4, /*E*/ false)]
        [TestCase( /*E1*/1, 2, 3, 4, /*E2*/ 1, 0, 3, 4, /*E*/ false)]
        [TestCase( /*E1*/1, 2, 3, 4, /*E2*/ 1, 2, 0, 4, /*E*/ false)]
        [TestCase( /*E1*/1, 2, 3, 4, /*E2*/ 1, 2, 3, 0, /*E*/ false)]
        public void EqualityMembers_ShouldEqualEllipse_WhenCenterAndRadiusXAndRadiusYAreEqual(double x1, double y1, double rx1, double ry1, double x2,
            double y2, double rx2, double ry2, bool expectedIsEqual)
        {
            // Arrange
            var ellipse1 = new Ellipse(new Vector2(x1, y1), rx1, ry1);
            var ellipse2 = new Ellipse(new Vector2(x2, y2), rx2, ry2);

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(ellipse1, ellipse2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }
    }
}