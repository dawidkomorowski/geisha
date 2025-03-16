using System.Diagnostics;
using System;

namespace Geisha.Engine.Core.Math;

/// <summary>
///     Provides methods for inspecting properties of 2D polygon.
/// </summary>
public static class Polygon2D
{
    /// <summary>
    ///     Calculates orientation of a specified convex polygon.
    /// </summary>
    /// <param name="convexPolygon">Ordered set of vertices representing 2D convex polygon.</param>
    /// <returns><see cref="Polygon2DOrientation" /> that describes orientation of the specified polygon.</returns>
    /// <remarks>
    ///     This method assumes the specified polygon is strictly convex, that is, every internal angle is strictly less
    ///     than 180 degrees.
    /// </remarks>
    public static Polygon2DOrientation GetOrientation(ReadOnlySpan<Vector2> convexPolygon)
    {
        Debug.Assert(convexPolygon.Length > 2, "convexPolygon.Length > 2");

        // Implementation based on: https://en.wikipedia.org/wiki/Curve_orientation
        var v0 = convexPolygon[0];
        var v1 = convexPolygon[1];
        var v2 = convexPolygon[2];

        var detOrientation = (v1.X - v0.X) * (v2.Y - v0.Y) - (v2.X - v0.X) * (v1.Y - v0.Y);

        return detOrientation switch
        {
            > 0 => Polygon2DOrientation.CounterClockwise,
            0 => Polygon2DOrientation.Collinear,
            <= 0 => Polygon2DOrientation.Clockwise,
            _ => throw new InvalidOperationException("Unreachable code. Use UnreachableException once migrated to .NET 7.")
        };
    }

    [Conditional("DEBUG")]
    internal static void DebugAssert_PolygonIsOrientedCounterClockwise(ReadOnlySpan<Vector2> polygon)
    {
        Debug.Assert(GetOrientation(polygon) == Polygon2DOrientation.CounterClockwise, "Polygon is not oriented counterclockwise.");
    }

    [Conditional("DEBUG")]
    internal static void DebugAssert_PolygonIsOrientedCounterClockwise(ReadOnlySpan<Vector2> polygon, string message)
    {
        Debug.Assert(GetOrientation(polygon) == Polygon2DOrientation.CounterClockwise, $"Polygon is not oriented counterclockwise. {message}");
    }
}

/// <summary>
///     Represents possible orientations of a 2D polygon specified by an ordered set of vertices.
/// </summary>
public enum Polygon2DOrientation
{
    /// <summary>
    ///     2D polygon is oriented clockwise, that is, following the subsequent vertices of the polygon its interior is always
    ///     to the right.
    /// </summary>
    Clockwise,

    /// <summary>
    ///     2D polygon is oriented counterclockwise, that is, following the subsequent vertices of the polygon its interior is
    ///     always to the left.
    /// </summary>
    CounterClockwise,

    /// <summary>
    ///     Vertices of 2D polygon are collinear making it not a simple polygon.
    /// </summary>
    Collinear
}