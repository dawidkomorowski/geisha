﻿using System;
using Geisha.Common.Math;
using Geisha.Common.Math.SAT;

namespace MicroBenchmark
{
    /// <summary>
    ///     Represents 2D rectangle.
    /// </summary>
    public sealed class RectangleBaseline : QuadBaseline
    {
        /// <summary>
        ///     Creates new instance of <see cref="RectangleBaseline" /> with given dimension and center at point (0,0).
        /// </summary>
        /// <param name="dimension">Dimension, width and height, of rectangle.</param>
        public RectangleBaseline(Vector2 dimension) : this(Vector2.Zero, dimension)
        {
        }

        /// <summary>
        ///     Creates new instance of <see cref="RectangleBaseline" /> with given dimension and center at given position.
        /// </summary>
        /// <param name="center">Position of rectangle center.</param>
        /// <param name="dimension">Dimension, width and height, or rectangle.</param>
        public RectangleBaseline(Vector2 center, Vector2 dimension) : base(
            new Vector2(-dimension.X / 2 + center.X, -dimension.Y / 2 + center.Y),
            new Vector2(dimension.X / 2 + center.X, -dimension.Y / 2 + center.Y),
            new Vector2(dimension.X / 2 + center.X, dimension.Y / 2 + center.Y),
            new Vector2(-dimension.X / 2 + center.X, dimension.Y / 2 + center.Y))
        {
        }

        private RectangleBaseline(QuadBaseline quad) : base(quad.V1, quad.V2, quad.V3, quad.V4)
        {
        }

        /// <summary>
        ///     Upper-left vertex of rectangle.
        /// </summary>
        public Vector2 UpperLeft => V4;

        /// <summary>
        ///     Upper-right vertex of rectangle.
        /// </summary>
        public Vector2 UpperRight => V3;

        /// <summary>
        ///     Lower-left vertex of rectangle.
        /// </summary>
        public Vector2 LowerLeft => V1;

        /// <summary>
        ///     Lower-right vertex of rectangle.
        /// </summary>
        public Vector2 LowerRight => V2;

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
        ///     Returns <see cref="RectangleBaseline" /> that is this <see cref="RectangleBaseline" /> transformed by given
        ///     <see cref="Matrix3x3" />.
        /// </summary>
        /// <param name="transform">Transformation matrix used to transform rectangle.</param>
        /// <returns><see cref="RectangleBaseline" /> transformed by given matrix.</returns>
        public new RectangleBaseline Transform(Matrix3x3 transform) => new RectangleBaseline(base.Transform(transform));

        /// <summary>
        ///     Tests whether this <see cref="RectangleBaseline" /> is overlapping other <see cref="RectangleBaseline" />.
        /// </summary>
        /// <param name="other"><see cref="RectangleBaseline" /> to test for overlapping.</param>
        /// <returns>True, if rectangles overlap, false otherwise.</returns>
        public bool Overlaps(RectangleBaseline other) => AsShape().Overlaps(other.AsShape());

        /// <summary>
        ///     Returns representation of this <see cref="RectangleBaseline" /> as implementation of <see cref="IShape" />.
        /// </summary>
        /// <returns><see cref="IShape" /> representing this <see cref="RectangleBaseline" />.</returns>
        public IShape AsShape() => new RectangleForSat(this);

        /// <summary>
        ///     Converts the value of the current <see cref="RectangleBaseline" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="RectangleBaseline" /> object.</returns>
        public override string ToString() =>
            $"{nameof(Center)}: {Center}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(UpperLeft)}: {UpperLeft}, {nameof(UpperRight)}: {UpperRight}, {nameof(LowerLeft)}: {LowerLeft}, {nameof(LowerRight)}: {LowerRight}";

        private class RectangleForSat : IShape
        {
            private readonly RectangleBaseline _rectangle;

            public RectangleForSat(RectangleBaseline rectangle)
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

    /// <summary>
    ///     Represents 2D quad.
    /// </summary>
    /// <remarks>
    ///     Vertices of <see cref="QuadBaseline" /> are indexed in counterclockwise winding order.
    /// </remarks>
    public class QuadBaseline
    {
        private readonly Vector3 _v1;
        private readonly Vector3 _v2;
        private readonly Vector3 _v3;
        private readonly Vector3 _v4;

        /// <summary>
        ///     Creates new instance of <see cref="QuadBaseline" /> with given vertices. Those should be in counterclockwise winding order.
        /// </summary>
        /// <param name="v1">First vertex of <see cref="QuadBaseline" />.</param>
        /// <param name="v2">Second vertex of <see cref="QuadBaseline" />.</param>
        /// <param name="v3">Third vertex of <see cref="QuadBaseline" />.</param>
        /// <param name="v4">Fourth vertex of <see cref="QuadBaseline" />.</param>
        public QuadBaseline(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
        {
            _v1 = v1.Homogeneous;
            _v2 = v2.Homogeneous;
            _v3 = v3.Homogeneous;
            _v4 = v4.Homogeneous;
        }

        /// <summary>
        ///     First vertex of quad.
        /// </summary>
        public Vector2 V1 => _v1.ToVector2();

        /// <summary>
        ///     Second vertex of quad.
        /// </summary>
        public Vector2 V2 => _v2.ToVector2();

        /// <summary>
        ///     Third vertex of quad.
        /// </summary>
        public Vector2 V3 => _v3.ToVector2();

        /// <summary>
        ///     Fourth vertex of quad.
        /// </summary>
        public Vector2 V4 => _v4.ToVector2();

        /// <summary>
        ///     Returns <see cref="QuadBaseline" /> that is this <see cref="QuadBaseline" /> transformed by given <see cref="Matrix3x3" />.
        /// </summary>
        /// <param name="transform">Transformation matrix used to transform quad.</param>
        /// <returns><see cref="QuadBaseline" /> transformed by given matrix.</returns>
        public QuadBaseline Transform(Matrix3x3 transform)
        {
            return new QuadBaseline(
                (transform * _v1).ToVector2(),
                (transform * _v2).ToVector2(),
                (transform * _v3).ToVector2(),
                (transform * _v4).ToVector2()
            );
        }
    }
}