using Geisha.Editor.CreateSpriteAsset.Model;
using Geisha.Engine.Rendering.Assets;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateSpriteAsset.Model
{
    [TestFixture]
    public class CreateSpriteAssetUtilsTests
    {
        [TestCase(".txt", false)]
        [TestCase(RenderingFileExtensions.Texture, true)]
        [TestCase(".png", true)]
        public void CanCreateSpriteAssetFromFile_ShouldReturnTrue_WhenFileExtensionIsEitherTextureMetadataFileOrTextureDataFile(string fileExtension,
            bool expectedCanCreateSprite)
        {
            // Arrange
            // Act
            var canCreateSprite = CreateSpriteAssetUtils.CanCreateSpriteAssetFromFile(fileExtension);

            // Assert
            Assert.That(canCreateSprite, Is.EqualTo(expectedCanCreateSprite));
        }

        [TestCase(".txt", false)]
        [TestCase(RenderingFileExtensions.Texture, true)]
        public void IsTextureAssetFile_ShouldReturnTrue_WhenFileExtensionIsTextureMetadataFile(string fileExtension, bool expectedCanCreateSprite)
        {
            // Arrange
            // Act
            var canCreateSprite = CreateSpriteAssetUtils.IsTextureAssetFile(fileExtension);

            // Assert
            Assert.That(canCreateSprite, Is.EqualTo(expectedCanCreateSprite));
        }

        [TestCase(".txt", false)]
        [TestCase(RenderingFileExtensions.Texture, false)]
        [TestCase(".png", true)]
        public void IsTextureFile_ShouldReturnTrue_WhenFileExtensionIsTextureDataFile(string fileExtension, bool expectedCanCreateSprite)
        {
            // Arrange
            // Act
            var canCreateSprite = CreateSpriteAssetUtils.IsTextureFile(fileExtension);

            // Assert
            Assert.That(canCreateSprite, Is.EqualTo(expectedCanCreateSprite));
        }
    }
}