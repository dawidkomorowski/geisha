using Geisha.Engine.Core.Components;
using Geisha.Engine.Rendering.Configuration;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components
{
    public class SpriteRenderer : IComponent
    {
        public Sprite Sprite { get; set; }
        public string SortingLayerName { get; set; } = RenderingDefaultConfigurationFactory.DefaultSortingLayerName;
        public int SortingOrder { get; set; } = 0;
    }
}