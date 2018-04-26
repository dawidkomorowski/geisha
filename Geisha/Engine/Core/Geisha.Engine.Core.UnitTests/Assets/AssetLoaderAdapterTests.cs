using Geisha.Engine.Core.Assets;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Assets
{
    [TestFixture]
    public class AssetLoaderAdapterTests
    {
        [Test]
        public void AssetType_ShouldReturnAssetTypeGivenAsTypeParameter()
        {
            // Arrange
            var loader = new TestAssetLoader();

            // Act
            var actual = loader.AssetType;

            // Assert
            Assert.That(actual, Is.EqualTo(typeof(object)));
        }

        [Test]
        public void Load_ShouldDelegateToLoadAsset()
        {
            // Arrange
            const string filePath = "Some file path";
            var asset = new object();

            var loader = new TestAssetLoader {Object = asset};

            // Act
            var actual = loader.Load(filePath);

            // Assert
            Assert.That(loader.FilePath, Is.EqualTo(filePath));
            Assert.That(actual, Is.EqualTo(asset));
        }

        private class TestAssetLoader : AssetLoaderAdapter<object>
        {
            public string FilePath { get; set; }
            public object Object { get; set; }

            protected override object LoadAsset(string filePath)
            {
                FilePath = filePath;
                return Object;
            }
        }
    }
}