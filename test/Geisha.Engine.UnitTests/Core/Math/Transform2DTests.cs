using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;
using System.Collections.Generic;

namespace Geisha.Engine.UnitTests.Core.Math
{
    [TestFixture]
    [DefaultFloatingPointTolerance(Epsilon)]
    public class Transform2DTests
    {
        private const double Epsilon = 0.0001;
        private static IEqualityComparer<Vector2> Vector2Comparer => CommonEqualityComparer.Vector2(Epsilon);

        [Test]
        public void Constructor_ShouldCreateTransformWithZeroTranslationZeroRotationZeroScale_GivenNoArguments()
        {
            // Arrange
            // Act
            var actual = new Transform2D();

            // Assert
            Assert.That(actual.Translation, Is.EqualTo(Vector2.Zero));
            Assert.That(actual.Rotation, Is.Zero);
            Assert.That(actual.Scale, Is.EqualTo(Vector2.Zero));
        }

        [Test]
        public void Constructor_ShouldCreateTransform_GivenTranslationRotationScale()
        {
            // Arrange
            var translation = new Vector2(1, 2);
            var rotation = 3;
            var scale = new Vector2(4, 5);

            // Act
            var actual = new Transform2D(translation, rotation, scale);

            // Assert
            Assert.That(actual.Translation, Is.EqualTo(translation));
            Assert.That(actual.Rotation, Is.EqualTo(rotation));
            Assert.That(actual.Scale, Is.EqualTo(scale));
        }

        [Test]
        public void Identity_ShouldReturnTransformWithZeroTranslationZeroRotationAndScaleOfOne()
        {
            // Arrange
            // Act
            var actual = Transform2D.Identity;

            // Assert
            Assert.That(actual.Translation, Is.EqualTo(Vector2.Zero));
            Assert.That(actual.Rotation, Is.Zero);
            Assert.That(actual.Scale, Is.EqualTo(Vector2.One));
        }

        [TestCase(0, 1, 0)]
        [TestCase(System.Math.PI / 2, 0, 1)]
        [TestCase(-3.3659, -0.97495, 0.22243)]
        public void VectorX_ShouldReturnUnitVectorInGlobalCoordinatesSystemPointingAlongXAxisOfLocalCoordinatesSystem(double r, double vx, double vy)
        {
            // Arrange
            var transform = new Transform2D
            {
                Rotation = r
            };

            // Act
            var actual = transform.VectorX;

            // Assert
            Assert.That(actual, Is.EqualTo(new Vector2(vx, vy)).Using(Vector2Comparer));
        }

        [TestCase(0, 0, 1)]
        [TestCase(System.Math.PI / 2, -1, 0)]
        [TestCase(-3.3659, -0.22243, -0.97495)]
        public void VectorY_ShouldReturnUnitVectorInGlobalCoordinatesSystemPointingAlongYAxisOfLocalCoordinatesSystem(double r, double vx, double vy)
        {
            // Arrange
            var transform = new Transform2D
            {
                Rotation = r
            };

            // Act
            var actual = transform.VectorY;

            // Assert
            Assert.That(actual, Is.EqualTo(new Vector2(vx, vy)).Using(Vector2Comparer));
        }

        [TestCase(1, 2, 3, 4, 5,
            1, 1, -2, -2, 3,
            0,
            1, 2, 3, 4, 5)]
        [TestCase(1, 2, 3, 4, 5,
            1, 1, -2, -2, 3,
            1,
            1, 1, -2, -2, 3)]
        [TestCase(1, 2, 3, 4, 5,
            1, 1, -2, -2, 3,
            0.5,
            1, 1.5, 0.5, 1, 4)]
        [TestCase(1, 2, 3, 4, 5,
            1, 1, -2, -2, 3,
            0.25,
            1, 1.75, 1.75, 2.5, 4.5)]
        public void Lerp_Test(
            double tx1, double ty1, double r1, double sx1, double sy1,
            double tx2, double ty2, double r2, double sx2, double sy2,
            double alpha,
            double tx3, double ty3, double r3, double sx3, double sy3
        )
        {
            // Arrange
            var t1 = new Transform2D(new Vector2(tx1, ty1), r1, new Vector2(sx1, sy1));
            var t2 = new Transform2D(new Vector2(tx2, ty2), r2, new Vector2(sx2, sy2));

            // Act
            var t3 = Transform2D.Lerp(t1, t2, alpha);

            // Assert
            Assert.That(t3.Translation, Is.EqualTo(new Vector2(tx3, ty3)));
            Assert.That(t3.Rotation, Is.EqualTo(r3));
            Assert.That(t3.Scale, Is.EqualTo(new Vector2(sx3, sy3)));
        }

        [TestCase(0, 0, 0, 1, 1,
            1, 0, 0,
            0, 1, 0,
            0, 0, 1)]
        [TestCase(1, -2, 0, 1, 1,
            1, 0, 1,
            0, 1, -2,
            0, 0, 1)]
        [TestCase(0, 0, System.Math.PI / 2, 1, 1,
            0, -1, 0,
            1, 0, 0,
            0, 0, 1)]
        [TestCase(0, 0, 0, 2, -3,
            2, 0, 0,
            0, -3, 0,
            0, 0, 1)]
        [TestCase(1, -2, System.Math.PI / 4, 2, -3,
            1.4142, 2.1213, 1,
            1.4142, -2.1213, -2,
            0, 0, 1)]
        public void ToMatrix_ShouldReturnMatrixRepresentingThisTransform(
            double tx, double ty, double r, double sx, double sy,
            double m11, double m12, double m13,
            double m21, double m22, double m23,
            double m31, double m32, double m33)
        {
            // Arrange
            var transform = new Transform2D(new Vector2(tx, ty), r, new Vector2(sx, sy));

            // Act
            var matrix = transform.ToMatrix();

            // Assert
            Assert.That(matrix.M11, Is.EqualTo(m11));
            Assert.That(matrix.M12, Is.EqualTo(m12));
            Assert.That(matrix.M13, Is.EqualTo(m13));

            Assert.That(matrix.M21, Is.EqualTo(m21));
            Assert.That(matrix.M22, Is.EqualTo(m22));
            Assert.That(matrix.M23, Is.EqualTo(m23));

            Assert.That(matrix.M31, Is.EqualTo(m31));
            Assert.That(matrix.M32, Is.EqualTo(m32));
            Assert.That(matrix.M33, Is.EqualTo(m33));
        }

        [TestCase(0, 0, 0, 0, 0, "Translation: X: 0, Y: 0, Rotation: 0, Scale: X: 0, Y: 0")]
        [TestCase(1.2, -3.4, 5.6, -7.8, 9.0, "Translation: X: 1.2, Y: -3.4, Rotation: 5.6, Scale: X: -7.8, Y: 9")]
        [SetCulture("")]
        public void ToString_Test(double tx, double ty, double r, double sx, double sy, string expected)
        {
            // Arrange
            var transform = new Transform2D(new Vector2(tx, ty), r, new Vector2(sx, sy));

            // Act
            var actual = transform.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(1, 2, 3, 4, 5, 1, 2, 3, 4, 5, true)]
        [TestCase(1, 2, 3, 4, 5, 0, 2, 3, 4, 5, false)]
        [TestCase(1, 2, 3, 4, 5, 1, 0, 3, 4, 5, false)]
        [TestCase(1, 2, 3, 4, 5, 1, 2, 0, 4, 5, false)]
        [TestCase(1, 2, 3, 4, 5, 1, 2, 3, 0, 5, false)]
        [TestCase(1, 2, 3, 4, 5, 1, 2, 3, 4, 0, false)]
        public void EqualityMembers_ShouldEqualTransform2D_WhenComponentsAreEqual(
            double tx1, double ty1, double r1, double sx1, double sy1,
            double tx2, double ty2, double r2, double sx2, double sy2,
            bool expectedIsEqual
        )
        {
            // Arrange
            var t1 = new Transform2D(new Vector2(tx1, ty1), r1, new Vector2(sx1, sy1));
            var t2 = new Transform2D(new Vector2(tx2, ty2), r2, new Vector2(sx2, sy2));

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(t1, t2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }
    }
}