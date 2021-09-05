using System.IO;
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

            const string soundFilePath = "sound.wav";
            const SoundFormat soundFormat = SoundFormat.Wav;

            const string assetFilePath = "sound-asset-path";
            var soundAssetContent = new SoundAssetContent
            {
                SoundFilePath = soundFilePath
            };

            var assetInfo = new AssetInfo(AssetId.CreateUnique(), AudioAssetTypes.Sound, assetFilePath);
            var assetData = AssetData.CreateWithJsonContent(assetInfo.AssetId, assetInfo.AssetType, soundAssetContent);
            var memoryStream = new MemoryStream();
            assetData.Save(memoryStream);
            memoryStream.Position = 0;

            var assetFile = Substitute.For<IFile>();
            assetFile.OpenRead().Returns(memoryStream);
            _fileSystem.GetFile(assetFilePath).Returns(assetFile);

            var soundFile = Substitute.For<IFile>();
            var stream = Substitute.For<Stream>();
            soundFile.OpenRead().Returns(stream);
            _fileSystem.GetFile(soundFilePath).Returns(soundFile);

            _sound = Substitute.For<ISound>();
            _audioBackend.CreateSound(stream, soundFormat).Returns(_sound);

            _soundManagedAsset = new SoundManagedAsset(assetInfo, _audioBackend, _fileSystem);
        }

        [Test]
        public void Load_ShouldLoadSoundFromFile()
        {
            // Arrange
            // Act
            _soundManagedAsset.Load();
            var actual = (ISound?)_soundManagedAsset.AssetInstance;

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