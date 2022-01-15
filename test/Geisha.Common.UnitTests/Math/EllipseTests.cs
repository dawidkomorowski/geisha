using Geisha.Common.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Math
{
    [TestFixture]
    public class EllipseTests
    {
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