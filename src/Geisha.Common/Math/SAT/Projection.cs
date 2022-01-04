using System.Diagnostics;

namespace Geisha.Common.Math.SAT
{
    /// <summary>
    ///     Projection of a point or 2D shape onto 2D axis used in SAT.
    /// </summary>
    /// <remarks>
    ///     <see cref="Projection" /> is represented as pair of values: <see cref="Min" /> and <see cref="Max" />, that
    ///     define interval on an axis being the projection of a shape.
    /// </remarks>
    public readonly struct Projection
    {
        /// <summary>
        ///     Min value of projection.
        /// </summary>
        public readonly double Min;

        /// <summary>
        ///     Max value of projection.
        /// </summary>
        public readonly double Max;

        /// <summary>
        ///     Creates new instance of <see cref="Projection" /> with given min and max values.
        /// </summary>
        /// <param name="min">Min value of projection.</param>
        /// <param name="max">Max value of projection.</param>
        public Projection(double min, double max)
        {
            // This struct is public. Should it throw an exception instead of assertion or should it be internal?
            Debug.Assert(min <= max, $"min < max -- min[{min}] is not lower than max[{max}]");
            Min = min;
            Max = max;
        }

        /// <summary>
        ///     Tests whether this <see cref="Projection" /> is overlapping other <see cref="Projection" />.
        /// </summary>
        /// <param name="other"><see cref="Projection" /> to test for overlapping.</param>
        /// <returns>True, if projections overlap, false otherwise.</returns>
        public bool Overlaps(Projection other)
        {
            return System.Math.Abs((Min + Max - (other.Min + other.Max)) / 2) <= (Max - Min + (other.Max - other.Min)) / 2;
        }
    }
}