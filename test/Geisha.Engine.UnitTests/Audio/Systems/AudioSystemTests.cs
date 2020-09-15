using System.Diagnostics;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Audio.Systems;
using Geisha.Engine.Core.SceneModel;
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
            _audioSystem.ProcessAudio(scene);

            // Assert
            Debug.Assert(audioSource.Sound != null, "audioSource.Sound != null");
            _audioPlayer.Received(1).PlayOnce(audioSource.Sound);
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
            _audioSystem.ProcessAudio(scene);

            // Assert
            Debug.Assert(audioSource.Sound != null, "audioSource.Sound != null");
            _audioPlayer.DidNotReceive().PlayOnce(audioSource.Sound);
        }

        [Test]
        public void ProcessAudio_ShouldChangeAudioSourceStateToPlaying_WhenAudioSourceIsNotPlayingYet()
        {
            // Arrange
            var audioSceneBuilder = new AudioSceneBuilder();
            var entity = audioSceneBuilder.AddAudioSource(false);
            var audioSource = entity.GetComponent<AudioSourceComponent>();
            var scene = audioSceneBuilder.Build();

            // Act
            _audioSystem.ProcessAudio(scene);

            // Assert
            Assert.That(audioSource.IsPlaying, Is.True);
        }

        [Test]
        public void ProcessAudio_ShouldLeaveAudioSourceStateAsPlaying_WhenAudioSourceIsAlreadyPlaying()
        {
            // Arrange
            var audioSceneBuilder = new AudioSceneBuilder();
            var entity = audioSceneBuilder.AddAudioSource(true);
            var audioSource = entity.GetComponent<AudioSourceComponent>();
            var scene = audioSceneBuilder.Build();

            // Act
            _audioSystem.ProcessAudio(scene);

            // Assert
            Assert.That(audioSource.IsPlaying, Is.True);
        }

        private class AudioSceneBuilder
        {
            private readonly Scene _scene = new Scene();

            public Entity AddAudioSource(bool isPlaying)
            {
                var entity = new Entity();
                entity.AddComponent(new AudioSourceComponent
                {
                    Sound = Substitute.For<ISound>(),
                    IsPlaying = isPlaying
                });

                _scene.AddEntity(entity);

                return entity;
            }

            public Scene Build()
            {
                return _scene;
            }
        }
    }
}