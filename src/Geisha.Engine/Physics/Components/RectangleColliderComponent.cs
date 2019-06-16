using Geisha.Common.Math;

namespace Geisha.Engine.Physics.Components
{
    /// <summary>
    ///     2D collider component in shape of a rectangle.
    /// </summary>
    public sealed class RectangleColliderComponent : Collider2DComponent
    {
        // TODO Which point of rectangle is taken as anchor for rendering. If rotation is applied then around which point?
        /// <summary>
        ///     Dimension of rectangle.
        /// </summary>
        public Vector2 Dimension { get; set; }
    }
}