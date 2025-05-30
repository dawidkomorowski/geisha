﻿using System;
using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math
{
    [TestFixture]
    [DefaultFloatingPointTolerance(Epsilon)]
    public class Vector4Tests
    {
        private const double Epsilon = 1e-6;

        #region Static properties

        [Test]
        public void Zero()
        {
            // Arrange
            // Act
            var v1 = Vector4.Zero;

            // Assert
            Assert.That(v1.X, Is.Zero);
            Assert.That(v1.Y, Is.Zero);
            Assert.That(v1.Z, Is.Zero);
            Assert.That(v1.W, Is.Zero);
        }

        [Test]
        public void One()
        {
            // Arrange
            // Act
            var v1 = Vector4.One;

            // Assert
            Assert.That(v1.X, Is.EqualTo(1));
            Assert.That(v1.Y, Is.EqualTo(1));
            Assert.That(v1.Z, Is.EqualTo(1));
            Assert.That(v1.W, Is.EqualTo(1));
        }

        [Test]
        public void UnitX()
        {
            // Arrange
            // Act
            var v = Vector4.UnitX;

            // Assert
            Assert.That(v.X, Is.EqualTo(1));
            Assert.That(v.Y, Is.EqualTo(0));
            Assert.That(v.Z, Is.EqualTo(0));
            Assert.That(v.W, Is.EqualTo(0));
        }

        [Test]
        public void UnitY()
        {
            // Arrange
            // Act
            var v = Vector4.UnitY;

            // Assert
            Assert.That(v.X, Is.EqualTo(0));
            Assert.That(v.Y, Is.EqualTo(1));
            Assert.That(v.Z, Is.EqualTo(0));
            Assert.That(v.W, Is.EqualTo(0));
        }

        [Test]
        public void UnitZ()
        {
            // Arrange
            // Act
            var v = Vector4.UnitZ;

            // Assert
            Assert.That(v.X, Is.EqualTo(0));
            Assert.That(v.Y, Is.EqualTo(0));
            Assert.That(v.Z, Is.EqualTo(1));
            Assert.That(v.W, Is.EqualTo(0));
        }

        [Test]
        public void UnitW()
        {
            // Arrange
            // Act
            var v = Vector4.UnitW;

            // Assert
            Assert.That(v.X, Is.EqualTo(0));
            Assert.That(v.Y, Is.EqualTo(0));
            Assert.That(v.Z, Is.EqualTo(0));
            Assert.That(v.W, Is.EqualTo(1));
        }

        #endregion

        #region Properties

        [TestCase(0, 0, 0, 0, 0)]
        [TestCase(5, 0, 0, 0, 5)]
        [TestCase(0, 0, 0, 3.14, 3.14)]
        [TestCase(3, 4, 5, 6, 9.27361849)]
        [TestCase(46.294, 54.684, 3.116, 91.407, 116.18271556)]
        public void Length_ShouldReturnLengthOfVector(double x1, double y1, double z1, double w1, double expected)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);

            // Act
            var actual = v1.Length;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0)]
        [TestCase(5, 0, 0, 0, 25)]
        [TestCase(0, 0, 0, 3.14, 9.8596)]
        [TestCase(3, 4, 5, 6, 86)]
        [TestCase(46.294, 54.684, 3.116, 91.407, 13498.423397)]
        public void LengthSquared_ShouldReturnLengthOfVectorSquared(double x1, double y1, double z1, double w1, double expected)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);

            // Act
            var actual = v1.LengthSquared;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(2.51, 0, 0, 0, 1, 0, 0, 0)]
        [TestCase(0, 3.14, 0, 0, 0, 1, 0, 0)]
        [TestCase(0, 0, 1.44, 0, 0, 0, 1, 0)]
        [TestCase(0, 0, 0, 2.71, 0, 0, 0, 1)]
        [TestCase(-3, 4, -5, 6, -0.323498, 0.431331, -0.539163, 0.646996)]
        [TestCase(-0.54, -0.065, 0.17, 1.23, -0.398350, -0.047949, 0.125406, 0.907353)]
        [TestCase(89.727, 59.751, 9.027, 8.670, 0.826783, 0.550571, 0.083178, 0.079889)]
        public void Unit(double x1, double y1, double z1, double w1, double x2, double y2, double z2, double w2)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);

            // Act
            var actualVector = v1.Unit;

            // Assert
            Assert.That(actualVector.Length, Is.EqualTo(1));
            Assert.That(actualVector.X, Is.EqualTo(x2));
            Assert.That(actualVector.Y, Is.EqualTo(y2));
            Assert.That(actualVector.Z, Is.EqualTo(z2));
            Assert.That(actualVector.W, Is.EqualTo(w2));
        }

        [Test]
        public void Unit_ShouldReturnZeroVector_WhenVectorLengthIsToSmall()
        {
            // Arrange
            var v1 = Vector4.Zero;

            // Act
            var actualVector = v1.Unit;

            // Assert
            Assert.That(actualVector, Is.EqualTo(Vector4.Zero));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, -2, 3, -4, -1, 2, -3, 4)]
        [TestCase(89.727, 59.751, 41.960, 40.845, -89.727, -59.751, -41.960, -40.845)]
        public void Opposite(double x1, double y1, double z1, double w1, double x2, double y2, double z2, double w2)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);

            // Act
            var actual = v1.Opposite;

            // Assert
            Assert.That(actual.X, Is.EqualTo(x2));
            Assert.That(actual.Y, Is.EqualTo(y2));
            Assert.That(actual.Z, Is.EqualTo(z2));
            Assert.That(actual.W, Is.EqualTo(w2));
        }

        #endregion

        #region Constructors

        [Test]
        public void ParameterlessConstructor()
        {
            // Arrange
            // Act
            var v1 = new Vector4();

            // Assert
            Assert.That(v1.X, Is.Zero);
            Assert.That(v1.Y, Is.Zero);
            Assert.That(v1.Z, Is.Zero);
            Assert.That(v1.W, Is.Zero);
        }

        [Test]
        public void ConstructorFromFourDoubles()
        {
            // Arrange
            // Act
            var v1 = new Vector4(1, 2, 3, 4);

            // Assert
            Assert.That(v1.X, Is.EqualTo(1));
            Assert.That(v1.Y, Is.EqualTo(2));
            Assert.That(v1.Z, Is.EqualTo(3));
            Assert.That(v1.W, Is.EqualTo(4));
        }

        [Test]
        public void ConstructorFromArray()
        {
            // Arrange
            // Act
            var v1 = new Vector4(new double[] { 1, 2, 3, 4 });

            // Assert
            Assert.That(v1.X, Is.EqualTo(1));
            Assert.That(v1.Y, Is.EqualTo(2));
            Assert.That(v1.Z, Is.EqualTo(3));
            Assert.That(v1.W, Is.EqualTo(4));
        }

        [TestCase(3)]
        [TestCase(5)]
        public void ConstructorFromArrayThrowsException_GivenArrayOfLengthDifferentFromFour(int length)
        {
            // Arrange
            var array = new double[length];

            // Act
            // Assert
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new Vector4(array));
        }

        #endregion

        #region Static methods

        [TestCase(-4, -2, 3, 5, 4, 6, -7, 15, 0, -4, -2, 3, 5)]
        [TestCase(-4, -2, 3, 5, 4, 6, -7, 15, 1, 4, 6, -7, 15)]
        [TestCase(-4, -2, 3, 5, 4, 6, -7, 15, 0.5, 0, 2, -2, 10)]
        [TestCase(-4, -2, 3, 5, 4, 6, -7, 15, 0.25, -2, 0, 0.5, 7.5)]
        public void Lerp_Test(double x1, double y1, double z1, double w1, double x2, double y2, double z2, double w2, double alpha, double expectedX,
            double expectedY, double expectedZ, double expectedW)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);
            var v2 = new Vector4(x2, y2, z2, w2);

            // Act
            var actual = Vector4.Lerp(v1, v2, alpha);

            // Assert
            Assert.That(actual.X, Is.EqualTo(expectedX));
            Assert.That(actual.Y, Is.EqualTo(expectedY));
            Assert.That(actual.Z, Is.EqualTo(expectedZ));
            Assert.That(actual.W, Is.EqualTo(expectedW));
        }

        #endregion

        #region Methods

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 4, 2, 0, 0, 0, 0, 5, 3, 4, 2)]
        [TestCase(2, 4, 1, -2, -3, -3, -3, -3, -1, 1, -2, -5)]
        [TestCase(96.747, 71.087, 93.255, 77.986, 65.603, 91.750, 60.467, 36.633, 162.35, 162.837, 153.722, 114.619)]
        public void Add(double x1, double y1, double z1, double w1, double x2, double y2, double z2, double w2,
            double x3, double y3, double z3, double w3)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);
            var v2 = new Vector4(x2, y2, z2, w2);

            // Act
            var v3 = v1.Add(v2);

            // Assert
            Assert.That(v3.X, Is.EqualTo(x3));
            Assert.That(v3.Y, Is.EqualTo(y3));
            Assert.That(v3.Z, Is.EqualTo(z3));
            Assert.That(v3.W, Is.EqualTo(w3));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 4, 2, 0, 0, 0, 0, 5, 3, 4, 2)]
        [TestCase(-1, 1, 2, -3, -3, -3, -3, -3, 2, 4, 5, 0)]
        [TestCase(20.069, 46.724, 84.883, 9.828, 74.225, 18.948, 87.805, 63.155, -54.156, 27.776, -2.922, -53.327)]
        public void Subtract(double x1, double y1, double z1, double w1, double x2, double y2, double z2, double w2,
            double x3, double y3, double z3, double w3)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);
            var v2 = new Vector4(x2, y2, z2, w2);

            // Act
            var v3 = v1.Subtract(v2);

            // Assert
            Assert.That(v3.X, Is.EqualTo(x3));
            Assert.That(v3.Y, Is.EqualTo(y3));
            Assert.That(v3.Z, Is.EqualTo(z3));
            Assert.That(v3.W, Is.EqualTo(w3));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 1, 1, 2, 3, 4)]
        [TestCase(1, -2, 3, -4, 2, 2, -4, 6, -8)]
        [TestCase(89.184, 78.899, 3.166, 89.235, 91.237, 8136.880608, 7198.508063, 288.856342, 8141.533695)]
        public void Multiply(double x1, double y1, double z1, double w1, double s, double x2,
            double y2, double z2, double w2)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);

            // Act
            var v2 = v1.Multiply(s);

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
            Assert.That(v2.W, Is.EqualTo(w2));
        }

        [TestCase(1, 2, 3, 4, 1, 1, 2, 3, 4)]
        [TestCase(1, -2, 3, -4, 2, 0.5, -1, 1.5, -2)]
        [TestCase(8136.880608, 7198.508063, 90.099, 12.231, 91.237, 89.184, 78.899, 0.9875269901, 0.134057)]
        public void Divide(double x1, double y1, double z1, double w1, double s, double x2,
            double y2, double z2, double w2)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);

            // Act
            var v2 = v1.Divide(s);

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
            Assert.That(v2.W, Is.EqualTo(w2));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 4, 2, 5, 3, 4, 2, 54)]
        [TestCase(-1, 1, 0, 0.5, -3, -3, 0, 2, 1)]
        [TestCase(0, 0, 0, 0, 7, -3, 12, 2, 0)]
        [TestCase(20.069, 46.724, 69.044, 39.670, 74.225, 18.948, 26.323, 28.330, 5316.244189)]
        public void Dot(double x1, double y1, double z1, double w1, double x2, double y2, double z2, double w2,
            double expected)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);
            var v2 = new Vector4(x2, y2, z2, w2);

            // Act
            var actual = v1.Dot(v2);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 4, 2, 5, 3, 4, 2, 0)]
        [TestCase(-1, 1, 2, 0.5, -3, -3, 4, 1.23, 4.953069755)]
        [TestCase(0, 0, 0, 0, 7, -3, 0.8, 12, 14.23516772)]
        [TestCase(20.069, 46.724, 46.883, 91.734, 74.225, 18.948, 4.096, 45.032, 87.84180488)]
        public void Distance_ShouldReturnDistanceBetweenPoints(double x1, double y1, double z1, double w1, double x2, double y2, double z2, double w2,
            double expected)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);
            var v2 = new Vector4(x2, y2, z2, w2);

            // Act
            var actual = v1.Distance(v2);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 4, 2, 5, 3, 4, 2, 0)]
        [TestCase(-1, 1, 2, 0.5, -3, -3, 4, 1.23, 24.5329)]
        [TestCase(0, 0, 0, 0, 7, -3, 0.8, 12, 202.64)]
        [TestCase(20.069, 46.724, 46.883, 91.734, 74.225, 18.948, 4.096, 45.032, 7716.1826845)]
        public void DistanceSquared_ShouldReturnDistanceBetweenPointsSquared(double x1, double y1, double z1, double w1, double x2, double y2, double z2,
            double w2, double expected)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);
            var v2 = new Vector4(x2, y2, z2, w2);

            // Act
            var actual = v1.DistanceSquared(v2);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 0, 0, 0, 15, 15, 0, 0, 0)]
        [TestCase(0, -3, 0, 0, 12, 0, -12, 0, 0)]
        [TestCase(0, 0, 0.13, 0, 7.38, 0, 0, 7.38, 0)]
        [TestCase(0, 0, 0, -1.2, 0.35, 0, 0, 0, -0.35)]
        [TestCase(-20.069, 46.724, 46.883, 91.734, 18.948, -3.309923, 7.706057, 7.732280, 15.129429)]
        public void OfLength(double x1, double y1, double z1, double w1, double length, double x2, double y2, double z2, double w2)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);

            // Act
            var v2 = v1.OfLength(length);

            // Assert
            Assert.That(v2.Length, Is.EqualTo(length));
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
            Assert.That(v2.W, Is.EqualTo(w2));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 0, 0, 0, 10, 5, 0, 0, 0, 5)]
        [TestCase(5, 0, 0, 0, 2, 2, 0, 0, 0, 2)]
        [TestCase(0, 5, 0, 0, 10, 0, 5, 0, 0, 5)]
        [TestCase(0, 5, 0, 0, 2, 0, 2, 0, 0, 2)]
        [TestCase(0, 0, 5, 0, 10, 0, 0, 5, 0, 5)]
        [TestCase(0, 0, 5, 0, 2, 0, 0, 2, 0, 2)]
        [TestCase(0, 0, 0, 5, 10, 0, 0, 0, 5, 5)]
        [TestCase(0, 0, 0, 5, 2, 0, 0, 0, 2, 2)]
        [TestCase(-20.069, 46.724, 46.883, 91.734, 160, -20.069, 46.724, 46.883, 91.734, 114.887072)]
        [TestCase(-20.069, 46.724, 46.883, 91.734, 80, -13.974766, 32.535601, 32.646318, 63.877683, 80)]
        public void Clamp_Max(double x1, double y1, double z1, double w1, double maxLength, double x2, double y2, double z2, double w2, double expectedLength)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);

            // Act
            var v2 = v1.Clamp(maxLength);

            // Assert
            Assert.That(v2.Length, Is.EqualTo(expectedLength));
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
            Assert.That(v2.W, Is.EqualTo(w2));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 0, 0, 0, 4, 10, 5, 0, 0, 0, 5)]
        [TestCase(5, 0, 0, 0, 8, 10, 8, 0, 0, 0, 8)]
        [TestCase(5, 0, 0, 0, 1, 2, 2, 0, 0, 0, 2)]
        [TestCase(0, 5, 0, 0, 4, 10, 0, 5, 0, 0, 5)]
        [TestCase(0, 5, 0, 0, 8, 10, 0, 8, 0, 0, 8)]
        [TestCase(0, 5, 0, 0, 1, 2, 0, 2, 0, 0, 2)]
        [TestCase(0, 0, 5, 0, 4, 10, 0, 0, 5, 0, 5)]
        [TestCase(0, 0, 5, 0, 8, 10, 0, 0, 8, 0, 8)]
        [TestCase(0, 0, 5, 0, 1, 2, 0, 0, 2, 0, 2)]
        [TestCase(0, 0, 0, 5, 4, 10, 0, 0, 0, 5, 5)]
        [TestCase(0, 0, 0, 5, 8, 10, 0, 0, 0, 8, 8)]
        [TestCase(0, 0, 0, 5, 1, 2, 0, 0, 0, 2, 2)]
        [TestCase(-20.069, 46.724, 46.883, 91.734, 80, 160, -20.069, 46.724, 46.883, 91.734, 114.887072)]
        [TestCase(-20.069, 46.724, 46.883, 91.734, 120, 160, -20.962149, 48.803402, 48.969478, 95.816524, 120)]
        [TestCase(-20.069, 46.724, 46.883, 91.734, 40, 80, -13.974766, 32.535601, 32.646318, 63.877683, 80)]
        public void Clamp_MinMax(double x1, double y1, double z1, double w1, double minLength, double maxLength, double x2, double y2, double z2, double w2,
            double expectedLength)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);

            // Act
            var v2 = v1.Clamp(minLength, maxLength);

            // Assert
            Assert.That(v2.Length, Is.EqualTo(expectedLength));
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
            Assert.That(v2.W, Is.EqualTo(w2));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(-20.069, 46.724, 46.883, 91.734, 27.113386, 27.113386, 46.724, 46.883, 91.734)]
        public void WithX(double x1, double y1, double z1, double w1, double newX, double x2, double y2, double z2, double w2)
        {
            // Arrange
            var v = new Vector4(x1, y1, z1, w1);
            var expected = new Vector4(x2, y2, z2, w2);

            // Act
            var actual = v.WithX(newX);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(-20.069, 46.724, 46.883, 91.734, 27.113386, -20.069, 27.113386, 46.883, 91.734)]
        public void WithY(double x1, double y1, double z1, double w1, double newY, double x2, double y2, double z2, double w2)
        {
            // Arrange
            var v = new Vector4(x1, y1, z1, w1);
            var expected = new Vector4(x2, y2, z2, w2);

            // Act
            var actual = v.WithY(newY);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(-20.069, 46.724, 46.883, 91.734, 27.113386, -20.069, 46.724, 27.113386, 91.734)]
        public void WithZ(double x1, double y1, double z1, double w1, double newZ, double x2, double y2, double z2, double w2)
        {
            // Arrange
            var v = new Vector4(x1, y1, z1, w1);
            var expected = new Vector4(x2, y2, z2, w2);

            // Act
            var actual = v.WithZ(newZ);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(-20.069, 46.724, 46.883, 91.734, 27.113386, -20.069, 46.724, 46.883, 27.113386)]
        public void WithW(double x1, double y1, double z1, double w1, double newW, double x2, double y2, double z2, double w2)
        {
            // Arrange
            var v = new Vector4(x1, y1, z1, w1);
            var expected = new Vector4(x2, y2, z2, w2);

            // Act
            var actual = v.WithW(newW);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ToArray(
            [Random(-100d, 100d, 1)] double x,
            [Random(-100d, 100d, 1)] double y,
            [Random(-100d, 100d, 1)] double z,
            [Random(-100d, 100d, 1)] double w)
        {
            // Arrange
            var v = new Vector4(x, y, z, w);

            // Act
            var actual = v.ToArray();

            // Assert
            Assert.That(actual[0], Is.EqualTo(x));
            Assert.That(actual[1], Is.EqualTo(y));
            Assert.That(actual[2], Is.EqualTo(z));
            Assert.That(actual[3], Is.EqualTo(w));
        }

        [TestCase(1, 2, 3, 4, 1, 2, 3, 4, true)]
        [TestCase(1, 2, 3, 4, 0, 2, 3, 4, false)]
        [TestCase(1, 2, 3, 4, 1, 0, 3, 4, false)]
        [TestCase(1, 2, 3, 4, 1, 2, 0, 4, false)]
        [TestCase(1, 2, 3, 4, 1, 2, 3, 0, false)]
        [TestCase(60.86360580, 4.47213595, 8.910, 58.832, 60.86360580, 4.47213595, 8.910, 58.832, true)]
        [TestCase(60.86360580, 4.47213595, 8.910, 58.832, 60.86360580, 4.47213596, 8.910, 58.832, false)]
        public void EqualityMembers_ShouldEqualVector4_WhenComponentsAreEqual(double x1, double y1, double z1, double w1, double x2, double y2, double z2,
            double w2, bool expectedIsEqual)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);
            var v2 = new Vector4(x2, y2, z2, w2);

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(v1, v2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }

        [TestCase(0, 0, 0, 0, "X: 0, Y: 0, Z: 0, W: 0")]
        [TestCase(74.025, -27.169, -25.159, 55.412, "X: 74.025, Y: -27.169, Z: -25.159, W: 55.412")]
        [SetCulture("")]
        public void ToString_Test(double x, double y, double z, double w, string expected)
        {
            // Arrange
            var v = new Vector4(x, y, z, w);

            // Act
            var actual = v.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(1, 2, 3, 4)]
        [TestCase(91.3376, 63.2359, 9.7540, 27.8498)]
        public void ToVector2(double x, double y, double z, double w)
        {
            // Arrange
            var vector4 = new Vector4(x, y, z, w);

            // Act
            var vector2 = vector4.ToVector2();

            // Assert
            Assert.That(vector2.X, Is.EqualTo(x));
            Assert.That(vector2.Y, Is.EqualTo(y));
        }

        [TestCase(1, 2, 3, 4)]
        [TestCase(91.3376, 63.2359, 9.7540, 27.8498)]
        public void ToVector3(double x, double y, double z, double w)
        {
            // Arrange
            var vector4 = new Vector4(x, y, z, w);

            // Act
            var vector3 = vector4.ToVector3();

            // Assert
            Assert.That(vector3.X, Is.EqualTo(x));
            Assert.That(vector3.Y, Is.EqualTo(y));
            Assert.That(vector3.Z, Is.EqualTo(z));
        }

        #endregion

        #region Operators

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(0, 0, 0, 0, 5, 3, 4, 2, 5, 3, 4, 2)]
        [TestCase(2, 4, 1, -2, -3, -3, -3, -3, -1, 1, -2, -5)]
        [TestCase(-7, 3, -2, 5, 7, -3, 2, -5, 0, 0, 0, 0)]
        [TestCase(96.747, 71.087, 93.255, 77.986, 65.603, 91.750, 60.467, 36.633, 162.35, 162.837, 153.722, 114.619)]
        public void AdditionOperator(double x1, double y1, double z1, double w1, double x2, double y2, double z2,
            double w2,
            double x3, double y3, double z3, double w3)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);
            var v2 = new Vector4(x2, y2, z2, w2);

            // Act
            var v3 = v1 + v2;

            // Assert
            Assert.That(v3.X, Is.EqualTo(x3));
            Assert.That(v3.Y, Is.EqualTo(y3));
            Assert.That(v3.Z, Is.EqualTo(z3));
            Assert.That(v3.W, Is.EqualTo(w3));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(5, 3, 4, 2, 5, 3, 4, 2, 0, 0, 0, 0)]
        [TestCase(-1, 1, 2, -3, -3, -3, -3, -3, 2, 4, 5, 0)]
        [TestCase(0, 0, 0, 0, 7, -3, 1, -2, -7, 3, -1, 2)]
        [TestCase(20.069, 46.724, 84.883, 9.828, 74.225, 18.948, 87.805, 63.155, -54.156, 27.776, -2.922, -53.327)]
        public void SubtractionOperator(double x1, double y1, double z1, double w1, double x2, double y2, double z2,
            double w2,
            double x3, double y3, double z3, double w3)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);
            var v2 = new Vector4(x2, y2, z2, w2);

            // Act
            var v3 = v1 - v2;

            // Assert
            Assert.That(v3.X, Is.EqualTo(x3));
            Assert.That(v3.Y, Is.EqualTo(y3));
            Assert.That(v3.Z, Is.EqualTo(z3));
            Assert.That(v3.W, Is.EqualTo(w3));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 1, 1, 1, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 5, 10, 15, 20)]
        [TestCase(5, 0, -0.5, 50, 2, 10, 0, -1, 100)]
        [TestCase(20, -8, 0.3, -0.07, 0.5, 10, -4, 0.15, -0.035)]
        [TestCase(89.184, 78.899, 3.166, 89.235, 91.237, 8136.880608, 7198.508063, 288.856342, 8141.533695)]
        public void MultiplicationOperator(double x1, double y1, double z1, double w1, double s, double x2,
            double y2, double z2, double w2)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);

            // Act
            var v2 = v1 * s;

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
            Assert.That(v2.W, Is.EqualTo(w2));
        }

        [TestCase(3, 6, 9, 12, 3, 1, 2, 3, 4)]
        [TestCase(10, 0, -2, -0.5, 2, 5, 0, -1, -0.25)]
        [TestCase(10, -4, 3, 3.14, 0.5, 20, -8, 6, 6.28)]
        [TestCase(8136.880608, 7198.508063, 90.099, 12.231, 91.237, 89.184, 78.899, 0.9875269901, 0.134057)]
        public void DivisionOperator(double x1, double y1, double z1, double w1, double s, double x2,
            double y2, double z2, double w2)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);

            // Act
            var v2 = v1 / s;

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
            Assert.That(v2.W, Is.EqualTo(w2));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, -2, 3, -4, -1, 2, -3, 4)]
        [TestCase(89.727, 59.751, 41.960, 40.845, -89.727, -59.751, -41.960, -40.845)]
        public void OppositionOperator(double x1, double y1, double z1, double w1, double x2, double y2, double z2,
            double w2)
        {
            // Arrange
            var v1 = new Vector4(x1, y1, z1, w1);

            // Act
            var actual = -v1;

            // Assert
            Assert.That(actual.X, Is.EqualTo(x2));
            Assert.That(actual.Y, Is.EqualTo(y2));
            Assert.That(actual.Z, Is.EqualTo(z2));
            Assert.That(actual.W, Is.EqualTo(w2));
        }

        #endregion
    }
}