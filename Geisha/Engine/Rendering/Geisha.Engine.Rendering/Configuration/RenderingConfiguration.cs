using System.Collections.Generic;
using Geisha.Engine.Core.Configuration;

namespace Geisha.Engine.Rendering.Configuration
{
    public class RenderingConfiguration : IConfiguration
    {
        public const string DefaultSortingLayerName = "Default";

        public List<string> SortingLayersOrder { get; set; } = new List<string> {DefaultSortingLayerName};
    }
}