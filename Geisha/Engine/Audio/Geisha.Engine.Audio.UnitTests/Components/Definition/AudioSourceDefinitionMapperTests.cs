using System;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Audio.Components.Definition;
using Geisha.Engine.Core.Assets;
using Geisha.Framework.Audio;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Audio.UnitTests.Components.Definition
{
    [TestFixture]
    public class AudioSourceDefinitionMapperTests
    {
        private IAssetStore _assetStore;
        private AudioSourceDefinitionMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _mapper = new AudioSourceDefinitionMapper(_assetStore);
        }

        [Test]
        public void ToDefinition()
        {
            // Arrange
            var sound = Substitute.For<ISound>();
            var soundAssetId = Guid.NewGuid();

            var audioSource = new AudioSourceComponent
            {
                Sound = sound,
                IsPlaying = true
            };

            _assetStore.GetAssetId(sound).Returns(soundAssetId);

            // Act
            var actual = (AudioSourceDefinition) _mapper.MapToSerializable(audioSource);

            // Assert
            Assert.That(actual.SoundAssetId, Is.EqualTo(soundAssetId));
            Assert.That(actual.IsPlaying, Is.True);
        }

        [Test]
        public void FromDefinition()
        {
            // Arrange
            var sound = Substitute.For<ISound>();
            var soundAssetId = Guid.NewGuid();

            var audioSourceDefinition = new AudioSourceDefinition
            {
                SoundAssetId = soundAssetId,
                IsPlaying = true
            };

            _assetStore.GetAsset<ISound>(soundAssetId).Returns(sound);

            // Act
            var actual = (AudioSourceComponent) _mapper.MapFromSerializable(audioSourceDefinition);

            // Assert
            Assert.That(actual.Sound, Is.EqualTo(sound));
            Assert.That(actual.IsPlaying, Is.True);
        }
    }
}