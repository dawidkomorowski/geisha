using NUnit.Framework;

namespace Geisha.Tools.UnitTests
{
    [TestFixture]
    public class AssetToolTests
    {
        [TestCase(".unsupported", false)]
        [TestCase(".wav", true)]
        [TestCase(".mp3", true)]
        public void IsSupportedSoundFileFormat_ShouldReturnTrue_WhenFileExtensionIsOfSupportedSoundFileFormat(string fileExtension, bool expectedIsSupported)
        {
            // Arrange
            // Act
            var isSupported = AssetTool.IsSupportedSoundFileFormat(fileExtension);

            // Assert
            Assert.That(isSupported, Is.EqualTo(expectedIsSupported));
        }

        [TestCase(".unsupported", false)]
        [TestCase(".bmp", true)]
        [TestCase(".png", true)]
        [TestCase(".jpg", true)]
        public void IsSupportedTextureFileFormat_ShouldReturnTrue_WhenFileExtensionIsOfSupportedTextureFileFormat(string fileExtension,
            bool expectedIsSupported)
        {
            // Arrange
            // Act
            var isSupported = AssetTool.IsSupportedTextureFileFormat(fileExtension);

            // Assert
            Assert.That(isSupported, Is.EqualTo(expectedIsSupported));
        }
    }
}