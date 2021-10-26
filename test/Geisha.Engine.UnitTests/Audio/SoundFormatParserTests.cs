using Geisha.Engine.Audio;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Audio
{
    [TestFixture]
    public class SoundFormatParserTests
    {
        [TestCase(".unsupported", false)]
        [TestCase(".wav", true)]
        [TestCase(".mp3", true)]
        public void IsSupportedFileExtension_ShouldReturnTrue_WhenFileExtensionIsOfSupportedSoundFormat(string fileExtension, bool expectedIsSupported)
        {
            // Arrange
            // Act
            var isSupported = SoundFormatParser.IsSupportedFileExtension(fileExtension);

            // Assert
            Assert.That(isSupported, Is.EqualTo(expectedIsSupported));
        }

        [TestCase(".wav", SoundFormat.Wav)]
        [TestCase(".mp3", SoundFormat.Mp3)]
        public void ParseFromFileExtension_ShouldMapStringFileExtensionToEnumSoundFormat(string fileExtension, SoundFormat soundFormat)
        {
            // Arrange
            // Act
            var actual = SoundFormatParser.ParseFromFileExtension(fileExtension);

            // Assert
            Assert.That(actual, Is.EqualTo(soundFormat));
        }

        [Test]
        public void ParseFromFileExtension_ShouldThrowException_GivenUnsupportedFileFormatExtension()
        {
            // Arrange
            const string fileExtension = ".unsupported";

            // Act
            // Assert
            Assert.That(() => SoundFormatParser.ParseFromFileExtension(fileExtension), Throws.TypeOf<UnsupportedSoundFileFormatException>());
        }
    }
}