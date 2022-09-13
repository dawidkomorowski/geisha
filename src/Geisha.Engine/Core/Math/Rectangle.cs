﻿using System;
using Geisha.Engine.Core.Math.SAT;

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
        public Vector2 Center => new Vector2((LowerLeft.X + UpperRight.X) / 2, (LowerLeft.Y + UpperRight.Y) / 2);

        /// <summary>
        ///     Returns <see cref="Rectangle" /> that is this <see cref="Rectangle" /> transformed by given
        ///     <see cref="Matrix3x3" />.
        /// </summary>
        /// <param name="transform">Transformation matrix used to transform rectangle.</param>
        /// <returns><see cref="Rectangle" /> transformed by given matrix.</returns>
        public Rectangle Transform(in Matrix3x3 transform) =>
            new Rectangle(
                (transform * UpperLeft.Homogeneous).ToVector2(),
                (transform * UpperRight.Homogeneous).ToVector2(),
                (transform * LowerLeft.Homogeneous).ToVector2(),
                (transform * LowerRight.Homogeneous).ToVector2()
            );

        /// <summary>
        ///     Tests whether this <see cref="Rectangle" /> is overlapping other <see cref="Rectangle" />.
        /// </summary>
        /// <param name="other"><see cref="Rectangle" /> to test for overlapping.</param>
        /// <returns>True, if rectangles overlap, false otherwise.</returns>
        public bool Overlaps(in Rectangle other) => AsShape().Overlaps(other.AsShape());

        /// <summary>
        ///     Returns representation of this <see cref="Rectangle" /> as implementation of <see cref="IShape" />.
        /// </summary>
        /// <returns><see cref="IShape" /> representing this <see cref="Rectangle" />.</returns>
        public IShape AsShape() => new RectangleForSat(this);

        /// <summary>
        ///     Gets <see cref="AxisAlignedRectangle" /> that encloses this <see cref="Rectangle" />.
        /// </summary>
        /// <returns><see cref="AxisAlignedRectangle" /> that encloses this <see cref="Rectangle" />.</returns>
        public AxisAlignedRectangle GetBoundingRectangle()
        {
            Span<Vector2> vertices = stackalloc Vector2[4];
            vertices[0] = UpperLeft;
            vertices[1] = UpperRight;
            vertices[2] = LowerLeft;
            vertices[3] = LowerRight;
            return new AxisAlignedRectangle(vertices);
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

        private class RectangleForSat : IShape
        {
            private readonly Rectangle _rectangle;

            public RectangleForSat(Rectangle rectangle)
            {
                _rectangle = rectangle;
            }

            public bool IsCircle => false;
            public Vector2 Center => throw new NotSupportedException();
            public double Radius => throw new NotSupportedException();

            public Axis[] GetAxes()
            {
                var normal1 = (_rectangle.UpperLeft - _rectangle.LowerLeft).Normal;
                var normal2 = (_rectangle.UpperRight - _rectangle.UpperLeft).Normal;
                return new[] { new Axis(normal1), new Axis(normal2) };
            }

            public Vector2[] GetVertices()
            {
                return new[] { _rectangle.LowerLeft, _rectangle.LowerRight, _rectangle.UpperRight, _rectangle.UpperLeft };
            }
        }
    }
}