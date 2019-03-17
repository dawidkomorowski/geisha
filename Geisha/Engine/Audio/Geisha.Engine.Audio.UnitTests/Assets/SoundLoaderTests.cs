using System.IO;
using Geisha.Common.Serialization;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Core;
using Geisha.Framework.Audio;
using Geisha.Framework.FileSystem;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Audio.UnitTests.Assets
{
    [TestFixture]
    public class SoundLoaderTests
    {
        private IAudioProvider _audioProvider;
        private IFileSystem _fileSystem;
        private IJsonSerializer _jsonSerializer;
        private SoundLoader _soundLoader;

        [SetUp]
        public void SetUp()
        {
            _audioProvider = Substitute.For<IAudioProvider>();
            _fileSystem = Substitute.For<IFileSystem>();
            _jsonSerializer = Substitute.For<IJsonSerializer>();
            _soundLoader = new SoundLoader(_audioProvider, _fileSystem, _jsonSerializer);
        }

        [TestCase("sound.wav", SoundFormat.Wav)]
        [TestCase("sound.mp3", SoundFormat.Mp3)]
        public void Load_ShouldLoadSoundFromFile(string actualSoundFilePath, SoundFormat soundFormat)
        {
            // Arrange
            var sound = Substitute.For<ISound>();
            var stream = Substitute.For<Stream>();

            const string soundFilePath = "sound.sound";
            var soundFile = Substitute.For<IFile>();
            const string soundFileContentJson = "sound file content json";
            soundFile.ReadAllText().Returns(soundFileContentJson);
            _fileSystem.GetFile(soundFilePath).Returns(soundFile);
            _jsonSerializer.Deserialize<SoundFileContent>(soundFileContentJson).Returns(new SoundFileContent
            {
                SoundFilePath = actualSoundFilePath
            });

            var actualSoundFile = Substitute.For<IFile>();
            actualSoundFile.OpenRead().Returns(stream);
            _fileSystem.GetFile(actualSoundFilePath).Returns(actualSoundFile);
            _audioProvider.CreateSound(stream, soundFormat).Returns(sound);

            // Act
            var actual = (ISound) _soundLoader.Load(soundFilePath);

            // Assert
            Assert.That(actual, Is.EqualTo(sound));
        }

        [Test]
        public void Load_ShouldThrowException_GivenUnsupportedSoundFile()
        {
            // Arrange
            const string actualSoundFilePath = "sound.unsupported";
            var stream = Substitute.For<Stream>();

            const string soundFilePath = "sound.sound";
            var soundFile = Substitute.For<IFile>();
            const string soundFileContentJson = "sound file content json";
            soundFile.ReadAllText().Returns(soundFileContentJson);
            _fileSystem.GetFile(soundFilePath).Returns(soundFile);
            _jsonSerializer.Deserialize<SoundFileContent>(soundFileContentJson).Returns(new SoundFileContent
            {
                SoundFilePath = actualSoundFilePath
            });

            var file = Substitute.For<IFile>();
            file.OpenRead().Returns(stream);
            _fileSystem.GetFile(actualSoundFilePath).Returns(file);

            // Act
            // Assert
            Assert.That(() => _soundLoader.Load(soundFilePath),
                Throws.TypeOf<GeishaEngineException>().With.Message.Contain("Unsupported sound file format:"));
        }
    }
}