namespace Geisha.Common.Math.SAT
{
    /// <summary>
    ///     Interface representing 2D geometric shape that can be used in Separating Axis Theorem algorithm.
    /// </summary>
    /// <remarks>
    ///     If you would like to enable some 2D geometric shape representation to be able to be consumed by SAT create
    ///     implementation of this interface.
    /// </remarks>
    public interface IShape
    {
        /// <summary>
        ///     Indicates whether the shape is a circle.
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

        Axis[] GetAxes();
        Vector2[] GetVertices();
    }
}