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

        public AxisAlignedRectangle(Span<Vector2> points)
        {
            throw new NotImplementedException();
            // TODO Introduce Vector2.Max() and Vector2.Min()
        }

        public AxisAlignedRectangle(Span<AxisAlignedRectangle> rectangles)
        {
            throw new NotImplementedException();
            // TODO Introduce GetBoundingRectangle() for other shapes?
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

        public Vector2 Max => throw new NotImplementedException();
        public Vector2 Min => throw new NotImplementedException();

        /// <summary>
        ///     Upper-left vertex of rectangle.
        /// </summary>
        public Vector2 UpperLeft => throw new NotImplementedException();

        /// <summary>
        ///     Upper-right vertex of rectangle.
        /// </summary>
        public Vector2 UpperRight => throw new NotImplementedException();

        /// <summary>
        ///     Lower-left vertex of rectangle.
        /// </summary>
        public Vector2 LowerLeft => throw new NotImplementedException();

        /// <summary>
        ///     Lower-right vertex of rectangle.
        /// </summary>
        public Vector2 LowerRight => throw new NotImplementedException();

        public bool Contains(in Vector2 point)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(in AxisAlignedRectangle other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Converts the value of the current <see cref="AxisAlignedRectangle" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="AxisAlignedRectangle" /> object.</returns>
        public override string ToString() =>
            $"{nameof(Center)}: {Center}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(UpperLeft)}: {UpperLeft}, {nameof(UpperRight)}: {UpperRight}, {nameof(LowerLeft)}: {LowerLeft}, {nameof(LowerRight)}: {LowerRight}";

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