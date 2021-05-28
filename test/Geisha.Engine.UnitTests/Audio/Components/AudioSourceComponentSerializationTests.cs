using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Audio.Components
{
    [TestFixture]
    public class AudioSourceComponentSerializationTests : ComponentSerializationTestsBase
    {
        protected override IComponentFactory ComponentFactory => new AudioSourceComponentFactory();

        [Test]
        public void SerializeAndDeserialize_WhenSoundIsNotNull()
        {
            // Arrange
            var sound = Substitute.For<ISound>();
            var soundAssetId = AssetId.CreateUnique();

            var component = new AudioSourceComponent
            {
                Sound = sound,
                IsPlaying = true
            };

            AssetStore.GetAssetId(sound).Returns(soundAssetId);
            AssetStore.GetAsset<ISound>(soundAssetId).Returns(sound);

            // Act
            var actual = SerializeAndDeserialize(component);

            // Assert
            Assert.That(actual.Sound, Is.EqualTo(sound));
            Assert.That(actual.IsPlaying, Is.True);
        }

        [Test]
        public void SerializeAndDeserialize_WhenSoundIsNull()
        {
            // Arrange
            var component = new AudioSourceComponent
            {
                Sound = null,
                IsPlaying = true
            };

            // Act
            var actual = SerializeAndDeserialize(component);

            // Assert
            Assert.That(actual.Sound, Is.Null);
            Assert.That(actual.IsPlaying, Is.True);
        }
    }
}