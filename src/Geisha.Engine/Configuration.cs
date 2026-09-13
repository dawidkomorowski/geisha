using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Geisha.Engine.Audio;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Logging;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics;
using Geisha.Engine.Rendering;
using Geisha.Engine.Windowing;

namespace Geisha.Engine;

/// <summary>
///     Provides access to engine configuration.
/// </summary>
public sealed record Configuration
{
    private Configuration()
    {
        Audio = new AudioConfiguration();
        Core = new CoreConfiguration();
        Physics = new PhysicsConfiguration();
        Rendering = new RenderingConfiguration();
        Windowing = new WindowingConfiguration();
    }

    /// <summary>
    ///     <see cref="AudioConfiguration" /> loaded from Audio configuration section.
    /// </summary>
    public AudioConfiguration Audio { get; init; }

    /// <summary>
    ///     <see cref="CoreConfiguration" /> loaded from Core configuration section.
    /// </summary>
    public CoreConfiguration Core { get; init; }

    /// <summary>
    ///     <see cref="PhysicsConfiguration" /> loaded from Physics configuration section.
    /// </summary>
    public PhysicsConfiguration Physics { get; init; }

    /// <summary>
    ///     <see cref="RenderingConfiguration" /> loaded from Rendering configuration section.
    /// </summary>
    public RenderingConfiguration Rendering { get; init; }

    // TODO: Add documentation.
    public WindowingConfiguration Windowing { get; init; }

    /// <summary>
    ///     Creates default configuration.
    /// </summary>
    /// <returns>New instance of <see cref="Configuration" /> class with default data.</returns>
    public static Configuration CreateDefault() => new();

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
        if (fileContent.Audio?.EnableSound is not null)
            audioConfiguration = audioConfiguration with { EnableSound = fileContent.Audio.EnableSound.Value };
        if (fileContent.Audio?.Volume is not null)
            audioConfiguration = audioConfiguration with { Volume = fileContent.Audio.Volume.Value };

        var coreConfiguration = new CoreConfiguration();
        if (fileContent.Core?.AssetsRootDirectoryPath is not null)
            coreConfiguration = coreConfiguration with { AssetsRootDirectoryPath = fileContent.Core.AssetsRootDirectoryPath };
        if (fileContent.Core?.CustomGameLoopSteps is not null)
            coreConfiguration = coreConfiguration with { CustomGameLoopSteps = fileContent.Core.CustomGameLoopSteps };
        if (fileContent.Core?.EnableGCLogging is not null)
            coreConfiguration = coreConfiguration with { EnableGCLogging = fileContent.Core.EnableGCLogging.Value };
        if (fileContent.Core?.FixedUpdatesPerFrameLimit is not null)
            coreConfiguration = coreConfiguration with { FixedUpdatesPerFrameLimit = fileContent.Core.FixedUpdatesPerFrameLimit.Value };
        if (fileContent.Core?.FixedUpdatesPerSecond is not null)
            coreConfiguration = coreConfiguration with { FixedUpdatesPerSecond = fileContent.Core.FixedUpdatesPerSecond.Value };
        if (fileContent.Core?.LogLevel is not null)
            coreConfiguration = coreConfiguration with { LogLevel = fileContent.Core.LogLevel.Value };
        if (fileContent.Core?.ShowAllEntitiesCount is not null)
            coreConfiguration = coreConfiguration with { ShowAllEntitiesCount = fileContent.Core.ShowAllEntitiesCount.Value };
        if (fileContent.Core?.ShowRootEntitiesCount is not null)
            coreConfiguration = coreConfiguration with { ShowRootEntitiesCount = fileContent.Core.ShowRootEntitiesCount.Value };
        if (fileContent.Core?.ShowFps is not null)
            coreConfiguration = coreConfiguration with { ShowFps = fileContent.Core.ShowFps.Value };
        if (fileContent.Core?.ShowFrameTime is not null)
            coreConfiguration = coreConfiguration with { ShowFrameTime = fileContent.Core.ShowFrameTime.Value };
        if (fileContent.Core?.ShowGameLoopStatistics is not null)
            coreConfiguration = coreConfiguration with { ShowGameLoopStatistics = fileContent.Core.ShowGameLoopStatistics.Value };
        if (fileContent.Core?.ShowTotalFrames is not null)
            coreConfiguration = coreConfiguration with { ShowTotalFrames = fileContent.Core.ShowTotalFrames.Value };
        if (fileContent.Core?.ShowTotalTime is not null)
            coreConfiguration = coreConfiguration with { ShowTotalTime = fileContent.Core.ShowTotalTime.Value };
        if (fileContent.Core?.StartUpScene is not null)
            coreConfiguration = coreConfiguration with { StartUpScene = fileContent.Core.StartUpScene };
        if (fileContent.Core?.StartUpSceneBehavior is not null)
            coreConfiguration = coreConfiguration with { StartUpSceneBehavior = fileContent.Core.StartUpSceneBehavior };

        var physicsConfiguration = new PhysicsConfiguration();
        if (fileContent.Physics?.Substeps is not null)
            physicsConfiguration = physicsConfiguration with { Substeps = fileContent.Physics.Substeps.Value };
        if (fileContent.Physics?.VelocityIterations is not null)
            physicsConfiguration = physicsConfiguration with { VelocityIterations = fileContent.Physics.VelocityIterations.Value };
        if (fileContent.Physics?.PositionIterations is not null)
            physicsConfiguration = physicsConfiguration with { PositionIterations = fileContent.Physics.PositionIterations.Value };
        if (fileContent.Physics?.PenetrationTolerance is not null)
            physicsConfiguration = physicsConfiguration with { PenetrationTolerance = fileContent.Physics.PenetrationTolerance.Value };
        if (fileContent.Physics?.TileSize is not null)
            physicsConfiguration = physicsConfiguration with { TileSize = fileContent.Physics.TileSize.Value };
        if (fileContent.Physics?.BroadPhaseGridCellSize is not null)
            physicsConfiguration = physicsConfiguration with { BroadPhaseGridCellSize = fileContent.Physics.BroadPhaseGridCellSize.Value };
        if (fileContent.Physics?.EnableDebugRendering is not null)
            physicsConfiguration = physicsConfiguration with { EnableDebugRendering = fileContent.Physics.EnableDebugRendering.Value };

        var renderingConfiguration = new RenderingConfiguration();
        if (fileContent.Rendering?.AllowWindowResizing is not null)
            renderingConfiguration = renderingConfiguration with { AllowWindowResizing = fileContent.Rendering.AllowWindowResizing.Value };
        if (fileContent.Rendering?.EnableVSync is not null)
            renderingConfiguration = renderingConfiguration with { EnableVSync = fileContent.Rendering.EnableVSync.Value };
        if (fileContent.Rendering?.ScreenSize is not null)
            renderingConfiguration = renderingConfiguration with { ScreenSize = fileContent.Rendering.ScreenSize.Value };
        if (fileContent.Rendering?.ShowRenderingStatistics is not null)
            renderingConfiguration = renderingConfiguration with { ShowRenderingStatistics = fileContent.Rendering.ShowRenderingStatistics.Value };
        if (fileContent.Rendering?.SortingLayersOrder is not null)
            renderingConfiguration = renderingConfiguration with { SortingLayersOrder = fileContent.Rendering.SortingLayersOrder };

        var windowingConfiguration = new WindowingConfiguration();
        if (fileContent.Windowing?.AllowWindowResizing is not null)
            windowingConfiguration = windowingConfiguration with { AllowWindowResizing = fileContent.Windowing.AllowWindowResizing.Value };

        return new Configuration
        {
            Audio = audioConfiguration,
            Core = coreConfiguration,
            Physics = physicsConfiguration,
            Rendering = renderingConfiguration,
            Windowing = windowingConfiguration
        };
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
    public Configuration Overwrite(Game game) => game.Configure(this);

    private sealed record FileContent
    {
        public AudioSection? Audio { get; init; }
        public CoreSection? Core { get; init; }
        public PhysicsSection? Physics { get; init; }
        public RenderingSection? Rendering { get; init; }
        public WindowingSection? Windowing { get; init; }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private sealed record AudioSection
    {
        public bool? EnableSound { get; init; }
        public double? Volume { get; init; }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private sealed record CoreSection
    {
        public string? AssetsRootDirectoryPath { get; init; }
        public string[]? CustomGameLoopSteps { get; init; }
        public bool? EnableGCLogging { get; init; }
        public int? FixedUpdatesPerFrameLimit { get; init; }
        public int? FixedUpdatesPerSecond { get; init; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogLevel? LogLevel { get; init; }

        public bool? ShowAllEntitiesCount { get; init; }
        public bool? ShowRootEntitiesCount { get; init; }
        public bool? ShowFps { get; init; }
        public bool? ShowFrameTime { get; init; }
        public bool? ShowGameLoopStatistics { get; init; }
        public bool? ShowTotalFrames { get; init; }
        public bool? ShowTotalTime { get; init; }
        public string? StartUpScene { get; init; }
        public string? StartUpSceneBehavior { get; init; }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private sealed record PhysicsSection
    {
        public int? Substeps { get; init; }
        public int? VelocityIterations { get; init; }
        public int? PositionIterations { get; init; }
        public double? PenetrationTolerance { get; init; }
        public SizeD? TileSize { get; init; }
        public SizeD? BroadPhaseGridCellSize { get; init; }
        public bool? EnableDebugRendering { get; init; }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private sealed record RenderingSection
    {
        public bool? AllowWindowResizing { get; init; }
        public bool? EnableVSync { get; init; }
        public Size? ScreenSize { get; init; }
        public bool? ShowRenderingStatistics { get; init; }
        public string[]? SortingLayersOrder { get; init; }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private sealed record WindowingSection
    {
        public bool? AllowWindowResizing { get; init; }
    }
}