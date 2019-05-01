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
        private IFile _file;
        private IFileSystem _fileSystem;
        private IJsonSerializer _jsonSerializer;
        private ConfigurationManager _configurationManager;

        [SetUp]
        public void SetUp()
        {
            _defaultConfigurationFactories = new List<IDefaultConfigurationFactory>();
            _file = Substitute.For<IFile>();
            _fileSystem = Substitute.For<IFileSystem>();
            _fileSystem.GetFile("game.json").Returns(_file);
            _jsonSerializer = Substitute.For<IJsonSerializer>();
            _configurationManager = new ConfigurationManager(_defaultConfigurationFactories, _fileSystem, _jsonSerializer);
        }

        [Test]
        public void GetConfiguration_ShouldReturnConfigurationFromFile_WhenItExists()
        {
            // Arrange
            const string json = "serialized data";
            var configuration = new TestConfiguration {TestData = "Custom"};
            var gameConfigurationFile = new GameConfigurationFile {Configurations = new List<IConfiguration> {configuration}};

            _defaultConfigurationFactories.Add(new TestConfigurationDefaultConfigurationFactory());
            _file.ReadAllText().Returns(json);
            _jsonSerializer.Deserialize<GameConfigurationFile>(json).Returns(gameConfigurationFile);

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
            const string json = "serialized data";
            var gameConfigurationFile = new GameConfigurationFile {Configurations = new List<IConfiguration>()};

            _defaultConfigurationFactories.Add(new TestConfigurationDefaultConfigurationFactory());
            _file.ReadAllText().Returns(json);
            _jsonSerializer.Deserialize<GameConfigurationFile>(json).Returns(gameConfigurationFile);

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
            const string json = "serialized data";
            var gameConfigurationFile = new GameConfigurationFile {Configurations = new List<IConfiguration>()};

            _file.ReadAllText().Returns(json);
            _jsonSerializer.Deserialize<GameConfigurationFile>(json).Returns(gameConfigurationFile);

            // Act
            // Assert
            var expectedMessage =
                $"No registered implementation of {nameof(IDefaultConfigurationFactory)} exists for configuration type: {typeof(TestConfiguration).Name}.";
            Assert.That(() => _configurationManager.GetConfiguration<TestConfiguration>(),
                Throws.TypeOf<GeishaEngineException>().With.Message.EqualTo(expectedMessage));
        }

        private class TestConfiguration : IConfiguration
        {
            public string TestData { get; set; } = "Default";
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