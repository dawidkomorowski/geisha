namespace Geisha.Engine.Core.Math.SAT
{
    /// <summary>
    ///     Represents 2D axis used in SAT.
    /// </summary>
    public readonly struct Axis
    {
        private readonly Vector2 _axisAlignedUnitVector;

        /// <summary>
        ///     Creates new instance of <see cref="Axis" /> with direction given by vector.
        /// </summary>
        /// <param name="axisAlignedVector">Vector being source of direction for an axis.</param>
        public Axis(Vector2 axisAlignedVector)
        {
            _axisAlignedUnitVector = axisAlignedVector.Unit; // Unit vector is required for simple projection with dot product.
        }

        /// <summary>
        ///     Returns orthogonal projection of an <see cref="IShape" /> onto an axis.
        /// </summary>
        /// <param name="shape"><see cref="IShape" /> to be projected.</param>
        /// <returns>Orthogonal projection of an <see cref="IShape" /> onto an axis.</returns>
        public Projection GetProjectionOf(IShape shape)
        {
            if (shape.IsCircle)
            {
                var projected = shape.Center.Dot(_axisAlignedUnitVector);
                return new Projection(projected - shape.Radius, projected + shape.Radius);
            }

            return GetProjectionOf(shape.GetVertices());
        }

        /// <summary>
        ///     Returns orthogonal projection of a polygon, defined as set of points, onto an axis.
        /// </summary>
        /// <param name="vertices">Set of points to be projected.</param>
        /// <returns>Orthogonal projection of a polygon, defined as set of points, onto an axis.</returns>
        public Projection GetProjectionOf(Vector2[] vertices)
        {
            var min = double.MaxValue;
            var max = double.MinValue;

            for (var i = 0; i < vertices.Length; i++)
            {
                var projected = vertices[i].Dot(_axisAlignedUnitVector);
                min = System.Math.Min(min, projected);
                max = System.Math.Max(max, projected);
            }

            return new Projection(min, max);
        }

        /// <summary>
        ///     Returns orthogonal projection of a point onto an axis.
        /// </summary>
        /// <param name="point">Point to be projected.</param>
        /// <returns>Orthogonal projection of a point onto an axis.</returns>
        /// <remarks>
        ///     <see cref="Projection" /> for a single point has <see cref="Projection.Min" /> equal to
        ///     <see cref="Projection.Max" />.
        /// </remarks>
        public Projection GetProjectionOf(Vector2 point)
        {
            var pointProjection = point.Dot(_axisAlignedUnitVector);
            return new Projection(pointProjection, pointProjection);
        }
    }
}