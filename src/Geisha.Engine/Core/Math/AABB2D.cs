using System;
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

    public static AABB2D FromSize(in Vector2 size)
    {
        var halfSize = size * 0.5;
        return new AABB2D(-halfSize, halfSize);
    }

    public static AABB2D FromSize(in SizeD size) => FromSize(size.ToVector2());
    public static AABB2D FromSize(in Size size) => FromSize(size.ToVector2());

    public static AABB2D FromSize(double width, double height)
    {
        var halfWidth = width * 0.5;
        var halfHeight = height * 0.5;
        return new AABB2D(-halfWidth, -halfHeight, halfWidth, halfHeight);
    }

    public static AABB2D FromCenterAndSize(in Vector2 center, in Vector2 size)
    {
        var halfSize = size * 0.5;
        return new AABB2D(center - halfSize, center + halfSize);
    }

    public static AABB2D FromCenterAndSize(in Vector2 center, in SizeD size) => FromCenterAndSize(center, size.ToVector2());
    public static AABB2D FromCenterAndSize(in Vector2 center, in Size size) => FromCenterAndSize(center, size.ToVector2());

    public static AABB2D FromCenterAndSize(double centerX, double centerY, double width, double height)
    {
        var halfWidth = width * 0.5;
        var halfHeight = height * 0.5;
        return new AABB2D(centerX - halfWidth, centerY - halfHeight, centerX + halfWidth, centerY + halfHeight);
    }

    public static AABB2D FromPoints(ReadOnlySpan<Vector2> points)
    {
        if (points.Length == 0)
        {
            return default;
        }

        var min = points[0];
        var max = points[0];

        for (var i = 1; i < points.Length; i++)
        {
            min = Vector2.Min(min, points[i]);
            max = Vector2.Max(max, points[i]);
        }

        return new AABB2D(min, max);
    }

    public static AABB2D FromAABBs(ReadOnlySpan<AABB2D> aabbs)
    {
        if (aabbs.Length == 0)
        {
            return default;
        }

        var min = aabbs[0].Min;
        var max = aabbs[0].Max;

        for (var i = 1; i < aabbs.Length; i++)
        {
            min = Vector2.Min(min, aabbs[i].Min);
            max = Vector2.Max(max, aabbs[i].Max);
        }

        return new AABB2D(min, max);
    }

    public Vector2 Min { get; }
    public Vector2 Max { get; }

    public Vector2 Center => Min.Midpoint(Max);
    public Vector2 Size => Max - Min;
    public double Width => Max.X - Min.X;
    public double Height => Max.Y - Min.Y;

    public bool Contains(in Vector2 point) => Min.X <= point.X && point.X <= Max.X && Min.Y <= point.Y && point.Y <= Max.Y;
    public bool Contains(in AABB2D other) => Min.X <= other.Min.X && Max.X >= other.Max.X && Min.Y <= other.Min.Y && Max.Y >= other.Max.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Overlaps(in AABB2D other) => Max.X >= other.Min.X && Min.X <= other.Max.X && Max.Y >= other.Min.Y && Min.Y <= other.Max.Y;

    public AxisAlignedRectangle ToAxisAlignedRectangle() => new(Center, Size);

    // TODO: Add test.
    public Rectangle ToRectangle() => new(Center, Size);
}