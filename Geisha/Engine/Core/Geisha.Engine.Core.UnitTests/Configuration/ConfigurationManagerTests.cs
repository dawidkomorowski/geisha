using System;
using System.Collections.Generic;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Configuration;
using Geisha.Framework.FileSystem;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Configuration
{
    [TestFixture]
    public class ConfigurationManagerTests
    {
        private IList<IDefaultConfigurationFactory> _defaultConfigurationFactories;
        private IFileSystem _fileSystem;
        private ConfigurationManager _configurationManager;

        [SetUp]
        public void SetUp()
        {
            _defaultConfigurationFactories = new List<IDefaultConfigurationFactory>();
            _fileSystem = Substitute.For<IFileSystem>();
            _configurationManager = new ConfigurationManager(_defaultConfigurationFactories, _fileSystem);
        }

        [Test]
        public void GetConfiguration_ShouldReturnConfigurationFromFile_WhenItExists()
        {
            // Arrange
            var configuration = new TestConfiguration {TestData = "Custom"};
            var gameConfigurationFile = new GameConfigurationFile {Configurations = new List<IConfiguration> {configuration}};
            var json = Serializer.SerializeJson(gameConfigurationFile);

            _defaultConfigurationFactories.Add(new TestConfigurationDefaultConfigurationFactory());
            _fileSystem.ReadAllTextFromFile("game.json").Returns(json);

            // Act
            var actual = _configurationManager.GetConfiguration<TestConfiguration>();

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.TestData, Is.EqualTo(configuration.TestData));
        }

        [Test]
        public void GetConfiguration_ShouldReturnDefaultConfiguration_WhenConfigurationFromFileDoesNotExist()
        {
            // Arrange
            var systemsConfigurations = new GameConfigurationFile {Configurations = new List<IConfiguration>()};
            var json = Serializer.SerializeJson(systemsConfigurations);

            _defaultConfigurationFactories.Add(new TestConfigurationDefaultConfigurationFactory());
            _fileSystem.ReadAllTextFromFile("game.json").Returns(json);

            // Act
            var actual = _configurationManager.GetConfiguration<TestConfiguration>();

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.TestData, Is.EqualTo("Default"));
        }

        [Test]
        public void GetConfiguration_ShouldThrow_GeishaEngineException_WhenThereIsNoDefaultConfigurationFactoryForGivenConfigurationType()
        {
            // Arrange
            var systemsConfigurations = new GameConfigurationFile {Configurations = new List<IConfiguration>()};
            var json = Serializer.SerializeJson(systemsConfigurations);

            _fileSystem.ReadAllTextFromFile("game.json").Returns(json);

            // Act
            // Assert
            var expectedMessage =
                $"No registered implementation of {nameof(IDefaultConfigurationFactory)} exists for configuration type: {typeof(TestConfiguration).Name}.";
            Assert.That(() => _configurationManager.GetConfiguration<TestConfiguration>(),
                Throws.TypeOf<GeishaEngineException>().With.Message.EqualTo(expectedMessage));
        }

        private class TestConfiguration : IConfiguration
        {
            public string TestData { get; set; }
        }

        private class TestConfigurationDefaultConfigurationFactory : IDefaultConfigurationFactory
        {
            public Type ConfigurationType => typeof(TestConfiguration);

            public IConfiguration CreateDefault()
            {
                return new TestConfiguration {TestData = "Default"};
            }
        }
    }
}