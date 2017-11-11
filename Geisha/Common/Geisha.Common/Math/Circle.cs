using System;
using Geisha.Common.Math.SAT;

namespace Geisha.Common.Math
{
    // TODO Consider changing to struct?
    /// <summary>
    ///     Represents 2D circle.
    /// </summary>
    public sealed class Circle
    {
        private readonly Vector3 _center;

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
        public Circle(Vector2 center, double radius)
        {
            _center = center.Homogeneous;
            Radius = radius;
        }

        /// <summary>
        ///     Center of circle.
        /// </summary>
        public Vector2 Center => _center.ToVector2();

        /// <summary>
        ///     Radius of circle.
        /// </summary>
        public double Radius { get; }

        /// <summary>
        ///     Returns <see cref="Circle" /> that is this <see cref="Circle" /> transformed by given <see cref="Matrix3" />.
        /// </summary>
        /// <param name="transform">Transformation matrix used to transform circle.</param>
        /// <returns><see cref="Circle" /> transformed by given matrix.</returns>
        /// <remarks>
        ///     This method transforms only circle center therefore scaling of circle is not supported.
        /// </remarks>
        public Circle Transform(Matrix3 transform)
        {
            return new Circle((transform * _center).ToVector2(), Radius);
        }

        /// <summary>
        ///     Tests whether this <see cref="Circle" /> is overlapping other <see cref="Circle" />.
        /// </summary>
        /// <param name="other"><see cref="Circle" /> to test for overlapping.</param>
        /// <returns>True, if circles overlap, false otherwise.</returns>
        public bool Overlaps(Circle other)
        {
            return AsShape().Overlaps(other.AsShape());
        }

        /// <summary>
        ///     Returns representation of this <see cref="Circle" /> as implementation of <see cref="IShape" />.
        /// </summary>
        /// <returns><see cref="IShape" /> representing this <see cref="Circle" />.</returns>
        public IShape AsShape()
        {
            return new CircleForSat(this);
        }

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

            public Axis[] GetAxes()
            {
                throw new NotSupportedException();
            }

            public Vector2[] GetVertices()
            {
                throw new NotSupportedException();
            }
        }
    }
}