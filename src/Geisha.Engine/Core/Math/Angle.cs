namespace Geisha.Engine.Core.Math
{
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
    }
}