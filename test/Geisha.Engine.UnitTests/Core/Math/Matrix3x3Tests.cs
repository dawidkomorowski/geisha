﻿using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Geisha.Engine.UnitTests.Core.Math
{
    [TestFixture]
    [DefaultFloatingPointTolerance(Epsilon)]
    public class Matrix3x3Tests
    {
        private const double Epsilon = 0.0001;
        private static IEqualityComparer<Matrix3x3> Matrix3x3Comparer => CommonEqualityComparer.Matrix3x3(Epsilon);

        #region Static properties

        [Test]
        public void Zero()
        {
            // Arrange
            // Act
            var m = Matrix3x3.Zero;

            // Assert
            Assert.That(m.M11, Is.Zero);
            Assert.That(m.M12, Is.Zero);
            Assert.That(m.M13, Is.Zero);

            Assert.That(m.M21, Is.Zero);
            Assert.That(m.M22, Is.Zero);
            Assert.That(m.M23, Is.Zero);

            Assert.That(m.M31, Is.Zero);
            Assert.That(m.M32, Is.Zero);
            Assert.That(m.M33, Is.Zero);
        }

        [Test]
        public void Identity()
        {
            // Arrange
            // Act
            var m = Matrix3x3.Identity;

            // Assert
            Assert.That(m.M11, Is.EqualTo(1));
            Assert.That(m.M12, Is.Zero);
            Assert.That(m.M13, Is.Zero);

            Assert.That(m.M21, Is.Zero);
            Assert.That(m.M22, Is.EqualTo(1));
            Assert.That(m.M23, Is.Zero);

            Assert.That(m.M31, Is.Zero);
            Assert.That(m.M32, Is.Zero);
            Assert.That(m.M33, Is.EqualTo(1));
        }

        #endregion

        #region Properties

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            -1, 2, -3, 4, -5, 6, -7, 8, -9)]
        [TestCase(48.204, 67.909, 16.677, -61.618, 31.834, 19.980, -92.613, -93.628, -21.756,
            -48.204, -67.909, -16.677, 61.618, -31.834, -19.980, 92.613, 93.628, 21.756)]
        public void Opposite(double m1_11, double m1_12, double m1_13, double m1_21, double m1_22, double m1_23,
            double m1_31, double m1_32, double m1_33, double m2_11, double m2_12, double m2_13, double m2_21,
            double m2_22, double m2_23, double m2_31, double m2_32, double m2_33)
        {
            // Arrange
            var m = new Matrix3x3(m1_11, m1_12, m1_13, m1_21, m1_22, m1_23, m1_31, m1_32, m1_33);

            // Act
            var actual = m.Opposite;

            // Assert
            Assert.That(actual.M11, Is.EqualTo(m2_11));
            Assert.That(actual.M12, Is.EqualTo(m2_12));
            Assert.That(actual.M13, Is.EqualTo(m2_13));

            Assert.That(actual.M21, Is.EqualTo(m2_21));
            Assert.That(actual.M22, Is.EqualTo(m2_22));
            Assert.That(actual.M23, Is.EqualTo(m2_23));

            Assert.That(actual.M31, Is.EqualTo(m2_31));
            Assert.That(actual.M32, Is.EqualTo(m2_32));
            Assert.That(actual.M33, Is.EqualTo(m2_33));
        }


        [TestCase(48.204, 67.909, 16.677, -61.618, 31.834, 19.980, -92.613, -93.628, -21.756)]
        public void Indexer(double m11, double m12, double m13, double m21, double m22, double m23, double m31,
            double m32, double m33)
        {
            // Arrange
            var m = new Matrix3x3(m11, m12, m13, m21, m22, m23, m31, m32, m33);

            // Act
            // Assert
            Assert.That(m[0, 0], Is.EqualTo(m11));
            Assert.That(m[0, 1], Is.EqualTo(m12));
            Assert.That(m[0, 2], Is.EqualTo(m13));

            Assert.That(m[1, 0], Is.EqualTo(m21));
            Assert.That(m[1, 1], Is.EqualTo(m22));
            Assert.That(m[1, 2], Is.EqualTo(m23));

            Assert.That(m[2, 0], Is.EqualTo(m31));
            Assert.That(m[2, 1], Is.EqualTo(m32));
            Assert.That(m[2, 2], Is.EqualTo(m33));
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(3, 2)]
        [TestCase(2, 3)]
        public void Indexer_ShouldThrowIndexOutOfRangeException_GivenIndexOutOfRange(int ix, int iy)
        {
            // Arrange
            var m = new Matrix3x3();

            // Act
            // Assert
            Assert.Throws<IndexOutOfRangeException>(() => { _ = m[ix, iy]; });
        }

        [TestCase(1, 0, 0, 0, 1, 0, 0, 0, 1, true)]
        [TestCase(0.34729635533386083, 1.969615506024416, -200, -1.969615506024416, 0.34729635533386083, -200, 0, 0, 1, true)]
        [TestCase(1, 0, 0, 0, 1, 0, 999, 0, 1, false)]
        [TestCase(1, 0, 0, 0, 1, 0, 0, 999, 1, false)]
        [TestCase(1, 0, 0, 0, 1, 0, 0, 0, 999, false)]
        [TestCase(1, 1, 0, 0, 1, 0, 0, 0, 1, false)]
        public void IsTRS_ShouldReturnTrue_WhenMatrixIsComposedOfTranslationRotationAndScale(
            double m11, double m12, double m13,
            double m21, double m22, double m23,
            double m31, double m32, double m33,
            bool expected
        )
        {
            // Arrange
            var m = new Matrix3x3(m11, m12, m13, m21, m22, m23, m31, m32, m33);

            // Act
            var actual = m.IsTRS;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        #endregion

        #region Constructors

        [Test]
        public void ParameterlessConstructor()
        {
            // Arrange
            // Act
            var m = new Matrix3x3();

            // Assert
            Assert.That(m.M11, Is.Zero);
            Assert.That(m.M12, Is.Zero);
            Assert.That(m.M13, Is.Zero);

            Assert.That(m.M21, Is.Zero);
            Assert.That(m.M22, Is.Zero);
            Assert.That(m.M23, Is.Zero);

            Assert.That(m.M31, Is.Zero);
            Assert.That(m.M32, Is.Zero);
            Assert.That(m.M33, Is.Zero);
        }

        [Test]
        public void ConstructorFromNineDoubles()
        {
            // Arrange
            // Act
            var m = new Matrix3x3(1, 2, 3, 4, 5, 6, 7, 8, 9);

            // Assert
            Assert.That(m.M11, Is.EqualTo(1));
            Assert.That(m.M12, Is.EqualTo(2));
            Assert.That(m.M13, Is.EqualTo(3));

            Assert.That(m.M21, Is.EqualTo(4));
            Assert.That(m.M22, Is.EqualTo(5));
            Assert.That(m.M23, Is.EqualTo(6));

            Assert.That(m.M31, Is.EqualTo(7));
            Assert.That(m.M32, Is.EqualTo(8));
            Assert.That(m.M33, Is.EqualTo(9));
        }

        [Test]
        public void ConstructorFromArray()
        {
            // Arrange
            // Act
            var m = new Matrix3x3(new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            // Assert
            Assert.That(m.M11, Is.EqualTo(1));
            Assert.That(m.M12, Is.EqualTo(2));
            Assert.That(m.M13, Is.EqualTo(3));

            Assert.That(m.M21, Is.EqualTo(4));
            Assert.That(m.M22, Is.EqualTo(5));
            Assert.That(m.M23, Is.EqualTo(6));

            Assert.That(m.M31, Is.EqualTo(7));
            Assert.That(m.M32, Is.EqualTo(8));
            Assert.That(m.M33, Is.EqualTo(9));
        }

        [TestCase(4)]
        [TestCase(16)]
        public void ConstructorFromArrayThrowsException_GivenArrayOfLengthDifferentFromNine(int length)
        {
            // Arrange
            var array = new double[length];

            // Act
            // Assert
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new Matrix3x3(array));
        }

        #endregion

        #region Methods

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 2, 3, 4, 5, 6, 7, 8, 9)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            2, 4, -8, -16, 32, -64, 128, 256, -512,
            3, 2, -5, -20, 37, -70, 135, 248, -503)]
        [TestCase(51.5367, -72.2349, 13.4338, -65.7531, -40.0080, -6.0467, -95.0915, 83.1871, -8.4247,
            -97.4222, -55.3542, -43.0002, -57.0838, 51.5458, 49.1806, -99.6850, -33.0682, 7.1037,
            -45.8855, -127.5891, -29.5664, -122.8369, 11.5378, 43.1339, -194.7765, 50.1189, -1.3210)]
        public void Add(double m1_11, double m1_12, double m1_13, double m1_21, double m1_22, double m1_23,
            double m1_31, double m1_32, double m1_33, double m2_11, double m2_12, double m2_13, double m2_21,
            double m2_22, double m2_23, double m2_31, double m2_32, double m2_33, double m3_11, double m3_12,
            double m3_13, double m3_21, double m3_22, double m3_23, double m3_31, double m3_32, double m3_33)
        {
            // Arrange
            var m1 = new Matrix3x3(m1_11, m1_12, m1_13, m1_21, m1_22, m1_23, m1_31, m1_32, m1_33);
            var m2 = new Matrix3x3(m2_11, m2_12, m2_13, m2_21, m2_22, m2_23, m2_31, m2_32, m2_33);

            // Act
            var m3 = m1.Add(m2);

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m3_11));
            Assert.That(m3.M12, Is.EqualTo(m3_12));
            Assert.That(m3.M13, Is.EqualTo(m3_13));

            Assert.That(m3.M21, Is.EqualTo(m3_21));
            Assert.That(m3.M22, Is.EqualTo(m3_22));
            Assert.That(m3.M23, Is.EqualTo(m3_23));

            Assert.That(m3.M31, Is.EqualTo(m3_31));
            Assert.That(m3.M32, Is.EqualTo(m3_32));
            Assert.That(m3.M33, Is.EqualTo(m3_33));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 2, 3, 4, 5, 6, 7, 8, 9)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            2, 4, -8, -16, 32, -64, 128, 256, -512,
            -1, -6, 11, 12, -27, 58, -121, -264, 521)]
        [TestCase(59.0905, 22.8688, 86.3711, 45.9380, 83.4189, -7.8069, -5.0340, 1.5645, -66.9043,
            1.9621, -61.7390, -9.7698, -43.5176, 52.0129, -90.8052, 83.2221, 86.3868, -10.8017,
            57.1284, 84.6078, 96.1409, 89.4556, 31.4060, 82.9983, -88.2561, -84.8223, -56.1026)]
        public void Subtract(double m1_11, double m1_12, double m1_13, double m1_21, double m1_22, double m1_23,
            double m1_31, double m1_32, double m1_33, double m2_11, double m2_12, double m2_13, double m2_21,
            double m2_22, double m2_23, double m2_31, double m2_32, double m2_33, double m3_11, double m3_12,
            double m3_13, double m3_21, double m3_22, double m3_23, double m3_31, double m3_32, double m3_33)
        {
            // Arrange
            var m1 = new Matrix3x3(m1_11, m1_12, m1_13, m1_21, m1_22, m1_23, m1_31, m1_32, m1_33);
            var m2 = new Matrix3x3(m2_11, m2_12, m2_13, m2_21, m2_22, m2_23, m2_31, m2_32, m2_33);

            // Act
            var m3 = m1.Subtract(m2);

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m3_11));
            Assert.That(m3.M12, Is.EqualTo(m3_12));
            Assert.That(m3.M13, Is.EqualTo(m3_13));

            Assert.That(m3.M21, Is.EqualTo(m3_21));
            Assert.That(m3.M22, Is.EqualTo(m3_22));
            Assert.That(m3.M23, Is.EqualTo(m3_23));

            Assert.That(m3.M31, Is.EqualTo(m3_31));
            Assert.That(m3.M32, Is.EqualTo(m3_32));
            Assert.That(m3.M33, Is.EqualTo(m3_33));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0,
            0,
            0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            0,
            0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1,
            1, 2, 3, 4, 5, 6, 7, 8, 9)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            2,
            2, -4, 6, -8, 10, -12, 14, -16, 18)]
        [TestCase(37.0865, 40.2434, -91.0195, -89.0929, 31.8019, 90.9098, 85.6377, 60.8635, -59.1594,
            -15.4159,
            -571.7218, -620.3882, 1403.1475, 1373.4472, -490.2549, -1401.4564, -1320.1822, -938.2656, 911.9954)]
        public void MultiplyByScalar(double m1_11, double m1_12, double m1_13, double m1_21, double m1_22, double m1_23,
            double m1_31, double m1_32, double m1_33, double s, double m2_11, double m2_12, double m2_13, double m2_21,
            double m2_22, double m2_23, double m2_31, double m2_32, double m2_33)
        {
            // Arrange
            var m1 = new Matrix3x3(m1_11, m1_12, m1_13, m1_21, m1_22, m1_23, m1_31, m1_32, m1_33);

            // Act
            var m3 = m1.Multiply(s);

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m2_11));
            Assert.That(m3.M12, Is.EqualTo(m2_12));
            Assert.That(m3.M13, Is.EqualTo(m2_13));

            Assert.That(m3.M21, Is.EqualTo(m2_21));
            Assert.That(m3.M22, Is.EqualTo(m2_22));
            Assert.That(m3.M23, Is.EqualTo(m2_23));

            Assert.That(m3.M31, Is.EqualTo(m2_31));
            Assert.That(m3.M32, Is.EqualTo(m2_32));
            Assert.That(m3.M33, Is.EqualTo(m2_33));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1, 0, 0, 0, 1, 0, 0, 0, 1,
            1, 2, 3, 4, 5, 6, 7, 8, 9)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            2, 4, -8, -16, 32, -64, 128, 256, -512,
            418, 708, -1416, -856, -1392, 2784, 1294, 2076, -4152)]
        [TestCase(35.8128, 92.9169, 43.1218, 48.8988, 46.6757, 70.2530, -25.5962, 25.4008, -40.2330,
            -7.0450, -13.6652, 60.7340, 84.7333, -85.8402, -54.3045, 67.9880, -19.9834, 16.2325,
            10552.6193, -9327.1145, -2170.7766, 8386.8550, -6078.7571, 1575.5010, -402.7424, -1026.6404, -3587.0195)]
        public void MultiplyByMatrix(double m1_11, double m1_12, double m1_13, double m1_21, double m1_22, double m1_23,
            double m1_31, double m1_32, double m1_33, double m2_11, double m2_12, double m2_13, double m2_21,
            double m2_22, double m2_23, double m2_31, double m2_32, double m2_33, double m3_11, double m3_12,
            double m3_13, double m3_21, double m3_22, double m3_23, double m3_31, double m3_32, double m3_33)
        {
            // Arrange
            var m1 = new Matrix3x3(m1_11, m1_12, m1_13, m1_21, m1_22, m1_23, m1_31, m1_32, m1_33);
            var m2 = new Matrix3x3(m2_11, m2_12, m2_13, m2_21, m2_22, m2_23, m2_31, m2_32, m2_33);

            // Act
            var m3 = m1.Multiply(m2);

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m3_11));
            Assert.That(m3.M12, Is.EqualTo(m3_12));
            Assert.That(m3.M13, Is.EqualTo(m3_13));

            Assert.That(m3.M21, Is.EqualTo(m3_21));
            Assert.That(m3.M22, Is.EqualTo(m3_22));
            Assert.That(m3.M23, Is.EqualTo(m3_23));

            Assert.That(m3.M31, Is.EqualTo(m3_31));
            Assert.That(m3.M32, Is.EqualTo(m3_32));
            Assert.That(m3.M33, Is.EqualTo(m3_33));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0,
            0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            0, 0, 0,
            0, 0, 0)]
        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 2, 3,
            0, 0, 0)]
        [TestCase(1, 0, 0, 0, 1, 0, 0, 0, 1,
            1, 2, 3,
            1, 2, 3)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            2, 4, -8,
            -30, 60, -90)]
        [TestCase(85.6182, 13.7763, -58.8209, 28.1508, -83.6723, -36.6157, 73.1051, -13.8602, 80.6760,
            23.1238, -40.3491, 12.2021,
            706.2183, 3580.2670, 3234.1309)]
        public void MultiplyByVector(double m11, double m12, double m13, double m21, double m22, double m23, double m31,
            double m32, double m33, double x1, double y1, double z1, double x2, double y2, double z2)
        {
            // Arrange
            var m = new Matrix3x3(m11, m12, m13, m21, m22, m23, m31, m32, m33);
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var v2 = m.Multiply(v1);

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
        }

        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1,
            1, 2, 3, 4, 5, 6, 7, 8, 9)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            2,
            0.5, -1, 1.5, -2, 2.5, -3, 3.5, -4, 4.5)]
        [TestCase(38.0149, -2.9668, -97.5845, 21.3270, 47.2321, 55.5444, -38.2938, 33.3373, -84.6304,
            -54.2992,
            -0.7001, 0.0546, 1.7972, -0.3928, -0.8698, -1.0229, 0.7052, -0.6140, 1.5586)]
        public void DivideByScalar(double m1_11, double m1_12, double m1_13, double m1_21, double m1_22, double m1_23,
            double m1_31, double m1_32, double m1_33, double s, double m2_11, double m2_12, double m2_13, double m2_21,
            double m2_22, double m2_23, double m2_31, double m2_32, double m2_33)
        {
            // Arrange
            var m1 = new Matrix3x3(m1_11, m1_12, m1_13, m1_21, m1_22, m1_23, m1_31, m1_32, m1_33);

            // Act
            var m3 = m1.Divide(s);

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m2_11));
            Assert.That(m3.M12, Is.EqualTo(m2_12));
            Assert.That(m3.M13, Is.EqualTo(m2_13));

            Assert.That(m3.M21, Is.EqualTo(m2_21));
            Assert.That(m3.M22, Is.EqualTo(m2_22));
            Assert.That(m3.M23, Is.EqualTo(m2_23));

            Assert.That(m3.M31, Is.EqualTo(m2_31));
            Assert.That(m3.M32, Is.EqualTo(m2_32));
            Assert.That(m3.M33, Is.EqualTo(m2_33));
        }

        [Test]
        public void ToArray(
            [Random(-100d, 100d, 1)] double m11,
            [Random(-100d, 100d, 1)] double m12,
            [Random(-100d, 100d, 1)] double m13,
            [Random(-100d, 100d, 1)] double m21,
            [Random(-100d, 100d, 1)] double m22,
            [Random(-100d, 100d, 1)] double m23,
            [Random(-100d, 100d, 1)] double m31,
            [Random(-100d, 100d, 1)] double m32,
            [Random(-100d, 100d, 1)] double m33)
        {
            // Arrange
            var m = new Matrix3x3(m11, m12, m13, m21, m22, m23, m31, m32, m33);

            // Act
            var actual = m.ToArray();

            // Assert
            Assert.That(actual[0], Is.EqualTo(m11));
            Assert.That(actual[1], Is.EqualTo(m12));
            Assert.That(actual[2], Is.EqualTo(m13));

            Assert.That(actual[3], Is.EqualTo(m21));
            Assert.That(actual[4], Is.EqualTo(m22));
            Assert.That(actual[5], Is.EqualTo(m23));

            Assert.That(actual[6], Is.EqualTo(m31));
            Assert.That(actual[7], Is.EqualTo(m32));
            Assert.That(actual[8], Is.EqualTo(m33));
        }

        [Test]
        public void ToTransform_ShouldThrowException_WhenMatrixIsNotTRS()
        {
            // Arrange
            var matrix = new Matrix3x3(1, 1, 0, 0, 1, 0, 0, 0, 1);

            // Act
            // Assert
            Assert.That(() => matrix.ToTransform(), Throws.InvalidOperationException);
        }

        // Identity
        [TestCase(0, 0, 0, 1, 1,
            0, 0, 0, 1, 1)]
        // Translation
        [TestCase(10, 20, 30 * (System.Math.PI / 180), 2, 3,
            10, 20, 30 * (System.Math.PI / 180), 2, 3)]
        [TestCase(11, -21, 30 * (System.Math.PI / 180), 2, 3,
            11, -21, 30 * (System.Math.PI / 180), 2, 3)]
        [TestCase(-12, 22, 30 * (System.Math.PI / 180), 2, 3,
            -12, 22, 30 * (System.Math.PI / 180), 2, 3)]
        [TestCase(-13, -23, 30 * (System.Math.PI / 180), 2, 3,
            -13, -23, 30 * (System.Math.PI / 180), 2, 3)]
        // Rotation
        [TestCase(10, -20, 30 * (System.Math.PI / 180), 2, 3,
            10, -20, 30 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, 90 * (System.Math.PI / 180), 2, 3,
            10, -20, 90 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, 170 * (System.Math.PI / 180), 2, 3,
            10, -20, 170 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, 180 * (System.Math.PI / 180), 2, 3,
            10, -20, 180 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, 190 * (System.Math.PI / 180), 2, 3,
            10, -20, -170 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, 330 * (System.Math.PI / 180), 2, 3,
            10, -20, -30 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, 360 * (System.Math.PI / 180), 2, 3,
            10, -20, 0 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, 390 * (System.Math.PI / 180), 2, 3,
            10, -20, 30 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, -30 * (System.Math.PI / 180), 2, 3,
            10, -20, -30 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, -90 * (System.Math.PI / 180), 2, 3,
            10, -20, -90 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, -170 * (System.Math.PI / 180), 2, 3,
            10, -20, -170 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, -180 * (System.Math.PI / 180), 2, 3,
            10, -20, -180 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, -190 * (System.Math.PI / 180), 2, 3,
            10, -20, 170 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, -330 * (System.Math.PI / 180), 2, 3,
            10, -20, 30 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, -360 * (System.Math.PI / 180), 2, 3,
            10, -20, 0 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, -390 * (System.Math.PI / 180), 2, 3,
            10, -20, -30 * (System.Math.PI / 180), 2, 3)]
        // Scale
        [TestCase(10, -20, 30 * (System.Math.PI / 180), 0.2, 0.3,
            10, -20, 30 * (System.Math.PI / 180), 0.2, 0.3)]
        [TestCase(10, -20, 30 * (System.Math.PI / 180), -2, 3,
            10, -20, 30 * (System.Math.PI / 180), -2, 3)]
        [TestCase(10, -20, 30 * (System.Math.PI / 180), 2, -3,
            10, -20, -150 * (System.Math.PI / 180), -2, 3)]
        [TestCase(10, -20, 30 * (System.Math.PI / 180), -2, -3,
            10, -20, -150 * (System.Math.PI / 180), 2, 3)]
        [TestCase(10, -20, 30 * (System.Math.PI / 180), 0, 1,
            10, -20, 0 * (System.Math.PI / 180), 0, 1)]
        [TestCase(10, -20, 30 * (System.Math.PI / 180), 1, 0,
            10, -20, 30 * (System.Math.PI / 180), 1, 0)]
        [TestCase(10, -20, 30 * (System.Math.PI / 180), 0, 0,
            10, -20, 0 * (System.Math.PI / 180), 0, 0)]
        public void ToTransform_ShouldReturnTransform2D_WhenMatrixIsTRS(
            double tx, double ty, double r, double sx, double sy,
            double expectedTx, double expectedTy, double expectedR, double expectedSx, double expectedSy
        )
        {
            // Arrange
            var matrix = new Transform2D(new Vector2(tx, ty), r, new Vector2(sx, sy)).ToMatrix();

            // Act
            var actual = matrix.ToTransform();

            // Assert
            Assert.That(actual.Translation.X, Is.EqualTo(expectedTx));
            Assert.That(actual.Translation.Y, Is.EqualTo(expectedTy));
            Assert.That(actual.Rotation, Is.EqualTo(expectedR));
            Assert.That(actual.Scale.X, Is.EqualTo(expectedSx));
            Assert.That(actual.Scale.Y, Is.EqualTo(expectedSy));
        }

        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1, 2, 3, 4, 5, 6, 7, 8, 9, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            0, 2, 3, 4, 5, 6, 7, 8, 9, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1, 0, 3, 4, 5, 6, 7, 8, 9, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1, 2, 0, 4, 5, 6, 7, 8, 9, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1, 2, 3, 0, 5, 6, 7, 8, 9, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1, 2, 3, 4, 0, 6, 7, 8, 9, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1, 2, 3, 4, 5, 0, 7, 8, 9, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1, 2, 3, 4, 5, 6, 0, 8, 9, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1, 2, 3, 4, 5, 6, 7, 0, 9, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1, 2, 3, 4, 5, 6, 7, 8, 0, false)]
        [TestCase(48.204, 67.909, 16.677, -61.618, 31.834, 19.980, -92.613, -93.628, -21.756,
            48.204, 67.909, 16.677, -61.618, 31.834, 19.980, -92.613, -93.628, -21.756, true)]
        [TestCase(48.204, 67.909, 16.677, -61.618, 31.834, 19.980, -92.613, -93.628, -21.756,
            48.204, 67.909, 16.677, -61.618, 31.834, 19.980, -92.613, -93.628, -21.7561, false)]
        public void EqualityMembers_ShouldEqualMatrix3x3_WhenComponentsAreEqual(double m1_11, double m1_12, double m1_13, double m1_21, double m1_22,
            double m1_23, double m1_31, double m1_32, double m1_33, double m2_11, double m2_12, double m2_13, double m2_21, double m2_22, double m2_23,
            double m2_31, double m2_32, double m2_33, bool expectedIsEqual)
        {
            // Arrange
            var m1 = new Matrix3x3(m1_11, m1_12, m1_13, m1_21, m1_22, m1_23, m1_31, m1_32, m1_33);
            var m2 = new Matrix3x3(m2_11, m2_12, m2_13, m2_21, m2_22, m2_23, m2_31, m2_32, m2_33);

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(m1, m2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }

        #endregion

        #region Static methods

        [TestCase(0, 0,
            1, 0, 0, 0, 1, 0, 0, 0, 1)]
        [TestCase(2, -3,
            1, 0, 2, 0, 1, -3, 0, 0, 1)]
        [TestCase(-56.150, 70.203,
            1, 0, -56.150, 0, 1, 70.203, 0, 0, 1)]
        public void CreateTranslation(double tx, double ty, double m11, double m12, double m13, double m21, double m22,
            double m23, double m31, double m32, double m33)
        {
            // Arrange
            var translationVector = new Vector2(tx, ty);

            // Act
            var translationMatrix = Matrix3x3.CreateTranslation(translationVector);

            // Assert
            Assert.That(translationMatrix.M11, Is.EqualTo(m11));
            Assert.That(translationMatrix.M12, Is.EqualTo(m12));
            Assert.That(translationMatrix.M13, Is.EqualTo(m13));

            Assert.That(translationMatrix.M21, Is.EqualTo(m21));
            Assert.That(translationMatrix.M22, Is.EqualTo(m22));
            Assert.That(translationMatrix.M23, Is.EqualTo(m23));

            Assert.That(translationMatrix.M31, Is.EqualTo(m31));
            Assert.That(translationMatrix.M32, Is.EqualTo(m32));
            Assert.That(translationMatrix.M33, Is.EqualTo(m33));
        }

        [TestCase(0,
            1, 0, 0, 0, 1, 0, 0, 0, 1)]
        [TestCase(System.Math.PI,
            -1, 0, 0, 0, -1, 0, 0, 0, 1)]
        [TestCase(-56.150,
            0.92157, -0.38819, 0, 0.38819, 0.92157, 0, 0, 0, 1)]
        public void CreateRotation(double angle, double m11, double m12, double m13, double m21, double m22,
            double m23, double m31, double m32, double m33)
        {
            // Arrange
            // Act
            var rotationMatrix = Matrix3x3.CreateRotation(angle);

            // Assert
            Assert.That(rotationMatrix.M11, Is.EqualTo(m11));
            Assert.That(rotationMatrix.M12, Is.EqualTo(m12));
            Assert.That(rotationMatrix.M13, Is.EqualTo(m13));

            Assert.That(rotationMatrix.M21, Is.EqualTo(m21));
            Assert.That(rotationMatrix.M22, Is.EqualTo(m22));
            Assert.That(rotationMatrix.M23, Is.EqualTo(m23));

            Assert.That(rotationMatrix.M31, Is.EqualTo(m31));
            Assert.That(rotationMatrix.M32, Is.EqualTo(m32));
            Assert.That(rotationMatrix.M33, Is.EqualTo(m33));
        }

        [TestCase(1, 1,
            1, 0, 0, 0, 1, 0, 0, 0, 1)]
        [TestCase(2, -3,
            2, 0, 0, 0, -3, 0, 0, 0, 1)]
        [TestCase(-56.150, 70.203,
            -56.150, 0, 0, 0, 70.203, 0, 0, 0, 1)]
        public void CreateScale(double sx, double sy, double m11, double m12, double m13, double m21, double m22,
            double m23, double m31, double m32, double m33)
        {
            // Arrange
            var scaleVector = new Vector2(sx, sy);

            // Act
            var scaleMatrix = Matrix3x3.CreateScale(scaleVector);

            // Assert
            Assert.That(scaleMatrix.M11, Is.EqualTo(m11));
            Assert.That(scaleMatrix.M12, Is.EqualTo(m12));
            Assert.That(scaleMatrix.M13, Is.EqualTo(m13));

            Assert.That(scaleMatrix.M21, Is.EqualTo(m21));
            Assert.That(scaleMatrix.M22, Is.EqualTo(m22));
            Assert.That(scaleMatrix.M23, Is.EqualTo(m23));

            Assert.That(scaleMatrix.M31, Is.EqualTo(m31));
            Assert.That(scaleMatrix.M32, Is.EqualTo(m32));
            Assert.That(scaleMatrix.M33, Is.EqualTo(m33));
        }

        [TestCase(0, 0, 0, 1, 1)]
        [TestCase(5, -3, 0, 1, 1)]
        [TestCase(0, 0, System.Math.PI / 4, 1, 1)]
        [TestCase(0, 0, 0, 3, -2)]
        [TestCase(5, -3, System.Math.PI / 4, 3, -2)]
        public void CreateTRS_Test(double tx, double ty, double rotation, double sx, double sy)
        {
            // Arrange
            var translation = new Vector2(tx, ty);
            var scale = new Vector2(sx, sy);

            // Act
            var actual = Matrix3x3.CreateTRS(translation, rotation, scale);

            // Assert
            var expected = Matrix3x3.CreateTranslation(translation) * Matrix3x3.CreateRotation(rotation) * Matrix3x3.CreateScale(scale);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            1, 1, -2, -2, 3, 3, -4, -4, 5,
            0,
            1, -2, 3, -4, 5, -6, 7, -8, 9)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            1, 1, -2, -2, 3, 3, -4, -4, 5,
            1,
            1, 1, -2, -2, 3, 3, -4, -4, 5)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            1, 1, -2, -2, 3, 3, -4, -4, 5,
            0.5,
            1, -0.5, 0.5, -3, 4, -1.5, 1.5, -6, 7)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            1, 1, -2, -2, 3, 3, -4, -4, 5,
            0.25,
            1, -1.25, 1.75, -3.5, 4.5, -3.75, 4.25, -7, 8)]
        public void Lerp_Test(
            double m1_11, double m1_12, double m1_13, double m1_21, double m1_22, double m1_23, double m1_31, double m1_32, double m1_33,
            double m2_11, double m2_12, double m2_13, double m2_21, double m2_22, double m2_23, double m2_31, double m2_32, double m2_33,
            double alpha,
            double m3_11, double m3_12, double m3_13, double m3_21, double m3_22, double m3_23, double m3_31, double m3_32, double m3_33)
        {
            // Arrange
            var m1 = new Matrix3x3(m1_11, m1_12, m1_13, m1_21, m1_22, m1_23, m1_31, m1_32, m1_33);
            var m2 = new Matrix3x3(m2_11, m2_12, m2_13, m2_21, m2_22, m2_23, m2_31, m2_32, m2_33);

            // Act
            var m3 = Matrix3x3.Lerp(m1, m2, alpha);

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m3_11));
            Assert.That(m3.M12, Is.EqualTo(m3_12));
            Assert.That(m3.M13, Is.EqualTo(m3_13));
            Assert.That(m3.M21, Is.EqualTo(m3_21));
            Assert.That(m3.M22, Is.EqualTo(m3_22));
            Assert.That(m3.M23, Is.EqualTo(m3_23));
            Assert.That(m3.M31, Is.EqualTo(m3_31));
            Assert.That(m3.M32, Is.EqualTo(m3_32));
            Assert.That(m3.M33, Is.EqualTo(m3_33));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(0.5)]
        [TestCase(0.25)]
        public void LerpTRS_Test(double alpha)
        {
            // Arrange
            var t1 = new Transform2D
            {
                Translation = new Vector2(-400, -500),
                Rotation = Angle.Deg2Rad(60),
                Scale = new Vector2(0.5, 0.5)
            };

            var t2 = new Transform2D
            {
                Translation = new Vector2(-200, -200),
                Rotation = Angle.Deg2Rad(-80),
                Scale = new Vector2(2, 2)
            };

            var m1 = t1.ToMatrix();
            var m2 = t2.ToMatrix();

            // Act
            var m3 = Matrix3x3.LerpTRS(m1, m2, alpha);

            // Assert
            Assert.That(m3, Is.EqualTo(Transform2D.Lerp(t1, t2, alpha).ToMatrix()).Using(Matrix3x3Comparer));
        }

        #endregion

        #region Operators

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 2, 3, 4, 5, 6, 7, 8, 9)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            2, 4, -8, -16, 32, -64, 128, 256, -512,
            3, 2, -5, -20, 37, -70, 135, 248, -503)]
        [TestCase(51.5367, -72.2349, 13.4338, -65.7531, -40.0080, -6.0467, -95.0915, 83.1871, -8.4247,
            -97.4222, -55.3542, -43.0002, -57.0838, 51.5458, 49.1806, -99.6850, -33.0682, 7.1037,
            -45.8855, -127.5891, -29.5664, -122.8369, 11.5378, 43.1339, -194.7765, 50.1189, -1.3210)]
        public void AdditionOperator(double m1_11, double m1_12, double m1_13, double m1_21, double m1_22, double m1_23,
            double m1_31, double m1_32, double m1_33, double m2_11, double m2_12, double m2_13, double m2_21,
            double m2_22, double m2_23, double m2_31, double m2_32, double m2_33, double m3_11, double m3_12,
            double m3_13, double m3_21, double m3_22, double m3_23, double m3_31, double m3_32, double m3_33)
        {
            // Arrange
            var m1 = new Matrix3x3(m1_11, m1_12, m1_13, m1_21, m1_22, m1_23, m1_31, m1_32, m1_33);
            var m2 = new Matrix3x3(m2_11, m2_12, m2_13, m2_21, m2_22, m2_23, m2_31, m2_32, m2_33);

            // Act
            var m3 = m1 + m2;

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m3_11));
            Assert.That(m3.M12, Is.EqualTo(m3_12));
            Assert.That(m3.M13, Is.EqualTo(m3_13));

            Assert.That(m3.M21, Is.EqualTo(m3_21));
            Assert.That(m3.M22, Is.EqualTo(m3_22));
            Assert.That(m3.M23, Is.EqualTo(m3_23));

            Assert.That(m3.M31, Is.EqualTo(m3_31));
            Assert.That(m3.M32, Is.EqualTo(m3_32));
            Assert.That(m3.M33, Is.EqualTo(m3_33));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 2, 3, 4, 5, 6, 7, 8, 9)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            2, 4, -8, -16, 32, -64, 128, 256, -512,
            -1, -6, 11, 12, -27, 58, -121, -264, 521)]
        [TestCase(59.0905, 22.8688, 86.3711, 45.9380, 83.4189, -7.8069, -5.0340, 1.5645, -66.9043,
            1.9621, -61.7390, -9.7698, -43.5176, 52.0129, -90.8052, 83.2221, 86.3868, -10.8017,
            57.1284, 84.6078, 96.1409, 89.4556, 31.4060, 82.9983, -88.2561, -84.8223, -56.1026)]
        public void SubtractionOperator(double m1_11, double m1_12, double m1_13, double m1_21, double m1_22,
            double m1_23, double m1_31, double m1_32, double m1_33, double m2_11, double m2_12, double m2_13,
            double m2_21, double m2_22, double m2_23, double m2_31, double m2_32, double m2_33, double m3_11,
            double m3_12, double m3_13, double m3_21, double m3_22, double m3_23, double m3_31, double m3_32,
            double m3_33)
        {
            // Arrange
            var m1 = new Matrix3x3(m1_11, m1_12, m1_13, m1_21, m1_22, m1_23, m1_31, m1_32, m1_33);
            var m2 = new Matrix3x3(m2_11, m2_12, m2_13, m2_21, m2_22, m2_23, m2_31, m2_32, m2_33);

            // Act
            var m3 = m1 - m2;

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m3_11));
            Assert.That(m3.M12, Is.EqualTo(m3_12));
            Assert.That(m3.M13, Is.EqualTo(m3_13));

            Assert.That(m3.M21, Is.EqualTo(m3_21));
            Assert.That(m3.M22, Is.EqualTo(m3_22));
            Assert.That(m3.M23, Is.EqualTo(m3_23));

            Assert.That(m3.M31, Is.EqualTo(m3_31));
            Assert.That(m3.M32, Is.EqualTo(m3_32));
            Assert.That(m3.M33, Is.EqualTo(m3_33));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0,
            0,
            0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            0,
            0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1,
            1, 2, 3, 4, 5, 6, 7, 8, 9)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            2,
            2, -4, 6, -8, 10, -12, 14, -16, 18)]
        [TestCase(37.0865, 40.2434, -91.0195, -89.0929, 31.8019, 90.9098, 85.6377, 60.8635, -59.1594,
            -15.4159,
            -571.7218, -620.3882, 1403.1475, 1373.4472, -490.2549, -1401.4564, -1320.1822, -938.2656, 911.9954)]
        public void MultiplicationOperatorByScalar(double m1_11, double m1_12, double m1_13, double m1_21, double m1_22,
            double m1_23, double m1_31, double m1_32, double m1_33, double s, double m2_11, double m2_12, double m2_13,
            double m2_21, double m2_22, double m2_23, double m2_31, double m2_32, double m2_33)
        {
            // Arrange
            var m1 = new Matrix3x3(m1_11, m1_12, m1_13, m1_21, m1_22, m1_23, m1_31, m1_32, m1_33);

            // Act
            var m3 = m1 * s;

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m2_11));
            Assert.That(m3.M12, Is.EqualTo(m2_12));
            Assert.That(m3.M13, Is.EqualTo(m2_13));

            Assert.That(m3.M21, Is.EqualTo(m2_21));
            Assert.That(m3.M22, Is.EqualTo(m2_22));
            Assert.That(m3.M23, Is.EqualTo(m2_23));

            Assert.That(m3.M31, Is.EqualTo(m2_31));
            Assert.That(m3.M32, Is.EqualTo(m2_32));
            Assert.That(m3.M33, Is.EqualTo(m2_33));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1, 0, 0, 0, 1, 0, 0, 0, 1,
            1, 2, 3, 4, 5, 6, 7, 8, 9)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            2, 4, -8, -16, 32, -64, 128, 256, -512,
            418, 708, -1416, -856, -1392, 2784, 1294, 2076, -4152)]
        [TestCase(35.8128, 92.9169, 43.1218, 48.8988, 46.6757, 70.2530, -25.5962, 25.4008, -40.2330,
            -7.0450, -13.6652, 60.7340, 84.7333, -85.8402, -54.3045, 67.9880, -19.9834, 16.2325,
            10552.6193, -9327.1145, -2170.7766, 8386.8550, -6078.7571, 1575.5010, -402.7424, -1026.6404, -3587.0195)]
        public void MultiplicationOperatorByMatrix(double m1_11, double m1_12, double m1_13, double m1_21, double m1_22,
            double m1_23, double m1_31, double m1_32, double m1_33, double m2_11, double m2_12, double m2_13,
            double m2_21, double m2_22, double m2_23, double m2_31, double m2_32, double m2_33, double m3_11,
            double m3_12, double m3_13, double m3_21, double m3_22, double m3_23, double m3_31, double m3_32,
            double m3_33)
        {
            // Arrange
            var m1 = new Matrix3x3(m1_11, m1_12, m1_13, m1_21, m1_22, m1_23, m1_31, m1_32, m1_33);
            var m2 = new Matrix3x3(m2_11, m2_12, m2_13, m2_21, m2_22, m2_23, m2_31, m2_32, m2_33);

            // Act
            var m3 = m1 * m2;

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m3_11));
            Assert.That(m3.M12, Is.EqualTo(m3_12));
            Assert.That(m3.M13, Is.EqualTo(m3_13));

            Assert.That(m3.M21, Is.EqualTo(m3_21));
            Assert.That(m3.M22, Is.EqualTo(m3_22));
            Assert.That(m3.M23, Is.EqualTo(m3_23));

            Assert.That(m3.M31, Is.EqualTo(m3_31));
            Assert.That(m3.M32, Is.EqualTo(m3_32));
            Assert.That(m3.M33, Is.EqualTo(m3_33));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0,
            0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            0, 0, 0,
            0, 0, 0)]
        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 2, 3,
            0, 0, 0)]
        [TestCase(1, 0, 0, 0, 1, 0, 0, 0, 1,
            1, 2, 3,
            1, 2, 3)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            2, 4, -8,
            -30, 60, -90)]
        [TestCase(85.6182, 13.7763, -58.8209, 28.1508, -83.6723, -36.6157, 73.1051, -13.8602, 80.6760,
            23.1238, -40.3491, 12.2021,
            706.2183, 3580.2670, 3234.1309)]
        public void MultiplicationOperatorByVector(double m11, double m12, double m13, double m21, double m22,
            double m23, double m31, double m32, double m33, double x1, double y1, double z1, double x2, double y2,
            double z2)
        {
            // Arrange
            var m = new Matrix3x3(m11, m12, m13, m21, m22, m23, m31, m32, m33);
            var v1 = new Vector3(x1, y1, z1);

            // Act
            var v2 = m * v1;

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2));
            Assert.That(v2.Y, Is.EqualTo(y2));
            Assert.That(v2.Z, Is.EqualTo(z2));
        }

        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9,
            1,
            1, 2, 3, 4, 5, 6, 7, 8, 9)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            2,
            0.5, -1, 1.5, -2, 2.5, -3, 3.5, -4, 4.5)]
        [TestCase(38.0149, -2.9668, -97.5845, 21.3270, 47.2321, 55.5444, -38.2938, 33.3373, -84.6304,
            -54.2992,
            -0.7001, 0.0546, 1.7972, -0.3928, -0.8698, -1.0229, 0.7052, -0.6140, 1.5586)]
        public void DivisionOperatorByScalar(double m1_11, double m1_12, double m1_13, double m1_21, double m1_22,
            double m1_23, double m1_31, double m1_32, double m1_33, double s, double m2_11, double m2_12, double m2_13,
            double m2_21, double m2_22, double m2_23, double m2_31, double m2_32, double m2_33)
        {
            // Arrange
            var m1 = new Matrix3x3(m1_11, m1_12, m1_13, m1_21, m1_22, m1_23, m1_31, m1_32, m1_33);

            // Act
            var m3 = m1 / s;

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m2_11));
            Assert.That(m3.M12, Is.EqualTo(m2_12));
            Assert.That(m3.M13, Is.EqualTo(m2_13));

            Assert.That(m3.M21, Is.EqualTo(m2_21));
            Assert.That(m3.M22, Is.EqualTo(m2_22));
            Assert.That(m3.M23, Is.EqualTo(m2_23));

            Assert.That(m3.M31, Is.EqualTo(m2_31));
            Assert.That(m3.M32, Is.EqualTo(m2_32));
            Assert.That(m3.M33, Is.EqualTo(m2_33));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9,
            -1, 2, -3, 4, -5, 6, -7, 8, -9)]
        [TestCase(48.204, 67.909, 16.677, -61.618, 31.834, 19.980, -92.613, -93.628, -21.756,
            -48.204, -67.909, -16.677, 61.618, -31.834, -19.980, 92.613, 93.628, 21.756)]
        public void OppositionOperator(double m1_11, double m1_12, double m1_13, double m1_21, double m1_22,
            double m1_23, double m1_31, double m1_32, double m1_33, double m2_11, double m2_12, double m2_13,
            double m2_21, double m2_22, double m2_23, double m2_31, double m2_32, double m2_33)
        {
            // Arrange
            var m = new Matrix3x3(m1_11, m1_12, m1_13, m1_21, m1_22, m1_23, m1_31, m1_32, m1_33);

            // Act
            var actual = -m;

            // Assert
            Assert.That(actual.M11, Is.EqualTo(m2_11));
            Assert.That(actual.M12, Is.EqualTo(m2_12));
            Assert.That(actual.M13, Is.EqualTo(m2_13));

            Assert.That(actual.M21, Is.EqualTo(m2_21));
            Assert.That(actual.M22, Is.EqualTo(m2_22));
            Assert.That(actual.M23, Is.EqualTo(m2_23));

            Assert.That(actual.M31, Is.EqualTo(m2_31));
            Assert.That(actual.M32, Is.EqualTo(m2_32));
            Assert.That(actual.M33, Is.EqualTo(m2_33));
        }

        #endregion
    }
}