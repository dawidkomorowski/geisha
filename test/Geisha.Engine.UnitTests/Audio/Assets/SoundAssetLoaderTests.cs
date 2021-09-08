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
    public class SoundAssetLoaderTests
    {
        private IAudioBackend _audioBackend = null!;
        private IFileSystem _fileSystem = null!;
        private IAssetStore _assetStore = null!;
        private SoundAssetLoader _soundAssetLoader = null!;

        private AssetInfo _assetInfo;
        private ISound _sound = null!;

        [SetUp]
        public void SetUp()
        {
            _audioBackend = Substitute.For<IAudioBackend>();
            _fileSystem = Substitute.For<IFileSystem>();
            _assetStore = Substitute.For<IAssetStore>();

            const string soundFilePath = "sound.wav";
            const SoundFormat soundFormat = SoundFormat.Wav;

            const string assetFilePath = "sound-asset-path";
            var soundAssetContent = new SoundAssetContent
            {
                SoundFilePath = soundFilePath
            };

            _assetInfo = new AssetInfo(AssetId.CreateUnique(), AudioAssetTypes.Sound, assetFilePath);
            var assetData = AssetData.CreateWithJsonContent(_assetInfo.AssetId, _assetInfo.AssetType, soundAssetContent);
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

            _soundAssetLoader = new SoundAssetLoader(_audioBackend, _fileSystem);
        }

        [Test]
        public void LoadAsset_ShouldLoadSoundFromFile()
        {
            // Arrange
            // Act
            var actual = (ISound)_soundAssetLoader.LoadAsset(_assetInfo, _assetStore);

            // Assert
            Assert.That(actual, Is.EqualTo(_sound));
        }

        [Test]
        public void UnloadAsset_ShouldDisposeSound()
        {
            // Arrange
            // Act
            _soundAssetLoader.UnloadAsset(_sound);

            // Assert
            _sound.Received().Dispose();
        }
    }
}