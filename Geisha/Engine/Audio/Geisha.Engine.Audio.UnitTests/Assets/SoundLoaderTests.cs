using System.IO;
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
        private SoundLoader _soundLoader;

        [SetUp]
        public void SetUp()
        {
            _audioProvider = Substitute.For<IAudioProvider>();
            _fileSystem = Substitute.For<IFileSystem>();
            _soundLoader = new SoundLoader(_audioProvider, _fileSystem);
        }

        [TestCase("sound.wav", SoundFormat.Wav)]
        [TestCase("sound.mp3", SoundFormat.Mp3)]
        public void Load_ShouldLoadSoundFromFile(string filePath, SoundFormat soundFormat)
        {
            // Arrange
            var sound = Substitute.For<ISound>();
            var stream = Substitute.For<Stream>();

            var file = Substitute.For<IFile>();
            file.OpenRead().Returns(stream);
            _fileSystem.GetFile(filePath).Returns(file);
            _audioProvider.CreateSound(stream, soundFormat).Returns(sound);

            // Act
            var actual = (ISound) _soundLoader.Load(filePath);

            // Assert
            Assert.That(actual, Is.EqualTo(sound));
        }

        [Test]
        public void Load_ShouldThrowException_GivenUnsupportedSoundFile()
        {
            // Arrange
            const string filePath = "sound.unsupported";
            var sound = Substitute.For<ISound>();
            var stream = Substitute.For<Stream>();

            var file = Substitute.For<IFile>();
            file.OpenRead().Returns(stream);
            _fileSystem.GetFile(filePath).Returns(file);

            // Act
            // Assert
            Assert.That(() => _soundLoader.Load(filePath), Throws.TypeOf<GeishaEngineException>().With.Message.Contain("Unsupported sound file format:"));
        }
    }
}