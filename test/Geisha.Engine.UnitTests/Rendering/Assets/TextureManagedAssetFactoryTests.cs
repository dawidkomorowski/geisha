using System.Linq;
using Geisha.Common.FileSystem;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Backend;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Assets
{
    [TestFixture]
    public class TextureManagedAssetFactoryTests
    {
        private IAssetStore _assetStore = null!;
        private IRenderingBackend _renderingBackend = null!;
        private IFileSystem _fileSystem = null!;
        private TextureManagedAssetFactory _textureManagedAssetFactory = null!;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _renderingBackend = Substitute.For<IRenderingBackend>();
            _fileSystem = Substitute.For<IFileSystem>();
            _textureManagedAssetFactory = new TextureManagedAssetFactory(_renderingBackend, _fileSystem);
        }

        [Test]
        public void Create_ShouldReturnEmpty_GivenAssetInfoWithNotMatchingAssetType()
        {
            Assert.Fail("TODO");
            //// Arrange
            //var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(object), "asset file path");

            //// Act
            //var actual = _textureManagedAssetFactory.Create(assetInfo, _assetStore);

            //// Assert
            //Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Create_ShouldReturnSingleAsset_GivenAssetInfoWithMatchingAssetType()
        {
            Assert.Fail("TODO");
            //// Arrange
            //var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(ITexture), "asset file path");

            //// Act
            //var actual = _textureManagedAssetFactory.Create(assetInfo, _assetStore);

            //// Assert
            //Assert.That(actual, Is.Not.Empty);
            //var managedAsset = actual.Single();
            //Assert.That(managedAsset, Is.TypeOf<TextureManagedAsset>());
            //Assert.That(managedAsset.AssetInfo, Is.EqualTo(assetInfo));
        }
    }
}