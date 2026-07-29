using System;
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
///     <para>
///         A bounding box is not guaranteed to be well-formed. Constructors and factory methods do not validate or
///         normalize the corners, so a box built from inverted coordinates, or returned by <see cref="Intersect" /> for
///         non-overlapping boxes, can be invalid. See <see cref="IsValid" />.
///     </para>
///     <para>
///         Containment and overlap queries assume well-formed operands. Results are unspecified when a bounding box
///         involved in the query is invalid, with one exception: <see cref="Intersect" /> propagates invalidity, so an
///         invalid operand always produces an invalid result.
///     </para>
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
    ///     A bounding box that contains all points, or an invalid bounding box when <paramref name="points" /> is empty.
    /// </returns>
    public static AABB2D FromPoints(ReadOnlySpan<Vector2> points)
    {
        var min = new Vector2(double.PositiveInfinity, double.PositiveInfinity);
        var max = new Vector2(double.NegativeInfinity, double.NegativeInfinity);

        foreach (var point in points)
        {
            min = Vector2.Min(min, point);
            max = Vector2.Max(max, point);
        }

        return new AABB2D(min, max);
    }

    /// <summary>
    ///     Creates a bounding box that contains all specified bounding boxes.
    /// </summary>
    /// <param name="aabbs">The bounding boxes to include.</param>
    /// <returns>
    ///     A bounding box that contains all specified bounding boxes, or an invalid bounding box when
    ///     <paramref name="aabbs" /> is empty.
    /// </returns>
    // ReSharper disable once InconsistentNaming
    public static AABB2D FromAABBs(ReadOnlySpan<AABB2D> aabbs)
    {
        var min = new Vector2(double.PositiveInfinity, double.PositiveInfinity);
        var max = new Vector2(double.NegativeInfinity, double.NegativeInfinity);

        foreach (var aabb in aabbs)
        {
            min = Vector2.Min(min, aabb.Min);
            max = Vector2.Max(max, aabb.Max);
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

    /// <summary>
    ///     Gets a value indicating whether the bounding box is well-formed, that is, whether each component of
    ///     <see cref="Min" /> is less than or equal to the corresponding component of <see cref="Max" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A degenerate bounding box is valid. A box that collapses to a line or to a single point still satisfies the
    ///         condition, because the boundaries are included.
    ///     </para>
    ///     <para>
    ///         Use this property to test the result of <see cref="Intersect" />, which returns an invalid bounding box when
    ///         the two boxes do not overlap.
    ///     </para>
    /// </remarks>
    /// <seealso cref="Intersect" />
    public bool IsValid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Min.X <= Max.X && Min.Y <= Max.Y;
    }

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

    /// <summary>
    ///     Computes the intersection of this bounding box and another bounding box.
    /// </summary>
    /// <param name="other">The bounding box to intersect with.</param>
    /// <returns>
    ///     The bounding box covering the common area of both boxes, or an invalid bounding box when they do not overlap.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         The result is not guaranteed to be a well-formed bounding box. When the boxes are separated along either
    ///         axis, the returned box has <see cref="Min" /> greater than <see cref="Max" /> on that axis. Check
    ///         <see cref="IsValid" /> on the result, or call <see cref="Overlaps" /> first, before using the returned box.
    ///     </para>
    ///     <para>
    ///         Boxes that only touch at an edge or a corner are treated as overlapping and produce a valid degenerate
    ///         result, because the boundaries are included.
    ///     </para>
    ///     <para>
    ///         Invalidity propagates. When either box is invalid on an axis, the result is invalid on that axis, so chained
    ///         intersections cannot recover a valid result once one becomes invalid.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IsValid" />
    /// <seealso cref="Overlaps" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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