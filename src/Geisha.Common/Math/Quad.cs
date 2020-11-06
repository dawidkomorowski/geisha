namespace Geisha.Common.Math
{
    // TODO add AsShape()?
    /// <summary>
    ///     Represents 2D quad.
    /// </summary>
    /// <remarks>
    ///     Vertices of <see cref="Quad" /> are indexed in counterclockwise winding order.
    /// </remarks>
    public class Quad
    {
        private readonly Vector3 _v1;
        private readonly Vector3 _v2;
        private readonly Vector3 _v3;
        private readonly Vector3 _v4;

        /// <summary>
        ///     Creates new instance of <see cref="Quad" /> with given vertices. Those should be in counterclockwise winding order.
        /// </summary>
        /// <param name="v1">First vertex of <see cref="Quad" />.</param>
        /// <param name="v2">Second vertex of <see cref="Quad" />.</param>
        /// <param name="v3">Third vertex of <see cref="Quad" />.</param>
        /// <param name="v4">Fourth vertex of <see cref="Quad" />.</param>
        public Quad(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
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
        ///     Returns <see cref="Quad" /> that is this <see cref="Quad" /> transformed by given <see cref="Matrix3x3" />.
        /// </summary>
        /// <param name="transform">Transformation matrix used to transform quad.</param>
        /// <returns><see cref="Quad" /> transformed by given matrix.</returns>
        public Quad Transform(Matrix3x3 transform)
        {
            return new Quad(
                (transform * _v1).ToVector2(),
                (transform * _v2).ToVector2(),
                (transform * _v3).ToVector2(),
                (transform * _v4).ToVector2()
            );
        }
    }
}