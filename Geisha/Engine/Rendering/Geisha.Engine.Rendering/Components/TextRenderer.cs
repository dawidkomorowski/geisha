using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Text renderer component enables entity with text rendering functionality.
    /// </summary>
    public sealed class TextRenderer : RendererBase
    {
        /// <summary>
        ///     Text content to be rendered.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Size of font used for text rendering defined in pixels.
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        ///     Color of font used for text rendering.
        /// </summary>
        public Color Color { get; set; }
    }
}