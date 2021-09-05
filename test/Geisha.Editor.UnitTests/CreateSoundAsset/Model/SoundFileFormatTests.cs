using Geisha.Editor.CreateSoundAsset.Model;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateSoundAsset.Model
{
    [TestFixture]
    public class SoundFileFormatTests
    {
        [TestCase(".txt", false)]
        [TestCase(".wav", true)]
        [TestCase(".mp3", true)]
        public void IsSupported_ShouldReturnTrue_WhenFileExtensionIsOfSupportedSoundFileFormat(string fileExtension, bool expectedIsSupported)
        {
            // Arrange
            // Act
            var isSupported = SoundFileFormat.IsSupported(fileExtension);

            // Assert
            Assert.That(isSupported, Is.EqualTo(expectedIsSupported));
        }
    }
}