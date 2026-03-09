using System;
using Geisha.Engine.Core.Math;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Math;

[TestFixture]
public class MathExTests
{
    #region AlmostEqual

    [TestCase(0, 0)]
    [TestCase(1d, 1d)]
    [TestCase(-1d, -1d)]
    [TestCase(double.MaxValue, double.MaxValue)]
    [TestCase(double.MinValue, double.MinValue)]
    [TestCase(double.Epsilon, double.Epsilon)]
    [TestCase(0d, -0d)] // Positive and negative zero
    public void AlmostEqual_ShouldReturnTrue_WhenValuesAreExactlyEqual(double a, double b)
    {
        // Arrange
        // Act
        var actual = MathEx.AlmostEqual(a, b);

        // Assert
        Assert.That(actual, Is.True);
    }

    // Infinity handling
    [TestCase(double.PositiveInfinity, double.PositiveInfinity, true)] // Same positive infinity
    [TestCase(double.NegativeInfinity, double.NegativeInfinity, true)] // Same negative infinity
    [TestCase(double.PositiveInfinity, double.NegativeInfinity, false)] // Different infinities
    [TestCase(double.PositiveInfinity, 1e100, false)] // Infinity vs large finite
    [TestCase(double.NegativeInfinity, -1e100, false)]
    [TestCase(double.PositiveInfinity, 0, false)] // Infinity vs zero
    [TestCase(double.NegativeInfinity, 0, false)]
    public void AlmostEqual_ShouldHandleInfinityCorrectly(double a, double b, bool expected)
    {
        // Arrange
        // Act
        var actual = MathEx.AlmostEqual(a, b);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(double.NaN, double.NaN)]
    [TestCase(double.NaN, 0)]
    [TestCase(0, double.NaN)]
    [TestCase(double.NaN, double.PositiveInfinity)]
    [TestCase(double.PositiveInfinity, double.NaN)]
    public void AlmostEqual_ShouldReturnFalse_WhenEitherValueIsNaN(double a, double b)
    {
        // Arrange
        // Act
        var actual = MathEx.AlmostEqual(a, b);

        // Assert
        Assert.That(actual, Is.False);
    }

    // Absolute tolerance: values near zero
    [TestCase(0, 1e-13, true)] // Within absolute tolerance (1e-12)
    [TestCase(0, 5e-13, true)]
    [TestCase(1e-100, 0, true)] // Very small values effectively zero
    [TestCase(1e-20, 1e-20 + 1e-35, true)] // Difference much smaller than tolerance
    [TestCase(0, 2e-12, false)] // Just outside absolute tolerance
    [TestCase(0, 1e-11, false)]
    [TestCase(1e-11, 0, false)]
    public void AlmostEqual_ShouldUseAbsoluteTolerance_ForValuesNearZero(double a, double b, bool expected)
    {
        // Arrange
        // Act
        var actual = MathEx.AlmostEqual(a, b);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    // Relative tolerance: large values
    [TestCase(1e10, 1e10 + 1e-3, true)] // diff: 1e-3, tolerance: 1e-12 * 1e10 = 1e-2
    [TestCase(1e20, 1e20 + 1e7, true)] // diff: 1e7, tolerance: 1e-12 * 1e20 = 1e8
    [TestCase(1000, 1000.000000001, true)] // diff: 1e-9, tolerance: 1e-12 * 1000 = 1e-9
    [TestCase(1e10, 1e10 + 1e-1, false)] // diff: 1e-1, exceeds tolerance
    [TestCase(1e20, 1e20 + 1e9, false)] // diff: 1e9, exceeds tolerance
    [TestCase(1000, 1000.01, false)] // diff: 1e-2, exceeds tolerance
    public void AlmostEqual_ShouldUseRelativeTolerance_ForLargeValues(double a, double b, bool expected)
    {
        // Arrange
        // Act
        var actual = MathEx.AlmostEqual(a, b);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    // Custom tolerances
    [TestCase(1.0, 1.0, 1e-10, 1e-10, true)]
    [TestCase(100.0, 100.001, 1e-5, 1e-3, true)]
    [TestCase(1.0, 2.0, 1e-15, 1e-15, false)]
    [TestCase(100.0, 100.1, 1e-5, 1e-3, false)]
    public void AlmostEqual_ShouldUseCustomTolerances_WhenProvided(double a, double b, double relativeTolerance, double absoluteTolerance, bool expected)
    {
        // Arrange
        // Act
        var actual = MathEx.AlmostEqual(a, b, relativeTolerance, absoluteTolerance);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(-1.0)]
    [TestCase(double.NaN)]
    public void AlmostEqual_ShouldThrowArgumentOutOfRangeException_WhenRelativeToleranceIsInvalid(double invalidTolerance)
    {
        // Arrange
        // Act & Assert
        Assert.That(() => MathEx.AlmostEqual(0, 0, invalidTolerance, 1e-12), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase(-1.0)]
    [TestCase(double.NaN)]
    public void AlmostEqual_ShouldThrowArgumentOutOfRangeException_WhenAbsoluteToleranceIsInvalid(double invalidTolerance)
    {
        // Arrange
        // Act & Assert
        Assert.That(() => MathEx.AlmostEqual(0, 0, 1e-12, invalidTolerance), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    #endregion

    [TestCase(-3, 5, 0, -3)]
    [TestCase(-3, 5, 1, 5)]
    [TestCase(-3, 5, 0.5, 1)]
    [TestCase(-3, 5, 0.25, -1)]
    public void Lerp_Test(double a, double b, double alpha, double expected)
    {
        // Arrange
        // Act
        var actual = MathEx.Lerp(a, b, alpha);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }
}
