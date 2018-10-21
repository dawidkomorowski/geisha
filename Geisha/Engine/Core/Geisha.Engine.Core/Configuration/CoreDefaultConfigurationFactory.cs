using System;
using System.Collections.Generic;

namespace Geisha.Engine.Core.Configuration
{
    // TODO Introduce adapter and remove redundant tests
    public class CoreDefaultConfigurationFactory : IDefaultConfigurationFactory
    {
        public Type ConfigurationType => typeof(CoreConfiguration);

        public IConfiguration CreateDefault()
        {
            return new CoreConfiguration
            {
                FixedUpdatesPerSecond = 60,
                ShowAllEntitiesCount = false,
                ShowRootEntitiesCount = false,
                ShowFps = false,
                ShowFrameTime = false,
                ShowTotalFrames = false,
                ShowTotalTime = false,
                StartUpScene = string.Empty,
                SystemsExecutionChain = new List<string>()
            };
        }
    }
}