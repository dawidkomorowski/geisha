using System;

namespace Geisha.Common
{
    /// <summary>
    ///     Static class providing utility methods for handling <see cref="TimeSpan" /> with single tick precision.
    /// </summary>
    public static class HighResolutionTimeSpan
    {
        /// <summary>
        ///     Returns a <see cref="TimeSpan" /> that represents a specified number of seconds.
        /// </summary>
        /// <param name="seconds">A number of seconds.</param>
        /// <returns>An object that represents <paramref name="seconds" />.</returns>
        public static TimeSpan FromSeconds(double seconds)
        {
            return TimeSpan.FromTicks((long) (TimeSpan.TicksPerSecond * seconds));
        }
    }
}