﻿using Geisha.Engine.Audio;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Logging;
using Geisha.Engine.Physics;
using Geisha.Engine.Rendering;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests
{
    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        public void Overwrite_ShouldCreateNewConfigurationOverwrittenByGame()
        {
            // Arrange
            var configuration = Configuration.CreateDefault();
            var game = new ConfigurationTestGame();

            // Act
            var actual = configuration.Overwrite(game);

            // Assert
            Assert.That(actual.Audio.EnableSound, Is.False);
            Assert.That(actual.Audio.Volume, Is.EqualTo(0.5));

            Assert.That(actual.Core.AssetsRootDirectoryPath, Is.EqualTo("Path to directory with assets"));
            Assert.That(actual.Core.CustomGameLoopSteps, Is.EqualTo(new[] { "CustomStep1", "CustomStep2", "CustomStep3" }));
            Assert.That(actual.Core.FixedUpdatesPerFrameLimit, Is.EqualTo(123));
            Assert.That(actual.Core.FixedUpdatesPerSecond, Is.EqualTo(456));
            Assert.That(actual.Core.LogLevel, Is.EqualTo(LogLevel.Trace));
            Assert.That(actual.Core.ShowAllEntitiesCount, Is.True);
            Assert.That(actual.Core.ShowRootEntitiesCount, Is.True);
            Assert.That(actual.Core.ShowFps, Is.True);
            Assert.That(actual.Core.ShowFrameTime, Is.True);
            Assert.That(actual.Core.ShowTotalFrames, Is.True);
            Assert.That(actual.Core.ShowTotalTime, Is.True);
            Assert.That(actual.Core.ShowGameLoopStatistics, Is.True);
            Assert.That(actual.Core.StartUpScene, Is.EqualTo("Path to start up scene file"));
            Assert.That(actual.Core.StartUpSceneBehavior, Is.EqualTo("Name of scene behavior for empty start up scene"));

            Assert.That(actual.Physics.RenderCollisionGeometry, Is.True);

            Assert.That(actual.Rendering.EnableVSync, Is.True);
            Assert.That(actual.Rendering.ScreenHeight, Is.EqualTo(2160));
            Assert.That(actual.Rendering.ScreenWidth, Is.EqualTo(3840));
            Assert.That(actual.Rendering.ShowRenderingStatistics, Is.True);
            Assert.That(actual.Rendering.SortingLayersOrder, Is.EqualTo(new[] { "Layer1", "Layer2", "Layer3" }));
        }

        private sealed class ConfigurationTestGame : Game
        {
            public override AudioConfiguration ConfigureAudio(AudioConfiguration configuration) =>
                base.ConfigureAudio(configuration) with
                {
                    EnableSound = false,
                    Volume = 0.5
                };

            public override CoreConfiguration ConfigureCore(CoreConfiguration configuration) =>
                base.ConfigureCore(configuration) with
                {
                    AssetsRootDirectoryPath = "Path to directory with assets",
                    CustomGameLoopSteps = new[] { "CustomStep1", "CustomStep2", "CustomStep3" },
                    FixedUpdatesPerFrameLimit = 123,
                    FixedUpdatesPerSecond = 456,
                    LogLevel = LogLevel.Trace,
                    ShowAllEntitiesCount = true,
                    ShowRootEntitiesCount = true,
                    ShowFps = true,
                    ShowFrameTime = true,
                    ShowTotalFrames = true,
                    ShowTotalTime = true,
                    ShowGameLoopStatistics = true,
                    StartUpScene = "Path to start up scene file",
                    StartUpSceneBehavior = "Name of scene behavior for empty start up scene"
                };

            public override PhysicsConfiguration ConfigurePhysics(PhysicsConfiguration configuration) =>
                base.ConfigurePhysics(configuration) with { RenderCollisionGeometry = true };

            public override RenderingConfiguration ConfigureRendering(RenderingConfiguration configuration) => base.ConfigureRendering(configuration) with
            {
                EnableVSync = true,
                ScreenHeight = 2160,
                ScreenWidth = 3840,
                ShowRenderingStatistics = true,
                SortingLayersOrder = new[] { "Layer1", "Layer2", "Layer3" }
            };
        }
    }
}