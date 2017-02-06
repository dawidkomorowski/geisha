using System;
using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Components
{
    [TestFixture]
    public class TransformTests
    {
        private const double Epsilon = 0.0001;

        [TestCase(0, 0, 0, 0, 0, 0, 1, 1, 1,
            1, 0, 0,
            0, 1, 0,
            0, 0, 1)]
        [TestCase(1, -2, 0, 0, 0, 0, 1, 1, 1,
            1, 0, 1,
            0, 1, -2,
            0, 0, 1)]
        [TestCase(0, 0, 0, 0, 0, Math.PI / 2, 1, 1, 1,
            0, -1, 0,
            1, 0, 0,
            0, 0, 1)]
        [TestCase(0, 0, 0, 0, 0, 0, 2, -3, 1,
            2, 0, 0,
            0, -3, 0,
            0, 0, 1)]
        [TestCase(1, -2, 3, 0, 0, 0, 1, 1, 1,
            1, 0, 1,
            0, 1, -2,
            0, 0, 1)]
        [TestCase(0, 0, 0, Math.PI / 4, Math.PI / 4, Math.PI / 2, 1, 1, 1,
            0, -1, 0,
            1, 0, 0,
            0, 0, 1)]
        [TestCase(0, 0, 0, 0, 0, 0, 2, -3, 4,
            2, 0, 0,
            0, -3, 0,
            0, 0, 1)]
        [TestCase(1, -2, 0, 0, 0, Math.PI / 4, 2, -3, 1,
            1.4142, 2.1213, 1,
            1.4142, -2.1213, -2,
            0, 0, 1)]
        public void Create2DTransformatonMatrix(double tx, double ty, double tz, double rx, double ry, double rz,
            double sx, double sy, double sz, double m11, double m12, double m13, double m21, double m22, double m23,
            double m31, double m32, double m33)
        {
            // Arrange
            var transform = new Transform
            {
                Translation = new Vector3(tx, ty, tz),
                Rotation = new Vector3(rx, ry, rz),
                Scale = new Vector3(sx, sy, sz)
            };

            // Act
            var matrix = transform.Create2DTransformationMatrix();

            // Assert
            Assert.That(matrix.M11, Is.EqualTo(m11).Within(Epsilon));
            Assert.That(matrix.M12, Is.EqualTo(m12).Within(Epsilon));
            Assert.That(matrix.M13, Is.EqualTo(m13).Within(Epsilon));

            Assert.That(matrix.M21, Is.EqualTo(m21).Within(Epsilon));
            Assert.That(matrix.M22, Is.EqualTo(m22).Within(Epsilon));
            Assert.That(matrix.M23, Is.EqualTo(m23).Within(Epsilon));

            Assert.That(matrix.M31, Is.EqualTo(m31).Within(Epsilon));
            Assert.That(matrix.M32, Is.EqualTo(m32).Within(Epsilon));
            Assert.That(matrix.M33, Is.EqualTo(m33).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 1, 1, 1,
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1)]
        [TestCase(1, -2, 3, 0, 0, 0, 1, 1, 1,
            1, 0, 0, 1,
            0, 1, 0, -2,
            0, 0, 1, 3,
            0, 0, 0, 1)]
        [TestCase(0, 0, 0, 1, -2, 3, 1, 1, 1,
            0.3040, 0.8162, -0.4913, 0,
            0.0762, -0.5349, -0.8415, 0,
            -0.9496, 0.2184, -0.2248, 0,
            0, 0, 0, 1)]
        [TestCase(0, 0, 0, 0, 0, 0, 2, -3, 4,
            2, 0, 0, 0,
            0, -3, 0, 0,
            0, 0, 4, 0,
            0, 0, 0, 1)]
        [TestCase(1, -2, 3, 1, -2, 3, 2, -3, 4,
            0.6080, -2.4487, -1.9652, 1,
            0.1525, 1.6047, -3.3659, -2,
            -1.8992, -0.6551, -0.8994, 3,
            0, 0, 0, 1)]
        public void Create3DTransformatonMatrix(double tx, double ty, double tz, double rx, double ry, double rz,
            double sx, double sy, double sz, double m11, double m12, double m13, double m14, double m21, double m22,
            double m23, double m24, double m31, double m32, double m33, double m34, double m41, double m42, double m43,
            double m44)
        {
            // Arrange
            var transform = new Transform
            {
                Translation = new Vector3(tx, ty, tz),
                Rotation = new Vector3(rx, ry, rz),
                Scale = new Vector3(sx, sy, sz)
            };

            // Act
            var matrix = transform.Create3DTransformationMatrix();

            // Assert
            Assert.That(matrix.M11, Is.EqualTo(m11).Within(Epsilon));
            Assert.That(matrix.M12, Is.EqualTo(m12).Within(Epsilon));
            Assert.That(matrix.M13, Is.EqualTo(m13).Within(Epsilon));
            Assert.That(matrix.M14, Is.EqualTo(m14).Within(Epsilon));

            Assert.That(matrix.M21, Is.EqualTo(m21).Within(Epsilon));
            Assert.That(matrix.M22, Is.EqualTo(m22).Within(Epsilon));
            Assert.That(matrix.M23, Is.EqualTo(m23).Within(Epsilon));
            Assert.That(matrix.M24, Is.EqualTo(m24).Within(Epsilon));

            Assert.That(matrix.M31, Is.EqualTo(m31).Within(Epsilon));
            Assert.That(matrix.M32, Is.EqualTo(m32).Within(Epsilon));
            Assert.That(matrix.M33, Is.EqualTo(m33).Within(Epsilon));
            Assert.That(matrix.M34, Is.EqualTo(m34).Within(Epsilon));

            Assert.That(matrix.M41, Is.EqualTo(m41).Within(Epsilon));
            Assert.That(matrix.M42, Is.EqualTo(m42).Within(Epsilon));
            Assert.That(matrix.M43, Is.EqualTo(m43).Within(Epsilon));
            Assert.That(matrix.M44, Is.EqualTo(m44).Within(Epsilon));
        }
    }
}