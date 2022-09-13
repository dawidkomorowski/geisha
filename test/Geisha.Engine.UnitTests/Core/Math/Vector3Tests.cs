using System;
using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math
{
    [TestFixture]
    [DefaultFloatingPointTolerance(Epsilon)]
    public class Vector3Tests
    {
        private const double Epsilon = 0.000001;

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

        [Test]
        public void UnitX()
        {
            // Arrange
            // Act
            var v = Vector3.UnitX;

            // Assert
            Assert.That(v.X, Is.EqualTo(1));
            Assert.That(v.Y, Is.EqualTo(0));
            Assert.That(v.Z, Is.EqualTo(0));
        }

        [Test]
        public void UnitY()
        {
            // Arrange
            // Act
            var v = Vector3.UnitY;

            // Assert
            Assert.That(v.X, Is.EqualTo(0));
            Assert.That(v.Y, Is.EqualTo(1));
            Assert.That(v.Z, Is.EqualTo(0));
        }

        [Test]
        public void UnitZ()
        {
            // Arrange
            // Act
            var v = Vector3.UnitZ;

            // Assert
            Assert.That(v.X, Is.EqualTo(0));
            Assert.That(v.Y, Is.EqualTo(0));
            Assert.That(v.Z, Is.EqualTo(1));
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
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(2.51, 0, 0, 1, 0, 0)]
        [TestCase(0, 3.14, 0, 0, 1, 0)]
        [TestCase(0, 0, 1.44, 0, 0, 1)]
        [TestCase(-3, 4, -5, -0.424264, 0.565685, -0.707106)]
        [TestCase(-0.54, -0.065, 0.17, -0.947623, -0.1140658, 0.298325)]
        [TestCase(89.727, 59.751, 9.027, 0.829434, 0.552336, 0.083445)]
        public void Unit(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var actualVector = v1.Unit;

            // Assert
            Assert.That(actualVector.Length, Is.EqualTo(1));
            Assert.That(actualVector.X, Is.EqualTo(x2));
            Assert.That(actualVector.Y, Is.EqualTo(y2));
            Assert.That(actualVector.Z, Is.EqualTo(z2));
        }

        [Test]
        public void Unit_ShouldReturnZeroVector_WhenVectorLengthIsToSmall()
        {
            // Arrange
            var v1 = Vector3.Zero;

            // Act
            var actualVector = v1.Unit;

            // Assert
            Assert.That(actualVector, Is.EqualTo(Vector3.Zero));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(1, -2, 3, -1, 2, -3)]
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

        [TestCase(0, 0, 0)]
        [TestCase(1, -2, 3)]
        [TestCase(89.727, 59.751, 41.960)]
        public void Homogeneous(double x, double y, double z)
        {
            // Arrange
            var v = new Vector3(x, y, z);

            // Act
            var actual = v.Homogeneous;

            // Assert
            Assert.That(actual.X, Is.EqualTo(x));
            Assert.That(actual.Y, Is.EqualTo(y));
            Assert.That(actual.Z, Is.EqualTo(z));
            Assert.That(actual.W, Is.EqualTo(1));
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
            var v1 = new Vector3(new double[] { 1, 2, 3 });

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
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new Vector3(array));
        }

        #endregion

        #region Methods

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 4, 0, 0, 0, 5, 3, 4)]
        [TestCase(2, 4, 1, -3, -3, -3, -1, 1, -2)]
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
            Assert.That(v3.X, Is.EqualTo(x3));
            Assert.That(v3.Y, Is.EqualTo(y3));
            Assert.That(v3.Z, Is.EqualTo(z3));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 4, 0, 0, 0, 5, 3, 4)]
        [TestCase(-1, 1, 2, -3, -3, 4, 2, 4, -2)]
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
            Assert.That(v3.X, Is.EqualTo(x3));
            Assert.That(v3.Y, Is.EqualTo(y3));
            Assert.That(v3.Z, Is.EqualTo(z3));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 1, 1, 2, 3)]
        [TestCase(1, -2, 3, -2, -2, 4, -6)]
        [TestCase(89.184, 78.899, 3.166, 91.237, 8136.880608, 7198.508063, 288.856342)]
        public void Multiply(double x1, double y1, double z1, double s, double x2,
            double y2, double z2)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var v2 = v1.Multiply(s);

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
        }

        [TestCase(1, 2, 3, 1, 1, 2, 3)]
        [TestCase(1, -2, 3, -2, -0.5, 1, -1.5)]
        [TestCase(8136.880608, 7198.508063, 90.099, 91.237, 89.184, 78.899, 0.9875269901)]
        public void Divide(double x1, double y1, double z1, double s, double x2,
            double y2, double z2)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var v2 = v1.Divide(s);

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
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
            Assert.That(actual, Is.EqualTo(expected));
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
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 0, 0, 15, 15, 0, 0)]
        [TestCase(0, -3, 0, 12, 0, -12, 0)]
        [TestCase(0, 0, 0.13, 7.38, 0, 0, 7.38)]
        [TestCase(-20.069, 46.724, 46.883, 18.948, -5.497908, 12.800053, 12.843611)]
        public void OfLength(double x1, double y1, double z1, double length, double x2, double y2, double z2)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var v2 = v1.OfLength(length);

            // Assert
            Assert.That(v2.Length, Is.EqualTo(length));
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 0, 0, 10, 5, 0, 0, 5)]
        [TestCase(5, 0, 0, 2, 2, 0, 0, 2)]
        [TestCase(0, 5, 0, 10, 0, 5, 0, 5)]
        [TestCase(0, 5, 0, 2, 0, 2, 0, 2)]
        [TestCase(0, 0, 5, 10, 0, 0, 5, 5)]
        [TestCase(0, 0, 5, 2, 0, 0, 2, 2)]
        [TestCase(-20.069, 46.724, 46.883, 80, -20.069, 46.724, 46.883, 69.165834)]
        [TestCase(-20.069, 46.724, 46.883, 40, -11.606308, 27.021433, 27.113386, 40)]
        public void Clamp_Max(double x1, double y1, double z1, double maxLength, double x2, double y2, double z2, double expectedLength)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var v2 = v1.Clamp(maxLength);

            // Assert
            Assert.That(v2.Length, Is.EqualTo(expectedLength));
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 0, 0, 4, 10, 5, 0, 0, 5)]
        [TestCase(5, 0, 0, 8, 10, 8, 0, 0, 8)]
        [TestCase(5, 0, 0, 1, 2, 2, 0, 0, 2)]
        [TestCase(0, 5, 0, 4, 10, 0, 5, 0, 5)]
        [TestCase(0, 5, 0, 8, 10, 0, 8, 0, 8)]
        [TestCase(0, 5, 0, 1, 2, 0, 2, 0, 2)]
        [TestCase(0, 0, 5, 4, 10, 0, 0, 5, 5)]
        [TestCase(0, 0, 5, 8, 10, 0, 0, 8, 8)]
        [TestCase(0, 0, 5, 1, 2, 0, 0, 2, 2)]
        [TestCase(-20.069, 46.724, 46.883, 20, 80, -20.069, 46.724, 46.883, 69.165834)]
        [TestCase(-20.069, 46.724, 46.883, 75, 80, -21.761828, 50.665188, 50.837599, 75)]
        [TestCase(-20.069, 46.724, 46.883, 20, 40, -11.606308, 27.021433, 27.113386, 40)]
        public void Clamp_MinMax(double x1, double y1, double z1, double minLength, double maxLength, double x2, double y2, double z2, double expectedLength)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var v2 = v1.Clamp(minLength, maxLength);

            // Assert
            Assert.That(v2.Length, Is.EqualTo(expectedLength));
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0)]
        [TestCase(-20.069, 46.724, 46.883, 27.113386, 27.113386, 46.724, 46.883)]
        public void WithX(double x1, double y1, double z1, double newX, double x2, double y2, double z2)
        {
            // Arrange
            var v = new Vector3(x1, y1, z1);
            var expected = new Vector3(x2, y2, z2);

            // Act
            var actual = v.WithX(newX);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0)]
        [TestCase(-20.069, 46.724, 46.883, 27.113386, -20.069, 27.113386, 46.883)]
        public void WithY(double x1, double y1, double z1, double newY, double x2, double y2, double z2)
        {
            // Arrange
            var v = new Vector3(x1, y1, z1);
            var expected = new Vector3(x2, y2, z2);

            // Act
            var actual = v.WithY(newY);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0)]
        [TestCase(-20.069, 46.724, 46.883, 27.113386, -20.069, 46.724, 27.113386)]
        public void WithZ(double x1, double y1, double z1, double newZ, double x2, double y2, double z2)
        {
            // Arrange
            var v = new Vector3(x1, y1, z1);
            var expected = new Vector3(x2, y2, z2);

            // Act
            var actual = v.WithZ(newZ);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ToArray(
            [Random(-100d, 100d, 1)] double x,
            [Random(-100d, 100d, 1)] double y,
            [Random(-100d, 100d, 1)] double z)
        {
            // Arrange
            var v = new Vector3(x, y, z);

            // Act
            var actual = v.ToArray();

            // Assert
            Assert.That(actual[0], Is.EqualTo(x));
            Assert.That(actual[1], Is.EqualTo(y));
            Assert.That(actual[2], Is.EqualTo(z));
        }

        [TestCase(1, 2, 3, 1, 2, 3, true)]
        [TestCase(1, 2, 3, 0, 2, 3, false)]
        [TestCase(1, 2, 3, 1, 0, 3, false)]
        [TestCase(1, 2, 3, 1, 2, 0, false)]
        [TestCase(60.86360580, 4.47213595, 8.910, 60.86360580, 4.47213595, 8.910, true)]
        [TestCase(60.86360580, 4.47213595, 8.910, 60.86360580, 4.47213596, 8.910, false)]
        public void EqualityMembers_ShouldEqualVector3_WhenComponentsAreEqual(double x1, double y1, double z1, double x2, double y2, double z2,
            bool expectedIsEqual)
        {
            // Arrange
            var v1 = new Vector3(x1, y1, z1);
            var v2 = new Vector3(x2, y2, z2);

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(v1, v2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }

        [TestCase(0, 0, 0, "X: 0, Y: 0, Z: 0")]
        [TestCase(74.025, -27.169, -25.159, "X: 74.025, Y: -27.169, Z: -25.159")]
        [SetCulture("")]
        public void ToString_Test(double x, double y, double z, string expected)
        {
            // Arrange
            var v = new Vector3(x, y, z);

            // Act
            var actual = v.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(1, 2, 3)]
        [TestCase(91.3376, 63.2359, 9.7540)]
        public void ToVector2(double x, double y, double z)
        {
            // Arrange
            var vector3 = new Vector3(x, y, z);

            // Act
            var vector2 = vector3.ToVector2();

            // Assert
            Assert.That(vector2.X, Is.EqualTo(x));
            Assert.That(vector2.Y, Is.EqualTo(y));
        }

        [TestCase(1, 2, 3)]
        [TestCase(91.3376, 63.2359, 9.7540)]
        public void ToVector4(double x, double y, double z)
        {
            // Arrange
            var vector3 = new Vector3(x, y, z);

            // Act
            var vector4 = vector3.ToVector4();

            // Assert
            Assert.That(vector4.X, Is.EqualTo(x));
            Assert.That(vector4.Y, Is.EqualTo(y));
            Assert.That(vector4.Z, Is.EqualTo(z));
            Assert.That(vector4.W, Is.Zero);
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
            Assert.That(v3.X, Is.EqualTo(x3));
            Assert.That(v3.Y, Is.EqualTo(y3));
            Assert.That(v3.Z, Is.EqualTo(z3));
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
            Assert.That(v3.X, Is.EqualTo(x3));
            Assert.That(v3.Y, Is.EqualTo(y3));
            Assert.That(v3.Z, Is.EqualTo(z3));
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
            var v2 = v1 * s;

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
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
            var v2 = v1 / s;

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(1, -2, 3, -1, 2, -3)]
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

        #endregion
    }
}