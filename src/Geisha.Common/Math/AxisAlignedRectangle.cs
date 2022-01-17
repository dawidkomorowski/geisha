using System;

namespace Geisha.Common.Math
{
    /// <summary>
    ///     Represents 2D axis aligned rectangle.
    /// </summary>
    public readonly struct AxisAlignedRectangle : IEquatable<AxisAlignedRectangle>
    {
        /// <summary>
        ///     Creates new instance of <see cref="AxisAlignedRectangle" /> with given dimensions and center at point (0,0).
        /// </summary>
        /// <param name="dimensions">Dimensions, width and height, of rectangle.</param>
        public AxisAlignedRectangle(in Vector2 dimensions) : this(Vector2.Zero, dimensions)
        {
        }

        /// <summary>
        ///     Creates new instance of <see cref="AxisAlignedRectangle" /> with given dimensions and center at given position.
        /// </summary>
        /// <param name="center">Position of rectangle center.</param>
        /// <param name="dimensions">Dimensions, width and height, of rectangle.</param>
        public AxisAlignedRectangle(in Vector2 center, in Vector2 dimensions)
        {
            Center = center;
            Width = dimensions.X;
            Height = dimensions.Y;
        }

        public AxisAlignedRectangle(ReadOnlySpan<Vector2> points)
        {
            if (points.Length == 0)
            {
                Center = Vector2.Zero;
                Width = 0;
                Height = 0;

                return;
            }

            var max = points[0];
            var min = points[0];

            for (var i = 1; i < points.Length; i++)
            {
                max = Vector2.Max(max, points[i]);
                min = Vector2.Min(min, points[i]);
            }

            var dimensions = max - min;

            Center = (max + min) / 2;
            Width = dimensions.X;
            Height = dimensions.Y;
        }

        // TODO Introduce GetBoundingRectangle() for other shapes?
        public AxisAlignedRectangle(ReadOnlySpan<AxisAlignedRectangle> rectangles)
        {
            if (rectangles.Length == 0)
            {
                Center = Vector2.Zero;
                Width = 0;
                Height = 0;

                return;
            }

            var max = rectangles[0].Max;
            var min = rectangles[0].Min;

            for (var i = 1; i < rectangles.Length; i++)
            {
                max = Vector2.Max(max, rectangles[i].Max);
                min = Vector2.Min(min, rectangles[i].Min);
            }

            var dimensions = max - min;

            Center = (max + min) / 2;
            Width = dimensions.X;
            Height = dimensions.Y;
        }

        /// <summary>
        ///     Center of rectangle.
        /// </summary>
        public Vector2 Center { get; }

        /// <summary>
        ///     Width of rectangle.
        /// </summary>
        public double Width { get; }

        /// <summary>
        ///     Height of rectangle.
        /// </summary>
        public double Height { get; }

        public Vector2 Max => new Vector2(Center.X + Width / 2, Center.Y + Height / 2);
        public Vector2 Min => new Vector2(Center.X - Width / 2, Center.Y - Height / 2);

        /// <summary>
        ///     Upper-left vertex of rectangle.
        /// </summary>
        public Vector2 UpperLeft => new Vector2(Center.X - Width / 2, Center.Y + Height / 2);

        /// <summary>
        ///     Upper-right vertex of rectangle.
        /// </summary>
        public Vector2 UpperRight => new Vector2(Center.X + Width / 2, Center.Y + Height / 2);

        /// <summary>
        ///     Lower-left vertex of rectangle.
        /// </summary>
        public Vector2 LowerLeft => new Vector2(Center.X - Width / 2, Center.Y - Height / 2);

        /// <summary>
        ///     Lower-right vertex of rectangle.
        /// </summary>
        public Vector2 LowerRight => new Vector2(Center.X + Width / 2, Center.Y - Height / 2);

        public bool Contains(in Vector2 point) => throw new NotImplementedException();

        public bool Overlaps(in AxisAlignedRectangle other) => throw new NotImplementedException();

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