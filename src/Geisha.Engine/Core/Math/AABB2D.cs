using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Geisha.Engine.Core.Math;

// TODO: Add documentation.
// TODO: Add tests.
// TODO: Add missing APIs.
// ReSharper disable once InconsistentNaming
public readonly record struct AABB2D
{
    public AABB2D(in Vector2 min, in Vector2 max)
    {
        Debug.Assert(min.X <= max.X, "Min.X must be less than or equal to Max.X.");
        Debug.Assert(min.Y <= max.Y, "Min.Y must be less than or equal to Max.Y.");

        Min = min;
        Max = max;
    }

    public AABB2D(double minX, double minY, double maxX, double maxY) : this(new Vector2(minX, minY), new Vector2(maxX, maxY))
    {
    }

    public Vector2 Min { get; }
    public Vector2 Max { get; }

    public Vector2 Center => Min.Midpoint(Max);
    public Vector2 Size => Max - Min;
    public double Width => Max.X - Min.X;
    public double Height => Max.Y - Min.Y;

    public bool Contains(in Vector2 point) => Min.X <= point.X && point.X <= Max.X && Min.Y <= point.Y && point.Y <= Max.Y;
    public bool Contains(in AABB2D other) => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Overlaps(in AABB2D other) => Max.X >= other.Min.X && Min.X <= other.Max.X && Max.Y >= other.Min.Y && Min.Y <= other.Max.Y;

    public AxisAlignedRectangle ToAxisAlignedRectangle() => new(Center, Size);
}