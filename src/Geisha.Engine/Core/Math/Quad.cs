using System;

namespace Geisha.Engine.Core.Math;

public readonly struct Quad : IEquatable<Quad>
{
    public Quad(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
    {
        V1 = v1;
        V2 = v2;
        V3 = v3;
        V4 = v4;
    }

    public Vector2 V1 { get; }
    public Vector2 V2 { get; }
    public Vector2 V3 { get; }
    public Vector2 V4 { get; }

    public Quad Transform(in Matrix3x3 transform)
    {
        return new Quad(
            (transform * V1.Homogeneous).ToVector2(),
            (transform * V2.Homogeneous).ToVector2(),
            (transform * V3.Homogeneous).ToVector2(),
            (transform * V4.Homogeneous).ToVector2()
        );
    }

    public AxisAlignedRectangle GetBoundingRectangle()
    {
        return new AxisAlignedRectangle();
    }

    public override string ToString()
    {
        return $"{nameof(V1)}: {V1}, {nameof(V2)}: {V2}, {nameof(V3)}: {V3}, {nameof(V4)}: {V4}";
    }

    public bool Equals(Quad other)
    {
        return V1.Equals(other.V1) && V2.Equals(other.V2) && V3.Equals(other.V3) && V4.Equals(other.V4);
    }

    public override bool Equals(object? obj)
    {
        return obj is Quad other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(V1, V2, V3, V4);
    }

    public static bool operator ==(Quad left, Quad right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Quad left, Quad right)
    {
        return !left.Equals(right);
    }
}