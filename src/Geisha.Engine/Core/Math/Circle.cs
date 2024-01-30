using System;

namespace Geisha.Engine.Core.Math
{
    /// <summary>
    ///     Represents 2D circle.
    /// </summary>
    public readonly struct Circle : IEquatable<Circle>
    {
        /// <summary>
        ///     Creates new instance of <see cref="Circle" /> with given radius and center at point (0,0).
        /// </summary>
        /// <param name="radius">Length of circle radius.</param>
        public Circle(double radius) : this(Vector2.Zero, radius)
        {
        }

        /// <summary>
        ///     Creates new instance of <see cref="Circle" /> with given radius and center at given position.
        /// </summary>
        /// <param name="center">Position of circle center.</param>
        /// <param name="radius">Length of circle radius.</param>
        public Circle(in Vector2 center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        ///     Center of circle.
        /// </summary>
        public Vector2 Center { get; }

        /// <summary>
        ///     Radius of circle.
        /// </summary>
        public double Radius { get; }

        /// <summary>
        ///     Returns <see cref="Circle" /> that is this <see cref="Circle" /> transformed by given <see cref="Matrix3x3" />.
        /// </summary>
        /// <param name="transform">Transformation matrix used to transform circle.</param>
        /// <returns><see cref="Circle" /> transformed by given matrix.</returns>
        /// <remarks>
        ///     This method does not support transformation with nonuniform scaling along x-axis and y-axis.
        /// </remarks>
        public Circle Transform(in Matrix3x3 transform)
        {
            var center = (transform * Center.Homogeneous).ToVector2();
            var radialPoint = (transform * (Center + Vector2.UnitX * Radius).Homogeneous).ToVector2();
            return new Circle(center, center.Distance(radialPoint));
        }

        /// <summary>
        ///     Tests whether this <see cref="Circle" /> contains a point.
        /// </summary>
        /// <param name="point">Point to be tested for containment in a circle.</param>
        /// <returns>True, if circle contains a point, false otherwise.</returns>
        public bool Contains(in Vector2 point) => Center.Distance(point) <= Radius;

        /// <summary>
        ///     Tests whether this <see cref="Circle" /> is overlapping other <see cref="Circle" />.
        /// </summary>
        /// <param name="other"><see cref="Circle" /> to test for overlapping.</param>
        /// <returns>True, if circles overlap, false otherwise.</returns>
        public bool Overlaps(in Circle other) => Center.Distance(other.Center) <= Radius + other.Radius;

        // TODO Add documentation.
        // TODO Add tests.
        public bool Overlaps(in Circle other, out SeparationInfo separationInfo)
        {
            var translation = other.Center - Center;
            var distance = translation.Length;
            var radii = Radius + other.Radius;

            // TODO
            separationInfo = new SeparationInfo(translation.Unit, )
        }

        /// <summary>
        ///     Tests whether this <see cref="Circle" /> is overlapping specified <see cref="Rectangle" />.
        /// </summary>
        /// <param name="rectangle"><see cref="Rectangle" /> to test for overlapping.</param>
        /// <returns>True, if circle and rectangle overlaps, false otherwise.</returns>
        public bool Overlaps(in Rectangle rectangle) => rectangle.Overlaps(this);

        /// <summary>
        ///     Returns <see cref="Ellipse" /> which is equivalent to this <see cref="Circle" />.
        /// </summary>
        /// <returns><see cref="Ellipse" /> which is equivalent to this <see cref="Circle" />.</returns>
        public Ellipse ToEllipse() => new(Center, Radius, Radius);

        /// <summary>
        ///     Gets <see cref="AxisAlignedRectangle" /> that encloses this <see cref="Circle" />.
        /// </summary>
        /// <returns><see cref="AxisAlignedRectangle" /> that encloses this <see cref="Circle" />.</returns>
        public AxisAlignedRectangle GetBoundingRectangle() => new(Center, new Vector2(2 * Radius, 2 * Radius));

        /// <summary>
        ///     Converts the value of the current <see cref="Circle" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Circle" /> object.</returns>
        public override string ToString() => $"{nameof(Center)}: {Center}, {nameof(Radius)}: {Radius}";

        #region Equality members

        /// <inheritdoc />
        public bool Equals(Circle other) => Center.Equals(other.Center) && Radius.Equals(other.Radius);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Circle other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Center, Radius);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Circle" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="Circle" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(in Circle left, in Circle right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Circle" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="Circle" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(in Circle left, in Circle right) => !left.Equals(right);

        #endregion
    }
}