using System.Diagnostics;

namespace Geisha.Engine.Core.Math;

/// <summary>
///     Projection of a point or 2D shape onto 2D axis.
/// </summary>
/// <remarks>
///     <see cref="Projection" /> is represented as a pair of values: <see cref="Min" /> and <see cref="Max" />, that
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
    internal Projection(double min, double max)
    {
        Debug.Assert(min <= max, "min <= max");
        Min = min;
        Max = max;
    }

    /// <summary>
    ///     Tests whether this <see cref="Projection" /> is overlapping other <see cref="Projection" />.
    /// </summary>
    /// <param name="other"><see cref="Projection" /> to test for overlapping.</param>
    /// <returns>True, if projections overlap, false otherwise.</returns>
    public bool Overlaps(in Projection other)
    {
        return Distance(other) <= 0;
    }

    /// <summary>
    ///     Calculates distance between this projection and other projection. Distance between two instances of
    ///     <see cref="Projection" /> is the length of the gap between them.
    /// </summary>
    /// <param name="other">Other instance of <see cref="Projection" />.</param>
    /// <returns>
    ///     Positive value of distance when there is a gap between two instances of <see cref="Projection" />. Negative value
    ///     of distance when two instances of <see cref="Projection" /> are overlapping.
    /// </returns>
    public double Distance(in Projection other)
    {
        return (System.Math.Abs(Min + Max - other.Min - other.Max) - (Max - Min + other.Max - other.Min)) * 0.5;
    }
}