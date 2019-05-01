using System.Collections.Generic;

namespace Geisha.Engine.Rendering.Configuration
{
    // TODO Add xml documentation.
    public sealed class RenderingConfiguration
    {
        public const string DefaultSortingLayerName = "Default";

        public List<string> SortingLayersOrder { get; set; } = new List<string> {DefaultSortingLayerName};
    }
}