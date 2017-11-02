namespace Geisha.Common.Math.SAT
{
    /// <summary>
    ///     Interface representing 2D geometric shape that can be used in Separating Axis Theorem algorithm. Represents either
    ///     circle or convex polygon.
    /// </summary>
    /// <remarks>
    ///     If you would like to enable some 2D geometric shape representation to be able to be consumed by SAT create
    ///     implementation of this interface. SAT supports circles and convex polygons only.
    /// </remarks>
    public interface IShape
    {
        /// <summary>
        ///     Indicates whether the shape is a circle. If it is not a circle then it is assumed that the shape is convex polygon.
        /// </summary>
        bool IsCircle { get; }

        /// <summary>
        ///     Represents center of a circle.
        /// </summary>
        /// <remarks>When implementing <see cref="IShape" /> this property is required only for circle.</remarks>
        Vector2 Center { get; }

        /// <summary>
        ///     Represents radius of a circle.
        /// </summary>
        /// <remarks>When implementing <see cref="IShape" /> this property is required only for circle.</remarks>
        double Radius { get; }

        /// <summary>
        ///     Returns unique axes used for projection testing in SAT.
        /// </summary>
        /// <returns>Array of <see cref="Axis" /> that will be used for projection testing in SAT.</returns>
        /// <remarks>
        ///     When implementing <see cref="IShape" /> this method is required only for polygon.
        ///     <br />
        ///     You can return null as simple implementation of this method.
        ///     <br />
        ///     If this method returns null then SAT provides automatic axis discovery. For each polygon edge there is computed
        ///     single axis. Therefore for certain shapes it is highly recommended to provide custom axes to reduce complexity and
        ///     improve performance. In example rectangle requires only two unique axes (as it consists of two pairs of parallel
        ///     edges) while default SAT discovery will produce four axes for rectangle (one for each edge).
        /// </remarks>
        Axis[] GetAxes();

        /// <summary>
        ///     Returns vertices of convex polygon in counterclockwise winding order.
        /// </summary>
        /// <returns>Array of <see cref="Vector2" /> that contains all vertices of polygon in counterclockwise winding order.</returns>
        /// <remarks>When implementing <see cref="IShape" /> this method is required only for polygon.</remarks>
        Vector2[] GetVertices();
    }
}