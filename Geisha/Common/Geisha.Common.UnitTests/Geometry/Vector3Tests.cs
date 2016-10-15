using System;
using Geisha.Common.Geometry;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Geometry
{
    [TestFixture]
    public class Vector3Tests
    {
        private const double Epsilon = 0.000001;

        private static bool AreParallel(Vector3 v1, Vector3 v2)
        {
            var factorX1 = v1.X/v1.Length;
            var factorY1 = v1.Y/v1.Length;
            var factorZ1 = v1.Z/v1.Length;

            var factorX2 = v2.X/v2.Length;
            var factorY2 = v2.Y/v2.Length;
            var factorZ2 = v2.Z/v2.Length;

            return Math.Abs(factorX1 - factorX2) < Epsilon && Math.Abs(factorY1 - factorY2) < Epsilon &&
                   Math.Abs(factorZ1 - factorZ2) < Epsilon;
        }

        #region Static properties

        [Test]
        public void Zero()
        {
            // Arrange
            // Act
            var v1 = Vector3.Zero;

            // Assert
            Assert.That(v1.X, Is.Zero);
            Assert.That(v1.Y, Is.Zero);
            Assert.That(v1.Z, Is.Zero);
        }

        [Test]
        public void One()
        {
            // Arrange
            // Act
            var v1 = Vector3.One;

            // Assert
            Assert.That(v1.X, Is.EqualTo(1));
            Assert.That(v1.Y, Is.EqualTo(1));
            Assert.That(v1.Z, Is.EqualTo(1));
        }

        #endregion

        #region Properties

        [TestCase(0, 0, 0, 0)]
        [TestCase(5, 0, 0, 5)]
        [TestCase(0, 0, 3.14, 3.14)]
        [TestCase(3, 4, 5, 7.0710678118654755)]
        [TestCase(46.294, 54.684, 3.116, 71.71599366947376)]
        public void Length(double x1, double y1, double z1, double expected)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var actual = v1.Length;

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(Epsilon));
        }

        [TestCase(2.51, 0, 0)]
        [TestCase(0, 0, 1.44)]
        [TestCase(-3, 4, -5)]
        [TestCase(-0.54, -0.065, 0.17)]
        [TestCase(89.727, 59.751, 9.027)]
        public void Unit(double x1, double y1, double z1)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var actualVector = v1.Unit;
            var actualLength = actualVector.Length;

            // Assert
            Assert.That(actualLength, Is.EqualTo(1).Within(Epsilon));
            Assert.That(AreParallel(v1, actualVector), Is.True);
        }

        [TestCase(2.51, 0, 0, -2.51, 0, 0)]
        [TestCase(0, 0, 1.44, 0, 0, -1.44)]
        [TestCase(-3, 4, -5, 3, -4, 5)]
        [TestCase(-0.54, -0.065, 0.13, 0.54, 0.065, -0.13)]
        [TestCase(89.727, 59.751, 41.960, -89.727, -59.751, -41.960)]
        public void Opposite(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var actual = v1.Opposite;

            // Assert
            Assert.That(actual.X, Is.EqualTo(x2));
            Assert.That(actual.Y, Is.EqualTo(y2));
            Assert.That(actual.Z, Is.EqualTo(z2));
        }

        [TestCase(2.51, 0, 0)]
        [TestCase(0, 0, 1.44)]
        [TestCase(-3, 4, 5)]
        [TestCase(-0.54, -0.065, 0.13)]
        [TestCase(89.727, 59.751, 41.960)]
        public void Array(double x1, double y1, double z1)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var actual = v1.Array;

            // Assert
            Assert.That(actual[0], Is.EqualTo(x1));
            Assert.That(actual[1], Is.EqualTo(y1));
            Assert.That(actual[2], Is.EqualTo(z1));
        }

        #endregion

        #region Constructors

        [Test]
        public void ParameterlessConstructor()
        {
            // Arrange
            // Act
            var v1 = new Vector3();

            // Assert
            Assert.That(v1.X, Is.Zero);
            Assert.That(v1.Y, Is.Zero);
            Assert.That(v1.Z, Is.Zero);
        }

        [Test]
        public void ConstructorFromThreeDoubles()
        {
            // Arrange
            // Act
            var v1 = new Vector3(1, 2, 3);

            // Assert
            Assert.That(v1.X, Is.EqualTo(1));
            Assert.That(v1.Y, Is.EqualTo(2));
            Assert.That(v1.Z, Is.EqualTo(3));
        }

        [Test]
        public void ConstructorFromArray()
        {
            // Arrange
            // Act
            var v1 = new Vector3(new double[] {1, 2, 3});

            // Assert
            Assert.That(v1.X, Is.EqualTo(1));
            Assert.That(v1.Y, Is.EqualTo(2));
            Assert.That(v1.Z, Is.EqualTo(3));
        }

        [TestCase(2)]
        [TestCase(4)]
        public void ConstructorFromArrayThrowsException_GivenArrayOfLengthDifferentFromThree(int length)
        {
            // Arrange
            var array = new double[length];

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => new Vector3(array));
        }

        #endregion

        #region Methods

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(0, 0, 0, 5, 3, 4, 5, 3, 4)]
        [TestCase(2, 4, 1, -3, -3, -3, -1, 1, -2)]
        [TestCase(-7, 3, -2, 7, -3, 2, 0, 0, 0)]
        [TestCase(96.747, 71.087, 93.255, 65.603, 91.750, 60.467, 162.35, 162.837, 153.722)]
        public void Add(double x1, double y1, double z1, double x2, double y2, double z2, double x3,
            double y3, double z3)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);
            var v2 = new Vector3(x2, y2, z2);

            // Act
            var v3 = v1.Add(v2);

            // Assert
            Assert.That(v3.X, Is.EqualTo(x3).Within(Epsilon));
            Assert.That(v3.Y, Is.EqualTo(y3).Within(Epsilon));
            Assert.That(v3.Z, Is.EqualTo(z3).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 4, 5, 3, 4, 0, 0, 0)]
        [TestCase(-1, 1, 2, -3, -3, 4, 2, 4, -2)]
        [TestCase(0, 0, 0, 7, -3, 1, -7, 3, -1)]
        [TestCase(20.069, 46.724, 84.883, 74.225, 18.948, 87.805, -54.156, 27.776, -2.922)]
        public void Subtract(double x1, double y1, double z1, double x2, double y2, double z2, double x3,
            double y3, double z3)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);
            var v2 = new Vector3(x2, y2, z2);

            // Act
            var v3 = v1.Subtract(v2);

            // Assert
            Assert.That(v3.X, Is.EqualTo(x3).Within(Epsilon));
            Assert.That(v3.Y, Is.EqualTo(y3).Within(Epsilon));
            Assert.That(v3.Z, Is.EqualTo(z3).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 1, 1, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 4, 8, 12)]
        [TestCase(5, 0, -0.5, 2, 10, 0, -1)]
        [TestCase(20, -8, 0.3, 0.5, 10, -4, 0.15)]
        [TestCase(89.184, 78.899, 3.166, 91.237, 8136.880608, 7198.508063, 288.856342)]
        public void Multiply(double x1, double y1, double z1, double s, double x2,
            double y2, double z2)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var v2 = v1.Multiply(s);

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2).Within(Epsilon));
            Assert.That(v2.Y, Is.EqualTo(y2).Within(Epsilon));
            Assert.That(v2.Z, Is.EqualTo(z2).Within(Epsilon));
        }

        [TestCase(3, 6, 9, 3, 1, 2, 3)]
        [TestCase(10, 0, -2, 2, 5, 0, -1)]
        [TestCase(10, -4, 3, 0.5, 20, -8, 6)]
        [TestCase(8136.880608, 7198.508063, 90.099, 91.237, 89.184, 78.899, 0.9875269901)]
        public void Divide(double x1, double y1, double z1, double s, double x2,
            double y2, double z2)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var v2 = v1.Divide(s);

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2).Within(Epsilon));
            Assert.That(v2.Y, Is.EqualTo(y2).Within(Epsilon));
            Assert.That(v2.Z, Is.EqualTo(z2).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 4, 5, 3, 4, 50)]
        [TestCase(-1, 1, 0, -3, -3, 0, 0)]
        [TestCase(0, 0, 0, 7, -3, 12, 0)]
        [TestCase(20.069, 46.724, 69.044, 74.225, 18.948, 26.323, 4192.393089)]
        public void Dot(double x1, double y1, double z1, double x2, double y2, double z2, double expected)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);
            var v2 = new Vector3(x2, y2, z2);

            // Act
            var actual = v1.Dot(v2);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 4, 5, 3, 4, 0)]
        [TestCase(-1, 1, 2, -3, -3, 4, 4.898979485566356)]
        [TestCase(0, 0, 0, 7, -3, 0.8, 7.657675887630659)]
        [TestCase(20.069, 46.724, 46.883, 74.225, 18.948, 4.096, 74.39829219142064)]
        public void Distance(double x1, double y1, double z1, double x2, double y2, double z2, double expected)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);
            var v2 = new Vector3(x2, y2, z2);

            // Act
            var actual = v1.Distance(v2);

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, true)]
        [TestCase(5, 3, 4, 5, 3, 4, true)]
        [TestCase(-1, 1, 0, -3, -3, 0, false)]
        [TestCase(0, 0, 0, 7, -3, 1, false)]
        [TestCase(60.86360580, 4.47213595, 8.910, 60.86360580, 4.47213595, 8.910, true)]
        [TestCase(60.86360580, 4.47213595, 8.910, 60.86360580, 4.47213596, 8.910, false)]
        public void Equals(double x1, double y1, double z1, double x2, double y2, double z2, bool expected)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);
            var v2 = new Vector3(x2, y2, z2);

            // Act
            var actual1 = v1.Equals(v2);
            var actual2 = v1.Equals((object) v2);

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
        }

        #endregion

        #region Operators

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(0, 0, 0, 5, 3, 4, 5, 3, 4)]
        [TestCase(2, 4, 1, -3, -3, -3, -1, 1, -2)]
        [TestCase(-7, 3, -2, 7, -3, 2, 0, 0, 0)]
        [TestCase(96.747, 71.087, 93.255, 65.603, 91.750, 60.467, 162.35, 162.837, 153.722)]
        public void AdditionOperator(double x1, double y1, double z1, double x2, double y2, double z2, double x3,
            double y3, double z3)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);
            var v2 = new Vector3(x2, y2, z2);

            // Act
            var v3 = v1 + v2;

            // Assert
            Assert.That(v3.X, Is.EqualTo(x3).Within(Epsilon));
            Assert.That(v3.Y, Is.EqualTo(y3).Within(Epsilon));
            Assert.That(v3.Z, Is.EqualTo(z3).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 4, 5, 3, 4, 0, 0, 0)]
        [TestCase(-1, 1, 2, -3, -3, 4, 2, 4, -2)]
        [TestCase(0, 0, 0, 7, -3, 1, -7, 3, -1)]
        [TestCase(20.069, 46.724, 84.883, 74.225, 18.948, 87.805, -54.156, 27.776, -2.922)]
        public void SubtractionOperator(double x1, double y1, double z1, double x2, double y2, double z2, double x3,
            double y3, double z3)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);
            var v2 = new Vector3(x2, y2, z2);

            // Act
            var v3 = v1 - v2;

            // Assert
            Assert.That(v3.X, Is.EqualTo(x3).Within(Epsilon));
            Assert.That(v3.Y, Is.EqualTo(y3).Within(Epsilon));
            Assert.That(v3.Z, Is.EqualTo(z3).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 1, 1, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 4, 8, 12)]
        [TestCase(5, 0, -0.5, 2, 10, 0, -1)]
        [TestCase(20, -8, 0.3, 0.5, 10, -4, 0.15)]
        [TestCase(89.184, 78.899, 3.166, 91.237, 8136.880608, 7198.508063, 288.856342)]
        public void MultiplicationOperator(double x1, double y1, double z1, double s, double x2,
            double y2, double z2)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var v2 = v1*s;

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2).Within(Epsilon));
            Assert.That(v2.Y, Is.EqualTo(y2).Within(Epsilon));
            Assert.That(v2.Z, Is.EqualTo(z2).Within(Epsilon));
        }

        [TestCase(3, 6, 9, 3, 1, 2, 3)]
        [TestCase(10, 0, -2, 2, 5, 0, -1)]
        [TestCase(10, -4, 3, 0.5, 20, -8, 6)]
        [TestCase(8136.880608, 7198.508063, 90.099, 91.237, 89.184, 78.899, 0.9875269901)]
        public void DivisionOperator(double x1, double y1, double z1, double s, double x2,
            double y2, double z2)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var v2 = v1/s;

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2).Within(Epsilon));
            Assert.That(v2.Y, Is.EqualTo(y2).Within(Epsilon));
            Assert.That(v2.Z, Is.EqualTo(z2).Within(Epsilon));
        }

        [TestCase(2.51, 0, 0, -2.51, 0, 0)]
        [TestCase(0, 0, 1.44, 0, 0, -1.44)]
        [TestCase(-3, 4, -5, 3, -4, 5)]
        [TestCase(-0.54, -0.065, 0.13, 0.54, 0.065, -0.13)]
        [TestCase(89.727, 59.751, 41.960, -89.727, -59.751, -41.960)]
        public void OppositionOperator(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var actual = -v1;

            // Assert
            Assert.That(actual.X, Is.EqualTo(x2));
            Assert.That(actual.Y, Is.EqualTo(y2));
            Assert.That(actual.Z, Is.EqualTo(z2));
        }

        [TestCase(0, 0, 0, 0, 0, 0, true)]
        [TestCase(5, 3, 4, 5, 3, 4, true)]
        [TestCase(-1, 1, 0, -3, -3, 0, false)]
        [TestCase(0, 0, 0, 7, -3, 1, false)]
        [TestCase(60.86360580, 4.47213595, 8.910, 60.86360580, 4.47213595, 8.910, true)]
        [TestCase(60.86360580, 4.47213595, 8.910, 60.86360580, 4.47213596, 8.910, false)]
        public void EqualityOperator(double x1, double y1, double z1, double x2, double y2, double z2, bool expected)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);
            var v2 = new Vector3(x2, y2, z2);

            // Act
            var actual1 = v1 == v2;

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0, 0, false)]
        [TestCase(5, 3, 4, 5, 3, 4, false)]
        [TestCase(-1, 1, 0, -3, -3, 0, true)]
        [TestCase(0, 0, 0, 7, -3, 1, true)]
        [TestCase(60.86360580, 4.47213595, 8.910, 60.86360580, 4.47213595, 8.910, false)]
        [TestCase(60.86360580, 4.47213595, 8.910, 60.86360580, 4.47213596, 8.910, true)]
        public void InequalityOperator(double x1, double y1, double z1, double x2, double y2, double z2, bool expected)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);
            var v2 = new Vector3(x2, y2, z2);

            // Act
            var actual1 = v1 != v2;

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
        }

        #endregion
    }
}