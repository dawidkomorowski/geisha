using System;
using System.IO;
using Geisha.Engine.Core.Logging;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests;

[TestFixture]
public class ConfigurationIntegrationTests
{
    [Test]
    public void LoadFromFile_ShouldCreateDefaultConfiguration_WhenConfigurationFileIsEmpty()
    {
        // Arrange
        var path = Utils.GetPathUnderTestDirectory(Path.Combine("Configuration", "empty-configuration.json"));

        // Act
        var configuration = Configuration.LoadFromFile(path);

        // Assert
        Assert.That(configuration.Audio.EnableSound, Is.True);
        Assert.That(configuration.Audio.Volume, Is.EqualTo(1.0));

        Assert.That(configuration.Core.AssetsRootDirectoryPath, Is.EqualTo("Assets"));
        Assert.That(configuration.Core.CustomGameLoopSteps, Is.EqualTo(Array.Empty<string>()));
        Assert.That(configuration.Core.EnableGCLogging, Is.False);
        Assert.That(configuration.Core.FixedUpdatesPerFrameLimit, Is.Zero);
        Assert.That(configuration.Core.FixedUpdatesPerSecond, Is.EqualTo(60));
        Assert.That(configuration.Core.LogLevel, Is.EqualTo(LogLevel.Info));
        Assert.That(configuration.Core.ShowAllEntitiesCount, Is.False);
        Assert.That(configuration.Core.ShowRootEntitiesCount, Is.False);
        Assert.That(configuration.Core.ShowFps, Is.False);
        Assert.That(configuration.Core.ShowFrameTime, Is.False);
        Assert.That(configuration.Core.ShowTotalFrames, Is.False);
        Assert.That(configuration.Core.ShowTotalTime, Is.False);
        Assert.That(configuration.Core.ShowGameLoopStatistics, Is.False);
        Assert.That(configuration.Core.StartUpScene, Is.Empty);
        Assert.That(configuration.Core.StartUpSceneBehavior, Is.Empty);

        Assert.That(configuration.Physics.Substeps, Is.EqualTo(1));
        Assert.That(configuration.Physics.VelocityIterations, Is.EqualTo(4));
        Assert.That(configuration.Physics.PositionIterations, Is.EqualTo(4));
        Assert.That(configuration.Physics.PenetrationTolerance, Is.EqualTo(0.01));
        Assert.That(configuration.Physics.TileSize, Is.EqualTo(new SizeD(1, 1)));
        Assert.That(configuration.Physics.RenderCollisionGeometry, Is.False);

        Assert.That(configuration.Rendering.EnableVSync, Is.False);
        Assert.That(configuration.Rendering.ScreenSize, Is.EqualTo(new Size(1280, 720)));
        Assert.That(configuration.Rendering.ShowRenderingStatistics, Is.False);
        Assert.That(configuration.Rendering.SortingLayersOrder, Is.EqualTo(new[] { RenderingConfiguration.DefaultSortingLayerName }));
    }

    [Test]
    public void LoadFromFile_ShouldCreateConfigurationCorrespondingToFileContent()
    {
        // Arrange
        var path = Utils.GetPathUnderTestDirectory(Path.Combine("Configuration", "full-configuration.json"));

        // Act
        var configuration = Configuration.LoadFromFile(path);

        // Assert
        Assert.That(configuration.Audio.EnableSound, Is.False);
        Assert.That(configuration.Audio.Volume, Is.EqualTo(0.5));

        Assert.That(configuration.Core.AssetsRootDirectoryPath, Is.EqualTo("Path to directory with assets"));
        Assert.That(configuration.Core.CustomGameLoopSteps, Is.EqualTo(new[] { "CustomStep1", "CustomStep2", "CustomStep3" }));
        Assert.That(configuration.Core.EnableGCLogging, Is.True);
        Assert.That(configuration.Core.FixedUpdatesPerFrameLimit, Is.EqualTo(123));
        Assert.That(configuration.Core.FixedUpdatesPerSecond, Is.EqualTo(456));
        Assert.That(configuration.Core.LogLevel, Is.EqualTo(LogLevel.Trace));
        Assert.That(configuration.Core.ShowAllEntitiesCount, Is.True);
        Assert.That(configuration.Core.ShowRootEntitiesCount, Is.True);
        Assert.That(configuration.Core.ShowFps, Is.True);
        Assert.That(configuration.Core.ShowFrameTime, Is.True);
        Assert.That(configuration.Core.ShowTotalFrames, Is.True);
        Assert.That(configuration.Core.ShowTotalTime, Is.True);
        Assert.That(configuration.Core.ShowGameLoopStatistics, Is.True);
        Assert.That(configuration.Core.StartUpScene, Is.EqualTo("Path to start up scene file"));
        Assert.That(configuration.Core.StartUpSceneBehavior, Is.EqualTo("Name of scene behavior for empty start up scene"));

        Assert.That(configuration.Physics.Substeps, Is.EqualTo(12));
        Assert.That(configuration.Physics.VelocityIterations, Is.EqualTo(34));
        Assert.That(configuration.Physics.PositionIterations, Is.EqualTo(56));
        Assert.That(configuration.Physics.PenetrationTolerance, Is.EqualTo(1.23));
        Assert.That(configuration.Physics.TileSize, Is.EqualTo(new SizeD(1.2, 3.4)));
        Assert.That(configuration.Physics.RenderCollisionGeometry, Is.True);

        Assert.That(configuration.Rendering.EnableVSync, Is.True);
        Assert.That(configuration.Rendering.ScreenSize, Is.EqualTo(new Size(3840, 2160)));
        Assert.That(configuration.Rendering.ShowRenderingStatistics, Is.True);
        Assert.That(configuration.Rendering.SortingLayersOrder, Is.EqualTo(new[] { "Layer1", "Layer2", "Layer3" }));
    }
}