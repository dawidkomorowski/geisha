using System.IO;
using Geisha.Editor.CreateAsset.Model;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.CreateSpriteAsset.Model
{
    [TestFixture]
    public class CreateSpriteAssetUtilsIntegrationTests
    {
        [TestCase("TestSound.mp3", false)]
        [TestCase("TestSound.geisha-asset", false)]
        [TestCase("TestTexture.geisha-asset", true)]
        [TestCase("TestTexture.png", true)]
        public void CanCreateSpriteAssetFromFile_ShouldReturnTrue_WhenFileIsEitherTextureAssetFileOrTextureFile(string fileName, bool expected)
        {
            // Arrange
            var filePath = Utils.GetPathUnderTestDirectory(Path.Combine("Assets", fileName));

            // Act
            var actual = CreateSpriteAssetUtils.CanCreateSpriteAssetFromFile(filePath);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("TestTexture.png", false)]
        [TestCase("TestSound.geisha-asset", false)]
        [TestCase("TestTexture.geisha-asset", true)]
        public void IsTextureAssetFile_ShouldReturnTrue_WhenFileIsTextureAssetFile(string fileName, bool expected)
        {
            // Arrange
            var filePath = Utils.GetPathUnderTestDirectory(Path.Combine("Assets", fileName));

            // Act
            var actual = CreateSpriteAssetUtils.IsTextureAssetFile(filePath);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("TestSound.mp3", false)]
        [TestCase("TestTexture.geisha-asset", false)]
        [TestCase("TestTexture.png", true)]
        public void IsTextureFile_ShouldReturnTrue_WhenFileExtensionIsTextureFile(string fileName, bool expected)
        {
            // Arrange
            var filePath = Utils.GetPathUnderTestDirectory(Path.Combine("Assets", fileName));

            // Act
            var actual = CreateSpriteAssetUtils.IsTextureFile(filePath);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}