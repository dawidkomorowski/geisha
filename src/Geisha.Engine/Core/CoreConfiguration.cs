using System.Collections.Generic;
using Geisha.Engine.Core.Logging;

namespace Geisha.Engine.Core
{
    /// <summary>
    ///     Configuration of engine core systems and components.
    /// </summary>
    public sealed record CoreConfiguration
    {
        /// <summary>
        ///     Specifies path to root directory for assets discovery and registration. Default is <c>"Assets"</c>.
        /// </summary>
        public string AssetsRootDirectoryPath { get; init; } = "Assets";

        /// <summary>
        ///     Specifies custom game loop steps that should be included in game loop update. Order of steps defines order of
        ///     execution, that is, first step in the list is executed first and last step in the list is executed last. Default is
        ///     empty list.
        /// </summary>
        public IReadOnlyList<string> CustomGameLoopSteps { get; init; } = new List<string>().AsReadOnly();

        /// <summary>
        ///     Specifies maximum number of fixed updates per single frame. Value <c>0</c> means unlimited. Default is <c>0</c>.
        /// </summary>
        public int FixedUpdatesPerFrameLimit { get; init; } = 0;

        /// <summary>
        ///     Target number of fixed updates to be performed in a single second. Default is <c>60</c>.
        /// </summary>
        public int FixedUpdatesPerSecond { get; init; } = 60;

        /// <summary>
        ///     Minimal level of logged messages.
        /// </summary>
        public LogLevel LogLevel { get; init; } = LogLevel.Info;

        /// <summary>
        ///     Specifies whether to display the count of all entities in the current scene. Default is <c>false</c>.
        /// </summary>
        public bool ShowAllEntitiesCount { get; init; } = false;

        /// <summary>
        ///     Specifies whether to display the count of root entities in the current scene. Default is <c>false</c>.
        /// </summary>
        public bool ShowRootEntitiesCount { get; init; } = false;

        /// <summary>
        ///     Specifies whether to display the FPS. Default is <c>false</c>.
        /// </summary>
        public bool ShowFps { get; init; } = false;

        /// <summary>
        ///     Specifies whether to display frame time of last executed frame. Default is <c>false</c>.
        /// </summary>
        public bool ShowFrameTime { get; init; } = false;

        /// <summary>
        ///     Specifies whether to display info about frame time and frame time share of game loop steps. Default is
        ///     <c>false</c>.
        /// </summary>
        public bool ShowGameLoopStatistics { get; init; } = false;

        /// <summary>
        ///     Specifies whether to display total number of frames executed since engine start-up. Default is <c>false</c>.
        /// </summary>
        public bool ShowTotalFrames { get; init; } = false;

        /// <summary>
        ///     Specifies whether to display total time that has passed since engine start-up. Default is <c>false</c>.
        /// </summary>
        public bool ShowTotalTime { get; init; } = false;

        /// <summary>
        ///     Path to scene file that is loaded and started at engine startup. Default is <c>""</c>.
        /// </summary>
        /// <remarks>If <see cref="StartUpScene" /> is non empty then <see cref="StartUpSceneBehavior" /> is ignored.</remarks>
        public string StartUpScene { get; init; } = string.Empty;

        /// <summary>
        ///     Name of scene behavior to use for empty scene that is loaded and started at engine startup. Default is <c>""</c>.
        /// </summary>
        /// <remarks>
        ///     This may be used to run custom initialization code when no scene file is used. If <see cref="StartUpScene" />
        ///     is non empty then <see cref="StartUpSceneBehavior" /> is ignored.
        /// </remarks>
        public string StartUpSceneBehavior { get; init; } = string.Empty;
    }
}