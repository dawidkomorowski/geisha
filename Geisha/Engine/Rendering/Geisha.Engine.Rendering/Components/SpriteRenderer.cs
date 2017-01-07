using Geisha.Engine.Core.Components;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components
{
    public class SpriteRenderer : IComponent
    {
        public ITexture Texture { get; set; }
    }
}