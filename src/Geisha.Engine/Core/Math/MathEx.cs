namespace Geisha.Engine.Core.Math;

/// <summary>
///     MathEx provides static methods for common mathematical operations not included in <see cref="System.Math" />.
/// </summary>
public static class MathEx
{
    /// <summary>
    ///     Checks for equality of two <see cref="double" /> numbers within specified absolute and relative tolerance.
    /// </summary>
    /// <param name="a">First value to check for equality.</param>
    /// <param name="b">Second value to check for equality.</param>
    /// <param name="relativeTolerance">
    ///     Relative tolerance for comparing values. Default value is <c>1e-12</c>.
    /// </param>
    /// <param name="absoluteTolerance">
    ///     Absolute tolerance for comparing values. Default value is <c>1e-12</c>.
    /// </param>
    /// <returns><c>true</c> if specified numbers are considered equal; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     Absolute difference between specified numbers is compared against an effective tolerance computed as:
    ///     <c>max(absoluteTolerance, relativeTolerance * max(|a|, |b|))</c>.
    /// </remarks>
    public static bool AlmostEqual(double a, double b, double relativeTolerance = 1e-12, double absoluteTolerance = 1e-12)
    {
        if (double.IsNaN(a) || double.IsNaN(b)) return false;

        // Handles exact equality (including +0.0 == -0.0) and same infinities.
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (a == b) return true;

        // At this point, if either is infinite they are not equal (since a == b already returned false).
        if (double.IsInfinity(a) || double.IsInfinity(b)) return false;

        if (double.IsNaN(relativeTolerance) || relativeTolerance < 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(relativeTolerance), relativeTolerance, "Tolerance must be a non-negative number.");
        }

        if (double.IsNaN(absoluteTolerance) || absoluteTolerance < 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(absoluteTolerance), absoluteTolerance, "Tolerance must be a non-negative number.");
        }

        var difference = System.Math.Abs(a - b);
        var largerMagnitude = System.Math.Max(System.Math.Abs(a), System.Math.Abs(b));
        var tolerance = System.Math.Max(absoluteTolerance, largerMagnitude * relativeTolerance);

        return difference <= tolerance;
    }

    /// <summary>
    ///     Linearly interpolates from <see cref="double" /> <paramref name="a" /> to <see cref="double" />
    ///     <paramref name="b" /> proportionally to factor <paramref name="alpha" />.
    /// </summary>
    /// <param name="a">Source value for <see cref="double" /> interpolation.</param>
    /// <param name="b">Target value for <see cref="double" /> interpolation.</param>
    /// <param name="alpha">Interpolation factor in range from <c>0.0</c> to <c>1.0</c>.</param>
    /// <returns>Interpolated value of <see cref="double" />.</returns>
    /// <remarks>
    ///     When <paramref name="alpha" /> value is <c>0.0</c> the returned value is equal to <paramref name="a" />. When
    ///     <paramref name="alpha" /> value is <c>1.0</c> the returned value is equal to <paramref name="b" />.
    /// </remarks>
    public static double Lerp(double a, double b, double alpha)
    {
        return a * (1 - alpha) + b * alpha;
    }
}