﻿using System.Collections.Generic;

namespace Geisha.Engine.Core.Configuration
{
    /// <summary>
    ///     Configuration of engine core.
    /// </summary>
    public sealed class CoreConfiguration
    {
        public static IBuilder CreateBuilder() => new Builder();

        private CoreConfiguration(
            string assetsRootDirectoryPath,
            IReadOnlyList<string> customSystemsExecutionOrder,
            int fixedUpdatesPerFrameLimit,
            int fixedUpdatesPerSecond,
            bool showAllEntitiesCount,
            bool showRootEntitiesCount,
            bool showFps,
            bool showFrameTime,
            bool showTotalFrames,
            bool showTotalTime,
            bool showSystemsExecutionTimes,
            string startUpScene)
        {
            AssetsRootDirectoryPath = assetsRootDirectoryPath;
            CustomSystemsExecutionOrder = customSystemsExecutionOrder;
            FixedUpdatesPerFrameLimit = fixedUpdatesPerFrameLimit;
            FixedUpdatesPerSecond = fixedUpdatesPerSecond;
            ShowAllEntitiesCount = showAllEntitiesCount;
            ShowRootEntitiesCount = showRootEntitiesCount;
            ShowFps = showFps;
            ShowFrameTime = showFrameTime;
            ShowTotalFrames = showTotalFrames;
            ShowTotalTime = showTotalTime;
            ShowSystemsExecutionTimes = showSystemsExecutionTimes;
            StartUpScene = startUpScene;
        }

        /// <summary>
        ///     Specifies path to root directory for assets discovery and registration. Default is <c>"Assets"</c>.
        /// </summary>
        public string AssetsRootDirectoryPath { get; }

        /// <summary>
        ///     Specifies execution order of custom systems that first system in the list is executed first, last system in the
        ///     list is executed last. Default is empty list.
        /// </summary>
        public IReadOnlyList<string> CustomSystemsExecutionOrder { get; }

        /// <summary>
        ///     Specifies maximum number of fixed updates per single frame. Value <c>0</c> means unlimited. Default is <c>0</c>.
        /// </summary>
        public int FixedUpdatesPerFrameLimit { get; }

        /// <summary>
        ///     Target number of fixed updates to be performed in a single second. Default is <c>60</c>.
        /// </summary>
        public int FixedUpdatesPerSecond { get; }

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
        ///     Specifies whether to display total number of frames executed since engine start-up. Default is <c>false</c>.
        /// </summary>
        public bool ShowTotalFrames { get; }

        /// <summary>
        ///     Specifies whether to display total time that has passed since engine start-up. Default is <c>false</c>.
        /// </summary>
        public bool ShowTotalTime { get; }

        /// <summary>
        ///     Specifies whether to display diagnostic info about systems execution time and frame time share. Default is
        ///     <c>false</c>.
        /// </summary>
        public bool ShowSystemsExecutionTimes { get; }

        /// <summary>
        ///     Path to scene file that is loaded and started at engine start-up. Default is <c>""</c>.
        /// </summary>
        public string StartUpScene { get; }

        public interface IBuilder
        {
            IBuilder WithAssetsRootDirectoryPath(string assetsRootDirectoryPath);
            IBuilder WithCustomSystemsExecutionOrder(IReadOnlyList<string> customSystemsExecutionOrder);
            IBuilder WithFixedUpdatesPerFrameLimit(int fixedUpdatesPerFrameLimit);
            IBuilder WithFixedUpdatesPerSecond(int fixedUpdatesPerSecond);
            IBuilder WithShowAllEntitiesCount(bool showAllEntitiesCount);
            IBuilder WithShowRootEntitiesCount(bool showRootEntitiesCount);
            IBuilder WithShowFps(bool showFps);
            IBuilder WithShowFrameTime(bool showFrameTime);
            IBuilder WithShowTotalFrames(bool showTotalFrames);
            IBuilder WithShowTotalTime(bool showTotalTime);
            IBuilder WithShowSystemsExecutionTimes(bool showSystemsExecutionTimes);
            IBuilder WithStartUpScene(string startUpScene);
            CoreConfiguration Build();
        }

        private sealed class Builder : IBuilder
        {
            private string _assetsRootDirectoryPath = "Assets";
            private IReadOnlyList<string> _customSystemsExecutionOrder = new List<string>().AsReadOnly();
            private int _fixedUpdatesPerFrameLimit = 0;
            private int _fixedUpdatesPerSecond = 60;
            private bool _showAllEntitiesCount = false;
            private bool _showRootEntitiesCount = false;
            private bool _showFps = false;
            private bool _showFrameTime = false;
            private bool _showTotalFrames = false;
            private bool _showTotalTime = false;
            private bool _showSystemsExecutionTimes = false;
            private string _startUpScene = string.Empty;

            public IBuilder WithAssetsRootDirectoryPath(string assetsRootDirectoryPath)
            {
                _assetsRootDirectoryPath = assetsRootDirectoryPath;
                return this;
            }

            public IBuilder WithCustomSystemsExecutionOrder(IReadOnlyList<string> customSystemsExecutionOrder)
            {
                _customSystemsExecutionOrder = customSystemsExecutionOrder;
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

            public IBuilder WithShowSystemsExecutionTimes(bool showSystemsExecutionTimes)
            {
                _showSystemsExecutionTimes = showSystemsExecutionTimes;
                return this;
            }

            public IBuilder WithStartUpScene(string startUpScene)
            {
                _startUpScene = startUpScene;
                return this;
            }

            public CoreConfiguration Build() => new CoreConfiguration(
                assetsRootDirectoryPath: _assetsRootDirectoryPath,
                customSystemsExecutionOrder: _customSystemsExecutionOrder,
                fixedUpdatesPerFrameLimit: _fixedUpdatesPerFrameLimit,
                fixedUpdatesPerSecond: _fixedUpdatesPerSecond,
                showAllEntitiesCount: _showAllEntitiesCount,
                showRootEntitiesCount: _showRootEntitiesCount,
                showFps: _showFps,
                showFrameTime: _showFrameTime,
                showTotalFrames: _showTotalFrames,
                showTotalTime: _showTotalTime,
                showSystemsExecutionTimes: _showSystemsExecutionTimes,
                startUpScene: _startUpScene);
        }
    }
}