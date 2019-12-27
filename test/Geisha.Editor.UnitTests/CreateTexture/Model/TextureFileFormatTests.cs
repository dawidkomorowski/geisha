using Geisha.Editor.CreateTexture.Model;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateTexture.Model
{
    [TestFixture]
    public class TextureFileFormatTests
    {
        [TestCase(".txt", false)]
        [TestCase(".bmp", true)]
        [TestCase(".png", true)]
        [TestCase(".jpg", true)]
        public void IsSupported_ShouldReturnTrue_WhenFileExtensionIsOfSupportedTextureFileFormat(string fileExtension, bool expectedIsSupported)
        {
            // Arrange
            // Act
            var isSupported = TextureFileFormat.IsSupported(fileExtension);

            // Assert
            Assert.That(isSupported, Is.EqualTo(expectedIsSupported));
        }
    }
}