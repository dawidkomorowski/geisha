using Geisha.Common.Math;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Rectangle renderer component enables entity with rectangle rendering functionality.
    /// </summary>
    public sealed class RectangleRendererComponent : Renderer2DComponent
    {
        /// <summary>
        ///     Dimension of rectangle. Rectangle has center at point (0,0) in local coordinate system.
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