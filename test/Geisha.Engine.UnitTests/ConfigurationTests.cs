using Geisha.Engine.Core.Logging;
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

            Assert.That(actual.Physics.Substeps, Is.EqualTo(12));
            Assert.That(actual.Physics.RenderCollisionGeometry, Is.True);

            Assert.That(actual.Rendering.EnableVSync, Is.True);
            Assert.That(actual.Rendering.ScreenHeight, Is.EqualTo(2160));
            Assert.That(actual.Rendering.ScreenWidth, Is.EqualTo(3840));
            Assert.That(actual.Rendering.ShowRenderingStatistics, Is.True);
            Assert.That(actual.Rendering.SortingLayersOrder, Is.EqualTo(new[] { "Layer1", "Layer2", "Layer3" }));
        }

        private sealed class ConfigurationTestGame : Game
        {
            public override Configuration Configure(Configuration configuration) => configuration with
            {
                Audio = configuration.Audio with
                {
                    EnableSound = false,
                    Volume = 0.5
                },
                Core = configuration.Core with
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
                },
                Physics = configuration.Physics with
                {
                    Substeps = 12,
                    RenderCollisionGeometry = true
                },
                Rendering = configuration.Rendering with
                {
                    EnableVSync = true,
                    ScreenHeight = 2160,
                    ScreenWidth = 3840,
                    ShowRenderingStatistics = true,
                    SortingLayersOrder = new[] { "Layer1", "Layer2", "Layer3" }
                }
            };
        }
    }
}