using System;
using Geisha.Engine.Core.Math.SAT;

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
        ///     This method transforms only circle center therefore scaling of circle is not supported.
        /// </remarks>
        public Circle Transform(in Matrix3x3 transform) => new Circle((transform * Center.Homogeneous).ToVector2(), Radius);

        /// <summary>
        ///     Tests whether this <see cref="Circle" /> is overlapping other <see cref="Circle" />.
        /// </summary>
        /// <param name="other"><see cref="Circle" /> to test for overlapping.</param>
        /// <returns>True, if circles overlap, false otherwise.</returns>
        public bool Overlaps(in Circle other) => AsShape().Overlaps(other.AsShape());

        /// <summary>
        ///     Returns representation of this <see cref="Circle" /> as implementation of <see cref="IShape" />.
        /// </summary>
        /// <returns><see cref="IShape" /> representing this <see cref="Circle" />.</returns>
        public IShape AsShape() => new CircleForSat(this);

        /// <summary>
        ///     Returns <see cref="Ellipse" /> which is equivalent to this <see cref="Circle" />.
        /// </summary>
        /// <returns><see cref="Ellipse" /> which is equivalent to this <see cref="Circle" />.</returns>
        public Ellipse ToEllipse() => new Ellipse(Center, Radius, Radius);

        /// <summary>
        ///     Gets <see cref="AxisAlignedRectangle" /> that encloses this <see cref="Circle" />.
        /// </summary>
        /// <returns><see cref="AxisAlignedRectangle" /> that encloses this <see cref="Circle" />.</returns>
        public AxisAlignedRectangle GetBoundingRectangle() => new AxisAlignedRectangle(Center, new Vector2(2 * Radius, 2 * Radius));

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

        private class CircleForSat : IShape
        {
            private readonly Circle _circle;

            public CircleForSat(Circle circle)
            {
                _circle = circle;
            }

            public bool IsCircle => true;
            public Vector2 Center => _circle.Center;
            public double Radius => _circle.Radius;

            public Axis[] GetAxes() => throw new NotSupportedException();

            public Vector2[] GetVertices() => throw new NotSupportedException();
        }
    }
}