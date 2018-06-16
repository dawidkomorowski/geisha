using System.Collections.Generic;

namespace Geisha.Engine.Core.Configuration
{
    public class CoreConfiguration : IConfiguration
    {
        public double FixedDeltaTime { get; set; }
        public bool ShowAllEntitiesCount { get; set; }
        public bool ShowRootEntitiesCount { get; set; }
        public bool ShowFps { get; set; }
        public bool ShowFrameTime { get; set; }
        public bool ShowTotalFrames { get; set; }
        public bool ShowTotalTime { get; set; }
        public string StartUpScene { get; set; }
        public List<string> SystemsExecutionChain { get; set; } = new List<string>();
    }
}