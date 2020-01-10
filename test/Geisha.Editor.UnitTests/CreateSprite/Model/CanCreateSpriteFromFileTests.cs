using Geisha.Editor.CreateSprite.Model;
using Geisha.Engine.Rendering.Assets;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateSprite.Model
{
    [TestFixture]
    public class CanCreateSpriteFromFileTests
    {
        [TestCase(".txt", false)]
        [TestCase(RenderingFileExtensions.Texture, true)]
        public void Check_ShouldReturnTrue_WhenFileExtensionIsOfTextureAssetFile(string fileExtension, bool expectedCanCreateSprite)
        {
            // Arrange
            // Act
            var canCreateSprite = CanCreateSpriteFromFile.Check(fileExtension);

            // Assert
            Assert.That(canCreateSprite, Is.EqualTo(expectedCanCreateSprite));
        }
    }
}