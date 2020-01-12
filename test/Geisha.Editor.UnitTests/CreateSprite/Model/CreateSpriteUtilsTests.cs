using Geisha.Editor.CreateSprite.Model;
using Geisha.Engine.Rendering.Assets;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateSprite.Model
{
    [TestFixture]
    public class CreateSpriteUtilsTests
    {
        [TestCase(".txt", false)]
        [TestCase(RenderingFileExtensions.Texture, true)]
        [TestCase(".png", true)]
        public void CanCreateSpriteFromFile_ShouldReturnTrue_WhenFileExtensionIsEitherTextureMetadataFileOrTextureDataFile(string fileExtension,
            bool expectedCanCreateSprite)
        {
            // Arrange
            // Act
            var canCreateSprite = CreateSpriteUtils.CanCreateSpriteFromFile(fileExtension);

            // Assert
            Assert.That(canCreateSprite, Is.EqualTo(expectedCanCreateSprite));
        }

        [TestCase(".txt", false)]
        [TestCase(RenderingFileExtensions.Texture, true)]
        public void IsTextureMetadataFile_ShouldReturnTrue_WhenFileExtensionIsTextureMetadataFile(string fileExtension, bool expectedCanCreateSprite)
        {
            // Arrange
            // Act
            var canCreateSprite = CreateSpriteUtils.IsTextureMetadataFile(fileExtension);

            // Assert
            Assert.That(canCreateSprite, Is.EqualTo(expectedCanCreateSprite));
        }

        [TestCase(".txt", false)]
        [TestCase(RenderingFileExtensions.Texture, false)]
        [TestCase(".png", true)]
        public void IsTextureDataFile_ShouldReturnTrue_WhenFileExtensionIsTextureDataFile(string fileExtension, bool expectedCanCreateSprite)
        {
            // Arrange
            // Act
            var canCreateSprite = CreateSpriteUtils.IsTextureDataFile(fileExtension);

            // Assert
            Assert.That(canCreateSprite, Is.EqualTo(expectedCanCreateSprite));
        }
    }
}