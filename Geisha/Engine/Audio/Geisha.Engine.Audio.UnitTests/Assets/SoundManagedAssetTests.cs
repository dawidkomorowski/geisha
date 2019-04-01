using System.IO;
using Geisha.Common.Serialization;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Core.Assets;
using Geisha.Framework.Audio;
using Geisha.Framework.FileSystem;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Audio.UnitTests.Assets
{
    [TestFixture]
    public class SoundManagedAssetTests
    {
        private IAudioProvider _audioProvider;
        private IFileSystem _fileSystem;
        private IJsonSerializer _jsonSerializer;

        [SetUp]
        public void SetUp()
        {
            _audioProvider = Substitute.For<IAudioProvider>();
            _fileSystem = Substitute.For<IFileSystem>();
            _jsonSerializer = Substitute.For<IJsonSerializer>();
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

            var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(ISound), soundFilePath);
            var soundManagedAsset = new SoundManagedAsset(assetInfo, _audioProvider, _fileSystem, _jsonSerializer);

            // Act
            soundManagedAsset.Load();
            var actual = (ISound) soundManagedAsset.AssetInstance;

            // Assert
            Assert.That(actual, Is.EqualTo(sound));
        }

        [Test]
        public void Load_ShouldThrowException_GivenUnsupportedSoundFileFormat()
        {
            // Arrange
            const string actualSoundFilePath = "sound.unsupported";

            const string soundFilePath = "sound.sound";
            var soundFile = Substitute.For<IFile>();
            const string soundFileContentJson = "sound file content json";
            soundFile.ReadAllText().Returns(soundFileContentJson);
            _fileSystem.GetFile(soundFilePath).Returns(soundFile);
            _jsonSerializer.Deserialize<SoundFileContent>(soundFileContentJson).Returns(new SoundFileContent
            {
                SoundFilePath = actualSoundFilePath
            });

            var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(ISound), soundFilePath);
            var soundManagedAsset = new SoundManagedAsset(assetInfo, _audioProvider, _fileSystem, _jsonSerializer);

            // Act
            // Assert
            Assert.That(() => soundManagedAsset.Load(), Throws.TypeOf<UnsupportedSoundFileFormatException>());
        }
    }
}