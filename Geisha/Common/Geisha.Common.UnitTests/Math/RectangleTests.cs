using System.Collections.Generic;
using Geisha.Common.Math;
using Geisha.Common.UnitTests.TestHelpers;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Math
{
    [TestFixture]
    public class RectangleTests
    {
        private const double Epsilon = 0.000001;
        private static IEqualityComparer<Vector2> Vector2Comparer => VectorEqualityComparer.Vector2(Epsilon);

        #region Properties

        [TestCase(0, 0, 1, 1)]
        [TestCase(1.234, -4.321, 12.34, 43.21)]
        public void Center(double centerX, double centerY, double dimensionX, double dimensionY)
        {
            // Arrange
            var center = new Vector2(centerX, centerY);
            var dimension = new Vector2(dimensionX, dimensionY);

            var rectangle = new Rectangle(center, dimension);

            // Act
            var actualCenter = rectangle.Center;

            // Assert
            Assert.That(actualCenter, Is.EqualTo(center).Using(Vector2Comparer));
        }

        [TestCase(0, 0, 1, 1, 1)]
        [TestCase(1.234, -4.321, 12.34, 43.21, 12.34)]
        public void Width(double centerX, double centerY, double dimensionX, double dimensionY, double expectedWidth)
        {
            // Arrange
            var center = new Vector2(centerX, centerY);
            var dimension = new Vector2(dimensionX, dimensionY);

            var rectangle = new Rectangle(center, dimension);

            // Act
            var actualWidth = rectangle.Width;

            // Assert
            Assert.That(actualWidth, Is.EqualTo(expectedWidth));
        }

        [TestCase(0, 0, 1, 1, 1)]
        [TestCase(1.234, -4.321, 12.34, 43.21, 43.21)]
        public void Height(double centerX, double centerY, double dimensionX, double dimensionY, double expectedHeight)
        {
            // Arrange
            var center = new Vector2(centerX, centerY);
            var dimension = new Vector2(dimensionX, dimensionY);

            var rectangle = new Rectangle(center, dimension);

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
            var transform = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);

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
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 1.5, 0, 1, 2, /*E*/ false)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 0, 1.5, 1, 2, /*E*/ false)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 1.5, 1.5, 1, 2, /*E*/ false)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 1.4, 0, 1, 2, /*E*/ true)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 0, 1.4, 1, 2, /*E*/ true)]
        [TestCase( /*R1*/ 0, 0, 2, 1, /*R2*/ 1.4, 1.4, 1, 2, /*E*/ true)]
        public void Overlaps_WithRectangle_AxisAligned(double c1x, double c1y, double w1, double h1, double c2x, double c2y, double w2, double h2,
            bool expected)
        {
            // Arrange
            var rectangle1 = new Rectangle(new Vector2(c1x, c1y), new Vector2(w1, h1));
            var rectangle2 = new Rectangle(new Vector2(c2x, c2y), new Vector2(w2, h2));

            // Act
            var actual1 = rectangle1.Overlaps(rectangle2);
            var actual2 = rectangle2.Overlaps(rectangle1);

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
        }

        #endregion
    }
}