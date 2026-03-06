namespace Geisha.Engine.Core.Math;

/// <summary>
///     Provides collection of helper methods for common angle operations.
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

    // TODO: Add documentation.
    // TODO: Include information about the range of the output and how it handles edge cases (e.g., exactly -π)?
    public static double NormalizeRadiansToPi(double radians) => System.Math.IEEERemainder(radians, 2 * System.Math.PI);

    // TODO: Add documentation.
    public static double NormalizeRadiansTo2Pi(double radians)
    {
        const double twoPi = 2 * System.Math.PI;
        return radians - twoPi * System.Math.Floor(radians / twoPi);
    }
}