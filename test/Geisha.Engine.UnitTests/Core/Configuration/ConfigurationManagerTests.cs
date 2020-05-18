using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Configuration
{
    [TestFixture]
    public class ConfigurationManagerTests
    {
        private IFile _file = null!;
        private IFileSystem _fileSystem = null!;
        private IJsonSerializer _jsonSerializer = null!;

        [SetUp]
        public void SetUp()
        {
            _file = Substitute.For<IFile>();
            _fileSystem = Substitute.For<IFileSystem>();
            _fileSystem.GetFile("game.json").Returns(_file);
            _jsonSerializer = Substitute.For<IJsonSerializer>();
        }

        [Test]
        public void GetConfiguration_ShouldReturnConfigurationFromFile_WhenItExists()
        {
            // Arrange
            const string json = "serialized data";
            var configuration = new TestConfiguration {TestData = "Custom"};

            _file.ReadAllText().Returns(json);
            _jsonSerializer.DeserializePart<TestConfiguration>(json, "TestConfiguration").Returns(configuration);

            // Act
            var configurationManager = new ConfigurationManager(_fileSystem, _jsonSerializer);
            var actual = configurationManager.GetConfiguration<TestConfiguration>();

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.TestData, Is.EqualTo(configuration.TestData));
        }

        [Test]
        public void GetConfiguration_ShouldReturnDefaultConfiguration_WhenConfigurationFromFileDoesNotExist()
        {
            // Arrange
            const string json = "serialized data";

            _file.ReadAllText().Returns(json);
            _jsonSerializer.DeserializePart<TestConfiguration?>(json, "TestConfiguration").Returns((TestConfiguration?) null);

            // Act
            var configurationManager = new ConfigurationManager(_fileSystem, _jsonSerializer);
            var actual = configurationManager.GetConfiguration<TestConfiguration>();

            // Assert
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.TestData, Is.EqualTo("Default"));
        }

        private class TestConfiguration
        {
            public string TestData { get; set; } = "Default";
        }
    }
}