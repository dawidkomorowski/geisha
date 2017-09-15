using System.Collections.Generic;
using Geisha.Common.Geometry;
using Geisha.Common.UnitTests.TestHelpers;
using NUnit.Framework;

namespace Geisha.Framework.Rendering.UnitTests
{
    [TestFixture]
    public class SpriteTests
    {
        private const double Epsilon = 0.000001;
        private static IEqualityComparer<Vector2> Vector2Comparer => VectorEqualityComparer.Vector2(Epsilon);

        [TestCase(200, 100, 100, 50, 1,
            -100, 50, 100, 50, -100, -50, 100, -50)]
        [TestCase(200, 100, 0, 0, 50,
            0, 0, 4, 0, 0, -2, 4, -2)]
        [TestCase(200, 100, 200, 100, 50,
            -4, 2, 0, 2, -4, 0, 0, 0)]
        [TestCase(100, 60, 40, 20, 20,
            -2, 1, 3, 1, -2, -2, 3, -2)]
        [TestCase(70.929, 21.519, 59.861, 11.884, 1.633,
            -36.657072872014, 7.277403551745, 6.777709736676, 7.277403551745, -36.657072872014, -5.900183710961,
            6.777709736676, -5.900183710961)]
        public void Rectangle_ShouldReturnRectangleBasedOnSourceDimensionSourceAnchorAndPixelsPerUnit(double dimX,
            double dimY, double anchorX, double anchorY, double ppu, double expectedULx, double expectedULy,
            double expectedURx, double expectedURy, double expectedLLx, double expectedLLy, double expectedLRx,
            double expectedLRy)
        {
            // Arrange
            var sprite = new Sprite
            {
                SourceDimension = new Vector2(dimX, dimY),
                SourceAnchor = new Vector2(anchorX, anchorY),
                PixelsPerUnit = ppu
            };

            var expectedUpperLeft = new Vector2(expectedULx, expectedULy);
            var expectedUpperRight = new Vector2(expectedURx, expectedURy);
            var expectedLowerLeft = new Vector2(expectedLLx, expectedLLy);
            var expectedLowerRight = new Vector2(expectedLRx, expectedLRy);

            // Act
            var rectangle = sprite.Rectangle;

            // Assert
            Assert.That(rectangle.UpperLeft, Is.EqualTo(expectedUpperLeft).Using(Vector2Comparer));
            Assert.That(rectangle.UpperRight, Is.EqualTo(expectedUpperRight).Using(Vector2Comparer));
            Assert.That(rectangle.LowerLeft, Is.EqualTo(expectedLowerLeft).Using(Vector2Comparer));
            Assert.That(rectangle.LowerRight, Is.EqualTo(expectedLowerRight).Using(Vector2Comparer));
        }
    }
}