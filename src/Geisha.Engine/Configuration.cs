﻿using System;
using System.IO;
using System.Text.Json;
using Geisha.Engine.Core;
using Geisha.Engine.Physics;
using Geisha.Engine.Rendering;

namespace Geisha.Engine
{
    /// <summary>
    ///     Provides access to engine configuration.
    /// </summary>
    public sealed class Configuration
    {
        private Configuration(CoreConfiguration coreConfiguration, PhysicsConfiguration physics, RenderingConfiguration renderingConfiguration)
        {
            Core = coreConfiguration;
            Physics = physics;
            Rendering = renderingConfiguration;
        }

        /// <summary>
        ///     <see cref="CoreConfiguration" /> loaded from Core configuration section.
        /// </summary>
        public CoreConfiguration Core { get; }

        /// <summary>
        ///     <see cref="PhysicsConfiguration" /> loaded from Physics configuration section.
        /// </summary>
        public PhysicsConfiguration Physics { get; }

        /// <summary>
        ///     <see cref="RenderingConfiguration" /> loaded from Rendering configuration section.
        /// </summary>
        public RenderingConfiguration Rendering { get; }

        /// <summary>
        ///     Loads configuration from specified file.
        /// </summary>
        /// <param name="path">Path to configuration file.</param>
        /// <returns>New instance of <see cref="Configuration" /> class with data loaded from file or defaults if data was missing.</returns>
        public static Configuration LoadFromFile(string path)
        {
            var rawFileContent = File.ReadAllText(path);
            var fileContent = JsonSerializer.Deserialize<FileContent>(rawFileContent);

            if (fileContent is null) throw new InvalidOperationException($"Cannot load configuration from file: {path}.");

            var coreConfigurationBuilder = CoreConfiguration.CreateBuilder();
            if (fileContent.Core?.AssetsRootDirectoryPath != null)
                coreConfigurationBuilder.WithAssetsRootDirectoryPath(fileContent.Core.AssetsRootDirectoryPath);
            if (fileContent.Core?.CustomSystemsExecutionOrder != null)
                coreConfigurationBuilder.WithCustomSystemsExecutionOrder(fileContent.Core.CustomSystemsExecutionOrder);
            if (fileContent.Core?.FixedUpdatesPerFrameLimit != null)
                coreConfigurationBuilder.WithFixedUpdatesPerFrameLimit(fileContent.Core.FixedUpdatesPerFrameLimit.Value);
            if (fileContent.Core?.FixedUpdatesPerSecond != null)
                coreConfigurationBuilder.WithFixedUpdatesPerSecond(fileContent.Core.FixedUpdatesPerSecond.Value);
            if (fileContent.Core?.ShowAllEntitiesCount != null)
                coreConfigurationBuilder.WithShowAllEntitiesCount(fileContent.Core.ShowAllEntitiesCount.Value);
            if (fileContent.Core?.ShowRootEntitiesCount != null)
                coreConfigurationBuilder.WithShowRootEntitiesCount(fileContent.Core.ShowRootEntitiesCount.Value);
            if (fileContent.Core?.ShowFps != null)
                coreConfigurationBuilder.WithShowFps(fileContent.Core.ShowFps.Value);
            if (fileContent.Core?.ShowFrameTime != null)
                coreConfigurationBuilder.WithShowFrameTime(fileContent.Core.ShowFrameTime.Value);
            if (fileContent.Core?.ShowTotalFrames != null)
                coreConfigurationBuilder.WithShowTotalFrames(fileContent.Core.ShowTotalFrames.Value);
            if (fileContent.Core?.ShowTotalTime != null)
                coreConfigurationBuilder.WithShowTotalTime(fileContent.Core.ShowTotalTime.Value);
            if (fileContent.Core?.ShowSystemsExecutionTimes != null)
                coreConfigurationBuilder.WithShowSystemsExecutionTimes(fileContent.Core.ShowSystemsExecutionTimes.Value);
            if (fileContent.Core?.StartUpScene != null)
                coreConfigurationBuilder.WithStartUpScene(fileContent.Core.StartUpScene);
            if (fileContent.Core?.StartUpSceneBehavior != null)
                coreConfigurationBuilder.WithStartUpSceneBehavior(fileContent.Core.StartUpSceneBehavior);

            var physicsConfigurationBuilder = PhysicsConfiguration.CreateBuilder();
            if (fileContent.Physics?.RenderCollisionGeometry != null)
                physicsConfigurationBuilder.WithRenderCollisionGeometry(fileContent.Physics.RenderCollisionGeometry.Value);

            var renderingConfigurationBuilder = RenderingConfiguration.CreateBuilder();
            if (fileContent.Rendering?.EnableVSync != null)
                renderingConfigurationBuilder.WithEnableVSync(fileContent.Rendering.EnableVSync.Value);
            if (fileContent.Rendering?.ScreenHeight != null)
                renderingConfigurationBuilder.WithScreenHeight(fileContent.Rendering.ScreenHeight.Value);
            if (fileContent.Rendering?.ScreenWidth != null)
                renderingConfigurationBuilder.WithScreenWidth(fileContent.Rendering.ScreenWidth.Value);
            if (fileContent.Rendering?.SortingLayersOrder != null)
                renderingConfigurationBuilder.WithSortingLayersOrder(fileContent.Rendering.SortingLayersOrder);

            return new Configuration(
                coreConfigurationBuilder.Build(),
                physicsConfigurationBuilder.Build(),
                renderingConfigurationBuilder.Build()
            );
        }

        private sealed class FileContent
        {
            public CoreSection? Core { get; set; }
            public PhysicsSection? Physics { get; set; }
            public RenderingSection? Rendering { get; set; }
        }

        private sealed class CoreSection
        {
            public string? AssetsRootDirectoryPath { get; set; }
            public string[]? CustomSystemsExecutionOrder { get; set; }
            public int? FixedUpdatesPerFrameLimit { get; set; }
            public int? FixedUpdatesPerSecond { get; set; }
            public bool? ShowAllEntitiesCount { get; set; }
            public bool? ShowRootEntitiesCount { get; set; }
            public bool? ShowFps { get; set; }
            public bool? ShowFrameTime { get; set; }
            public bool? ShowTotalFrames { get; set; }
            public bool? ShowTotalTime { get; set; }
            public bool? ShowSystemsExecutionTimes { get; set; }
            public string? StartUpScene { get; set; }
            public string? StartUpSceneBehavior { get; set; }
        }

        private sealed class PhysicsSection
        {
            public bool? RenderCollisionGeometry { get; set; }
        }

        private sealed class RenderingSection
        {
            public bool? EnableVSync { get; set; }
            public int? ScreenHeight { get; set; }
            public int? ScreenWidth { get; set; }
            public string[]? SortingLayersOrder { get; set; }
        }
    }
}