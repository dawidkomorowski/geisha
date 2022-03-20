using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Audio.Components
{
    [TestFixture]
    public class AudioSourceComponentSerializationTests : ComponentSerializationTestsBase
    {
        [Test]
        public void SerializeAndDeserialize_WhenSoundIsNotNull()
        {
            // Arrange
            var sound = Substitute.For<ISound>();
            var soundAssetId = AssetId.CreateUnique();

            AssetStore.GetAssetId(sound).Returns(soundAssetId);
            AssetStore.GetAsset<ISound>(soundAssetId).Returns(sound);

            // Act
            var actual = SerializeAndDeserialize<AudioSourceComponent>(component =>
            {
                component.Sound = sound;
                component.IsPlaying = true;
            });

            // Assert
            Assert.That(actual.Sound, Is.EqualTo(sound));
            Assert.That(actual.IsPlaying, Is.True);
        }

        [Test]
        public void SerializeAndDeserialize_WhenSoundIsNull()
        {
            // Arrange
            // Act
            var actual = SerializeAndDeserialize<AudioSourceComponent>(component =>
            {
                component.Sound = null;
                component.IsPlaying = true;
            });

            // Assert
            Assert.That(actual.Sound, Is.Null);
            Assert.That(actual.IsPlaying, Is.True);
        }
    }
}