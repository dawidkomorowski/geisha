﻿using System;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Audio.Components.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Framework.Audio;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Audio.UnitTests.Components.Serialization
{
    [TestFixture]
    public class SerializableAudioSourceComponentMapperTests
    {
        private IAssetStore _assetStore;
        private SerializableAudioSourceComponentMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _mapper = new SerializableAudioSourceComponentMapper(_assetStore);
        }

        [Test]
        public void MapToSerializable()
        {
            // Arrange
            var sound = Substitute.For<ISound>();
            var soundAssetId = Guid.NewGuid();

            var audioSource = new AudioSourceComponent
            {
                Sound = sound,
                IsPlaying = true
            };

            _assetStore.GetAssetId(sound).Returns(new AssetId(soundAssetId));

            // Act
            var actual = (SerializableAudioSourceComponent) _mapper.MapToSerializable(audioSource);

            // Assert
            Assert.That(actual.SoundAssetId, Is.EqualTo(soundAssetId));
            Assert.That(actual.IsPlaying, Is.True);
        }

        [Test]
        public void MapFromSerializable()
        {
            // Arrange
            var sound = Substitute.For<ISound>();
            var soundAssetId = Guid.NewGuid();

            var serializableAudioSourceComponent = new SerializableAudioSourceComponent
            {
                SoundAssetId = soundAssetId,
                IsPlaying = true
            };

            _assetStore.GetAsset<ISound>(new AssetId(soundAssetId)).Returns(sound);

            // Act
            var actual = (AudioSourceComponent) _mapper.MapFromSerializable(serializableAudioSourceComponent);

            // Assert
            Assert.That(actual.Sound, Is.EqualTo(sound));
            Assert.That(actual.IsPlaying, Is.True);
        }
    }
}