using Geisha.Common.Math;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components
{
    // TODO Create serializable version of this component.
    /// <summary>
    ///     Rectangle renderer component enables entity with rectangle rendering functionality.
    /// </summary>
    public sealed class RectangleRendererComponent : Renderer2DComponent
    {
        // TODO Which point of rectangle is taken as anchor for rendering. If rotation is applied then around which point?
        /// <summary>
        ///     Dimension of rectangle.
        /// </summary>
        public Vector2 Dimension { get; set; }

        /// <summary>
        ///     Color of the rectangle.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        ///     Specifies whether to fill interior of rectangle or draw only border. If true interior is filled with color.
        /// </summary>
        public bool FillInterior { get; set; }
    }
}