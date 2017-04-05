using System.Collections.Generic;
using Geisha.Engine.Core.Configuration;

namespace Geisha.Engine.Rendering.Configuration
{
    public class RenderingConfiguration : IConfiguration
    {
        public List<string> SortingLayersOrder { get; set; }
    }
}