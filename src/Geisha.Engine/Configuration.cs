﻿using System.IO;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Rendering.Configuration;

namespace Geisha.Engine
{
    public sealed class Configuration
    {
        private Configuration(CoreConfiguration coreConfiguration, RenderingConfiguration renderingConfiguration)
        {
            Core = coreConfiguration;
            Rendering = renderingConfiguration;
        }

        public static Configuration LoadFromFile(string path)
        {
            var rawFileContent = File.ReadAllText(path);

            var jsonSerializer = new JsonSerializer();
            var fileContent = jsonSerializer.Deserialize<FileContent>(rawFileContent);

            var coreConfigurationBuilder = CoreConfiguration.CreateBuilder();
            if (fileContent?.Core?.AssetsRootDirectoryPath != null)
                coreConfigurationBuilder.WithAssetsRootDirectoryPath(fileContent.Core.AssetsRootDirectoryPath);
            if (fileContent?.Core?.CustomSystemsExecutionOrder != null)
                coreConfigurationBuilder.WithCustomSystemsExecutionOrder(fileContent.Core.CustomSystemsExecutionOrder);
            if (fileContent?.Core?.FixedUpdatesPerFrameLimit != null)
                coreConfigurationBuilder.WithFixedUpdatesPerFrameLimit(fileContent.Core.FixedUpdatesPerFrameLimit.Value);
            if (fileContent?.Core?.FixedUpdatesPerSecond != null)
                coreConfigurationBuilder.WithFixedUpdatesPerSecond(fileContent.Core.FixedUpdatesPerSecond.Value);
            if (fileContent?.Core?.ShowAllEntitiesCount != null)
                coreConfigurationBuilder.WithShowAllEntitiesCount(fileContent.Core.ShowAllEntitiesCount.Value);
            if (fileContent?.Core?.ShowRootEntitiesCount != null)
                coreConfigurationBuilder.WithShowRootEntitiesCount(fileContent.Core.ShowRootEntitiesCount.Value);
            if (fileContent?.Core?.ShowFps != null)
                coreConfigurationBuilder.WithShowFps(fileContent.Core.ShowFps.Value);
            if (fileContent?.Core?.ShowFrameTime != null)
                coreConfigurationBuilder.WithShowFrameTime(fileContent.Core.ShowFrameTime.Value);
            if (fileContent?.Core?.ShowTotalFrames != null)
                coreConfigurationBuilder.WithShowTotalFrames(fileContent.Core.ShowTotalFrames.Value);
            if (fileContent?.Core?.ShowTotalTime != null)
                coreConfigurationBuilder.WithShowTotalTime(fileContent.Core.ShowTotalTime.Value);
            if (fileContent?.Core?.ShowSystemsExecutionTimes != null)
                coreConfigurationBuilder.WithShowSystemsExecutionTimes(fileContent.Core.ShowSystemsExecutionTimes.Value);
            if (fileContent?.Core?.StartUpScene != null)
                coreConfigurationBuilder.WithStartUpScene(fileContent.Core.StartUpScene);

            var renderingConfigurationBuilder = RenderingConfiguration.CreateBuilder();
            if (fileContent?.Rendering?.EnableVSync != null)
                renderingConfigurationBuilder.WithEnableVSync(fileContent.Rendering.EnableVSync.Value);
            if (fileContent?.Rendering?.SortingLayersOrder != null)
                renderingConfigurationBuilder.WithSortingLayersOrder(fileContent.Rendering.SortingLayersOrder);

            return new Configuration(coreConfigurationBuilder.Build(), renderingConfigurationBuilder.Build());
        }

        public CoreConfiguration Core { get; }
        public RenderingConfiguration Rendering { get; }

        private sealed class FileContent
        {
            public CoreSection? Core { get; set; }
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
        }

        private sealed class RenderingSection
        {
            public bool? EnableVSync { get; set; }
            public string[]? SortingLayersOrder { get; set; }
        }
    }
}