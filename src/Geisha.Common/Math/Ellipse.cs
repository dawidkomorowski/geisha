namespace Geisha.Common.Math
{
    /// <summary>
    ///     Represents 2D ellipse.
    /// </summary>
    public struct Ellipse
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
    }
}