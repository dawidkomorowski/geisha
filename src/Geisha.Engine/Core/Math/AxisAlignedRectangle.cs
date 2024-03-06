using System;

namespace Geisha.Engine.Core.Math
{
    /// <summary>
    ///     Represents 2D axis aligned rectangle.
    /// </summary>
    /// <remarks>
    ///     <see cref="AxisAlignedRectangle" /> represents 2D rectangle with edges pairwise parallel to X and Y axes of
    ///     coordinate system. It is suited to represent raw rectangle that is not rotatable. It can be used as a bounding
    ///     shape for other geometry to perform fast intersection tests.
    /// </remarks>
    public readonly struct AxisAlignedRectangle : IEquatable<AxisAlignedRectangle>
    {
        /// <summary>
        ///     Creates new instance of <see cref="AxisAlignedRectangle" /> with specified dimensions and center at point (0,0).
        /// </summary>
        /// <param name="dimensions">Dimensions, width and height, of rectangle.</param>
        public AxisAlignedRectangle(in Vector2 dimensions) : this(Vector2.Zero, dimensions)
        {
        }

        /// <summary>
        ///     Creates new instance of <see cref="AxisAlignedRectangle" /> with specified dimensions and center at point (0,0).
        /// </summary>
        /// <param name="width">Width of rectangle.</param>
        /// <param name="height">Height of rectangle.</param>
        public AxisAlignedRectangle(double width, double height) : this(0, 0, width, height)
        {
        }

        /// <summary>
        ///     Creates new instance of <see cref="AxisAlignedRectangle" /> with specified position of center and dimensions.
        /// </summary>
        /// <param name="center">Position of rectangle center.</param>
        /// <param name="dimensions">Dimensions, width and height, of rectangle.</param>
        public AxisAlignedRectangle(in Vector2 center, in Vector2 dimensions)
        {
            Center = center;
            Dimensions = dimensions;
        }


        /// <summary>
        ///     Creates new instance of <see cref="AxisAlignedRectangle" /> with specified position of center and dimensions.
        /// </summary>
        /// <param name="centerX">X component of rectangle center.</param>
        /// <param name="centerY">Y component of rectangle center.</param>
        /// <param name="width">Width of rectangle.</param>
        /// <param name="height">Height of rectangle.</param>
        public AxisAlignedRectangle(double centerX, double centerY, double width, double height)
        {
            Center = new Vector2(centerX, centerY);
            Dimensions = new Vector2(width, height);
        }

        /// <summary>
        ///     Creates new instance of <see cref="AxisAlignedRectangle" /> for given set of points. Created
        ///     <see cref="AxisAlignedRectangle" /> is minimal rectangle that encloses all given points.
        /// </summary>
        /// <param name="points">Points to be enclosed by <see cref="AxisAlignedRectangle" />.</param>
        public AxisAlignedRectangle(ReadOnlySpan<Vector2> points)
        {
            if (points.Length == 0)
            {
                Center = Vector2.Zero;
                Dimensions = Vector2.Zero;

                return;
            }

            var max = points[0];
            var min = points[0];

            for (var i = 1; i < points.Length; i++)
            {
                max = Vector2.Max(max, points[i]);
                min = Vector2.Min(min, points[i]);
            }

            Center = (max + min) / 2;
            Dimensions = max - min;
        }

        /// <summary>
        ///     Creates new instance of <see cref="AxisAlignedRectangle" /> for given set of rectangles. Created
        ///     <see cref="AxisAlignedRectangle" /> is minimal rectangle that encloses all given rectangles.
        /// </summary>
        /// <param name="rectangles">Rectangles to be enclosed by <see cref="AxisAlignedRectangle" />.</param>
        public AxisAlignedRectangle(ReadOnlySpan<AxisAlignedRectangle> rectangles)
        {
            if (rectangles.Length == 0)
            {
                Center = Vector2.Zero;
                Dimensions = Vector2.Zero;

                return;
            }

            var max = rectangles[0].Max;
            var min = rectangles[0].Min;

            for (var i = 1; i < rectangles.Length; i++)
            {
                max = Vector2.Max(max, rectangles[i].Max);
                min = Vector2.Min(min, rectangles[i].Min);
            }

            Center = (max + min) / 2;
            Dimensions = max - min;
        }

        /// <summary>
        ///     Center of rectangle.
        /// </summary>
        public Vector2 Center { get; }

        /// <summary>
        ///     Dimensions, width and height, of rectangle.
        /// </summary>
        public Vector2 Dimensions { get; }

        /// <summary>
        ///     Width of rectangle.
        /// </summary>
        public double Width => Dimensions.X;

        /// <summary>
        ///     Height of rectangle.
        /// </summary>
        public double Height => Dimensions.Y;

        /// <summary>
        ///     Gets <see cref="Vector2" /> representing maximal X and Y extents of <see cref="AxisAlignedRectangle" />.
        /// </summary>
        public Vector2 Max => new(Center.X + Width / 2, Center.Y + Height / 2);

        /// <summary>
        ///     Gets <see cref="Vector2" /> representing minimal X and Y extents of <see cref="AxisAlignedRectangle" />.
        /// </summary>
        public Vector2 Min => new(Center.X - Width / 2, Center.Y - Height / 2);

        /// <summary>
        ///     Upper-left vertex of rectangle.
        /// </summary>
        public Vector2 UpperLeft => new(Center.X - Width / 2, Center.Y + Height / 2);

        /// <summary>
        ///     Upper-right vertex of rectangle.
        /// </summary>
        public Vector2 UpperRight => new(Center.X + Width / 2, Center.Y + Height / 2);

        /// <summary>
        ///     Lower-left vertex of rectangle.
        /// </summary>
        public Vector2 LowerLeft => new(Center.X - Width / 2, Center.Y - Height / 2);

        /// <summary>
        ///     Lower-right vertex of rectangle.
        /// </summary>
        public Vector2 LowerRight => new(Center.X + Width / 2, Center.Y - Height / 2);


        /// <summary>
        ///     Checks if <see cref="AxisAlignedRectangle" /> contains given <paramref name="point" />.
        /// </summary>
        /// <param name="point">Point to be checked.</param>
        /// <returns>True, if <see cref="AxisAlignedRectangle" /> contains a point, false otherwise.</returns>
        /// <remarks>
        ///     Point is considered to be contained in <see cref="AxisAlignedRectangle" /> when it is strictly located inside
        ///     <see cref="AxisAlignedRectangle" /> or it is a part of it's edge.
        /// </remarks>
        public bool Contains(in Vector2 point) => Min.X <= point.X && point.X <= Max.X && Min.Y <= point.Y && point.Y <= Max.Y;

        /// <summary>
        ///     Checks if <see cref="AxisAlignedRectangle" /> contains other <see cref="AxisAlignedRectangle" />.
        /// </summary>
        /// <param name="other"><see cref="AxisAlignedRectangle" /> to be checked.</param>
        /// <returns>
        ///     True, if <see cref="AxisAlignedRectangle" /> contains other <see cref="AxisAlignedRectangle" />, false
        ///     otherwise.
        /// </returns>
        /// <remarks>
        ///     <see cref="AxisAlignedRectangle" /> is considered to be contained in other <see cref="AxisAlignedRectangle" />
        ///     when all its vertices are contained in other <see cref="AxisAlignedRectangle" />.
        /// </remarks>
        public bool Contains(in AxisAlignedRectangle other) => Min.X <= other.Min.X && Min.Y <= other.Min.Y && Max.X >= other.Max.X && Max.Y >= other.Max.Y;

        /// <summary>
        ///     Checks if <see cref="AxisAlignedRectangle" /> overlaps other <see cref="AxisAlignedRectangle" />.
        /// </summary>
        /// <param name="other"><see cref="AxisAlignedRectangle" /> to be checked.</param>
        /// <returns>
        ///     True, if <see cref="AxisAlignedRectangle" /> overlaps other <see cref="AxisAlignedRectangle" />, false
        ///     otherwise.
        /// </returns>
        /// <remarks>
        ///     Two axis aligned rectangles are considered overlapping when they strictly intersect each other or they have a
        ///     common edge
        ///     points or vertices.
        /// </remarks>
        public bool Overlaps(in AxisAlignedRectangle other) => !(Max.X < other.Min.X || Max.Y < other.Min.Y || Min.X > other.Max.X || Min.Y > other.Max.Y);

        /// <summary>
        ///     Creates <see cref="Quad" /> representing the same shape as this <see cref="AxisAlignedRectangle" />.
        /// </summary>
        /// <returns><see cref="Quad" /> that has all vertices the same as <see cref="AxisAlignedRectangle" />.</returns>
        public Quad ToQuad() => new(LowerLeft, LowerRight, UpperRight, UpperLeft);

        // TODO Add documentation and tests.
        public Rectangle ToRectangle() => new(Center, Dimensions);

        /// <summary>
        ///     Converts the value of the current <see cref="AxisAlignedRectangle" /> object to its equivalent string
        ///     representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="AxisAlignedRectangle" /> object.</returns>
        public override string ToString() => $"{nameof(Center)}: {Center}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}";

        #region Equality members

        /// <inheritdoc />
        public bool Equals(AxisAlignedRectangle other) => Center.Equals(other.Center) && Width.Equals(other.Width) && Height.Equals(other.Height);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is AxisAlignedRectangle other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Center, Width, Height);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="AxisAlignedRectangle" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="AxisAlignedRectangle" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(in AxisAlignedRectangle left, in AxisAlignedRectangle right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="AxisAlignedRectangle" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="AxisAlignedRectangle" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(in AxisAlignedRectangle left, in AxisAlignedRectangle right) => !left.Equals(right);

        #endregion
    }
}