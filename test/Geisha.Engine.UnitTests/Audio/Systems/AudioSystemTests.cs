using System.Diagnostics;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Audio.Systems;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Audio.Systems
{
    [TestFixture]
    public class AudioSystemTests
    {
        private IAudioPlayer _audioPlayer = null!;
        private AudioSystem _audioSystem = null!;

        [SetUp]
        public void SetUp()
        {
            _audioPlayer = Substitute.For<IAudioPlayer>();
            var audioBackend = Substitute.For<IAudioBackend>();
            audioBackend.AudioPlayer.Returns(_audioPlayer);
            _audioSystem = new AudioSystem(audioBackend);
        }

        [Test]
        public void ProcessAudio_ShouldPlaySound_WhenAudioSourceIsNotPlayingYet()
        {
            // Arrange
            var audioSceneBuilder = new AudioSceneBuilder();
            var entity = audioSceneBuilder.AddAudioSource(false);
            var audioSource = entity.GetComponent<AudioSourceComponent>();
            var scene = audioSceneBuilder.Build();

            // Act
            _audioSystem.ProcessAudio();

            // Assert
            Debug.Assert(audioSource.Sound != null, "audioSource.Sound != null");
            _audioPlayer.Received(1).PlayOnce(audioSource.Sound);
            Assert.That(audioSource.IsPlaying, Is.True);
        }

        [Test]
        public void ProcessAudio_ShouldNotPlaySound_WhenAudioSourceIsNotPlayingYetButSoundIsNull()
        {
            // Arrange
            var audioSceneBuilder = new AudioSceneBuilder();
            var entity = audioSceneBuilder.AddAudioSource(false);
            var audioSource = entity.GetComponent<AudioSourceComponent>();
            audioSource.Sound = null;
            var scene = audioSceneBuilder.Build();

            // Act
            _audioSystem.ProcessAudio();

            // Assert
            _audioPlayer.DidNotReceiveWithAnyArgs().PlayOnce(null!);
            Assert.That(audioSource.IsPlaying, Is.False);
        }

        [Test]
        public void ProcessAudio_ShouldNotPlaySound_WhenAudioSourceIsAlreadyPlaying()
        {
            // Arrange
            var audioSceneBuilder = new AudioSceneBuilder();
            var entity = audioSceneBuilder.AddAudioSource(true);
            var audioSource = entity.GetComponent<AudioSourceComponent>();
            var scene = audioSceneBuilder.Build();

            // Act
            _audioSystem.ProcessAudio();

            // Assert
            Debug.Assert(audioSource.Sound != null, "audioSource.Sound != null");
            _audioPlayer.DidNotReceive().PlayOnce(audioSource.Sound);
            Assert.That(audioSource.IsPlaying, Is.True);
        }

        private class AudioSceneBuilder
        {
            private readonly Scene _scene = TestSceneFactory.Create();

            public Entity AddAudioSource(bool isPlaying)
            {
                var entity = _scene.CreateEntity();
                var audioSource = entity.CreateComponent<AudioSourceComponent>();
                audioSource.Sound = Substitute.For<ISound>();
                audioSource.IsPlaying = isPlaying;

                return entity;
            }

            public Scene Build()
            {
                return _scene;
            }
        }
    }
}