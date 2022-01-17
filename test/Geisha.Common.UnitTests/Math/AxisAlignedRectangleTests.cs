using System.Collections.Generic;
using Geisha.Common.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Math
{
    [TestFixture]
    public class AxisAlignedRectangleTests
    {
        private const double Epsilon = 0.000001;
        private static IEqualityComparer<Vector2> Vector2Comparer => CommonEqualityComparer.Vector2(Epsilon);

        #region Constructors

        [Test]
        public void Constructor_FromDimensions_ShouldSetCenterAndWidthAndHeight()
        {
            // Arrange
            // Act
            var rectangle = new AxisAlignedRectangle(new Vector2(123, 456));

            // Assert
            Assert.That(rectangle.Center, Is.EqualTo(Vector2.Zero));
            Assert.That(rectangle.Width, Is.EqualTo(123));
            Assert.That(rectangle.Height, Is.EqualTo(456));
        }

        [Test]
        public void Constructor_FromCenterAndDimensions_ShouldSetCenterAndWidthAndHeight()
        {
            // Arrange
            // Act
            var rectangle = new AxisAlignedRectangle(new Vector2(12, 34), new Vector2(56, 78));

            // Assert
            Assert.That(rectangle.Center, Is.EqualTo(new Vector2(12, 34)));
            Assert.That(rectangle.Width, Is.EqualTo(56));
            Assert.That(rectangle.Height, Is.EqualTo(78));
        }

        #endregion

        #region Properties

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(10, 20, 200, 100, 110, 70)]
        [TestCase(4.928, -34.791, 47.196, 75.639, 28.526, 3.0285)]
        public void Max_Test(double centerX, double centerY, double width, double height, double maxX, double maxY)
        {
            // Arrange
            var rectangle = new AxisAlignedRectangle(new Vector2(centerX, centerY), new Vector2(width, height));

            // Act
            // Assert
            Assert.That(rectangle.Max, Is.EqualTo(new Vector2(maxX, maxY)).Using(Vector2Comparer));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(10, 20, 200, 100, -90, -30)]
        [TestCase(4.928, -34.791, 47.196, 75.639, -18.67, -72.6105)]
        public void Min_Test(double centerX, double centerY, double width, double height, double minX, double minY)
        {
            // Arrange
            var rectangle = new AxisAlignedRectangle(new Vector2(centerX, centerY), new Vector2(width, height));

            // Act
            // Assert
            Assert.That(rectangle.Min, Is.EqualTo(new Vector2(minX, minY)).Using(Vector2Comparer));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(10, 20, 200, 100, -90, 70)]
        [TestCase(4.928, -34.791, 47.196, 75.639, -18.67, 3.0285)]
        public void UpperLeft_Test(double centerX, double centerY, double width, double height, double expectedX, double expectedY)
        {
            // Arrange
            var rectangle = new AxisAlignedRectangle(new Vector2(centerX, centerY), new Vector2(width, height));

            // Act
            // Assert
            Assert.That(rectangle.UpperLeft, Is.EqualTo(new Vector2(expectedX, expectedY)).Using(Vector2Comparer));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(10, 20, 200, 100, 110, 70)]
        [TestCase(4.928, -34.791, 47.196, 75.639, 28.526, 3.0285)]
        public void UpperRight_Test(double centerX, double centerY, double width, double height, double expectedX, double expectedY)
        {
            // Arrange
            var rectangle = new AxisAlignedRectangle(new Vector2(centerX, centerY), new Vector2(width, height));

            // Act
            // Assert
            Assert.That(rectangle.UpperRight, Is.EqualTo(new Vector2(expectedX, expectedY)).Using(Vector2Comparer));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(10, 20, 200, 100, -90, -30)]
        [TestCase(4.928, -34.791, 47.196, 75.639, -18.67, -72.6105)]
        public void LowerLeft_Test(double centerX, double centerY, double width, double height, double expectedX, double expectedY)
        {
            // Arrange
            var rectangle = new AxisAlignedRectangle(new Vector2(centerX, centerY), new Vector2(width, height));

            // Act
            // Assert
            Assert.That(rectangle.LowerLeft, Is.EqualTo(new Vector2(expectedX, expectedY)).Using(Vector2Comparer));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(10, 20, 200, 100, 110, -30)]
        [TestCase(4.928, -34.791, 47.196, 75.639, 28.526, -72.6105)]
        public void LowerRight_Test(double centerX, double centerY, double width, double height, double expectedX, double expectedY)
        {
            // Arrange
            var rectangle = new AxisAlignedRectangle(new Vector2(centerX, centerY), new Vector2(width, height));

            // Act
            // Assert
            Assert.That(rectangle.LowerRight, Is.EqualTo(new Vector2(expectedX, expectedY)).Using(Vector2Comparer));
        }

        #endregion

        #region Methods

        [TestCase(0, 0, 0, 0,
            "Center: X: 0, Y: 0, Width: 0, Height: 0")]
        [TestCase(10, 20, 50, 100,
            "Center: X: 10, Y: 20, Width: 50, Height: 100")]
        [SetCulture("")]
        public void ToString_Test(double x, double y, double w, double h, string expected)
        {
            // Arrange
            var rectangle = new AxisAlignedRectangle(new Vector2(x, y), new Vector2(w, h));

            // Act
            var actual = rectangle.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase( /*R1*/0, 0, 0, 0, /*R2*/ 0, 0, 0, 0, /*E*/ true)]
        [TestCase( /*R1*/1, 2, 3, 4, /*R2*/ 1, 2, 3, 4, /*E*/ true)]
        [TestCase( /*R1*/1, 2, 3, 4, /*R2*/ 0, 2, 3, 4, /*E*/ false)]
        [TestCase( /*R1*/1, 2, 3, 4, /*R2*/ 1, 0, 3, 4, /*E*/ false)]
        [TestCase( /*R1*/1, 2, 3, 4, /*R2*/ 1, 2, 0, 4, /*E*/ false)]
        [TestCase( /*R1*/1, 2, 3, 4, /*R2*/ 1, 2, 3, 0, /*E*/ false)]
        public void EqualityMembers_ShouldEqualAxisAlignedRectangle_WhenCenterAndWidthAndHeightAreEqual(double x1, double y1, double w1, double h1, double x2,
            double y2, double w2, double h2, bool expectedIsEqual)
        {
            // Arrange
            var rectangle1 = new AxisAlignedRectangle(new Vector2(x1, y1), new Vector2(w1, h1));
            var rectangle2 = new AxisAlignedRectangle(new Vector2(x2, y2), new Vector2(w2, h2));

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(rectangle1, rectangle2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }

        #endregion
    }
}