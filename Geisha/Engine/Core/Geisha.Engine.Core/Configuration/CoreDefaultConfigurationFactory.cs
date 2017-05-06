using System;
using System.ComponentModel.Composition;

namespace Geisha.Engine.Core.Configuration
{
    [Export(typeof(IDefaultConfigurationFactory))]
    public class CoreDefaultConfigurationFactory : IDefaultConfigurationFactory
    {
        public Type ConfigurationType => typeof(CoreConfiguration);

        public IConfiguration CreateDefault()
        {
            return new CoreConfiguration
            {
                FixedDeltaTime = 0.016,
                ShowAllEntitiesCount = false,
                ShowRootEntitiesCount = false,
                ShowFps = false,
                ShowFrameTime = false,
                ShowTotalFrames = false,
                ShowTotalTime = false
            };
        }
    }
}