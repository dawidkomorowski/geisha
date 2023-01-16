namespace Geisha.Engine.Core.Math
{
    /// <summary>
    ///     Geisha math provides static methods for common mathematical operations not included in <see cref="System.Math" />.
    /// </summary>
    public static class GMath
    {
        /// <summary>
        ///     Checks for equality of two <see cref="double" /> numbers within absolute difference of <paramref name="maxDiff" />
        ///     and relative difference of <paramref name="maxRelativeDiff" />.
        /// </summary>
        /// <param name="a">First value to check for equality.</param>
        /// <param name="b">Second value to check for equality.</param>
        /// <param name="maxDiff">
        ///     Maximum absolute difference between values to treat them as equal. Default value is
        ///     <see cref="double.Epsilon" />.
        /// </param>
        /// <param name="maxRelativeDiff">
        ///     Maximum relative difference between values to treat them as equal. Default value is
        ///     <c>1e-15</c>.
        /// </param>
        /// <returns><c>true</c> if specified numbers are consider equal; otherwise, <c>false</c>.</returns>
        /// <remarks>
        ///     Absolute value of difference between specified numbers is compared with <paramref name="maxDiff" />. If
        ///     difference is less than <paramref name="maxDiff" /> the numbers are considered equal. If difference is greater than
        ///     <paramref name="maxDiff" />, it is then compared with magnitude of greater number scaled by
        ///     <paramref name="maxRelativeDiff" />. If difference is lesser the numbers are considered equal.
        /// </remarks>
        public static bool AlmostEqual(double a, double b, double maxDiff = double.Epsilon, double maxRelativeDiff = 1e-15)
        {
            var diff = System.Math.Abs(a - b);

            if (diff <= maxDiff)
            {
                return true;
            }

            var largerMagnitude = System.Math.Max(System.Math.Abs(a), System.Math.Abs(b));

            return diff <= largerMagnitude * maxRelativeDiff;
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
}