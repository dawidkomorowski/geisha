using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Configuration;
using Geisha.Framework.FileSystem;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Assets
{
    [TestFixture]
    public class AssetsDiscoveryEngineTests
    {
        private IConfigurationManager _configurationManager;
        private IFileSystem _fileSystem;
        private AssetsDiscoveryEngine _assetsDiscoveryEngine;

        [SetUp]
        public void SetUp()
        {
            _configurationManager = Substitute.For<IConfigurationManager>();
            _fileSystem = Substitute.For<IFileSystem>();
            _assetsDiscoveryEngine = new AssetsDiscoveryEngine(_configurationManager, _fileSystem);

            _configurationManager.GetConfiguration<CoreConfiguration>().Returns(new CoreConfiguration());
        }

        [Test]
        public void DiscoverAssets_ShouldSearchInDirectorySpecifiedByConfiguration()
        {
            // Arrange
            const string assetsRootDirectory = "Assets Root Directory";
            _configurationManager.GetConfiguration<CoreConfiguration>().Returns(new CoreConfiguration
            {
                AssetsRootDirectoryPath = assetsRootDirectory
            });

            // Act
            _assetsDiscoveryEngine.DiscoverAssets();

            // Assert
            _fileSystem.Received().GetDirectory(assetsRootDirectory);
        }

        [Test]
        public void DiscoverAssets_ShouldReturnNoAssetInfos_WhenDirectoryHasNoFiles()
        {
            // Arrange
            var root = new DirectoryStub();

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            // Act
            var actual = _assetsDiscoveryEngine.DiscoverAssets();

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void DiscoverAssets_ShouldReturnThreeAssetInfos_WhenDirectoryHasThreeFiles()
        {
            // Arrange
            var root = new DirectoryStub
            {
                Files = new[]
                {
                    new FileStub(),
                    new FileStub(),
                    new FileStub()
                }
            };

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            // Act
            var actual = _assetsDiscoveryEngine.DiscoverAssets();

            // Assert
            Assert.That(actual, Has.Exactly(3).Items);
        }

        private class DirectoryStub : IDirectory
        {
            public string Name { get; set; }
            public IEnumerable<IFile> Files { get; set; } = Enumerable.Empty<IFile>();
        }

        private class FileStub : IFile
        {
            public string Name { get; set; }
        }
    }
}