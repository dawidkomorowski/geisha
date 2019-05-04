using System.Collections.Generic;

namespace Geisha.Engine.Core.Configuration
{
    /// <summary>
    ///     Represents type safe configuration of engine core.
    /// </summary>
    public sealed class CoreConfiguration
    {
        /// <summary>
        ///     Specifies path to root directory for assets discovery and registration. Default is <c>"Assets"</c>.
        /// </summary>
        public string AssetsRootDirectoryPath { get; set; } = "Assets";

        /// <summary>
        ///     Target number of fixed updates to be performed in a single second. Default is <c>60</c>.
        /// </summary>
        public int FixedUpdatesPerSecond { get; set; } = 60;

        /// <summary>
        ///     Specifies whether to display the count of all entities in the current scene. Default is <c>false</c>.
        /// </summary>
        public bool ShowAllEntitiesCount { get; set; } = false;

        /// <summary>
        ///     Specifies whether to display the count of root entities in the current scene. Default is <c>false</c>.
        /// </summary>
        public bool ShowRootEntitiesCount { get; set; } = false;

        /// <summary>
        ///     Specifies whether to display the FPS. Default is <c>false</c>.
        /// </summary>
        public bool ShowFps { get; set; } = false;

        /// <summary>
        ///     Specifies whether to display frame time of last executed frame. Default is <c>false</c>.
        /// </summary>
        public bool ShowFrameTime { get; set; } = false;

        /// <summary>
        ///     Specifies whether to display total number of frames executed since engine start-up. Default is <c>false</c>.
        /// </summary>
        public bool ShowTotalFrames { get; set; } = false;

        /// <summary>
        ///     Specifies whether to display total time that has passed since engine start-up. Default is <c>false</c>.
        /// </summary>
        public bool ShowTotalTime { get; set; } = false;

        /// <summary>
        ///     Specifies whether to display diagnostic info about systems execution time and frame time share. Default is
        ///     <c>false</c>.
        /// </summary>
        public bool ShowSystemsExecutionTimes { get; set; } = false;

        /// <summary>
        ///     Path to scene file that is loaded and started at engine start-up. Default is <c>""</c>.
        /// </summary>
        public string StartUpScene { get; set; } = string.Empty;

        /// <summary>
        ///     Specifies execution chain of systems in order of execution that first system in the list is executed first, last
        ///     system in the list is executed last. Default is empty list.
        /// </summary>
        public List<string> SystemsExecutionChain { get; set; } = new List<string>();
    }
}