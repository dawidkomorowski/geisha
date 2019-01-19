using Geisha.Common.Math;

namespace Geisha.Engine.Physics.Components
{
    /// <summary>
    ///     2D collider component in shape of a rectangle.
    /// </summary>
    public sealed class RectangleCollider : Collider2DComponent
    {
        /// <summary>
        ///     Dimension of rectangle.
        /// </summary>
        public Vector2 Dimension { get; set; }
    }
}