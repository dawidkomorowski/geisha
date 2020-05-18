﻿using System.IO;
using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
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
        private IJsonSerializer _jsonSerializer = null!;
        private SoundManagedAsset _soundManagedAsset = null!;

        private ISound _sound = null!;

        [SetUp]
        public void SetUp()
        {
            _audioBackend = Substitute.For<IAudioBackend>();
            _fileSystem = Substitute.For<IFileSystem>();
            _jsonSerializer = Substitute.For<IJsonSerializer>();

            const string actualSoundFilePath = "sound.wav";
            const SoundFormat soundFormat = SoundFormat.Wav;

            _sound = Substitute.For<ISound>();
            var stream = Substitute.For<Stream>();

            var soundFilePath = $"sound{AudioFileExtensions.Sound}";
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
            _audioBackend.CreateSound(stream, soundFormat).Returns(_sound);

            var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(ISound), soundFilePath);
            _soundManagedAsset = new SoundManagedAsset(assetInfo, _audioBackend, _fileSystem, _jsonSerializer);
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