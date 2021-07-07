using System.IO;
using System.Text.Json;
using Geisha.Common.FileSystem;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core.Assets;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Audio.Assets
{
    [TestFixture]
    public class SoundManagedAssetTests
    {
        private IAudioBackend _audioBackend = null!;
        private IFileSystem _fileSystem = null!;
        private SoundManagedAsset _soundManagedAsset = null!;

        private ISound _sound = null!;

        [SetUp]
        public void SetUp()
        {
            _audioBackend = Substitute.For<IAudioBackend>();
            _fileSystem = Substitute.For<IFileSystem>();

            const string actualSoundFilePath = "sound.wav";
            const SoundFormat soundFormat = SoundFormat.Wav;

            _sound = Substitute.For<ISound>();
            var stream = Substitute.For<Stream>();

            var soundFilePath = $"sound{AudioFileExtensions.Sound}";
            var soundFile = Substitute.For<IFile>();
            var soundFileContentJson = JsonSerializer.Serialize(new SoundFileContent
            {
                SoundFilePath = actualSoundFilePath
            });
            soundFile.ReadAllText().Returns(soundFileContentJson);
            _fileSystem.GetFile(soundFilePath).Returns(soundFile);

            var actualSoundFile = Substitute.For<IFile>();
            actualSoundFile.OpenRead().Returns(stream);
            _fileSystem.GetFile(actualSoundFilePath).Returns(actualSoundFile);
            _audioBackend.CreateSound(stream, soundFormat).Returns(_sound);

            Assert.Fail("TODO");
            //var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(ISound), soundFilePath);
            //_soundManagedAsset = new SoundManagedAsset(assetInfo, _audioBackend, _fileSystem);
        }

        [Test]
        public void Load_ShouldLoadSoundFromFile()
        {
            // Arrange
            // Act
            _soundManagedAsset.Load();
            var actual = (ISound?) _soundManagedAsset.AssetInstance;

            // Assert
            Assert.That(actual, Is.EqualTo(_sound));
        }

        [Test]
        public void Unload_ShouldDisposeSound()
        {
            // Arrange
            _soundManagedAsset.Load();

            // Act
            _soundManagedAsset.Unload();

            // Assert
            _sound.Received().Dispose();
        }
    }
}