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

        private class TestAssetLoader : AssetLoaderAdapter<object>
        {
            public override object Load(string filePath)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}