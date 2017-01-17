﻿using System.Collections.Generic;
using Geisha.Common.Geometry;
using Geisha.Common.Geometry.Shape;
using Geisha.Common.UnitTests.TestHelpers;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Geometry.Shape
{
    [TestFixture]
    public class RectangleTests
    {
        private const double Epsilon = 0.000001;
        private IEqualityComparer<Vector2> Vector2Comparer => VectorEqualityComparer.Vector2(Epsilon);

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

        [TestCase(1, 1)]
        [TestCase(47.196, 75.639)]
        public void Transform_ShouldTransformEachVertexOfRectangle(double dimX, double dimY)
        {
            // Arrange
            var dimension = new Vector2(dimX, dimY);

            var rectangle = new Rectangle(dimension);
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
    }
}