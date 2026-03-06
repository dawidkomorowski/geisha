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
    public void DegreesToRadians_And_RadiansToDegrees_Test(double degrees, double radians)
    {
        // Arrange
        // Act
        var actualRadians = Angle.DegreesToRadians(degrees);
        var actualDegrees = Angle.RadiansToDegrees(radians);

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
    [TestCase(170 * System.Math.PI / 180, 170 * System.Math.PI / 180)]
    [TestCase(-170 * System.Math.PI / 180, -170 * System.Math.PI / 180)]
    [TestCase(190 * System.Math.PI / 180, -170 * System.Math.PI / 180)]
    [TestCase(-190 * System.Math.PI / 180, 170 * System.Math.PI / 180)]
    // Large multiples of 2*PI should normalize near 0.
    [TestCase(1_000_000 * 2 * System.Math.PI, 0d, 1e-9)]
    [TestCase(double.NaN, double.NaN)]
    [TestCase(double.PositiveInfinity, double.NaN)]
    [TestCase(double.NegativeInfinity, double.NaN)]
    public void NormalizeRadiansToPi_Test(double radians, double expected, double tolerance = 1e-12)
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

    [TestCase(0d, 0d)]
    [TestCase(System.Math.PI, System.Math.PI)]
    [TestCase(-System.Math.PI, System.Math.PI)]
    [TestCase(2 * System.Math.PI, 0d)]
    [TestCase(-2 * System.Math.PI, 0d)]
    [TestCase(3 * System.Math.PI, System.Math.PI)]
    [TestCase(-3 * System.Math.PI, System.Math.PI)]
    [TestCase(5 * System.Math.PI / 2, System.Math.PI / 2)]
    [TestCase(-5 * System.Math.PI / 2, 3 * System.Math.PI / 2)]
    [TestCase(10 * System.Math.PI / 180, 10 * System.Math.PI / 180)]
    [TestCase(350 * System.Math.PI / 180, 350 * System.Math.PI / 180)]
    [TestCase(-10 * System.Math.PI / 180, 350 * System.Math.PI / 180)]
    [TestCase(-350 * System.Math.PI / 180, 10 * System.Math.PI / 180)]
    // Large multiples of 2*PI should normalize near 0.
    [TestCase(1_000_000 * 2 * System.Math.PI, 0d)]
    [TestCase(double.NaN, double.NaN)]
    [TestCase(double.PositiveInfinity, double.NaN)]
    [TestCase(double.NegativeInfinity, double.NaN)]
    public void NormalizeRadiansTo2Pi_Test(double radians, double expected)
    {
        // Arrange
        const double tolerance = 1e-12;

        // Act
        var actual = Angle.NormalizeRadiansTo2Pi(radians);

        // Assert
        if (double.IsNaN(expected))
        {
            Assert.That(actual, Is.NaN);
        }
        else
        {
            Assert.That(actual, Is.EqualTo(expected).Within(tolerance));
            Assert.That(actual, Is.GreaterThanOrEqualTo(0));
            Assert.That(actual, Is.LessThan(2 * System.Math.PI));
        }
    }
}