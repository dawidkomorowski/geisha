using System;
using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math
{
    [TestFixture]
    [DefaultFloatingPointTolerance(Epsilon)]
    public class Vector2Tests
    {
        private const double Epsilon = 1e-6;

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

        [Test]
        public void UnitX()
        {
            // Arrange
            // Act
            var v = Vector2.UnitX;

            // Assert
            Assert.That(v.X, Is.EqualTo(1));
            Assert.That(v.Y, Is.EqualTo(0));
        }

        [Test]
        public void UnitY()
        {
            // Arrange
            // Act
            var v = Vector2.UnitY;

            // Assert
            Assert.That(v.X, Is.EqualTo(0));
            Assert.That(v.Y, Is.EqualTo(1));
        }

        #endregion

        #region Properties

        [TestCase(0, 0, 0)]
        [TestCase(5, 0, 5)]
        [TestCase(0, 3.14, 3.14)]
        [TestCase(3, 4, 5)]
        [TestCase(46.294, 54.684, 71.64826789253177)]
        public void Length_ShouldReturnLengthOfVector(double x1, double y1, double expected)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var actual = v1.Length;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0)]
        [TestCase(5, 0, 25)]
        [TestCase(0, 3.14, 9.8596)]
        [TestCase(3, 4, 25)]
        [TestCase(46.294, 54.684, 5133.4742919999989)]
        public void LengthSquared_ShouldReturnLengthOfVectorSquared(double x1, double y1, double expected)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var actual = v1.LengthSquared;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(2.51, 0, 1, 0)]
        [TestCase(0, 1.44, 0, 1)]
        [TestCase(-3, 4, -0.6, 0.8)]
        [TestCase(-0.54, -0.065, -0.992833, -0.119507)]
        [TestCase(89.727, 59.751, 0.832337, 0.554269)]
        public void Unit(double x1, double y1, double x2, double y2)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var actualVector = v1.Unit;

            // Assert
            Assert.That(actualVector.Length, Is.EqualTo(1));
            Assert.That(actualVector.X, Is.EqualTo(x2));
            Assert.That(actualVector.Y, Is.EqualTo(y2));
        }

        [Test]
        public void Unit_ShouldReturnZeroVector_WhenVectorLengthIsToSmall()
        {
            // Arrange
            var v1 = Vector2.Zero;

            // Act
            var actualVector = v1.Unit;

            // Assert
            Assert.That(actualVector, Is.EqualTo(Vector2.Zero));
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(1, -2, -1, 2)]
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

        [TestCase(0, 0, 0, 0)]
        [TestCase(5, 0, 0, 1)]
        [TestCase(0, 5, -1, 0)]
        [TestCase(-5, 0, 0, -1)]
        [TestCase(0, -5, 1, 0)]
        [TestCase(89.727, 59.751, -0.554269, 0.832337)]
        public void Normal(double x1, double y1, double x2, double y2)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var actual = v1.Normal;

            // Assert
            Assert.That(actual.X, Is.EqualTo(x2));
            Assert.That(actual.Y, Is.EqualTo(y2));
        }

        [TestCase(0, 0)]
        [TestCase(1, -2)]
        [TestCase(89.727, 59.751)]
        public void Homogeneous(double x, double y)
        {
            // Arrange
            var v = new Vector2(x, y);

            // Act
            var actual = v.Homogeneous;

            // Assert
            Assert.That(actual.X, Is.EqualTo(x));
            Assert.That(actual.Y, Is.EqualTo(y));
            Assert.That(actual.Z, Is.EqualTo(1));
        }

        #endregion

        #region Constructors

        [Test]
        public void ParameterlessConstructor()
        {
            // Arrange
            // Act
            var v1 = new Vector2();

            // Assert
            Assert.That(v1.X, Is.Zero);
            Assert.That(v1.Y, Is.Zero);
        }

        [Test]
        public void ConstructorFromTwoDoubles()
        {
            // Arrange
            // Act
            var v1 = new Vector2(1, 2);

            // Assert
            Assert.That(v1.X, Is.EqualTo(1));
            Assert.That(v1.Y, Is.EqualTo(2));
        }

        [Test]
        public void ConstructorFromArray()
        {
            // Arrange
            // Act
            var v1 = new Vector2(new double[] { 1, 2 });

            // Assert
            Assert.That(v1.X, Is.EqualTo(1));
            Assert.That(v1.Y, Is.EqualTo(2));
        }

        [TestCase(1)]
        [TestCase(3)]
        public void ConstructorFromArray_ThrowsException_GivenArrayOfLengthDifferentFromTwo(int length)
        {
            // Arrange
            var array = new double[length];

            // Act
            // Assert
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new Vector2(array));
        }

        #endregion

        #region Static methods

        [TestCase(-4, -2, 4, 6, 0, -4, -2)]
        [TestCase(-4, -2, 4, 6, 1, 4, 6)]
        [TestCase(-4, -2, 4, 6, 0.5, 0, 2)]
        [TestCase(-4, -2, 4, 6, 0.25, -2, 0)]
        public void Lerp_Test(double x1, double y1, double x2, double y2, double alpha, double expectedX, double expectedY)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            var actual = Vector2.Lerp(v1, v2, alpha);

            // Assert
            Assert.That(actual.X, Is.EqualTo(expectedX));
            Assert.That(actual.Y, Is.EqualTo(expectedY));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 3, 4)]
        [TestCase(3, 4, 1, 2, 3, 4)]
        [TestCase(3, 2, 1, 4, 3, 4)]
        [TestCase(1, 4, 3, 2, 3, 4)]
        public void Max_Test(double x1, double y1, double x2, double y2, double expectedX, double expectedY)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            var actual = Vector2.Max(v1, v2);

            // Assert
            Assert.That(actual.X, Is.EqualTo(expectedX));
            Assert.That(actual.Y, Is.EqualTo(expectedY));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 1, 2)]
        [TestCase(3, 4, 1, 2, 1, 2)]
        [TestCase(3, 2, 1, 4, 1, 2)]
        [TestCase(1, 4, 3, 2, 1, 2)]
        public void Min_Test(double x1, double y1, double x2, double y2, double expectedX, double expectedY)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            var actual = Vector2.Min(v1, v2);

            // Assert
            Assert.That(actual.X, Is.EqualTo(expectedX));
            Assert.That(actual.Y, Is.EqualTo(expectedY));
        }

        #endregion

        #region Methods

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 0, 0, 5, 3)]
        [TestCase(2, 4, -3, -3, -1, 1)]
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
        [TestCase(5, 3, 0, 0, 5, 3)]
        [TestCase(-1, 1, -3, -3, 2, 4)]
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
            Assert.That(v3.X, Is.EqualTo(x3));
            Assert.That(v3.Y, Is.EqualTo(y3));
        }

        [TestCase(0, 0, 0, 0, 0)]
        [TestCase(1, 2, 0, 0, 0)]
        [TestCase(1, 2, 1, 1, 2)]
        [TestCase(1, -2, 2, 2, -4)]
        [TestCase(89.184, 78.899, 91.237, 8136.880608, 7198.508063)]
        public void Multiply(double x1, double y1, double s, double x2,
            double y2)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var v2 = v1.Multiply(s);

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
        }

        [TestCase(1, 2, 1, 1, 2)]
        [TestCase(1, -2, 2, 0.5, -1)]
        [TestCase(8136.880608, 7198.508063, 91.237, 89.184, 78.899)]
        public void Divide(double x1, double y1, double s, double x2,
            double y2)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var v2 = v1.Divide(s);

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
        }

        [TestCase(0, 0, 0, 0, 0)]
        [TestCase(0, 0, 2, 3, 0)]
        [TestCase(2, 0, 0, 2, 0)]
        [TestCase(2, 3, 2, 3, 13)]
        [TestCase(-2, 3, -3, -2, 0)]
        [TestCase(20.069, 46.724, 74.225, 18.948, 2374.947877)]
        public void Dot_ShouldComputeDotProduct(double x1, double y1, double x2, double y2, double expected)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            var actual = v1.Dot(v2);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0)]
        [TestCase(0, 0, 2, 3, 0)]
        [TestCase(2, 0, 0, 2, 4)]
        [TestCase(2, 3, 2, 3, 0)]
        [TestCase(-2, 3, -3, -2, 13)]
        [TestCase(20.069, 46.724, 74.225, 18.948, -3087.821488)]
        public void Cross_ShouldComputeCrossProduct(double x1, double y1, double x2, double y2, double expected)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            var actual = v1.Cross(v2);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
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
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0)]
        [TestCase(5, 0, 15, 15, 0)]
        [TestCase(0, -3, 12, 0, -12)]
        [TestCase(-20.069, 46.724, 18.948, -7.477966291, 17.40996048)]
        public void OfLength(double x1, double y1, double length, double x2, double y2)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var v2 = v1.OfLength(length);

            // Assert
            Assert.That(v2.Length, Is.EqualTo(length));
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(5, 0, 10, 5, 0, 5)]
        [TestCase(5, 0, 2, 2, 0, 2)]
        [TestCase(0, 5, 10, 0, 5, 5)]
        [TestCase(0, 5, 2, 0, 2, 2)]
        [TestCase(-20.069, 46.724, 80, -20.069, 46.724, 50.851715)]
        [TestCase(-20.069, 46.724, 40, -15.786291, 36.753135, 40)]
        public void Clamp_Max(double x1, double y1, double maxLength, double x2, double y2, double expectedLength)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var v2 = v1.Clamp(maxLength);

            // Assert
            Assert.That(v2.Length, Is.EqualTo(expectedLength));
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 0, 4, 10, 5, 0, 5)]
        [TestCase(5, 0, 8, 10, 8, 0, 8)]
        [TestCase(5, 0, 1, 2, 2, 0, 2)]
        [TestCase(0, 5, 4, 10, 0, 5, 5)]
        [TestCase(0, 5, 8, 10, 0, 8, 8)]
        [TestCase(0, 5, 1, 2, 0, 2, 2)]
        [TestCase(-20.069, 46.724, 20, 80, -20.069, 46.724, 50.851715)]
        [TestCase(-20.069, 46.724, 60, 80, -23.679437, 55.129703, 60)]
        [TestCase(-20.069, 46.724, 20, 40, -15.786291, 36.753135, 40)]
        public void Clamp_MinMax(double x1, double y1, double minLength, double maxLength, double x2, double y2, double expectedLength)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);

            // Act
            var v2 = v1.Clamp(minLength, maxLength);

            // Assert
            Assert.That(v2.Length, Is.EqualTo(expectedLength));
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(0, 0, 1, 1, 0.5, 0.5)]
        [TestCase(1, 2, 3, 4, 2, 3)]
        [TestCase(1, 2, -3, -4, -1, -1)]
        [TestCase(-20.069, 46.724, -23.679, 55.129, -21.874, 50.9265)]
        public void Midpoint_ShouldComputeMidpointBetweenTwoPoints(double x1, double y1, double x2, double y2, double ex, double ey)
        {
            // Arrange
            var p1 = new Vector2(x1, y1);
            var p2 = new Vector2(x2, y2);
            var expected = new Vector2(ex, ey);

            // Act
            var actual1 = p1.Midpoint(p2);
            var actual2 = p2.Midpoint(p1);

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, double.NaN)]
        [TestCase(1, 0, 0, 0, double.NaN)]
        [TestCase(0, 0, 1, 0, double.NaN)]
        [TestCase(1, 0, 1, 0, 0)]
        [TestCase(12.34, 56.78, 12.34, 56.78, 0)]
        [TestCase(1, 0, 0, 1, System.Math.PI / 2d)]
        [TestCase(1, 0, -1, 0, System.Math.PI)]
        [TestCase(1.7320508075688772, 0, 1.7320508075688772, 1, System.Math.PI / 6d)]
        [TestCase(-3.74, -1.4, 0.2, -4.16, 1.260651)]
        // Test cases for Clamp that prevents NaN.
        [TestCase(-0.8012515883831227, 0.5983275792686839, -0.8012515883831226, 0.5983275792686837, 0)]
        [TestCase(-0.8788150639679754, 0.4771625334652371, -0.8788150639679754, 0.4771625334652371, 0)]
        public void Angle_ShouldComputeAngleBetweenTwoVectors(double x1, double y1, double x2, double y2, double expected)
        {
            // Arrange
            var p1 = new Vector2(x1, y1);
            var p2 = new Vector2(x2, y2);

            // Act
            var actual1 = p1.Angle(p2);
            var actual2 = p2.Angle(p1);

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0)]
        [TestCase(-20.069, 46.724, 27.113386, 27.113386, 46.724)]
        public void WithX(double x1, double y1, double newX, double x2, double y2)
        {
            // Arrange
            var v = new Vector2(x1, y1);
            var expected = new Vector2(x2, y2);

            // Act
            var actual = v.WithX(newX);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0)]
        [TestCase(-20.069, 46.724, 27.113386, -20.069, 27.113386)]
        public void WithY(double x1, double y1, double newY, double x2, double y2)
        {
            // Arrange
            var v = new Vector2(x1, y1);
            var expected = new Vector2(x2, y2);

            // Act
            var actual = v.WithY(newY);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ToArray(
            [Random(-100d, 100d, 1)] double x,
            [Random(-100d, 100d, 1)] double y)
        {
            // Arrange
            var v = new Vector2(x, y);

            // Act
            var actual = v.ToArray();

            // Assert
            Assert.That(actual[0], Is.EqualTo(x));
            Assert.That(actual[1], Is.EqualTo(y));
        }

        [TestCase(1, 2, 1, 2, true)]
        [TestCase(1, 2, 0, 2, false)]
        [TestCase(1, 2, 1, 0, false)]
        [TestCase(60.86360580, 4.47213595, 60.86360580, 4.47213595, true)]
        [TestCase(60.86360580, 4.47213595, 60.86360580, 4.47213596, false)]
        public void EqualityMembers_ShouldEqualVector2_WhenComponentsAreEqual(double x1, double y1, double x2, double y2, bool expectedIsEqual)
        {
            // Arrange
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(v1, v2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }

        [TestCase(0, 0, "X: 0, Y: 0")]
        [TestCase(74.025, -27.169, "X: 74.025, Y: -27.169")]
        [SetCulture("")]
        public void ToString_Test(double x, double y, string expected)
        {
            // Arrange
            var v = new Vector2(x, y);

            // Act
            var actual = v.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(1, 2)]
        [TestCase(91.3376, 63.2359)]
        public void ToVector3(double x, double y)
        {
            // Arrange
            var vector2 = new Vector2(x, y);

            // Act
            var vector3 = vector2.ToVector3();

            // Assert
            Assert.That(vector3.X, Is.EqualTo(x));
            Assert.That(vector3.Y, Is.EqualTo(y));
            Assert.That(vector3.Z, Is.Zero);
        }

        [TestCase(1, 2)]
        [TestCase(91.3376, 63.2359)]
        public void ToVector4(double x, double y)
        {
            // Arrange
            var vector2 = new Vector2(x, y);

            // Act
            var vector4 = vector2.ToVector4();

            // Assert
            Assert.That(vector4.X, Is.EqualTo(x));
            Assert.That(vector4.Y, Is.EqualTo(y));
            Assert.That(vector4.Z, Is.Zero);
            Assert.That(vector4.W, Is.Zero);
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
            Assert.That(v3.X, Is.EqualTo(x3));
            Assert.That(v3.Y, Is.EqualTo(y3));
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
            var v2 = v1 * s;

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
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
            var v2 = v1 / s;

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(1, -2, -1, 2)]
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

        #endregion
    }
}