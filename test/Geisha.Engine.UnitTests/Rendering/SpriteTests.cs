using System.Collections.Generic;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering
{
    [TestFixture]
    public class SpriteTests
    {
        private const double Epsilon = 0.000001;
        private static IEqualityComparer<Vector2> Vector2Comparer => CommonEqualityComparer.Vector2(Epsilon);

        [TestCase(200, 100, 100, 50, 1,
            0, 0, 200, 100)]
        [TestCase(200, 100, 0, 0, 50,
            2, -1, 4, 2)]
        [TestCase(200, 100, 200, 100, 50,
            -2, 1, 4, 2)]
        [TestCase(100, 60, 40, 20, 20,
            0.5, -0.5, 5, 3)]
        [TestCase(70.929, 21.519, 59.861, 11.884, 1.633,
            -14.939681, 0.688609, 43.434782, 13.177587)]
        public void Rectangle_ShouldReturnRectangleDerivedFrom_SourceDimensions_Pivot_And_PixelsPerUnit(double dimX, double dimY, double pivotX, double pivotY,
            double ppu, double expectedCenterX, double expectedCenterY, double expectedWidth, double expectedHeight)
        {
            // Arrange
            var texture = Substitute.For<ITexture>();
            var sprite = new Sprite(
                sourceTexture: texture,
                sourceUV: Vector2.Zero,
                sourceDimensions: new Vector2(dimX, dimY),
                pivot: new Vector2(pivotX, pivotY),
                pixelsPerUnit: ppu);

            var expectedCenter = new Vector2(expectedCenterX, expectedCenterY);
            var expectedDimensions = new Vector2(expectedWidth, expectedHeight);

            // Act
            var rectangle = sprite.Rectangle;

            // Assert
            Assert.That(rectangle.Center, Is.EqualTo(expectedCenter).Using(Vector2Comparer));
            Assert.That(rectangle.Dimensions, Is.EqualTo(expectedDimensions).Using(Vector2Comparer));
        }
    }
}