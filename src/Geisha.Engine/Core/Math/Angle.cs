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
    // TODO Rename to DegreesToRadians?
    public static double Deg2Rad(double degrees)
    {
        return degrees * (System.Math.PI / 180);
    }

    /// <summary>
    ///     Converts angle given in radians to respective angle in degrees.
    /// </summary>
    /// <param name="radians">Angle in radians.</param>
    /// <returns>Angle in degrees.</returns>
    public static double Rad2Deg(double radians)
    {
        return radians * (180 / System.Math.PI);
    }

    // TODO: Review.
    // TODO: Add tests.
    // TODO: Add documentation.
    // TODO: Include information about the range of the output and how it handles edge cases (e.g., exactly -π)?
    // TODO: Include implementation of NormalizeRadiansTo2Pi?
    public static double NormalizeRadiansToPi(double radians)
    {
        // Produces an angle in (-PI, PI], matching expectations like 190° => -170°.
        // TODO: Investigate "The comment says it produces (-PI, PI].
        //       IEEERemainder can return -π for some inputs (e.g., exactly -π), so the true range is closer to [-π, π].
        //       If you strictly need (-π, π], you’d special-case -π to +π."
        return System.Math.IEEERemainder(radians, 2 * System.Math.PI);
    }
}