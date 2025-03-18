using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math
{
    [TestFixture]
    [DefaultFloatingPointTolerance(Epsilon)]
    public class RectangleTests
    {
        private const double Epsilon = 1e-6;
        private static IEqualityComparer<Vector2> Vector2Comparer => CommonEqualityComparer.Vector2(Epsilon);

        #region Constructors

        [TestCase(1, 1, -0.5, 0.5, 0.5, 0.5, -0.5, -0.5, 0.5, -0.5)]
        [TestCase(47.196, 75.639, -23.598, 37.8195, 23.598, 37.8195, -23.598, -37.8195, 23.598, -37.8195)]
        public void Constructor_FromDimensions_ShouldSetupVerticesCorrectly(double width, double height, double expectedULx, double expectedULy,
            double expectedURx, double expectedURy, double expectedLLx, double expectedLLy, double expectedLRx, double expectedLRy)
        {
            // Arrange
            var dimensions = new Vector2(width, height);

            var expectedUpperLeft = new Vector2(expectedULx, expectedULy);
            var expectedUpperRight = new Vector2(expectedURx, expectedURy);
            var expectedLowerLeft = new Vector2(expectedLLx, expectedLLy);
            var expectedLowerRight = new Vector2(expectedLRx, expectedLRy);

            // Act
            var rectangle = new Rectangle(dimensions);

            // Assert
            Assert.That(rectangle.UpperLeft, Is.EqualTo(expectedUpperLeft));
            Assert.That(rectangle.UpperRight, Is.EqualTo(expectedUpperRight));
            Assert.That(rectangle.LowerLeft, Is.EqualTo(expectedLowerLeft));
            Assert.That(rectangle.LowerRight, Is.EqualTo(expectedLowerRight));
        }

        [TestCase(1, 1, 1, 1, 0.5, 1.5, 1.5, 1.5, 0.5, 0.5, 1.5, 0.5)]
        [TestCase(4.928, -34.791, 47.196, 75.639, -18.67, 3.0285, 28.526, 3.0285, -18.67, -72.6105, 28.526, -72.6105)]
        public void Constructor_FromCenterAndDimensions_ShouldSetupVerticesCorrectly(double centerX, double centerY, double width, double height,
            double expectedULx, double expectedULy, double expectedURx, double expectedURy, double expectedLLx, double expectedLLy, double expectedLRx,
            double expectedLRy)
        {
            // Arrange
            var center = new Vector2(centerX, centerY);
            var dimensions = new Vector2(width, height);

            var expectedUpperLeft = new Vector2(expectedULx, expectedULy);
            var expectedUpperRight = new Vector2(expectedURx, expectedURy);
            var expectedLowerLeft = new Vector2(expectedLLx, expectedLLy);
            var expectedLowerRight = new Vector2(expectedLRx, expectedLRy);

            // Act
            var rectangle = new Rectangle(center, dimensions);

            // Assert
            Assert.That(rectangle.UpperLeft, Is.EqualTo(expectedUpperLeft).Using(Vector2Comparer));
            Assert.That(rectangle.UpperRight, Is.EqualTo(expectedUpperRight).Using(Vector2Comparer));
            Assert.That(rectangle.LowerLeft, Is.EqualTo(expectedLowerLeft).Using(Vector2Comparer));
            Assert.That(rectangle.LowerRight, Is.EqualTo(expectedLowerRight).Using(Vector2Comparer));
        }

        #endregion

        #region Properties

        [TestCase(0, 0, 1, 1, 0)]
        [TestCase(1.234, -4.321, 12.34, 43.21, 0)]
        [TestCase(1.234, -4.321, 12.34, 43.21, 45)]
        [TestCase(1.234, -4.321, 12.34, 43.21, 90)]
        [TestCase(1.234, -4.321, 12.34, 43.21, 180)]
        public void Center_Test(double centerX, double centerY, double width, double height, double rotation)
        {
            // Arrange
            var center = new Vector2(centerX, centerY);
            var dimensions = new Vector2(width, height);

            // We want to rotate around center of rectangle thus we need to transform by center after rotation.
            var rectangle = rotation == 0
                ? new Rectangle(center, dimensions)
                : new Rectangle(dimensions).Transform(Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation))).Transform(Matrix3x3.CreateTranslation(center));

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
        public void Width_Test(double centerX, double centerY, double width, double height, double rotation, double expectedWidth)
        {
            // Arrange
            var center = new Vector2(centerX, centerY);
            var dimensions = new Vector2(width, height);

            var rectangle = new Rectangle(center, dimensions).Transform(Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation)));

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
        public void Height_Test(double centerX, double centerY, double width, double height, double rotation, double expectedHeight)
        {
            // Arrange
            var center = new Vector2(centerX, centerY);
            var dimensions = new Vector2(width, height);

            var rectangle = new Rectangle(center, dimensions).Transform(Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation)));

            // Act
            var actualHeight = rectangle.Height;

            // Assert
            Assert.That(actualHeight, Is.EqualTo(expectedHeight));
        }

        #endregion

        #region Methods

        [TestCase(1, 1)]
        [TestCase(47.196, 75.639)]
        public void Transform_ShouldTransformEachVertexOfRectangle(double width, double height)
        {
            // Arrange
            var rectangle = new Rectangle(new Vector2(width, height));
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

        [TestCase( /*R*/ 0, 0, 10, 5, 0, /*P*/ 10, 0, /*E*/ false)]
        [TestCase( /*R*/ 0, 0, 10, 5, 0, /*P*/ 0, 5, /*E*/ false)]
        [TestCase( /*R*/ 0, 0, 10, 5, 0, /*P*/ 5, 0, /*E*/ true)]
        [TestCase( /*R*/ 0, 0, 10, 5, 0, /*P*/ 0, 2.5, /*E*/ true)]
        [TestCase( /*R*/ 0, 0, 10, 5, 0, /*P*/ 5, 2.5, /*E*/ true)]
        [TestCase( /*R*/ 0, 0, 10, 5, 0, /*P*/ 0, 0, /*E*/ true)]
        [TestCase( /*R*/ 0, 0, 10, 5, 45, /*P*/ 3.7, -0.5, /*E*/ false)]
        [TestCase( /*R*/ 0, 0, 10, 5, 45, /*P*/ 2.6, 0.3, /*E*/ true)]
        public void Contains(double rx, double ry, double rw, double rh, double rotation, double px, double py, bool expected)
        {
            // Arrange
            var rotationMatrix = Matrix3x3.CreateTranslation(new Vector2(rx, ry)) *
                                 Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation)) *
                                 Matrix3x3.CreateTranslation(new Vector2(-rx, -ry));

            var rectangle = new Rectangle(new Vector2(rx, ry), new Vector2(rw, rh)).Transform(rotationMatrix);
            var point = new Vector2(px, py);

            // Act
            var actual = rectangle.Contains(point);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        // Horizontal
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 20, 0, 10, 5, 0, /*E*/ false)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 10, 0, 10, 5, 0, /*E*/ true)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 9, 0, 10, 5, 0, /*E*/ true)]
        // Vertical
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 0, 10, 10, 5, 0, /*E*/ false)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 0, 5, 10, 5, 0, /*E*/ true)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 0, 4, 10, 5, 0, /*E*/ true)]
        // Diagonal
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 20, 10, 10, 5, 0, /*E*/ false)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 10, 5, 10, 5, 0, /*E*/ true)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 9, 4, 10, 5, 0, /*E*/ true)]
        // Rectangle inside Rectangle
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 0, 0, 4, 2, 0, /*E*/ true)]
        // Rotated
        [TestCase( /*R1*/ 0, 0, 10, 10, 45, /*R2*/ 14.5, 0, 10, 10, 45, /*E*/ false)]
        [TestCase( /*R1*/ 0, 0, 10, 10, 45, /*R2*/ 9, 0, 10, 10, 45, /*E*/ true)]
        [TestCase( /*R1*/ 0, 0, 10, 10, 45, /*R2*/ 9, 5.5, 10, 10, 45, /*E*/ false)]
        [TestCase( /*R1*/ 174, 110, 100, 100, 102, /*R2*/ 271, 187, 100, 100, 44, /*E*/ false)]
        [TestCase( /*R1*/ 174, 110, 100, 100, 102, /*R2*/ 271, 187, 100, 100, 56, /*E*/ true)]
        public void Overlaps_Rectangle_ShouldReturnTrue_WhenRectanglesOverlap(
            double x1, double y1, double w1, double h1, double rotation1,
            double x2, double y2, double w2, double h2, double rotation2,
            bool expected
        )
        {
            // Arrange
            var rotationMatrix1 = Matrix3x3.CreateTranslation(new Vector2(x1, y1)) *
                                  Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation1)) *
                                  Matrix3x3.CreateTranslation(new Vector2(-x1, -y1));

            var rotationMatrix2 = Matrix3x3.CreateTranslation(new Vector2(x2, y2)) *
                                  Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation2)) *
                                  Matrix3x3.CreateTranslation(new Vector2(-x2, -y2));

            var rectangle1 = new Rectangle(new Vector2(x1, y1), new Vector2(w1, h1)).Transform(rotationMatrix1);
            var rectangle2 = new Rectangle(new Vector2(x2, y2), new Vector2(w2, h2)).Transform(rotationMatrix2);

            // Act
            var actual1 = rectangle1.Overlaps(rectangle2);
            var actual2 = rectangle2.Overlaps(rectangle1);

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
        }

        // Horizontal
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 20, 0, 10, 5, 0, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 10, 0, 10, 5, 0, /*E*/ true, -1, 0, 0)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 9, 0, 10, 5, 0, /*E*/ true, -1, 0, 1)]
        // Vertical
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 0, 10, 10, 5, 0, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 0, 5, 10, 5, 0, /*E*/ true, 0, -1, 0)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 0, 4, 10, 5, 0, /*E*/ true, 0, -1, 1)]
        // Diagonal
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 20, 10, 10, 5, 0, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 10, 5, 10, 5, 0, /*E*/ true, -1, 0, 0)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 9, 4, 10, 5, 0, /*E*/ true, -1, 0, 1)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 8, 4, 10, 5, 0, /*E*/ true, 0, -1, 1)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 9, 3, 10, 5, 0, /*E*/ true, -1, 0, 1)]
        // Rectangle inside Rectangle
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 3, 0, 2, 4, 0, /*E*/ true, -1, 0, 3)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ -3, 0, 2, 4, 0, /*E*/ true, 1, 0, 3)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 2, 1, 4, 2, 0, /*E*/ true, 0, -1, 2.5)]
        [TestCase( /*R1*/ 0, 0, 10, 5, 0, /*R2*/ 2, -1, 4, 2, 0, /*E*/ true, 0, 1, 2.5)]
        // One rotated
        [TestCase( /*R1*/ 0, 0, 10, 10, 0, /*R2*/ 12.5, 0, 10, 10, 45, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R1*/ 0, 0, 10, 10, 0, /*R2*/ 12, 0, 10, 10, 45, /*E*/ true, -1, 0, 0.071067)]
        [TestCase( /*R1*/ 0, 0, 10, 10, 0, /*R2*/ 0, 12.5, 10, 10, 45, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R1*/ 0, 0, 10, 10, 0, /*R2*/ 0, 12, 10, 10, 45, /*E*/ true, 0, -1, 0.071067)]
        // Two rotated
        [TestCase( /*R1*/ 0, 0, 10, 10, 45, /*R2*/ 14.5, 0, 10, 10, 45, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R1*/ 0, 0, 10, 10, 45, /*R2*/ 9, 0, 10, 10, 45, /*E*/ true, -0.707106, 0.707106, 3.636038)]
        [TestCase( /*R1*/ 0, 0, 10, 10, 45, /*R2*/ 10, 1, 10, 10, 45, /*E*/ true, -0.707106, -0.707106, 2.221825)]
        [TestCase( /*R1*/ 0, 0, 10, 10, 45, /*R2*/ 10, -1, 10, 10, 45, /*E*/ true, -0.707106, 0.707106, 2.221825)]
        [TestCase( /*R1*/ 0, 0, 10, 10, 45, /*R2*/ 9, 5.5, 10, 10, 45, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R1*/ 174, 110, 100, 100, 102, /*R2*/ 271, 187, 100, 100, 44, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R1*/ 174, 110, 100, 100, 102, /*R2*/ 271, 187, 100, 100, 56, /*E*/ true, -0.559192, -0.829037, 2.622303)]
        public void Overlaps_Rectangle_MTV_ShouldReturnTrueAndMTV_WhenRectanglesOverlap(
            double x1, double y1, double w1, double h1, double rotation1,
            double x2, double y2, double w2, double h2, double rotation2,
            bool overlap, double mtvX, double mtvY, double mtvLength
        )
        {
            // Arrange
            var rotationMatrix1 = Matrix3x3.CreateTranslation(new Vector2(x1, y1)) *
                                  Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation1)) *
                                  Matrix3x3.CreateTranslation(new Vector2(-x1, -y1));

            var rotationMatrix2 = Matrix3x3.CreateTranslation(new Vector2(x2, y2)) *
                                  Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation2)) *
                                  Matrix3x3.CreateTranslation(new Vector2(-x2, -y2));

            var rectangle1 = new Rectangle(new Vector2(x1, y1), new Vector2(w1, h1)).Transform(rotationMatrix1);
            var rectangle2 = new Rectangle(new Vector2(x2, y2), new Vector2(w2, h2)).Transform(rotationMatrix2);

            // Act
            var actual1 = rectangle1.Overlaps(rectangle2, out var mtv1);
            var actual2 = rectangle2.Overlaps(rectangle1, out var mtv2);

            // Assert
            Assert.That(actual1, Is.EqualTo(overlap));
            Assert.That(actual2, Is.EqualTo(overlap));

            Assert.That(mtv1.Direction, Is.EqualTo(new Vector2(mtvX, mtvY)).Using(Vector2Comparer));
            Assert.That(mtv1.Length, Is.EqualTo(mtvLength));

            Assert.That(mtv2.Direction, Is.EqualTo(new Vector2(mtvX, mtvY).Opposite).Using(Vector2Comparer));
            Assert.That(mtv2.Length, Is.EqualTo(mtvLength));
        }

        [Test]
        public void Overlaps_Rectangle_MTV_ShouldReturnTrueAndMTV_WhenRectanglesHaveTheSameCenter()
        {
            // Arrange
            var rectangle1 = new Rectangle(new Vector2(20, 20));
            var rectangle2 = new Rectangle(new Vector2(10, 10));

            // Act
            var actual1 = rectangle1.Overlaps(rectangle2, out var mtv1);
            var actual2 = rectangle2.Overlaps(rectangle1, out var mtv2);

            // Assert
            Assert.That(actual1, Is.True);
            Assert.That(actual2, Is.True);

            Assert.That(mtv1.Direction, Is.EqualTo(Vector2.UnitX.Opposite));
            Assert.That(mtv1.Length, Is.EqualTo(15));

            Assert.That(mtv2.Direction, Is.EqualTo(Vector2.UnitX.Opposite));
            Assert.That(mtv2.Length, Is.EqualTo(15));
        }

        // Circle outside of Rectangle
        [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 70, 20, 10, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ -10, 20, 10, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 30, 50, 10, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R*/ 30, 20, 40, 20, 0, /*C*/ 30, -10, 10, /*E*/ false, 0, 0, 0)]
        // Circle touching edge of Rectangle
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 6, 2, 1, /*E*/ true, 0, 0, 0, TestName = "AAA")]
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 0, 2, 1, /*E*/ true, 0, 0, 0)]
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 3, 4, 1, /*E*/ true, 0, 0, 0)]
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 3, 0, 1, /*E*/ true, 0, 0, 0)]
        // Circle contains vertex of Rectangle
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 5.25, 3.25, 0.5, /*E*/ true, 0, 0, 0)]
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 0.75, 3.25, 0.5, /*E*/ true, 0, 0, 0)]
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 0.75, 0.75, 0.5, /*E*/ true, 0, 0, 0)]
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 5.25, 0.75, 0.5, /*E*/ true, 0, 0, 0)]
        // Circle overlaps edge of Rectangle
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 5.25, 2, 0.5, /*E*/ true, 0, 0, 0)]
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 0.75, 2, 0.5, /*E*/ true, 0, 0, 0)]
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 3, 3.25, 0.5, /*E*/ true, 0, 0, 0)]
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 3, 0.75, 0.5, /*E*/ true, 0, 0, 0)]
        // Circle outside of Rectangle but with overlapping projection onto Rectangle axes
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 5.8, 3.8, 1, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 0.2, 3.8, 1, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 0.2, 0.2, 1, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 5.8, 0.2, 1, /*E*/ false, 0, 0, 0)]
        // Circle inside of Rectangle
        [TestCase( /*R*/ 3, 2, 4, 2, 0, /*C*/ 3, 2, 0.5, /*E*/ true, 0, 0, 0)]
        // Circle and rotated Rectangle
        [TestCase( /*R*/ 10, 20, 100, 50, 45, /*C*/ 65, 75, 25, /*E*/ false, 0, 0, 0)]
        [TestCase( /*R*/ 10, 20, 100, 50, 45, /*C*/ 60, 70, 25, /*E*/ true, 0, 0, 0)]
        [TestCase( /*R*/ 10, 20, 100, 50, 45, /*C*/ 85, 40, 25, /*E*/ true, 0, 0, 0)]
        [TestCase( /*R*/ 10, 20, 100, 50, 45, /*C*/ 50, -15, 25, /*E*/ false, 0, 0, 0)]
        public void Overlaps_Circle_ShouldReturnTrueAndMTV_WhenRectangleAndCircleOverlap(
            double rx, double ry, double rw, double rh, double rotation,
            double cx, double cy, double cr,
            bool overlap, double mtvX, double mtvY, double mtvLength
        )
        {
            // Arrange
            var rotationMatrix = Matrix3x3.CreateTranslation(new Vector2(rx, ry)) *
                                 Matrix3x3.CreateRotation(Angle.Deg2Rad(rotation)) *
                                 Matrix3x3.CreateTranslation(new Vector2(-rx, -ry));

            var rectangle = new Rectangle(new Vector2(rx, ry), new Vector2(rw, rh)).Transform(rotationMatrix);
            var circle = new Circle(new Vector2(cx, cy), cr);

            using var visualOutput = TestKit.CreateVisualOutput(2);
            visualOutput.DrawCircle(circle, Color.Red);
            visualOutput.DrawRectangle(rectangle, Color.Blue);
            visualOutput.SaveToFile();

            // Act
            var actual1 = rectangle.Overlaps(circle);
            var actual2 = circle.Overlaps(rectangle);
            var actual3 = rectangle.Overlaps(circle, out var mtv1);
            var actual4 = circle.Overlaps(rectangle, out var mtv2);

            // Assert
            Assert.That(actual1, Is.EqualTo(overlap));
            Assert.That(actual2, Is.EqualTo(overlap));
            Assert.That(actual3, Is.EqualTo(overlap));
            Assert.That(actual4, Is.EqualTo(overlap));

            Assert.That(mtv1.Direction, Is.EqualTo(new Vector2(mtvX, mtvY)).Using(Vector2Comparer));
            Assert.That(mtv1.Length, Is.EqualTo(mtvLength));

            Assert.That(mtv2.Direction, Is.EqualTo(new Vector2(mtvX, mtvY).Opposite).Using(Vector2Comparer));
            Assert.That(mtv2.Length, Is.EqualTo(mtvLength));
        }

        [Test]
        public void GetBoundingRectangle_ShouldReturnMinimalAxisAlignedRectangleContainingThisRectangle_WhenRectangleIsAxisAligned()
        {
            // Arrange
            var rectangle = new Rectangle(new Vector2(4, 2), new Vector2(10, 6));

            // Act
            var boundingRectangle = rectangle.GetBoundingRectangle();

            // Assert
            Assert.That(boundingRectangle.Center, Is.EqualTo(new Vector2(4, 2)));
            Assert.That(boundingRectangle.Dimensions, Is.EqualTo(new Vector2(10, 6)));
        }

        [Test]
        public void GetBoundingRectangle_ShouldReturnMinimalAxisAlignedRectangleContainingThisRectangle_WhenRectangleIsNotAxisAligned()
        {
            // Arrange
            var rectangle = new Rectangle(new Vector2(4, 2), new Vector2(10, 6))
                .Transform(Matrix3x3.CreateRotation(Angle.Deg2Rad(30)));

            // Act
            var boundingRectangle = rectangle.GetBoundingRectangle();

            // Assert
            Assert.That(boundingRectangle.Center, Is.EqualTo(new Vector2(2.464101, 3.732050)).Using(Vector2Comparer));
            Assert.That(boundingRectangle.Dimensions, Is.EqualTo(new Vector2(11.660254, 10.196152)).Using(Vector2Comparer));
        }

        [TestCase(0, 0, 20, 10)]
        [TestCase(12, -34, 56, 78)]
        public void WriteVertices_ShouldWriteVerticesIntoSpan(double x, double y, double w, double h)
        {
            // Arrange
            var rectangle = new Rectangle(new Vector2(x, y), new Vector2(w, h));
            Span<Vector2> span = stackalloc Vector2[4];

            // Act
            rectangle.WriteVertices(span);

            // Assert
            Assert.That(span[0], Is.EqualTo(rectangle.LowerLeft));
            Assert.That(span[1], Is.EqualTo(rectangle.LowerRight));
            Assert.That(span[2], Is.EqualTo(rectangle.UpperRight));
            Assert.That(span[3], Is.EqualTo(rectangle.UpperLeft));
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