using Geisha.Engine.Core.Components;
using Geisha.Engine.Rendering.Configuration;

namespace Geisha.Engine.Rendering.Components
{
    public abstract class RendererBase : IComponent
    {
        public bool Visible { get; set; } = true;
        public string SortingLayerName { get; set; } = RenderingDefaultConfigurationFactory.DefaultSortingLayerName;
        public int OrderInLayer { get; set; } = 0;
    }
}