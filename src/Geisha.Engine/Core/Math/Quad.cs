using System;

namespace Geisha.Engine.Core.Math;

/// <summary>
///     Represents arbitrary 2D quadrilateral.
/// </summary>
public readonly struct Quad : IEquatable<Quad>
{
    /// <summary>
    ///     Creates new instance of <see cref="Quad" /> with specified vertices.
    /// </summary>
    /// <param name="v1">First vertex of quad.</param>
    /// <param name="v2">Second vertex of quad.</param>
    /// <param name="v3">Third vertex of quad.</param>
    /// <param name="v4">Fourth vertex of quad.</param>
    public Quad(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
    {
        V1 = v1;
        V2 = v2;
        V3 = v3;
        V4 = v4;
    }

    /// <summary>
    ///     First vertex of quad.
    /// </summary>
    public Vector2 V1 { get; }

    /// <summary>
    ///     Second vertex of quad.
    /// </summary>
    public Vector2 V2 { get; }

    /// <summary>
    ///     Third vertex of quad.
    /// </summary>
    public Vector2 V3 { get; }

    /// <summary>
    ///     Fourth vertex of quad.
    /// </summary>
    public Vector2 V4 { get; }

    /// <summary>
    ///     Returns <see cref="Quad" /> that is this <see cref="Quad" /> transformed by given <see cref="Matrix3x3" />.
    /// </summary>
    /// <param name="transform">Transformation matrix used to transform quad.</param>
    /// <returns><see cref="Quad" /> transformed by given matrix.</returns>
    public Quad Transform(in Matrix3x3 transform)
    {
        return new Quad(
            (transform * V1.Homogeneous).ToVector2(),
            (transform * V2.Homogeneous).ToVector2(),
            (transform * V3.Homogeneous).ToVector2(),
            (transform * V4.Homogeneous).ToVector2()
        );
    }

    /// <summary>
    ///     Gets <see cref="AxisAlignedRectangle" /> that encloses this <see cref="Quad" />.
    /// </summary>
    /// <returns><see cref="AxisAlignedRectangle" /> that encloses this <see cref="Quad" />.</returns>
    public AxisAlignedRectangle GetBoundingRectangle()
    {
        ReadOnlySpan<Vector2> vertices = stackalloc Vector2[4] { V1, V2, V3, V4 };
        return new AxisAlignedRectangle(vertices);
    }

    /// <summary>
    ///     Converts the value of the current <see cref="Quad" /> object to its equivalent string representation.
    /// </summary>
    /// <returns>A string representation of the value of the current <see cref="Quad" /> object.</returns>
    public override string ToString()
    {
        return $"{nameof(V1)}: {V1}, {nameof(V2)}: {V2}, {nameof(V3)}: {V3}, {nameof(V4)}: {V4}";
    }

    /// <inheritdoc />
    public bool Equals(Quad other)
    {
        return V1.Equals(other.V1) && V2.Equals(other.V2) && V3.Equals(other.V3) && V4.Equals(other.V4);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Quad other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(V1, V2, V3, V4);
    }

    /// <summary>
    ///     Determines whether two specified instances of <see cref="Quad" /> are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
    ///     <see cref="Quad" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(Quad left, Quad right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Determines whether two specified instances of <see cref="Quad" /> are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
    ///     <see cref="Quad" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(Quad left, Quad right)
    {
        return !left.Equals(right);
    }
}