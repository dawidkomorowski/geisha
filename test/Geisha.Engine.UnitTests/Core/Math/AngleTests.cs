using Geisha.Engine.Core.Math;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math;

[TestFixture]
public class AngleTests
{
    [TestCase(0, 0)]
    [TestCase(90, System.Math.PI / 2)]
    [TestCase(-90, -System.Math.PI / 2)]
    [TestCase(180, System.Math.PI)]
    [TestCase(-180, -System.Math.PI)]
    [TestCase(360, 2 * System.Math.PI)]
    [TestCase(-360, -2 * System.Math.PI)]
    [TestCase(720, 4 * System.Math.PI)]
    [TestCase(-720, -4 * System.Math.PI)]
    [TestCase(37.375612, 0.65232748934790286)]
    [TestCase(-37.375612, -0.65232748934790286)]
    public void Deg2Rad_And_Rad2Deg(double degrees, double radians)
    {
        // Arrange
        // Act
        var actualRadians = Angle.Deg2Rad(degrees);
        var actualDegrees = Angle.Rad2Deg(radians);

        // Assert
        Assert.That(actualRadians, Is.EqualTo(radians));
        Assert.That(actualDegrees, Is.EqualTo(degrees));
    }

    [TestCase(0d, 0d)]
    [TestCase(System.Math.PI, System.Math.PI)]
    [TestCase(-System.Math.PI, -System.Math.PI)]
    [TestCase(2 * System.Math.PI, 0d)]
    [TestCase(-2 * System.Math.PI, 0d)]
    [TestCase(3 * System.Math.PI, -System.Math.PI)]
    [TestCase(-3 * System.Math.PI, System.Math.PI)]
    [TestCase(5 * System.Math.PI / 2, System.Math.PI / 2)]
    [TestCase(-5 * System.Math.PI / 2, -System.Math.PI / 2)]
    // 190° => -170°
    [TestCase(190 * System.Math.PI / 180, -170 * System.Math.PI / 180)]
    // -190° => 170°
    [TestCase(-190 * System.Math.PI / 180, 170 * System.Math.PI / 180)]
    // Large multiples of 2*PI should normalize near 0.
    [TestCase(1_000_000 * 2 * System.Math.PI, 0d, 1e-9)]
    [TestCase(double.NaN, double.NaN)]
    [TestCase(double.PositiveInfinity, double.NaN)]
    [TestCase(double.NegativeInfinity, double.NaN)]
    public void NormalizeRadiansToPi(double radians, double expected, double tolerance = 1e-12)
    {
        // Arrange
        // Act
        var actual = Angle.NormalizeRadiansToPi(radians);

        // Assert
        if (double.IsNaN(expected))
        {
            Assert.That(actual, Is.NaN);
        }
        else
        {
            Assert.That(actual, Is.EqualTo(expected).Within(tolerance));
            Assert.That(actual, Is.InRange(-System.Math.PI, System.Math.PI));
        }
    }
}