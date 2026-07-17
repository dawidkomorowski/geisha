using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Geisha.Engine.Core.Math;

/// <summary>
///     Represents a 2D axis-aligned bounding box.
/// </summary>
/// <remarks>
///     <para>
///         Geisha Engine math types use a coordinate system where X increases to the right and Y increases upward. In
///         this orientation, <see cref="Min" /> is the bottom-left corner and <see cref="Max" /> is the top-right corner.
///     </para>
///     <para>The bounding box includes its boundaries for containment and overlap checks.</para>
/// </remarks>
// ReSharper disable once InconsistentNaming
public readonly record struct AABB2D
{
    /// <summary>
    ///     Creates a bounding box from minimum and maximum coordinates.
    /// </summary>
    /// <param name="min">The minimum corner.</param>
    /// <param name="max">The maximum corner.</param>
    public AABB2D(in Vector2 min, in Vector2 max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>
    ///     Creates a bounding box from minimum and maximum coordinates.
    /// </summary>
    /// <param name="minX">The minimum X coordinate.</param>
    /// <param name="minY">The minimum Y coordinate.</param>
    /// <param name="maxX">The maximum X coordinate.</param>
    /// <param name="maxY">The maximum Y coordinate.</param>
    public AABB2D(double minX, double minY, double maxX, double maxY) : this(new Vector2(minX, minY), new Vector2(maxX, maxY))
    {
    }

    /// <summary>
    ///     Creates a bounding box centered at the origin from the specified size.
    /// </summary>
    /// <param name="size">The size of the bounding box.</param>
    /// <returns>A bounding box centered at <see cref="Vector2.Zero" />.</returns>
    public static AABB2D FromSize(in Vector2 size)
    {
        var halfSize = size * 0.5;
        return new AABB2D(-halfSize, halfSize);
    }

    /// <summary>
    ///     Creates a bounding box centered at the origin from the specified size.
    /// </summary>
    /// <param name="size">The size of the bounding box.</param>
    /// <returns>A bounding box centered at <see cref="Vector2.Zero" />.</returns>
    public static AABB2D FromSize(in SizeD size) => FromSize(size.ToVector2());

    /// <summary>
    ///     Creates a bounding box centered at the origin from the specified size.
    /// </summary>
    /// <param name="size">The size of the bounding box.</param>
    /// <returns>A bounding box centered at <see cref="Vector2.Zero" />.</returns>
    public static AABB2D FromSize(in Size size) => FromSize(size.ToVector2());

    /// <summary>
    ///     Creates a bounding box centered at the origin from the specified width and height.
    /// </summary>
    /// <param name="width">The width of the bounding box.</param>
    /// <param name="height">The height of the bounding box.</param>
    /// <returns>A bounding box centered at the origin.</returns>
    public static AABB2D FromSize(double width, double height)
    {
        var halfWidth = width * 0.5;
        var halfHeight = height * 0.5;
        return new AABB2D(-halfWidth, -halfHeight, halfWidth, halfHeight);
    }

    /// <summary>
    ///     Creates a bounding box from center and size.
    /// </summary>
    /// <param name="center">The center point of the bounding box.</param>
    /// <param name="size">The size of the bounding box.</param>
    /// <returns>A bounding box with the specified center and size.</returns>
    public static AABB2D FromCenterAndSize(in Vector2 center, in Vector2 size)
    {
        var halfSize = size * 0.5;
        return new AABB2D(center - halfSize, center + halfSize);
    }

    /// <summary>
    ///     Creates a bounding box from center and size.
    /// </summary>
    /// <param name="center">The center point of the bounding box.</param>
    /// <param name="size">The size of the bounding box.</param>
    /// <returns>A bounding box with the specified center and size.</returns>
    public static AABB2D FromCenterAndSize(in Vector2 center, in SizeD size) => FromCenterAndSize(center, size.ToVector2());

    /// <summary>
    ///     Creates a bounding box from center and size.
    /// </summary>
    /// <param name="center">The center point of the bounding box.</param>
    /// <param name="size">The size of the bounding box.</param>
    /// <returns>A bounding box with the specified center and size.</returns>
    public static AABB2D FromCenterAndSize(in Vector2 center, in Size size) => FromCenterAndSize(center, size.ToVector2());

    /// <summary>
    ///     Creates a bounding box from center coordinates and size.
    /// </summary>
    /// <param name="centerX">The X coordinate of the center point.</param>
    /// <param name="centerY">The Y coordinate of the center point.</param>
    /// <param name="width">The width of the bounding box.</param>
    /// <param name="height">The height of the bounding box.</param>
    /// <returns>A bounding box with the specified center and size.</returns>
    public static AABB2D FromCenterAndSize(double centerX, double centerY, double width, double height)
    {
        var halfWidth = width * 0.5;
        var halfHeight = height * 0.5;
        return new AABB2D(centerX - halfWidth, centerY - halfHeight, centerX + halfWidth, centerY + halfHeight);
    }

    /// <summary>
    ///     Creates a bounding box that contains all specified points.
    /// </summary>
    /// <param name="points">The points to include.</param>
    /// <returns>
    ///     A bounding box that contains all points, or <see langword="default" /> when <paramref name="points" /> is empty.
    /// </returns>
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

    /// <summary>
    ///     Creates a bounding box that contains all specified bounding boxes.
    /// </summary>
    /// <param name="aabbs">The bounding boxes to include.</param>
    /// <returns>
    ///     A bounding box that contains all specified bounding boxes, or <see langword="default" /> when
    ///     <paramref name="aabbs" /> is empty.
    /// </returns>
    // ReSharper disable once InconsistentNaming
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

    /// <summary>
    ///     Gets the minimum corner of the bounding box, which in Geisha coordinates is the bottom-left corner.
    /// </summary>
    public Vector2 Min { get; }

    /// <summary>
    ///     Gets the maximum corner of the bounding box, which in Geisha coordinates is the top-right corner.
    /// </summary>
    public Vector2 Max { get; }

    /// <summary>
    ///     Gets the center point of the bounding box.
    /// </summary>
    public Vector2 Center => Min.Midpoint(Max);

    /// <summary>
    ///     Gets the size of the bounding box.
    /// </summary>
    public Vector2 Size => Max - Min;

    /// <summary>
    ///     Gets the width of the bounding box.
    /// </summary>
    public double Width => Max.X - Min.X;

    /// <summary>
    ///     Gets the height of the bounding box.
    /// </summary>
    public double Height => Max.Y - Min.Y;

    // TODO: Add documentation.
    public bool IsValid => Min.X <= Max.X && Min.Y <= Max.Y;

    /// <summary>
    ///     Determines whether the specified point is inside this bounding box.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns><see langword="true" /> if the point is inside or on the boundary; otherwise, <see langword="false" />.</returns>
    public bool Contains(in Vector2 point) => Min.X <= point.X && point.X <= Max.X && Min.Y <= point.Y && point.Y <= Max.Y;

    /// <summary>
    ///     Determines whether this bounding box fully contains another bounding box.
    /// </summary>
    /// <param name="other">The bounding box to test.</param>
    /// <returns><see langword="true" /> if <paramref name="other" /> is fully contained; otherwise, <see langword="false" />.</returns>
    public bool Contains(in AABB2D other) => Min.X <= other.Min.X && Max.X >= other.Max.X && Min.Y <= other.Min.Y && Max.Y >= other.Max.Y;

    /// <summary>
    ///     Determines whether this bounding box overlaps another bounding box.
    /// </summary>
    /// <param name="other">The bounding box to test.</param>
    /// <returns>
    ///     <see langword="true" /> if the bounding boxes overlap or touch at the boundary; otherwise, <see langword="false" />
    ///     .
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Overlaps(in AABB2D other) => Max.X >= other.Min.X && Min.X <= other.Max.X && Max.Y >= other.Min.Y && Min.Y <= other.Max.Y;

    // TODO: Add documentation.
    public AABB2D Intersect(in AABB2D other) => new(Vector2.Max(Min, other.Min), Vector2.Min(Max, other.Max));

    /// <summary>
    ///     Converts this bounding box to an axis-aligned rectangle.
    /// </summary>
    /// <returns>An <see cref="AxisAlignedRectangle" /> with the same center and size.</returns>
    public AxisAlignedRectangle ToAxisAlignedRectangle() => new(Center, Size);

    /// <summary>
    ///     Converts this bounding box to a rectangle.
    /// </summary>
    /// <returns>A <see cref="Rectangle" /> with the same center and size.</returns>
    public Rectangle ToRectangle() => new(Center, Size);
}