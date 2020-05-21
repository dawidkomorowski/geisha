namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Text renderer component enables entity with text rendering functionality.
    /// </summary>
    public sealed class TextRendererComponent : Renderer2DComponent
    {
        /// <summary>
        ///     Text content to be rendered.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        ///     Size of font used for text rendering.
        /// </summary>
        public FontSize FontSize { get; set; }

        /// <summary>
        ///     Color of font used for text rendering.
        /// </summary>
        public Color Color { get; set; }
    }
}