using System;
using Geisha.Common.Math.SAT;

namespace Geisha.Common.Math
{
    /// <summary>
    ///     Represents 2D rectangle.
    /// </summary>
    public readonly struct Rectangle
    {
        private readonly Vector3 _upperLeft;
        private readonly Vector3 _upperRight;
        private readonly Vector3 _lowerLeft;
        private readonly Vector3 _lowerRight;

        /// <summary>
        ///     Creates new instance of <see cref="Rectangle" /> with given dimension and center at point (0,0).
        /// </summary>
        /// <param name="dimension">Dimension, width and height, of rectangle.</param>
        public Rectangle(in Vector2 dimension) : this(Vector2.Zero, dimension)
        {
        }

        /// <summary>
        ///     Creates new instance of <see cref="Rectangle" /> with given dimension and center at given position.
        /// </summary>
        /// <param name="center">Position of rectangle center.</param>
        /// <param name="dimension">Dimension, width and height, or rectangle.</param>
        public Rectangle(in Vector2 center, in Vector2 dimension)
        {
            _upperLeft = new Vector2(-dimension.X / 2 + center.X, dimension.Y / 2 + center.Y).Homogeneous;
            _upperRight = new Vector2(dimension.X / 2 + center.X, dimension.Y / 2 + center.Y).Homogeneous;
            _lowerLeft = new Vector2(-dimension.X / 2 + center.X, -dimension.Y / 2 + center.Y).Homogeneous;
            _lowerRight = new Vector2(dimension.X / 2 + center.X, -dimension.Y / 2 + center.Y).Homogeneous;
        }

        private Rectangle(in Vector3 upperLeft, in Vector3 upperRight, in Vector3 lowerLeft, in Vector3 lowerRight)
        {
            _upperLeft = upperLeft;
            _upperRight = upperRight;
            _lowerLeft = lowerLeft;
            _lowerRight = lowerRight;
        }

        /// <summary>
        ///     Upper-left vertex of rectangle.
        /// </summary>
        public Vector2 UpperLeft => _upperLeft.ToVector2();

        /// <summary>
        ///     Upper-right vertex of rectangle.
        /// </summary>
        public Vector2 UpperRight => _upperRight.ToVector2();

        /// <summary>
        ///     Lower-left vertex of rectangle.
        /// </summary>
        public Vector2 LowerLeft => _lowerLeft.ToVector2();

        /// <summary>
        ///     Lower-right vertex of rectangle.
        /// </summary>
        public Vector2 LowerRight => _lowerRight.ToVector2();

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
        public Rectangle Transform(in Matrix3x3 transform)
        {
            return new Rectangle(
                transform * _upperLeft,
                transform * _upperRight,
                transform * _lowerLeft,
                transform * _lowerRight
            );
        }

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
        ///     Converts the value of the current <see cref="Rectangle" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Rectangle" /> object.</returns>
        public override string ToString() =>
            $"{nameof(Center)}: {Center}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(UpperLeft)}: {UpperLeft}, {nameof(UpperRight)}: {UpperRight}, {nameof(LowerLeft)}: {LowerLeft}, {nameof(LowerRight)}: {LowerRight}";

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