using System.Collections.Generic;

namespace Geisha.Engine.Core.Configuration
{
    // TODO Add xml documentation.
    public sealed class CoreConfiguration : IConfiguration
    {
        public string AssetsRootDirectoryPath { get; set; } = "Assets";
        public int FixedUpdatesPerSecond { get; set; } = 60;
        public bool ShowAllEntitiesCount { get; set; } = false;
        public bool ShowRootEntitiesCount { get; set; } = false;
        public bool ShowFps { get; set; } = false;
        public bool ShowFrameTime { get; set; } = false;
        public bool ShowTotalFrames { get; set; } = false;
        public bool ShowTotalTime { get; set; } = false;
        public bool ShowSystemsExecutionTimes { get; set; } = false;
        public string StartUpScene { get; set; } = string.Empty;
        public List<string> SystemsExecutionChain { get; set; } = new List<string>();
    }
}