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
    }
}