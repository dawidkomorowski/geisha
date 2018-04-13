using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Sprite renderer component enables entity with sprite rendering functionality.
    /// </summary>
    public class SpriteRenderer : RendererBase
    {
        /// <summary>
        ///     Sprite to be rendered.
        /// </summary>
        public Sprite Sprite { get; set; }
    }
}