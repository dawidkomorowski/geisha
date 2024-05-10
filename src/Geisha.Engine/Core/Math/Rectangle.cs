using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Geisha.Engine.Core.Math
{
    /// <summary>
    ///     Represents 2D rectangle.
    /// </summary>
    public readonly struct Rectangle : IEquatable<Rectangle>
    {
        /// <summary>
        ///     Creates new instance of <see cref="Rectangle" /> with given dimensions and center at point (0,0).
        /// </summary>
        /// <param name="dimensions">Dimensions, width and height, of rectangle.</param>
        public Rectangle(in Vector2 dimensions) : this(Vector2.Zero, dimensions)
        {
        }

        /// <summary>
        ///     Creates new instance of <see cref="Rectangle" /> with given dimensions and center at given position.
        /// </summary>
        /// <param name="center">Position of rectangle center.</param>
        /// <param name="dimensions">Dimensions, width and height, of rectangle.</param>
        public Rectangle(in Vector2 center, in Vector2 dimensions)
        {
            UpperLeft = new Vector2(-dimensions.X / 2 + center.X, dimensions.Y / 2 + center.Y);
            UpperRight = new Vector2(dimensions.X / 2 + center.X, dimensions.Y / 2 + center.Y);
            LowerLeft = new Vector2(-dimensions.X / 2 + center.X, -dimensions.Y / 2 + center.Y);
            LowerRight = new Vector2(dimensions.X / 2 + center.X, -dimensions.Y / 2 + center.Y);
        }

        private Rectangle(in Vector2 upperLeft, in Vector2 upperRight, in Vector2 lowerLeft, in Vector2 lowerRight)
        {
            UpperLeft = upperLeft;
            UpperRight = upperRight;
            LowerLeft = lowerLeft;
            LowerRight = lowerRight;
        }

        /// <summary>
        ///     Upper-left vertex of rectangle.
        /// </summary>
        public Vector2 UpperLeft { get; }

        /// <summary>
        ///     Upper-right vertex of rectangle.
        /// </summary>
        public Vector2 UpperRight { get; }

        /// <summary>
        ///     Lower-left vertex of rectangle.
        /// </summary>
        public Vector2 LowerLeft { get; }

        /// <summary>
        ///     Lower-right vertex of rectangle.
        /// </summary>
        public Vector2 LowerRight { get; }

        /// <summary>
        ///     Width of rectangle.
        /// </summary>
        public double Width => UpperRight.Distance(UpperLeft);

        /// <summary>
        ///     Height of rectangle.
        /// </summary>
        public double Height => UpperLeft.Distance(LowerLeft);

        /// <summary>
        ///     Center of rectangle.
        /// </summary>
        public Vector2 Center => new((LowerLeft.X + UpperRight.X) / 2, (LowerLeft.Y + UpperRight.Y) / 2);

        /// <summary>
        ///     Returns <see cref="Rectangle" /> that is this <see cref="Rectangle" /> transformed by given
        ///     <see cref="Matrix3x3" />.
        /// </summary>
        /// <param name="transform">Transformation matrix used to transform rectangle.</param>
        /// <returns><see cref="Rectangle" /> transformed by given matrix.</returns>
        public Rectangle Transform(in Matrix3x3 transform) =>
            new(
                (transform * UpperLeft.Homogeneous).ToVector2(),
                (transform * UpperRight.Homogeneous).ToVector2(),
                (transform * LowerLeft.Homogeneous).ToVector2(),
                (transform * LowerRight.Homogeneous).ToVector2()
            );

        /// <summary>
        ///     Tests whether this <see cref="Rectangle" /> contains a point.
        /// </summary>
        /// <param name="point">Point to be tested for containment in a rectangle.</param>
        /// <returns>True, if rectangle contains a point, false otherwise.</returns>
        public bool Contains(in Vector2 point)
        {
            Span<Vector2> vertices = stackalloc Vector2[4];
            WriteVertices(vertices);

            Span<Axis> axes = stackalloc Axis[2];
            axes[0] = new Axis((UpperLeft - LowerLeft).Normal);
            axes[1] = new Axis((UpperRight - UpperLeft).Normal);

            return SeparatingAxisTheorem.PolygonContains(vertices, point, axes);
        }

        /// <summary>
        ///     Tests whether this <see cref="Rectangle" /> is overlapping other <see cref="Rectangle" />.
        /// </summary>
        /// <param name="other"><see cref="Rectangle" /> to test for overlap.</param>
        /// <returns>True, if rectangles overlap, false otherwise.</returns>
        public bool Overlaps(in Rectangle other)
        {
            Span<Vector2> rectangle1 = stackalloc Vector2[4];
            WriteVertices(rectangle1);

            Span<Vector2> rectangle2 = stackalloc Vector2[4];
            other.WriteVertices(rectangle2);

            Span<Axis> axes = stackalloc Axis[4];
            axes[0] = new Axis((UpperLeft - LowerLeft).Normal);
            axes[1] = new Axis((UpperRight - UpperLeft).Normal);
            axes[2] = new Axis((other.UpperLeft - other.LowerLeft).Normal);
            axes[3] = new Axis((other.UpperRight - other.UpperLeft).Normal);

            return SeparatingAxisTheorem.PolygonsOverlap(rectangle1, rectangle2, axes);
        }

        /// <summary>
        ///     Tests whether this <see cref="Rectangle" /> is overlapping other <see cref="Rectangle" /> and provides
        ///     <see cref="MinimumTranslationVector" /> for this <see cref="Rectangle" />.
        /// </summary>
        /// <param name="other"><see cref="Rectangle" /> to test for overlap.</param>
        /// <param name="mtv">
        ///     <see cref="MinimumTranslationVector" /> for this <see cref="Rectangle" />. Value is <c>default</c> when
        ///     return value is <c>false</c>.
        /// </param>
        /// <returns>True, if rectangles overlap, false otherwise.</returns>
        public bool Overlaps(in Rectangle other, out MinimumTranslationVector mtv)
        {
            Span<Vector2> rectangle1 = stackalloc Vector2[4];
            WriteVertices(rectangle1);

            Span<Vector2> rectangle2 = stackalloc Vector2[4];
            other.WriteVertices(rectangle2);

            return SeparatingAxisTheorem.PolygonsOverlap(rectangle1, rectangle2, out mtv);
        }

        /// <summary>
        ///     Tests whether this <see cref="Rectangle" /> is overlapping specified <see cref="Circle" />.
        /// </summary>
        /// <param name="circle"><see cref="Circle" /> to test for overlap.</param>
        /// <returns>True, if rectangle and circle overlaps, false otherwise.</returns>
        public bool Overlaps(in Circle circle)
        {
            Span<Vector2> polygon = stackalloc Vector2[4];
            WriteVertices(polygon);
            return SeparatingAxisTheorem.PolygonAndCircleOverlap(polygon, circle);
        }

        // TODO Add documentation.
        // TODO Add tests.
        // TODO Test cases when rectangle vertex is the same as circle center.
        public bool Overlaps(in Circle circle, out MinimumTranslationVector mtv)
        {
            Span<Vector2> polygon = stackalloc Vector2[4];
            WriteVertices(polygon);
            return SeparatingAxisTheorem.PolygonAndCircleOverlap(polygon, circle, out mtv);
        }

        /// <summary>
        ///     Gets <see cref="AxisAlignedRectangle" /> that encloses this <see cref="Rectangle" />.
        /// </summary>
        /// <returns><see cref="AxisAlignedRectangle" /> that encloses this <see cref="Rectangle" />.</returns>
        public AxisAlignedRectangle GetBoundingRectangle()
        {
            Span<Vector2> vertices = stackalloc Vector2[4];
            WriteVertices(vertices);
            return new AxisAlignedRectangle(vertices);
        }

        /// <summary>
        ///     Writes vertices of <see cref="Rectangle" /> into span in counterclockwise orientation.
        /// </summary>
        /// <param name="vertices">Target span for writing vertices. It must be of size 4.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVertices(Span<Vector2> vertices)
        {
            Debug.Assert(vertices.Length == 4, "vertices.Length == 4");

            vertices[0] = LowerLeft;
            vertices[1] = LowerRight;
            vertices[2] = UpperRight;
            vertices[3] = UpperLeft;
        }

        /// <summary>
        ///     Converts the value of the current <see cref="Rectangle" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Rectangle" /> object.</returns>
        public override string ToString() =>
            $"{nameof(Center)}: {Center}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(UpperLeft)}: {UpperLeft}, {nameof(UpperRight)}: {UpperRight}, {nameof(LowerLeft)}: {LowerLeft}, {nameof(LowerRight)}: {LowerRight}";

        #region Equality members

        /// <inheritdoc />
        public bool Equals(Rectangle other) => UpperLeft.Equals(other.UpperLeft) && UpperRight.Equals(other.UpperRight) && LowerLeft.Equals(other.LowerLeft) &&
                                               LowerRight.Equals(other.LowerRight);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Rectangle other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(UpperLeft, UpperRight, LowerLeft, LowerRight);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Rectangle" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="Rectangle" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(in Rectangle left, in Rectangle right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Rectangle" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="Rectangle" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(in Rectangle left, in Rectangle right) => !left.Equals(right);

        #endregion
    }
}