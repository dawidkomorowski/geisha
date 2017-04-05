using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Geisha.Engine.Core.Configuration;

namespace Geisha.Engine.Rendering.Configuration
{
    [Export(typeof(IDefaultConfigurationFactory))]
    public class RenderingDefaultConfigurationFactory : IDefaultConfigurationFactory
    {
        public Type ConfigurationType => typeof(RenderingConfiguration);

        public IConfiguration CreateDefault()
        {
            return new RenderingConfiguration
            {
                SortingLayersOrder = new List<string> {DefaultSortingLayerName}
            };
        }

        public const string DefaultSortingLayerName = "Default";
    }
}