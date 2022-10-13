﻿using System.Collections.Generic;
using Geisha.Engine.Core.Logging;

namespace Geisha.Engine.Core
{
    /// <summary>
    ///     Configuration of engine core systems and components.
    /// </summary>
    public sealed class CoreConfiguration
    {
        private CoreConfiguration(
            string assetsRootDirectoryPath,
            IReadOnlyList<string> customGameLoopSteps,
            int fixedUpdatesPerFrameLimit,
            int fixedUpdatesPerSecond,
            LogLevel level,
            bool showAllEntitiesCount,
            bool showRootEntitiesCount,
            bool showFps,
            bool showFrameTime,
            bool showGameLoopStatistics,
            bool showTotalFrames,
            bool showTotalTime,
            string startUpScene,
            string startUpSceneBehavior)
        {
            AssetsRootDirectoryPath = assetsRootDirectoryPath;
            CustomGameLoopSteps = customGameLoopSteps;
            FixedUpdatesPerFrameLimit = fixedUpdatesPerFrameLimit;
            FixedUpdatesPerSecond = fixedUpdatesPerSecond;
            LogLevel = level;
            ShowAllEntitiesCount = showAllEntitiesCount;
            ShowRootEntitiesCount = showRootEntitiesCount;
            ShowFps = showFps;
            ShowFrameTime = showFrameTime;
            ShowGameLoopStatistics = showGameLoopStatistics;
            ShowTotalFrames = showTotalFrames;
            ShowTotalTime = showTotalTime;
            StartUpScene = startUpScene;
            StartUpSceneBehavior = startUpSceneBehavior;
        }

        /// <summary>
        ///     Specifies path to root directory for assets discovery and registration. Default is <c>"Assets"</c>.
        /// </summary>
        public string AssetsRootDirectoryPath { get; }

        /// <summary>
        ///     Specifies custom game loop steps that should be included in game loop update. Order of steps defines order of
        ///     execution, that is, first step in the list is executed first and last step in the list is executed last. Default is
        ///     empty list.
        /// </summary>
        public IReadOnlyList<string> CustomGameLoopSteps { get; }

        /// <summary>
        ///     Specifies maximum number of fixed updates per single frame. Value <c>0</c> means unlimited. Default is <c>0</c>.
        /// </summary>
        public int FixedUpdatesPerFrameLimit { get; }

        /// <summary>
        ///     Target number of fixed updates to be performed in a single second. Default is <c>60</c>.
        /// </summary>
        public int FixedUpdatesPerSecond { get; }

        /// <summary>
        ///     Minimal level of logged messages.
        /// </summary>
        public LogLevel LogLevel { get; }

        /// <summary>
        ///     Specifies whether to display the count of all entities in the current scene. Default is <c>false</c>.
        /// </summary>
        public bool ShowAllEntitiesCount { get; }

        /// <summary>
        ///     Specifies whether to display the count of root entities in the current scene. Default is <c>false</c>.
        /// </summary>
        public bool ShowRootEntitiesCount { get; }

        /// <summary>
        ///     Specifies whether to display the FPS. Default is <c>false</c>.
        /// </summary>
        public bool ShowFps { get; }

        /// <summary>
        ///     Specifies whether to display frame time of last executed frame. Default is <c>false</c>.
        /// </summary>
        public bool ShowFrameTime { get; }

        /// <summary>
        ///     Specifies whether to display info about frame time and frame time share of game loop steps. Default is
        ///     <c>false</c>.
        /// </summary>
        public bool ShowGameLoopStatistics { get; }

        /// <summary>
        ///     Specifies whether to display total number of frames executed since engine start-up. Default is <c>false</c>.
        /// </summary>
        public bool ShowTotalFrames { get; }

        /// <summary>
        ///     Specifies whether to display total time that has passed since engine start-up. Default is <c>false</c>.
        /// </summary>
        public bool ShowTotalTime { get; }

        /// <summary>
        ///     Path to scene file that is loaded and started at engine startup. Default is <c>""</c>.
        /// </summary>
        /// <remarks>If <see cref="StartUpScene" /> is non empty then <see cref="StartUpSceneBehavior" /> is ignored.</remarks>
        public string StartUpScene { get; }

        /// <summary>
        ///     Name of scene behavior to use for empty scene that is loaded and started at engine startup. Default is <c>""</c>.
        /// </summary>
        /// <remarks>
        ///     This may be used to run custom initialization code when no scene file is used. If <see cref="StartUpScene" />
        ///     is non empty then <see cref="StartUpSceneBehavior" /> is ignored.
        /// </remarks>
        public string StartUpSceneBehavior { get; }

        public static IBuilder CreateBuilder() => new Builder();

        public interface IBuilder
        {
            IBuilder WithAssetsRootDirectoryPath(string assetsRootDirectoryPath);
            IBuilder WithCustomGameLoopSteps(IReadOnlyList<string> customGameLoopSteps);
            IBuilder WithFixedUpdatesPerFrameLimit(int fixedUpdatesPerFrameLimit);
            IBuilder WithFixedUpdatesPerSecond(int fixedUpdatesPerSecond);
            IBuilder WithLogLevel(LogLevel level);
            IBuilder WithShowAllEntitiesCount(bool showAllEntitiesCount);
            IBuilder WithShowRootEntitiesCount(bool showRootEntitiesCount);
            IBuilder WithShowFps(bool showFps);
            IBuilder WithShowFrameTime(bool showFrameTime);
            IBuilder WithShowGameLoopStatistics(bool showGameLoopStatistics);
            IBuilder WithShowTotalFrames(bool showTotalFrames);
            IBuilder WithShowTotalTime(bool showTotalTime);
            IBuilder WithStartUpScene(string startUpScene);
            IBuilder WithStartUpSceneBehavior(string startUpSceneBehavior);
            CoreConfiguration Build();
        }

        private sealed class Builder : IBuilder
        {
            private string _assetsRootDirectoryPath = "Assets";
            private IReadOnlyList<string> _customGameLoopSteps = new List<string>().AsReadOnly();
            private int _fixedUpdatesPerFrameLimit;
            private int _fixedUpdatesPerSecond = 60;
            private LogLevel _logLevel = LogLevel.Info;
            private bool _showAllEntitiesCount;
            private bool _showFps;
            private bool _showFrameTime;
            private bool _showGameLoopStatistics;
            private bool _showRootEntitiesCount;
            private bool _showTotalFrames;
            private bool _showTotalTime;
            private string _startUpScene = string.Empty;
            private string _startUpSceneBehavior = string.Empty;

            public IBuilder WithAssetsRootDirectoryPath(string assetsRootDirectoryPath)
            {
                _assetsRootDirectoryPath = assetsRootDirectoryPath;
                return this;
            }

            public IBuilder WithCustomGameLoopSteps(IReadOnlyList<string> customGameLoopSteps)
            {
                _customGameLoopSteps = customGameLoopSteps;
                return this;
            }

            public IBuilder WithFixedUpdatesPerFrameLimit(int fixedUpdatesPerFrameLimit)
            {
                _fixedUpdatesPerFrameLimit = fixedUpdatesPerFrameLimit;
                return this;
            }

            public IBuilder WithFixedUpdatesPerSecond(int fixedUpdatesPerSecond)
            {
                _fixedUpdatesPerSecond = fixedUpdatesPerSecond;
                return this;
            }

            public IBuilder WithLogLevel(LogLevel level)
            {
                _logLevel = level;
                return this;
            }

            public IBuilder WithShowAllEntitiesCount(bool showAllEntitiesCount)
            {
                _showAllEntitiesCount = showAllEntitiesCount;
                return this;
            }

            public IBuilder WithShowRootEntitiesCount(bool showRootEntitiesCount)
            {
                _showRootEntitiesCount = showRootEntitiesCount;
                return this;
            }

            public IBuilder WithShowFps(bool showFps)
            {
                _showFps = showFps;
                return this;
            }

            public IBuilder WithShowFrameTime(bool showFrameTime)
            {
                _showFrameTime = showFrameTime;
                return this;
            }

            public IBuilder WithShowGameLoopStatistics(bool showGameLoopStatistics)
            {
                _showGameLoopStatistics = showGameLoopStatistics;
                return this;
            }

            public IBuilder WithShowTotalFrames(bool showTotalFrames)
            {
                _showTotalFrames = showTotalFrames;
                return this;
            }

            public IBuilder WithShowTotalTime(bool showTotalTime)
            {
                _showTotalTime = showTotalTime;
                return this;
            }

            public IBuilder WithStartUpScene(string startUpScene)
            {
                _startUpScene = startUpScene;
                return this;
            }

            public IBuilder WithStartUpSceneBehavior(string startUpSceneBehavior)
            {
                _startUpSceneBehavior = startUpSceneBehavior;
                return this;
            }

            public CoreConfiguration Build() => new(
                _assetsRootDirectoryPath,
                _customGameLoopSteps,
                _fixedUpdatesPerFrameLimit,
                _fixedUpdatesPerSecond,
                _logLevel,
                _showAllEntitiesCount,
                _showRootEntitiesCount,
                _showFps,
                _showFrameTime,
                _showGameLoopStatistics,
                _showTotalFrames,
                _showTotalTime,
                _startUpScene,
                _startUpSceneBehavior);
        }
    }
}