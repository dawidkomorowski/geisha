using Geisha.Common.Math;
using Geisha.Common.Math.Shape;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Geisha.Common.UnitTests.Math.Shape
{
    [TestFixture]
    public class QuadTests
    {
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8)]
        [TestCase(55.708, -40.254, 88.091, 60.153, -16.043, -69.885, -49.055, 12.271)]
        public void Constructor_ShouldSetupVerticesCorrectly(double v1_x, double v1_y, double v2_x, double v2_y,
            double v3_x, double v3_y, double v4_x, double v4_y)
        {
            // Arrange
            var v1 = new Vector2(v1_x, v1_y);
            var v2 = new Vector2(v2_x, v2_y);
            var v3 = new Vector2(v3_x, v3_y);
            var v4 = new Vector2(v4_x, v4_y);

            // Act
            var actual = new Quad(v1, v2, v3, v4);

            // Assert
            Assert.That(actual.V1, Is.EqualTo(v1));
            Assert.That(actual.V2, Is.EqualTo(v2));
            Assert.That(actual.V3, Is.EqualTo(v3));
            Assert.That(actual.V4, Is.EqualTo(v4));
        }

        [TestCase(1, 2, 3, 4, 5, 6, 7, 8)]
        [TestCase(55.708, -40.254, 88.091, 60.153, -16.043, -69.885, -49.055, 12.271)]
        public void Transform_ShouldTransformEachVertexOfQuad(double v1_x, double v1_y, double v2_x, double v2_y,
            double v3_x, double v3_y, double v4_x, double v4_y)
        {
            // Arrange
            var v1 = new Vector2(v1_x, v1_y);
            var v2 = new Vector2(v2_x, v2_y);
            var v3 = new Vector2(v3_x, v3_y);
            var v4 = new Vector2(v4_x, v4_y);

            var quad = new Quad(v1, v2, v3, v4);
            var transform = new Matrix3(1, 2, 3, 4, 5, 6, 7, 8, 9);

            var expectedV1 = (transform * v1.Homogeneous).ToVector2();
            var expectedV2 = (transform * v2.Homogeneous).ToVector2();
            var expectedV3 = (transform * v3.Homogeneous).ToVector2();
            var expectedV4 = (transform * v4.Homogeneous).ToVector2();

            // Act
            var actual = quad.Transform(transform);

            // Assert

            Assert.That(actual.V1, Is.EqualTo(expectedV1));
            Assert.That(actual.V2, Is.EqualTo(expectedV2));
            Assert.That(actual.V3, Is.EqualTo(expectedV3));
            Assert.That(actual.V4, Is.EqualTo(expectedV4));
        }
    }
}