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
        Min = min;
        Max = max;
    }

    public Vector2 Min { get; }
    public Vector2 Max { get; }

    public Vector2 Center => Min.Midpoint(Max);
    public Vector2 Dimensions => Max - Min;

    public bool Contains(in Vector2 point) => Min.X <= point.X && point.X <= Max.X && Min.Y <= point.Y && point.Y <= Max.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Overlaps(in AABB2D other) => Max.X >= other.Min.X && Min.X <= other.Max.X && Max.Y >= other.Min.Y && Min.Y <= other.Max.Y;

    public AxisAlignedRectangle ToAxisAlignedRectangle() => new(Center, Dimensions);
}