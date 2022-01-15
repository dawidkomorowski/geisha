using System.Collections.Generic;
using Geisha.Common.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Math
{
    [TestFixture]
    [DefaultFloatingPointTolerance(Epsilon)]
    public class RectangleTests
    {
        private const double Epsilon = 0.000001;
        private static IEqualityComparer<Vector2> Vector2Comparer => CommonEqualityComparer.Vector2(Epsilon);

        #region Properties

        [TestCase(0, 0, 1, 1, 0)]
        [TestCase(1.234, -4.321, 12.34, 43.21, 0)]
        [TestCase(1.234, -4.321, 12.34, 43.21, 45)]
        [TestCase(1.234, -4.321, 12.34, 43.21, 90)]
        [TestCase(1.234, -4.321, 12.34, 43.21, 180)]
        public void Center(double centerX, double centerY, double dimensionX, double dimensionY, double rotation)
        {
            // Arrange
            var center = new Vector2(centerX, centerY);
            var dimension = new Vector2(dimensionX, dimensionY);

            // We want to rotate around center of rectangle thus we need to transform by center after rotation.
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            var rectangle = rotation == 0
                ? new Rectangle(center, dimension)
                : new Rectangle(dimension).Transform(Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation))).Transform(Matrix3x3.CreateTranslation(center));

            // Act
            var actualCenter = rectangle.Center;

            // Assert
            Assert.That(actualCenter, Is.EqualTo(center).Using(Vector2Comparer));
        }

        [TestCase(0, 0, 1, 1, 0, 1)]
        [TestCase(1.234, -4.321, 12.34, 43.21, 0, 12.34)]
        [TestCase(0, 0, 10, 5, 45, 10)]
        [TestCase(0, 0, 10, 5, 90, 10)]
        [TestCase(0, 0, 10, 5, 180, 10)]
        public void Width(double centerX, double centerY, double dimensionX, double dimensionY, double rotation, double expectedWidth)
        {
            // Arrange
            var center = new Vector2(centerX, centerY);
            var dimension = new Vector2(dimensionX, dimensionY);

            var rectangle = new Rectangle(center, dimension).Transform(Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation)));

            // Act
            var actualWidth = rectangle.Width;

            // Assert
            Assert.That(actualWidth, Is.EqualTo(expectedWidth));
        }

        [TestCase(0, 0, 1, 1, 0, 1)]
        [TestCase(1.234, -4.321, 12.34, 43.21, 0, 43.21)]
        [TestCase(0, 0, 10, 5, 45, 5)]
        [TestCase(0, 0, 10, 5, 90, 5)]
        [TestCase(0, 0, 10, 5, 180, 5)]
        public void Height(double centerX, double centerY, double dimensionX, double dimensionY, double rotation, double expectedHeight)
        {
            // Arrange
            var center = new Vector2(centerX, centerY);
            var dimension = new Vector2(dimensionX, dimensionY);

            var rectangle = new Rectangle(center, dimension).Transform(Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation)));

            // Act
            var actualHeight = rectangle.Height;

            // Assert
            Assert.That(actualHeight, Is.EqualTo(expectedHeight));
        }

        #endregion

        #region Constructors

        [TestCase(1, 1,
            -0.5, 0.5, 0.5, 0.5, -0.5, -0.5, 0.5, -0.5)]
        [TestCase(47.196, 75.639,
            -23.598, 37.8195, 23.598, 37.8195, -23.598, -37.8195, 23.598, -37.8195)]
        public void Constructor_FromDimension_ShouldSetupVerticesCorrectly(double dimX, double dimY, double expectedULx,
            double expectedULy, double expectedURx, double expectedURy, double expectedLLx, double expectedLLy,
            double expectedLRx, double expectedLRy)
        {
            // Arrange
            var dimension = new Vector2(dimX, dimY);

            var expectedUpperLeft = new Vector2(expectedULx, expectedULy);
            var expectedUpperRight = new Vector2(expectedURx, expectedURy);
            var expectedLowerLeft = new Vector2(expectedLLx, expectedLLy);
            var expectedLowerRight = new Vector2(expectedLRx, expectedLRy);

            // Act
            var rectangle = new Rectangle(dimension);

            // Assert
            Assert.That(rectangle.UpperLeft, Is.EqualTo(expectedUpperLeft));
            Assert.That(rectangle.UpperRight, Is.EqualTo(expectedUpperRight));
            Assert.That(rectangle.LowerLeft, Is.EqualTo(expectedLowerLeft));
            Assert.That(rectangle.LowerRight, Is.EqualTo(expectedLowerRight));
        }

        [TestCase(1, 1, 1, 1,
            0.5, 1.5, 1.5, 1.5, 0.5, 0.5, 1.5, 0.5)]
        [TestCase(4.928, -34.791, 47.196, 75.639,
            -18.67, 3.0285, 28.526, 3.0285, -18.67, -72.6105, 28.526, -72.6105)]
        public void Constructor_FromCenterAndDimension_ShouldSetupVerticesCorrectly(double centerX, double centerY,
            double dimX, double dimY, double expectedULx, double expectedULy, double expectedURx, double expectedURy,
            double expectedLLx, double expectedLLy, double expectedLRx, double expectedLRy)
        {
            // Arrange
            var center = new Vector2(centerX, centerY);
            var dimension = new Vector2(dimX, dimY);

            var expectedUpperLeft = new Vector2(expectedULx, expectedULy);
            var expectedUpperRight = new Vector2(expectedURx, expectedURy);
            var expectedLowerLeft = new Vector2(expectedLLx, expectedLLy);
            var expectedLowerRight = new Vector2(expectedLRx, expectedLRy);

            // Act
            var rectangle = new Rectangle(center, dimension);

            // Assert
            Assert.That(rectangle.UpperLeft, Is.EqualTo(expectedUpperLeft).Using(Vector2Comparer));
            Assert.That(rectangle.UpperRight, Is.EqualTo(expectedUpperRight).Using(Vector2Comparer));
            Assert.That(rectangle.LowerLeft, Is.EqualTo(expectedLowerLeft).Using(Vector2Comparer));
            Assert.That(rectangle.LowerRight, Is.EqualTo(expectedLowerRight).Using(Vector2Comparer));
        }

        #endregion

        #region Methods

        [TestCase(1, 1)]
        [TestCase(47.196, 75.639)]
        public void Transform_ShouldTransformEachVertexOfRectangle(double dimX, double dimY)
        {
            // Arrange
            var rectangle = new Rectangle(new Vector2(dimX, dimY));
            var transform = new Matrix3x3(1, 2, 3, 4, 5, 6, 7, 8, 9);

            var expectedUpperLeft = (transform * rectangle.UpperLeft.Homogeneous).ToVector2();
            var expectedUpperRight = (transform * rectangle.UpperRight.Homogeneous).ToVector2();
            var expectedLowerLeft = (transform * rectangle.LowerLeft.Homogeneous).ToVector2();
            var expectedLowerRight = (transform * rectangle.LowerRight.Homogeneous).ToVector2();

            // Act
            var actual = rectangle.Transform(transform);

            // Assert
            Assert.That(actual.UpperLeft, Is.EqualTo(expectedUpperLeft));
            Assert.That(actual.UpperRight, Is.EqualTo(expectedUpperRight));
            Assert.That(actual.LowerLeft, Is.EqualTo(expectedLowerLeft));
            Assert.That(actual.LowerRight, Is.EqualTo(expectedLowerRight));
        }

        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 10, 0, 1, 2, /*E*/ false)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 0, 10, 1, 2, /*E*/ false)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 10, 10, 1, 2, /*E*/ false)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 1.6, 0, 1, 2, /*E*/ false)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 0, 1.6, 1, 2, /*E*/ false)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 1.6, 1.6, 1, 2, /*E*/ false)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 1.5, 0, 1, 2, /*E*/ true)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 0, 1.5, 1, 2, /*E*/ true)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 1.5, 1.5, 1, 2, /*E*/ true)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 1.4, 0, 1, 2, /*E*/ true)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 0, 1.4, 1, 2, /*E*/ true)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 1.4, 1.4, 1, 2, /*E*/ true)]
        public void Overlaps_WithRectangle_AxisAligned(double c1X, double c1Y, double w1, double h1, double c2X, double c2Y, double w2, double h2,
            bool expected)
        {
            // Arrange
            var rectangle1 = new Rectangle(new Vector2(c1X, c1Y), new Vector2(w1, h1));
            var rectangle2 = new Rectangle(new Vector2(c2X, c2Y), new Vector2(w2, h2));

            // Act
            var actual1 = rectangle1.Overlaps(rectangle2);
            var actual2 = rectangle2.Overlaps(rectangle1);

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0,
            "Center: X: 0, Y: 0, Width: 0, Height: 0, UpperLeft: X: 0, Y: 0, UpperRight: X: 0, Y: 0, LowerLeft: X: 0, Y: 0, LowerRight: X: 0, Y: 0")]
        [TestCase(10, 20, 50, 100,
            "Center: X: 10, Y: 20, Width: 50, Height: 100, UpperLeft: X: -15, Y: 70, UpperRight: X: 35, Y: 70, LowerLeft: X: -15, Y: -30, LowerRight: X: 35, Y: -30")]
        [SetCulture("")]
        public void ToString_Test(double x, double y, double w, double h, string expected)
        {
            // Arrange
            var rectangle = new Rectangle(new Vector2(x, y), new Vector2(w, h));

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
        public void EqualityMembers_ShouldEqualRectangle_WhenAllVerticesAreEqual(double x1, double y1, double w1, double h1, double x2, double y2, double w2,
            double h2, bool expectedIsEqual)
        {
            // Arrange
            var rectangle1 = new Rectangle(new Vector2(x1, y1), new Vector2(w1, h1));
            var rectangle2 = new Rectangle(new Vector2(x2, y2), new Vector2(w2, h2));

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