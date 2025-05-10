using System;

namespace Geisha.Engine.Core.Math;

/// <summary>
///     Represents 2D axis with arbitrary orientation.
/// </summary>
public readonly struct Axis
{
    /// <summary>
    ///     Creates new instance of <see cref="Axis" /> with direction given by vector.
    /// </summary>
    /// <param name="axisAlignedVector">Vector being source of direction for an axis.</param>
    public Axis(in Vector2 axisAlignedVector)
    {
        AxisAlignedUnitVector = axisAlignedVector.Unit; // Unit vector is required for simple projection with dot product.
    }

    /// <summary>
    ///     Unit vector with the direction aligned along the axis.
    /// </summary>
    public Vector2 AxisAlignedUnitVector { get; }

    /// <summary>
    ///     Returns orthogonal projection of a polygon, defined as set of points, onto an axis.
    /// </summary>
    /// <param name="vertices">Set of points to be projected.</param>
    /// <returns>Orthogonal projection of a polygon, defined as set of points, onto an axis.</returns>
    public Projection GetProjectionOf(ReadOnlySpan<Vector2> vertices)
    {
        var min = double.MaxValue;
        var max = double.MinValue;

        for (var i = 0; i < vertices.Length; i++)
        {
            var projected = vertices[i].Dot(AxisAlignedUnitVector);
            min = System.Math.Min(min, projected);
            max = System.Math.Max(max, projected);
        }

        return new Projection(min, max);
    }

    /// <summary>
    ///     Returns orthogonal projection of a point onto an axis.
    /// </summary>
    /// <param name="point">Point to be projected.</param>
    /// <returns>Orthogonal projection of a point onto an axis.</returns>
    /// <remarks>
    ///     <see cref="Projection" /> for a single point has <see cref="Projection.Min" /> equal to
    ///     <see cref="Projection.Max" />.
    /// </remarks>
    public Projection GetProjectionOf(in Vector2 point)
    {
        var pointProjection = point.Dot(AxisAlignedUnitVector);
        return new Projection(pointProjection, pointProjection);
    }

    /// <summary>
    ///     Returns orthogonal projection of a line segment onto an axis.
    /// </summary>
    /// <param name="lineSegment">Line segment to be projected.</param>
    /// <returns>Orthogonal projection of a line segment onto an axis.</returns>
    public Projection GetProjectionOf(in LineSegment lineSegment)
    {
        var min = double.MaxValue;
        var max = double.MinValue;

        // TODO This implementation seems to be incorrect. Add tests for it.
        var startPointProjection = lineSegment.StartPoint.Dot(AxisAlignedUnitVector);
        var endPointProjection = lineSegment.EndPoint.Dot(AxisAlignedUnitVector);
        min = System.Math.Min(min, startPointProjection);
        max = System.Math.Max(max, endPointProjection);
        return new Projection(min, max);
    }
}