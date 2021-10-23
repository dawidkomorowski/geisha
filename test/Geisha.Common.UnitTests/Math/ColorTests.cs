using System;
using Geisha.Common.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Math
{
    [TestFixture]
    public class ColorTests
    {
        #region Properties

        [Test]
        public void A_ShouldReturnAlphaComponent()
        {
            // Arrange
            var random = new Random();
            var alpha = random.Next(0, 256);

            var color = Color.FromArgb(alpha, random.Next(), random.Next(), random.Next());

            // Act
            var actualAlpha = color.A;

            // Assert
            Assert.That(actualAlpha, Is.EqualTo(alpha));
        }

        [Test]
        public void R_ShouldReturnRedComponent()
        {
            // Arrange
            var random = new Random();
            var red = random.Next(0, 256);

            var color = Color.FromArgb(random.Next(), red, random.Next(), random.Next());

            // Act
            var actualRed = color.R;

            // Assert
            Assert.That(actualRed, Is.EqualTo(red));
        }

        [Test]
        public void G_ShouldReturnGreenComponent()
        {
            // Arrange
            var random = new Random();
            var green = random.Next(0, 256);

            var color = Color.FromArgb(random.Next(), random.Next(), green, random.Next());

            // Act
            var actualGreen = color.G;

            // Assert
            Assert.That(actualGreen, Is.EqualTo(green));
        }

        [Test]
        public void B_ShouldReturnBlueComponent()
        {
            // Arrange
            var random = new Random();
            var blue = random.Next(0, 256);

            var color = Color.FromArgb(random.Next(), random.Next(), random.Next(), blue);

            // Act
            var actualBlue = color.B;

            // Assert
            Assert.That(actualBlue, Is.EqualTo(blue));
        }

        [TestCase(0, 0)]
        [TestCase(255, 1)]
        [TestCase(127, 0.49803921568627450980392)]
        public void DoubleA_ShouldReturnAlphaComponentAsDouble(int alpha, double expectedAlpha)
        {
            // Arrange
            var random = new Random();
            var color = Color.FromArgb(alpha, random.Next(), random.Next(), random.Next());

            // Act
            var actualAlpha = color.DoubleA;

            // Assert
            Assert.That(actualAlpha, Is.EqualTo(expectedAlpha));
        }

        [TestCase(0, 0)]
        [TestCase(255, 1)]
        [TestCase(127, 0.49803921568627450980392)]
        public void DoubleR_ShouldReturnRedComponentAsDouble(int red, double expectedRed)
        {
            // Arrange
            var random = new Random();
            var color = Color.FromArgb(random.Next(), red, random.Next(), random.Next());

            // Act
            var actualRed = color.DoubleR;

            // Assert
            Assert.That(actualRed, Is.EqualTo(expectedRed));
        }

        [TestCase(0, 0)]
        [TestCase(255, 1)]
        [TestCase(127, 0.49803921568627450980392)]
        public void DoubleG_ShouldReturnGreenComponentAsDouble(int green, double expectedGreen)
        {
            // Arrange
            var random = new Random();
            var color = Color.FromArgb(random.Next(), random.Next(), green, random.Next());

            // Act
            var actualGreen = color.DoubleG;

            // Assert
            Assert.That(actualGreen, Is.EqualTo(expectedGreen));
        }

        [TestCase(0, 0)]
        [TestCase(255, 1)]
        [TestCase(127, 0.49803921568627450980392)]
        public void DoubleB_ShouldReturnBlueComponentAsDouble(int blue, double expectedBlue)
        {
            // Arrange
            var random = new Random();
            var color = Color.FromArgb(random.Next(), random.Next(), random.Next(), blue);

            // Act
            var actualBlue = color.DoubleB;

            // Assert
            Assert.That(actualBlue, Is.EqualTo(expectedBlue));
        }

        #endregion

        #region Static methods

        [TestCase(0x00000000u)]
        [TestCase(0xFFFFFFFFu)]
        [TestCase(0x0F0F0F0Fu)]
        public void FromArgb_ShouldCreateColorFromArgbInteger(uint argb)
        {
            // Arrange
            // Act
            var color = Color.FromArgb((int)argb);

            // Assert
            Assert.That(color.ToArgb(), Is.EqualTo((int)argb));
        }

        [TestCase(0, 0, 0, 0, 0x00000000u)]
        [TestCase(-1, -1, -1, -1, 0x00000000u)]
        [TestCase(255, 255, 255, 255, 0xFFFFFFFFu)]
        [TestCase(256, 256, 256, 256, 0xFFFFFFFFu)]
        [TestCase(15, 15, 15, 15, 0x0F0F0F0Fu)]
        public void FromArgb_ShouldCreateColorFromAIntRIntGIntBInt(int alpha, int red, int green, int blue, uint argb)
        {
            // Arrange
            // Act
            var color = Color.FromArgb(alpha, red, green, blue);

            // Assert
            Assert.That(color.ToArgb(), Is.EqualTo((int)argb));
        }

        [TestCase(0, 0, 0, 0, 0x00000000u)]
        [TestCase(-1, -1, -1, -1, 0x00000000u)]
        [TestCase(1, 1, 1, 1, 0xFFFFFFFFu)]
        [TestCase(2, 2, 2, 2, 0xFFFFFFFFu)]
        [TestCase(0.5, 0.5, 0.5, 0.5, 0x7F7F7F7Fu)]
        public void FromArgb_ShouldCreateColorFromADoubleRDoubleGDoubleBDouble(double alpha, double red, double green, double blue, uint argb)
        {
            // Arrange
            // Act
            var color = Color.FromArgb(alpha, red, green, blue);

            // Assert
            Assert.That(color.ToArgb(), Is.EqualTo((int)argb));
        }

        #endregion

        #region Methods

        [TestCase(0x00000000u, 0x00000000u, true)]
        [TestCase(0xFFFFFFFFu, 0xFFFFFFFFu, true)]
        [TestCase(0xFDB97531u, 0xFDB97531u, true)]
        [TestCase(0xFDB97531u, 0xFDB97530u, false)]
        public void EqualityMembers_ShouldEqualColor_WhenArgbIsEqual(uint argb1, uint argb2, bool expectedIsEqual)
        {
            // Arrange
            var color1 = Color.FromArgb((int)argb1);
            var color2 = Color.FromArgb((int)argb2);

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(color1, color2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }

        [TestCase(0, 0, 0, 0, "A: 0, R: 0, G: 0, B: 0")]
        [TestCase(255, 255, 255, 255, "A: 255, R: 255, G: 255, B: 255")]
        [TestCase(13, 87, 137, 249, "A: 13, R: 87, G: 137, B: 249")]
        public void ToString_Test(int alpha, int red, int green, int blue, string expected)
        {
            // Arrange
            var color = Color.FromArgb(alpha, red, green, blue);

            // Act
            var actual = color.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        #endregion
    }
}