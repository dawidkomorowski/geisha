using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Sprite renderer component enables entity with sprite rendering functionality.
    /// </summary>
    public sealed class SpriteRendererComponent : Renderer2DComponent
    {
        /// <summary>
        ///     Sprite to be rendered.
        /// </summary>
        public Sprite Sprite { get; set; }
    }
}