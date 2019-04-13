﻿using System.IO;
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
        private SoundManagedAsset _soundManagedAsset;

        private ISound _sound;

        [SetUp]
        public void SetUp()
        {
            _audioProvider = Substitute.For<IAudioProvider>();
            _fileSystem = Substitute.For<IFileSystem>();
            _jsonSerializer = Substitute.For<IJsonSerializer>();

            const string actualSoundFilePath = "sound.wav";
            const SoundFormat soundFormat = SoundFormat.Wav;

            _sound = Substitute.For<ISound>();
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
            _audioProvider.CreateSound(stream, soundFormat).Returns(_sound);

            var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(ISound), soundFilePath);
            _soundManagedAsset = new SoundManagedAsset(assetInfo, _audioProvider, _fileSystem, _jsonSerializer);
        }

        [Test]
        public void Load_ShouldLoadSoundFromFile()
        {
            // Arrange
            // Act
            _soundManagedAsset.Load();
            var actual = (ISound) _soundManagedAsset.AssetInstance;

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