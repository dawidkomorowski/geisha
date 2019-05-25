using NUnit.Framework;

namespace Geisha.Framework.Audio.UnitTests
{
    [TestFixture]
    public class SoundFormatParserTests
    {
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