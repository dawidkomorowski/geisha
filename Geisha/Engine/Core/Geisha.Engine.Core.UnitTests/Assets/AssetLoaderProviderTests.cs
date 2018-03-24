using Geisha.Engine.Core.Assets;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Assets
{
    [TestFixture]
    public class AssetLoaderProviderTests
    {
        [Test]
        public void GetLoaderFor_ShouldReturnLoader_WhenThereIsSingleMatching()
        {
            // Arrange
            var assetType = typeof(object);

            var loader1 = Substitute.For<IAssetLoader>();
            var loader2 = Substitute.For<IAssetLoader>();
            var loader3 = Substitute.For<IAssetLoader>();

            loader2.AssetType.Returns(assetType);

            var provider = new AssetLoaderProvider(new[] {loader1, loader2, loader3});

            // Act
            var actual = provider.GetLoaderFor(assetType);

            // Assert
            Assert.That(actual, Is.EqualTo(loader2));
        }

        [Test]
        public void GetLoaderFor_ShouldThrowException_WhenThereIsNoMatching()
        {
            // Arrange
            var assetType = typeof(object);

            var loader1 = Substitute.For<IAssetLoader>();
            var loader2 = Substitute.For<IAssetLoader>();
            var loader3 = Substitute.For<IAssetLoader>();

            var provider = new AssetLoaderProvider(new[] {loader1, loader2, loader3});

            // Act
            // Assert
            Assert.That(() => provider.GetLoaderFor(assetType), Throws.TypeOf<GeishaEngineException>());
        }

        [Test]
        public void GetLoaderFor_ShouldThrowException_WhenThereAreMultipleMatching()
        {
            // Arrange
            var assetType = typeof(object);

            var loader1 = Substitute.For<IAssetLoader>();
            var loader2 = Substitute.For<IAssetLoader>();
            var loader3 = Substitute.For<IAssetLoader>();

            loader1.AssetType.Returns(assetType);
            loader2.AssetType.Returns(assetType);

            var provider = new AssetLoaderProvider(new[] {loader1, loader2, loader3});

            // Act
            // Assert
            Assert.That(() => provider.GetLoaderFor(assetType), Throws.TypeOf<GeishaEngineException>());
        }
    }
}