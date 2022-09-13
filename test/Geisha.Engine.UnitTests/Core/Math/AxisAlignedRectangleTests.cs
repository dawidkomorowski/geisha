using System.Collections.Generic;
using Geisha.Common.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math
{
    [TestFixture]
    public class AxisAlignedRectangleTests
    {
        private const double Epsilon = 0.000001;
        private static IEqualityComparer<Vector2> Vector2Comparer => CommonEqualityComparer.Vector2(Epsilon);

        #region Constructors

        [Test]
        public void Constructor_FromDimensions_ShouldSetCenterAndDimensions()
        {
            // Arrange
            // Act
            var rectangle = new AxisAlignedRectangle(new Vector2(123, 456));

            // Assert
            Assert.That(rectangle.Center, Is.EqualTo(Vector2.Zero));
            Assert.That(rectangle.Dimensions, Is.EqualTo(new Vector2(123, 456)));
        }

        [Test]
        public void Constructor_FromWidthAndHeight_ShouldSetCenterAndDimensions()
        {
            // Arrange
            // Act
            var rectangle = new AxisAlignedRectangle(123, 456);

            // Assert
            Assert.That(rectangle.Center, Is.EqualTo(Vector2.Zero));
            Assert.That(rectangle.Dimensions, Is.EqualTo(new Vector2(123, 456)));
        }

        [Test]
        public void Constructor_FromCenterAndDimensions_ShouldSetCenterAndDimensions()
        {
            // Arrange
            // Act
            var rectangle = new AxisAlignedRectangle(new Vector2(12, 34), new Vector2(56, 78));

            // Assert
            Assert.That(rectangle.Center, Is.EqualTo(new Vector2(12, 34)));
            Assert.That(rectangle.Dimensions, Is.EqualTo(new Vector2(56, 78)));
        }

        [Test]
        public void Constructor_FromCenterXAndCenterYAndWidthAndHeight_ShouldSetCenterAndDimensions()
        {
            // Arrange
            // Act
            var rectangle = new AxisAlignedRectangle(12, 34, 56, 78);

            // Assert
            Assert.That(rectangle.Center, Is.EqualTo(new Vector2(12, 34)));
            Assert.That(rectangle.Dimensions, Is.EqualTo(new Vector2(56, 78)));
        }

        [TestCase(new double[] { }, 0, 0, 0, 0)]
        [TestCase(new[] { 2.0, -3.0 }, 2.0, -3.0, 0, 0)]
        [TestCase(new[] { 2.0, -3.0, /**/ 5.0, 8.0 }, 3.5, 2.5, 3, 11)]
        [TestCase(new[] { 2.0, 8.0, /**/ 5.0, -3.0 }, 3.5, 2.5, 3, 11)]
        [TestCase(new[] { 2.0, 5.0, /**/ 3.5, 8.0, /**/ 4.5, -3.0, /**/ 5.0, -1.5 }, 3.5, 2.5, 3, 11)]
        public void Constructor_FromPoints_ShouldSetCenterAndDimensions(double[] points, double centerX, double centerY, double width, double height)
        {
            // Arrange
            var pointsAsVectors = new List<Vector2>();
            for (var i = 0; i < points.Length; i += 2)
            {
                pointsAsVectors.Add(new Vector2(points[i], points[i + 1]));
            }

            // Act
            var rectangle = new AxisAlignedRectangle(pointsAsVectors.ToArray());

            // Assert
            Assert.That(rectangle.Center.X, Is.EqualTo(centerX));
            Assert.That(rectangle.Center.Y, Is.EqualTo(centerY));
            Assert.That(rectangle.Dimensions.X, Is.EqualTo(width));
            Assert.That(rectangle.Dimensions.Y, Is.EqualTo(height));
        }

        [TestCase(new double[] { }, 0, 0, 0, 0)]
        [TestCase(new double[] { 2, 4, 8, 6 }, 2, 4, 8, 6)]
        [TestCase(new double[]
        {
            /*R1*/ 2, 4, 8, 6, /*R2*/ -5, -2, 4, 10
        }, -0.5, 0, 13, 14)]
        public void Constructor_FromAxisAlignedRectangles_ShouldSetCenterAndDimensions(double[] rectangles /*x,y,w,h*/, double centerX, double centerY,
            double width, double height)
        {
            // Arrange
            var rectanglesAsStructures = new List<AxisAlignedRectangle>();
            for (var i = 0; i < rectangles.Length; i += 4)
            {
                var center = new Vector2(rectangles[i], rectangles[i + 1]);
                var dimensions = new Vector2(rectangles[i + 2], rectangles[i + 3]);
                rectanglesAsStructures.Add(new AxisAlignedRectangle(center, dimensions));
            }

            // Act
            var rectangle = new AxisAlignedRectangle(rectanglesAsStructures.ToArray());

            // Assert
            Assert.That(rectangle.Center.X, Is.EqualTo(centerX));
            Assert.That(rectangle.Center.Y, Is.EqualTo(centerY));
            Assert.That(rectangle.Dimensions.X, Is.EqualTo(width));
            Assert.That(rectangle.Dimensions.Y, Is.EqualTo(height));
        }

        #endregion

        #region Properties

        [Test]
        public void Width_Test()
        {
            // Arrange
            var rectangle = new AxisAlignedRectangle(20, 10);

            // Act
            // Assert
            Assert.That(rectangle.Width, Is.EqualTo(20));
        }

        [Test]
        public void Height_Test()
        {
            // Arrange
            var rectangle = new AxisAlignedRectangle(20, 10);

            // Act
            // Assert
            Assert.That(rectangle.Height, Is.EqualTo(10));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(10, 20, 200, 100, 110, 70)]
        [TestCase(4.928, -34.791, 47.196, 75.639, 28.526, 3.0285)]
        public void Max_Test(double centerX, double centerY, double width, double height, double maxX, double maxY)
        {
            // Arrange
            var rectangle = new AxisAlignedRectangle(centerX, centerY, width, height);

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
            var rectangle = new AxisAlignedRectangle(centerX, centerY, width, height);

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
            var rectangle = new AxisAlignedRectangle(centerX, centerY, width, height);

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
            var rectangle = new AxisAlignedRectangle(centerX, centerY, width, height);

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
            var rectangle = new AxisAlignedRectangle(centerX, centerY, width, height);

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
            var rectangle = new AxisAlignedRectangle(centerX, centerY, width, height);

            // Act
            // Assert
            Assert.That(rectangle.LowerRight, Is.EqualTo(new Vector2(expectedX, expectedY)).Using(Vector2Comparer));
        }

        #endregion

        #region Methods

        [TestCase( /*R*/4, 2, 10, 6, /*P*/4, 2, true)]
        [TestCase( /*R*/4, 2, 10, 6, /*P*/-1, 5, true)]
        [TestCase( /*R*/4, 2, 10, 6, /*P*/9, 5, true)]
        [TestCase( /*R*/4, 2, 10, 6, /*P*/-1, -1, true)]
        [TestCase( /*R*/4, 2, 10, 6, /*P*/9, -1, true)]
        [TestCase( /*R*/4, 2, 10, 6, /*P*/14, 2, false)]
        [TestCase( /*R*/4, 2, 10, 6, /*P*/-6, 2, false)]
        [TestCase( /*R*/4, 2, 10, 6, /*P*/4, 12, false)]
        [TestCase( /*R*/4, 2, 10, 6, /*P*/4, -8, false)]
        [TestCase( /*R*/4, 2, 10, 6, /*P*/14, 12, false)]
        [TestCase( /*R*/4, 2, 10, 6, /*P*/-6, -8, false)]
        public void Contains_ShouldReturnTrue_GivenPointThatIsContainedInAxisAlignedRectangle(double centerX, double centerY, double width, double height,
            double pointX, double pointY, bool expected)
        {
            // Arrange
            var rectangle = new AxisAlignedRectangle(centerX, centerY, width, height);
            var point = new Vector2(pointX, pointY);

            // Act
            var actual = rectangle.Contains(point);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(4, 2, 10, 6, 4, 2, 10, 6, true)]
        [TestCase(4, 2, 10, 6, 4, 2, 5, 3, true)]
        [TestCase(4, 2, 10, 6, 1.5, 3.5, 5, 3, true)]
        [TestCase(4, 2, 10, 6, 6.5, 3.5, 5, 3, true)]
        [TestCase(4, 2, 10, 6, 1.5, 0.5, 5, 3, true)]
        [TestCase(4, 2, 10, 6, 6.5, 0.5, 5, 3, true)]
        [TestCase(4, 2, 10, 6, -1, 5, 6, 4, false)]
        [TestCase(4, 2, 10, 6, 9, 5, 6, 4, false)]
        [TestCase(4, 2, 10, 6, -1, -1, 6, 4, false)]
        [TestCase(4, 2, 10, 6, 9, -1, 6, 4, false)]
        [TestCase(4, 2, 10, 6, 14, 2, 6, 4, false)]
        [TestCase(4, 2, 10, 6, -6, 2, 6, 4, false)]
        [TestCase(4, 2, 10, 6, 4, 12, 6, 4, false)]
        [TestCase(4, 2, 10, 6, 4, -8, 6, 4, false)]
        public void Contains_ShouldReturnTrue_GivenAxisAlignedRectangleThatIsContainedInAxisAlignedRectangle(double centerX1, double centerY1, double width1,
            double height1, double centerX2, double centerY2, double width2, double height2, bool expected)
        {
            // Arrange
            var rectangle1 = new AxisAlignedRectangle(centerX1, centerY1, width1, height1);
            var rectangle2 = new AxisAlignedRectangle(centerX2, centerY2, width2, height2);

            // Act
            var actual = rectangle1.Contains(rectangle2);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(4, 2, 10, 6, 4, 2, 10, 6, true)]
        [TestCase(4, 2, 10, 6, -1, 5, 6, 4, true)]
        [TestCase(4, 2, 10, 6, 9, 5, 6, 4, true)]
        [TestCase(4, 2, 10, 6, -1, -1, 6, 4, true)]
        [TestCase(4, 2, 10, 6, 9, -1, 6, 4, true)]
        [TestCase(4, 2, 10, 6, -4, 7, 6, 4, true)]
        [TestCase(4, 2, 10, 6, 12, 7, 6, 4, true)]
        [TestCase(4, 2, 10, 6, -4, -3, 6, 4, true)]
        [TestCase(4, 2, 10, 6, 12, -3, 6, 4, true)]
        [TestCase(4, 2, 10, 6, 14, 2, 6, 4, false)]
        [TestCase(4, 2, 10, 6, -6, 2, 6, 4, false)]
        [TestCase(4, 2, 10, 6, 4, 12, 6, 4, false)]
        [TestCase(4, 2, 10, 6, 4, -8, 6, 4, false)]
        public void Overlaps_ShouldReturnTrue_GivenAxisAlignedRectangleThatOverlapsOtherAxisAlignedRectangle(double centerX1, double centerY1, double width1,
            double height1, double centerX2, double centerY2, double width2, double height2, bool expected)
        {
            // Arrange
            var rectangle1 = new AxisAlignedRectangle(centerX1, centerY1, width1, height1);
            var rectangle2 = new AxisAlignedRectangle(centerX2, centerY2, width2, height2);

            // Act
            var actual = rectangle1.Overlaps(rectangle2);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0,
            "Center: X: 0, Y: 0, Width: 0, Height: 0")]
        [TestCase(10, 20, 50, 100,
            "Center: X: 10, Y: 20, Width: 50, Height: 100")]
        [SetCulture("")]
        public void ToString_Test(double x, double y, double w, double h, string expected)
        {
            // Arrange
            var rectangle = new AxisAlignedRectangle(x, y, w, h);

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
            var rectangle1 = new AxisAlignedRectangle(x1, y1, w1, h1);
            var rectangle2 = new AxisAlignedRectangle(x2, y2, w2, h2);

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