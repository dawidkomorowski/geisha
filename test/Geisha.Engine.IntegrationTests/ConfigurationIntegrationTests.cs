using System;
using System.IO;
using Geisha.Engine.Rendering.Configuration;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests
{
    [TestFixture]
    public class ConfigurationIntegrationTests
    {
        [Test]
        public void LoadFromFile_ShouldCreateConfigurationCorrespondingToFileContent()
        {
            // Arrange
            // Act
            var configuration = Configuration.LoadFromFile(Path.Combine("Configuration", "full-configuration.json"));

            // Assert
            Assert.That(configuration.Core.AssetsRootDirectoryPath, Is.EqualTo("Path to directory with assets"));
            Assert.That(configuration.Core.CustomSystemsExecutionOrder, Is.EqualTo(new[] {"CustomSystem1", "CustomSystem2", "CustomSystem3"}));
            Assert.That(configuration.Core.FixedUpdatesPerFrameLimit, Is.EqualTo(123));
            Assert.That(configuration.Core.FixedUpdatesPerSecond, Is.EqualTo(456));
            Assert.That(configuration.Core.ShowAllEntitiesCount, Is.True);
            Assert.That(configuration.Core.ShowRootEntitiesCount, Is.True);
            Assert.That(configuration.Core.ShowFps, Is.True);
            Assert.That(configuration.Core.ShowFrameTime, Is.True);
            Assert.That(configuration.Core.ShowTotalFrames, Is.True);
            Assert.That(configuration.Core.ShowTotalTime, Is.True);
            Assert.That(configuration.Core.ShowSystemsExecutionTimes, Is.True);
            Assert.That(configuration.Core.StartUpScene, Is.EqualTo("Path to start up scene file"));

            Assert.That(configuration.Rendering.EnableVSync, Is.True);
            Assert.That(configuration.Rendering.SortingLayersOrder, Is.EqualTo(new[] {"Layer1", "Layer2", "Layer3"}));
        }

        [Test]
        public void LoadFromFile_ShouldCreateConfigurationWithDefaultValuesWhenFileConfigurationIsEmpty()
        {
            // Arrange
            // Act
            var configuration = Configuration.LoadFromFile(Path.Combine("Configuration", "empty-configuration.json"));

            // Assert
            Assert.That(configuration.Core.AssetsRootDirectoryPath, Is.EqualTo("Assets"));
            Assert.That(configuration.Core.CustomSystemsExecutionOrder, Is.EqualTo(Array.Empty<string>()));
            Assert.That(configuration.Core.FixedUpdatesPerFrameLimit, Is.Zero);
            Assert.That(configuration.Core.FixedUpdatesPerSecond, Is.EqualTo(60));
            Assert.That(configuration.Core.ShowAllEntitiesCount, Is.False);
            Assert.That(configuration.Core.ShowRootEntitiesCount, Is.False);
            Assert.That(configuration.Core.ShowFps, Is.False);
            Assert.That(configuration.Core.ShowFrameTime, Is.False);
            Assert.That(configuration.Core.ShowTotalFrames, Is.False);
            Assert.That(configuration.Core.ShowTotalTime, Is.False);
            Assert.That(configuration.Core.ShowSystemsExecutionTimes, Is.False);
            Assert.That(configuration.Core.StartUpScene, Is.Empty);

            Assert.That(configuration.Rendering.EnableVSync, Is.False);
            Assert.That(configuration.Rendering.SortingLayersOrder, Is.EqualTo(new[] {RenderingConfiguration.DefaultSortingLayerName}));
        }
    }
}