using System;
using Geisha.Engine.Core.Math;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math
{
    [TestFixture]
    public class GMathTests
    {
        [TestCase(0, 0, true)]
        [TestCase(1d, 1d, true)]
        [TestCase(double.MaxValue, double.MaxValue, true)]
        [TestCase(double.MinValue, double.MinValue, true)]
        [TestCase(double.Epsilon, double.Epsilon, true)]
        [TestCase(double.Epsilon, 2 * double.Epsilon, true)]
        [TestCase(double.Epsilon, 3 * double.Epsilon, false)]
        [TestCase(1e20d, 1e20d + 1e5d, true)]
        [TestCase(1e20d, 1e20d + 1e6d, false)]
        [TestCase(1e-20d, 1e-20d + 1e-36d, true)]
        [TestCase(1e-20d, 1e-20d + 1e-35d, false)]
        public void AlmostEqual_Test(double a, double b, bool expected)
        {
            // Arrange
            // Act
            var actual = GMath.AlmostEqual(a, b);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(-3, 5, 0, -3)]
        [TestCase(-3, 5, 1, 5)]
        [TestCase(-3, 5, 0.5, 1)]
        [TestCase(-3, 5, 0.25, -1)]
        public void Lerp_Test(double a, double b, double alpha, double expected)
        {
            // Arrange
            // Act
            var actual = GMath.Lerp(a, b, alpha);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        // TODO Remove.
        [Test]
        public void Rotation_Test()
        {
            var transform = Matrix3x3.CreateTranslation(new Vector2(0, 0))
                            * Matrix3x3.CreateRotation(Angle.Deg2Rad(180))
                            * Matrix3x3.CreateScale(new Vector2(-1, 1))
                            * Matrix3x3.Identity;

            var sx = new Vector2(transform.M11, transform.M21).Length;
            Console.WriteLine($"sx = {sx}");

            var sy = new Vector2(transform.M12, transform.M22).Length;
            Console.WriteLine($"sy = {sy}");

            Console.WriteLine($"transform.M11 / sx = {transform.M11 / sx}");
            var rotation1 = System.Math.Acos(transform.M11 / sx);
            Console.WriteLine($"Angle.Rad2Deg(rotation1) = {Angle.Rad2Deg(rotation1)}");
            Console.WriteLine($"transform.M21 / sx = {transform.M21 / sx}");
            var rotation2 = System.Math.Asin(transform.M21 / sx);
            Console.WriteLine($"Angle.Rad2Deg(rotation2) = {Angle.Rad2Deg(rotation2)}");

            Console.WriteLine($"transform.M12 / sy = {transform.M12 / sy}");
            var rotation3 = System.Math.Asin(-transform.M12 / sy);
            Console.WriteLine($"Angle.Rad2Deg(rotation3) = {Angle.Rad2Deg(rotation3)}");
            Console.WriteLine($"transform.M22 / sy = {transform.M22 / sy}");
            var rotation4 = System.Math.Acos(transform.M22 / sy);
            Console.WriteLine($"Angle.Rad2Deg(rotation4) = {Angle.Rad2Deg(rotation4)}");
        }
    }
}