using System;

namespace Geisha.Engine.Core.Math;

/// <summary>
///     Represents 2D line segment defined by start point and end point.
/// </summary>
public readonly struct LineSegment : IEquatable<LineSegment>
{
    /// <summary>
    ///     Creates new instance of <see cref="LineSegment" /> with specified start point and end point.
    /// </summary>
    /// <param name="startPoint">Start point of line segment.</param>
    /// <param name="endPoint">End point of line segment.</param>
    public LineSegment(in Vector2 startPoint, in Vector2 endPoint)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
    }

    /// <summary>
    ///     Start point of line segment.
    /// </summary>
    public Vector2 StartPoint { get; }

    /// <summary>
    ///     End point of line segment.
    /// </summary>
    public Vector2 EndPoint { get; }

    /// <summary>
    ///     Represents possible results of intersection between two instances of <see cref="LineSegment" />.
    /// </summary>
    public enum IntersectionResult
    {
        /// <summary>
        ///     Two instances of <see cref="LineSegment" /> have no intersection point. Line segments are parallel and either
        ///     overlapping or disjoint.
        /// </summary>
        NoIntersection,

        /// <summary>
        ///     Two instances of <see cref="LineSegment" /> have no intersection point that belongs to either of them. However,
        ///     there is an intersection point for lines created from these line segments.
        /// </summary>
        LineIntersection,

        /// <summary>
        ///     Two instances of <see cref="LineSegment" /> have intersection point.
        /// </summary>
        LineSegmentIntersection
    }

    /// <summary>
    ///     Tests intersection of line segment with the other one.
    /// </summary>
    /// <param name="other">Other <see cref="LineSegment" />.</param>
    /// <param name="intersectionPoint">
    ///     Contains either intersection point or default if
    ///     <see cref="IntersectionResult.NoIntersection" /> was returned.
    /// </param>
    /// <returns><see cref="IntersectionResult" /> that specifies how to interpret <paramref name="intersectionPoint" />.</returns>
    public IntersectionResult Intersects(in LineSegment other, out Vector2 intersectionPoint)
    {
        var thisVector = EndPoint - StartPoint;
        var otherVector = other.EndPoint - other.StartPoint;

        var vectorCross = thisVector.Cross(otherVector);

        // Parallel lines have no intersection.
        if (vectorCross == 0)
        {
            intersectionPoint = default;
            return IntersectionResult.NoIntersection;
        }

        // Use parametric equation to describe a line:
        // L(t) = s + t*v
        // L(t) - line point for parameter t
        // s - start point of line segment
        // v - vector from start point to end point of line segment; v = e - s; e - end point
        // t - parameter; in range [0,1] it describes line segment; for all values it describes a line
        // Solve L1(t) = L2(u) and then use parametric equation to find intersection point.

        var startPointDiff = other.StartPoint - StartPoint;
        var thisParam = startPointDiff.Cross(otherVector) / vectorCross;
        var otherParam = startPointDiff.Cross(thisVector) / vectorCross;

        intersectionPoint = StartPoint + thisVector * thisParam;

        if (thisParam is >= 0 and <= 1 && otherParam is >= 0 and <= 1)
        {
            return IntersectionResult.LineSegmentIntersection;
        }

        return IntersectionResult.LineIntersection;
    }

    /// <summary>
    ///     Converts the value of the current <see cref="LineSegment" /> object to its equivalent string representation.
    /// </summary>
    /// <returns>A string representation of the value of the current <see cref="LineSegment" /> object.</returns>
    public override string ToString() => $"{nameof(StartPoint)}: {StartPoint}, {nameof(EndPoint)}: {EndPoint}";

    /// <inheritdoc />
    public bool Equals(LineSegment other) => StartPoint.Equals(other.StartPoint) && EndPoint.Equals(other.EndPoint);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is LineSegment other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(StartPoint, EndPoint);

    /// <summary>
    ///     Determines whether two specified instances of <see cref="LineSegment" /> are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
    ///     <see cref="LineSegment" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(LineSegment left, LineSegment right) => left.Equals(right);

    /// <summary>
    ///     Determines whether two specified instances of <see cref="LineSegment" /> are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
    ///     <see cref="LineSegment" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(LineSegment left, LineSegment right) => !left.Equals(right);
}