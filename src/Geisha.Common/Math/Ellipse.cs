using System;

namespace Geisha.Common.Math
{
    // TODO Rename to AxisAlignedEllipse as it does not support orientation?
    /// <summary>
    ///     Represents 2D ellipse.
    /// </summary>
    public readonly struct Ellipse : IEquatable<Ellipse>
    {
        /// <summary>
        ///     Creates new instance of <see cref="Ellipse" /> with given <paramref name="radiusX" />, <paramref name="radiusY" />
        ///     and center at point (0,0).
        /// </summary>
        /// <param name="radiusX">Length of the ellipse X radius.</param>
        /// <param name="radiusY">Length of the ellipse Y radius.</param>
        public Ellipse(double radiusX, double radiusY)
        {
            Center = Vector2.Zero;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        ///     Creates new instance of <see cref="Ellipse" /> with given <paramref name="radiusX" />, <paramref name="radiusY" />
        ///     and center at given position.
        /// </summary>
        /// <param name="center">Position of the ellipse center.</param>
        /// <param name="radiusX">Length of the ellipse X radius.</param>
        /// <param name="radiusY">Length of the ellipse Y radius.</param>
        public Ellipse(Vector2 center, double radiusX, double radiusY)
        {
            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        ///     Center of ellipse.
        /// </summary>
        public Vector2 Center { get; }

        /// <summary>
        ///     X radius of the ellipse. It is the ellipse radius alongside X axis.
        /// </summary>
        public double RadiusX { get; }

        /// <summary>
        ///     Y radius of the ellipse. It is the ellipse radius alongside Y axis.
        /// </summary>
        public double RadiusY { get; }

        /// <summary>
        ///     Gets <see cref="AxisAlignedRectangle" /> that encloses this <see cref="Ellipse" />.
        /// </summary>
        /// <returns><see cref="AxisAlignedRectangle" /> that encloses this <see cref="Ellipse" />.</returns>
        public AxisAlignedRectangle GetBoundingRectangle() => new AxisAlignedRectangle(Center, new Vector2(RadiusX * 2, RadiusY * 2));

        /// <summary>
        ///     Converts the value of the current <see cref="Ellipse" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Ellipse" /> object.</returns>
        public override string ToString() => $"{nameof(Center)}: {Center}, {nameof(RadiusX)}: {RadiusX}, {nameof(RadiusY)}: {RadiusY}";

        #region Equality members

        /// <inheritdoc />
        public bool Equals(Ellipse other) => Center.Equals(other.Center) && RadiusX.Equals(other.RadiusX) && RadiusY.Equals(other.RadiusY);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Ellipse other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Center, RadiusX, RadiusY);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Ellipse" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="Ellipse" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(in Ellipse left, in Ellipse right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Ellipse" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="Ellipse" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(in Ellipse left, in Ellipse right) => !left.Equals(right);

        #endregion
    }
}