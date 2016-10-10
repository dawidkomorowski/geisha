using Geisha.Common.Geometry;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Geometry
{
    [TestFixture]
    public class Vector2Tests
    {
        private const double Epsilon = 0.000001;

        private static bool AreParallel(Vector2 v1, Vector2 v2)
        {
            var factorX1 = v1.X/v1.Length;
            var factorY1 = v1.Y/v1.Length;

            var factorX2 = v2.X / v2.Length;
            var factorY2 = v2.Y / v2.Length;

            return factorX1 == factorX2 && factorY1 == factorY2;
        }

        #region Static properties

        [Test]
        public void Zero()
        {
            // Arrange
            // Act
            var v1 = Vector2.Zero;

            // Assert
            Assert.That(v1.X, Is.Zero);
            Assert.That(v1.Y, Is.Zero);
        }

        [Test]
        public void One()
        {
            // Arrange
            // Act
            var v1 = Vector2.One;

            // Assert
            Assert.That(v1.X, Is.EqualTo(1));
            Assert.That(v1.Y, Is.EqualTo(1));
        }

        #endregion

        #region Properties

        [TestCase(0, 0, 0)]
        [TestCase(5, 0, 5)]
        [TestCase(0, 3.14, 3.14)]
        [TestCase(3, 4, 5)]
        [TestCase(46.294, 54.684, 71.64826789253177)]
        public void Length(double x1, double y1, double expected)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var actual = v1.Length;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(2.51, 0)]
        [TestCase(0, 1.44)]
        [TestCase(-3, 4)]
        [TestCase(-0.54, -0.065)]
        [TestCase(89.727, 59.751)]
        public void Unit(double x1, double y1)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var actualVector = v1.Unit;
            var actualLength = actualVector.Length;

            // Assert
            Assert.That(actualLength, Is.EqualTo(1));
            Assert.That(AreParallel(v1, actualVector), Is.True);
        }

        [TestCase(2.51, 0, -2.51, 0)]
        [TestCase(0, 1.44, 0, -1.44)]
        [TestCase(-3, 4, 3, -4)]
        [TestCase(-0.54, -0.065, 0.54, 0.065)]
        [TestCase(89.727, 59.751, -89.727, -59.751)]
        public void Opposite(double x1, double y1, double x2, double y2)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var actual = v1.Opposite;

            // Assert
            Assert.That(actual.X, Is.EqualTo(x2));
            Assert.That(actual.Y, Is.EqualTo(y2));
        }

        [TestCase(2.51, 0)]
        [TestCase(0, 1.44)]
        [TestCase(-3, 4)]
        [TestCase(-0.54, -0.065)]
        [TestCase(89.727, 59.751)]
        public void Array(double x1, double y1)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var actual = v1.Array;

            // Assert
            Assert.That(actual[0], Is.EqualTo(x1));
            Assert.That(actual[1], Is.EqualTo(y1));
        }

        #endregion

        #region Methods

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(0, 0, 5, 3, 5, 3)]
        [TestCase(2, 4, -3, -3, -1, 1)]
        [TestCase(-7, 3, 7, -3, 0, 0)]
        [TestCase(96.747, 71.087, 65.603, 91.750, 162.35, 162.837)]
        public void Add(double x1, double y1, double x2, double y2, double x3,
            double y3)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            var v3 = v1.Add(v2);

            // Assert
            Assert.That(v3.X, Is.EqualTo(x3));
            Assert.That(v3.Y, Is.EqualTo(y3));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 5, 3, 0, 0)]
        [TestCase(-1, 1, -3, -3, 2, 4)]
        [TestCase(0, 0, 7, -3, -7, 3)]
        [TestCase(20.069, 46.724, 74.225, 18.948, -54.156, 27.776)]
        public void Subtract(double x1, double y1, double x2, double y2, double x3,
            double y3)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            var v3 = v1.Subtract(v2);

            // Assert
            Assert.That(v3.X, Is.EqualTo(x3).Within(Epsilon));
            Assert.That(v3.Y, Is.EqualTo(y3).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0)]
        [TestCase(1, 1, 0, 0, 0)]
        [TestCase(1, 2, 3, 3, 6)]
        [TestCase(5, 0, 2, 10, 0)]
        [TestCase(20, -8, 0.5, 10, -4)]
        [TestCase(89.184, 78.899, 91.237, 8136.880608, 7198.508063)]
        public void Multiply(double x1, double y1, double s, double x2,
            double y2)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var v2 = v1.Multiply(s);

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2).Within(Epsilon));
            Assert.That(v2.Y, Is.EqualTo(y2).Within(Epsilon));
        }

        [TestCase(3, 6, 3, 1, 2)]
        [TestCase(10, 0, 2, 5, 0)]
        [TestCase(10, -4, 0.5, 20, -8)]
        [TestCase(8136.880608, 7198.508063, 91.237, 89.184, 78.899)]
        public void Divide(double x1, double y1, double s, double x2,
            double y2)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var v2 = v1.Divide(s);

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2).Within(Epsilon));
            Assert.That(v2.Y, Is.EqualTo(y2).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0)]
        [TestCase(5, 3, 5, 3, 34)]
        [TestCase(-1, 1, -3, -3, 0)]
        [TestCase(0, 0, 7, -3, 0)]
        [TestCase(20.069, 46.724, 74.225, 18.948, 2374.947877)]
        public void Dot(double x1, double y1, double x2, double y2, double expected)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            var actual = v1.Dot(v2);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0)]
        [TestCase(5, 3, 5, 3, 0)]
        [TestCase(-1, 1, -3, -3, 4.47213595)]
        [TestCase(0, 0, 7, -3, 7.61577310)]
        [TestCase(20.069, 46.724, 74.225, 18.948, 60.86360580)]
        public void Distance(double x1, double y1, double x2, double y2, double expected)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            var actual = v1.Distance(v2);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, true)]
        [TestCase(5, 3, 5, 3, true)]
        [TestCase(-1, 1, -3, -3, false)]
        [TestCase(0, 0, 7, -3, false)]
        [TestCase(60.86360580, 4.47213595, 60.86360580, 4.47213595, true)]
        [TestCase(60.86360580, 4.47213595, 60.86360580, 4.47213596, false)]
        public void Equals(double x1, double y1, double x2, double y2, bool expected)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            var actual1 = v1.Equals(v2);
            var actual2 = v1.Equals((object) v2);

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
        }

        #endregion

        #region Operators

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(0, 0, 5, 3, 5, 3)]
        [TestCase(2, 4, -3, -3, -1, 1)]
        [TestCase(-7, 3, 7, -3, 0, 0)]
        [TestCase(96.747, 71.087, 65.603, 91.750, 162.35, 162.837)]
        public void AdditionOperator(double x1, double y1, double x2, double y2, double x3,
            double y3)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            var v3 = v1 + v2;

            // Assert
            Assert.That(v3.X, Is.EqualTo(x3));
            Assert.That(v3.Y, Is.EqualTo(y3));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 5, 3, 0, 0)]
        [TestCase(-1, 1, -3, -3, 2, 4)]
        [TestCase(0, 0, 7, -3, -7, 3)]
        [TestCase(20.069, 46.724, 74.225, 18.948, -54.156, 27.776)]
        public void SubtractionOperator(double x1, double y1, double x2, double y2, double x3,
            double y3)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            var v3 = v1 - v2;

            // Assert
            Assert.That(v3.X, Is.EqualTo(x3).Within(Epsilon));
            Assert.That(v3.Y, Is.EqualTo(y3).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0)]
        [TestCase(1, 1, 0, 0, 0)]
        [TestCase(1, 2, 3, 3, 6)]
        [TestCase(5, 0, 2, 10, 0)]
        [TestCase(20, -8, 0.5, 10, -4)]
        [TestCase(89.184, 78.899, 91.237, 8136.880608, 7198.508063)]
        public void MultiplicationOperator(double x1, double y1, double s, double x2,
            double y2)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var v2 = v1*s;

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2).Within(Epsilon));
            Assert.That(v2.Y, Is.EqualTo(y2).Within(Epsilon));
        }

        [TestCase(3, 6, 3, 1, 2)]
        [TestCase(10, 0, 2, 5, 0)]
        [TestCase(10, -4, 0.5, 20, -8)]
        [TestCase(8136.880608, 7198.508063, 91.237, 89.184, 78.899)]
        public void DivisionOperator(double x1, double y1, double s, double x2,
            double y2)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var v2 = v1/s;

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2).Within(Epsilon));
            Assert.That(v2.Y, Is.EqualTo(y2).Within(Epsilon));
        }

        [TestCase(2.51, 0, -2.51, 0)]
        [TestCase(0, 1.44, 0, -1.44)]
        [TestCase(-3, 4, 3, -4)]
        [TestCase(-0.54, -0.065, 0.54, 0.065)]
        [TestCase(89.727, 59.751, -89.727, -59.751)]
        public void OppositionOperator(double x1, double y1, double x2, double y2)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var actual = -v1;

            // Assert
            Assert.That(actual.X, Is.EqualTo(x2));
            Assert.That(actual.Y, Is.EqualTo(y2));
        }

        [TestCase(0, 0, 0, 0, true)]
        [TestCase(5, 3, 5, 3, true)]
        [TestCase(-1, 1, -3, -3, false)]
        [TestCase(0, 0, 7, -3, false)]
        [TestCase(60.86360580, 4.47213595, 60.86360580, 4.47213595, true)]
        [TestCase(60.86360580, 4.47213595, 60.86360580, 4.47213596, false)]
        public void EqualityOperator(double x1, double y1, double x2, double y2, bool expected)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            var actual1 = v1 == v2;

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, false)]
        [TestCase(5, 3, 5, 3, false)]
        [TestCase(-1, 1, -3, -3, true)]
        [TestCase(0, 0, 7, -3, true)]
        [TestCase(60.86360580, 4.47213595, 60.86360580, 4.47213595, false)]
        [TestCase(60.86360580, 4.47213595, 60.86360580, 4.47213596, true)]
        public void InequalityOperator(double x1, double y1, double x2, double y2, bool expected)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            var actual1 = v1 != v2;

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
        }

        #endregion
    }
}