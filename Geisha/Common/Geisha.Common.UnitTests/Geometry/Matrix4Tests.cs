using System;
using Geisha.Common.Geometry;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Geometry
{
    [TestFixture]
    public class Matrix4Tests
    {
        private const double Epsilon = 0.0001;

        #region Static properties

        [Test]
        public void Zero()
        {
            // Arrange
            // Act
            var m = Matrix4.Zero;

            // Assert
            Assert.That(m.M11, Is.Zero);
            Assert.That(m.M12, Is.Zero);
            Assert.That(m.M13, Is.Zero);
            Assert.That(m.M14, Is.Zero);

            Assert.That(m.M21, Is.Zero);
            Assert.That(m.M22, Is.Zero);
            Assert.That(m.M23, Is.Zero);
            Assert.That(m.M24, Is.Zero);

            Assert.That(m.M31, Is.Zero);
            Assert.That(m.M32, Is.Zero);
            Assert.That(m.M33, Is.Zero);
            Assert.That(m.M34, Is.Zero);

            Assert.That(m.M41, Is.Zero);
            Assert.That(m.M42, Is.Zero);
            Assert.That(m.M43, Is.Zero);
            Assert.That(m.M44, Is.Zero);
        }

        [Test]
        public void Identity()
        {
            // Arrange
            // Act
            var m = Matrix4.Identity;

            // Assert
            Assert.That(m.M11, Is.EqualTo(1));
            Assert.That(m.M12, Is.Zero);
            Assert.That(m.M13, Is.Zero);
            Assert.That(m.M14, Is.Zero);

            Assert.That(m.M21, Is.Zero);
            Assert.That(m.M22, Is.EqualTo(1));
            Assert.That(m.M23, Is.Zero);
            Assert.That(m.M24, Is.Zero);

            Assert.That(m.M31, Is.Zero);
            Assert.That(m.M32, Is.Zero);
            Assert.That(m.M33, Is.EqualTo(1));
            Assert.That(m.M34, Is.Zero);

            Assert.That(m.M41, Is.Zero);
            Assert.That(m.M42, Is.Zero);
            Assert.That(m.M43, Is.Zero);
            Assert.That(m.M44, Is.EqualTo(1));
        }

        #endregion

        #region Properties

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16,
             -1, 2, -3, 4, -5, 6, -7, 8, -9, 10, -11, 12, -13, 14, -15, 16)]
        [TestCase(-24.1691, -94.2051, -23.4780, -4.3024, -40.3912, -95.6135, -35.3159, -16.8990, -9.6455, 57.5209,
             -82.1194, 64.9115, -13.1973, 5.9780, 1.5403, -73.1722,
             24.1691, 94.2051, 23.4780, 4.3024, 40.3912, 95.6135, 35.3159, 16.8990, 9.6455, -57.5209, 82.1194,
             -64.9115, 13.1973, -5.9780, -1.5403, 73.1722)]
        public void Opposite(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21, double m1_22,
            double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34, double m1_41,
            double m1_42, double m1_43, double m1_44, double m2_11, double m2_12, double m2_13, double m2_14,
            double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32, double m2_33,
            double m2_34, double m2_41, double m2_42, double m2_43, double m2_44)
        {
            // Arrange
            var m = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            // Act
            var actual = m.Opposite;

            // Assert
            Assert.That(actual.M11, Is.EqualTo(m2_11));
            Assert.That(actual.M12, Is.EqualTo(m2_12));
            Assert.That(actual.M13, Is.EqualTo(m2_13));
            Assert.That(actual.M14, Is.EqualTo(m2_14));

            Assert.That(actual.M21, Is.EqualTo(m2_21));
            Assert.That(actual.M22, Is.EqualTo(m2_22));
            Assert.That(actual.M23, Is.EqualTo(m2_23));
            Assert.That(actual.M24, Is.EqualTo(m2_24));

            Assert.That(actual.M31, Is.EqualTo(m2_31));
            Assert.That(actual.M32, Is.EqualTo(m2_32));
            Assert.That(actual.M33, Is.EqualTo(m2_33));
            Assert.That(actual.M34, Is.EqualTo(m2_34));

            Assert.That(actual.M41, Is.EqualTo(m2_41));
            Assert.That(actual.M42, Is.EqualTo(m2_42));
            Assert.That(actual.M43, Is.EqualTo(m2_43));
            Assert.That(actual.M44, Is.EqualTo(m2_44));
        }

        [TestCase(-1.1634, -4.5347, -1.1302, -0.2071, -1.9443, -4.6025, -1.7000, -0.8135, -0.4643, 2.7689, -3.9530,
             3.1246, -0.6353, 0.2878, 0.0741, -3.522)]
        public void Array(double m11, double m12, double m13, double m14, double m21, double m22, double m23, double m24,
            double m31, double m32, double m33, double m34, double m41, double m42, double m43, double m44)
        {
            // Arrange
            var m = new Matrix4(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);

            // Act
            var actual = m.Array;

            // Assert
            Assert.That(actual[0], Is.EqualTo(m11));
            Assert.That(actual[1], Is.EqualTo(m12));
            Assert.That(actual[2], Is.EqualTo(m13));
            Assert.That(actual[3], Is.EqualTo(m14));

            Assert.That(actual[4], Is.EqualTo(m21));
            Assert.That(actual[5], Is.EqualTo(m22));
            Assert.That(actual[6], Is.EqualTo(m23));
            Assert.That(actual[7], Is.EqualTo(m24));

            Assert.That(actual[8], Is.EqualTo(m31));
            Assert.That(actual[9], Is.EqualTo(m32));
            Assert.That(actual[10], Is.EqualTo(m33));
            Assert.That(actual[11], Is.EqualTo(m34));

            Assert.That(actual[12], Is.EqualTo(m41));
            Assert.That(actual[13], Is.EqualTo(m42));
            Assert.That(actual[14], Is.EqualTo(m43));
            Assert.That(actual[15], Is.EqualTo(m44));
        }

        [TestCase(-1.1634, -4.5347, -1.1302, -0.2071, -1.9443, -4.6025, -1.7000, -0.8135, -0.4643, 2.7689, -3.9530,
             3.1246, -0.6353, 0.2878, 0.0741, -3.522)]
        public void Indexer(double m11, double m12, double m13, double m14, double m21, double m22, double m23,
            double m24, double m31, double m32, double m33, double m34, double m41, double m42, double m43, double m44)
        {
            // Arrange
            var m = new Matrix4(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);

            // Act
            // Assert
            Assert.That(m[0, 0], Is.EqualTo(m11));
            Assert.That(m[0, 1], Is.EqualTo(m12));
            Assert.That(m[0, 2], Is.EqualTo(m13));
            Assert.That(m[0, 3], Is.EqualTo(m14));

            Assert.That(m[1, 0], Is.EqualTo(m21));
            Assert.That(m[1, 1], Is.EqualTo(m22));
            Assert.That(m[1, 2], Is.EqualTo(m23));
            Assert.That(m[1, 3], Is.EqualTo(m24));

            Assert.That(m[2, 0], Is.EqualTo(m31));
            Assert.That(m[2, 1], Is.EqualTo(m32));
            Assert.That(m[2, 2], Is.EqualTo(m33));
            Assert.That(m[2, 3], Is.EqualTo(m34));

            Assert.That(m[3, 0], Is.EqualTo(m41));
            Assert.That(m[3, 1], Is.EqualTo(m42));
            Assert.That(m[3, 2], Is.EqualTo(m43));
            Assert.That(m[3, 3], Is.EqualTo(m44));
        }

        #endregion

        #region Constructors

        [Test]
        public void ParameterlessConstructor()
        {
            // Arrange
            // Act
            var m = new Matrix4();

            // Assert
            Assert.That(m.M11, Is.Zero);
            Assert.That(m.M12, Is.Zero);
            Assert.That(m.M13, Is.Zero);
            Assert.That(m.M14, Is.Zero);

            Assert.That(m.M21, Is.Zero);
            Assert.That(m.M22, Is.Zero);
            Assert.That(m.M23, Is.Zero);
            Assert.That(m.M24, Is.Zero);

            Assert.That(m.M31, Is.Zero);
            Assert.That(m.M32, Is.Zero);
            Assert.That(m.M33, Is.Zero);
            Assert.That(m.M34, Is.Zero);

            Assert.That(m.M41, Is.Zero);
            Assert.That(m.M42, Is.Zero);
            Assert.That(m.M43, Is.Zero);
            Assert.That(m.M44, Is.Zero);
        }

        [Test]
        public void ConstructorFromNineDoubles()
        {
            // Arrange
            // Act
            var m = new Matrix4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);

            // Assert
            Assert.That(m.M11, Is.EqualTo(1));
            Assert.That(m.M12, Is.EqualTo(2));
            Assert.That(m.M13, Is.EqualTo(3));
            Assert.That(m.M14, Is.EqualTo(4));

            Assert.That(m.M21, Is.EqualTo(5));
            Assert.That(m.M22, Is.EqualTo(6));
            Assert.That(m.M23, Is.EqualTo(7));
            Assert.That(m.M24, Is.EqualTo(8));

            Assert.That(m.M31, Is.EqualTo(9));
            Assert.That(m.M32, Is.EqualTo(10));
            Assert.That(m.M33, Is.EqualTo(11));
            Assert.That(m.M34, Is.EqualTo(12));

            Assert.That(m.M41, Is.EqualTo(13));
            Assert.That(m.M42, Is.EqualTo(14));
            Assert.That(m.M43, Is.EqualTo(15));
            Assert.That(m.M44, Is.EqualTo(16));
        }

        [Test]
        public void ConstructorFromArray()
        {
            // Arrange
            // Act
            var m = new Matrix4(new double[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16});

            // Assert
            Assert.That(m.M11, Is.EqualTo(1));
            Assert.That(m.M12, Is.EqualTo(2));
            Assert.That(m.M13, Is.EqualTo(3));
            Assert.That(m.M14, Is.EqualTo(4));

            Assert.That(m.M21, Is.EqualTo(5));
            Assert.That(m.M22, Is.EqualTo(6));
            Assert.That(m.M23, Is.EqualTo(7));
            Assert.That(m.M24, Is.EqualTo(8));

            Assert.That(m.M31, Is.EqualTo(9));
            Assert.That(m.M32, Is.EqualTo(10));
            Assert.That(m.M33, Is.EqualTo(11));
            Assert.That(m.M34, Is.EqualTo(12));

            Assert.That(m.M41, Is.EqualTo(13));
            Assert.That(m.M42, Is.EqualTo(14));
            Assert.That(m.M43, Is.EqualTo(15));
            Assert.That(m.M44, Is.EqualTo(16));
        }

        [TestCase(9)]
        [TestCase(25)]
        public void ConstructorFromArrayThrowsException_GivenArrayOfLengthDifferentFromNine(int length)
        {
            // Arrange
            var array = new double[length];

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => new Matrix4(array));
        }

        #endregion

        #region Methods

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16,
             2, 4, -8, -16, 32, -64, 128, 256, -512, -1024, 2048, -4096, 8192, 16384, -32768, -65536,
             3, 2, -5, -20, 37, -70, 135, 248, -503, -1034, 2059, -4108, 8205, 16370, -32753, -65552)]
        [TestCase(9.0823, 44.0085, 51.8052, -24.0707, -26.6471, 52.7143, 94.3623, 67.6122, -15.3657, -45.7424, 63.7709,
             28.9065, -28.1005, 87.5372, -95.7694, -67.1808,
             46.0916, 47.1357, 47.3486, 19.1745, -77.0160, 3.5763, -15.2721, -73.8427, -32.2472, -17.5874, 34.1125,
             -24.2850, -78.4739, 72.1758, -60.7389, -91.7424,
             55.1739, 91.1442, 99.1538, -4.8962, -103.6631, 56.2906, 79.0902, -6.2305, -47.6129, -63.3298, 97.8834,
             4.6215, -106.5744, 159.7130, -156.5083, -158.9232)]
        public void Add(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21, double m1_22,
            double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34, double m1_41,
            double m1_42, double m1_43, double m1_44, double m2_11, double m2_12, double m2_13, double m2_14,
            double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32, double m2_33,
            double m2_34, double m2_41, double m2_42, double m2_43, double m2_44, double m3_11, double m3_12,
            double m3_13, double m3_14, double m3_21, double m3_22, double m3_23, double m3_24, double m3_31,
            double m3_32, double m3_33, double m3_34, double m3_41, double m3_42, double m3_43, double m3_44)
        {
            // Arrange
            var m1 = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            var m2 = new Matrix4(m2_11, m2_12, m2_13, m2_14, m2_21, m2_22, m2_23, m2_24, m2_31, m2_32, m2_33, m2_34,
                m2_41, m2_42, m2_43, m2_44);

            // Act
            var m3 = m1.Add(m2);

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m3_11).Within(Epsilon));
            Assert.That(m3.M12, Is.EqualTo(m3_12).Within(Epsilon));
            Assert.That(m3.M13, Is.EqualTo(m3_13).Within(Epsilon));
            Assert.That(m3.M14, Is.EqualTo(m3_14).Within(Epsilon));

            Assert.That(m3.M21, Is.EqualTo(m3_21).Within(Epsilon));
            Assert.That(m3.M22, Is.EqualTo(m3_22).Within(Epsilon));
            Assert.That(m3.M23, Is.EqualTo(m3_23).Within(Epsilon));
            Assert.That(m3.M24, Is.EqualTo(m3_24).Within(Epsilon));

            Assert.That(m3.M31, Is.EqualTo(m3_31).Within(Epsilon));
            Assert.That(m3.M32, Is.EqualTo(m3_32).Within(Epsilon));
            Assert.That(m3.M33, Is.EqualTo(m3_33).Within(Epsilon));
            Assert.That(m3.M34, Is.EqualTo(m3_34).Within(Epsilon));

            Assert.That(m3.M41, Is.EqualTo(m3_41).Within(Epsilon));
            Assert.That(m3.M42, Is.EqualTo(m3_42).Within(Epsilon));
            Assert.That(m3.M43, Is.EqualTo(m3_43).Within(Epsilon));
            Assert.That(m3.M44, Is.EqualTo(m3_44).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16,
             2, 4, -8, -16, 32, -64, 128, 256, -512, -1024, 2048, -4096, 8192, 16384, -32768, -65536,
             -1, -6, 11, 12, -27, 58, -121, -264, 521, 1014, -2037, 4084, -8179, -16398, 32783, 65520)]
        [TestCase(11.9396, 66.1945, 41.6159, 61.3461, -60.7304, 77.0286, 84.1929, -58.2249, -45.0138, 35.0218, 83.2917,
             54.0739, -45.8725, -66.2010, 25.6441, -86.9941,
             -6.3591, 76.3505, 97.2741, 9.3820, -40.4580, -62.7896, 19.2028, -52.5404, -44.8373, 77.1980, 13.8874,
             -53.0344, 36.5816, 93.2854, 69.6266, -86.1140,
             18.2987, -10.1560, -55.6582, 51.9641, -20.2724, 139.8182, 64.9901, -5.6845, -0.1765, -42.1762, 69.4043,
             107.1083, -82.4541, -159.4864, -43.9825, -0.8801)]
        public void Subtract(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21, double m1_22,
            double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34, double m1_41,
            double m1_42, double m1_43, double m1_44, double m2_11, double m2_12, double m2_13, double m2_14,
            double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32, double m2_33,
            double m2_34, double m2_41, double m2_42, double m2_43, double m2_44, double m3_11, double m3_12,
            double m3_13, double m3_14, double m3_21, double m3_22, double m3_23, double m3_24, double m3_31,
            double m3_32, double m3_33, double m3_34, double m3_41, double m3_42, double m3_43, double m3_44)
        {
            // Arrange
            var m1 = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            var m2 = new Matrix4(m2_11, m2_12, m2_13, m2_14, m2_21, m2_22, m2_23, m2_24, m2_31, m2_32, m2_33, m2_34,
                m2_41, m2_42, m2_43, m2_44);

            // Act
            var m3 = m1.Subtract(m2);

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m3_11).Within(Epsilon));
            Assert.That(m3.M12, Is.EqualTo(m3_12).Within(Epsilon));
            Assert.That(m3.M13, Is.EqualTo(m3_13).Within(Epsilon));
            Assert.That(m3.M14, Is.EqualTo(m3_14).Within(Epsilon));

            Assert.That(m3.M21, Is.EqualTo(m3_21).Within(Epsilon));
            Assert.That(m3.M22, Is.EqualTo(m3_22).Within(Epsilon));
            Assert.That(m3.M23, Is.EqualTo(m3_23).Within(Epsilon));
            Assert.That(m3.M24, Is.EqualTo(m3_24).Within(Epsilon));

            Assert.That(m3.M31, Is.EqualTo(m3_31).Within(Epsilon));
            Assert.That(m3.M32, Is.EqualTo(m3_32).Within(Epsilon));
            Assert.That(m3.M33, Is.EqualTo(m3_33).Within(Epsilon));
            Assert.That(m3.M34, Is.EqualTo(m3_34).Within(Epsilon));

            Assert.That(m3.M41, Is.EqualTo(m3_41).Within(Epsilon));
            Assert.That(m3.M42, Is.EqualTo(m3_42).Within(Epsilon));
            Assert.That(m3.M43, Is.EqualTo(m3_43).Within(Epsilon));
            Assert.That(m3.M44, Is.EqualTo(m3_44).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16,
             2,
             2, -4, 6, -8, 10, -12, 14, -16, 18, -20, 22, -24, 26, -28, 30, -32)]
        [TestCase(86.0441, 78.5559, -13.3931, 29.5534, 93.4405, -51.3377, 3.0890, 33.2936, -98.4398, -17.7602, 93.9142,
             46.7068, -85.8939, -39.8589, -30.1306, -64.8198,
             -91.2132,
             -7848.3577, -7165.3350, 1221.6275, -2695.6602, -8523.0070, 4682.6759, -281.7576, -3036.8158, 8979.0092,
             1619.9647, -8566.2147, -4260.2767, 7834.6575, 3635.6578, 2748.3084, 5912.4214)]
        public void MultiplyByScalar(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21, double m1_22,
            double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34, double m1_41,
            double m1_42, double m1_43, double m1_44, double s, double m2_11, double m2_12, double m2_13, double m2_14,
            double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32, double m2_33,
            double m2_34, double m2_41, double m2_42, double m2_43, double m2_44)
        {
            // Arrange
            var m1 = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            // Act
            var m2 = m1.Multiply(s);

            // Assert
            Assert.That(m2.M11, Is.EqualTo(m2_11).Within(Epsilon));
            Assert.That(m2.M12, Is.EqualTo(m2_12).Within(Epsilon));
            Assert.That(m2.M13, Is.EqualTo(m2_13).Within(Epsilon));
            Assert.That(m2.M14, Is.EqualTo(m2_14).Within(Epsilon));

            Assert.That(m2.M21, Is.EqualTo(m2_21).Within(Epsilon));
            Assert.That(m2.M22, Is.EqualTo(m2_22).Within(Epsilon));
            Assert.That(m2.M23, Is.EqualTo(m2_23).Within(Epsilon));
            Assert.That(m2.M24, Is.EqualTo(m2_24).Within(Epsilon));

            Assert.That(m2.M31, Is.EqualTo(m2_31).Within(Epsilon));
            Assert.That(m2.M32, Is.EqualTo(m2_32).Within(Epsilon));
            Assert.That(m2.M33, Is.EqualTo(m2_33).Within(Epsilon));
            Assert.That(m2.M34, Is.EqualTo(m2_34).Within(Epsilon));

            Assert.That(m2.M41, Is.EqualTo(m2_41).Within(Epsilon));
            Assert.That(m2.M42, Is.EqualTo(m2_42).Within(Epsilon));
            Assert.That(m2.M43, Is.EqualTo(m2_43).Within(Epsilon));
            Assert.That(m2.M44, Is.EqualTo(m2_44).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16,
             2, 4, -8, -16, 32, -64, 128, 256, -512, -1024, 2048, -4096, 8192, 16384, -32768, -65536,
             -34366, -68476, 136952, 249328, -69302, -137836, 275672, 494000, -104238, -207196, 414392, 738672, -139174,
             -276556, 553112, 983344)]
        [TestCase(74.3688, -21.2163, 16.3570, -70.2702, 10.5920, -9.8519, 66.5987, 15.3590, -68.1560, -82.3574, 89.4389,
             95.3457, -46.3261, -17.5010, -51.6558, 54.0884,
             16.3512, -44.0036, -6.4187, -64.2061, -92.1097, 25.7614, -76.7330, 41.9048, 79.4658, -75.1946, -67.1202,
             39.0762, 57.7394, 22.8669, -71.5213, 81.6140,
             412.7091, -6655.8762, 5078.5703, -10759.8581, 7259.7859, -5376.5348, -4880.6347, 2763.0206, 19084.0203,
             -3667.5943, -6065.4020, 12201.3369, -127.3132, 6708.7362, 1238.9325, 4636.9006)]
        public void MultiplyByMatrix(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21, double m1_22,
            double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34, double m1_41,
            double m1_42, double m1_43, double m1_44, double m2_11, double m2_12, double m2_13, double m2_14,
            double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32, double m2_33,
            double m2_34, double m2_41, double m2_42, double m2_43, double m2_44, double m3_11, double m3_12,
            double m3_13, double m3_14, double m3_21, double m3_22, double m3_23, double m3_24, double m3_31,
            double m3_32, double m3_33, double m3_34, double m3_41, double m3_42, double m3_43, double m3_44)
        {
            // Arrange
            var m1 = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            var m2 = new Matrix4(m2_11, m2_12, m2_13, m2_14, m2_21, m2_22, m2_23, m2_24, m2_31, m2_32, m2_33, m2_34,
                m2_41, m2_42, m2_43, m2_44);

            // Act
            var m3 = m1.Multiply(m2);

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m3_11).Within(Epsilon));
            Assert.That(m3.M12, Is.EqualTo(m3_12).Within(Epsilon));
            Assert.That(m3.M13, Is.EqualTo(m3_13).Within(Epsilon));
            Assert.That(m3.M14, Is.EqualTo(m3_14).Within(Epsilon));

            Assert.That(m3.M21, Is.EqualTo(m3_21).Within(Epsilon));
            Assert.That(m3.M22, Is.EqualTo(m3_22).Within(Epsilon));
            Assert.That(m3.M23, Is.EqualTo(m3_23).Within(Epsilon));
            Assert.That(m3.M24, Is.EqualTo(m3_24).Within(Epsilon));

            Assert.That(m3.M31, Is.EqualTo(m3_31).Within(Epsilon));
            Assert.That(m3.M32, Is.EqualTo(m3_32).Within(Epsilon));
            Assert.That(m3.M33, Is.EqualTo(m3_33).Within(Epsilon));
            Assert.That(m3.M34, Is.EqualTo(m3_34).Within(Epsilon));

            Assert.That(m3.M41, Is.EqualTo(m3_41).Within(Epsilon));
            Assert.That(m3.M42, Is.EqualTo(m3_42).Within(Epsilon));
            Assert.That(m3.M43, Is.EqualTo(m3_43).Within(Epsilon));
            Assert.That(m3.M44, Is.EqualTo(m3_44).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0,
             0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             0, 0, 0, 0,
             0, 0, 0, 0)]
        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             1, 2, 3, 4,
             0, 0, 0, 0)]
        [TestCase(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1,
             1, 2, 3, 4,
             1, 2, 3, 4)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16,
             2, 4, -8, -16,
             34, 58, 82, 106)]
        [TestCase(-89.3633, -19.5477, 58.2433, 28.5947, -5.4792, -72.0166, -7.0684, 54.3663, -30.3661, 72.1753, -92.2745,
             98.4776, -4.6192, -87.7799, -80.0372, 71.5678,
             52.1203, -37.2313, -93.7135, 82.9533,
             -7016.0145, 7567.9626, 12546.5378, 16464.7570)]
        public void MultiplyByVector(double m11, double m12, double m13, double m14, double m21, double m22, double m23,
            double m24, double m31, double m32, double m33, double m34, double m41, double m42, double m43, double m44,
            double x1, double y1, double z1, double w1, double x2, double y2, double z2, double w2)
        {
            // Arrange
            var m = new Matrix4(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
            var v1 = new Vector4(x1, y1, z1, w1);

            // Act
            var v2 = m.Multiply(v1);

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2).Within(Epsilon));
            Assert.That(v2.Y, Is.EqualTo(y2).Within(Epsilon));
            Assert.That(v2.Z, Is.EqualTo(z2).Within(Epsilon));
            Assert.That(v2.W, Is.EqualTo(w2).Within(Epsilon));
        }

        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16,
             2,
             0.5, -1, 1.5, -2, 2.5, -3, 3.5, -4, 4.5, -5, 5.5, -6, 6.5, -7, 7.5, -8)]
        [TestCase(-63.1766, -14.2027, -31.6429, 70.3223, -12.6500, 16.8251, -21.7563, 55.5738, 13.4303, 19.6249,
             -25.1042, 18.4434, -9.8594, -31.7480, -89.2922, -21.2031,
             -35.9606,
             1.7568, 0.3950, 0.8799, -1.9555, 0.3518, -0.4679, 0.6050, -1.5454, -0.3735, -0.5457, 0.6981, -0.5129,
             0.2742, 0.8829, 2.4831, 0.5896)]
        public void DivideByScalar(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21, double m1_22,
            double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34, double m1_41,
            double m1_42, double m1_43, double m1_44, double s, double m2_11, double m2_12, double m2_13, double m2_14,
            double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32, double m2_33,
            double m2_34, double m2_41, double m2_42, double m2_43, double m2_44)
        {
            // Arrange
            var m1 = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            // Act
            var m2 = m1.Divide(s);

            // Assert
            Assert.That(m2.M11, Is.EqualTo(m2_11).Within(Epsilon));
            Assert.That(m2.M12, Is.EqualTo(m2_12).Within(Epsilon));
            Assert.That(m2.M13, Is.EqualTo(m2_13).Within(Epsilon));
            Assert.That(m2.M14, Is.EqualTo(m2_14).Within(Epsilon));

            Assert.That(m2.M21, Is.EqualTo(m2_21).Within(Epsilon));
            Assert.That(m2.M22, Is.EqualTo(m2_22).Within(Epsilon));
            Assert.That(m2.M23, Is.EqualTo(m2_23).Within(Epsilon));
            Assert.That(m2.M24, Is.EqualTo(m2_24).Within(Epsilon));

            Assert.That(m2.M31, Is.EqualTo(m2_31).Within(Epsilon));
            Assert.That(m2.M32, Is.EqualTo(m2_32).Within(Epsilon));
            Assert.That(m2.M33, Is.EqualTo(m2_33).Within(Epsilon));
            Assert.That(m2.M34, Is.EqualTo(m2_34).Within(Epsilon));

            Assert.That(m2.M41, Is.EqualTo(m2_41).Within(Epsilon));
            Assert.That(m2.M42, Is.EqualTo(m2_42).Within(Epsilon));
            Assert.That(m2.M43, Is.EqualTo(m2_43).Within(Epsilon));
            Assert.That(m2.M44, Is.EqualTo(m2_44).Within(Epsilon));
        }

        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 0, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 0, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 0, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 0, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 0, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 0, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 0, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 0, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 0, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 0, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0, false)]
        [TestCase(14.2187, 72.5775, 57.1026, -92.4581, 2.5135, 37.0363, 17.6855, 22.3770, 42.1112, -84.1560, -95.7384,
             37.3564, 18.4100, -73.4230, 26.5322, -8.7500,
             14.2187, 72.5775, 57.1026, -92.4581, 2.5135, 37.0363, 17.6855, 22.3770, 42.1112, -84.1560, -95.7384,
             37.3564, 18.4100, -73.4230, 26.5322, -8.7500, true)]
        [TestCase(14.2187, 72.5775, 57.1026, -92.4581, 2.5135, 37.0363, 17.6855, 22.3770, 42.1112, -84.1560, -95.7384,
             37.3564, 18.4100, -73.4230, 26.5322, -8.7500,
             14.2187, 72.5775, 57.1026, -92.4581, 2.5135, 37.0363, 17.6855, 22.3770, 42.1112, -84.1560, -95.7384,
             37.3564, 18.4100, -73.4230, 26.5322, -8.75001, false)]
        public void Equals(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21, double m1_22,
            double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34, double m1_41,
            double m1_42, double m1_43, double m1_44, double m2_11, double m2_12, double m2_13, double m2_14,
            double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32, double m2_33,
            double m2_34, double m2_41, double m2_42, double m2_43, double m2_44, bool expected)
        {
            // Arrange
            var m1 = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            var m2 = new Matrix4(m2_11, m2_12, m2_13, m2_14, m2_21, m2_22, m2_23, m2_24, m2_31, m2_32, m2_33, m2_34,
                m2_41, m2_42, m2_43, m2_44);

            // Act
            var actual1 = m1.Equals(m2);
            var actual2 = m1.Equals((object) m2);

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
        }

        #endregion

        #region Operators

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16,
             2, 4, -8, -16, 32, -64, 128, 256, -512, -1024, 2048, -4096, 8192, 16384, -32768, -65536,
             3, 2, -5, -20, 37, -70, 135, 248, -503, -1034, 2059, -4108, 8205, 16370, -32753, -65552)]
        [TestCase(9.0823, 44.0085, 51.8052, -24.0707, -26.6471, 52.7143, 94.3623, 67.6122, -15.3657, -45.7424, 63.7709,
             28.9065, -28.1005, 87.5372, -95.7694, -67.1808,
             46.0916, 47.1357, 47.3486, 19.1745, -77.0160, 3.5763, -15.2721, -73.8427, -32.2472, -17.5874, 34.1125,
             -24.2850, -78.4739, 72.1758, -60.7389, -91.7424,
             55.1739, 91.1442, 99.1538, -4.8962, -103.6631, 56.2906, 79.0902, -6.2305, -47.6129, -63.3298, 97.8834,
             4.6215, -106.5744, 159.7130, -156.5083, -158.9232)]
        public void AdditionOperator(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21, double m1_22,
            double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34, double m1_41,
            double m1_42, double m1_43, double m1_44, double m2_11, double m2_12, double m2_13, double m2_14,
            double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32, double m2_33,
            double m2_34, double m2_41, double m2_42, double m2_43, double m2_44, double m3_11, double m3_12,
            double m3_13, double m3_14, double m3_21, double m3_22, double m3_23, double m3_24, double m3_31,
            double m3_32, double m3_33, double m3_34, double m3_41, double m3_42, double m3_43, double m3_44)
        {
            // Arrange
            var m1 = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            var m2 = new Matrix4(m2_11, m2_12, m2_13, m2_14, m2_21, m2_22, m2_23, m2_24, m2_31, m2_32, m2_33, m2_34,
                m2_41, m2_42, m2_43, m2_44);

            // Act
            var m3 = m1 + m2;

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m3_11).Within(Epsilon));
            Assert.That(m3.M12, Is.EqualTo(m3_12).Within(Epsilon));
            Assert.That(m3.M13, Is.EqualTo(m3_13).Within(Epsilon));
            Assert.That(m3.M14, Is.EqualTo(m3_14).Within(Epsilon));

            Assert.That(m3.M21, Is.EqualTo(m3_21).Within(Epsilon));
            Assert.That(m3.M22, Is.EqualTo(m3_22).Within(Epsilon));
            Assert.That(m3.M23, Is.EqualTo(m3_23).Within(Epsilon));
            Assert.That(m3.M24, Is.EqualTo(m3_24).Within(Epsilon));

            Assert.That(m3.M31, Is.EqualTo(m3_31).Within(Epsilon));
            Assert.That(m3.M32, Is.EqualTo(m3_32).Within(Epsilon));
            Assert.That(m3.M33, Is.EqualTo(m3_33).Within(Epsilon));
            Assert.That(m3.M34, Is.EqualTo(m3_34).Within(Epsilon));

            Assert.That(m3.M41, Is.EqualTo(m3_41).Within(Epsilon));
            Assert.That(m3.M42, Is.EqualTo(m3_42).Within(Epsilon));
            Assert.That(m3.M43, Is.EqualTo(m3_43).Within(Epsilon));
            Assert.That(m3.M44, Is.EqualTo(m3_44).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16,
             2, 4, -8, -16, 32, -64, 128, 256, -512, -1024, 2048, -4096, 8192, 16384, -32768, -65536,
             -1, -6, 11, 12, -27, 58, -121, -264, 521, 1014, -2037, 4084, -8179, -16398, 32783, 65520)]
        [TestCase(11.9396, 66.1945, 41.6159, 61.3461, -60.7304, 77.0286, 84.1929, -58.2249, -45.0138, 35.0218, 83.2917,
             54.0739, -45.8725, -66.2010, 25.6441, -86.9941,
             -6.3591, 76.3505, 97.2741, 9.3820, -40.4580, -62.7896, 19.2028, -52.5404, -44.8373, 77.1980, 13.8874,
             -53.0344, 36.5816, 93.2854, 69.6266, -86.1140,
             18.2987, -10.1560, -55.6582, 51.9641, -20.2724, 139.8182, 64.9901, -5.6845, -0.1765, -42.1762, 69.4043,
             107.1083, -82.4541, -159.4864, -43.9825, -0.8801)]
        public void SubtractionOperator(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21,
            double m1_22, double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34,
            double m1_41, double m1_42, double m1_43, double m1_44, double m2_11, double m2_12, double m2_13,
            double m2_14, double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32,
            double m2_33, double m2_34, double m2_41, double m2_42, double m2_43, double m2_44, double m3_11,
            double m3_12, double m3_13, double m3_14, double m3_21, double m3_22, double m3_23, double m3_24,
            double m3_31, double m3_32, double m3_33, double m3_34, double m3_41, double m3_42, double m3_43,
            double m3_44)
        {
            // Arrange
            var m1 = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            var m2 = new Matrix4(m2_11, m2_12, m2_13, m2_14, m2_21, m2_22, m2_23, m2_24, m2_31, m2_32, m2_33, m2_34,
                m2_41, m2_42, m2_43, m2_44);

            // Act
            var m3 = m1 - m2;

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m3_11).Within(Epsilon));
            Assert.That(m3.M12, Is.EqualTo(m3_12).Within(Epsilon));
            Assert.That(m3.M13, Is.EqualTo(m3_13).Within(Epsilon));
            Assert.That(m3.M14, Is.EqualTo(m3_14).Within(Epsilon));

            Assert.That(m3.M21, Is.EqualTo(m3_21).Within(Epsilon));
            Assert.That(m3.M22, Is.EqualTo(m3_22).Within(Epsilon));
            Assert.That(m3.M23, Is.EqualTo(m3_23).Within(Epsilon));
            Assert.That(m3.M24, Is.EqualTo(m3_24).Within(Epsilon));

            Assert.That(m3.M31, Is.EqualTo(m3_31).Within(Epsilon));
            Assert.That(m3.M32, Is.EqualTo(m3_32).Within(Epsilon));
            Assert.That(m3.M33, Is.EqualTo(m3_33).Within(Epsilon));
            Assert.That(m3.M34, Is.EqualTo(m3_34).Within(Epsilon));

            Assert.That(m3.M41, Is.EqualTo(m3_41).Within(Epsilon));
            Assert.That(m3.M42, Is.EqualTo(m3_42).Within(Epsilon));
            Assert.That(m3.M43, Is.EqualTo(m3_43).Within(Epsilon));
            Assert.That(m3.M44, Is.EqualTo(m3_44).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16,
             2,
             2, -4, 6, -8, 10, -12, 14, -16, 18, -20, 22, -24, 26, -28, 30, -32)]
        [TestCase(86.0441, 78.5559, -13.3931, 29.5534, 93.4405, -51.3377, 3.0890, 33.2936, -98.4398, -17.7602, 93.9142,
             46.7068, -85.8939, -39.8589, -30.1306, -64.8198,
             -91.2132,
             -7848.3577, -7165.3350, 1221.6275, -2695.6602, -8523.0070, 4682.6759, -281.7576, -3036.8158, 8979.0092,
             1619.9647, -8566.2147, -4260.2767, 7834.6575, 3635.6578, 2748.3084, 5912.4214)]
        public void MultiplicationOperatorByScalar(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21,
            double m1_22, double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34,
            double m1_41, double m1_42, double m1_43, double m1_44, double s, double m2_11, double m2_12, double m2_13,
            double m2_14, double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32,
            double m2_33, double m2_34, double m2_41, double m2_42, double m2_43, double m2_44)
        {
            // Arrange
            var m1 = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            // Act
            var m2 = m1*s;

            // Assert
            Assert.That(m2.M11, Is.EqualTo(m2_11).Within(Epsilon));
            Assert.That(m2.M12, Is.EqualTo(m2_12).Within(Epsilon));
            Assert.That(m2.M13, Is.EqualTo(m2_13).Within(Epsilon));
            Assert.That(m2.M14, Is.EqualTo(m2_14).Within(Epsilon));

            Assert.That(m2.M21, Is.EqualTo(m2_21).Within(Epsilon));
            Assert.That(m2.M22, Is.EqualTo(m2_22).Within(Epsilon));
            Assert.That(m2.M23, Is.EqualTo(m2_23).Within(Epsilon));
            Assert.That(m2.M24, Is.EqualTo(m2_24).Within(Epsilon));

            Assert.That(m2.M31, Is.EqualTo(m2_31).Within(Epsilon));
            Assert.That(m2.M32, Is.EqualTo(m2_32).Within(Epsilon));
            Assert.That(m2.M33, Is.EqualTo(m2_33).Within(Epsilon));
            Assert.That(m2.M34, Is.EqualTo(m2_34).Within(Epsilon));

            Assert.That(m2.M41, Is.EqualTo(m2_41).Within(Epsilon));
            Assert.That(m2.M42, Is.EqualTo(m2_42).Within(Epsilon));
            Assert.That(m2.M43, Is.EqualTo(m2_43).Within(Epsilon));
            Assert.That(m2.M44, Is.EqualTo(m2_44).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16,
             2, 4, -8, -16, 32, -64, 128, 256, -512, -1024, 2048, -4096, 8192, 16384, -32768, -65536,
             -34366, -68476, 136952, 249328, -69302, -137836, 275672, 494000, -104238, -207196, 414392, 738672, -139174,
             -276556, 553112, 983344)]
        [TestCase(74.3688, -21.2163, 16.3570, -70.2702, 10.5920, -9.8519, 66.5987, 15.3590, -68.1560, -82.3574, 89.4389,
             95.3457, -46.3261, -17.5010, -51.6558, 54.0884,
             16.3512, -44.0036, -6.4187, -64.2061, -92.1097, 25.7614, -76.7330, 41.9048, 79.4658, -75.1946, -67.1202,
             39.0762, 57.7394, 22.8669, -71.5213, 81.6140,
             412.7091, -6655.8762, 5078.5703, -10759.8581, 7259.7859, -5376.5348, -4880.6347, 2763.0206, 19084.0203,
             -3667.5943, -6065.4020, 12201.3369, -127.3132, 6708.7362, 1238.9325, 4636.9006)]
        public void MultiplicationOperatorByMatrix(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21,
            double m1_22, double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34,
            double m1_41, double m1_42, double m1_43, double m1_44, double m2_11, double m2_12, double m2_13,
            double m2_14, double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32,
            double m2_33, double m2_34, double m2_41, double m2_42, double m2_43, double m2_44, double m3_11,
            double m3_12, double m3_13, double m3_14, double m3_21, double m3_22, double m3_23, double m3_24,
            double m3_31, double m3_32, double m3_33, double m3_34, double m3_41, double m3_42, double m3_43,
            double m3_44)
        {
            // Arrange
            var m1 = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            var m2 = new Matrix4(m2_11, m2_12, m2_13, m2_14, m2_21, m2_22, m2_23, m2_24, m2_31, m2_32, m2_33, m2_34,
                m2_41, m2_42, m2_43, m2_44);

            // Act
            var m3 = m1*m2;

            // Assert
            Assert.That(m3.M11, Is.EqualTo(m3_11).Within(Epsilon));
            Assert.That(m3.M12, Is.EqualTo(m3_12).Within(Epsilon));
            Assert.That(m3.M13, Is.EqualTo(m3_13).Within(Epsilon));
            Assert.That(m3.M14, Is.EqualTo(m3_14).Within(Epsilon));

            Assert.That(m3.M21, Is.EqualTo(m3_21).Within(Epsilon));
            Assert.That(m3.M22, Is.EqualTo(m3_22).Within(Epsilon));
            Assert.That(m3.M23, Is.EqualTo(m3_23).Within(Epsilon));
            Assert.That(m3.M24, Is.EqualTo(m3_24).Within(Epsilon));

            Assert.That(m3.M31, Is.EqualTo(m3_31).Within(Epsilon));
            Assert.That(m3.M32, Is.EqualTo(m3_32).Within(Epsilon));
            Assert.That(m3.M33, Is.EqualTo(m3_33).Within(Epsilon));
            Assert.That(m3.M34, Is.EqualTo(m3_34).Within(Epsilon));

            Assert.That(m3.M41, Is.EqualTo(m3_41).Within(Epsilon));
            Assert.That(m3.M42, Is.EqualTo(m3_42).Within(Epsilon));
            Assert.That(m3.M43, Is.EqualTo(m3_43).Within(Epsilon));
            Assert.That(m3.M44, Is.EqualTo(m3_44).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0,
             0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             0, 0, 0, 0,
             0, 0, 0, 0)]
        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             1, 2, 3, 4,
             0, 0, 0, 0)]
        [TestCase(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1,
             1, 2, 3, 4,
             1, 2, 3, 4)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16,
             2, 4, -8, -16,
             34, 58, 82, 106)]
        [TestCase(-89.3633, -19.5477, 58.2433, 28.5947, -5.4792, -72.0166, -7.0684, 54.3663, -30.3661, 72.1753, -92.2745,
             98.4776, -4.6192, -87.7799, -80.0372, 71.5678,
             52.1203, -37.2313, -93.7135, 82.9533,
             -7016.0145, 7567.9626, 12546.5378, 16464.7570)]
        public void MultiplicationOperatorByVector(double m11, double m12, double m13, double m14, double m21,
            double m22, double m23, double m24, double m31, double m32, double m33, double m34, double m41, double m42,
            double m43, double m44, double x1, double y1, double z1, double w1, double x2, double y2, double z2,
            double w2)
        {
            // Arrange
            var m = new Matrix4(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
            var v1 = new Vector4(x1, y1, z1, w1);

            // Act
            var v2 = m*v1;

            // Assert
            Assert.That(v2.X, Is.EqualTo(x2).Within(Epsilon));
            Assert.That(v2.Y, Is.EqualTo(y2).Within(Epsilon));
            Assert.That(v2.Z, Is.EqualTo(z2).Within(Epsilon));
            Assert.That(v2.W, Is.EqualTo(w2).Within(Epsilon));
        }

        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16,
             2,
             0.5, -1, 1.5, -2, 2.5, -3, 3.5, -4, 4.5, -5, 5.5, -6, 6.5, -7, 7.5, -8)]
        [TestCase(-63.1766, -14.2027, -31.6429, 70.3223, -12.6500, 16.8251, -21.7563, 55.5738, 13.4303, 19.6249,
             -25.1042, 18.4434, -9.8594, -31.7480, -89.2922, -21.2031,
             -35.9606,
             1.7568, 0.3950, 0.8799, -1.9555, 0.3518, -0.4679, 0.6050, -1.5454, -0.3735, -0.5457, 0.6981, -0.5129,
             0.2742, 0.8829, 2.4831, 0.5896)]
        public void DivisionOperatorByScalar(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21,
            double m1_22, double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34,
            double m1_41, double m1_42, double m1_43, double m1_44, double s, double m2_11, double m2_12, double m2_13,
            double m2_14, double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32,
            double m2_33, double m2_34, double m2_41, double m2_42, double m2_43, double m2_44)
        {
            // Arrange
            var m1 = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            // Act
            var m2 = m1/s;

            // Assert
            Assert.That(m2.M11, Is.EqualTo(m2_11).Within(Epsilon));
            Assert.That(m2.M12, Is.EqualTo(m2_12).Within(Epsilon));
            Assert.That(m2.M13, Is.EqualTo(m2_13).Within(Epsilon));
            Assert.That(m2.M14, Is.EqualTo(m2_14).Within(Epsilon));

            Assert.That(m2.M21, Is.EqualTo(m2_21).Within(Epsilon));
            Assert.That(m2.M22, Is.EqualTo(m2_22).Within(Epsilon));
            Assert.That(m2.M23, Is.EqualTo(m2_23).Within(Epsilon));
            Assert.That(m2.M24, Is.EqualTo(m2_24).Within(Epsilon));

            Assert.That(m2.M31, Is.EqualTo(m2_31).Within(Epsilon));
            Assert.That(m2.M32, Is.EqualTo(m2_32).Within(Epsilon));
            Assert.That(m2.M33, Is.EqualTo(m2_33).Within(Epsilon));
            Assert.That(m2.M34, Is.EqualTo(m2_34).Within(Epsilon));

            Assert.That(m2.M41, Is.EqualTo(m2_41).Within(Epsilon));
            Assert.That(m2.M42, Is.EqualTo(m2_42).Within(Epsilon));
            Assert.That(m2.M43, Is.EqualTo(m2_43).Within(Epsilon));
            Assert.That(m2.M44, Is.EqualTo(m2_44).Within(Epsilon));
        }

        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 11, -12, 13, -14, 15, -16,
             -1, 2, -3, 4, -5, 6, -7, 8, -9, 10, -11, 12, -13, 14, -15, 16)]
        [TestCase(-24.1691, -94.2051, -23.4780, -4.3024, -40.3912, -95.6135, -35.3159, -16.8990, -9.6455, 57.5209,
             -82.1194, 64.9115, -13.1973, 5.9780, 1.5403, -73.1722,
             24.1691, 94.2051, 23.4780, 4.3024, 40.3912, 95.6135, 35.3159, 16.8990, 9.6455, -57.5209, 82.1194,
             -64.9115, 13.1973, -5.9780, -1.5403, 73.1722)]
        public void OppositionOperator(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21,
            double m1_22, double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34,
            double m1_41, double m1_42, double m1_43, double m1_44, double m2_11, double m2_12, double m2_13,
            double m2_14, double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32,
            double m2_33, double m2_34, double m2_41, double m2_42, double m2_43, double m2_44)
        {
            // Arrange
            var m = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            // Act
            var actual = -m;

            // Assert
            Assert.That(actual.M11, Is.EqualTo(m2_11));
            Assert.That(actual.M12, Is.EqualTo(m2_12));
            Assert.That(actual.M13, Is.EqualTo(m2_13));
            Assert.That(actual.M14, Is.EqualTo(m2_14));

            Assert.That(actual.M21, Is.EqualTo(m2_21));
            Assert.That(actual.M22, Is.EqualTo(m2_22));
            Assert.That(actual.M23, Is.EqualTo(m2_23));
            Assert.That(actual.M24, Is.EqualTo(m2_24));

            Assert.That(actual.M31, Is.EqualTo(m2_31));
            Assert.That(actual.M32, Is.EqualTo(m2_32));
            Assert.That(actual.M33, Is.EqualTo(m2_33));
            Assert.That(actual.M34, Is.EqualTo(m2_34));

            Assert.That(actual.M41, Is.EqualTo(m2_41));
            Assert.That(actual.M42, Is.EqualTo(m2_42));
            Assert.That(actual.M43, Is.EqualTo(m2_43));
            Assert.That(actual.M44, Is.EqualTo(m2_44));
        }

        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 0, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 0, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 0, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 0, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 0, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 0, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 0, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 0, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 0, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 0, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0, false)]
        [TestCase(14.2187, 72.5775, 57.1026, -92.4581, 2.5135, 37.0363, 17.6855, 22.3770, 42.1112, -84.1560, -95.7384,
             37.3564, 18.4100, -73.4230, 26.5322, -8.7500,
             14.2187, 72.5775, 57.1026, -92.4581, 2.5135, 37.0363, 17.6855, 22.3770, 42.1112, -84.1560, -95.7384,
             37.3564, 18.4100, -73.4230, 26.5322, -8.7500, true)]
        [TestCase(14.2187, 72.5775, 57.1026, -92.4581, 2.5135, 37.0363, 17.6855, 22.3770, 42.1112, -84.1560, -95.7384,
             37.3564, 18.4100, -73.4230, 26.5322, -8.7500,
             14.2187, 72.5775, 57.1026, -92.4581, 2.5135, 37.0363, 17.6855, 22.3770, 42.1112, -84.1560, -95.7384,
             37.3564, 18.4100, -73.4230, 26.5322, -8.75001, false)]
        public void EqualityOperator(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21, double m1_22,
            double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34, double m1_41,
            double m1_42, double m1_43, double m1_44, double m2_11, double m2_12, double m2_13, double m2_14,
            double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32, double m2_33,
            double m2_34, double m2_41, double m2_42, double m2_43, double m2_44, bool expected)
        {
            // Arrange
            var m1 = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            var m2 = new Matrix4(m2_11, m2_12, m2_13, m2_14, m2_21, m2_22, m2_23, m2_24, m2_31, m2_32, m2_33, m2_34,
                m2_41, m2_42, m2_43, m2_44);

            // Act
            var actual = m1 == m2;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, false)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 0, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 0, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 0, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 0, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 0, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 0, 8, 9, 10, 11, 12, 13, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 0, 9, 10, 11, 12, 13, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 0, 10, 11, 12, 13, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 11, 12, 13, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 12, 13, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 13, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 14, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 0, 15, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 0, 16, true)]
        [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0, true)]
        [TestCase(14.2187, 72.5775, 57.1026, -92.4581, 2.5135, 37.0363, 17.6855, 22.3770, 42.1112, -84.1560, -95.7384,
             37.3564, 18.4100, -73.4230, 26.5322, -8.7500,
             14.2187, 72.5775, 57.1026, -92.4581, 2.5135, 37.0363, 17.6855, 22.3770, 42.1112, -84.1560, -95.7384,
             37.3564, 18.4100, -73.4230, 26.5322, -8.7500, false)]
        [TestCase(14.2187, 72.5775, 57.1026, -92.4581, 2.5135, 37.0363, 17.6855, 22.3770, 42.1112, -84.1560, -95.7384,
             37.3564, 18.4100, -73.4230, 26.5322, -8.7500,
             14.2187, 72.5775, 57.1026, -92.4581, 2.5135, 37.0363, 17.6855, 22.3770, 42.1112, -84.1560, -95.7384,
             37.3564, 18.4100, -73.4230, 26.5322, -8.75001, true)]
        public void InequalityOperator(double m1_11, double m1_12, double m1_13, double m1_14, double m1_21,
            double m1_22, double m1_23, double m1_24, double m1_31, double m1_32, double m1_33, double m1_34,
            double m1_41, double m1_42, double m1_43, double m1_44, double m2_11, double m2_12, double m2_13,
            double m2_14, double m2_21, double m2_22, double m2_23, double m2_24, double m2_31, double m2_32,
            double m2_33, double m2_34, double m2_41, double m2_42, double m2_43, double m2_44, bool expected)
        {
            // Arrange
            var m1 = new Matrix4(m1_11, m1_12, m1_13, m1_14, m1_21, m1_22, m1_23, m1_24, m1_31, m1_32, m1_33, m1_34,
                m1_41, m1_42, m1_43, m1_44);

            var m2 = new Matrix4(m2_11, m2_12, m2_13, m2_14, m2_21, m2_22, m2_23, m2_24, m2_31, m2_32, m2_33, m2_34,
                m2_41, m2_42, m2_43, m2_44);

            // Act
            var actual = m1 != m2;

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        #endregion
    }
}