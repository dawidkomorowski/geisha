using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Geisha.Engine.Audio;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Logging;
using Geisha.Engine.Physics;
using Geisha.Engine.Rendering;

namespace Geisha.Engine
{
    /// <summary>
    ///     Provides access to engine configuration.
    /// </summary>
    public sealed class Configuration
    {
        private Configuration(
            AudioConfiguration audio,
            CoreConfiguration coreConfiguration,
            PhysicsConfiguration physics,
            RenderingConfiguration renderingConfiguration)
        {
            Audio = audio;
            Core = coreConfiguration;
            Physics = physics;
            Rendering = renderingConfiguration;
        }

        /// <summary>
        ///     <see cref="AudioConfiguration" /> loaded from Audio configuration section.
        /// </summary>
        public AudioConfiguration Audio { get; }

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
        ///     Creates default configuration.
        /// </summary>
        /// <returns>New instance of <see cref="Configuration" /> class with default data.</returns>
        public static Configuration CreateDefault() =>
            new(
                new AudioConfiguration(),
                new CoreConfiguration(),
                new PhysicsConfiguration(),
                new RenderingConfiguration()
            );

        /// <summary>
        ///     Loads configuration from specified file.
        /// </summary>
        /// <param name="path">Path to configuration file.</param>
        /// <returns>New instance of <see cref="Configuration" /> class with data loaded from file or defaults if data was missing.</returns>
        public static Configuration LoadFromFile(string path)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            var rawFileContent = File.ReadAllText(path);
            var fileContent = JsonSerializer.Deserialize<FileContent>(rawFileContent, jsonSerializerOptions);

            if (fileContent is null) throw new InvalidOperationException($"Cannot load configuration from file: {path}.");

            var audioConfiguration = new AudioConfiguration();
            if (fileContent.Audio?.EnableSound != null)
                audioConfiguration = audioConfiguration with { EnableSound = fileContent.Audio.EnableSound.Value };
            if (fileContent.Audio?.Volume != null)
                audioConfiguration = audioConfiguration with { Volume = fileContent.Audio.Volume.Value };

            var coreConfiguration = new CoreConfiguration();
            if (fileContent.Core?.AssetsRootDirectoryPath != null)
                coreConfiguration = coreConfiguration with { AssetsRootDirectoryPath = fileContent.Core.AssetsRootDirectoryPath };
            if (fileContent.Core?.CustomGameLoopSteps != null)
                coreConfiguration = coreConfiguration with { CustomGameLoopSteps = fileContent.Core.CustomGameLoopSteps };
            if (fileContent.Core?.FixedUpdatesPerFrameLimit != null)
                coreConfiguration = coreConfiguration with { FixedUpdatesPerFrameLimit = fileContent.Core.FixedUpdatesPerFrameLimit.Value };
            if (fileContent.Core?.FixedUpdatesPerSecond != null)
                coreConfiguration = coreConfiguration with { FixedUpdatesPerSecond = fileContent.Core.FixedUpdatesPerSecond.Value };
            if (fileContent.Core?.LogLevel is not null)
                coreConfiguration = coreConfiguration with { LogLevel = fileContent.Core.LogLevel.Value };
            if (fileContent.Core?.ShowAllEntitiesCount != null)
                coreConfiguration = coreConfiguration with { ShowAllEntitiesCount = fileContent.Core.ShowAllEntitiesCount.Value };
            if (fileContent.Core?.ShowRootEntitiesCount != null)
                coreConfiguration = coreConfiguration with { ShowRootEntitiesCount = fileContent.Core.ShowRootEntitiesCount.Value };
            if (fileContent.Core?.ShowFps != null)
                coreConfiguration = coreConfiguration with { ShowFps = fileContent.Core.ShowFps.Value };
            if (fileContent.Core?.ShowFrameTime != null)
                coreConfiguration = coreConfiguration with { ShowFrameTime = fileContent.Core.ShowFrameTime.Value };
            if (fileContent.Core?.ShowGameLoopStatistics != null)
                coreConfiguration = coreConfiguration with { ShowGameLoopStatistics = fileContent.Core.ShowGameLoopStatistics.Value };
            if (fileContent.Core?.ShowTotalFrames != null)
                coreConfiguration = coreConfiguration with { ShowTotalFrames = fileContent.Core.ShowTotalFrames.Value };
            if (fileContent.Core?.ShowTotalTime != null)
                coreConfiguration = coreConfiguration with { ShowTotalTime = fileContent.Core.ShowTotalTime.Value };
            if (fileContent.Core?.StartUpScene != null)
                coreConfiguration = coreConfiguration with { StartUpScene = fileContent.Core.StartUpScene };
            if (fileContent.Core?.StartUpSceneBehavior != null)
                coreConfiguration = coreConfiguration with { StartUpSceneBehavior = fileContent.Core.StartUpSceneBehavior };

            var physicsConfiguration = new PhysicsConfiguration();
            if (fileContent.Physics?.RenderCollisionGeometry != null)
                physicsConfiguration = physicsConfiguration with { RenderCollisionGeometry = fileContent.Physics.RenderCollisionGeometry.Value };

            var renderingConfiguration = new RenderingConfiguration();
            if (fileContent.Rendering?.EnableVSync != null)
                renderingConfiguration = renderingConfiguration with { EnableVSync = fileContent.Rendering.EnableVSync.Value };
            if (fileContent.Rendering?.ScreenHeight != null)
                renderingConfiguration = renderingConfiguration with { ScreenHeight = fileContent.Rendering.ScreenHeight.Value };
            if (fileContent.Rendering?.ScreenWidth != null)
                renderingConfiguration = renderingConfiguration with { ScreenWidth = fileContent.Rendering.ScreenWidth.Value };
            if (fileContent.Rendering?.ShowRenderingStatistics != null)
                renderingConfiguration = renderingConfiguration with { ShowRenderingStatistics = fileContent.Rendering.ShowRenderingStatistics.Value };
            if (fileContent.Rendering?.SortingLayersOrder != null)
                renderingConfiguration = renderingConfiguration with { SortingLayersOrder = fileContent.Rendering.SortingLayersOrder };

            return new Configuration(
                audioConfiguration,
                coreConfiguration,
                physicsConfiguration,
                renderingConfiguration
            );
        }

        /// <summary>
        ///     Creates new instance of <see cref="Configuration" /> with data of this <see cref="Configuration" /> overwritten by
        ///     <paramref name="game" />.
        /// </summary>
        /// <param name="game"><see cref="Game" /> instance used to overwrite the configuration.</param>
        /// <returns>
        ///     New instance of <see cref="Configuration" /> with data of this <see cref="Configuration" /> overwritten by
        ///     <paramref name="game" />.
        /// </returns>
        public Configuration Overwrite(Game game) =>
            new(
                game.ConfigureAudio(Audio),
                game.ConfigureCore(Core),
                game.ConfigurePhysics(Physics),
                game.ConfigureRendering(Rendering)
            );

        private sealed class FileContent
        {
            public AudioSection? Audio { get; set; }
            public CoreSection? Core { get; set; }
            public PhysicsSection? Physics { get; set; }
            public RenderingSection? Rendering { get; set; }
        }

        private sealed class AudioSection
        {
            public bool? EnableSound { get; set; }
            public double? Volume { get; set; }
        }

        private sealed class CoreSection
        {
            public string? AssetsRootDirectoryPath { get; set; }
            public string[]? CustomGameLoopSteps { get; set; }
            public int? FixedUpdatesPerFrameLimit { get; set; }
            public int? FixedUpdatesPerSecond { get; set; }

            [JsonConverter(typeof(JsonStringEnumConverter))]
            public LogLevel? LogLevel { get; set; }

            public bool? ShowAllEntitiesCount { get; set; }
            public bool? ShowRootEntitiesCount { get; set; }
            public bool? ShowFps { get; set; }
            public bool? ShowFrameTime { get; set; }
            public bool? ShowGameLoopStatistics { get; set; }
            public bool? ShowTotalFrames { get; set; }
            public bool? ShowTotalTime { get; set; }
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
            public bool? ShowRenderingStatistics { get; set; }
            public string[]? SortingLayersOrder { get; set; }
        }
    }
}