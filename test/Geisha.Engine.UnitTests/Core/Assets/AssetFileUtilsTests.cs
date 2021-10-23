using Geisha.Engine.Core.Assets;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets
{
    [TestFixture]
    public class AssetFileUtilsTests
    {
        [Test]
        public void AppendExtension_ShouldAppendAssetExtensionToGivenPath()
        {
            // Arrange
            const string filePath = @"SomeDirectory\SomeFile";

            // Act
            var actual = AssetFileUtils.AppendExtension(filePath);

            // Assert
            Assert.That(actual, Is.EqualTo($"{filePath}{AssetFileUtils.Extension}"));
        }

        [TestCase(@"Assets\asset.geisha-asset", true)]
        [TestCase(@"Assets\asset.ext", false)]
        public void IsFileAsset_ShouldReturnTrue_GivenFilePathWithAssetExtension(string filePath, bool expected)
        {
            // Arrange
            // Act
            // Assert
            Assert.That(AssetFileUtils.IsAssetFile(filePath), Is.EqualTo(expected));
        }
    }
}