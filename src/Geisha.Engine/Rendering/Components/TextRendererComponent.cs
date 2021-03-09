using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;

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

    internal sealed class TextRendererComponentFactory : IComponentFactory
    {
        public Type ComponentType { get; } = typeof(TextRendererComponent);
        public ComponentId ComponentId { get; } = new ComponentId("Geisha.Engine.Rendering.TextRendererComponent");
        public IComponent Create() => new TextRendererComponent();
    }
}