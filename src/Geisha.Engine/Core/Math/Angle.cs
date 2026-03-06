namespace Geisha.Engine.Core.Math;

/// <summary>
///     Provides a collection of helper methods for common angle operations.
/// </summary>
public static class Angle
{
    /// <summary>
    ///     Converts angle given in degrees to respective angle in radians.
    /// </summary>
    /// <param name="degrees">Angle in degrees.</param>
    /// <returns>Angle in radians.</returns>
    public static double DegreesToRadians(double degrees)
    {
        return degrees * (System.Math.PI / 180);
    }

    /// <summary>
    ///     Converts angle given in radians to respective angle in degrees.
    /// </summary>
    /// <param name="radians">Angle in radians.</param>
    /// <returns>Angle in degrees.</returns>
    public static double RadiansToDegrees(double radians)
    {
        return radians * (180 / System.Math.PI);
    }

    /// <summary>
    ///     Normalizes an angle given in radians to the range from <c>-π</c> to <c>π</c>.
    /// </summary>
    /// <param name="radians">Angle in radians.</param>
    /// <returns>Angle in radians normalized to the range from <c>-π</c> to <c>π</c>.</returns>
    /// <remarks>
    ///     The returned value is within the range <c>[-π, π]</c>.
    ///     This method uses <see cref="System.Math.IEEERemainder(double,double)" /> and therefore its handling of edge cases
    ///     follows IEEE remainder rules (e.g. <c>3π</c> normalizes to <c>-π</c>, while <c>-3π</c> normalizes to <c>π</c>).
    ///     If <paramref name="radians" /> is <see cref="double.NaN" /> or an infinity, the result is <see cref="double.NaN" />.
    /// </remarks>
    public static double NormalizeRadiansToPi(double radians) => System.Math.IEEERemainder(radians, 2 * System.Math.PI);

    /// <summary>
    ///     Normalizes an angle given in radians to the range from <c>0</c> (inclusive) to <c>2π</c> (exclusive).
    /// </summary>
    /// <param name="radians">Angle in radians.</param>
    /// <returns>Angle in radians normalized to the range from <c>0</c> (inclusive) to <c>2π</c> (exclusive).</returns>
    /// <remarks>
    ///     The returned value is within the range <c>[0, 2π)</c>.
    ///     If <paramref name="radians" /> is <see cref="double.NaN" /> or an infinity, the result is <see cref="double.NaN" />.
    /// </remarks>
    public static double NormalizeRadiansTo2Pi(double radians)
    {
        const double twoPi = 2 * System.Math.PI;
        return radians - twoPi * System.Math.Floor(radians / twoPi);
    }
}