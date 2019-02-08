using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Common;
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

        [SetUp]
        public void SetUp()
        {
            _configurationManager = Substitute.For<IConfigurationManager>();
            _configurationManager.GetConfiguration<CoreConfiguration>().Returns(new CoreConfiguration());

            _fileSystem = Substitute.For<IFileSystem>();
        }

        private AssetsDiscoveryEngine GetAssetDiscoveryEngine(IEnumerable<IAssetDiscoveryRule> assetDiscoveryRules)
        {
            return new AssetsDiscoveryEngine(_configurationManager, _fileSystem, assetDiscoveryRules);
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
            GetAssetDiscoveryEngine(Enumerable.Empty<IAssetDiscoveryRule>()).DiscoverAssets();

            // Assert
            _fileSystem.Received().GetDirectory(assetsRootDirectory);
        }

        [Test]
        public void DiscoverAssets_ShouldReturnNoAssetInfos_WhenDirectoryHasNoFiles()
        {
            // Arrange
            var root = new DirectoryStub();

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            var allFilesAreAssetsDiscoveryRule = Substitute.For<IAssetDiscoveryRule>();
            allFilesAreAssetsDiscoveryRule.Discover(Arg.Any<IFile>()).Returns(SingleOrEmpty.Single(new AssetInfo(new AssetId(Guid.NewGuid()), null, null)));

            // Act
            var actual = GetAssetDiscoveryEngine(new[] {allFilesAreAssetsDiscoveryRule}).DiscoverAssets();

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void DiscoverAssets_ShouldReturnNoAssetInfos_WhenThereAreNoDiscoveryRules()
        {
            // Arrange
            var root = new DirectoryStub
            {
                Files = new[]
                {
                    Substitute.For<IFile>(),
                    Substitute.For<IFile>(),
                    Substitute.For<IFile>()
                }
            };

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            // Act
            var actual = GetAssetDiscoveryEngine(Enumerable.Empty<IAssetDiscoveryRule>()).DiscoverAssets();

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void DiscoverAssets_ShouldReturnThreeAssetInfos_WhenDirectoryHasThreeFiles_AndThereIsMatchingRule()
        {
            // Arrange
            var root = new DirectoryStub
            {
                Files = new[]
                {
                    Substitute.For<IFile>(),
                    Substitute.For<IFile>(),
                    Substitute.For<IFile>()
                }
            };

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            var allFilesAreAssetsDiscoveryRule = Substitute.For<IAssetDiscoveryRule>();
            allFilesAreAssetsDiscoveryRule.Discover(Arg.Any<IFile>()).Returns(SingleOrEmpty.Single(new AssetInfo(new AssetId(Guid.NewGuid()), null, null)));

            // Act
            var actual = GetAssetDiscoveryEngine(new[] {allFilesAreAssetsDiscoveryRule}).DiscoverAssets();

            // Assert
            Assert.That(actual, Has.Exactly(3).Items);
        }

        [Test]
        public void DiscoverAssets_ShouldReturnOneAssetInfo_WhenDirectoryHasThreeFiles_AndOnlyOneMatchesDiscoveryRule()
        {
            // Arrange
            var matchingFile = Substitute.For<IFile>();
            var assetInfo = new AssetInfo(new AssetId(Guid.NewGuid()), null, null);

            var root = new DirectoryStub
            {
                Files = new[]
                {
                    Substitute.For<IFile>(),
                    Substitute.For<IFile>(),
                    matchingFile
                }
            };

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            var singleFileIsAssetDiscoveryRule = Substitute.For<IAssetDiscoveryRule>();
            singleFileIsAssetDiscoveryRule.Discover(Arg.Any<IFile>()).Returns(SingleOrEmpty.Empty<AssetInfo>());
            singleFileIsAssetDiscoveryRule.Discover(matchingFile).Returns(SingleOrEmpty.Single(assetInfo));

            // Act
            var actual = GetAssetDiscoveryEngine(new[] {singleFileIsAssetDiscoveryRule}).DiscoverAssets();

            // Assert
            Assert.That(actual, Has.One.Items);
            Assert.That(actual.Single(), Is.EqualTo(assetInfo));
        }

        [Test]
        public void
            DiscoverAssets_ShouldReturnTwoAssetInfos_WhenDirectoryHasThreeFilesAndTwoDiscoveryRules_AndFirstFileMatchesFirstRule_AndSecondFileMatchesSecondRule()
        {
            // Arrange
            var file1 = Substitute.For<IFile>();
            var file2 = Substitute.For<IFile>();
            var assetInfo1 = new AssetInfo(new AssetId(Guid.NewGuid()), null, null);
            var assetInfo2 = new AssetInfo(new AssetId(Guid.NewGuid()), null, null);

            var root = new DirectoryStub
            {
                Files = new[]
                {
                    Substitute.For<IFile>(),
                    file1,
                    file2
                }
            };

            _fileSystem.GetDirectory(Arg.Any<string>()).Returns(root);

            var assetDiscoveryRule1 = Substitute.For<IAssetDiscoveryRule>();
            assetDiscoveryRule1.Discover(Arg.Any<IFile>()).Returns(SingleOrEmpty.Empty<AssetInfo>());
            assetDiscoveryRule1.Discover(file1).Returns(SingleOrEmpty.Single(assetInfo1));

            var assetDiscoveryRule2 = Substitute.For<IAssetDiscoveryRule>();
            assetDiscoveryRule2.Discover(Arg.Any<IFile>()).Returns(SingleOrEmpty.Empty<AssetInfo>());
            assetDiscoveryRule2.Discover(file2).Returns(SingleOrEmpty.Single(assetInfo2));

            // Act
            var actual = GetAssetDiscoveryEngine(new[] {assetDiscoveryRule1, assetDiscoveryRule2}).DiscoverAssets();

            // Assert
            Assert.That(actual, Has.Exactly(2).Items);
            Assert.That(actual, Contains.Item(assetInfo1));
            Assert.That(actual, Contains.Item(assetInfo2));
        }

        private class DirectoryStub : IDirectory
        {
            public string Name { get; set; }
            public IEnumerable<IFile> Files { get; set; } = Enumerable.Empty<IFile>();
        }
    }
}