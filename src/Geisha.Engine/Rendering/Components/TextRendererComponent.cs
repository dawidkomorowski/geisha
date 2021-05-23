using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Text renderer component enables entity with text rendering functionality.
    /// </summary>
    [ComponentId("Geisha.Engine.Rendering.TextRendererComponent")]
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

    internal sealed class TextRendererComponentFactory : ComponentFactory<TextRendererComponent>
    {
        protected override TextRendererComponent CreateComponent() => new TextRendererComponent();
    }
}